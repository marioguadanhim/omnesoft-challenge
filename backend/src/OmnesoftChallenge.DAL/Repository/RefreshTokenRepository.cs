using Dapper;
using Microsoft.EntityFrameworkCore;
using OmnesoftChallenge.DAL.Context;
using OmnesoftChallenge.DAL.Entities;
using OmnesoftChallenge.DAL.Interfaces.Repository;
using System.Data;

namespace OmnesoftChallenge.DAL.Repository;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly EntityFrameworkContext _context;
    private readonly DapperContext _dapperContext;

    public RefreshTokenRepository(EntityFrameworkContext context, DapperContext dapperContext)
    {
        _context = context;
        _dapperContext = dapperContext;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        using (IDbConnection db = _dapperContext.CreateConnection())
        {
            var query = @"
                SELECT rt.id, rt.token, rt.system_user_id, rt.expires_at, rt.created_at, rt.revoked_at,
                       su.id, su.user_name, su.password, su.role, su.active
                FROM refresh_token rt
                INNER JOIN ""system_user"" su ON su.id = rt.system_user_id
                WHERE rt.token = @Token";

            var result = await db.QueryAsync<RefreshToken, SystemUser, RefreshToken>(
                query,
                (refreshToken, systemUser) =>
                {
                    refreshToken.SystemUser = systemUser;
                    return refreshToken;
                },
                new { Token = token },
                splitOn: "id");

            return result.FirstOrDefault();
        }
    }

    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshToken.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshToken.Update(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeUserTokensAsync(int systemUserId)
    {
        var tokens = await _context.RefreshToken
            .Where(x => x.SystemUserId == systemUserId && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        _context.RefreshToken.UpdateRange(tokens);
        await _context.SaveChangesAsync();
    }
}
