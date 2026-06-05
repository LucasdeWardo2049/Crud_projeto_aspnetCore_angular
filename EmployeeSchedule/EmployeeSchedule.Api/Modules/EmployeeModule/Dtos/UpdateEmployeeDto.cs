using System.ComponentModel.DataAnnotations;

namespace EmployeeSchedule.Api.Modules.EmployeeModule.Dtos
{
    public class UpdateEmployeeDTO
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Registration { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}