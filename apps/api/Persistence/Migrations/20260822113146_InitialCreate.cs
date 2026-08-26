using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BangaloreTaxi.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gist", ",,")
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "booking_number_sequence",
                columns: table => new
                {
                    year = table.Column<int>(type: "integer", nullable: false),
                    last_value = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_number_sequence", x => x.year);
                });

            migrationBuilder.CreateTable(
                name: "booking_status",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customer_status",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "driver_availability_status",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_availability_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "driver_employment_status",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_employment_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "journey_type",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journey_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_channel",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_channel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_status",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_type",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "operational_setting",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    value = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_operational_setting", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "pricing_component",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pricing_component", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pricing_plan",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    effective_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    effective_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pricing_plan", x => x.id);
                    table.CheckConstraint("ck_pricing_plan_effective", "effective_to IS NULL OR effective_to > effective_from");
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "seo_page_status",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seo_page_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "trip_type",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trip_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_status",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_status",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_type",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    typical_capacity = table.Column<short>(type: "smallint", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "seo_page",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    title = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    meta_description = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    h1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    canonical_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    featured_image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status_id = table.Column<short>(type: "smallint", nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seo_page", x => x.id);
                    table.ForeignKey(
                        name: "fk_seo_page_seo_page_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "seo_page_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "citext", maxLength: 256, nullable: true),
                    phone_e164 = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    status_id = table.Column<short>(type: "smallint", nullable: false),
                    email_confirmed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    phone_confirmed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.CheckConstraint("ck_users_email_or_phone", "email IS NOT NULL OR phone_e164 IS NOT NULL");
                    table.ForeignKey(
                        name: "fk_users_user_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "user_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pricing_rate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pricing_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_type_id = table.Column<short>(type: "smallint", nullable: false),
                    journey_type_id = table.Column<short>(type: "smallint", nullable: false),
                    component_id = table.Column<short>(type: "smallint", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pricing_rate", x => x.id);
                    table.ForeignKey(
                        name: "fk_pricing_rate_journey_type_journey_type_id",
                        column: x => x.journey_type_id,
                        principalTable: "journey_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pricing_rate_pricing_component_component_id",
                        column: x => x.component_id,
                        principalTable: "pricing_component",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pricing_rate_pricing_plan_pricing_plan_id",
                        column: x => x.pricing_plan_id,
                        principalTable: "pricing_plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pricing_rate_trip_types_trip_type_id",
                        column: x => x.trip_type_id,
                        principalTable: "trip_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pricing_rate_vehicle_types_vehicle_type_id",
                        column: x => x.vehicle_type_id,
                        principalTable: "vehicle_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vehicle",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registration_number = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    vehicle_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    capacity = table.Column<short>(type: "smallint", nullable: false),
                    status_id = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicle_vehicle_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "vehicle_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_vehicle_vehicle_types_vehicle_type_id",
                        column: x => x.vehicle_type_id,
                        principalTable: "vehicle_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "seo_page_faq",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seo_page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    answer = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seo_page_faq", x => x.id);
                    table.ForeignKey(
                        name: "fk_seo_page_faq_seo_page_seo_page_id",
                        column: x => x.seo_page_id,
                        principalTable: "seo_page",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    actor_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    old_value = table.Column<string>(type: "jsonb", nullable: true),
                    new_value = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_log", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_log_users_actor_user_id",
                        column: x => x.actor_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    status_id = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_customer_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "customer_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_customer_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "driver",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    employment_status_id = table.Column<short>(type: "smallint", nullable: false),
                    availability_status_id = table.Column<short>(type: "smallint", nullable: false),
                    license_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    license_expires_on = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_driver_availability_status_availability_status_id",
                        column: x => x.availability_status_id,
                        principalTable: "driver_availability_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_driver_driver_employment_statuses_employment_status_id",
                        column: x => x.employment_status_id,
                        principalTable: "driver_employment_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_driver_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    assigned_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_role_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_role_users_assigned_by_user_id",
                        column: x => x.assigned_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_role_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "booking",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_number = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    contact_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    contact_mobile_e164 = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    contact_email = table.Column<string>(type: "citext", maxLength: 256, nullable: true),
                    pickup_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    pickup_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    pickup_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    drop_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    drop_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    drop_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    pickup_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    pickup_time_zone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    pickup_local_date = table.Column<DateOnly>(type: "date", nullable: false),
                    estimated_end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    estimated_distance_km = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    estimated_fare_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: true),
                    requested_vehicle_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_type_id = table.Column<short>(type: "smallint", nullable: false),
                    journey_type_id = table.Column<short>(type: "smallint", nullable: false),
                    assigned_driver_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_driver_display_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    assigned_driver_phone_e164 = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    assigned_vehicle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_vehicle_registration = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    assigned_vehicle_type_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    assigned_vehicle_type_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    assignment_window = table.Column<NpgsqlRange<DateTime>>(type: "tstzrange", nullable: true),
                    status_id = table.Column<short>(type: "smallint", nullable: false),
                    customer_notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    pricing_plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking", x => x.id);
                    table.CheckConstraint("ck_booking_assignment_complete", "assigned_vehicle_id IS NULL OR (assigned_driver_id IS NOT NULL AND estimated_end_at IS NOT NULL AND assignment_window IS NOT NULL AND assigned_driver_display_name IS NOT NULL AND assigned_driver_phone_e164 IS NOT NULL AND assigned_vehicle_registration IS NOT NULL AND assigned_vehicle_type_code IS NOT NULL AND assigned_vehicle_type_name IS NOT NULL)");
                    table.CheckConstraint("ck_booking_drop_coords", "(drop_latitude IS NULL AND drop_longitude IS NULL) OR (drop_latitude IS NOT NULL AND drop_longitude IS NOT NULL)");
                    table.CheckConstraint("ck_booking_drop_lat_range", "drop_latitude IS NULL OR (drop_latitude >= -90 AND drop_latitude <= 90)");
                    table.CheckConstraint("ck_booking_drop_lng_range", "drop_longitude IS NULL OR (drop_longitude >= -180 AND drop_longitude <= 180)");
                    table.CheckConstraint("ck_booking_fare_currency", "estimated_fare_amount IS NULL OR currency_code IS NOT NULL");
                    table.CheckConstraint("ck_booking_pickup_coords", "(pickup_latitude IS NULL AND pickup_longitude IS NULL) OR (pickup_latitude IS NOT NULL AND pickup_longitude IS NOT NULL)");
                    table.CheckConstraint("ck_booking_pickup_lat_range", "pickup_latitude IS NULL OR (pickup_latitude >= -90 AND pickup_latitude <= 90)");
                    table.CheckConstraint("ck_booking_pickup_lng_range", "pickup_longitude IS NULL OR (pickup_longitude >= -180 AND pickup_longitude <= 180)");
                    table.ForeignKey(
                        name: "fk_booking_booking_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "booking_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_drivers_assigned_driver_id",
                        column: x => x.assigned_driver_id,
                        principalTable: "driver",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_journey_types_journey_type_id",
                        column: x => x.journey_type_id,
                        principalTable: "journey_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_pricing_plans_pricing_plan_id",
                        column: x => x.pricing_plan_id,
                        principalTable: "pricing_plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_trip_types_trip_type_id",
                        column: x => x.trip_type_id,
                        principalTable: "trip_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_vehicle_types_requested_vehicle_type_id",
                        column: x => x.requested_vehicle_type_id,
                        principalTable: "vehicle_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_vehicles_assigned_vehicle_id",
                        column: x => x.assigned_vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "driver_vehicle_assignment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    assigned_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    assigned_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_vehicle_assignment", x => x.id);
                    table.CheckConstraint("ck_driver_vehicle_assignment_range", "assigned_to IS NULL OR assigned_to > assigned_from");
                    table.ForeignKey(
                        name: "fk_driver_vehicle_assignment_driver_driver_id",
                        column: x => x.driver_id,
                        principalTable: "driver",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_driver_vehicle_assignment_users_assigned_by_user_id",
                        column: x => x.assigned_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_driver_vehicle_assignment_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "booking_status_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_status_id = table.Column<short>(type: "smallint", nullable: true),
                    to_status_id = table.Column<short>(type: "smallint", nullable: false),
                    changed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_status_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_booking_status_history_booking_booking_id",
                        column: x => x.booking_id,
                        principalTable: "booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_status_history_booking_status_from_status_id",
                        column: x => x.from_status_id,
                        principalTable: "booking_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_status_history_booking_status_to_status_id",
                        column: x => x.to_status_id,
                        principalTable: "booking_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_status_history_users_changed_by_user_id",
                        column: x => x.changed_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    recipient_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type_id = table.Column<short>(type: "smallint", nullable: false),
                    channel_id = table.Column<short>(type: "smallint", nullable: false),
                    status_id = table.Column<short>(type: "smallint", nullable: false),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    failure_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    provider_message_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_booking_booking_id",
                        column: x => x.booking_id,
                        principalTable: "booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_notification_customer_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_notification_notification_channel_channel_id",
                        column: x => x.channel_id,
                        principalTable: "notification_channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_notification_notification_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "notification_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_notification_notification_types_type_id",
                        column: x => x.type_id,
                        principalTable: "notification_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_notification_users_recipient_user_id",
                        column: x => x.recipient_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "booking_status",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "pending", "Pending" },
                    { (short)2, "accepted", "Accepted" },
                    { (short)3, "rejected", "Rejected" },
                    { (short)4, "driver_assigned", "Driver assigned" },
                    { (short)5, "confirmed", "Confirmed" },
                    { (short)6, "driver_en_route", "Driver en route" },
                    { (short)7, "picked_up", "Picked up" },
                    { (short)8, "completed", "Completed" },
                    { (short)9, "cancelled", "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "customer_status",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "active", "Active" },
                    { (short)2, "inactive", "Inactive" }
                });

            migrationBuilder.InsertData(
                table: "driver_availability_status",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "available", "Available" },
                    { (short)2, "unavailable", "Unavailable" },
                    { (short)3, "on_trip", "On trip" },
                    { (short)4, "off_duty", "Off duty" }
                });

            migrationBuilder.InsertData(
                table: "driver_employment_status",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "active", "Active" },
                    { (short)2, "inactive", "Inactive" },
                    { (short)3, "suspended", "Suspended" }
                });

            migrationBuilder.InsertData(
                table: "journey_type",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "one_way", "One way" },
                    { (short)2, "round_trip", "Round trip" }
                });

            migrationBuilder.InsertData(
                table: "notification_channel",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "whatsapp", "WhatsApp" },
                    { (short)2, "sms", "SMS" },
                    { (short)3, "email", "Email" }
                });

            migrationBuilder.InsertData(
                table: "notification_status",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "pending", "Pending" },
                    { (short)2, "sent", "Sent" },
                    { (short)3, "failed", "Failed" }
                });

            migrationBuilder.InsertData(
                table: "notification_type",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "booking_received", "Booking received" },
                    { (short)2, "booking_accepted", "Booking accepted" },
                    { (short)3, "booking_rejected", "Booking rejected" },
                    { (short)4, "booking_confirmed", "Booking confirmed" },
                    { (short)5, "driver_assigned", "Driver assigned" },
                    { (short)6, "trip_reminder", "Trip reminder" },
                    { (short)7, "booking_cancelled", "Booking cancelled" },
                    { (short)8, "admin_new_request", "Admin new request" }
                });

            migrationBuilder.InsertData(
                table: "operational_setting",
                columns: new[] { "key", "updated_at", "value" },
                values: new object[,]
                {
                    { "assignment_buffer_minutes", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "15" },
                    { "default_trip_duration_minutes", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "120" }
                });

            migrationBuilder.InsertData(
                table: "pricing_component",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "base_fare", "Base fare" },
                    { (short)2, "per_km", "Per kilometre" },
                    { (short)3, "minimum_fare", "Minimum fare" },
                    { (short)4, "airport_surcharge", "Airport surcharge" },
                    { (short)5, "night_surcharge", "Night surcharge" },
                    { (short)6, "waiting_per_minute", "Waiting per minute" },
                    { (short)7, "toll_pass_through", "Toll pass-through" },
                    { (short)8, "outstation_per_km", "Outstation per kilometre" },
                    { (short)9, "round_trip_multiplier", "Round-trip multiplier" }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "id", "code", "created_at", "name" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-4111-8111-000000000001"), "customer", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Customer" },
                    { new Guid("a1111111-1111-4111-8111-000000000002"), "admin", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Admin" },
                    { new Guid("a1111111-1111-4111-8111-000000000003"), "driver", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Driver" }
                });

            migrationBuilder.InsertData(
                table: "seo_page_status",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "draft", "Draft" },
                    { (short)2, "published", "Published" }
                });

            migrationBuilder.InsertData(
                table: "trip_type",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "airport", "Airport" },
                    { (short)2, "local", "Local" },
                    { (short)3, "outstation", "Outstation" },
                    { (short)4, "corporate", "Corporate" }
                });

            migrationBuilder.InsertData(
                table: "user_status",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "active", "Active" },
                    { (short)2, "disabled", "Disabled" },
                    { (short)3, "locked", "Locked" }
                });

            migrationBuilder.InsertData(
                table: "vehicle_status",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { (short)1, "active", "Active" },
                    { (short)2, "inactive", "Inactive" },
                    { (short)3, "maintenance", "Maintenance" }
                });

            migrationBuilder.InsertData(
                table: "vehicle_type",
                columns: new[] { "id", "code", "created_at", "is_active", "name", "sort_order", "typical_capacity", "updated_at" },
                values: new object[,]
                {
                    { new Guid("b2222222-2222-4222-8222-000000000001"), "sedan", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Sedan", 1, (short)4, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b2222222-2222-4222-8222-000000000002"), "suv", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "SUV", 2, (short)6, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b2222222-2222-4222-8222-000000000003"), "innova", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Innova", 3, (short)7, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b2222222-2222-4222-8222-000000000004"), "premium", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Premium", 4, (short)4, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_actor_user_id_created_at",
                table: "audit_log",
                columns: new[] { "actor_user_id", "created_at" },
                descending: new[] { false, true },
                filter: "actor_user_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_created_at",
                table: "audit_log",
                column: "created_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_entity_type_entity_id_created_at",
                table: "audit_log",
                columns: new[] { "entity_type", "entity_id", "created_at" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "ix_booking_assigned_driver_id_pickup_at",
                table: "booking",
                columns: new[] { "assigned_driver_id", "pickup_at" },
                filter: "assigned_driver_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_booking_assigned_vehicle_id_pickup_at",
                table: "booking",
                columns: new[] { "assigned_vehicle_id", "pickup_at" },
                filter: "assigned_vehicle_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_booking_booking_number",
                table: "booking",
                column: "booking_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_booking_contact_mobile_e164_pickup_at",
                table: "booking",
                columns: new[] { "contact_mobile_e164", "pickup_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_booking_customer_id_pickup_at",
                table: "booking",
                columns: new[] { "customer_id", "pickup_at" },
                descending: new[] { false, true },
                filter: "customer_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_booking_journey_type_id",
                table: "booking",
                column: "journey_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_pickup_at",
                table: "booking",
                column: "pickup_at");

            migrationBuilder.CreateIndex(
                name: "ix_booking_pickup_local_date_status_id",
                table: "booking",
                columns: new[] { "pickup_local_date", "status_id" });

            migrationBuilder.CreateIndex(
                name: "ix_booking_pricing_plan_id",
                table: "booking",
                column: "pricing_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_requested_vehicle_type_id",
                table: "booking",
                column: "requested_vehicle_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_status_id_pickup_at",
                table: "booking",
                columns: new[] { "status_id", "pickup_at" });

            migrationBuilder.CreateIndex(
                name: "ix_booking_trip_type_id",
                table: "booking",
                column: "trip_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_status_code",
                table: "booking_status",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_booking_status_history_booking_id_created_at",
                table: "booking_status_history",
                columns: new[] { "booking_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_booking_status_history_changed_by_user_id",
                table: "booking_status_history",
                column: "changed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_status_history_from_status_id",
                table: "booking_status_history",
                column: "from_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_status_history_to_status_id",
                table: "booking_status_history",
                column: "to_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_status_id",
                table: "customer",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_user_id",
                table: "customer",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_customer_status_code",
                table: "customer_status",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_driver_availability_status_id",
                table: "driver",
                column: "availability_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_employment_status_id_availability_status_id",
                table: "driver",
                columns: new[] { "employment_status_id", "availability_status_id" });

            migrationBuilder.CreateIndex(
                name: "ix_driver_license_number",
                table: "driver",
                column: "license_number",
                unique: true,
                filter: "license_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_driver_user_id",
                table: "driver",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_driver_availability_status_code",
                table: "driver_availability_status",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_driver_employment_status_code",
                table: "driver_employment_status",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_driver_vehicle_assignment_assigned_by_user_id",
                table: "driver_vehicle_assignment",
                column: "assigned_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ux_driver_vehicle_assignment_current_driver",
                table: "driver_vehicle_assignment",
                column: "driver_id",
                unique: true,
                filter: "assigned_to IS NULL");

            migrationBuilder.CreateIndex(
                name: "ux_driver_vehicle_assignment_current_vehicle",
                table: "driver_vehicle_assignment",
                column: "vehicle_id",
                unique: true,
                filter: "assigned_to IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_journey_type_code",
                table: "journey_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_booking_id_created_at",
                table: "notification",
                columns: new[] { "booking_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_notification_channel_id",
                table: "notification",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_customer_id",
                table: "notification",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_pending",
                table: "notification",
                columns: new[] { "status_id", "created_at" },
                filter: "status_id = 1");

            migrationBuilder.CreateIndex(
                name: "ix_notification_recipient_user_id",
                table: "notification",
                column: "recipient_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_type_id",
                table: "notification",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_channel_code",
                table: "notification_channel",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_status_code",
                table: "notification_status",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_type_code",
                table: "notification_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pricing_component_code",
                table: "pricing_component",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pricing_plan_code",
                table: "pricing_plan",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pricing_rate_component_id",
                table: "pricing_rate",
                column: "component_id");

            migrationBuilder.CreateIndex(
                name: "ix_pricing_rate_journey_type_id",
                table: "pricing_rate",
                column: "journey_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_pricing_rate_pricing_plan_id_vehicle_type_id_component_id_t",
                table: "pricing_rate",
                columns: new[] { "pricing_plan_id", "vehicle_type_id", "component_id", "trip_type_id", "journey_type_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pricing_rate_trip_type_id",
                table: "pricing_rate",
                column: "trip_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_pricing_rate_vehicle_type_id",
                table: "pricing_rate",
                column: "vehicle_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_code",
                table: "role",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_seo_page_slug",
                table: "seo_page",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_seo_page_status_id",
                table: "seo_page",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_seo_page_faq_seo_page_id_sort_order",
                table: "seo_page_faq",
                columns: new[] { "seo_page_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_seo_page_status_code",
                table: "seo_page_status",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_trip_type_code",
                table: "trip_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_role_assigned_by_user_id",
                table: "user_role",
                column: "assigned_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_role_role_id",
                table: "user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_status_code",
                table: "user_status",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true,
                filter: "email IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_phone_e164",
                table: "users",
                column: "phone_e164",
                unique: true,
                filter: "phone_e164 IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_status_id",
                table: "users",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_registration_number",
                table: "vehicle",
                column: "registration_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_status_id",
                table: "vehicle",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_vehicle_type_id",
                table: "vehicle",
                column: "vehicle_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_status_code",
                table: "vehicle_status",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_type_code",
                table: "vehicle_type",
                column: "code",
                unique: true);

            migrationBuilder.Sql("""
                ALTER TABLE booking ADD CONSTRAINT ex_booking_vehicle_assignment_window
                EXCLUDE USING gist (
                    assigned_vehicle_id WITH =,
                    assignment_window WITH &&
                )
                WHERE (assigned_vehicle_id IS NOT NULL AND status_id NOT IN (3, 9));
                """);

            migrationBuilder.Sql("""
                ALTER TABLE booking ADD CONSTRAINT ex_booking_driver_assignment_window
                EXCLUDE USING gist (
                    assigned_driver_id WITH =,
                    assignment_window WITH &&
                )
                WHERE (assigned_driver_id IS NOT NULL AND status_id NOT IN (3, 9));
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE booking DROP CONSTRAINT IF EXISTS ex_booking_vehicle_assignment_window;");
            migrationBuilder.Sql("ALTER TABLE booking DROP CONSTRAINT IF EXISTS ex_booking_driver_assignment_window;");

            migrationBuilder.DropTable(
                name: "audit_log");

            migrationBuilder.DropTable(
                name: "booking_number_sequence");

            migrationBuilder.DropTable(
                name: "booking_status_history");

            migrationBuilder.DropTable(
                name: "driver_vehicle_assignment");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "operational_setting");

            migrationBuilder.DropTable(
                name: "pricing_rate");

            migrationBuilder.DropTable(
                name: "seo_page_faq");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "booking");

            migrationBuilder.DropTable(
                name: "notification_channel");

            migrationBuilder.DropTable(
                name: "notification_status");

            migrationBuilder.DropTable(
                name: "notification_type");

            migrationBuilder.DropTable(
                name: "pricing_component");

            migrationBuilder.DropTable(
                name: "seo_page");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "booking_status");

            migrationBuilder.DropTable(
                name: "customer");

            migrationBuilder.DropTable(
                name: "driver");

            migrationBuilder.DropTable(
                name: "journey_type");

            migrationBuilder.DropTable(
                name: "pricing_plan");

            migrationBuilder.DropTable(
                name: "trip_type");

            migrationBuilder.DropTable(
                name: "vehicle");

            migrationBuilder.DropTable(
                name: "seo_page_status");

            migrationBuilder.DropTable(
                name: "customer_status");

            migrationBuilder.DropTable(
                name: "driver_availability_status");

            migrationBuilder.DropTable(
                name: "driver_employment_status");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "vehicle_status");

            migrationBuilder.DropTable(
                name: "vehicle_type");

            migrationBuilder.DropTable(
                name: "user_status");
        }
    }
}
