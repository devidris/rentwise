using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentWise.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDataFromAgentRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AgentRegistrations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AgentRegistrations_UserId",
                table: "AgentRegistrations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentRegistrations_AspNetUsers_UserId",
                table: "AgentRegistrations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentRegistrations_AspNetUsers_UserId",
                table: "AgentRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_AgentRegistrations_UserId",
                table: "AgentRegistrations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AgentRegistrations");
        }
    }
}
