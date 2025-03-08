using BudgetBackend.Data;
using BudgetBackend.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configure Database (Azure SQL or Local SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// 🔹 Add Controllers
builder.Services.AddControllers();

// 🔹 Enable Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BudgetBackend API", Version = "v1" });
});

// 🔹 Register AI Plugins (Semantic Kernel)
builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.Plugins.AddFromType<IncomeTrackerPlugin>(); // Register AI Plugin
    return kernelBuilder.Build();
});

var app = builder.Build();

// 🔹 Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BudgetBackend API v1"));
}

// 🔹 Enable CORS
app.UseCors("AllowFrontend");

// 🔹 Enable HTTPS Redirection
app.UseHttpsRedirection();

// 🔹 Map Controllers
app.MapControllers();

app.Run();
