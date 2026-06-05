using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeSchedule.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTasksToEmployeeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_schedules_schedule_id",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_schedule_id",
                table: "tasks");

            migrationBuilder.AddColumn<int>(
                name: "employee_id",
                table: "tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE tasks
                SET employee_id = schedules.employee_id
                FROM schedules
                WHERE schedules.id = tasks.schedule_id;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "employee_id",
                table: "tasks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "schedule_id",
                table: "tasks");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_employee_id",
                table: "tasks",
                column: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_employees_employee_id",
                table: "tasks",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_employees_employee_id",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_employee_id",
                table: "tasks");

            migrationBuilder.AddColumn<int>(
                name: "schedule_id",
                table: "tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "employee_id",
                table: "tasks");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_schedule_id",
                table: "tasks",
                column: "schedule_id");
        }
    }
}
