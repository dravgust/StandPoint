using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace StandPoint.AspNet.Authentication.Token
{
    public class TokenProviderMiddleware<TUser> where TUser: class 
    {
        private readonly RequestDelegate _next;
        private readonly TokenIssuerOptions _options;

        public TokenProviderMiddleware(RequestDelegate next, IOptions<TokenIssuerOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context, UserManager<TUser> userManager)
        {
            //If the request path doesn't match, skip
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                await _next.Invoke(context);
            }
            //Request must be POST with Content-Type: application/x-www-form-urlencoded
            else if (!context.Request.Method.Equals("POST") || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Bad request");
            }
            else
            {
                try
                {
                    var identity = await GetIdentity(context, userManager);
                    await GenerateToken(context, identity);
                }
                catch (ArgumentException e)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid username or password.");
                }
            }         
        }

        private async Task<ClaimsIdentity> GetIdentity(HttpContext context, UserManager<TUser> userManager)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException("Invalid username or password.");

            var user = await userManager.FindByEmailAsync(username);
            if (user == null)
                throw new ArgumentException("Invalid username or password.");

             if (!await userManager.CheckPasswordAsync(user, password))
                throw new ArgumentException("Invalid username or password.");

             return new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] { });
        }

        private async Task GenerateToken(HttpContext context, IIdentity identity)
        {
            ThrowIfInvalidOptions(_options);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, identity.Name),
                new Claim(JwtRegisteredClaimNames.Jti, await _options.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, _options.IssuedAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            //Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: _options.NotBefore.DateTime,
                expires: _options.Expiration.DateTime,
                signingCredentials: _options.SigningCredentials);

            var handler = new JwtSecurityTokenHandler();
            var encodedJwt = handler.WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                token_type = "Bearer",
                expires_in = (int)_options.ValidFor.TotalSeconds
            };

            //Seralize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response,
                new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private static void ThrowIfInvalidOptions(TokenIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(options.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(options.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(options.JtiGenerator));
            }
        }
    }
}
