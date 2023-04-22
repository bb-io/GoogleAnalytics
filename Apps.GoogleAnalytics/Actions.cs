using Apps.GoogleAnalytics.Dtos;
using Apps.GoogleAnalytics.Models.Requests;
using Apps.GoogleAnalytics.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Apis.AnalyticsReporting.v4.Data;

namespace Apps.GoogleAnalytics
{
    [ActionList]
    public class Actions
    {
        [Action("Get all pages views data", Description = "Get all pages views data")]
        public GetPageViewsDataResponse GetAllPagesViewsData(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] BaseReportRequest input)
        {
            var client = new AnalyticsClient(authenticationCredentialsProviders);
            return client.GetContentViewsByDimension("ga:hostname", input.StartDate, input.EndDate);
        }

        [Action("Get page views data", Description = "Get page views data")]
        public GetPageViewsDataResponse GetPageViewsData(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] BaseReportRequest input)
        {
            var client = new AnalyticsClient(authenticationCredentialsProviders);
            return client.GetContentViewsByDimension("ga:pagePath", input.StartDate, input.EndDate);
        }

        [Action("Get users number by country", Description = "Get users number by country")]
        public GetUsersCountryResponse GetUsersNumberByCountry(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] BaseReportRequest input)
        {
            var client = new AnalyticsClient(authenticationCredentialsProviders);
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:country" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:users", Alias = "Users number" } };

            var result = client.GetReports(input.StartDate, input.EndDate, dimensions, metrics);

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
        public GetUsersAcquisitionResponse GetUsersNumberByAcquisition(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] BaseReportRequest input)
        {
            var client = new AnalyticsClient(authenticationCredentialsProviders);
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:acquisitionTrafficChannel" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:sessions", Alias = "Sessions number" } };

            var result = client.GetReports(input.StartDate, input.EndDate, dimensions, metrics);

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
        public TotalSessionsResponse GetTotalSessions(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] BaseReportRequest input)
        {
            var client = new AnalyticsClient(authenticationCredentialsProviders);
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:hostname" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:sessions", Alias = "Sessions" } };
            var result = client.GetReports(input.StartDate, input.EndDate, dimensions, metrics);

            var totalSessions = result.Reports.First().Data.Rows.First().Metrics.First().Values.First();
            return new TotalSessionsResponse()
            {
                TotalSessionsNumber = Int32.Parse(totalSessions)
            };
        }

        [Action("Get number of unique users", Description = "Get number of unique users")]
        public UniqueUsersResponse GetUniqueUsers(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] BaseReportRequest input)
        {
            var client = new AnalyticsClient(authenticationCredentialsProviders);
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:hostname" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:users", Alias = "Users" } };
            var result = client.GetReports(input.StartDate, input.EndDate, dimensions, metrics);

            var usersNumber = result.Reports.First().Data.Rows.First().Metrics.First().Values.First();
            return new UniqueUsersResponse()
            {
                UsersNumber = Int32.Parse(usersNumber)
            };
        }

        [Action("Get bounce rate", Description = "Get bounce rate")]
        public BounceRateResponce GetBounceRate(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] BaseReportRequest input)
        {
            var client = new AnalyticsClient(authenticationCredentialsProviders);
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:hostname" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:bounceRate", Alias = "Bounce rate" } };
            var result = client.GetReports(input.StartDate, input.EndDate, dimensions, metrics);

            var bounceRate = result.Reports.First().Data.Rows.First().Metrics.First().Values.First();
            return new BounceRateResponce()
            {
                BounceRate = float.Parse(bounceRate)
            };
        }

        [Action("Get conversion rate", Description = "Get goal conversion rate per page")]
        public ConversionRateResponse GetConversionRate(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] BaseReportRequest input)
        {
            var client = new AnalyticsClient(authenticationCredentialsProviders);
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:goalCompletionLocation" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:goalConversionRateAll", Alias = "Conversion rate" } };
            var result = client.GetReports(input.StartDate, input.EndDate, dimensions, metrics);

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
        public GetUsersLanguageResponse GetUsersNumberByLanguage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] BaseReportRequest input)
        {
            var client = new AnalyticsClient(authenticationCredentialsProviders);
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:language" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:users", Alias = "Users number" } };

            var result = client.GetReports(input.StartDate, input.EndDate, dimensions, metrics);

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
    }
}
