
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace AuthTest.Middlewares;

public class JwtMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        //var jsonKeyString = File.ReadAllText("key.json");
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        var jsonKeyString = File.ReadAllText("key.json");
        var issuerSigningKey = new JsonWebKey(jsonKeyString);

        return next(context);
    }
}
