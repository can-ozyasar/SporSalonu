using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OZ_SporSalonu.Migrations
{
    /// <inheritdoc />
    public partial class AddSalonWorkingHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "AcilisSaati",
                table: "Salonlar",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "KapanisSaati",
                table: "Salonlar",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcilisSaati",
                table: "Salonlar");

            migrationBuilder.DropColumn(
                name: "KapanisSaati",
                table: "Salonlar");
        }
    }
}
