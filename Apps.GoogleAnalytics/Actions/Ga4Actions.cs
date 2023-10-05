using Apps.GoogleAnalytics.Constants;
using Apps.GoogleAnalytics.Extensions;
using Apps.GoogleAnalytics.Invocables;
using Apps.GoogleAnalytics.Models.Requests;
using Apps.GoogleAnalytics.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Analytics.Data.V1Beta;

namespace Apps.GoogleAnalytics.Actions;

[ActionList]
public class Ga4Actions : Ga4Invocable
{
    public Ga4Actions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get page data", Description = "Get metrics of a specific page from the last x days")]
    public async Task<PageDataResponse> GetPageData([ActionParameter] PageReportRequest input)
    {
        var request = new RunReportRequest
        {
            DateRanges =
            {
                new DateRange
                {
                    StartDate = DateTime.Now.AddDays(-input.Days).ToString("yyyy-MM-dd"),
                    EndDate = DateTime.Now.ToString("yyyy-MM-dd")
                }
            },
            Metrics = { AnalyticsRequestData.Metrics },
            Dimensions = { AnalyticsRequestData.Dimensions },
            DimensionFilter = new()
            {
                Filter = new()
                {
                    FieldName = "pagePath",
                    StringFilter = new()
                    {
                        Value = new Uri(input.Url).AbsolutePath,
                        MatchType = input.MatchExact
                            ? Filter.Types.StringFilter.Types.MatchType.Exact
                            : Filter.Types.StringFilter.Types.MatchType.Contains
                    }
                }
            }
        };

        var report = await Client.RunReport(request);

        if (report == null) throw new Exception(ExceptionMessages.ReportNull);

        if (report.RowCount == 0) throw new Exception(ExceptionMessages.ReportEmptyRows);

        if (report.RowCount > 1) throw new Exception(ExceptionMessages.ReportMultiplePages);

        var row = report.Rows.First();

        return new()
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
            Summary = row.MetricValues.GetSummary()
        };
    }
}