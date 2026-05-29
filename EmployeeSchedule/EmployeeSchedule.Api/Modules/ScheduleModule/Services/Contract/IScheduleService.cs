using EmployeeSchedule.Api.Modules.ScheduleModule.Dtos;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Services.Contract;

public interface IScheduleService
{
    Task<List<ScheduleResponseDto>> GetAllAsync();

    Task<ScheduleResponseDto?> GetByIdAsync(int id);

    Task<ScheduleResponseDto> CreateAsync(CreateScheduleDto dto);

    Task<ScheduleResponseDto?> UpdateAsync(int id, UpdateScheduleDto dto);

    Task<bool> DeleteAsync(int id);
}
