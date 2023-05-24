using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DemoWebApp.Helpers;

namespace DemoWebApp.Handlers
{
    public class JwtBearerAuthenticationHandler : AuthenticationHandler<JwtBearerAuthenticationOptions>
    {
        public JwtBearerAuthenticationHandler(
            IOptionsMonitor<JwtBearerAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return await Task.Run(() => {
                if (Request.Query.TryGetValue("access_token", out var accessToken))
                {
                    Request.Headers.Add("Authorization", $"Bearer {accessToken}");
                }

                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (String.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.Fail("Unauthorized");
                }

                var ticket = AuthHelper.ValidateJwtToken(Options.JwtKey, Options.JwtIssuer, token);

                if (ticket == null)
                {
                    return AuthenticateResult.Fail("Unauthorized");
                }

                return AuthenticateResult.Success(ticket);
            });
        }

    }

    public class JwtBearerAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }
    }
}