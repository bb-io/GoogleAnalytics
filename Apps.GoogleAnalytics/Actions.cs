using Apps.GoogleAnalytics.Dtos;
using Apps.GoogleAnalytics.Models.Requests;
using Apps.GoogleAnalytics.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace Apps.GoogleAnalytics
{
    [ActionList]
    public class Actions
    {
        [Action("Get all pages views data", Description = "Get all pages views data")]
        public GetPageViewsDataResponse GetAllPagesViewsData(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] GetPageViewsDataRequest input)
        {
            return GetContentViewsByDimension("ga:hostname", serviceAccountConfString, authenticationCredentialsProvider.Value,
                input.StartDate, input.EndDate);
        }

        [Action("Get page views data", Description = "Get page views data")]
        public GetPageViewsDataResponse GetPageViewsData(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] GetPageViewsDataRequest input)
        {
            return GetContentViewsByDimension("ga:pagePath", serviceAccountConfString, authenticationCredentialsProvider.Value,
                input.StartDate, input.EndDate);
        }

        [Action("Get users number by country", Description = "Get users number by country")]
        public GetUsersCountryResponse GetUsersNumberByCountry(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] GetUsersCountryRequest input)
        {  
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:country" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:users", Alias = "Users number" } };

            var result = GetReports(serviceAccountConfString, authenticationCredentialsProvider.Value, input.StartDate, input.EndDate,
                dimensions, metrics);

            var response = new Dictionary<string, int>();
            foreach (var reportRow in result.Reports.First().Data.Rows)
            {
                response.Add(reportRow.Dimensions.First(), Int32.Parse(reportRow.Metrics.First().Values[0]));
            }

            return new GetUsersCountryResponse()
            {
                CountryUsers = response
            };
        }

        private GetPageViewsDataResponse GetContentViewsByDimension(string dimensionCode, string serviceAccountConfString,
            string viewId, DateTime startDate, DateTime endDate)
        {
            var dimensions = new List<Dimension> { new Dimension { Name = dimensionCode } };

            var metrics = new List<Metric> {
                new Metric { Expression = "ga:sessions", Alias = "Sessions" },
                new Metric { Expression = "ga:pageviews", Alias = "Views" },
                new Metric { Expression = "ga:avgTimeOnPage", Alias = "Average time on pages" },
                new Metric { Expression = "ga:exits", Alias = "Exits" } };

            var result = GetReports(serviceAccountConfString, viewId, startDate, endDate,
                dimensions, metrics);

            var response = new Dictionary<string, PageViewsDataDto>();
            foreach (var reportRow in result.Reports.First().Data.Rows) // iterate through the dimensions
            {
                response.Add(reportRow.Dimensions.First(), new PageViewsDataDto()
                {
                    Sessions = Int32.Parse(reportRow.Metrics.First().Values[0]),  // Metrics.First() to pick metric for the first date range (action accepts only one date range)
                    PageViews = Int32.Parse(reportRow.Metrics.First().Values[1]),
                    AverageTimeOnPage = reportRow.Metrics.First().Values[2],
                    Exits = Int32.Parse(reportRow.Metrics.First().Values[3]),
                });
            }

            return new GetPageViewsDataResponse()
            {
                PageViewsData = response
            };
        }

        private GetReportsResponse GetReports(string serviceAccountConfString, string viewId, DateTime startDate, DateTime endDate,
            List<Dimension> dimensions, List<Metric> metrics)
        {
            string[] scopes = { AnalyticsReportingService.Scope.AnalyticsReadonly }; //Read-only access to Google Analytics

            var dateRange = new DateRange
            {
                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd")
            };

            var reportRequest = new ReportRequest
            {
                DateRanges = new List<DateRange> { dateRange },
                Metrics = metrics,
                Dimensions = dimensions,
                ViewId = viewId
            };
            var getReportsRequest = new GetReportsRequest();
            getReportsRequest.ReportRequests = new List<ReportRequest> { reportRequest };

            ServiceAccountCredential? credential = GoogleCredential.FromJson(serviceAccountConfString)
                                                  .CreateScoped(scopes)
                                                  .UnderlyingCredential as ServiceAccountCredential;

            var analyticsService = new AnalyticsReportingService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Blackbird",
            });
            var result = analyticsService.Reports.BatchGet(getReportsRequest).Execute();
            return result;
        }
    }
}
