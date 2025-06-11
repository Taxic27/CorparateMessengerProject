using Dapper;
using Npgsql;
using System.Data;

namespace Server.Tools;

public class MainConnector : IMainConnector
{
    private readonly string _connectionString;

    public MainConnector(string connectionString)
    {
        _connectionString = connectionString;
    }

    public T Get<T>(String sqlCommand, DynamicParameters parameters)
    {
        using (IDbConnection connection = new NpgsqlConnection(_connectionString))
        {
            return connection.Query<T>(sqlCommand, parameters).FirstOrDefault();
        }
    }

    public List<T> GetList<T>(String sqlCommand, DynamicParameters parameters = null)
    {
        using (IDbConnection connection = new NpgsqlConnection(_connectionString))
        {
            return connection.Query<T>(sqlCommand, parameters).ToList();
        }
    }

    public void Execute(String sqlCommand, DynamicParameters parameters)
    {
        using (IDbConnection connection = new NpgsqlConnection(_connectionString))
        {
            connection.Execute(sqlCommand, parameters);
        }
    }
}
