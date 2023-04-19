using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Bunnymail
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authHeader = Request.Headers.Authorization;

            if (!string.IsNullOrWhiteSpace(authHeader) 
                && authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase)
                && authHeader.Length > 6)
            {
                string credString = authHeader.Split(' ')[1];
                credString = Encoding.UTF8.GetString(Convert.FromBase64String(credString));

                string user = Environment.GetEnvironmentVariable("username") ?? string.Empty;
                string pass = Environment.GetEnvironmentVariable("password") ?? string.Empty;

                string[] creds = credString.Split(':');
                if (creds[0] == user && creds[1] == pass)
                {
                    //add identity
                    var identity = new ClaimsIdentity(new[] { new Claim("user", user) }, "Basic");
                    var ar = AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), "Basic"));
                    return Task.FromResult(ar);
                }
            }

            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", "Basic");
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }
    }
}
