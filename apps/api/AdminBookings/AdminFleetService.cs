using System.Data;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using BangaloreTaxi.Api.Application;
using BangaloreTaxi.Api.Auth;
using BangaloreTaxi.Api.Persistence;
using BangaloreTaxi.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BangaloreTaxi.Api.AdminBookings;

public sealed class AdminFleetService(BangaloreTaxiDbContext db, TimeProvider clock)
{
    private const int DefaultPageSize = 25;
    private const int MaxPageSize = 100;
    private static readonly Regex Registration = new("^[A-Z0-9-]{4,16}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Dictionary<string, short> Employment = new(StringComparer.OrdinalIgnoreCase)
    { ["active"] = ReferenceData.DriverEmploymentActive, ["inactive"] = ReferenceData.DriverEmploymentInactive, ["suspended"] = ReferenceData.DriverEmploymentSuspended };
    private static readonly Dictionary<string, short> Availability = new(StringComparer.OrdinalIgnoreCase)
    { ["available"] = ReferenceData.DriverAvailabilityAvailable, ["unavailable"] = ReferenceData.DriverAvailabilityUnavailable, ["on_trip"] = ReferenceData.DriverAvailabilityOnTrip, ["off_duty"] = ReferenceData.DriverAvailabilityOffDuty };
    private static readonly Dictionary<string, short> VehicleStatuses = new(StringComparer.OrdinalIgnoreCase)
    { ["active"] = ReferenceData.VehicleStatusActive, ["inactive"] = ReferenceData.VehicleStatusInactive, ["maintenance"] = ReferenceData.VehicleStatusMaintenance };

    public async Task<AdminDriverPage> ListDriversAsync(bool eligibleOnly, string? search, int page, int pageSize, CancellationToken ct)
    {
        ValidatePage(ref pageSize, page);
        var query = db.Drivers.AsNoTracking().Include(x => x.User).Include(x => x.EmploymentStatus)
            .Include(x => x.AvailabilityStatus).Include(x => x.VehicleAssignments.Where(a => a.AssignedTo == null)).ThenInclude(x => x.Vehicle).AsQueryable();
        if (eligibleOnly) query = query.Where(EligibleDriver());
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim(); query = query.Where(x => x.DriverNumber.Contains(term) || x.DisplayName.Contains(term) || (x.User.PhoneE164 != null && x.User.PhoneE164.Contains(term)));
        }
        var total = await query.CountAsync(ct);
        var rows = await query.OrderBy(x => x.DriverNumber).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new AdminDriverPage(rows.Select(MapDriver).ToList(), page, pageSize, total, Pages(total, pageSize));
    }

    public async Task<AdminVehiclePage> ListVehiclesAsync(bool eligibleOnly, string? vehicleType, string? search, int page, int pageSize, CancellationToken ct)
    {
        ValidatePage(ref pageSize, page);
        var query = db.Vehicles.AsNoTracking().Include(x => x.Status).Include(x => x.VehicleType)
            .Include(x => x.DriverAssignments.Where(a => a.AssignedTo == null)).ThenInclude(x => x.Driver).AsQueryable();
        if (eligibleOnly) query = query.Where(x => x.StatusId == ReferenceData.VehicleStatusActive && x.VehicleType.IsActive);
        if (!string.IsNullOrWhiteSpace(vehicleType)) query = query.Where(x => x.VehicleType.Code == vehicleType.Trim().ToLowerInvariant());
        if (!string.IsNullOrWhiteSpace(search)) { var term = search.Trim().ToUpperInvariant(); query = query.Where(x => x.RegistrationNumber.Contains(term)); }
        var total = await query.CountAsync(ct);
        var rows = await query.OrderBy(x => x.RegistrationNumber).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new AdminVehiclePage(rows.Select(MapVehicle).ToList(), page, pageSize, total, Pages(total, pageSize));
    }

