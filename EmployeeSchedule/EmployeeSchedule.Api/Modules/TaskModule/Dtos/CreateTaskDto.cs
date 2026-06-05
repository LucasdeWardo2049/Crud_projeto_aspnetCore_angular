using System.ComponentModel.DataAnnotations;

namespace EmployeeSchedule.Api.Modules.TaskModule.Dtos;

public class CreateTaskDto
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
