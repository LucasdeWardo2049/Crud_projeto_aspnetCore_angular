using EmployeeSchedule.Api.Context;
using EmployeeSchedule.Api.Modules.EmployeeModule.Entity;
using EmployeeSchedule.Api.Modules.EmployeeModule.Repository.Contract;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSchedule.Api.Modules.EmployeeModule.Repository.Implementation;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .AsNoTracking()
            .OrderBy(employee => employee.Name)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(employee => employee.Id == id);
    }

    public async Task<Employee?> GetByRegistrationAsync(string registration)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(employee => employee.Registration == registration);
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}