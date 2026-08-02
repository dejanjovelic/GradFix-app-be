using DotNetEnv;
using GradFix_app_be;
using GradFix_app_be.Domain;
using GradFix_app_be.Infrastructure;
using GradFix_app_be.Services.Mappings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Env.Load("../.env");

// loading environmental variables
builder.Configuration.AddEnvironmentVariables();

Startup.AddCors(builder);
Startup.AddSwagger(builder);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ReportProfile>();
 
});


var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Database=gradfix;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();
// Register application services
builder.Services.AddScoped<GradFix_app_be.Services.IReportService, GradFix_app_be.Services.ReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();
app.MapControllers();

// Seed roles and admin user
await DbSeeder.SeedAsync(app.Services);

app.Run();
