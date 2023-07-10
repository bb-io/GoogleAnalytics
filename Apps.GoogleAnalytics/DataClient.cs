using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Analytics.Data.V1Beta;
using Google.Apis.Auth.OAuth2;
using System;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Apps.GoogleAnalytics
{
    public class DataClient
    {
        private string _accessToken;
        private string _propertyId;

        public DataClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            _accessToken = authenticationCredentialsProviders.First(p => p.KeyName == "Authorization").Value;
            _propertyId = authenticationCredentialsProviders.First(p => p.KeyName == "propertyId").Value;
        }

        public RunReportResponse? RunReport(RunReportRequest reportRequest)
        {
            GoogleCredential credentials = GoogleCredential.FromAccessToken(_accessToken);
            var client = new BetaAnalyticsDataClientBuilder { GoogleCredential = credentials }.Build();
            reportRequest.Property = $"properties/{_propertyId}";
            return client.RunReport(reportRequest);
        }
    }
}
