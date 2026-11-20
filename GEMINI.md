# `how-long-ago` (Main API) — Context & Rules

## Project Structure (`How.sln`)
- **`Common` (`How.Common`)**: `Result`/`Error` types, CQRS interfaces (`ICommand`, `IQuery`), `PaginationDTO`, `FileSignatureValidator`, `user_account.proto`.
- **`Core` (`How.Core`)**: Entities, `BaseDbContext` (`public` schema), `TemporaryStorageDbContext` (`temporary` schema), `DapperConnection`, MediatR CQRS Handlers, FluentValidation validators, `FileProcessingHub` (SignalR), `UserAccountGrpcService`, MassTransit consumers, and `BackgroundImageProcessing`.
- **`Server` (`How.Server`)**: Controllers (`Account`, `Dashboard`, `Identity`, `Public`), EF migrations, Kestrel setup (`7060` REST/SignalR, `7061` gRPC).

## Rules
- Ensure `DapperConnection` reuses singleton `NpgsqlDataSource` instances for the main and temporary schemas.
- Never call `GC.Collect()` inside request or image processing paths.
- Use `@createdById` consistently in `EventAccessQueryAccessBuilder` and `RecordAccessAccessBuilder`.
- Always check `result.Failed` before dereferencing `result.Data` in controllers and background services.
- Ensure `BaseController.HttpResult` uses `result.Code` for non-200 HTTP responses.
