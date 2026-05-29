namespace EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

public class Schedule
{
    public int Id { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string EmployeeRegistration { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string ShiftName { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public DateOnly WorkDate { get; set; }

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
