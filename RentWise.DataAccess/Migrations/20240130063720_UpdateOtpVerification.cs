using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentWise.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOtpVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpVerifications",
                table: "OtpVerifications");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OtpVerifications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "OtpVerifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpVerifications",
                table: "OtpVerifications",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpVerifications",
                table: "OtpVerifications");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "OtpVerifications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OtpVerifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpVerifications",
                table: "OtpVerifications",
                column: "Value");
        }
    }
}
