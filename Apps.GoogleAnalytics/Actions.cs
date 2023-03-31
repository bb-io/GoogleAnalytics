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
            [ActionParameter] BaseReportRequest input)
        {
            return GetContentViewsByDimension("ga:hostname", serviceAccountConfString, authenticationCredentialsProvider.Value,
                input.StartDate, input.EndDate);
        }

        [Action("Get page views data", Description = "Get page views data")]
        public GetPageViewsDataResponse GetPageViewsData(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] BaseReportRequest input)
        {
            return GetContentViewsByDimension("ga:pagePath", serviceAccountConfString, authenticationCredentialsProvider.Value,
                input.StartDate, input.EndDate);
        }

        [Action("Get users number by country", Description = "Get users number by country")]
        public GetUsersCountryResponse GetUsersNumberByCountry(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] BaseReportRequest input)
        {  
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:country" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:users", Alias = "Users number" } };

            var result = GetReports(serviceAccountConfString, authenticationCredentialsProvider.Value, input.StartDate, input.EndDate,
                dimensions, metrics);

            var response = new List<CountryUsersDto>();
            foreach (var reportRow in result.Reports.First().Data.Rows)
            {
                response.Add(new CountryUsersDto()
                {
                    Country = reportRow.Dimensions.First(),
                    UsersNumber = Int32.Parse(reportRow.Metrics.First().Values[0])
                });
            }

            return new GetUsersCountryResponse()
            {
                CountryUsers = response
            };
        }

        [Action("Get users number by acquisition channel", Description = "Get users number by acquisition channel")]
        public GetUsersAcquisitionResponse GetUsersNumberByAcquisition(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] BaseReportRequest input)
        {
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:acquisitionTrafficChannel" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:sessions", Alias = "Sessions number" } };

            var result = GetReports(serviceAccountConfString, authenticationCredentialsProvider.Value, input.StartDate, input.EndDate,
                dimensions, metrics);

            var response = new List<AcquisitionChannelUsersDto>();
            foreach (var reportRow in result.Reports.First().Data.Rows)
            {
                response.Add(new AcquisitionChannelUsersDto()
                {
                    Channel = reportRow.Dimensions.First(),
                    UsersNumber = Int32.Parse(reportRow.Metrics.First().Values[0])
                });
            }

            return new GetUsersAcquisitionResponse()
            {
                AcquisitionUsers = response
            };
        }

        [Action("Get total sessions", Description = "Get total sessions")]
        public TotalSessionsResponse GetTotalSessions(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] BaseReportRequest input)
        {
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:hostname" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:sessions", Alias = "Sessions" } };
            var result = GetReports(serviceAccountConfString, authenticationCredentialsProvider.Value,
                input.StartDate, input.EndDate, dimensions, metrics);

            var totalSessions = result.Reports.First().Data.Rows.First().Metrics.First().Values.First();
            return new TotalSessionsResponse()
            {
                TotalSessionsNumber = Int32.Parse(totalSessions)
            };
        }

        [Action("Get number of unique users", Description = "Get number of unique users")]
        public UniqueUsersResponse GetUniqueUsers(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] BaseReportRequest input)
        {
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:hostname" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:users", Alias = "Users" } };
            var result = GetReports(serviceAccountConfString, authenticationCredentialsProvider.Value,
                input.StartDate, input.EndDate, dimensions, metrics);

            var usersNumber = result.Reports.First().Data.Rows.First().Metrics.First().Values.First();
            return new UniqueUsersResponse()
            {
                UsersNumber = Int32.Parse(usersNumber)
            };
        }

        [Action("Get bounce rate", Description = "Get bounce rate")]
        public BounceRateResponce GetBounceRate(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] BaseReportRequest input)
        {
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:hostname" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:bounceRate", Alias = "Bounce rate" } };
            var result = GetReports(serviceAccountConfString, authenticationCredentialsProvider.Value,
                input.StartDate, input.EndDate, dimensions, metrics);

            var bounceRate = result.Reports.First().Data.Rows.First().Metrics.First().Values.First();
            return new BounceRateResponce()
            {
                BounceRate = float.Parse(bounceRate)
            };
        }

        [Action("Get conversion rate", Description = "Get goal conversion rate per page")]
        public ConversionRateResponse GetConversionRate(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] BaseReportRequest input)
        {
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:goalCompletionLocation" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:goalConversionRateAll", Alias = "Conversion rate" } };
            var result = GetReports(serviceAccountConfString, authenticationCredentialsProvider.Value,
                input.StartDate, input.EndDate, dimensions, metrics);

            var response = new List<GoalConversionRateDto>();
            foreach (var reportRow in result.Reports.First().Data.Rows)
            {
                response.Add(new GoalConversionRateDto()
                {
                    Path = reportRow.Dimensions.First(),
                    Rate = float.Parse(reportRow.Metrics.First().Values[0]),
                });
            }
            return new ConversionRateResponse()
            {
                ConvertionRates = response
            };
        }

        [Action("Get users number by language", Description = "Get users number by language")]
        public GetUsersLanguageResponse GetUsersNumberByLanguage(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] BaseReportRequest input)
        {
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:language" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:users", Alias = "Users number" } };

            var result = GetReports(serviceAccountConfString, authenticationCredentialsProvider.Value, input.StartDate, input.EndDate,
                dimensions, metrics);

            var response = new List<LanguageUsersDto>();
            foreach (var reportRow in result.Reports.First().Data.Rows)
            {
                response.Add(new LanguageUsersDto()
                {
                    LanguageCode = reportRow.Dimensions.First(),
                    UsersNumber = Int32.Parse(reportRow.Metrics.First().Values[0])
                });
            }

            return new GetUsersLanguageResponse()
            {
                LanguageUsers = response
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
                ViewId = viewId,
                IncludeEmptyRows = true,
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
