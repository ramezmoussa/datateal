using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckHouse.Orchestrator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeRunJobAndTaskFksNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobRuns_Jobs_JobId",
                table: "JobRuns");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskRuns_JobTasks_TaskId",
                table: "TaskRuns");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "TaskRuns",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobId",
                table: "JobRuns",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRuns_Jobs_JobId",
                table: "JobRuns",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskRuns_JobTasks_TaskId",
                table: "TaskRuns",
                column: "TaskId",
                principalTable: "JobTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobRuns_Jobs_JobId",
                table: "JobRuns");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskRuns_JobTasks_TaskId",
                table: "TaskRuns");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "TaskRuns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "JobId",
                table: "JobRuns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobRuns_Jobs_JobId",
                table: "JobRuns",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskRuns_JobTasks_TaskId",
                table: "TaskRuns",
                column: "TaskId",
                principalTable: "JobTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
