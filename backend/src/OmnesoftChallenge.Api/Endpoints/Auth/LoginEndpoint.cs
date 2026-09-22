using FastEndpoints;
using OmnesoftChallenge.Api.Endpoints.Errors;
using OmnesoftChallenge.BLL.Interfaces.Security;
using OmnesoftChallenge.BLL.ViewModels.LoginContracts;

namespace OmnesoftChallenge.Api.Endpoints.Auth;

public class LoginEndpoint(IUserAuthenticationService userAuthenticationService) : Endpoint<OmnesoftLoginRequest, OmnesoftLoginResponse>
{
    private readonly IUserAuthenticationService _userAuthenticationService = userAuthenticationService;

    public override void Configure()
    {
        Post("/Auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(OmnesoftLoginRequest req, CancellationToken ct)
    {
        var result = await _userAuthenticationService.AuthenticateUserForLogin(req.UserName, req.Password);

        await result.SwitchAsync(
            response => Send.OkAsync(response, ct),
            errors => Send.ResultAsync(errors.ToHandledErrorResult()));
    }
}
