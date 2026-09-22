using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OmnesoftChallenge.Api.Endpoints.Errors;
using OmnesoftChallenge.Api.Extensions;
using OmnesoftChallenge.BLL.Interfaces.Products;
using OmnesoftChallenge.BLL.Interfaces.Security;
using OmnesoftChallenge.BLL.Services;
using OmnesoftChallenge.BLL.Utils.Security;
using OmnesoftChallenge.BLL.Utils.Security.Hashers;
using OmnesoftChallenge.DAL.Context;
using OmnesoftChallenge.DAL.Interfaces.Repository;
using OmnesoftChallenge.DAL.Repository;
using OmnesoftChallenge.DAL.Repository.Base;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ISystemUserRepository, SystemUserRepository>();
builder.Services.AddScoped<EntityFrameworkContext>();
builder.Services.AddScoped<DapperContext>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddFastEndpoints();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = builder.Configuration["JwtSettings:SecretKey"]
        ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("GuestOnly", policy =>
        policy.RequireRole("Guest"));

    options.AddPolicy("AdminOrGuest", policy =>
        policy.RequireRole("Admin", "Guest"));
});

builder.Services.SwaggerDocument(options =>
{
    options.DocumentSettings = settings =>
    {
        settings.Title = "OmnesoftChallenge API";
        settings.Version = "v1";
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

await EnsureDatabaseIsUpToDateAsync(app);

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(config =>
{
    config.Validation.UsePropertyNamingPolicy = false;
    config.Errors.ResponseBuilder = (failures, _, _) =>
        HandledErrorResponse.Body(failures[0].PropertyName, failures.Select(failure => failure.ErrorMessage));
});

app.UseSwaggerGen();

app.MapDefaultEndpoints();

app.Run();

static async Task EnsureDatabaseIsUpToDateAsync(WebApplication app)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EntityFrameworkContext>();
        var connection = context.Database.GetDbConnection();

        app.Logger.LogInformation(
            "Checking database {Database} on {DataSource} for pending migrations",
            connection.Database,
            connection.DataSource);

        if (context.Database.HasPendingModelChanges())
            throw new InvalidOperationException(
                "The entity model has changes that no migration captures. Run 'dotnet ef migrations add <Name>' and start again.");

        List<string> pendingMigrations = [.. await context.Database.GetPendingMigrationsAsync()];

        if (pendingMigrations.Count == 0)
        {
            List<string> appliedMigrations = [.. await context.Database.GetAppliedMigrationsAsync()];

            app.Logger.LogInformation(
                "Database is up to date: {AppliedCount} migration(s) applied, latest {LatestMigration}. Nothing to migrate.",
                appliedMigrations.Count,
                appliedMigrations.LastOrDefault());

            return;
        }

        app.Logger.LogInformation(
            "Database is not up to date: {PendingCount} pending migration(s) {PendingMigrations}. Applying them now.",
            pendingMigrations.Count,
            pendingMigrations);

        await context.Database.MigrateAsync();

        List<string> remainingMigrations = [.. await context.Database.GetPendingMigrationsAsync()];

        if (remainingMigrations.Count > 0)
            throw new InvalidOperationException(
                $"Migrations are still pending after migrating: {string.Join(", ", remainingMigrations)}");

        app.Logger.LogInformation(
            "Database migrated and up to date: applied {AppliedCount} migration(s) {AppliedMigrations}",
            pendingMigrations.Count,
            pendingMigrations);
    }
    catch (Exception ex)
    {
        app.Logger.LogCritical(ex, "Database is not up to date and could not be migrated. The API will not start.");
        throw;
    }
}
