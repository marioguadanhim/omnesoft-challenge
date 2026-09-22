using ErrorOr;
using OmnesoftChallenge.BLL.Interfaces.Security;
using OmnesoftChallenge.BLL.Utils.Validators;
using OmnesoftChallenge.BLL.ViewModels.LoginContracts;
using OmnesoftChallenge.DAL.Entities;
using OmnesoftChallenge.DAL.Interfaces.Repository;

namespace OmnesoftChallenge.BLL.Utils.Security;

public class UserAuthenticationService(
    ISystemUserRepository systemUserRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenRepository refreshTokenRepository) : IUserAuthenticationService
{
    private readonly ISystemUserRepository _systemUserRepository = systemUserRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<ErrorOr<OmnesoftLoginResponse>> AuthenticateUserForLogin(string userName, string password)
    {
        var errors = RequestValidator.ValidateLogin(userName, password);
        if (errors.Count > 0)
        {
            return errors;
        }

        var systemUser = await ValidateUserCredentials(userName, password);
        if (systemUser is null || !systemUser.Active)
        {
            return Error.Unauthorized(
                code: ErrorCodes.AuthenticationError,
                description: "Invalid username or password"
            );
        }

        var loginResponse = GenerateTokensForUser(systemUser);

        await SaveRefreshToken(loginResponse.RefreshToken, systemUser.Id);

        return loginResponse;
    }

    public async Task<ErrorOr<OmnesoftLoginResponse>> RefreshToken(string refreshToken)
    {
        var errors = RequestValidator.ValidateRefreshToken(refreshToken);
        if (errors.Count > 0)
        {
            return errors;
        }

        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (token is null || !token.IsActive)
        {
            return Error.Unauthorized(
                code: ErrorCodes.RefreshTokenError,
                description: "Invalid or expired refresh token"
            );
        }

        await RevokeRefreshToken(token);

        var loginResponse = GenerateTokensForUser(token.SystemUser);

        await SaveRefreshToken(loginResponse.RefreshToken, token.SystemUserId);

        return loginResponse;
    }

    #region Private Helper Methods

    private async Task<SystemUser?> ValidateUserCredentials(string userName, string password)
    {
        string hashedPassword = _passwordHasher.HashPasswordWithKey(password);
        return await _systemUserRepository.GetForLogin(userName, hashedPassword);
    }

    private OmnesoftLoginResponse GenerateTokensForUser(SystemUser systemUser)
    {
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(
            systemUser.UserName,
            systemUser.Id.ToString(),
            systemUser.Role
        );

        return new OmnesoftLoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = _jwtTokenGenerator.GenerateRefreshToken(),
            ExpiresAt = _jwtTokenGenerator.GetAccessTokenExpiration(),
            UserName = systemUser.UserName,
            Role = systemUser.Role
        };
    }

    private async Task SaveRefreshToken(string token, int systemUserId)
    {
        var refreshTokenEntity = new RefreshToken
        {
            Token = token,
            SystemUserId = systemUserId,
            ExpiresAt = _jwtTokenGenerator.GetRefreshTokenExpiration(),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
    }

    private async Task RevokeRefreshToken(RefreshToken token)
    {
        token.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(token);
    }

    #endregion
}
