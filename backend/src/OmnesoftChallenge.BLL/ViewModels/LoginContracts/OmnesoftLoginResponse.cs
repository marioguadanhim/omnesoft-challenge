namespace OmnesoftChallenge.BLL.ViewModels.LoginContracts;

public record OmnesoftLoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
