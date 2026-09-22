using ErrorOr;
using OmnesoftChallenge.BLL.ViewModels.LoginContracts;

namespace OmnesoftChallenge.BLL.Interfaces.Security;

public interface IUserAuthenticationService
{
    Task<ErrorOr<OmnesoftLoginResponse>> AuthenticateUserForLogin(string userName, string password);
    Task<ErrorOr<OmnesoftLoginResponse>> RefreshToken(string refreshToken);
}
