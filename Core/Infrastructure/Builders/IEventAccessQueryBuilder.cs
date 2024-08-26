namespace How.Core.Infrastructure.Builders;

using Dapper;
using Enums;

public interface IEventAccessQueryBuilder
{
    void Init(int recordId);
    void FilterCreatedBy(int userId, AccessFilterType accessFilterType);
    void FilterByEventStatus(EventStatus status);
    void FilterByEventAccessType(EventAccessType accessType);
    (string sqlQuery, DynamicParameters paramsQuery) BuildQuery();
}