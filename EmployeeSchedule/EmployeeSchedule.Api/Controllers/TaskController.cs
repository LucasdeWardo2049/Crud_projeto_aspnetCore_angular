using EmployeeSchedule.Api.Context;
using EmployeeSchedule.Api.Modules.TaskModule.Dtos;
using EmployeeSchedule.Api.Modules.TaskModule.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSchedule.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Produces("application/json")]
public class TaskController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskResponseDto>>> GetAll()
    {
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Include(task => task.Employee)
            .OrderBy(task => task.Id)
            .ToListAsync();

        return Ok(tasks.Select(MapToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(int id)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .Include(item => item.Employee)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (task is null)
        {
            return NotFound(new { message = "Task nao encontrada." });
        }

        return Ok(MapToResponse(task));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> Create(CreateTaskDto dto)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(item => item.Id == dto.EmployeeId);

        if (employee is null)
        {
            return BadRequest(new { message = "employeeId invalido." });
        }

        var task = new TaskItem
        {
            EmployeeId = dto.EmployeeId,
            Employee = employee,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, MapToResponse(task));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskResponseDto>> Update(int id, UpdateTaskDto dto)
    {
        var task = await _context.Tasks
            .Include(item => item.Employee)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (task is null)
        {
            return NotFound(new { message = "Task nao encontrada." });
        }

        task.Title = dto.Title.Trim();
        task.Description = dto.Description?.Trim();
        task.IsDone = dto.IsDone;
        task.CompletedAt = dto.IsDone ? DateTime.UtcNow : null;

        await _context.SaveChangesAsync();

        return Ok(MapToResponse(task));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(item => item.Id == id);

        if (task is null)
        {
            return NotFound(new { message = "Task nao encontrada." });
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static TaskResponseDto MapToResponse(TaskItem task)
    {
        var employee = task.Employee;

        return new TaskResponseDto
        {
            Id = task.Id,
            EmployeeId = task.EmployeeId,
            EmployeeName = employee?.Name ?? string.Empty,
            Department = employee?.Department ?? string.Empty,
            Title = task.Title,
            Description = task.Description,
            IsDone = task.IsDone,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt
        };
    }
}
