namespace How.Core.Database;

using Npgsql;

public class DapperConnection
{
    private readonly string _connectionString;
    private readonly string _temporaryConnectionString;

    public DapperConnection(string connectionString, string temporaryConnectionString)
    {
        _connectionString = connectionString;
        _temporaryConnectionString = temporaryConnectionString;
    }

    public NpgsqlConnection InitConnection()
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
        dataSourceBuilder.UseNodaTime();
        var dataSource = dataSourceBuilder.Build();
        
        var connection = dataSource.OpenConnection();
        return connection;
    }
    
    public NpgsqlConnection InitTemporaryConnection()
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_temporaryConnectionString);
        var dataSource = dataSourceBuilder.Build();
        
        var connection = dataSource.OpenConnection();
        return connection;
    }
}