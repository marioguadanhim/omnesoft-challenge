namespace OmnesoftChallenge.BLL.Interfaces.Security;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(string userName, string userId, params string[] roles);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiration();
    DateTime GetRefreshTokenExpiration();
}
