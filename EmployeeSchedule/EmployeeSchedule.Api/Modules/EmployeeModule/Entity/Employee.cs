using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;
using EmployeeSchedule.Api.Modules.TaskModule.Entity;

namespace EmployeeSchedule.Api.Modules.EmployeeModule.Entity;

public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Registration { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public List<Schedule> Schedules { get; set; } = [];

    public List<TaskItem> Tasks { get; set; } = [];
}
