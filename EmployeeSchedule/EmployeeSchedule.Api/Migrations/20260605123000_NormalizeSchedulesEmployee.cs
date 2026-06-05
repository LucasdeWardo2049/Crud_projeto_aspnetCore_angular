using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeSchedule.Api.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeSchedulesEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO employees (name, registration, department, is_active, created_at)
                SELECT DISTINCT ON (employee_registration)
                    employee_name,
                    employee_registration,
                    department,
                    TRUE,
                    NOW()
                FROM schedules
                WHERE employee_id IS NULL
                ON CONFLICT (registration) DO UPDATE
                SET
                    name = EXCLUDED.name,
                    department = EXCLUDED.department;

                UPDATE schedules AS schedule
                SET employee_id = employee.id
                FROM employees AS employee
                WHERE schedule.employee_id IS NULL
                  AND schedule.employee_registration = employee.registration;
                """
            );

            migrationBuilder.DropForeignKey(
                name: "FK_schedules_employees_employee_id",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "department",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "employee_name",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "employee_registration",
                table: "schedules");

            migrationBuilder.AlterColumn<int>(
                name: "employee_id",
                table: "schedules",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_schedules_employees_employee_id",
                table: "schedules",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedules_employees_employee_id",
                table: "schedules");

            migrationBuilder.AlterColumn<int>(
                name: "employee_id",
                table: "schedules",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "department",
                table: "schedules",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "employee_name",
                table: "schedules",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "employee_registration",
                table: "schedules",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE schedules AS schedule
                SET
                    employee_name = employee.name,
                    employee_registration = employee.registration,
                    department = employee.department
                FROM employees AS employee
                WHERE schedule.employee_id = employee.id;
                """
            );

            migrationBuilder.AddForeignKey(
                name: "FK_schedules_employees_employee_id",
                table: "schedules",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
