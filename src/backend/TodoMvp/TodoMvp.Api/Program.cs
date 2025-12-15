using Microsoft.AspNetCore.Mvc;
using TodoMvp.Api.Contracts.Errors;
using TodoMvp.Api.Infrastructure.Errors;
using TodoMvp.Application.Configurations;
using TodoMvp.Persistence.Configurations;

var builder = WebApplication.CreateBuilder(args);
const string AllowFrontendPolicy = "AllowFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowFrontendPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddApplicationDependencies(builder.Configuration)
    .AddPersistenceDependencies(builder.Configuration);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage))
            .Where(msg => !string.IsNullOrWhiteSpace(msg))
            .ToArray();

        var body = new ApiErrorResponse
        {
            Error = "ValidationFailed",
            Details = errors
        };

        return new BadRequestObjectResult(body);
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TodoMvp v1");
    });
}

app.UseHttpsRedirection();

app.UseCors(AllowFrontendPolicy);

app.UseAuthorization();

app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext httpContext) =>
{
    var traceId = httpContext.TraceIdentifier;

    return Results.Json(
        new ApiErrorResponse
        {
            Error = "ServerError",
            Message = "An unexpected error occurred.",
            Details = new[] { $"TraceId: {traceId}" }
        },
        statusCode: StatusCodes.Status500InternalServerError);
});

app.MapControllers();

app.Run();

/// <summary>
/// Partial Program class required for WebApplicationFactory integration tests.
/// </summary>
public partial class Program { }