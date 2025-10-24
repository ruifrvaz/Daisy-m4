# Daisy MCP-Compliant API

This is a Model Context Protocol (MCP) compliant ASP.NET API for the Daisy workflow orchestration engine. The API provides OAuth 2.0 authentication and OData query capabilities for all available functionalities.

## Features

- **MCP Compliance**: Follows Model Context Protocol standards for API design
- **OAuth 2.0 Authentication**: Secure JWT Bearer token authentication
- **OData Support**: Full OData query capabilities (filter, select, orderby, expand, count)
- **RESTful Design**: Standard CRUD operations on all resources
- **Comprehensive Coverage**: Access to Impulses, Receivers, Abilities, Transmitters, and Workflows
- **Swagger Documentation**: Interactive API documentation at the root endpoint

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Basic understanding of OAuth 2.0 and OData

### Running the API

1. Navigate to the API directory:
```bash
cd src/Daisy.Api
```

2. Run the API:
```bash
dotnet run
```

3. Access Swagger UI at: `https://localhost:<port>/` (or `http://localhost:<port>/`)

### Authentication

Before accessing protected endpoints, you need to obtain a JWT token:

**Request:**
```http
POST /api/auth/token
Content-Type: application/json

{
  "username": "admin",
  "password": "admin"
}
```

**Response:**
```json
{
  "access_token": "eyJhbGc...",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

**Using the token:**
Include the token in the Authorization header:
```http
Authorization: Bearer eyJhbGc...
```

## API Endpoints

All OData endpoints are prefixed with `/odata` and support standard OData query options.

### Impulses

The central data carriers in Daisy workflows.

- `GET /odata/Impulses` - Get all impulses (with OData query support)
- `GET /odata/Impulses({id})` - Get a specific impulse
- `POST /odata/Impulses` - Create a new impulse
- `PUT /odata/Impulses({id})` - Update an impulse
- `DELETE /odata/Impulses({id})` - Delete an impulse

**Example OData Queries:**
```
GET /odata/Impulses?$filter=Status eq 'Processed'
GET /odata/Impulses?$orderby=CreatedAt desc
GET /odata/Impulses?$select=Id,Status,CreatedAt
GET /odata/Impulses?$top=10&$skip=20
```

### Receivers

Entry points that ingest external input.

- `GET /odata/Receivers` - Get all receivers
- `GET /odata/Receivers('{name}')` - Get a specific receiver
- `POST /odata/Receivers('{name}')/Trigger` - Trigger a receiver with input

**Example:**
```json
POST /odata/Receivers('ConsoleReceiver')/Trigger
{
  "input": "Hello, Daisy!",
  "metadata": {
    "source": "api"
  }
}
```

### Abilities

Processing units that transform impulses.

- `GET /odata/Abilities` - Get all abilities
- `GET /odata/Abilities('{name}')` - Get a specific ability
- `POST /odata/Abilities('{name}')/Execute` - Execute an ability with an impulse

**Example:**
```json
POST /odata/Abilities('WeatherAbility')/Execute
{
  "impulseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "pathName": "FetchWeather"
}
```

### Transmitters

Output processors that send results to external systems.

- `GET /odata/Transmitters` - Get all transmitters
- `GET /odata/Transmitters('{name}')` - Get a specific transmitter
- `POST /odata/Transmitters('{name}')/Transmit` - Transmit an impulse

**Example:**
```json
POST /odata/Transmitters('ConsoleTransmitter')/Transmit
{
  "impulseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Workflows

Execution containers that manage workflow lifecycles.

- `GET /odata/Workflows` - Get all workflows
- `GET /odata/Workflows('{name}')` - Get a specific workflow
- `GET /odata/Workflows('{name}')/Status` - Get workflow status

## OData Query Options

The API supports the following OData query options:

- `$filter` - Filter results based on conditions
- `$select` - Select specific properties
- `$orderby` - Sort results
- `$top` - Limit number of results
- `$skip` - Skip a number of results
- `$count` - Include total count
- `$expand` - Expand related entities

## Configuration

### JWT Settings

Edit `appsettings.json` to configure JWT authentication:

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "DaisyApi",
    "Audience": "DaisyApiClients"
  },
  "Authentication": {
    "DefaultUsername": "admin",
    "DefaultPassword": "admin"
  }
}
```

**Important:** Change the default credentials and secret key in production!

## Security Considerations

1. **Change Default Credentials**: Update the default username and password
2. **Secure Secret Key**: Use a strong, randomly generated secret key
3. **HTTPS Only**: Always use HTTPS in production
4. **Token Expiration**: JWT tokens expire after 1 hour
5. **User Store**: Implement proper user authentication against a database in production

## Example Workflow

Here's a complete workflow example using the API:

```bash
# 1. Get authentication token
TOKEN=$(curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin"}' \
  | jq -r '.access_token')

# 2. Create an impulse
IMPULSE_ID=$(curl -X POST http://localhost:5000/odata/Impulses \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"input":"Process this data","isLoopback":false}' \
  | jq -r '.id')

# 3. List all receivers
curl -X GET "http://localhost:5000/odata/Receivers" \
  -H "Authorization: Bearer $TOKEN"

# 4. Trigger a receiver
curl -X POST "http://localhost:5000/odata/Receivers('ConsoleReceiver')/Trigger" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"input":"Hello from API"}'

# 5. Get all impulses with OData query
curl -X GET "http://localhost:5000/odata/Impulses?\$filter=Status eq 'Created'&\$orderby=CreatedAt desc" \
  -H "Authorization: Bearer $TOKEN"
```

## Development

### Adding Custom Endpoints

To add custom endpoints, create a new controller in `Controllers/` directory:

```csharp
[ApiController]
[Route("odata/[controller]")]
[Authorize]
public class MyController : ODataController
{
    // Your implementation
}
```

### Extending the EDM Model

Update the `GetEdmModel()` method in `Program.cs` to add new entity types:

```csharp
modelBuilder.EntitySet<MyDto>("MyEntities").EntityType.HasKey(e => e.Id);
```

## Troubleshooting

### Authentication Issues

- Verify the token is included in the Authorization header
- Check token expiration
- Ensure the secret key matches in appsettings.json

### OData Query Issues

- URL encode special characters in queries
- Use single quotes for string values in filters
- Check OData syntax (e.g., `$filter` not `filter`)

## License

Apache License 2.0 © 2025 Rui Filipe Rodrigues Vaz. See LICENSE and NOTICE for details.
