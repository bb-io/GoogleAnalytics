using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.GoogleAnalytics.Dtos;
using Apps.GoogleAnalytics.Models.Responses;

namespace Apps.GoogleAnalytics
{
    public class AnalyticsClient
    {
        private string _accessToken;
        private string _viewId;

        public AnalyticsClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) 
        {
            _accessToken = authenticationCredentialsProviders.First(p => p.KeyName == "Authorization").Value;
            _viewId = authenticationCredentialsProviders.First(p => p.KeyName == "viewId").Value;
        }

        public GetReportsResponse GetReports(DateTime startDate, DateTime endDate,
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
                ViewId = _viewId,
                IncludeEmptyRows = true,
            };
            var getReportsRequest = new GetReportsRequest();
            getReportsRequest.ReportRequests = new List<ReportRequest> { reportRequest };

            GoogleCredential credentials = GoogleCredential.FromAccessToken(_accessToken);

            var analyticsService = new AnalyticsReportingService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "Blackbird",
            });
            var result = analyticsService.Reports.BatchGet(getReportsRequest).Execute();
            return result;
        }

        public GetPageViewsDataResponse GetContentViewsByDimension(string dimensionCode, DateTime startDate, DateTime endDate)
        {
            var dimensions = new List<Dimension> { new Dimension { Name = dimensionCode } };

            var metrics = new List<Metric> {
                new Metric { Expression = "ga:sessions", Alias = "Sessions" },
                new Metric { Expression = "ga:pageviews", Alias = "Views" },
                new Metric { Expression = "ga:avgTimeOnPage", Alias = "Average time on pages" },
                new Metric { Expression = "ga:exits", Alias = "Exits" } };

            var result = GetReports(startDate, endDate, dimensions, metrics);

            var response = new List<PageViewsDataDto>();
            foreach (var reportRow in result.Reports.First().Data.Rows) // iterate through the dimensions
            {
                response.Add(new PageViewsDataDto()
                {
                    PagePath = reportRow.Dimensions.First(),
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
    }
}
