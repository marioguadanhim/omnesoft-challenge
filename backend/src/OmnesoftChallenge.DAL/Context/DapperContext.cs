using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace OmnesoftChallenge.DAL.Context;

public class DapperContext(IConfiguration configuration)
{
    static DapperContext()
    {
        // Columns are snake_case in Postgres, so insertion_date maps to InsertionDate
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    private readonly string _connectionString = configuration.GetConnectionString("OmnesoftChallengeDB")
        ?? throw new InvalidOperationException("ConnectionStrings:OmnesoftChallengeDB is not configured");

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
