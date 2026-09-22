using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OmnesoftChallenge.BLL.Interfaces.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OmnesoftChallenge.BLL.Utils.Security.Hashers;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _secretKey;
    private readonly int _accessTokenExpirationInDays;
    private readonly int _refreshTokenExpirationInDays;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _secretKey = configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("Not possible to find JwtSettings:SecretKey");

        var expirationInDays = configuration["JwtSettings:ExpirationInDays"]
            ?? throw new InvalidOperationException("Not possible to find JwtSettings:ExpirationInDays");

        var refreshTokenExpirationInDays = configuration["JwtSettings:RefreshTokenExpirationDays"]
            ?? throw new InvalidOperationException("Not possible to find JwtSettings:RefreshTokenExpirationDays");

        _issuer = configuration["JwtSettings:Issuer"]
            ?? throw new InvalidOperationException("Not possible to find JwtSettings:Issuer");

        _audience = configuration["JwtSettings:Audience"]
            ?? throw new InvalidOperationException("Not possible to find JwtSettings:Audience");

        _accessTokenExpirationInDays = int.Parse(expirationInDays);
        _refreshTokenExpirationInDays = int.Parse(refreshTokenExpirationInDays);
    }

    public string GenerateAccessToken(string userName, string userId, params string[] roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = GetAccessTokenExpiration(),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public DateTime GetAccessTokenExpiration() => DateTime.UtcNow.AddDays(_accessTokenExpirationInDays);

    public DateTime GetRefreshTokenExpiration() => DateTime.UtcNow.AddDays(_refreshTokenExpirationInDays);
}
