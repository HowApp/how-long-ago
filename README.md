# how-long-ago

to add migrations

```csharp
dotnet ef migrations add {migration_name} --project Server/How.Server.csproj --startup-project Server/How.Server.csproj --context How.Core.Database.BaseDbContext --output-dir Migrations/PublicSchema
```
```csharp
dotnet ef migrations add {migration_name} --project Server/How.Server.csproj --startup-project Server/How.Server.csproj --context How.Core.Database.TemporaryStorageDbContext --output-dir Migrations/TemporarySchema
```

example of secrets to run app
```json
{
  "BaseApplicationSettings": {
    "AllowedOrigins": [
      "https://localhost:7560",
      "http://localhost:5560"
    ]
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost; Port=5432; Database=/your_database_name/; Username=/your_username/; Password=/your_password/;",
    "TemporaryStorageConnection": "Host=localhost; Port=5432; Database=/your_database_name/; Username=/your_username/; Password=/your_password/; SearchPath=temporary"
  },
  "AdminCredentials": {
    "UserId": "1",
    "FirstName": "SuperAdmin",
    "LastName": "god-mode"
  },
  "IdentityServerConfiguration": {
    "Authority": "https://localhost:5001",
    "Audience": "https://localhost:5001/resources",
    "ClientId": "resource.how-api",
    "ClientSecret": "/your_secret_for_resource/",
    "ClientSwaggerSecret": "/your_secret_for_swagger/"
  },
  "RabbitMqConfiguration": {
    "Host": "127.0.0.1",
    "User": "admin",
    "Password": "admin",
    "Port": 5672
  }
}
```