namespace How.Core.CQRS.Queries.General.CheckExistForUser;

using Common.CQRS;
using Common.ResultType;
using Infrastructure.Enums;

public sealed class CheckExistForUserQuery : IQuery<Result<bool>>
{
    public int CurrentUserId { get; set; }
    public int Id { get; set; }
    public string Table { get; set; }
    public FilterType FilterType { get; set; }
}