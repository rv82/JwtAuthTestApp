using AuthTest.Models;

namespace AuthTest.Services;

public class AuthService : IAuthService
{
    private const string AuthUrl = "http://localhost:8082/realms/investmaprus/protocol/openid-connect/token";
    private const string ClientSecret = "Hkn50p9QFRyyNlsciLGTPyEMYWJ3juZX";

    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(
        IHttpClientFactory httpClientFactory
    )
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string?> GetTokenAsync(AuthModel authModel)
    {
        using var client = _httpClientFactory.CreateClient();
        var formData = new Dictionary<string, string>
        {
            { "client_id", "investmaprus" },
            { "username", authModel.Name },
            { "password", authModel.Password },
            { "grant_type", "password" },
            { "client_secret", ClientSecret }
        };

        var msg = new HttpRequestMessage
        {
            RequestUri = new Uri(AuthUrl),
            Content = new FormUrlEncodedContent(formData),
            Method = HttpMethod.Post
        };

        var response = await client.SendAsync(msg);

        return await response.Content.ReadAsStringAsync();
    }
}
