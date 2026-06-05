using EmployeeSchedule.Api.Modules.EmployeeModule.Dtos;

namespace EmployeeSchedule.Api.Modules.EmployeeModule.Services.Contract
{
    public interface IEmployeeService
    {
       Task<List<EmployeeResponseDTO>> GetAllAsync();

        Task<EmployeeResponseDTO?> GetByIdAsync(int id);

        Task<EmployeeResponseDTO> CreateAsync(CreateEmployeeDTO dto);

        Task<EmployeeResponseDTO?> UpdateAsync(int id, UpdateEmployeeDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}