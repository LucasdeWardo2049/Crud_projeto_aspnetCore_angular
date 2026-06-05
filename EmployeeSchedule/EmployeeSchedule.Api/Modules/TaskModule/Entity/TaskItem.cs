using EmployeeSchedule.Api.Modules.EmployeeModule.Entity;

namespace EmployeeSchedule.Api.Modules.TaskModule.Entity;

public class TaskItem
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsDone { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}
