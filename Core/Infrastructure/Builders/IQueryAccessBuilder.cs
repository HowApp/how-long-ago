namespace How.Core.Infrastructure.Builders;

using Dapper;

public interface IQueryAccessBuilder
{
    (string sqlQuery, DynamicParameters paramsQuery) BuildQuery();
}