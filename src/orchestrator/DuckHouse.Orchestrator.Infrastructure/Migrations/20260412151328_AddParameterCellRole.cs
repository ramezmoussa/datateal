using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckHouse.Orchestrator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParameterCellRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CellRole",
                table: "TaskRunCellOutputs",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CellRole",
                table: "TaskRunCellOutputs");
        }
    }
}
