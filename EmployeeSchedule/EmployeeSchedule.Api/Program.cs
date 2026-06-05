using System.Text.Json.Serialization;
using EmployeeSchedule.Api.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(CorsConfig.AllowAngularPolicy);
app.UseAuthorization();

app.MapControllers();

app.Run();
