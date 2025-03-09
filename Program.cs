using BudgetBackend.Data;
using BudgetBackend.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    )
);

var openAiConfig = builder.Configuration.GetSection("AzureOpenAI");
string? endpoint = openAiConfig["Endpoint"];
string? apiKey = openAiConfig["ApiKey"];
string? deploymentName = openAiConfig["DeploymentName"];
string? modelId = openAiConfig["ModelId"];

if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(deploymentName) || string.IsNullOrEmpty(apiKey))
{
    throw new InvalidOperationException("Azure OpenAI configuration is missing or incorrect.");
}

builder.Services.AddSingleton<IChatCompletionService>(serviceProvider =>
{
    return new AzureOpenAIChatCompletionService(
        deploymentName: deploymentName!,
        apiKey: apiKey!,
        endpoint: endpoint!,
        modelId: modelId!
    );
});

builder.Services.AddSingleton<Kernel>(serviceProvider =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: deploymentName!,
        apiKey: apiKey!,
        endpoint: endpoint!,
        modelId: modelId!,
        serviceId: "openai-service"
    );

    kernelBuilder.Plugins.AddFromType<BudgetPlugin>();

    return kernelBuilder.Build();
});

builder.Services.AddScoped<BudgetPlugin>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

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
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
