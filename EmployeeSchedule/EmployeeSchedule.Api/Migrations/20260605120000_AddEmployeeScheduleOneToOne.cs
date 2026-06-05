using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeSchedule.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeScheduleOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "employee_id",
                table: "schedules",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_schedules_employee_id",
                table: "schedules",
                column: "employee_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_schedules_employees_employee_id",
                table: "schedules",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedules_employees_employee_id",
                table: "schedules");

            migrationBuilder.DropIndex(
                name: "IX_schedules_employee_id",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "employee_id",
                table: "schedules");
        }
    }
}
