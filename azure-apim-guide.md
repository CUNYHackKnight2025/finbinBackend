# Azure API Management Guide for FinBin Backend

## What is API Management?

Azure API Management (APIM) is a complete solution for publishing, securing, transforming, maintaining, and monitoring APIs. For your FinBin application, it sits as a façade between your backend services and API consumers.

## Getting Started with API Management

### 1. Create an API Management Service

1. In Azure Portal, go to **Create a resource** > **Integration** > **API Management**
2. Fill in the basics:
   - **Name**: `finbin-apim` (must be unique)
   - **Subscription**: Your subscription
   - **Resource Group**: Use the same as your App Service (`rg-foundrysandbox`)
   - **Location**: Same region as your App Service
   - **Organization name**: Your organization
   - **Administrator email**: Your email
   - **Pricing tier**: Developer (for testing) or Basic/Standard (for production)

### 2. Import Your FinBin API

#### Option A: Import from OpenAPI (Swagger)

1. Go to your API Management service
2. Select **APIs** > **+ Add API** > **OpenAPI**
3. Enter the URL to your Swagger endpoint: `https://finbinbackend.azurewebsites.net/swagger/v1/swagger.json`
   - **Note**: This URL must be publicly accessible for API Management to import it
4. Configure:
   - **Display name**: `FinBin API`
   - **Name**: `finbin-api`
   - **API URL suffix**: `finbin`
   - **Base URL**: Your backend URL
   - **Version**: `v1`

#### Option B: Import from App Service

1. Go to your API Management service
2. Select **APIs** > **+ Add API** > **App Service**
3. Select your App Service (`finbinbackend`)
4. Configure similar settings as above

### 2.1 Configure API Definition (Swagger/OpenAPI)

For API Management to properly discover and import your API:

1. **Make sure your Swagger endpoint is publicly accessible**:
   - In App Service, ensure that the Swagger endpoint doesn't require authentication
   - Temporarily disable any IP restrictions that might block APIM from accessing it
   - Test the URL in an incognito browser to verify it's publicly accessible

2. **Set up CORS for your Swagger endpoint** (if needed):
   ```csharp
   app.UseSwagger(c => {
       c.PreSerializeFilters.Add((swaggerDoc, httpReq) => {
           swaggerDoc.Servers = new List<OpenApiServer> { 
               new OpenApiServer { Url = $"https://{httpReq.Host.Value}" } 
           };
       });
   });
   ```

3. **Configure API definition in APIM**:
   - Go to your API in APIM
   - Select **Settings** tab
   - Under **API definition**, choose "OpenAPI" format
   - Enter your Swagger URL or paste the JSON content directly
   - Click "Save" to update the definition

4. **Keep your Swagger documentation synchronized**:
   - Set up automatic import from your Swagger endpoint
   - In your API settings, enable "Always use latest API specification"
   - Configure a schedule for automatic updates

5. **Add API definition to Developer Portal**:
   - Enable the Developer Portal
   - Make sure API documentation is enabled
   - Customize the documentation appearance

### 3. Configure Security

#### Set Up JWT Validation

1. Go to your API > **Settings**
2. In the **Security** tab, add a JWT validation policy:
   ```xml
   <validate-jwt header-name="Authorization" failed-validation-httpcode="401" failed-validation-error-message="Unauthorized">
     <openid-config url="your-auth-metadata-endpoint" />
     <audiences>
         <audience>same-as-your-JWT-audience</audience>
     </audiences>
     <issuers>
         <issuer>same-as-your-JWT-issuer</issuer>
     </issuers>
   </validate-jwt>
   ```

### 4. Configure Policies

At the API level, add these important policies:

1. **CORS Policy** (if needed):
   ```xml
   <cors allow-credentials="true">
     <allowed-origins>
       <origin>https://your-frontend-url.azurewebsites.net</origin>
     </allowed-origins>
     <allowed-methods preflight-result-max-age="300">
       <method>GET</method>
       <method>POST</method>
       <method>PUT</method>
       <method>DELETE</method>
     </allowed-methods>
     <allowed-headers>
       <header>*</header>
     </allowed-headers>
   </cors>
   ```

2. **Rate Limiting**:
   ```xml
   <rate-limit calls="20" renewal-period="60" />
   ```

3. **Caching** (for appropriate endpoints):
   ```xml
   <cache-lookup vary-by-developer="false" vary-by-developer-groups="false" downstream-caching-type="none" />
   <cache-store duration="60" />
   ```

### 5. Set Up Products

1. Create a Product (e.g., "FinBin Standard")
2. Add your API to the product
3. Configure access control (subscription required or open)
4. Publish the product

### 6. Configure Diagnostics and Monitoring

1. Go to **Diagnostics** in your API Management
2. Create a new diagnostic setting
3. Send logs to:
   - Application Insights (same one used by your App Service)
   - Log Analytics Workspace

### 7. Connect to Backend with Managed Identity

1. Enable System Assigned Managed Identity for your API Management
2. Grant necessary permissions to access:
   - Azure SQL Database
   - Azure OpenAI service
3. Configure backend with Managed Identity authentication:
   ```xml
   <authentication-managed-identity resource="https://management.azure.com/" />
   ```

### 8. Developer Portal (Optional)

1. Go to **Developer Portal** > **Portal Overview**
2. Customize branding and content
3. Publish the portal

## Testing Your API

1. Go to **APIs** > Your API > **Test** tab
2. Select an operation
3. Configure any parameters
4. Add Authorization header if required
5. Click **Send**

## Best Practices for FinBin

1. **Layer Security**:
   - Use API keys for frontend applications
   - Use OAuth/JWT for user authentication
   - Use Managed Identity for backend-to-backend communication

2. **Implement Versioning**:
   - Use URL path versioning (/v1/, /v2/)
   - Use API versioning in APIM

3. **Set Up Monitoring**:
   - Monitor API usage and performance
   - Set up alerts for error thresholds
   - Review logs regularly

4. **Deploy Changes Safely**:
   - Use separate instances for dev/test/prod
   - Use revisions for non-breaking changes
   - Use versions for breaking changes

## Next Steps

1. Set up a custom domain for your API Management
2. Implement advanced policies (transformation, validation)
3. Configure mutual TLS for backend communication
4. Set up a CI/CD pipeline for API Management using Azure DevOps or GitHub Actions
