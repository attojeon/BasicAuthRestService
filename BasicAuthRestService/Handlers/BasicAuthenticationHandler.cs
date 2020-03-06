using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BasicAuthRestService.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                          ILoggerFactory logger,
                                          UrlEncoder encoder,
                                          ISystemClock clock) : base(options, logger, encoder, clock)
        {

        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("AtoWorldSecretCode"))
                return Task.FromResult(AuthenticateResult.Fail("Authentication header was not found"));

            try
            {
                var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["AtoWorldSecretCode"]);
                var bytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                string[] credentials = Encoding.UTF8.GetString(bytes).Split(":");
                string val1 = credentials[0];
                string val2 = credentials[1];
                string val3 = credentials[2];

                if ( val1 == null && val2 == null)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid values"));
                }
                else
                {
                    // Claims 에 email, user_name 등의 정보를 채워서 responseMessage로 사용할 수 있음.
                    var claims = new[] { new Claim(ClaimTypes.Name, "AtoManse") };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }
            catch(Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail("Some errors."));
            }
        }
    }
}
