using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleAnalytics.Models.Requests;

public class PageReportRequest
{
    [Display("Last number of days")]
    public int Days { get; set; }

    [Display("URL")]
    public string Url { get; set; }

    [Display("Match exact?")]
    public bool? MatchExact { get; set; }
}