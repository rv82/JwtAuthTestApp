using AuthTest.Models;
using AuthTest.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using IdentityModel;

namespace AuthTest.Controllers;

public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(
        ILogger<AuthController> logger,
        IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("auth")]
    public async Task<string?> AuthAsync([FromBody] AuthModel authModel)
    {
        var response = await _authService.GetTokenAsync(authModel);
        var jObject = JObject.Parse(response);
        var token = (string?)jObject["access_token"];

        return string.IsNullOrEmpty(token) ? "error" : token;
    }

    [HttpPost("introspect")]
    public Task<string?> IntrospectTokenAsync([FromBody] IntrospectionRequestModel model)
    {
        return _authService.IntrospectTokenAsync(model.Token);
    }

    [HttpPost("refresh")]
    public Task<string> RefreshTokenAsync([FromBody] RefreshTokenRequestModel model)
    {
        return _authService.RefreshTokenAsync(model.RefreshToken);
    }

    /* [HttpGet("jwks")]
    public Task<string> GetJsonWebKitAsync()
    {
        return _authService.GetJsonWebKeysAsync();
    } */
}
