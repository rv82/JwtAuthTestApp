using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AuthTest.Services;

namespace AuthTest.Extensions.JwtBearer;

public static class JwtBearerExtensions
{
    public static IServiceCollection AddJwtBearerAuth(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton(new JsonWebKeyStore());
        services
            .AddAuthentication(option =>
            {
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = issuerSigningKey,
                    LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
                    {
                        if (expires.HasValue)
                        {
                            var time = expires.Value.Kind == DateTimeKind.Utc ? expires.Value.ToLocalTime() : expires.Value;
                            bool result = time > DateTime.Now;
                            return result;
                        }
                        return false;
                    },

                    IssuerSigningKeyResolver = GetIssuerSigningKeyResolver(services),

                    ValidIssuer = configuration["Jwt:Authority"],
                    ValidAudience = configuration["Jwt:Audience"]
                };
            });

        return services;
    }

    private static IEnumerable<SecurityKey> IssuerSigningKeyResolver(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters)
    {
        return Array.Empty<SecurityKey>();
    }

    private static IssuerSigningKeyResolver GetIssuerSigningKeyResolver(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var jsonWebKeyStore = serviceProvider.GetService<JsonWebKeyStore>();

        return (token, securityToken, kid, validationParameters) =>
            jsonWebKeyStore.JsonWebKeys;
    }
}
