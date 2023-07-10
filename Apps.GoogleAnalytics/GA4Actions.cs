using Apps.GoogleAnalytics.Models.Requests;
using Apps.GoogleAnalytics.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Analytics.Data.V1Beta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleAnalytics
{
    [ActionList]
    public class GA4Actions
    {
        [Action("Get page data", Description = "Get metrics of a specific page from the last x days")]
        public PageDataResponse GetPageData(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] PageReportRequest input)
        {
            var client = new DataClient(authenticationCredentialsProviders);

            var matchType = input.MatchExact ? Filter.Types.StringFilter.Types.MatchType.Exact : Filter.Types.StringFilter.Types.MatchType.Contains;
            var request = new RunReportRequest
            {
                DateRanges = { 
                    new DateRange
                    {
                        StartDate = DateTime.Now.AddDays(-input.Days).ToString("yyyy-MM-dd"),
                        EndDate = DateTime.Now.ToString("yyyy-MM-dd")
                    } 
                },
                Metrics = {
                    new Metric { Name = "newUsers" },
                    new Metric { Name = "totalUsers"},
                    new Metric { Name = "activeUsers"},
                    new Metric { Name = "userConversionRate"},
                    new Metric { Name = "transactions"},
                    new Metric { Name = "sessions"},
                    new Metric { Name = "scrolledUsers"},
                    new Metric { Name = "conversions"},
                    new Metric { Name = "bounceRate"}
                },
                Dimensions = { 
                    new Dimension { Name = "pagePath" } 
                },
                DimensionFilter = new FilterExpression
                {
                    Filter = new Filter
                    {
                        FieldName = "pagePath",
                        StringFilter = new Filter.Types.StringFilter{ MatchType = matchType, Value = input.Path}
                    }
                }
            };
            var report = client.RunReport(request);
            if (report == null)
                throw new Exception("Unable to fetch any data for this report request");

            if (report.RowCount == 0)
                throw new Exception("The requested report returned no results. Wait atleast 48 hours after setting up a new property.");

            if (report.RowCount > 1)
                throw new Exception("The path parameter provided matched multiple pages. Please specify your path to have it match a unique page.");

            var row = report.Rows.First();
            return new PageDataResponse
            {
                NewUsers = int.Parse(row.MetricValues[0].Value),
                TotalUsers = int.Parse(row.MetricValues[1].Value),
                ActiveUsers = int.Parse(row.MetricValues[2].Value),
                UserConversionRate = double.Parse(row.MetricValues[3].Value),
                Transactions = int.Parse(row.MetricValues[4].Value),
                Sessions = int.Parse(row.MetricValues[5].Value),
                ScrolledUsers = int.Parse(row.MetricValues[6].Value),
                Conversions = int.Parse(row.MetricValues[7].Value),
                BounceRate = double.Parse(row.MetricValues[8].Value),
                Summary = @$"
                    New users: {row.MetricValues[0].Value}
                    Total users: {row.MetricValues[1].Value}
                    Active users: {row.MetricValues[2].Value}
                    Conversion rate: {row.MetricValues[3].Value}
                    Transactions: {row.MetricValues[4].Value}
                    Sessions: {row.MetricValues[5].Value}
                    Scrolled users: {row.MetricValues[6].Value}
                    Conversions: {row.MetricValues[7].Value}
                    Bounce rate: {row.MetricValues[8].Value}
                "
            };
        }
    }
}
