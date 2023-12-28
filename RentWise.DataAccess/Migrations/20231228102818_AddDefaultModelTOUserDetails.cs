using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentWise.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultModelTOUserDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UsersDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "UsersDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UsersDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UsersDetails");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "UsersDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UsersDetails");
        }
    }
}
