using BudgetBackend.Data;
using BudgetBackend.Plugins;
using BudgetBackend.Services;
using BudgetBackend.Agents;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using System.Text;
using System.Reflection;
using System.IO;

namespace BudgetBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database configuration
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
                )
            );

            // OpenAI configuration
            var openAiConfig = builder.Configuration.GetSection("AiServiceSettings");
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

            // Add AI services
            builder.Services.AddAIServices(builder.Configuration);

            // Register TokenService
            builder.Services.AddScoped<TokenService>();

            // Register AgentFactory - specify the correct namespace
            builder.Services.AddScoped<BudgetBackend.Agents.AgentFactory>();

            // Register history service
            builder.Services.AddScoped<IHistoryService, HistoryService>();

            // JWT Authentication configuration
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                            builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured")
                        )),
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Add authorization
            builder.Services.AddAuthorization();

            // Register plugins and services
            builder.Services.AddScoped<BudgetPlugin>();
            builder.Services.AddScoped<FinancialChatService>();

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(builder.Configuration["AllowedOrigins"]?.Split(',') ?? new[] { "http://localhost:3000" })
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });

            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinBin API", Version = "v1" });
                // If you have XML comments for API documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            // Configure email settings
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();

            // Add Application Insights if in production
            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddApplicationInsightsTelemetry();
            }

            var app = builder.Build();

            // Apply migrations at startup
            // Only do this in development - for production use proper migration strategy
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.Migrate();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) // Allow Swagger in production too
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinBin API v1");
                });
            }
            else 
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            
            // Make sure UseAuthentication is called before UseAuthorization
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapControllers();
            app.Run();
        }
    }
}
