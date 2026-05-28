# 01 — Passo a passo do projeto Employee Schedule

Este documento guia a criação do projeto **Employee Schedule**, um MVP full-stack com apenas um CRUD: `Schedule`.

A ideia vem do escopo original do projeto: aprender Angular, ASP.NET Core Web API e PostgreSQL local, sem Docker, sem NGINX, sem autenticação, sem SignalR/Hub e sem múltiplos CRUDs.

## 1. Stack validada para esta versão

Para uma base atual, estável e simples, este guia usa:

- **Backend:** ASP.NET Core Web API com controllers.
- **Target Framework:** `.NET 10 LTS`.
- **ORM:** Entity Framework Core.
- **Provider PostgreSQL:** `Npgsql.EntityFrameworkCore.PostgreSQL`.
- **Documentação/teste da API:** Swagger UI via `Swashbuckle.AspNetCore`.
- **Banco:** PostgreSQL local.
- **Frontend:** Angular standalone com routing.
- **HTTP no Angular:** `HttpClient` via `provideHttpClient()`.
- **Formulários:** Reactive Forms.
- **IDE:** VS Code.
- **Padrões:** DTOs, Repository Pattern simples, Service Layer, interfaces para inversão de dependência e separação por feature.

> Observação: se sua máquina já está com `.NET 8 LTS`, os conceitos são os mesmos. Para este guia atualizado, a versão padrão é `.NET 10`, por ser LTS mais recente. Se preferir `.NET 8`, troque `net10.0` por `net8.0` e use versões 8.x dos pacotes.

## 2. Arquitetura geral

```txt
Angular Frontend
http://localhost:4200
        |
        | HTTP/JSON
        v
ASP.NET Core Web API
http://localhost:5000
        |
        | EF Core + Npgsql
        v
PostgreSQL local
localhost:5432
```

Fluxo de uma requisição:

```txt
Angular Component
  -> Angular ScheduleService
    -> HTTP /api/schedules
      -> ScheduleController
        -> IScheduleService / ScheduleService
          -> IScheduleRepository / ScheduleRepository
            -> AppDbContext
              -> PostgreSQL
```

Responsabilidades:

```txt
Controller
Recebe requisições HTTP e devolve respostas HTTP.

Service
Aplica regras de negócio e converte DTOs para entidades.

Repository
Centraliza acesso ao banco usando EF Core.

DTOs
Definem os dados de entrada e saída da API.

Entity
Representa a tabela schedules no banco.

DbContext
Configura o mapeamento C# -> PostgreSQL.

Config
Organiza DI, CORS e Swagger para manter Program.cs limpo.
```

A pasta `Hub` fica fora deste MVP porque não haverá SignalR. A pasta `core` do Angular fica reservada para interceptors, guards e serviços globais futuros, mas não precisa receber código agora.

## 3. Estrutura final de pastas

### 3.1. Estrutura geral

```txt
EmployeeSchedule
┣ EmployeeSchedule.sln
┣ EmployeeSchedule.Api
┗ employee-schedule-web
```

### 3.2. Backend

```txt
EmployeeSchedule.Api
┣ 📂 Config
┃ ┣ 📄 CorsConfig.cs
┃ ┣ 📄 DependencyInjectionConfig.cs
┃ ┗ 📄 SwaggerConfig.cs
┣ 📂 Context
┃ ┗ 📄 AppDbContext.cs
┣ 📂 Controllers
┃ ┗ 📄 ScheduleController.cs
┣ 📂 Migrations
┣ 📂 Modules
┃ ┗ 📂 ScheduleModule
┃   ┣ 📂 Dtos
┃   ┃ ┣ 📄 CreateScheduleDto.cs
┃   ┃ ┣ 📄 UpdateScheduleDto.cs
┃   ┃ ┗ 📄 ScheduleResponseDto.cs
┃   ┣ 📂 Entity
┃   ┃ ┣ 📄 Schedule.cs
┃   ┃ ┗ 📄 ScheduleStatus.cs
┃   ┣ 📂 Repository
┃   ┃ ┣ 📂 Contract
┃   ┃ ┃ ┗ 📄 IScheduleRepository.cs
┃   ┃ ┗ 📂 Implementation
┃   ┃   ┗ 📄 ScheduleRepository.cs
┃   ┗ 📂 Services
┃     ┣ 📂 Contract
┃     ┃ ┗ 📄 IScheduleService.cs
┃     ┗ 📂 Implementation
┃       ┗ 📄 ScheduleService.cs
┣ 📄 appsettings.json
┣ 📄 Program.cs
┗ 📄 EmployeeSchedule.Api.csproj
```

### 3.3. Frontend

