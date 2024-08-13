using System.Globalization;
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
            NewUsers = GetSumOfColumnAsInt(report, 0),
            TotalUsers = GetSumOfColumnAsInt(report, 1),
            ActiveUsers = GetSumOfColumnAsInt(report, 2),
            UserConversionRate = GetSumOfColumnAsDouble(report, 3),
            Transactions = GetSumOfColumnAsInt(report, 4),
            Sessions = GetSumOfColumnAsInt(report, 5),
            ScrolledUsers = GetSumOfColumnAsInt(report, 6),
            Conversions = GetSumOfColumnAsInt(report, 7),
            BounceRate = GetSumOfColumnAsDouble(report, 8),
            Summary = GetSummary(report),
        };
    }

    private int GetSumOfColumnAsInt(RunReportResponse report, int column)
        => report.Rows.Sum(x => int.Parse(x.MetricValues[column].Value));

    private double GetSumOfColumnAsDouble(RunReportResponse report, int column)
        => report.Rows.Sum(x => double.Parse(x.MetricValues[column].Value, CultureInfo.InvariantCulture));

    private string GetSummary(RunReportResponse report)
        => @$"
        New users: {GetSumOfColumnAsInt(report, 0)}
        Total users: {GetSumOfColumnAsInt(report, 1)}
        Active users: {GetSumOfColumnAsInt(report, 2)}
        Conversion rate: {GetSumOfColumnAsDouble(report, 3)}
        Transactions: {GetSumOfColumnAsInt(report, 4)}
        Sessions: {GetSumOfColumnAsInt(report, 5)}
        Scrolled users: {GetSumOfColumnAsInt(report, 6)}
        Conversions: {GetSumOfColumnAsInt(report, 7)}
        Bounce rate: {GetSumOfColumnAsDouble(report, 8)}
    ";
}