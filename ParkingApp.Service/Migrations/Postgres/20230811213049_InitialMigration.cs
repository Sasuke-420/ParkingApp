using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

#nullable disable

namespace Lisec.ParkingApp.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CardNumber = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "master_authorization",
                columns: table => new
                {
                    auth_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    auth_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    auth_desc = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    desc_id = table.Column<decimal>(type: "numeric(9,0)", nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    commercial = table.Column<int>(type: "integer", nullable: true),
                    dependent_on = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_authorization", x => x.auth_id);
                });

            migrationBuilder.CreateTable(
                name: "master_job_title",
                columns: table => new
                {
                    job_title_id = table.Column<decimal>(type: "numeric(3,0)", nullable: false),
                    job_title_desc = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    disabled = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_job_title", x => x.job_title_id);
                });

            migrationBuilder.CreateTable(
                name: "master_application_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    desc = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    deployment_type = table.Column<int>(type: "integer", maxLength: 240, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    entity_id = table.Column<decimal>(type: "numeric(9,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_application_details", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "master_company_entity",
                columns: table => new
                {
                    entity_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    entity_desc = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    entity_shdesc = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    entity_type = table.Column<decimal>(type: "numeric(2,0)", nullable: true),
                    disabled = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    host_ip = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    port_offset = table.Column<decimal>(type: "numeric(9,0)", nullable: true),
                    sub_entity_of = table.Column<decimal>(type: "numeric(9,0)", nullable: true),
                    assigned_user = table.Column<decimal>(type: "numeric(9,0)", nullable: true),
                    db_type = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    license_site_id = table.Column<int>(type: "integer", nullable: true),
                    license_site_api_key = table.Column<string>(type: "text", nullable: true),
                    license_signature = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_company_entity", x => x.entity_id);
                    table.ForeignKey(
                        name: "fk_mascoment_mascoment",
                        column: x => x.sub_entity_of,
                        principalTable: "master_company_entity",
                        principalColumn: "entity_id");
                });

            migrationBuilder.CreateTable(
                name: "master_company_entity_feature",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    entity_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    feature_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    sap_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    multiterm_id = table.Column<decimal>(type: "numeric(9,0)", nullable: true),
                    description = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    commercial = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    dependent_on = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    enabled = table.Column<decimal>(type: "numeric(1,0)", nullable: true, defaultValue: 1m),
                    valid_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    valid_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_company_entity_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_mastercompanyentityfeature_masterauthorization",
                        column: x => x.feature_id,
                        principalTable: "master_authorization",
                        principalColumn: "auth_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mastercompanyentityfeature_mastercompanyentity",
                        column: x => x.entity_id,
                        principalTable: "master_company_entity",
                        principalColumn: "entity_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "master_role",
                columns: table => new
                {
                    role_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    role_desc = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    role_shdesc = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    disabled = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    sub_role_of = table.Column<decimal>(type: "numeric(9,0)", nullable: true),
                    scope_level = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    entity_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    user_type = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_role", x => x.role_id);
                    table.ForeignKey(
                        name: "fk_masrol_mascoment",
                        column: x => x.entity_id,
                        principalTable: "master_company_entity",
                        principalColumn: "entity_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_masrol_masrol",
                        column: x => x.sub_role_of,
                        principalTable: "master_role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "master_user",
                columns: table => new
                {
                    user_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    login_name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    user_type = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    last_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    first_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    personal_number = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    abbreviation = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    reset_token = table.Column<string>(type: "text", nullable: true),
                    disabled = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    locked_yn = table.Column<decimal>(type: "numeric(2,0)", nullable: true),
                    osuser_yn = table.Column<decimal>(type: "numeric(2,0)", nullable: true),
                    admin_yn = table.Column<decimal>(type: "numeric(2,0)", nullable: true),
                    password = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    email = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    login_failed = table.Column<decimal>(type: "numeric(2,0)", nullable: true),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_pwd_change = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    pwd_change_req = table.Column<decimal>(type: "numeric(2,0)", nullable: true),
                    entity_id = table.Column<decimal>(type: "numeric(9,0)", nullable: true),
                    graphic_file_ref = table.Column<decimal>(type: "numeric(9,0)", nullable: true),
                    pw_algorithm = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    date_restriction = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    date_restriction_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_restriction_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hw_key = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    external_directory_user = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    external_directory_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_user", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_masuse_mascoment",
                        column: x => x.entity_id,
                        principalTable: "master_company_entity",
                        principalColumn: "entity_id");
                });

            migrationBuilder.CreateTable(
                name: "master_role_ad_groups",
                columns: table => new
                {
                    role_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    azure_ad_group_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    azure_ad_group_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_role_ad_groups", x => new { x.role_id, x.azure_ad_group_id });
                    table.ForeignKey(
                        name: "FK_master_role_ad_groups_master_role_role_id",
                        column: x => x.role_id,
                        principalTable: "master_role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "master_role_auth_assgnmt",
                columns: table => new
                {
                    role_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    auth_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    fat_client = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    web = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    mobile = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    third_party = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_role_auth_assgnmt", x => new { x.role_id, x.auth_id });
                    table.ForeignKey(
                        name: "fk_masrolautass_masaut",
                        column: x => x.auth_id,
                        principalTable: "master_authorization",
                        principalColumn: "auth_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_masrolautass_masrol",
                        column: x => x.role_id,
                        principalTable: "master_role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "master_role_setting",
                columns: table => new
                {
                    role_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    key_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_role_setting", x => new { x.role_id, x.key_name });
                    table.ForeignKey(
                        name: "FK_master_role_setting_master_role_role_id",
                        column: x => x.role_id,
                        principalTable: "master_role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "master_user_job_title",
                columns: table => new
                {
                    user_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    job_title_seq = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    job_title_id = table.Column<decimal>(type: "numeric(3,0)", nullable: false),
                    from_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    to_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_user_job_title", x => new { x.user_id, x.job_title_seq });
                    table.ForeignKey(
                        name: "fk_masusejobtit_masjobtit",
                        column: x => x.job_title_id,
                        principalTable: "master_job_title",
                        principalColumn: "job_title_id");
                    table.ForeignKey(
                        name: "fk_masusejobtit_masuse",
                        column: x => x.user_id,
                        principalTable: "master_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "master_user_roles",
                columns: table => new
                {
                    user_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    role_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_masuserol_masrol",
                        column: x => x.role_id,
                        principalTable: "master_role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_masuserol_masuse",
                        column: x => x.user_id,
                        principalTable: "master_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "master_user_setting",
                columns: table => new
                {
                    user_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    key_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_user_setting", x => new { x.user_id, x.key_name });
                    table.ForeignKey(
                        name: "FK_master_user_setting_master_user_user_id",
                        column: x => x.user_id,
                        principalTable: "master_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "master_user_token",
                columns: table => new
                {
                    token = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    token_type = table.Column<decimal>(type: "numeric(2,0)", nullable: true),
                    token_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    token_last_used = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    token_deactivated = table.Column<decimal>(type: "numeric(1,0)", nullable: true),
                    workstation_user_id = table.Column<decimal>(type: "numeric(9,0)", nullable: true),
                    deactivated_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_user_token", x => x.token);
                    table.ForeignKey(
                        name: "fk_masusetok_masuse",
                        column: x => x.user_id,
                        principalTable: "master_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_masusetok_masuse2",
                        column: x => x.workstation_user_id,
                        principalTable: "master_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaidParkings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    AmountPaid = table.Column<double>(type: "double precision", nullable: false),
                    Settled = table.Column<bool>(type: "boolean", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaidParkings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaidParkings_master_user_UserId",
                        column: x => x.UserId,
                        principalTable: "master_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    CardId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Latitude = table.Column<string>(type: "text", nullable: true),
                    Longitude = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCards_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCards_master_user_UserId",
                        column: x => x.UserId,
                        principalTable: "master_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<decimal>(type: "numeric(9,0)", nullable: false),
                    CarNumber = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCars_master_user_UserId",
                        column: x => x.UserId,
                        principalTable: "master_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_master_application_details_entity_id",
                table: "master_application_details",
                column: "entity_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_application_details_url",
                table: "master_application_details",
                column: "url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mascoment_mascoment_idx",
                table: "master_company_entity",
                column: "sub_entity_of");

            migrationBuilder.CreateIndex(
                name: "mascoment_masuse_idx",
                table: "master_company_entity",
                column: "assigned_user");

            migrationBuilder.CreateIndex(
                name: "IX_master_company_entity_feature_entity_id",
                table: "master_company_entity_feature",
                column: "entity_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_company_entity_feature_feature_id",
                table: "master_company_entity_feature",
                column: "feature_id");

            migrationBuilder.CreateIndex(
                name: "masrol_mascoment_idx",
                table: "master_role",
                column: "entity_id");

            migrationBuilder.CreateIndex(
                name: "masrol_masrol_idx",
                table: "master_role",
                column: "sub_role_of");

            migrationBuilder.CreateIndex(
                name: "IX_master_role_auth_assgnmt_auth_id",
                table: "master_role_auth_assgnmt",
                column: "auth_id");

            migrationBuilder.CreateIndex(
                name: "masuse_mascoment_idx",
                table: "master_user",
                column: "entity_id");

            migrationBuilder.CreateIndex(
                name: "uk_master_user_login_name",
                table: "master_user",
                column: "login_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "masusejobtit_masjobtit_idx",
                table: "master_user_job_title",
                column: "job_title_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_user_roles_role_id",
                table: "master_user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "masusetok_masuse_idx",
                table: "master_user_token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "masusetok_masuse2_idx",
                table: "master_user_token",
                column: "workstation_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_PaidParkings_UserId",
                table: "PaidParkings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCards_CardId",
                table: "UserCards",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCards_UserId",
                table: "UserCards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_CarNumber",
                table: "UserCars",
                column: "CarNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_UserId",
                table: "UserCars",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_master_application_details_master_company_entity_entity_id",
                table: "master_application_details",
                column: "entity_id",
                principalTable: "master_company_entity",
                principalColumn: "entity_id");

            migrationBuilder.AddForeignKey(
                name: "fk_mascoment_masuse",
                table: "master_company_entity",
                column: "assigned_user",
                principalTable: "master_user",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_masuse_mascoment",
                table: "master_user");

            migrationBuilder.DropTable(
                name: "master_application_details");

            migrationBuilder.DropTable(
                name: "master_company_entity_feature");

            migrationBuilder.DropTable(
                name: "master_role_ad_groups");

            migrationBuilder.DropTable(
                name: "master_role_auth_assgnmt");

            migrationBuilder.DropTable(
                name: "master_role_setting");

            migrationBuilder.DropTable(
                name: "master_user_job_title");

            migrationBuilder.DropTable(
                name: "master_user_roles");

            migrationBuilder.DropTable(
                name: "master_user_setting");

            migrationBuilder.DropTable(
                name: "master_user_token");

            migrationBuilder.DropTable(
                name: "PaidParkings");

            migrationBuilder.DropTable(
                name: "UserCards");

            migrationBuilder.DropTable(
                name: "UserCars");

            migrationBuilder.DropTable(
                name: "master_authorization");

            migrationBuilder.DropTable(
                name: "master_job_title");

            migrationBuilder.DropTable(
                name: "master_role");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "master_company_entity");

            migrationBuilder.DropTable(
                name: "master_user");
        }
    }
}
