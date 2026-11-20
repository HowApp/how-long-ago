namespace How.Core.Database;

using Npgsql;

public class DapperConnection : IDisposable, IAsyncDisposable
{
    private readonly NpgsqlDataSource _mainDataSource;
    private readonly NpgsqlDataSource _tempDataSource;

    public DapperConnection(string connectionString, string temporaryConnectionString)
    {
        var mainBuilder = new NpgsqlDataSourceBuilder(connectionString);
        mainBuilder.UseNodaTime();
        _mainDataSource = mainBuilder.Build();

        var tempBuilder = new NpgsqlDataSourceBuilder(temporaryConnectionString);
        tempBuilder.UseNodaTime();
        _tempDataSource = tempBuilder.Build();
    }

    public NpgsqlConnection InitConnection()
    {
        return _mainDataSource.OpenConnection();
    }

    public NpgsqlConnection InitTemporaryConnection()
    {
        return _tempDataSource.OpenConnection();
    }

    public void Dispose()
    {
        _mainDataSource.Dispose();
        _tempDataSource.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _mainDataSource.DisposeAsync();
        await _tempDataSource.DisposeAsync();
    }
}