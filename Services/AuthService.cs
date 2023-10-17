using AuthTest.Models;
using IdentityModel.Client;

namespace AuthTest.Services;

public class AuthService : IAuthService
{
    private const string AuthUrl = "http://localhost:8082/realms/investmaprus/protocol/openid-connect/token";
    private const string ClientSecret = "Hkn50p9QFRyyNlsciLGTPyEMYWJ3juZX";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonWebKeyStore _jsonWebKeyStore;

    public AuthService(
        IHttpClientFactory httpClientFactory,
        JsonWebKeyStore jsonWebKeyStore
    )
    {
        _httpClientFactory = httpClientFactory;
        _jsonWebKeyStore = jsonWebKeyStore;
    }

    public async Task<string?> GetTokenAsync(AuthModel authModel)
    {
        var request = new PasswordTokenRequest
        {
            ClientId = "investmaprus",
            ClientSecret = ClientSecret,
            GrantType = "password",
            Method = HttpMethod.Post,
            UserName = authModel.Name,
            Password = authModel.Password,
            Scope = "roles"
        };

        using var client = _httpClientFactory.CreateClient();
        var disco = await GetDiscoveryDocumentAsync(client);
        request.RequestUri = new Uri(disco.TokenEndpoint);

        var response = await client.RequestPasswordTokenAsync(request);

        // Получение ключей
        await SaveJsonWebKeysAsync(client);

        return response.Raw;
    }

    public async Task<string?> IntrospectTokenAsync(string accessToken)
    {
        var request = new TokenIntrospectionRequest
        {
            Method = HttpMethod.Post,
            ClientId = "investmaprus",
            ClientSecret = ClientSecret,
            Token = accessToken
        };

        using var client = _httpClientFactory.CreateClient();
        var disco = await GetDiscoveryDocumentAsync(client);
        request.RequestUri = new Uri(disco.IntrospectionEndpoint);

        var response = await client.IntrospectTokenAsync(request);
        return response.Raw;
    }

    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        using var client = _httpClientFactory.CreateClient();
        var discoveryResponse = await GetDiscoveryDocumentAsync(client);

        if (discoveryResponse is null || discoveryResponse.IsError)
        {
            throw new Exception(discoveryResponse?.Error);
        }

        var options = new TokenClientOptions
        {
            ClientId = "investmaprus",
            ClientSecret = ClientSecret,
            Address = discoveryResponse.TokenEndpoint
        };

        var tokenClient = new TokenClient(client, options);
        var tokenResponse = await tokenClient.RequestRefreshTokenAsync(refreshToken);

        return tokenResponse.IsError ? tokenResponse.Error : tokenResponse.Raw;
    }

    public async Task SaveJsonWebKeysAsync(HttpClient? client)
    {
        //using var client = _httpClientFactory.CreateClient();

        var keyset =await GetJsonWebKeySetAsync(client);

        foreach (var key in keyset.Keys)
        {
            if (!_jsonWebKeyStore.JsonWebKeys.Any(x=>x.Kid == key.Kid))
            {
                _jsonWebKeyStore.JsonWebKeys.Add(key);
            }
        }
    }

    private Task<DiscoveryDocumentResponse?> GetDiscoveryDocumentAsync()
    {
        using var client = _httpClientFactory.CreateClient();
        var issuer = "http://localhost:8082/realms/investmaprus";
        return client.GetDiscoveryDocumentAsync(issuer);
    }

    private async Task<Microsoft.IdentityModel.Tokens.JsonWebKeySet> GetJsonWebKeySetAsync(HttpClient? client)
    {
        var discoveryResponse = await GetDiscoveryDocumentAsync(client);

        if (discoveryResponse is null || discoveryResponse.IsError)
        {
            throw new Exception(discoveryResponse?.Error);
        }

        var request = new JsonWebKeySetRequest
        {
            RequestUri = new Uri(discoveryResponse.JwksUri)
        };

        var response = await client.GetJsonWebKeySetAsync(request);
        var keyset = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(response.Raw);

        return keyset;
    }

    private Task<DiscoveryDocumentResponse?> GetDiscoveryDocumentAsync(HttpClient? client)
    {
        var issuer = "http://localhost:8082/realms/investmaprus";
        return client.GetDiscoveryDocumentAsync(issuer);
    }
}
