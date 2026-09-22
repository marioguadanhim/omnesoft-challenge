using OmnesoftChallenge.DAL.Entities;

namespace OmnesoftChallenge.DAL.Interfaces.Repository;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task RevokeUserTokensAsync(int systemUserId);
}
