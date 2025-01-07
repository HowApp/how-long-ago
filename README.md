# how-long-ago

to add migrations

```csharp
dotnet ef migrations add {migration_name} --project Server/How.Server.csproj --startup-project Server/How.Server.csproj --context How.Core.Database.BaseDbContext --output-dir Migrations/PublicSchema
```
```csharp
dotnet ef migrations add {migration_name} --project Server/How.Server.csproj --startup-project Server/How.Server.csproj --context How.Core.Database.TemporaryStorageDbContext --output-dir Migrations/TemporarySchema
```