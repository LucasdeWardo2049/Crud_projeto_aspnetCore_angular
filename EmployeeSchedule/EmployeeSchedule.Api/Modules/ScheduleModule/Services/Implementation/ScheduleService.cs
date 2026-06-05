using System.Globalization;
using EmployeeSchedule.Api.Modules.EmployeeModule.Repository.Contract;
using EmployeeSchedule.Api.Modules.ScheduleModule.Dtos;
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;
using EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Contract;
using EmployeeSchedule.Api.Modules.ScheduleModule.Services.Contract;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Services.Implementation;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public ScheduleService(
        IScheduleRepository scheduleRepository,
        IEmployeeRepository employeeRepository)
    {
        _scheduleRepository = scheduleRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<List<ScheduleResponseDto>> GetAllAsync()
    {
        var schedules = await _scheduleRepository.GetAllAsync();

        return schedules
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ScheduleResponseDto?> GetByIdAsync(int id)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id);

        return schedule is null ? null : MapToResponse(schedule);
    }

    public async Task<ScheduleResponseDto> CreateAsync(CreateScheduleDto dto)
    {
        var parsedData = ValidateAndParse(
            dto.ShiftName,
            dto.StartTime,
            dto.EndTime,
            dto.WorkDate,
            dto.Status
        );

        var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);

        if (employee is null)
        {
            throw new ArgumentException("employeeId invalido.");
        }

        await EnsureEmployeeHasNoScheduleOnDateAsync(dto.EmployeeId, parsedData.WorkDate);

        var schedule = new Schedule
        {
            EmployeeId = dto.EmployeeId,
            Employee = employee,
            ShiftName = dto.ShiftName.Trim(),
            StartTime = parsedData.StartTime,
            EndTime = parsedData.EndTime,
            WorkDate = parsedData.WorkDate,
            Status = parsedData.Status,
            Notes = NormalizeOptionalText(dto.Notes),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        var createdSchedule = await _scheduleRepository.CreateAsync(schedule);

        return MapToResponse(createdSchedule);
    }

    public async Task<ScheduleResponseDto?> UpdateAsync(int id, UpdateScheduleDto dto)
    {
        var existingSchedule = await _scheduleRepository.GetTrackedByIdAsync(id);

        if (existingSchedule is null)
        {
            return null;
        }

        var parsedData = ValidateAndParse(
            dto.ShiftName,
            dto.StartTime,
            dto.EndTime,
            dto.WorkDate,
            dto.Status
        );

        var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);

        if (employee is null)
        {
            throw new ArgumentException("employeeId invalido.");
        }

        await EnsureEmployeeHasNoScheduleOnDateAsync(dto.EmployeeId, parsedData.WorkDate, id);

        existingSchedule.EmployeeId = dto.EmployeeId;
        existingSchedule.Employee = employee;
        existingSchedule.ShiftName = dto.ShiftName.Trim();
        existingSchedule.StartTime = parsedData.StartTime;
        existingSchedule.EndTime = parsedData.EndTime;
        existingSchedule.WorkDate = parsedData.WorkDate;
        existingSchedule.Status = parsedData.Status;
        existingSchedule.Notes = NormalizeOptionalText(dto.Notes);
        existingSchedule.UpdatedAt = DateTime.UtcNow;

        await _scheduleRepository.UpdateAsync(existingSchedule);

        return MapToResponse(existingSchedule);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var schedule = await _scheduleRepository.GetTrackedByIdAsync(id);

        if (schedule is null)
        {
            return false;
        }

        await _scheduleRepository.DeleteAsync(schedule);

        return true;
    }

    private static (TimeOnly StartTime, TimeOnly EndTime, DateOnly WorkDate, ScheduleStatus Status) ValidateAndParse(
        string shiftName,
        string startTimeValue,
        string endTimeValue,
        string workDateValue,
        ScheduleStatus? status)
    {
        EnsureRequired(shiftName, "shiftName");
        EnsureRequired(startTimeValue, "startTime");
        EnsureRequired(endTimeValue, "endTime");
        EnsureRequired(workDateValue, "workDate");

        if (status is null)
        {
            throw new ArgumentException("status e obrigatorio.");
        }

        if (!TimeOnly.TryParse(startTimeValue.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var startTime))
        {
            throw new ArgumentException("startTime deve estar em um formato valido. Exemplo: 08:00 ou 08:00:00.");
        }

        if (!TimeOnly.TryParse(endTimeValue.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var endTime))
        {
            throw new ArgumentException("endTime deve estar em um formato valido. Exemplo: 17:00 ou 17:00:00.");
        }

        if (!DateOnly.TryParseExact(workDateValue.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var workDate))
        {
            throw new ArgumentException("workDate deve estar no formato yyyy-MM-dd. Exemplo: 2026-05-28.");
        }

        if (endTime == startTime)
        {
            throw new ArgumentException("endTime nao pode ser igual a startTime.");
        }

        return (startTime, endTime, workDate, status.Value);
    }

    private static void EnsureRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} e obrigatorio.");
        }
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private async Task EnsureEmployeeHasNoScheduleOnDateAsync(
        int employeeId,
        DateOnly workDate,
        int? ignoredScheduleId = null)
    {
        var employeeHasScheduleOnDate = await _scheduleRepository.EmployeeHasScheduleOnDateAsync(
            employeeId,
            workDate,
            ignoredScheduleId);

        if (employeeHasScheduleOnDate)
        {
            throw new ArgumentException("O mesmo funcionario nao pode ter duas escalas na mesma data.");
        }
    }

    private static ScheduleResponseDto MapToResponse(Schedule schedule)
    {
        var employee = schedule.Employee;

        return new ScheduleResponseDto
        {
            Id = schedule.Id,
            EmployeeId = schedule.EmployeeId,
            EmployeeName = employee?.Name ?? string.Empty,
            EmployeeRegistration = employee?.Registration ?? string.Empty,
            Department = employee?.Department ?? string.Empty,
            ShiftName = schedule.ShiftName,
            StartTime = schedule.StartTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            EndTime = schedule.EndTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            WorkDate = schedule.WorkDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Status = schedule.Status,
            Notes = schedule.Notes,
            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt
        };
    }
}
