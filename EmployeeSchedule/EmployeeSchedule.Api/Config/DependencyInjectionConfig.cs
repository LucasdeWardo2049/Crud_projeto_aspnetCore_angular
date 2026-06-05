using EmployeeSchedule.Api.Context;
using EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Contract;
using EmployeeSchedule.Api.Modules.ScheduleModule.Repository.Implementation;
using EmployeeSchedule.Api.Modules.ScheduleModule.Services.Contract;
using EmployeeSchedule.Api.Modules.ScheduleModule.Services.Implementation;
using EmployeeSchedule.Api.Modules.EmployeeModule.Repository.Contract;
using EmployeeSchedule.Api.Modules.EmployeeModule.Repository.Implementation;
using EmployeeSchedule.Api.Modules.EmployeeModule.Services.Contract;
using EmployeeSchedule.Api.Modules.EmployeeModule.Services.Implementation;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSchedule.Api.Config;

public static class DependencyInjectionConfig // classe de extensão para configurar a injeção de dependências
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // registra a implementação do repositório de funcionários
        services.AddScoped<IEmployeeService, EmployeeService>(); // registra a implementação do serviço de funcionários
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string DefaultConnection nao foi configurada.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IScheduleRepository, ScheduleRepository>(); // registra a implementação do repositório de agendamento
        services.AddScoped<IScheduleService, ScheduleService>(); // registra a implementação do serviço de agendamento

        return services;
    }
}
