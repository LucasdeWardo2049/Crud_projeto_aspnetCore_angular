using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Contract;

public interface IScheduleRepository
{
    Task<List<Schedule>> GetAllAsync();

    Task<Schedule?> GetByIdAsync(int id);

    Task<Schedule?> GetTrackedByIdAsync(int id);

    Task<Schedule> CreateAsync(Schedule schedule);

    Task UpdateAsync(Schedule schedule);

    Task DeleteAsync(Schedule schedule);
}
