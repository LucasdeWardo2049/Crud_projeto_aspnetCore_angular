using System.ComponentModel.DataAnnotations;
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Dtos;

public class UpdateScheduleDto
{
    [Required]
    [MaxLength(150)]
    public string EmployeeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string EmployeeRegistration { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

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
