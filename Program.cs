using BudgetBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configure Database (Azure SQL or Local SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Enable CORS (Allows frontend to access API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// 🔹 Add Controllers for API
builder.Services.AddControllers();

// 🔹 Enable Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinBin API", Version = "v1" });
});

var app = builder.Build();

// 🔹 Enable Swagger (Only in Development Mode)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinBin API v1"));
}

// 🔹 Enable CORS
app.UseCors("AllowFrontend");

// 🔹 Enable HTTPS Redirection
app.UseHttpsRedirection();

// 🔹 Map Controllers (Ensure API Endpoints Work)
app.MapControllers();

app.Run();