```txt
employee-schedule-web
┣ 📂 src
┃ ┣ 📂 app
┃ ┃ ┣ 📂 core
┃ ┃ ┣ 📂 layouts
┃ ┃ ┃ ┗ 📂 main-layout
┃ ┃ ┃   ┣ 📄 main-layout.component.css
┃ ┃ ┃   ┣ 📄 main-layout.component.html
┃ ┃ ┃   ┗ 📄 main-layout.component.ts
┃ ┃ ┣ 📂 modules
┃ ┃ ┃ ┗ 📂 schedules
┃ ┃ ┃   ┣ 📂 models
┃ ┃ ┃   ┃ ┗ 📄 schedule.model.ts
┃ ┃ ┃   ┣ 📂 pages
┃ ┃ ┃   ┃ ┣ 📂 schedule-form
┃ ┃ ┃   ┃ ┃ ┣ 📄 schedule-form.component.css
┃ ┃ ┃   ┃ ┃ ┣ 📄 schedule-form.component.html
┃ ┃ ┃   ┃ ┃ ┗ 📄 schedule-form.component.ts
┃ ┃ ┃   ┃ ┗ 📂 schedule-list
┃ ┃ ┃   ┃   ┣ 📄 schedule-list.component.css
┃ ┃ ┃   ┃   ┣ 📄 schedule-list.component.html
┃ ┃ ┃   ┃   ┗ 📄 schedule-list.component.ts
┃ ┃ ┃   ┗ 📂 services
┃ ┃ ┃     ┗ 📄 schedule.service.ts
┃ ┃ ┣ 📂 shared
┃ ┃ ┣ 📄 app.component.html
┃ ┃ ┣ 📄 app.component.ts
┃ ┃ ┣ 📄 app.config.ts
┃ ┃ ┗ 📄 app.routes.ts
┃ ┣ 📂 assets
┃ ┣ 📂 environments
┃ ┃ ┣ 📄 environment.prod.ts
┃ ┃ ┗ 📄 environment.ts
┃ ┣ 📂 fonts
┃ ┣ 📄 main.ts
┃ ┗ 📄 styles.css
┗ 📄 angular.json
```

## 4. Criar solution e backend

Na pasta onde você guarda seus projetos:

```bash
mkdir EmployeeSchedule
cd EmployeeSchedule

dotnet new sln -n EmployeeSchedule

dotnet new webapi -n EmployeeSchedule.Api --use-controllers --framework net10.0

dotnet sln add EmployeeSchedule.Api/EmployeeSchedule.Api.csproj
```

Entre na API:

```bash
cd EmployeeSchedule.Api
```

Apague arquivos de exemplo, se o template criar algo como `WeatherForecast.cs` ou `WeatherForecastController.cs`.

## 5. Instalar pacotes do backend

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.8
dotnet add package Swashbuckle.AspNetCore --version 10.1.7
```

Instale ou atualize a ferramenta global do EF Core:

```bash
dotnet tool install --global dotnet-ef --version 10.0.8
```

Se já existir:

```bash
dotnet tool update --global dotnet-ef --version 10.0.8
```

Verifique:

```bash
dotnet ef --version
```

## 6. Criar banco PostgreSQL local

Opção com `createdb`:

```bash
createdb -h localhost -p 5432 -U postgres employee_schedule_db
```

Opção com `psql`:

```bash
psql -h localhost -p 5432 -U postgres
```

Dentro do `psql`:

```sql
CREATE DATABASE employee_schedule_db;
\l
\q
```

Neste guia, a connection string usa:

```txt
Host=localhost
Port=5432
Database=employee_schedule_db
Username=postgres
Password=postgres
```

Troque a senha conforme sua instalação local.

## 7. Criar pastas do backend

Dentro de `EmployeeSchedule.Api`:

```bash
mkdir Config
mkdir Context
mkdir Controllers
mkdir Modules
mkdir Modules/ScheduleModule
mkdir Modules/ScheduleModule/Dtos
mkdir Modules/ScheduleModule/Entity
mkdir Modules/ScheduleModule/Repository
mkdir Modules/ScheduleModule/Repository/Contract
mkdir Modules/ScheduleModule/Repository/Implementation
mkdir Modules/ScheduleModule/Services
mkdir Modules/ScheduleModule/Services/Contract
mkdir Modules/ScheduleModule/Services/Implementation
```

## 8. Código do backend

### 8.1. `Modules/ScheduleModule/Entity/ScheduleStatus.cs`

```csharp
namespace EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

public enum ScheduleStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    Absent = 4
}
```

### 8.2. `Modules/ScheduleModule/Entity/Schedule.cs`

```csharp
namespace EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

