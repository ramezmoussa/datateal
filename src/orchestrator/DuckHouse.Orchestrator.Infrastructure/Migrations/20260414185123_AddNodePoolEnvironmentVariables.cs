using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckHouse.Orchestrator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNodePoolEnvironmentVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnvironmentVariableIds",
                table: "NodePoolConfigs",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretIds",
                table: "NodePoolConfigs",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnvironmentVariableIds",
                table: "NodePoolConfigs");

            migrationBuilder.DropColumn(
                name: "SecretIds",
                table: "NodePoolConfigs");
        }
    }
}
