using EmployeeSchedule.Api.Modules.ScheduleModule.Dtos;
using EmployeeSchedule.Api.Modules.ScheduleModule.Services.Contract;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSchedule.Api.Controllers;

[ApiController]
[Route("api/schedules")]
[Produces("application/json")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ScheduleResponseDto>>> GetAll() // endpoint para obter todos os agendamentos
    {
        var schedules = await _scheduleService.GetAllAsync();

        return Ok(schedules);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScheduleResponseDto>> GetById(int id) // endpoint para obter um agendamento por ID
    {
        var schedule = await _scheduleService.GetByIdAsync(id);

        if (schedule is null)
        {
            return NotFound(new { message = "Schedule nao encontrado." });
        }

        return Ok(schedule);
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleResponseDto>> Create([FromBody] CreateScheduleDto dto) // endpoint para criar um novo agendamento
    {
        try
        {
            var createdSchedule = await _scheduleService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdSchedule.Id },
                createdSchedule
            );
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ScheduleResponseDto>> Update(int id, [FromBody] UpdateScheduleDto dto) // endpoint para atualizar um agendamento existente

    {
        try
        {
            var updatedSchedule = await _scheduleService.UpdateAsync(id, dto);

            if (updatedSchedule is null)
            {
                return NotFound(new { message = "Schedule nao encontrado." });
            }

            return Ok(updatedSchedule);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) // endpoint para excluir um agendamento por ID
    {
        var deleted = await _scheduleService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = "Schedule nao encontrado." });
        }

        return NoContent();
    }
}
