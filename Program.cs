using DevHabit.API.Database;
using DevHabit.API.Database.Extensions;
using DevHabit.API.DTOs;
using DevHabit.API.Middlewares;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
    .AddNewtonsoftJson()
    .AddXmlSerializerFormatters(); // enable xml type

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddExceptionHandler<ValidationExceptionHandler>(); // MW 1
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();    // MW 2



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

    await app.ApplyMigrationsAsync();
}

app.UseExceptionHandler();

app.MapControllers();

app.Run();
