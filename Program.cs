using System.Threading.RateLimiting;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using PersonalFinanceTrackerAPI;
using PersonalFinanceTrackerAPI.Data;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;
using PersonalFinanceTrackerAPI.Repositories;
using PersonalFinanceTrackerAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFinancialGoalsRepository, FinancialGoalRepository>();
builder.Services.AddScoped<IFinancialGoalService, FinancialGoalService>();

// Health check
builder.Services.AddHealthChecks().AddNpgSql(
  connectionString!, name: "Neon PostgreSQL");

// Add Authentication
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

//Add Authorization
builder.Services.AddAuthorizationBuilder();

// Add Rate Limiting
builder.Services.AddRateLimiter(opt =>
{
  opt.AddSlidingWindowLimiter("SlidingWindowPolicy", opt =>
  {
    opt.Window = TimeSpan.FromSeconds(10);
    opt.PermitLimit = 4;
    opt.QueueLimit = 3;
    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    opt.SegmentsPerWindow = 3;
  }).RejectionStatusCode = 429;

  opt.AddFixedWindowLimiter("HealthCheckPolicy", fixedOptions =>
 {
   fixedOptions.Window = TimeSpan.FromMinutes(1);
   fixedOptions.PermitLimit = 4;
   fixedOptions.QueueLimit = 2;
   fixedOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
 }).RejectionStatusCode = 429;
});

// Add Versioning
builder.Services.AddApiVersioning(opt =>
{
  opt.AssumeDefaultVersionWhenUnspecified = true;
  opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
  opt.ReportApiVersions = true;
  opt.ApiVersionReader = new HeaderApiVersionReader("x-version");
});

// Config DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
{
  opt.UseNpgsql(connectionString);
});

builder.Services.AddIdentityCore<AppUser>()
  .AddEntityFrameworkStores<AppDbContext>()
  .AddApiEndpoints();

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "PersonalFinanceTrackerAPI", Version = "v1" });
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    In = ParameterLocation.Header,
    Description = "Please enter your token",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey
  });
  c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new string[] {} }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(builder =>
  {
    builder.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader();
  });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
else
{
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PersonalFinanceTrackerAPI v1");
    c.RoutePrefix = string.Empty;
  });
}
app.UseHttpsRedirection();
app.MapHealthChecks("/health", new HealthCheckOptions()
{
  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
  ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
}).RequireAuthorization().RequireRateLimiting("HealthCheckPolicy");
app.UseRateLimiter();
app.UseAuthorization();
app.MapIdentityApi<AppUser>();
app.MapControllers();
app.Run();
