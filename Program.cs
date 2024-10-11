using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PersonalFinanceTrackerAPI;
using PersonalFinanceTrackerAPI.Data;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;
using PersonalFinanceTrackerAPI.Repositories;
using PersonalFinanceTrackerAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
    opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    opt.SegmentsPerWindow = 3;
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
var connectionString = builder.Configuration.GetConnectionString("DevelopmentPostgreSQLConnection");
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
  c.OperationFilter<AddVersionHeaderOperationFilter>(); // Asegúrate de implementar este filtro
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
  // Esto habilitará Swagger en producción
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PersonalFinanceTrackerAPI v1");
    c.RoutePrefix = string.Empty; // Para que Swagger UI esté en la raíz
  });
}
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthorization();
app.MapIdentityApi<AppUser>();
app.MapControllers();
app.Run();
