using EmployeeSchedule.Api.Modules.EmployeeModule.Entity;

namespace EmployeeSchedule.Api.Modules.EmployeeModule.Repository.Contract;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync();

    Task<Employee?> GetByIdAsync(int id);

    Task<Employee?> GetByRegistrationAsync(string registration);

    Task<Employee> CreateAsync(Employee employee);

    Task UpdateAsync(Employee employee);

    Task DeleteAsync(Employee employee);
}