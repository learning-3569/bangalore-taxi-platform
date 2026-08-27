using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BangaloreTaxi.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PhoneOtpAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "otp_challenge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_e164 = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    code_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    salt = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    attempt_count = table.Column<short>(type: "smallint", nullable: false),
                    consumed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    request_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_otp_challenge", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    replaced_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    request_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_session", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_session_refresh_session_replaced_by_id",
                        column: x => x.replaced_by_id,
                        principalTable: "refresh_session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_refresh_session_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_phone_e164_format",
                table: "users",
                sql: "phone_e164 IS NULL OR phone_e164 ~ '^\\+[1-9][0-9]{7,14}$'");

            migrationBuilder.CreateIndex(
                name: "ix_otp_challenge_phone_active",
                table: "otp_challenge",
                column: "phone_e164",
                filter: "consumed_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_otp_challenge_phone_e164_created_at",
                table: "otp_challenge",
                columns: new[] { "phone_e164", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_session_replaced_by_id",
                table: "refresh_session",
                column: "replaced_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_session_token_hash",
                table: "refresh_session",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_session_user_id_created_at",
                table: "refresh_session",
                columns: new[] { "user_id", "created_at" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "otp_challenge");

            migrationBuilder.DropTable(
                name: "refresh_session");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_phone_e164_format",
                table: "users");
        }
    }
}
