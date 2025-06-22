using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Frontend_Dashboard.Components.Services;

public sealed class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _auth;

    public JwtAuthStateProvider(AuthService auth) => _auth = auth;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = _auth.IsAuthenticated
            ? new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _auth.UserId!.Value.ToString()),
                new Claim(ClaimTypes.Name,           _auth.Email!)
            }, "jwt")
            : new ClaimsIdentity();        // not logged-in

        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    /*  whenever AuthService logs in/out:  */
    public void NotifyUserChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}