public class Schedule
{
    public int Id { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string EmployeeRegistration { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string ShiftName { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public DateOnly WorkDate { get; set; }

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
```

### 8.3. `Modules/ScheduleModule/Dtos/CreateScheduleDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Dtos;

public class CreateScheduleDto
{
    [Required]
    [MaxLength(150)]
    public string EmployeeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string EmployeeRegistration { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ShiftName { get; set; } = string.Empty;

    [Required]
    public string StartTime { get; set; } = string.Empty;

    [Required]
    public string EndTime { get; set; } = string.Empty;

    [Required]
    public string WorkDate { get; set; } = string.Empty;

    [Required]
    public ScheduleStatus? Status { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
```

### 8.4. `Modules/ScheduleModule/Dtos/UpdateScheduleDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Dtos;

public class UpdateScheduleDto
{
    [Required]
    [MaxLength(150)]
    public string EmployeeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string EmployeeRegistration { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ShiftName { get; set; } = string.Empty;

    [Required]
    public string StartTime { get; set; } = string.Empty;

    [Required]
    public string EndTime { get; set; } = string.Empty;

    [Required]
    public string WorkDate { get; set; } = string.Empty;

    [Required]
    public ScheduleStatus? Status { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
```

### 8.5. `Modules/ScheduleModule/Dtos/ScheduleResponseDto.cs`

```csharp
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Dtos;

public class ScheduleResponseDto
{
    public int Id { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string EmployeeRegistration { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string ShiftName { get; set; } = string.Empty;

    public string StartTime { get; set; } = string.Empty;

    public string EndTime { get; set; } = string.Empty;

    public string WorkDate { get; set; } = string.Empty;

    public ScheduleStatus Status { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
```

### 8.6. `Context/AppDbContext.cs`

```csharp
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSchedule.Api.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Schedule> Schedules => Set<Schedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("schedules");

            entity.HasKey(schedule => schedule.Id);

            entity.Property(schedule => schedule.Id)
                .HasColumnName("id");

            entity.Property(schedule => schedule.EmployeeName)
                .HasColumnName("employee_name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(schedule => schedule.EmployeeRegistration)
                .HasColumnName("employee_registration")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(schedule => schedule.Department)
                .HasColumnName("department")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(schedule => schedule.ShiftName)
                .HasColumnName("shift_name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(schedule => schedule.StartTime)
                .HasColumnName("start_time")
                .IsRequired();

            entity.Property(schedule => schedule.EndTime)
                .HasColumnName("end_time")
                .IsRequired();

            entity.Property(schedule => schedule.WorkDate)
                .HasColumnName("work_date")
                .IsRequired();

            entity.Property(schedule => schedule.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(schedule => schedule.Notes)
                .HasColumnName("notes")
                .HasMaxLength(500);

            entity.Property(schedule => schedule.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(schedule => schedule.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone");
        });
    }
}
```

### 8.7. `Modules/ScheduleModule/Repository/Contract/IScheduleRepository.cs`

```csharp
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
```

### 8.8. `Modules/ScheduleModule/Repository/Implementation/ScheduleRepository.cs`

```csharp
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
```

### 8.9. `Modules/ScheduleModule/Services/Contract/IScheduleService.cs`

```csharp
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
```

### 8.10. `Modules/ScheduleModule/Services/Implementation/ScheduleService.cs`

```csharp
using System.Globalization;
using EmployeeSchedule.Api.Modules.ScheduleModule.Dtos;
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;
using EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Contract;
using EmployeeSchedule.Api.Modules.ScheduleModule.Services.Contract;

namespace EmployeeSchedule.Api.Modules.ScheduleModule.Services.Implementation;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository;

    public ScheduleService(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
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
            dto.EmployeeName,
            dto.EmployeeRegistration,
            dto.Department,
            dto.ShiftName,
            dto.StartTime,
            dto.EndTime,
            dto.WorkDate,
            dto.Status
        );

        var now = DateTime.UtcNow;

        var schedule = new Schedule
        {
            EmployeeName = dto.EmployeeName.Trim(),
            EmployeeRegistration = dto.EmployeeRegistration.Trim(),
            Department = dto.Department.Trim(),
            ShiftName = dto.ShiftName.Trim(),
            StartTime = parsedData.StartTime,
            EndTime = parsedData.EndTime,
            WorkDate = parsedData.WorkDate,
            Status = parsedData.Status,
            Notes = NormalizeOptionalText(dto.Notes),
            CreatedAt = now,
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
            dto.EmployeeName,
            dto.EmployeeRegistration,
            dto.Department,
            dto.ShiftName,
            dto.StartTime,
            dto.EndTime,
            dto.WorkDate,
            dto.Status
        );

        existingSchedule.EmployeeName = dto.EmployeeName.Trim();
        existingSchedule.EmployeeRegistration = dto.EmployeeRegistration.Trim();
        existingSchedule.Department = dto.Department.Trim();
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
        string employeeName,
        string employeeRegistration,
        string department,
        string shiftName,
        string startTimeValue,
        string endTimeValue,
        string workDateValue,
        ScheduleStatus? status)
    {
        EnsureRequired(employeeName, "employeeName");
        EnsureRequired(employeeRegistration, "employeeRegistration");
        EnsureRequired(department, "department");
        EnsureRequired(shiftName, "shiftName");
        EnsureRequired(startTimeValue, "startTime");
        EnsureRequired(endTimeValue, "endTime");
        EnsureRequired(workDateValue, "workDate");

        if (status is null)
        {
            throw new ArgumentException("status é obrigatório.");
        }

        if (!TimeOnly.TryParse(startTimeValue.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var startTime))
        {
            throw new ArgumentException("startTime deve estar em um formato válido. Exemplo: 08:00 ou 08:00:00.");
        }

        if (!TimeOnly.TryParse(endTimeValue.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var endTime))
        {
            throw new ArgumentException("endTime deve estar em um formato válido. Exemplo: 17:00 ou 17:00:00.");
        }

        if (!DateOnly.TryParseExact(workDateValue.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var workDate))
        {
            throw new ArgumentException("workDate deve estar no formato yyyy-MM-dd. Exemplo: 2026-05-28.");
        }

        if (endTime == startTime)
        {
            throw new ArgumentException("endTime não pode ser igual a startTime.");
        }

        return (startTime, endTime, workDate, status.Value);
    }

    private static void EnsureRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} é obrigatório.");
        }
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static ScheduleResponseDto MapToResponse(Schedule schedule)
    {
        return new ScheduleResponseDto
        {
            Id = schedule.Id,
            EmployeeName = schedule.EmployeeName,
            EmployeeRegistration = schedule.EmployeeRegistration,
            Department = schedule.Department,
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
```

### 8.11. `Controllers/ScheduleController.cs`

```csharp
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
    public async Task<ActionResult<List<ScheduleResponseDto>>> GetAll()
    {
        var schedules = await _scheduleService.GetAllAsync();

        return Ok(schedules);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScheduleResponseDto>> GetById(int id)
    {
        var schedule = await _scheduleService.GetByIdAsync(id);

        if (schedule is null)
        {
            return NotFound(new { message = "Schedule não encontrado." });
        }

        return Ok(schedule);
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleResponseDto>> Create([FromBody] CreateScheduleDto dto)
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
    public async Task<ActionResult<ScheduleResponseDto>> Update(int id, [FromBody] UpdateScheduleDto dto)
    {
        try
        {
            var updatedSchedule = await _scheduleService.UpdateAsync(id, dto);

            if (updatedSchedule is null)
            {
                return NotFound(new { message = "Schedule não encontrado." });
            }

            return Ok(updatedSchedule);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _scheduleService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = "Schedule não encontrado." });
        }

        return NoContent();
    }
}
```

### 8.12. `Config/CorsConfig.cs`

```csharp
namespace EmployeeSchedule.Api.Config;

public static class CorsConfig
{
    public const string AllowAngularPolicy = "AllowAngularPolicy";

    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Frontend:AllowedOrigins")
            .Get<string[]>() ?? new[] { "http://localhost:4200" };

        services.AddCors(options =>
        {
            options.AddPolicy(AllowAngularPolicy, policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
```

### 8.13. `Config/DependencyInjectionConfig.cs`

```csharp
using EmployeeSchedule.Api.Context;
using EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Contract;
using EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Implementation;
using EmployeeSchedule.Api.Modules.ScheduleModule.Services.Contract;
using EmployeeSchedule.Api.Modules.ScheduleModule.Services.Implementation;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSchedule.Api.Config;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string DefaultConnection não foi configurada.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IScheduleService, ScheduleService>();

        return services;
    }
}
```

### 8.14. `Config/SwaggerConfig.cs`

```csharp
namespace EmployeeSchedule.Api.Config;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
```

### 8.15. `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=employee_schedule_db;Username=postgres;Password=postgres"
  },
  "Frontend": {
    "AllowedOrigins": [
      "http://localhost:4200"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 8.16. `Program.cs`

```csharp
using System.Text.Json.Serialization;
using EmployeeSchedule.Api.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddApiCors(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

app.UseSwaggerDocumentation();

app.UseRouting();
app.UseCors(CorsConfig.AllowAngularPolicy);
app.UseAuthorization();

app.MapControllers();

app.Run();
```

## 9. Criar migration e atualizar banco

Dentro de `EmployeeSchedule.Api`:

```bash
dotnet build

dotnet ef migrations add InitialCreate

dotnet ef database update
```

## 10. Rodar backend

```bash
dotnet run --urls "http://localhost:5000"
```

Acesse:

```txt
http://localhost:5000/swagger
```

Endpoints:

```txt
GET     /api/schedules
GET     /api/schedules/{id}
POST    /api/schedules
PUT     /api/schedules/{id}
DELETE  /api/schedules/{id}
```

## 11. Testar API no Swagger ou REST Client

Body para `POST /api/schedules`:

```json
{
  "employeeName": "Ana Souza",
  "employeeRegistration": "EMP001",
  "department": "Operations",
  "shiftName": "Morning Shift",
  "startTime": "08:00:00",
  "endTime": "17:00:00",
  "workDate": "2026-05-28",
  "status": "Scheduled",
  "notes": "Primeira escala cadastrada."
}
```

Arquivo opcional `requests.http` na raiz da solution:

```http
### Listar schedules
GET http://localhost:5000/api/schedules

### Buscar schedule por id
GET http://localhost:5000/api/schedules/1

### Criar schedule
POST http://localhost:5000/api/schedules
Content-Type: application/json

{
  "employeeName": "Ana Souza",
  "employeeRegistration": "EMP001",
  "department": "Operations",
  "shiftName": "Morning Shift",
  "startTime": "08:00:00",
  "endTime": "17:00:00",
  "workDate": "2026-05-28",
  "status": "Scheduled",
  "notes": "Criado pelo REST Client."
}

### Atualizar schedule
PUT http://localhost:5000/api/schedules/1
Content-Type: application/json

{
  "employeeName": "Ana Souza",
  "employeeRegistration": "EMP001",
  "department": "Operations",
  "shiftName": "Afternoon Shift",
  "startTime": "13:00:00",
  "endTime": "21:00:00",
  "workDate": "2026-05-29",
  "status": "Completed",
  "notes": "Atualizado pelo REST Client."
}

### Excluir schedule
DELETE http://localhost:5000/api/schedules/1
```

## 12. Criar projeto Angular

Volte para a pasta raiz `EmployeeSchedule`:

```bash
cd ..
```

Instale a CLI do Angular:

```bash
npm install -g @angular/cli
```

Crie o projeto:

```bash
ng new employee-schedule-web --routing --style css --standalone --strict --skip-tests --file-name-style-guide 2016 --ssr=false
```

Entre na pasta:

```bash
cd employee-schedule-web
```

Crie as pastas:

```bash
mkdir src/app/core
mkdir src/app/layouts
mkdir src/app/layouts/main-layout
mkdir src/app/modules
mkdir src/app/modules/schedules
mkdir src/app/modules/schedules/models
mkdir src/app/modules/schedules/services
mkdir src/app/modules/schedules/pages
mkdir src/app/modules/schedules/pages/schedule-list
mkdir src/app/modules/schedules/pages/schedule-form
mkdir src/app/shared
mkdir src/environments
mkdir src/fonts
```

## 13. Código do frontend

### 13.1. `src/environments/environment.ts`

```ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

### 13.2. `src/environments/environment.prod.ts`

```ts
export const environment = {
  production: true,
  apiUrl: 'http://localhost:5000/api'
};
```

> Para este MVP local, o Angular usa `environment.ts`. O `environment.prod.ts` fica preparado para uma configuração de build futura.

### 13.3. `src/app/app.config.ts`

```ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient()
  ]
};
```

### 13.4. `src/app/app.routes.ts`

```ts
import { Routes } from '@angular/router';

import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { ScheduleFormComponent } from './modules/schedules/pages/schedule-form/schedule-form.component';
import { ScheduleListComponent } from './modules/schedules/pages/schedule-list/schedule-list.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'schedules',
        pathMatch: 'full'
      },
      {
        path: 'schedules',
        component: ScheduleListComponent
      },
      {
        path: 'schedules/new',
        component: ScheduleFormComponent
      },
      {
        path: 'schedules/:id/edit',
        component: ScheduleFormComponent
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'schedules'
  }
];
```

### 13.5. `src/app/app.component.ts`

```ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html'
})
export class AppComponent {
}
```

### 13.6. `src/app/app.component.html`

```html
<router-outlet></router-outlet>
```

### 13.7. `src/app/layouts/main-layout/main-layout.component.ts`

```ts
import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterLink, RouterOutlet],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
})
export class MainLayoutComponent {
}
```

### 13.8. `src/app/layouts/main-layout/main-layout.component.html`

```html
<header class="header">
  <div>
    <h1>Employee Schedule</h1>
    <p>Gestão simples de escalas de funcionários.</p>
  </div>

  <nav>
    <a routerLink="/schedules">Schedules</a>
    <a routerLink="/schedules/new">New Schedule</a>
  </nav>
</header>

<main class="container">
  <router-outlet></router-outlet>
</main>
```

### 13.9. `src/app/layouts/main-layout/main-layout.component.css`

```css
.header {
  padding: 16px 24px;
  border-bottom: 1px solid #ddd;
}

.header h1 {
  margin: 0;
}

.header p {
  margin: 4px 0 12px 0;
}

.header nav {
  display: flex;
  gap: 12px;
}

.header a {
  text-decoration: none;
  font-weight: 600;
}

.container {
  padding: 24px;
}
```

### 13.10. `src/app/modules/schedules/models/schedule.model.ts`

```ts
export type ScheduleStatus = 'Scheduled' | 'Completed' | 'Cancelled' | 'Absent';

export const SCHEDULE_STATUS_OPTIONS: readonly ScheduleStatus[] = [
  'Scheduled',
  'Completed',
  'Cancelled',
  'Absent'
];

export interface Schedule {
  id: number;
  employeeName: string;
  employeeRegistration: string;
  department: string;
  shiftName: string;
  startTime: string;
  endTime: string;
  workDate: string;
  status: ScheduleStatus;
  notes?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateScheduleRequest {
  employeeName: string;
  employeeRegistration: string;
  department: string;
  shiftName: string;
  startTime: string;
  endTime: string;
  workDate: string;
  status: ScheduleStatus;
  notes?: string | null;
}

export type UpdateScheduleRequest = CreateScheduleRequest;
```

### 13.11. `src/app/modules/schedules/services/schedule.service.ts`

```ts
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import {
  CreateScheduleRequest,
  Schedule,
  UpdateScheduleRequest
} from '../models/schedule.model';

@Injectable({
  providedIn: 'root'
})
export class ScheduleService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/schedules`;

  getAll(): Observable<Schedule[]> {
    return this.http.get<Schedule[]>(this.apiUrl);
  }

  getById(id: number): Observable<Schedule> {
    return this.http.get<Schedule>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateScheduleRequest): Observable<Schedule> {
    return this.http.post<Schedule>(this.apiUrl, request);
  }

  update(id: number, request: UpdateScheduleRequest): Observable<Schedule> {
    return this.http.put<Schedule>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

### 13.12. `src/app/modules/schedules/pages/schedule-list/schedule-list.component.ts`

```ts
import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { Schedule } from '../../models/schedule.model';
import { ScheduleService } from '../../services/schedule.service';

@Component({
  selector: 'app-schedule-list',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './schedule-list.component.html',
  styleUrl: './schedule-list.component.css'
})
export class ScheduleListComponent implements OnInit {
  private readonly scheduleService = inject(ScheduleService);

  schedules: Schedule[] = [];
  loading = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadSchedules();
  }

  loadSchedules(): void {
    this.loading = true;
    this.errorMessage = '';

    this.scheduleService.getAll().subscribe({
      next: (schedules) => {
        this.schedules = schedules;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar schedules.';
        this.loading = false;
      }
    });
  }

  deleteSchedule(schedule: Schedule): void {
    const confirmed = confirm(`Deseja excluir a escala de ${schedule.employeeName}?`);

    if (!confirmed) {
      return;
    }

    this.scheduleService.delete(schedule.id).subscribe({
      next: () => {
        this.schedules = this.schedules.filter(item => item.id !== schedule.id);
      },
      error: () => {
        this.errorMessage = 'Erro ao excluir schedule.';
      }
    });
  }
}
```

### 13.13. `src/app/modules/schedules/pages/schedule-list/schedule-list.component.html`

```html
<section class="page-header">
  <div>
    <h2>Schedules</h2>
    <p>Lista de escalas cadastradas.</p>
  </div>

  <a routerLink="/schedules/new" class="button">
    New Schedule
  </a>
</section>

@if (loading) {
  <div class="message">Carregando...</div>
}

@if (errorMessage) {
  <div class="error">{{ errorMessage }}</div>
}

@if (!loading && schedules.length === 0) {
  <div class="message">Nenhuma escala cadastrada.</div>
}

@if (!loading && schedules.length > 0) {
  <table class="table">
    <thead>
      <tr>
        <th>Employee</th>
        <th>Registration</th>
        <th>Department</th>
        <th>Shift</th>
        <th>Date</th>
        <th>Start</th>
        <th>End</th>
        <th>Status</th>
        <th>Actions</th>
      </tr>
    </thead>

    <tbody>
      @for (schedule of schedules; track schedule.id) {
        <tr>
          <td>{{ schedule.employeeName }}</td>
          <td>{{ schedule.employeeRegistration }}</td>
          <td>{{ schedule.department }}</td>
          <td>{{ schedule.shiftName }}</td>
          <td>{{ schedule.workDate }}</td>
          <td>{{ schedule.startTime }}</td>
          <td>{{ schedule.endTime }}</td>
          <td>{{ schedule.status }}</td>
          <td>
            <div class="action-buttons">
              <a [routerLink]="['/schedules', schedule.id, 'edit']">
                Edit
              </a>

              <button type="button" (click)="deleteSchedule(schedule)">
                Delete
              </button>
            </div>
          </td>
        </tr>
      }
    </tbody>
  </table>
}
```

### 13.14. `src/app/modules/schedules/pages/schedule-list/schedule-list.component.css`

```css
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-header h2 {
  margin: 0;
}

.page-header p {
  margin: 4px 0 0 0;
}

.button {
  padding: 8px 12px;
  border: 1px solid #333;
  text-decoration: none;
  border-radius: 4px;
}

.table {
  width: 100%;
  border-collapse: collapse;
}

.table th,
.table td {
  padding: 10px;
  border: 1px solid #ddd;
  text-align: left;
}

.action-buttons {
  display: flex;
  gap: 8px;
}

.message {
  padding: 12px;
  border: 1px solid #ddd;
}

.error {
  padding: 12px;
  border: 1px solid #900;
  margin-bottom: 16px;
}
```

### 13.15. `src/app/modules/schedules/pages/schedule-form/schedule-form.component.ts`

```ts
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import {
  CreateScheduleRequest,
  SCHEDULE_STATUS_OPTIONS,
  ScheduleStatus
} from '../../models/schedule.model';
import { ScheduleService } from '../../services/schedule.service';

@Component({
  selector: 'app-schedule-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './schedule-form.component.html',
  styleUrl: './schedule-form.component.css'
})
export class ScheduleFormComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly scheduleService = inject(ScheduleService);

  statusOptions = SCHEDULE_STATUS_OPTIONS;

  isEditMode = false;
  scheduleId: number | null = null;
  loading = false;
  errorMessage = '';

  form = this.formBuilder.nonNullable.group({
    employeeName: ['', Validators.required],
    employeeRegistration: ['', Validators.required],
    department: ['', Validators.required],
    shiftName: ['', Validators.required],
    startTime: ['', Validators.required],
    endTime: ['', Validators.required],
    workDate: ['', Validators.required],
    status: ['Scheduled' as ScheduleStatus, Validators.required],
    notes: ['']
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');

    if (!idParam) {
      return;
    }

    this.isEditMode = true;
    this.scheduleId = Number(idParam);

    if (Number.isNaN(this.scheduleId)) {
      this.errorMessage = 'ID inválido.';
      return;
    }

    this.loadSchedule(this.scheduleId);
  }

  onSubmit(): void {
    this.errorMessage = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request = this.buildRequest();

    if (request.startTime === request.endTime) {
      this.errorMessage = 'O horário final não pode ser igual ao horário inicial.';
      return;
    }

    this.loading = true;

    if (this.isEditMode && this.scheduleId !== null) {
      this.scheduleService.update(this.scheduleId, request).subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/schedules']);
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = error?.error?.message ?? 'Erro ao atualizar schedule.';
        }
      });

      return;
    }

    this.scheduleService.create(request).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/schedules']);
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error?.error?.message ?? 'Erro ao criar schedule.';
      }
    });
  }

  isInvalid(controlName: string): boolean {
    const control = this.form.get(controlName);

    if (!control) {
      return false;
    }

    return control.invalid && (control.dirty || control.touched);
  }

  private loadSchedule(id: number): void {
    this.loading = true;

    this.scheduleService.getById(id).subscribe({
      next: (schedule) => {
        this.form.patchValue({
          employeeName: schedule.employeeName,
          employeeRegistration: schedule.employeeRegistration,
          department: schedule.department,
          shiftName: schedule.shiftName,
          startTime: this.toHtmlTime(schedule.startTime),
          endTime: this.toHtmlTime(schedule.endTime),
          workDate: schedule.workDate,
          status: schedule.status,
          notes: schedule.notes ?? ''
        });

        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar schedule.';
        this.loading = false;
      }
    });
  }

  private buildRequest(): CreateScheduleRequest {
    const value = this.form.getRawValue();

    return {
      employeeName: value.employeeName.trim(),
      employeeRegistration: value.employeeRegistration.trim(),
      department: value.department.trim(),
      shiftName: value.shiftName.trim(),
      startTime: this.normalizeTime(value.startTime),
      endTime: this.normalizeTime(value.endTime),
      workDate: value.workDate,
      status: value.status,
      notes: value.notes.trim() ? value.notes.trim() : null
    };
  }

  private normalizeTime(value: string): string {
    if (value.length === 5) {
      return `${value}:00`;
    }

    return value;
  }

  private toHtmlTime(value: string): string {
    if (!value) {
      return '';
    }

    return value.slice(0, 5);
  }
}
```

### 13.16. `src/app/modules/schedules/pages/schedule-form/schedule-form.component.html`

```html
<section class="page-header">
  <div>
    <h2>{{ isEditMode ? 'Edit Schedule' : 'New Schedule' }}</h2>
    <p>{{ isEditMode ? 'Atualize os dados da escala.' : 'Cadastre uma nova escala.' }}</p>
  </div>

