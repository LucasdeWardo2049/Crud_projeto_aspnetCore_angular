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
            ?? throw new InvalidOperationException("Connection string DefaultConnection nao foi configurada.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IScheduleService, ScheduleService>();

        return services;
    }
}
