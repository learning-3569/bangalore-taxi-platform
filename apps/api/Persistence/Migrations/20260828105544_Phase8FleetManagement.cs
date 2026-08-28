using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BangaloreTaxi.Api.Persistence.Migrations;

public partial class Phase8FleetManagement : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateSequence<long>(name: "driver_number_seq");

        migrationBuilder.AddColumn<string>(
            name: "driver_number",
            table: "driver",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);

        migrationBuilder.Sql(
            "UPDATE driver SET driver_number = 'DRV-' || lpad(nextval('driver_number_seq')::text, 6, '0') WHERE driver_number IS NULL");

        migrationBuilder.AlterColumn<string>(
            name: "driver_number",
            table: "driver",
            type: "character varying(10)",
            maxLength: 10,
            nullable: false,
            defaultValueSql: "'DRV-' || lpad(nextval('driver_number_seq')::text, 6, '0')",
            oldClrType: typeof(string),
            oldType: "character varying(10)",
            oldMaxLength: 10,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_driver_driver_number",
            table: "driver",
            column: "driver_number",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "ix_driver_driver_number", table: "driver");
        migrationBuilder.DropColumn(name: "driver_number", table: "driver");
        migrationBuilder.DropSequence(name: "driver_number_seq");
    }
}
