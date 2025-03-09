# Importing OpenAPI Specification into Azure API Management

## Preparing Your OpenAPI Specification

1. **Validate your specification**:
   - Use the Swagger Editor (https://editor.swagger.io/) to validate your YAML
   - Fix any errors or warnings before importing

2. **Make sure your specification includes**:
   - Proper server URLs that match your backend endpoints
   - Security definitions (in our case, JWT Bearer authentication)
   - Complete schema definitions for requests and responses

## Import Methods

### Method 1: Import via Azure Portal

1. Go to your Azure API Management service
2. Select **APIs** from the menu
3. Click **+ Add API**
4. Select **OpenAPI** from the available options
5. Choose one of the following:
   - **OpenAPI specification** - Upload or paste your YAML/JSON specification
   - **OpenAPI link** - Provide a URL to your specification file 
   - *(Note: If using URL, it must be publicly accessible)*
6. Configure these settings:
   - **Display name**: FinBin API
   - **Name**: finbin-api
   - **API URL suffix**: finbin
   - **Products**: Associate with your products (optional)
   - **Tags**: Add relevant tags (optional)
   - **Gateway URL**: Will be auto-generated
7. Click **Create**

### Method 2: Import via Azure CLI

```bash
# Upload API definition from local file
az apim api import --resource-group rg-foundrysandbox \
  --service-name finbin-apim \
  --path "finbin" \
  --display-name "FinBin API" \
  --api-id finbin-api \
  --specification-format OpenApi \
  --specification-path "c:\Users\Brandon\finbinBackend\finbin-openapi.yaml"
```

### Method 3: Import via PowerShell

```powershell
$context = New-AzApiManagementContext -ResourceGroupName "rg-foundrysandbox" -ServiceName "finbin-apim"
$openApiSpec = Get-Content -Path "c:\Users\Brandon\finbinBackend\finbin-openapi.yaml" -Raw
Import-AzApiManagementApi -Context $context -SpecificationFormat "OpenApi" -SpecificationPath $openApiSpec -Path "finbin" -ApiId "finbin-api"
```

## After Importing

1. **Test your API operations**:
   - Use the built-in test console in API Management
   - Verify request/response formats match your expectations

2. **Configure policies** (if not defined in OpenAPI):
   - Add JWT validation
   - Set rate limiting
   - Configure caching for appropriate endpoints

3. **Set up backend service URL**:
   - Verify the backend service URL points to your App Service
   - Configure backend authentication (Managed Identity if applicable)

4. **Create products and subscriptions**:
   - Add your API to appropriate products
   - Generate subscription keys for consumers

5. **Enable CORS** (if needed):
   - Add CORS policy at API or operation level
   - Configure allowed origins for your frontend

## Versioning and Revisions

- Use **revisions** for non-breaking changes to your API
- Use **versions** when introducing breaking changes
- Configure **API versioning scheme** (path, header, or query parameter)

## Troubleshooting Common Import Issues

- **Schema validation errors**: Ensure your OpenAPI spec conforms to the OpenAPI 3.0 specification
- **Server URL issues**: Make sure server URLs are properly defined and accessible
- **Authentication problems**: Verify security definitions match your backend configuration
- **Operation conflicts**: Check for duplicate operation IDs or paths
