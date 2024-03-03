using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentWise.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddOneSignalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OneSignalId",
                table: "UsersDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OneSignalId",
                table: "UsersDetails");
        }
    }
}
