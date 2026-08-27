using BangaloreTaxi.Api.Application;
using BangaloreTaxi.Api.Hosting;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BangaloreTaxi.UnitTests;

public sealed class ExceptionHttpMapperTests
{
    [Fact]
    public void ConflictException_maps_to_409()
    {
        var mapped = ExceptionHttpMapper.Map(new ConflictException("Assignment window overlaps."));

        Assert.Equal(409, mapped.Status);
        Assert.Equal("Conflict", mapped.Title);
        Assert.Equal("Assignment window overlaps.", mapped.Detail);
    }

    [Fact]
    public void TooManyRequests_maps_to_429_with_retry_after()
    {
        var mapped = ExceptionHttpMapper.Map(new TooManyRequestsException("Please wait before requesting another code.", 12));
        Assert.Equal(429, mapped.Status);
        Assert.Equal(12, mapped.RetryAfterSeconds);
        Assert.DoesNotContain("otp", mapped.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UnauthorizedException_maps_to_401()
    {
        var mapped = ExceptionHttpMapper.Map(new UnauthorizedException("Unable to verify the code."));
        Assert.Equal(401, mapped.Status);
        Assert.DoesNotContain("otp", mapped.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NotFoundException_maps_to_404()
    {
        var mapped = ExceptionHttpMapper.Map(new NotFoundException("Booking was not found."));

        Assert.Equal(404, mapped.Status);
    }

    [Fact]
    public void Exclusion_violation_maps_to_409()
    {
        var postgres = new PostgresException("overlap", "ERROR", "ERROR", PostgresErrorCodes.ExclusionViolation);
        var mapped = ExceptionHttpMapper.Map(new DbUpdateException("failed", postgres));

        Assert.Equal(409, mapped.Status);
        Assert.DoesNotContain("overlap", mapped.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Unexpected_exception_does_not_leak_message()
    {
        var mapped = ExceptionHttpMapper.Map(new InvalidOperationException("secret-connection-string"));

        Assert.Equal(500, mapped.Status);
        Assert.DoesNotContain("secret-connection-string", mapped.Detail, StringComparison.Ordinal);
    }
}
