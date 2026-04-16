using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckHouse.Orchestrator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueJobTaskNamesIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobTasks_JobId",
                table: "JobTasks");

            migrationBuilder.CreateIndex(
                name: "IX_JobTasks_JobId_Name",
                table: "JobTasks",
                columns: new[] { "JobId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobTasks_JobId_Name",
                table: "JobTasks");

            migrationBuilder.CreateIndex(
                name: "IX_JobTasks_JobId",
                table: "JobTasks",
                column: "JobId");
        }
    }
}
