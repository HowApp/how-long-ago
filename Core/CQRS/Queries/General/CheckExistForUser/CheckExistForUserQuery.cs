namespace How.Core.CQRS.Queries.General.CheckExistForUser;

using Common.CQRS;
using Common.ResultType;

public sealed class CheckExistForUserQuery : IQuery<Result<bool>>
{
    public int CurrentUserId { get; set; }
    public int Id { get; set; }
    public string Table { get; set; }
    public bool WithShared { get; set; }
}