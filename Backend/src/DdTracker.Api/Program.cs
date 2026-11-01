using DdTracker.Api.Application;
using DdTracker.Api.BackgroundJobs;
using DdTracker.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IClock, SystemClock>();
builder.Services.AddScoped<ContainerService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddHostedService<AlertBackgroundService>();
var app = builder.Build();

// Apply migrations + seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbInitializer.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();  // disabled: Angular dev + API both HTTP

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () =>
{
    return Results.Ok(new { message = "Server running! Use prefix /api for api endpoints and /api/health for checking server health" });
});
app.MapGet("/api/health", () =>
{
    return Results.Ok(new { message = "API Working" });
});

app.Run();