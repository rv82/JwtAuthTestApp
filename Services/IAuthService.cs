using AuthTest.Models;

namespace AuthTest.Services;

public interface IAuthService
{
    Task<string?> GetTokenAsync(AuthModel authModel);
    Task<string?> IntrospectTokenAsync(string accessToken);
    Task<string> RefreshTokenAsync(string refreshToken);
}
