using System.ComponentModel.DataAnnotations;
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Dtos;

public class CreateScheduleDto
{

    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ShiftName { get; set; } = string.Empty;

    [Required]
    public string StartTime { get; set; } = string.Empty;

    [Required]
    public string EndTime { get; set; } = string.Empty;

    [Required]
    public string WorkDate { get; set; } = string.Empty;

    [Required]
    public ScheduleStatus? Status { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
