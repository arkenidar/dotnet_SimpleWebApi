using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace SimpleWebApi
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
#pragma warning disable CS0618 // Il tipo o il membro è obsoleto
            ISystemClock clock)
#pragma warning restore CS0618 // Il tipo o il membro è obsoleto
#pragma warning disable CS0618 // Il tipo o il membro è obsoleto
            : base(options, logger, encoder, clock)
#pragma warning restore CS0618 // Il tipo o il membro è obsoleto
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if the Authorization header exists
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            try
            {
                // Parse the Authorization header
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                // Validate credentials (replace with actual validation logic)
                if (username != "testuser" || password != "testpassword")
                    return AuthenticateResult.Fail("Invalid Username or Password");

                // Create claims and identity for the authenticated user
                var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Name, username),
        };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                // Return success with the authentication ticket
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., malformed header, etc.)
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }

    }
}
