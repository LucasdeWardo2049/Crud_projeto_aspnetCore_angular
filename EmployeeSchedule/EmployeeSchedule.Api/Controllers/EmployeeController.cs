using EmployeeSchedule.Api.Modules.EmployeeModule.Dtos;
using EmployeeSchedule.Api.Modules.EmployeeModule.Services.Contract;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSchedule.Api.Controllers;

[ApiController]
[Route("api/employees")]
[Produces("application/json")]

public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponseDTO>>> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();

        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponseDTO>> GetById(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);

        if (employee is null)
        {
            return NotFound(new { message = "Employee nao encontrado." });
        }

        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDTO>> Create(CreateEmployeeDTO dto)
    {
        try
        {
            var createdEmployee = await _employeeService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = createdEmployee.Id }, createdEmployee);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmployeeResponseDTO>> Update(int id, UpdateEmployeeDTO dto)
    {
        try
        {
            var updatedEmployee = await _employeeService.UpdateAsync(id, dto);

            if (updatedEmployee is null)
            {
                return NotFound(new { message = "Employee nao encontrado." });
            }

            return Ok(updatedEmployee);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _employeeService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = "Employee nao encontrado." });
        }

        return NoContent();
    }
}
