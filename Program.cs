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
using System.Text;

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

            // Register TokenService
            builder.Services.AddScoped<TokenService>();

            // Register AgentFactory - specify the correct namespace
            builder.Services.AddScoped<BudgetBackend.Agents.AgentFactory>();

            // JWT Authentication configuration
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                            builder.Configuration["JWT:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured")
                        )),
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["JWT:Audience"],
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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BudgetBackend API", Version = "v1" });
                
                // Add JWT authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Apply migrations at startup
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BudgetBackend API v1"));
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
