namespace EmployeeSchedule.Api.Config;

public static class CorsConfig
{
    public const string AllowAngularPolicy = "AllowAngularPolicy";

    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration // lê as origens permitidas do arquivo de configuração, ou usa um valor padrão se não estiver configurado
            .GetSection("Frontend:AllowedOrigins") 
            .Get<string[]>() ?? ["http://localhost:4200"]; // valor padrão para desenvolvimento local com Angular CLI

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
