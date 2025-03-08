using BudgetBackend.Data;
using BudgetBackend.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var openAiConfig = builder.Configuration.GetSection("AzureOpenAI");
string? endpoint = openAiConfig["Endpoint"];
string? apiKey = openAiConfig["ApiKey"];
string? deploymentName = openAiConfig["DeploymentName"];

builder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: deploymentName!,
    apiKey: apiKey!,
    endpoint: endpoint!
);

builder.Services.AddTransient<Kernel>(serviceProvider =>
{
    return new Kernel(serviceProvider);
});

builder.Services.AddSingleton<IncomeTrackerPlugin>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BudgetBackend API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BudgetBackend API v1"));
    var swaggerUrl = $"http://localhost:{builder.Configuration["ASPNETCORE_URLS"]?.Split(":").Last() ?? "5000"}/swagger/index.html";
    Console.WriteLine($"Swagger available at: {swaggerUrl}");
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
