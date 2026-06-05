using EmployeeSchedule.Api.Context;
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;
using EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Contract;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Implementation;

public class ScheduleRepository : IScheduleRepository // implementação do repositório de agendamento usando Entity Framework Core
{
    private readonly AppDbContext _context;

    public ScheduleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Schedule>> GetAllAsync()
    {
        return await _context.Schedules
            .AsNoTracking()
            .Include(schedule => schedule.Employee)
            .OrderBy(schedule => schedule.WorkDate)
            .ThenBy(schedule => schedule.StartTime)
            .ToListAsync();
    }

    public async Task<Schedule?> GetByIdAsync(int id)
    {
        return await _context.Schedules
            .AsNoTracking()
            .Include(schedule => schedule.Employee)
            .FirstOrDefaultAsync(schedule => schedule.Id == id);
    }

    public async Task<Schedule?> GetTrackedByIdAsync(int id)
    {
        return await _context.Schedules
            .Include(schedule => schedule.Employee)
            .FirstOrDefaultAsync(schedule => schedule.Id == id);
    }

    public async Task<bool> EmployeeHasScheduleOnDateAsync(int employeeId, DateOnly workDate, int? ignoredScheduleId = null)
    {
        var query = _context.Schedules
            .Where(schedule =>
                schedule.EmployeeId == employeeId &&
                schedule.WorkDate == workDate);

        if (ignoredScheduleId is not null)
        {
            query = query.Where(schedule => schedule.Id != ignoredScheduleId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Schedule> CreateAsync(Schedule schedule)
    {
        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        return schedule;
    }

    public async Task UpdateAsync(Schedule schedule)
    {
        _context.Schedules.Update(schedule);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Schedule schedule)
    {
        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();
    }
}
