using DotNetEnv;
using GradFix_app_be;
using GradFix_app_be.Controllers.Middleware;
using GradFix_app_be.Domain;
using GradFix_app_be.Infrastructure;
using GradFix_app_be.Services;
using GradFix_app_be.Services.IServices;
using GradFix_app_be.Services.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Env.Load("../.env");

// loading environmental variables
builder.Configuration.AddEnvironmentVariables();

Startup.AddCors(builder);
Startup.AddSwagger(builder);
Startup.AddAuthenticationAndAuthorization(builder);



builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ReportProfile>();
    cfg.AddProfile<UserProfile>();
});

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Database=gradfix;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));


// Register auth services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed roles and admin user
await DbSeeder.SeedAsync(app.Services);

app.Run();
