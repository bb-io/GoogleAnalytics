using Apps.GoogleAnalytics.Constants;
using Apps.GoogleAnalytics.Extensions;
using Apps.GoogleAnalytics.Invocables;
using Apps.GoogleAnalytics.Models.Requests;
using Apps.GoogleAnalytics.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Analytics.Data.V1Beta;
using Google.Protobuf.Collections;

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
        var matchTypeInt = input.MatchType == null ? 0 : int.Parse(input.MatchType);
        var path = input.UrlOrPath;

        try
        {
            path = new Uri(input.UrlOrPath).AbsolutePath;
        }
        catch { }

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
                        Value = path,
                        MatchType = (Filter.Types.StringFilter.Types.MatchType) matchTypeInt,
                    }
                }
            }
        };

        var report = await Client.RunReport(request);

        if (report == null) throw new Exception(ExceptionMessages.ReportNull);

        //if (report.RowCount > 1) throw new Exception(ExceptionMessages.ReportMultiplePages);

        if (report.Rows.Count == 0)
        {
            var metrics = new RepeatedField<MetricValue>();
            Enumerable.Range(0, 9).ToList().ForEach(i => metrics.Add(new MetricValue { Value = "0" }));
            return new()
            {
                MatchedPaths = new List<string>() { },
                Summary = metrics.GetSummary()
            };
        }

        return new()
        {
            MatchedPaths = report.Rows.Select(x => x.DimensionValues[0].Value),
            NewUsers = GetSumOfColumn(report, 0),
            TotalUsers = GetSumOfColumn(report, 1),
            ActiveUsers = GetSumOfColumn(report, 2),
            UserConversionRate = GetSumOfColumn(report, 3),
            Transactions = GetSumOfColumn(report, 4),
            Sessions = GetSumOfColumn(report, 5),
            ScrolledUsers = GetSumOfColumn(report, 6),
            Conversions = GetSumOfColumn(report, 7),
            BounceRate = GetSumOfColumn(report, 8),
            Summary = GetSummary(report),
        };
    }

    private int GetSumOfColumn(RunReportResponse report, int column) => report.Rows.Sum(x => int.Parse(x.MetricValues[column].Value));

    private string GetSummary(RunReportResponse report)
        => @$"
            New users: {GetSumOfColumn(report, 0)}
            Total users: {GetSumOfColumn(report, 1)}
            Active users: {GetSumOfColumn(report, 2)}
            Conversion rate: {GetSumOfColumn(report, 3)}
            Transactions: {GetSumOfColumn(report, 4)}
            Sessions: {GetSumOfColumn(report, 5)}
            Scrolled users: {GetSumOfColumn(report, 6)}
            Conversions: {GetSumOfColumn(report, 7)}
            Bounce rate: {GetSumOfColumn(report, 8)}
        ";
}