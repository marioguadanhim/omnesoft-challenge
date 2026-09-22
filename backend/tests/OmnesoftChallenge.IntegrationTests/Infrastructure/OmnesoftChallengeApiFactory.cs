using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace OmnesoftChallenge.IntegrationTests.Infrastructure;

public class OmnesoftChallengeApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string PostgresImage = "postgres:18.3";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder(PostgresImage)
        .WithDatabase("OmnesoftChallengeDB")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string ConnectionString => _postgres.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:OmnesoftChallengeDB"] = ConnectionString
            }));
    }

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        StartServer();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
