using AuthTest.Models;

namespace AuthTest.Services;

public interface IAuthService
{
    Task<string?> GetTokenAsync(AuthModel authModel);
}