  <a routerLink="/schedules">Back</a>
</section>

@if (loading) {
  <div class="message">Processando...</div>
}

@if (errorMessage) {
  <div class="error">{{ errorMessage }}</div>
}

<form [formGroup]="form" (ngSubmit)="onSubmit()" class="form">
  <div class="form-row">
    <label for="employeeName">Employee Name</label>
    <input id="employeeName" type="text" formControlName="employeeName" />
    @if (isInvalid('employeeName')) {
      <small>employeeName é obrigatório.</small>
    }
  </div>

  <div class="form-row">
    <label for="employeeRegistration">Employee Registration</label>
    <input id="employeeRegistration" type="text" formControlName="employeeRegistration" />
    @if (isInvalid('employeeRegistration')) {
      <small>employeeRegistration é obrigatório.</small>
    }
  </div>

  <div class="form-row">
    <label for="department">Department</label>
    <input id="department" type="text" formControlName="department" />
    @if (isInvalid('department')) {
      <small>department é obrigatório.</small>
    }
  </div>

  <div class="form-row">
    <label for="shiftName">Shift Name</label>
    <input id="shiftName" type="text" formControlName="shiftName" />
    @if (isInvalid('shiftName')) {
      <small>shiftName é obrigatório.</small>
    }
  </div>

