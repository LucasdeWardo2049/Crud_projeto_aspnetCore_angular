using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Dtos;

public class ScheduleResponseDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string EmployeeRegistration { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string ShiftName { get; set; } = string.Empty;

    public string StartTime { get; set; } = string.Empty;

    public string EndTime { get; set; } = string.Empty;

    public string WorkDate { get; set; } = string.Empty;

    public ScheduleStatus Status { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
