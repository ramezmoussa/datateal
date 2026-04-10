using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckHouse.Orchestrator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobRunDenormalizedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaskName",
                table: "TaskRuns",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaskType",
                table: "TaskRuns",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobName",
                table: "JobRuns",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskName",
                table: "TaskRuns");

            migrationBuilder.DropColumn(
                name: "TaskType",
                table: "TaskRuns");

            migrationBuilder.DropColumn(
                name: "JobName",
                table: "JobRuns");
        }
    }
}
