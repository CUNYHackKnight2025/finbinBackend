# Backend

Make sure you got Microsoft Entity Framework installed:

```
dotnet tool install --global dotnet-ef
```

If you already have it installed make sure to update:

```
dotnet tool update --global dotnet-ef
```

you can check if it installed:

```
dotnet ef
```

you should see a unicorn thingy and EF

like this:

```
                     _/\__       
               ---==/    \\      
         ___  ___   |.    \|\    
        | __|| __|  |  )   \\\
        | _| | _|   \_/ |  //|\\
        |___||_|       /   \\\/\\
```


For in production we gotta authenticate for security, but temp we can use local:

https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-9.0#secure-authentication-flows

### **FinBin - AI-Powered Budget Management API**  

**FinBin** is a financial planning API that helps users manage their budgets, track expenses, and save towards financial goals (**buckets**). It integrates with **Microsoft Semantic Kernel** to provide AI-driven financial insights and savings recommendations while leveraging **Azure SQL Database** for secure and scalable data storage.  

---

## **Features**  
✔ **User Management** – Create and manage user profiles  
✔ **Income & Expense Tracking** – Store and analyze financial transactions  
✔ **Buckets (Savings Goals)** – Set up financial goals with target amounts and deadlines  
✔ **AI-Powered Financial Insights** – Personalized analysis of income, expenses, and goal feasibility  
✔ **Automated Savings Recommendations** – AI suggests adjustments to spending and savings  
✔ **Azure SQL Database** – Secure and scalable cloud database storage  

---

## **Tech Stack**  
- **.NET 8 Web API**  
- **Entity Framework Core (Azure SQL Database)**  
- **Microsoft Semantic Kernel (Azure OpenAI GPT-4)**  
- **Swagger (API Documentation)**  
- **Azure App Services (For Future Deployment)**  

---

## **Cloud Infrastructure**  

### **Azure SQL Database**  
FinBin uses **Azure SQL Database** for storing user data, financial records, and budget goals. The database is managed via **Entity Framework Core**, allowing seamless schema migrations and scalability.  

### **Microsoft Semantic Kernel (AI Processing)**  
FinBin integrates **Microsoft Semantic Kernel** to analyze financial data and provide personalized AI-driven savings recommendations. **Azure OpenAI** models, such as **GPT-4**, process financial insights dynamically.  

---

## **Setup Guide**  

### **1. Clone the Repository**  
```bash
git clone https://github.com/your-repo/FinBin.git
cd FinBin
```

### **2. Configure Azure SQL Database**  
- **Update `appsettings.json`** with your Azure SQL Database connection string:  
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=FinBinDB;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}
```
- Run database migrations:  
```bash
dotnet ef database update
```

### **3. Configure Azure OpenAI & Semantic Kernel**  
- **Set up Azure OpenAI in `appsettings.json`**:  
```json
"AzureOpenAI": {
  "Endpoint": "https://your-openai-instance.cognitiveservices.azure.com/",
  "ApiKey": "your-api-key",
  "DeploymentName": "gpt-4"
}
```

### **4. Run the API**  
```bash
dotnet run
```
API will be available at **`http://localhost:5000/swagger`**  

---

## **Endpoints**  

### **User Management**  
📌 **POST** `/api/users` → Create a new user  
📌 **GET** `/api/users/{id}` → Get user details  

### **Financial Summary**  
📌 **GET** `/api/financial-summary/{userId}` → Retrieve financial summary  
📌 **PUT** `/api/financial-summary/{userId}` → Update financial summary  

### **Buckets (Savings Goals)**  
📌 **POST** `/api/buckets/{userId}` → Create a new savings goal  
📌 **GET** `/api/buckets/{userId}` → List all buckets for a user  
📌 **PUT** `/api/buckets/{userId}/{bucketId}` → Update a savings goal  
📌 **DELETE** `/api/buckets/{userId}/{bucketId}` → Delete a savings goal  

### **AI-Powered Budget Analysis (Microsoft Semantic Kernel)**  
📌 **GET** `/api/buckets/analyze/{userId}` → AI analysis on user’s financial health  
📌 **POST** `/api/buckets/adjust/{userId}/{bucketId}` → AI-driven savings recommendations  

---

## **AI-Driven Budget Analysis (Microsoft Semantic Kernel + Azure OpenAI)**  

The **AI agent** fetches a user's financial data and provides **smart recommendations** based on:  
✅ **Savings Timeline** – How long will it take to reach a goal?  
✅ **Spending Adjustments** – Suggested expense cuts to save more  
✅ **Probability Score (0-1)** – Likelihood of achieving the goal in time  

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

---

## **Azure Deployment (Future Plans)**  
🔹 **Azure App Service** – Host the API for cloud accessibility  
🔹 **Azure Key Vault** – Secure storage of API keys and database credentials  
🔹 **Azure Monitor** – Track API performance and usage  

---

## **Contributing**  
1. **Fork** the repository  
2. **Create a feature branch**  
3. **Submit a pull request**  

---

## **Future Enhancements**  
✅ AI chatbot for personalized financial coaching  
✅ Auto-transfer savings recommendations  
✅ Mobile app integration  

---

## **License**  
MIT License.  

---

Let me know if you'd like any modifications!