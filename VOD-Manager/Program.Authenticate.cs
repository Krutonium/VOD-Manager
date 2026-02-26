using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;

namespace VOD_Manager;

internal partial class Program
{
    private static async Task<YouTubeService> Authenticate()
    {
        var configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VOD-Manager"
        );
        try
        {
            var clientSecretPath = Path.Combine(configPath, "client_secret.json");
            using var stream = new FileStream(clientSecretPath, FileMode.Open, FileAccess.Read);

            var credPath = Path.Combine(
                configPath,
                "TokenStore"
            );

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)
            );

            return new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
                HttpClientFactory = new HttpClientFactory()
            });
        }
        catch (TokenResponseException)
        {
            Console.WriteLine("Stored token invalid or revoked. Clearing and re-authenticating...");

            var credPath = Path.Combine(
                configPath,
                "YouTubeUploader.Token"
            );

            if (Directory.Exists(credPath))
                Directory.Delete(credPath, true);

            return await Authenticate();
        }
    }
}