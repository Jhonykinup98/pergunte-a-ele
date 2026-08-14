using Google.Apis.Auth;

namespace PergunteAele.Api.Services;

public class GoogleAuthService
{
    private readonly IConfiguration _config;

    public GoogleAuthService(IConfiguration config)
    {
        _config = config;
    }
    public async Task<GoogleJsonWebSignature.Payload?> ValidateAsync(string idToken)
    {
        var clientId = _config["Google:ClientId"];

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { clientId }
        };

        try
        {
            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}
