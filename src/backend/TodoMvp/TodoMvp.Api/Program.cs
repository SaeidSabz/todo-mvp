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

builder.Services.AddControllers();
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

app.MapControllers();

app.Run();
