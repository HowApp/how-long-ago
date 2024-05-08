namespace How.Core.Database.DapperConnection;

using Npgsql;

public class DapperConnection
{
    private readonly string _connectionString;

    public DapperConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public NpgsqlConnection InitConnection()
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
        dataSourceBuilder.UseNodaTime();
        var dataSource = dataSourceBuilder.Build();
        
        var connection = dataSource.OpenConnection();
        connection.Open();
        return connection;
    }
}