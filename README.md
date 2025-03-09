<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/55b67de4-3d26-418c-9222-1bcd3c6bfefd">
  <source media="(prefers-color-scheme: light)" srcset="https://github.com/user-attachments/assets/83a0c752-6f11-48d5-a4bb-9e1ca165fbd6">
  <img src="https://github.com/user-attachments/assets/d5145b77-fecd-4e46-949a-b8510e097553">
</picture>

# FinBins - AI-Powered Budget Management API

**FinBin** is a financial planning API that helps users manage their budgets, track expenses, and save towards financial goals (called "buckets"). It integrates with **Microsoft Semantic Kernel** to provide AI-driven financial insights and savings recommendations while leveraging **Azure SQL Database** for secure and scalable data storage.

## Quick Start (For Hackathon Judges)

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-repo/FinBin.git
   cd FinBin
   ```

2. **Update connection strings in `appsettings.json`**
   - Set your database connection
   - Add your Azure OpenAI API keys

3. **Run the migrations and start the API**
   ```bash
   dotnet ef database update
   dotnet run
   ```

4. **Access the API documentation**
   - Open your browser to `http://localhost:5263/swagger`

## Features

- **User Management** – Create and manage user profiles  
- **Income & Expense Tracking** – Store and analyze financial transactions  
- **Buckets (Savings Goals)** – Set up financial goals with target amounts and deadlines  
- **AI-Powered Financial Insights** – Personalized analysis of income, expenses, and goal feasibility  
- **Automated Savings Recommendations** – AI suggests adjustments to spending and savings  
- **Azure SQL Database** – Secure and scalable cloud database storage  

## Tech Stack

- **.NET 8 Web API**  
- **Entity Framework Core (Azure SQL Database)**  
- **Microsoft Semantic Kernel (Azure OpenAI GPT-4)**  
- **Swagger (API Documentation)**  
- **Azure App Services (For Future Deployment)**  

## Setup Prerequisites

### 1. Install Entity Framework Core Tools

```bash
dotnet tool install --global dotnet-ef
```

If you already have it installed, update it:

```bash
dotnet tool update --global dotnet-ef
```

Verify installation:

```bash
dotnet ef
```

You should see the EF Core logo:

```
                     _/\__       
               ---==/    \\      
         ___  ___   |.    \|\    
        | __|| __|  |  )   \\\
        | _| | _|   \_/ |  //|\\
        |___||_|       /   \\\/\\
```

### 2. Required Software and Services

- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Azure Account** - For Azure SQL Database and Azure OpenAI (optional for local development)
- **SQL Server** - Local instance or Azure SQL Database

## Detailed Setup Guide

### 1. Clone the Repository

```bash
git clone https://github.com/your-repo/FinBin.git
cd FinBin
```

### 2. Configure Local Development

#### Option A: Local Database
- Update `appsettings.Development.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=FinBinDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

#### Option B: Azure SQL Database
- Update `appsettings.json` with your Azure SQL Database connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=FinBinDB;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}
```

### 3. Configure Azure OpenAI & Semantic Kernel
- Update `appsettings.json` with your Azure OpenAI settings:
```json
"AzureOpenAI": {
  "Endpoint": "https://your-openai-instance.cognitiveservices.azure.com/",
  "ApiKey": "your-api-key",
  "DeploymentName": "gpt-4"
}
```

### 4. Run Database Migrations
```bash
dotnet ef database update
```

### 5. Run the API
```bash
dotnet run
```

The API will be available at **`http://localhost:5263/swagger`**

## Authentication

For development, we're using local authentication. In production, we'll implement secure authentication flows.

Reference: [Secure Authentication Flows](https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-9.0#secure-authentication-flows)

## Cloud Infrastructure

### Azure SQL Database
FinBin uses **Azure SQL Database** for storing user data, financial records, and budget goals. The database is managed via **Entity Framework Core**, allowing seamless schema migrations and scalability.

### Microsoft Semantic Kernel (AI Processing)
FinBin integrates **Microsoft Semantic Kernel** to analyze financial data and provide personalized AI-driven savings recommendations. **Azure OpenAI** models, such as **GPT-4**, process financial insights dynamically.

## API Endpoints

### User Management
- **POST** `/api/users` – Create a new user  
- **GET** `/api/users/{id}` – Get user details  

### Financial Summary
- **GET** `/api/financial-summary/{userId}` – Retrieve financial summary  
- **PUT** `/api/financial-summary/{userId}` – Update financial summary  

### Buckets (Savings Goals)
- **POST** `/api/buckets/{userId}` – Create a new savings goal  
- **GET** `/api/buckets/{userId}` – List all buckets for a user  
- **PUT** `/api/buckets/{userId}/{bucketId}` – Update a savings goal  
- **DELETE** `/api/buckets/{userId}/{bucketId}` – Delete a savings goal  

### AI-Powered Budget Analysis
- **GET** `/api/buckets/analyze/{userId}` – AI analysis on user's financial health  
- **POST** `/api/buckets/adjust/{userId}/{bucketId}` – AI-driven savings recommendations  

## AI-Driven Budget Analysis

The AI agent fetches a user's financial data and provides smart recommendations based on:  
- **Savings Timeline** – How long will it take to reach a goal?  
- **Spending Adjustments** – Suggested expense cuts to save more  
- **Probability Score (0-1)** – Likelihood of achieving the goal in time  

Example AI Response:
```json
{
  "bucket": "Vacation Fund",
  "targetAmount": 5000,
  "currentSavedAmount": 200,
  "suggestedMonthlySavings": 400,
  "probabilityScore": 0.75,
  "recommendations": "Reduce subscriptions by $30 and allocate it to savings."
}
```

## Troubleshooting

### Common Issues

1. **Database Connection Errors**
   - Verify your connection string is correct
   - Ensure SQL Server is running
   - Check firewall settings for Azure SQL

2. **Azure OpenAI Integration**
   - Verify your API key and endpoint
   - Check deployment name matches your Azure OpenAI setup

3. **Entity Framework Errors**
   - Run `dotnet ef migrations add InitialCreate` if database doesn't exist

## Future Enhancements
- AI chatbot for personalized financial coaching  
- Auto-transfer savings recommendations  
- Mobile app integration  

## Contributing
1. **Fork** the repository  
2. **Create a feature branch**  
3. **Submit a pull request**  

## License
MIT License.
