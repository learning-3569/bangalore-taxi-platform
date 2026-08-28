using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BangaloreTaxi.Api.Persistence.Migrations;

[DbContext(typeof(BangaloreTaxiDbContext))]
[Migration("20260827150000_AirportRoundTripReturn")]
public sealed class AirportRoundTripReturn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(name: "return_at", table: "booking", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateOnly>(name: "return_local_date", table: "booking", type: "date", nullable: true);
        migrationBuilder.AddCheckConstraint(
            name: "ck_booking_return_complete",
            table: "booking",
            sql: "(return_at IS NULL AND return_local_date IS NULL) OR (return_at IS NOT NULL AND return_local_date IS NOT NULL AND return_at > pickup_at)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(name: "ck_booking_return_complete", table: "booking");
        migrationBuilder.DropColumn(name: "return_at", table: "booking");
        migrationBuilder.DropColumn(name: "return_local_date", table: "booking");
    }
}
