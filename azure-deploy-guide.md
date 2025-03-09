# Azure Deployment Guide for FinBin Backend

## 1. Choose the Right Web App Type

For your .NET Core application:
- **App Service Plan**: Standard (S1) or Premium (P1V2) tier for production
- **Runtime Stack**: .NET 9 (or your specific .NET version)
- **Operating System**: Windows or Linux (Windows recommended if using SQL Server)
- **Region**: Choose the same region as your other Azure services

## 2. Set Up Azure Resources

### Azure SQL Database
1. Create an Azure SQL Server and Database
2. Configure firewall settings to allow Azure services
3. Note the connection string for your database

### Azure OpenAI Service
1. Create an Azure OpenAI resource
2. Deploy your required model
3. Note the endpoint URL, API key, deployment name, and model ID

## 3. Configure App Settings in Azure Portal

After creating your web app, add these application settings in the Configuration section:

### Connection Strings
Add a connection string named `DefaultConnection` with your Azure SQL connection string:
```
Server=your-server.database.windows.net;Database=your-db;User Id=your-username;Password=your-password;
```
Select type: `SQLAzure`

### Application Settings

Add these key-value pairs:

```
AzureOpenAI__Endpoint=https://your-openai-instance.openai.azure.com/
AzureOpenAI__ApiKey=your-api-key
AzureOpenAI__DeploymentName=your-deployment-name
AzureOpenAI__ModelId=gpt-35-turbo (or your model)

JWT__SecretKey=your-secret-key
JWT__Issuer=your-issuer
JWT__Audience=your-audience

EmailSettings__SmtpHost=your-smtp-server
EmailSettings__SmtpPort=587
EmailSettings__SmtpUsername=your-username
EmailSettings__SmtpPassword=your-password
EmailSettings__SenderEmail=your-email
EmailSettings__SenderName=FinBin

AllowedOrigins=https://your-frontend-url.azurewebsites.net
ASPNETCORE_ENVIRONMENT=Production
```

## 4. Deploy Your Application

### Using Visual Studio
1. Right-click on your project
2. Select "Publish"
3. Choose "Azure" and follow the wizard

### Using GitHub Actions
1. Set up a GitHub workflow for CI/CD
2. Use Azure credentials as GitHub secrets
3. Configure the workflow to build and deploy to your App Service

### Using Azure DevOps
1. Create a pipeline in Azure DevOps
2. Configure build steps for your .NET application
3. Add an Azure App Service deployment task

## 5. Post-Deployment Steps

1. Run database migrations using:
   - Azure CLI
   - Custom deployment script
   - EF Core Migrations Bundle

2. Verify application logs in:
   - Application Insights
   - Log stream in Azure Portal

3. Set up monitoring and alerts for:
   - Performance
   - Errors
   - Database connection issues
   - OpenAI API usage

## 6. Security Considerations

1. Enable Managed Identity for your App Service
2. Use Key Vault for storing secrets
3. Configure CORS properly
4. Implement proper authentication for your API
