namespace How.Core.Infrastructure.NpgsqlExtensions;

using System.Data;
using Dapper;
using NodaTime;
using Npgsql;
using NpgsqlTypes;

public class InstantHandler : SqlMapper.TypeHandler<Instant>
{
    private InstantHandler()
    {
    }

    public static readonly InstantHandler Default = new InstantHandler();
    
    public override void SetValue(IDbDataParameter parameter, Instant value)
    {
        parameter.Value = value.ToDateTimeUtc();

        if (parameter is NpgsqlParameter sqlParameter)
        {
            sqlParameter.NpgsqlDbType = NpgsqlDbType.TimestampTz;
        }
    }

    public override Instant Parse(object value)
    {
        if (value is DateTime dateTime)
        {
            var dt = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return Instant.FromDateTimeUtc(dt);
        }

        if (value is Instant instant)
        {
            return instant;
        }
        
        if (value is DateTimeOffset dateTimeOffset)
        {
            return Instant.FromDateTimeOffset(dateTimeOffset);
        }

        throw new DataException("Error while converting type " + value.GetType() + " to NodaTime.Instant");
    }
}