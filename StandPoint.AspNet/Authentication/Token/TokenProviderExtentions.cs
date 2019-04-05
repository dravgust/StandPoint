using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace StandPoint.AspNet.Authentication.Token
{
    public static class TokenProviderExtentions
    {
        public static IServiceCollection AddTokenProvider(this IServiceCollection services, IConfigurationSection options)
        {
            services.Configure<TokenIssuerOptions>(config =>
            {
                config.Audience = options[nameof(TokenIssuerOptions.Audience)] ??"CB@Audience";
                config.Issuer = options[nameof(TokenIssuerOptions.Issuer)] ??"CB@Issuer";

                var secretKey = options["SecretKey"];
                if (string.IsNullOrEmpty(secretKey))
                    throw new ArgumentException("SecretKey");

                config.SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

                long.TryParse(options[nameof(TokenIssuerOptions.Expiration)] ??"300000", out long tExpiration);
                config.ValidFor = TimeSpan.FromMilliseconds(tExpiration);
            });



            return services;
        }

        public static IApplicationBuilder UseTokenProvider<TUser>(this IApplicationBuilder builder, IServiceProvider serviceProvider) where TUser : class 
        {   
            builder.UseMiddleware<TokenProviderMiddleware<TUser>>();

            var options = serviceProvider.GetService<IOptions<TokenIssuerOptions>>();
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                //The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = options.Value.SigningKey,

                //Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = options.Value.Issuer,

                //Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = options.Value.Audience,

                //Validate the token expiry
                ValidateLifetime = true,

                //If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            builder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = "Cookie",
                CookieName = "access_token",
                TicketDataFormat = new CustomJwtDataFormat(SecurityAlgorithms.HmacSha256, tokenValidationParameters)
            });

            builder.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            return builder;
        }
    }
}
