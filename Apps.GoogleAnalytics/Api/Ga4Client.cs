using Apps.GoogleAnalytics.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Google.Analytics.Data.V1Beta;
using Google.Apis.Auth.OAuth2;

namespace Apps.GoogleAnalytics.Api;

public class Ga4Client
{
    private readonly string _propertyId;
    private readonly BetaAnalyticsDataClient _client;

    public Ga4Client(AuthenticationCredentialsProvider[] creds)
    {
        _propertyId = creds.Get(CredsNames.PropertyId).Value;

        var accessToken = creds.Get(CredsNames.AccessToken).Value;

        _client = new BetaAnalyticsDataClientBuilder
        {
            GoogleCredential = GoogleCredential.FromAccessToken(accessToken)
        }.Build();
    }

    public Task<RunReportResponse> RunReport(RunReportRequest reportRequest)
    {
        reportRequest.Property = $"properties/{_propertyId}";
        return _client.RunReportAsync(reportRequest);
    }
}