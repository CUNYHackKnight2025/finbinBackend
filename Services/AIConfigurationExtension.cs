using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace BudgetBackend.Services
{
    public static class AIConfigurationExtension
    {
        public static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IChatCompletionService>(serviceProvider =>
            {
                var aiSettings = configuration.GetSection("AiServiceSettings");
                string apiKey = aiSettings["ApiKey"];
                string endpoint = aiSettings["Endpoint"];
                
                var kernelBuilder = Kernel.CreateBuilder();
                
                // Determine if we're using Azure OpenAI or OpenAI based on endpoint
                if (!string.IsNullOrEmpty(endpoint))
                {
                    // Azure OpenAI
                    kernelBuilder.AddAzureOpenAIChatCompletion(
                        deploymentName: "gpt-35-turbo", // Update to match your deployment
                        endpoint: endpoint,
                        apiKey: apiKey
                    );
                }
                else
                {
                    // Direct OpenAI
                    kernelBuilder.AddOpenAIChatCompletion(
                        modelId: "gpt-3.5-turbo", 
                        apiKey: apiKey
                    );
                }
                
                var kernel = kernelBuilder.Build();
                return kernel.GetRequiredService<IChatCompletionService>();
            });
            
            return services;
        }
    }
}
