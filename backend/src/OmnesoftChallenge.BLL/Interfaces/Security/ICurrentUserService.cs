namespace OmnesoftChallenge.BLL.Interfaces.Security;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}
