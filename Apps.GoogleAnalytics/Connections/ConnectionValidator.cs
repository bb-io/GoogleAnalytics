using Apps.GoogleAnalytics.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Google.Analytics.Data.V1Beta;
using Google.Apis.Auth.OAuth2;

namespace Apps.GoogleAnalytics.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authProviders, CancellationToken cancellationToken)
    {
        var token = authProviders.Get(CredsNames.AccessToken).Value;
        var property = authProviders.Get(CredsNames.PropertyId).Value;

        var client = await new BetaAnalyticsDataClientBuilder
        {
            GoogleCredential = GoogleCredential.FromAccessToken(token)
        }.BuildAsync(cancellationToken);

        try
        {
            await client.CheckCompatibilityAsync(new()
            {
                Property = $"properties/{property}"
            });

            return new()
            {
                IsValid = true
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }
    }
}