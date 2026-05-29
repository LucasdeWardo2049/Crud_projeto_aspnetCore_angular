using EmployeeSchedule.Api.Context;
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;
using EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Contract;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Implementation;

public class ScheduleRepository : IScheduleRepository
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
            .OrderBy(schedule => schedule.WorkDate)
            .ThenBy(schedule => schedule.StartTime)
            .ToListAsync();
    }

    public async Task<Schedule?> GetByIdAsync(int id)
    {
        return await _context.Schedules
            .AsNoTracking()
            .FirstOrDefaultAsync(schedule => schedule.Id == id);
    }

    public async Task<Schedule?> GetTrackedByIdAsync(int id)
    {
        return await _context.Schedules
            .FirstOrDefaultAsync(schedule => schedule.Id == id);
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
