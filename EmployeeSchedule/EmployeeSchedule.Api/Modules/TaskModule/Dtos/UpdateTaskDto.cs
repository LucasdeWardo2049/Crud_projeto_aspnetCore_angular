using System.ComponentModel.DataAnnotations;

namespace EmployeeSchedule.Api.Modules.TaskModule.Dtos;

public class UpdateTaskDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsDone { get; set; }
}