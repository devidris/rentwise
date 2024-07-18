using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentWise.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedBoostTOPremium : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BoostExpiry",
                table: "Products",
                newName: "PremiumExpiry");

            migrationBuilder.RenameColumn(
                name: "Boost",
                table: "Products",
                newName: "Premium");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PremiumExpiry",
                table: "Products",
                newName: "BoostExpiry");

            migrationBuilder.RenameColumn(
                name: "Premium",
                table: "Products",
                newName: "Boost");
        }
    }
}