    public async Task<IReadOnlyList<AdminVehicleTypeItem>> VehicleTypesAsync(CancellationToken ct) =>
        await db.VehicleTypes.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.SortOrder)
            .Select(x => new AdminVehicleTypeItem(x.Id, x.Code, x.Name, x.TypicalCapacity)).ToListAsync(ct);

    public async Task<AdminDriverDetails> GetDriverAsync(Guid id, CancellationToken ct)
    {
        var driver = await DriverQuery().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Driver was not found.");
        return DriverDetails(driver);
    }

    public async Task<AdminVehicleDetails> GetVehicleAsync(Guid id, CancellationToken ct)
    {
        var vehicle = await VehicleQuery().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Vehicle was not found.");
        return VehicleDetails(vehicle);
    }

    public async Task<AdminDriverDetails> CreateDriverAsync(Guid adminId, CreateDriverRequest request, IPAddress? ip, CancellationToken ct)
    {
        var name = Required(request.DisplayName, 120, "Driver name");
        if (!PhoneNormalizer.TryNormalize(request.PhoneNumber, out var phone)) throw new InvalidRequestException("Enter a valid mobile number.");
        var employment = Code(Employment, request.EmploymentStatus, "employment status"); var availability = Code(Availability, request.AvailabilityStatus, "availability status");
        await using var tx = await db.Database.BeginTransactionAsync(ct); var now = clock.GetUtcNow(); var userId = Guid.NewGuid(); var driverId = Guid.NewGuid();
        db.Users.Add(new User { Id = userId, PhoneE164 = phone, StatusId = ReferenceData.UserStatusActive, PhoneConfirmedAt = null, CreatedAt = now, UpdatedAt = now });
        db.UserRoles.Add(new UserRole { UserId = userId, RoleId = ReferenceData.RoleIds.Driver, AssignedByUserId = adminId, AssignedAt = now });
        db.Drivers.Add(new Driver { Id = driverId, UserId = userId, DisplayName = name, EmploymentStatusId = employment, AvailabilityStatusId = availability, CreatedAt = now, UpdatedAt = now });
        Audit(adminId, "driver_created", "driver", driverId, null, new { name, phone, employmentStatus = request.EmploymentStatus, availabilityStatus = request.AvailabilityStatus }, ip, now);
        try { await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); }
        catch (DbUpdateException ex) when (Unique(ex)) { await tx.RollbackAsync(ct); throw new ConflictException("A user with this mobile number already exists."); }
        return await GetDriverAsync(driverId, ct);
    }

    public async Task<AdminDriverDetails> UpdateDriverAsync(Guid adminId, Guid id, UpdateDriverRequest request, IPAddress? ip, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var driver = await db.Drivers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Driver was not found.");
        db.Entry(driver).Property("xmin").OriginalValue = request.Version;
        var old = new { driver.DisplayName, driver.User.PhoneE164, driver.EmploymentStatusId, driver.AvailabilityStatusId };
        driver.DisplayName = Required(request.DisplayName, 120, "Driver name");
        if (!PhoneNormalizer.TryNormalize(request.PhoneNumber, out var phone)) throw new InvalidRequestException("Enter a valid mobile number.");
        var newEmployment = Code(Employment, request.EmploymentStatus, "employment status");
        if (driver.EmploymentStatusId == ReferenceData.DriverEmploymentActive && newEmployment != ReferenceData.DriverEmploymentActive)
        {
            if (await UnsafeDriverDeactivation(id, ct)) throw new ConflictException("This driver has an active or future booking assignment.");
            await CloseRoster(driverId: id, vehicleId: null, ct);
        }
        driver.User.PhoneE164 = phone; driver.EmploymentStatusId = newEmployment; driver.AvailabilityStatusId = Code(Availability, request.AvailabilityStatus, "availability status");
        Audit(adminId, "driver_updated", "driver", id, old, new { driver.DisplayName, phone, driver.EmploymentStatusId, driver.AvailabilityStatusId }, ip);
        if (old.EmploymentStatusId == ReferenceData.DriverEmploymentActive && newEmployment != ReferenceData.DriverEmploymentActive) Audit(adminId, "driver_deactivated", "driver", id, new { old.EmploymentStatusId }, new { driver.EmploymentStatusId }, ip);
        else if (old.EmploymentStatusId != ReferenceData.DriverEmploymentActive && newEmployment == ReferenceData.DriverEmploymentActive) Audit(adminId, "driver_reactivated", "driver", id, new { old.EmploymentStatusId }, new { driver.EmploymentStatusId }, ip);
        try { await SaveUpdate(ct, "The driver changed. Refresh and try again.", "A user with this mobile number already exists."); await tx.CommitAsync(ct); }
        catch { await tx.RollbackAsync(ct); throw; }
        return await GetDriverAsync(id, ct);
    }

    public Task<AdminDriverDetails> DeactivateDriverAsync(Guid adminId, Guid id, FleetVersionRequest request, IPAddress? ip, CancellationToken ct) => SetDriverActive(adminId, id, request.Version, false, ip, ct);
    public Task<AdminDriverDetails> ReactivateDriverAsync(Guid adminId, Guid id, FleetVersionRequest request, IPAddress? ip, CancellationToken ct) => SetDriverActive(adminId, id, request.Version, true, ip, ct);

    public async Task<AdminVehicleDetails> CreateVehicleAsync(Guid adminId, CreateVehicleRequest request, IPAddress? ip, CancellationToken ct)
    {
        var registration = NormalizeRegistration(request.RegistrationNumber); var status = Code(VehicleStatuses, request.Status, "vehicle status");
        var type = await db.VehicleTypes.SingleOrDefaultAsync(x => x.Id == request.VehicleTypeId && x.IsActive, ct) ?? throw new InvalidRequestException("Choose a valid active vehicle type.");
        var id = Guid.NewGuid(); var vehicle = new Vehicle { Id = id, RegistrationNumber = registration, VehicleTypeId = type.Id, Capacity = request.Capacity, StatusId = status };
        db.Vehicles.Add(vehicle); Audit(adminId, "vehicle_created", "vehicle", id, null, new { registration, vehicleTypeId = type.Id, request.Capacity, request.Status }, ip);
        try { await db.SaveChangesAsync(ct); } catch (DbUpdateException ex) when (Unique(ex)) { throw new ConflictException("A vehicle with this registration already exists."); }
        return await GetVehicleAsync(id, ct);
    }

    public async Task<AdminVehicleDetails> UpdateVehicleAsync(Guid adminId, Guid id, UpdateVehicleRequest request, IPAddress? ip, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var vehicle = await db.Vehicles.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Vehicle was not found."); db.Entry(vehicle).Property("xmin").OriginalValue = request.Version;
        var type = await db.VehicleTypes.SingleOrDefaultAsync(x => x.Id == request.VehicleTypeId && x.IsActive, ct) ?? throw new InvalidRequestException("Choose a valid active vehicle type.");
        var old = new { vehicle.RegistrationNumber, vehicle.VehicleTypeId, vehicle.Capacity, vehicle.StatusId };
        var newStatus = Code(VehicleStatuses, request.Status, "vehicle status");
        if (vehicle.StatusId == ReferenceData.VehicleStatusActive && newStatus != ReferenceData.VehicleStatusActive)
        {
            if (await UnsafeVehicleDeactivation(id, ct)) throw new ConflictException("This vehicle has an active or future booking assignment.");
            await CloseRoster(driverId: null, vehicleId: id, ct);
        }
        vehicle.RegistrationNumber = NormalizeRegistration(request.RegistrationNumber); vehicle.VehicleTypeId = type.Id; vehicle.Capacity = request.Capacity; vehicle.StatusId = newStatus;
        Audit(adminId, "vehicle_updated", "vehicle", id, old, new { vehicle.RegistrationNumber, vehicle.VehicleTypeId, vehicle.Capacity, vehicle.StatusId }, ip);
        if (old.StatusId == ReferenceData.VehicleStatusActive && newStatus != ReferenceData.VehicleStatusActive) Audit(adminId, "vehicle_deactivated", "vehicle", id, new { old.StatusId }, new { vehicle.StatusId }, ip);
        else if (old.StatusId != ReferenceData.VehicleStatusActive && newStatus == ReferenceData.VehicleStatusActive) Audit(adminId, "vehicle_reactivated", "vehicle", id, new { old.StatusId }, new { vehicle.StatusId }, ip);
        try { await SaveUpdate(ct, "The vehicle changed. Refresh and try again.", "A vehicle with this registration already exists."); await tx.CommitAsync(ct); }
        catch { await tx.RollbackAsync(ct); throw; }
        return await GetVehicleAsync(id, ct);
    }

    public Task<AdminVehicleDetails> DeactivateVehicleAsync(Guid adminId, Guid id, FleetVersionRequest request, IPAddress? ip, CancellationToken ct) => SetVehicleActive(adminId, id, request.Version, false, ip, ct);
    public Task<AdminVehicleDetails> ReactivateVehicleAsync(Guid adminId, Guid id, FleetVersionRequest request, IPAddress? ip, CancellationToken ct) => SetVehicleActive(adminId, id, request.Version, true, ip, ct);

    public async Task<AdminDriverDetails> TagVehicleAsync(Guid adminId, Guid driverId, TagVehicleRequest request, IPAddress? ip, CancellationToken ct)
    {
        await ChangeRosterAsync(adminId, driverId, request.VehicleId, request.Version, null, ip, ct); return await GetDriverAsync(driverId, ct);
    }

    public async Task<AdminVehicleDetails> TagDriverAsync(Guid adminId, Guid vehicleId, TagDriverRequest request, IPAddress? ip, CancellationToken ct)
    {
        if (request.DriverId is null) await UntagVehicleAsync(adminId, vehicleId, request.Version, ip, ct);
        else await ChangeRosterAsync(adminId, request.DriverId.Value, vehicleId, null, request.Version, ip, ct);
        return await GetVehicleAsync(vehicleId, ct);
    }

    private async Task UntagVehicleAsync(Guid adminId, Guid vehicleId, uint version, IPAddress? ip, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        try
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM vehicle WHERE id = {vehicleId} FOR UPDATE", ct);
            var vehicle = await db.Vehicles.SingleOrDefaultAsync(x => x.Id == vehicleId, ct) ?? throw new NotFoundException("Vehicle was not found.");
            if (db.Entry(vehicle).Property<uint>("xmin").CurrentValue != version) throw new ConflictException("The vehicle changed. Refresh and try again.");
            var current = await db.DriverVehicleAssignments.SingleOrDefaultAsync(x => x.VehicleId == vehicleId && x.AssignedTo == null, ct);
            if (current is not null)
            {
                var now = clock.GetUtcNow(); current.AssignedTo = now; vehicle.UpdatedAt = now;
                Audit(adminId, "driver_vehicle_association_changed", "vehicle", vehicleId, new { current.DriverId, current.VehicleId }, new { driverId = (Guid?)null, vehicleId }, ip, now);
                await db.SaveChangesAsync(ct);
            }
            await tx.CommitAsync(ct);
        }
        catch (DbUpdateConcurrencyException) { await tx.RollbackAsync(ct); throw new ConflictException("The vehicle changed. Refresh and try again."); }
        catch { await tx.RollbackAsync(ct); throw; }
    }

    private async Task ChangeRosterAsync(Guid adminId, Guid driverId, Guid? vehicleId, uint? driverVersion, uint? vehicleVersion, IPAddress? ip, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        try
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM driver WHERE id = {driverId} FOR UPDATE", ct);
            var driver = await db.Drivers.SingleOrDefaultAsync(x => x.Id == driverId, ct) ?? throw new NotFoundException("Driver was not found.");
            if (driverVersion.HasValue && db.Entry(driver).Property<uint>("xmin").CurrentValue != driverVersion.Value) throw new ConflictException("The driver changed. Refresh and try again.");
            Vehicle? vehicle = null;
            if (vehicleId.HasValue)
            {
                await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM vehicle WHERE id = {vehicleId.Value} FOR UPDATE", ct);
                vehicle = await db.Vehicles.SingleOrDefaultAsync(x => x.Id == vehicleId, ct) ?? throw new NotFoundException("Vehicle was not found.");
                if (vehicleVersion.HasValue && db.Entry(vehicle).Property<uint>("xmin").CurrentValue != vehicleVersion.Value) throw new ConflictException("The vehicle changed. Refresh and try again.");
                if (driver.EmploymentStatusId != ReferenceData.DriverEmploymentActive || vehicle.StatusId != ReferenceData.VehicleStatusActive) throw new ConflictException("Only active drivers and vehicles can be tagged.");
            }
            var now = clock.GetUtcNow();
            var current = await db.DriverVehicleAssignments.Where(x => x.AssignedTo == null && (x.DriverId == driverId || (vehicleId != null && x.VehicleId == vehicleId))).ToListAsync(ct);
            if (vehicleId.HasValue && current.Any(x => x.DriverId == driverId && x.VehicleId == vehicleId)) { await tx.CommitAsync(ct); return; }
            foreach (var assignment in current) assignment.AssignedTo = now;
            if (vehicleId.HasValue) db.DriverVehicleAssignments.Add(new DriverVehicleAssignment { Id = Guid.NewGuid(), DriverId = driverId, VehicleId = vehicleId.Value, AssignedFrom = now, AssignedByUserId = adminId, CreatedAt = now });
            driver.UpdatedAt = now;
            if (vehicle is not null) vehicle.UpdatedAt = now;
            Audit(adminId, current.Count == 0 ? "driver_vehicle_tagged" : "driver_vehicle_association_changed", "driver", driverId,
                new { assignments = current.Select(x => new { x.DriverId, x.VehicleId }).ToList() }, new { driverId, vehicleId }, ip, now);
            await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        }
        catch (DbUpdateConcurrencyException) { await tx.RollbackAsync(ct); throw new ConflictException("The fleet record changed. Refresh and try again."); }
        catch (DbUpdateException ex) when (Unique(ex)) { await tx.RollbackAsync(ct); throw new ConflictException("The driver or vehicle is already tagged elsewhere. Refresh and try again."); }
        catch { await tx.RollbackAsync(ct); throw; }
    }

    private async Task<AdminDriverDetails> SetDriverActive(Guid adminId, Guid id, uint version, bool active, IPAddress? ip, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct); var driver = await db.Drivers.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Driver was not found."); db.Entry(driver).Property("xmin").OriginalValue = version;
        if (!active && await UnsafeDriverDeactivation(id, ct)) throw new ConflictException("This driver has an active or future booking assignment.");
        var old = driver.EmploymentStatusId; driver.EmploymentStatusId = active ? ReferenceData.DriverEmploymentActive : ReferenceData.DriverEmploymentInactive;
        if (!active) await CloseRoster(driverId: id, vehicleId: null, ct);
        Audit(adminId, active ? "driver_reactivated" : "driver_deactivated", "driver", id, new { employmentStatusId = old }, new { driver.EmploymentStatusId }, ip);
        try { await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); } catch (DbUpdateConcurrencyException) { await tx.RollbackAsync(ct); throw new ConflictException("The driver changed. Refresh and try again."); }
        return await GetDriverAsync(id, ct);
    }

    private async Task<AdminVehicleDetails> SetVehicleActive(Guid adminId, Guid id, uint version, bool active, IPAddress? ip, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct); var vehicle = await db.Vehicles.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Vehicle was not found."); db.Entry(vehicle).Property("xmin").OriginalValue = version;
        if (!active && await UnsafeVehicleDeactivation(id, ct)) throw new ConflictException("This vehicle has an active or future booking assignment.");
        var old = vehicle.StatusId; vehicle.StatusId = active ? ReferenceData.VehicleStatusActive : ReferenceData.VehicleStatusInactive;
        if (!active) await CloseRoster(driverId: null, vehicleId: id, ct);
        Audit(adminId, active ? "vehicle_reactivated" : "vehicle_deactivated", "vehicle", id, new { statusId = old }, new { vehicle.StatusId }, ip);
        try { await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); } catch (DbUpdateConcurrencyException) { await tx.RollbackAsync(ct); throw new ConflictException("The vehicle changed. Refresh and try again."); }
        return await GetVehicleAsync(id, ct);
    }

    private Task<bool> UnsafeDriverDeactivation(Guid id, CancellationToken ct) => db.Bookings.AnyAsync(x => x.AssignedDriverId == id && x.StatusId != ReferenceData.BookingStatusRejected && x.StatusId != ReferenceData.BookingStatusCancelled && x.StatusId != ReferenceData.BookingStatusCompleted, ct);
    private Task<bool> UnsafeVehicleDeactivation(Guid id, CancellationToken ct) => db.Bookings.AnyAsync(x => x.AssignedVehicleId == id && x.StatusId != ReferenceData.BookingStatusRejected && x.StatusId != ReferenceData.BookingStatusCancelled && x.StatusId != ReferenceData.BookingStatusCompleted, ct);
    private async Task CloseRoster(Guid? driverId, Guid? vehicleId, CancellationToken ct) { var now = clock.GetUtcNow(); var rows = await db.DriverVehicleAssignments.Where(x => x.AssignedTo == null && (driverId == null || x.DriverId == driverId) && (vehicleId == null || x.VehicleId == vehicleId)).ToListAsync(ct); foreach (var row in rows) row.AssignedTo = now; }

    private IQueryable<Driver> DriverQuery() => db.Drivers.Include(x => x.User).Include(x => x.EmploymentStatus).Include(x => x.AvailabilityStatus).Include(x => x.VehicleAssignments).ThenInclude(x => x.Vehicle);
    private IQueryable<Vehicle> VehicleQuery() => db.Vehicles.Include(x => x.Status).Include(x => x.VehicleType).Include(x => x.DriverAssignments).ThenInclude(x => x.Driver);
    private AdminDriverDetails DriverDetails(Driver x) { var current = x.VehicleAssignments.SingleOrDefault(a => a.AssignedTo == null); return new(x.Id, x.DriverNumber, x.DisplayName, x.User.PhoneE164 ?? "", x.EmploymentStatus.Code, x.AvailabilityStatus.Code, IsEligible(x), db.Entry(x).Property<uint>("xmin").CurrentValue, current?.VehicleId, current?.Vehicle.RegistrationNumber, x.VehicleAssignments.OrderByDescending(a => a.AssignedFrom).Select(History).ToList()); }
    private AdminVehicleDetails VehicleDetails(Vehicle x) { var current = x.DriverAssignments.SingleOrDefault(a => a.AssignedTo == null); return new(x.Id, x.RegistrationNumber, x.VehicleTypeId, x.VehicleType.Code, x.VehicleType.Name, x.Capacity, x.Status.Code, IsEligible(x), db.Entry(x).Property<uint>("xmin").CurrentValue, current?.DriverId, current?.Driver.DriverNumber, current?.Driver.DisplayName, x.DriverAssignments.OrderByDescending(a => a.AssignedFrom).Select(History).ToList()); }
    private static AdminRosterHistory History(DriverVehicleAssignment x) => new(x.Id, x.DriverId, x.Driver.DriverNumber, x.Driver.DisplayName, x.VehicleId, x.Vehicle.RegistrationNumber, x.AssignedFrom, x.AssignedTo);
    private static AdminDriverItem MapDriver(Driver x) { var current = x.VehicleAssignments.SingleOrDefault(); return new(x.Id, x.DriverNumber, x.DisplayName, x.User.PhoneE164 ?? "", x.EmploymentStatus.Code, x.AvailabilityStatus.Code, IsEligible(x), current?.VehicleId, current?.Vehicle.RegistrationNumber); }
    private static AdminVehicleItem MapVehicle(Vehicle x) { var current = x.DriverAssignments.SingleOrDefault(); return new(x.Id, x.RegistrationNumber, x.VehicleType.Code, x.VehicleType.Name, x.Capacity, x.Status.Code, IsEligible(x), current?.DriverId, current?.Driver.DriverNumber, current?.Driver.DisplayName); }
    private static bool IsEligible(Driver x) => x.EmploymentStatusId == ReferenceData.DriverEmploymentActive && x.AvailabilityStatusId == ReferenceData.DriverAvailabilityAvailable && x.User.StatusId == ReferenceData.UserStatusActive && x.User.PhoneE164 is not null;
    private static bool IsEligible(Vehicle x) => x.StatusId == ReferenceData.VehicleStatusActive && x.VehicleType.IsActive;
    private static System.Linq.Expressions.Expression<Func<Driver, bool>> EligibleDriver() => x => x.EmploymentStatusId == ReferenceData.DriverEmploymentActive && x.AvailabilityStatusId == ReferenceData.DriverAvailabilityAvailable && x.User.StatusId == ReferenceData.UserStatusActive && x.User.PhoneE164 != null;
    private void Audit(Guid actor, string action, string entityType, Guid entityId, object? oldValue, object? newValue, IPAddress? ip, DateTimeOffset? now = null) => db.AuditLogs.Add(new AuditLog { ActorUserId = actor, Action = action, EntityType = entityType, EntityId = entityId, OldValue = oldValue is null ? null : JsonSerializer.Serialize(oldValue), NewValue = newValue is null ? null : JsonSerializer.Serialize(newValue), IpAddress = ip, CreatedAt = now ?? clock.GetUtcNow() });
    private async Task SaveUpdate(CancellationToken ct, string stale, string duplicate) { try { await db.SaveChangesAsync(ct); } catch (DbUpdateConcurrencyException) { throw new ConflictException(stale); } catch (DbUpdateException ex) when (Unique(ex)) { throw new ConflictException(duplicate); } }
    private static bool Unique(DbUpdateException ex) => ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    private static string Required(string value, int max, string label) => string.IsNullOrWhiteSpace(value) ? throw new InvalidRequestException($"{label} is required.") : value.Trim().Length > max ? throw new InvalidRequestException($"{label} is too long.") : value.Trim();
    private static short Code(IReadOnlyDictionary<string, short> values, string value, string label) => values.TryGetValue(value.Trim(), out var id) ? id : throw new InvalidRequestException($"Choose a valid {label}.");
    private static string NormalizeRegistration(string value) { var result = new string(value.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToUpperInvariant(); return Registration.IsMatch(result) ? result : throw new InvalidRequestException("Enter a valid registration number using letters, numbers, or hyphens."); }
    private static int Pages(int count, int pageSize) => count == 0 ? 0 : (int)Math.Ceiling(count / (double)pageSize);
    private static void ValidatePage(ref int pageSize, int page) { if (page < 1) throw new InvalidRequestException("Page must be at least 1."); if (pageSize == 0) pageSize = DefaultPageSize; if (pageSize is < 1 or > MaxPageSize) throw new InvalidRequestException($"Page size must be between 1 and {MaxPageSize}."); }
}
