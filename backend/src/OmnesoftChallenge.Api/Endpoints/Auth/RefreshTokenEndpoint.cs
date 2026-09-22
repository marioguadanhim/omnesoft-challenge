using FastEndpoints;
using OmnesoftChallenge.Api.Endpoints.Errors;
using OmnesoftChallenge.BLL.Interfaces.Security;
using OmnesoftChallenge.BLL.ViewModels.LoginContracts;

namespace OmnesoftChallenge.Api.Endpoints.Auth;

public class RefreshTokenEndpoint(IUserAuthenticationService userAuthenticationService) : Endpoint<RefreshTokenRequest, OmnesoftLoginResponse>
{
    private readonly IUserAuthenticationService _userAuthenticationService = userAuthenticationService;

    public override void Configure()
    {
        Post("/Auth/refresh");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var result = await _userAuthenticationService.RefreshToken(req.RefreshToken);

        await result.SwitchAsync(
            response => Send.OkAsync(response, ct),
            errors => Send.ResultAsync(errors.ToHandledErrorResult()));
    }
}
