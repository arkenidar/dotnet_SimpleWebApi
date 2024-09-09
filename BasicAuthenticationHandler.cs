using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace SimpleWebApi
{
    public class BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if the Authorization header exists
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            try
            {
                // Parse the Authorization header
                var presentHeader = Request.Headers.Authorization;
                var authHeader = AuthenticationHeaderValue.Parse(presentHeader.ToString());
                if (authHeader == null || authHeader.Parameter == null) throw new Exception("Invalid Authorization Header");
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                // Validate credentials (replace with actual validation logic)
                if (username != "testuser" || password != "testpassword")
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));

                // Create claims and identity for the authenticated user
                var claims = new[] {
                                    new Claim(ClaimTypes.NameIdentifier, username),
                                    new Claim(ClaimTypes.Name, username),
                                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                // Return success with the authentication ticket
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception)
            {
                // Handle exceptions (e.g., malformed header, etc.)
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }
    }
}
