using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Lisec.ParkingApp.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class AddColumn_Cards_ParkingApp_postgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresOn",
                table: "Cards",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresOn",
                table: "Cards");
        }
    }
}
