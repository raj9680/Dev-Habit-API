using DevHabit.API.Database;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
    .UseSnakeCaseNamingConvention());

// OPT
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resources => resources.AddService(builder.Environment.ApplicationName))
    .WithTracing(tracing => 
        tracing.AddHttpClientInstrumentation().AddAspNetCoreInstrumentation())
    .WithMetrics(metrics => metrics.AddHttpClientInstrumentation().AddAspNetCoreInstrumentation().AddRuntimeInstrumentation())
    .UseOtlpExporter();

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
});
// OPT -End

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
