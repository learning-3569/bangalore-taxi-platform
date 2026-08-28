using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BangaloreTaxi.Api.Persistence.Migrations;

[DbContext(typeof(BangaloreTaxiDbContext))]
[Migration("20260827120000_Phase6AuthenticatedBookings")]
public sealed class Phase6AuthenticatedBookings : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "idempotency_key",
            table: "booking",
            type: "character varying(64)",
            maxLength: 64,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_booking_customer_id_idempotency_key",
            table: "booking",
            columns: new[] { "customer_id", "idempotency_key" },
            unique: true,
            filter: "customer_id IS NOT NULL AND idempotency_key IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "ix_booking_customer_id_idempotency_key", table: "booking");
        migrationBuilder.DropColumn(name: "idempotency_key", table: "booking");
    }
}
