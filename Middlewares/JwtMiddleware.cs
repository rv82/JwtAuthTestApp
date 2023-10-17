
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace AuthTest.Middlewares;

public class JwtMiddleware : IMiddleware
{
    private readonly IConfiguration _configuration;

    public JwtMiddleware(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        return next(context);
    }
}
