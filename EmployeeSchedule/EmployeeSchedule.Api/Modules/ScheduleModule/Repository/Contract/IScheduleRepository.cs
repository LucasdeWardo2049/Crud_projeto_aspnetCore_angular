using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Contract;

public interface IScheduleRepository  // contrato do repositório de agendamento, definindo os métodos para acessar os dados de agendamento no banco de dados
{
    Task<List<Schedule>> GetAllAsync();

    Task<Schedule?> GetByIdAsync(int id);

    Task<Schedule?> GetTrackedByIdAsync(int id);

    Task<bool> EmployeeHasScheduleOnDateAsync(int employeeId, DateOnly workDate, int? ignoredScheduleId = null);

    Task<Schedule> CreateAsync(Schedule schedule);

    Task UpdateAsync(Schedule schedule);

    Task DeleteAsync(Schedule schedule);
}
