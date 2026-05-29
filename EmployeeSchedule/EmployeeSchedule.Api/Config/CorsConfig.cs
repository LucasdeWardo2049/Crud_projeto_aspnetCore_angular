namespace EmployeeSchedule.Api.Config;

public static class CorsConfig
{
    public const string AllowAngularPolicy = "AllowAngularPolicy";

    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Frontend:AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:4200"];

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
