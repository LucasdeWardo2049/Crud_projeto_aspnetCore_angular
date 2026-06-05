using EmployeeSchedule.Api.Modules.EmployeeModule.Dtos;
using EmployeeSchedule.Api.Modules.EmployeeModule.Entity;
using EmployeeSchedule.Api.Modules.EmployeeModule.Repository.Contract;
using EmployeeSchedule.Api.Modules.EmployeeModule.Services.Contract;

namespace EmployeeSchedule.Api.Modules.EmployeeModule.Services.Implementation;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<List<EmployeeResponseDTO>> GetAllAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();

        return employees.Select(MapToResponse).ToList();
    }

    public async Task<EmployeeResponseDTO?> GetByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        return employee is null ? null : MapToResponse(employee);
    }

    public async Task<EmployeeResponseDTO> CreateAsync(CreateEmployeeDTO dto)
    {
        var registration = dto.Registration.Trim();
        var existingEmployee = await _employeeRepository.GetByRegistrationAsync(registration);

        if (existingEmployee is not null)
        {
            throw new ArgumentException("registration ja esta cadastrada.");
        }

        var employee = new Employee
        {
            Name = dto.Name.Trim(),
            Registration = registration,
            Department = dto.Department.Trim(),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var createdEmployee = await _employeeRepository.CreateAsync(employee);

        return MapToResponse(createdEmployee);
    }

    public async Task<EmployeeResponseDTO?> UpdateAsync(int id, UpdateEmployeeDTO dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee is null)
        {
            return null;
        }

        var registration = dto.Registration.Trim();
        var existingEmployee = await _employeeRepository.GetByRegistrationAsync(registration);

        if (existingEmployee is not null && existingEmployee.Id != id)
        {
            throw new ArgumentException("registration ja esta cadastrada.");
        }

        employee.Name = dto.Name.Trim();
        employee.Registration = registration;
        employee.Department = dto.Department.Trim();
        employee.IsActive = dto.IsActive;

        await _employeeRepository.UpdateAsync(employee);

        return MapToResponse(employee);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee is null)
        {
            return false;
        }

        await _employeeRepository.DeleteAsync(employee);

        return true;
    }

    private static EmployeeResponseDTO MapToResponse(Employee employee)
    {
        return new EmployeeResponseDTO
        {
            Id = employee.Id,
            Name = employee.Name,
            Registration = employee.Registration,
            Department = employee.Department,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt
        };
    }
}