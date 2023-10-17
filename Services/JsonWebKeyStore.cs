using Microsoft.IdentityModel.Tokens;

namespace AuthTest.Services;

public class JsonWebKeyStore
{
    public JsonWebKeyStore()
    {
        if (JsonWebKeys is null)
        {
            JsonWebKeys = new List<JsonWebKey>();
        }
    }

    public ICollection<JsonWebKey> JsonWebKeys { get; set; }
}
