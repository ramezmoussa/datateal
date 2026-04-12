using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckHouse.Orchestrator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDependsOnTaskDeleteToCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskDependencies_JobTasks_DependsOnTaskId",
                table: "TaskDependencies");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDependencies_JobTasks_DependsOnTaskId",
                table: "TaskDependencies",
                column: "DependsOnTaskId",
                principalTable: "JobTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskDependencies_JobTasks_DependsOnTaskId",
                table: "TaskDependencies");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDependencies_JobTasks_DependsOnTaskId",
                table: "TaskDependencies",
                column: "DependsOnTaskId",
                principalTable: "JobTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
