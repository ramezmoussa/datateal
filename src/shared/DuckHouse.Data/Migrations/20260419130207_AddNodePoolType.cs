using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckHouse.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNodePoolType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PoolType",
                table: "NodePoolConfigs",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE \"NodePoolConfigs\" SET \"PoolType\" = 'Job' WHERE \"PoolType\" = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PoolType",
                table: "NodePoolConfigs");
        }
    }
}