  <div class="form-row">
    <label for="workDate">Work Date</label>
    <input id="workDate" type="date" formControlName="workDate" />
    @if (isInvalid('workDate')) {
      <small>workDate é obrigatório.</small>
    }
  </div>

  <div class="form-row">
    <label for="startTime">Start Time</label>
    <input id="startTime" type="time" formControlName="startTime" />
    @if (isInvalid('startTime')) {
      <small>startTime é obrigatório.</small>
    }
  </div>

  <div class="form-row">
    <label for="endTime">End Time</label>
    <input id="endTime" type="time" formControlName="endTime" />
    @if (isInvalid('endTime')) {
      <small>endTime é obrigatório.</small>
    }
  </div>

  <div class="form-row">
    <label for="status">Status</label>
    <select id="status" formControlName="status">
      @for (status of statusOptions; track status) {
        <option [value]="status">
          {{ status }}
        </option>
      }
    </select>
    @if (isInvalid('status')) {
      <small>status é obrigatório.</small>
    }
  </div>

  <div class="form-row">
    <label for="notes">Notes</label>
    <textarea id="notes" rows="4" formControlName="notes"></textarea>
  </div>

  <div class="form-actions">
    <button type="submit" [disabled]="loading">
      {{ isEditMode ? 'Update' : 'Create' }}
    </button>

