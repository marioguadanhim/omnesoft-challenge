using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using OmnesoftChallenge.DAL.EntitiyMapping;
using OmnesoftChallenge.DAL.Entities;

namespace OmnesoftChallenge.DAL.Context;

public class EntityFrameworkContext(IConfiguration configuration) : DbContext
{
    private readonly IConfiguration _configuration = configuration;

    public DbSet<Product> Product { get; set; }
    public DbSet<SystemUser> SystemUser { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connection = _configuration.GetConnectionString("OmnesoftChallengeDB");
        optionsBuilder.UseNpgsql(connection).UseSnakeCaseNamingConvention();
        // Npgsql only quotes identifiers it knows are reserved and misses system_user, so quote them all
        optionsBuilder.ReplaceService<ISqlGenerationHelper, RelationalSqlGenerationHelper>();
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductMapping());
        modelBuilder.ApplyConfiguration(new SystemUserMapping());
        modelBuilder.ApplyConfiguration(new RefreshTokenMapping());

        modelBuilder.Entity<SystemUser>().HasData(
           new SystemUser
           {
               Id = 1,
               UserName = "admin",
               Password = "GqUOUAuIMkk/PrMHRvVxltY1BsgjTSbADpJTN+WrbdI=",
               Role = UserRoles.Admin.ToString()
           },
           new SystemUser
           {
               Id = 2,
               UserName = "guest",
               Password = "6HaA0dxGQcaL5PHuRJm+WAkW6s+0h7KHFA6Y5zFPHsg=",
               Role = UserRoles.Guest.ToString()
           }
        );

        modelBuilder.Entity<Product>().HasData(
           new Product
           {
               Id = 1,
               Name = "Sample Product",
               Price = 10.99m,
               Description = "A sample product for testing.",
               InsertionDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
               LastUpdateDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
           }
        );
    }
}