    <a routerLink="/schedules">Cancel</a>
  </div>
</form>
```

### 13.17. `src/app/modules/schedules/pages/schedule-form/schedule-form.component.css`

```css
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-header h2 {
  margin: 0;
}

.page-header p {
  margin: 4px 0 0 0;
}

.form {
  max-width: 700px;
}

.form-row {
  display: flex;
  flex-direction: column;
  margin-bottom: 16px;
}

.form-row label {
  font-weight: 600;
  margin-bottom: 6px;
}

.form-row input,
.form-row select,
.form-row textarea {
  padding: 8px;
  border: 1px solid #aaa;
  border-radius: 4px;
}

.form-row small {
  margin-top: 4px;
}

.form-actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

.form-actions button {
  padding: 8px 14px;
}

.message {
  padding: 12px;
  border: 1px solid #ddd;
  margin-bottom: 16px;
}

.error {
  padding: 12px;
  border: 1px solid #900;
  margin-bottom: 16px;
}
```

### 13.18. `src/styles.css`

```css
body {
  margin: 0;
  font-family: Arial, Helvetica, sans-serif;
}

a {
  color: inherit;
}

button {
  cursor: pointer;
}
```

## 14. Rodar frontend

Com a API rodando em `http://localhost:5000`:

```bash
ng serve
```

Acesse:

```txt
http://localhost:4200
```

## 15. Ordem recomendada de desenvolvimento

```txt
1. Criar solution e backend
2. Instalar pacotes NuGet
3. Criar banco PostgreSQL local
4. Criar ScheduleStatus e Schedule
5. Criar DTOs
6. Criar AppDbContext
7. Criar repository contract e implementation
8. Criar service contract e implementation
9. Criar controller
10. Criar Config: CORS, DI e Swagger
11. Configurar Program.cs e appsettings.json
12. Rodar migration e database update
13. Testar API pelo Swagger
14. Criar projeto Angular
15. Configurar environment, routes e HttpClient
16. Criar model Schedule no Angular
17. Criar ScheduleService no Angular
18. Criar layout
19. Criar listagem
20. Criar formulário de criar/editar
21. Testar CRUD completo pelo navegador
22. Documentar no README
```

## 16. Branches e commits semânticos

Branches:

```bash
git checkout -b feat/create-backend-project
git checkout -b feat/add-schedule-entity
git checkout -b feat/create-schedule-crud-api
git checkout -b feat/create-angular-project
git checkout -b feat/create-schedule-list-page
git checkout -b feat/create-schedule-form-page
git checkout -b docs/update-project-docs
```

Commits:

```bash
git add .
git commit -m "feat: create backend project"

git add .
git commit -m "feat: add schedule entity and db context"

git add .
git commit -m "feat: create schedule repository and service"

git add .
git commit -m "feat: create schedule crud controller"

git add .
git commit -m "feat: create angular project"

git add .
git commit -m "feat: create schedule list page"

git add .
git commit -m "feat: create schedule form page"

git add .
git commit -m "docs: update project documentation"
```

## 17. Checklist final

```txt
[ ] PostgreSQL está rodando em localhost:5432
[ ] Banco employee_schedule_db existe
[ ] appsettings.json tem a senha correta do PostgreSQL
[ ] dotnet build executa sem erro
[ ] dotnet ef migrations add InitialCreate executou sem erro
[ ] dotnet ef database update criou a tabela schedules
[ ] API roda em http://localhost:5000
[ ] Swagger abre em http://localhost:5000/swagger
[ ] POST /api/schedules cria uma escala
[ ] GET /api/schedules lista as escalas
[ ] GET /api/schedules/{id} busca uma escala
[ ] PUT /api/schedules/{id} atualiza uma escala
[ ] DELETE /api/schedules/{id} remove uma escala
[ ] Angular roda em http://localhost:4200
[ ] Angular lista schedules vindos da API
[ ] Angular cria schedule
[ ] Angular edita schedule
[ ] Angular exclui schedule
[ ] Navegador não mostra erro de CORS
```

## 18. README simples sugerido

Crie um `README.md` na raiz:

```md
# Employee Schedule

MVP full-stack para aprender Angular, ASP.NET Core Web API, Entity Framework Core e PostgreSQL local.

## Stack

- ASP.NET Core Web API
- Entity Framework Core
- Npgsql
- PostgreSQL local
- Angular standalone
- HttpClient
- Reactive Forms

## Como rodar o banco

```bash
createdb -h localhost -p 5432 -U postgres employee_schedule_db
```

## Como rodar a API

```bash
cd EmployeeSchedule.Api

dotnet restore

dotnet ef database update

dotnet run --urls "http://localhost:5000"
```

Swagger:

```txt
http://localhost:5000/swagger
```

## Como rodar o frontend

```bash
cd employee-schedule-web
npm install
ng serve
```

Frontend:

```txt
http://localhost:4200
```

## Endpoints

```txt
GET     /api/schedules
GET     /api/schedules/{id}
POST    /api/schedules
PUT     /api/schedules/{id}
DELETE  /api/schedules/{id}
```
```

## 19. Referências oficiais usadas

- .NET Support Policy: https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core
- ASP.NET Core Web API: https://learn.microsoft.com/en-us/aspnet/core/web-api/
- ASP.NET Core CORS: https://learn.microsoft.com/en-us/aspnet/core/security/cors
- EF Core CLI: https://learn.microsoft.com/en-us/ef/core/cli/dotnet
- Npgsql EF Core Provider: https://www.npgsql.org/efcore/
- Angular CLI setup: https://angular.dev/tools/cli/setup-local
- Angular HttpClient setup: https://angular.dev/guide/http/setup
- Angular Reactive Forms: https://angular.dev/guide/forms/reactive-forms
- PostgreSQL CREATE DATABASE: https://www.postgresql.org/docs/current/sql-createdatabase.html
- Conventional Commits: https://www.conventionalcommits.org/en/v1.0.0/
