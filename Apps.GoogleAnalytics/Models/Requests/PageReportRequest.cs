using Apps.GoogleAnalytics.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.GoogleAnalytics.Models.Requests;

public class PageReportRequest
{
    [Display("Last number of days")]
    public int Days { get; set; }

    [Display("URL or path", Description = "Either the full URL or path part of the URL, use the 'Match type' option to specify further.")]
    public string UrlOrPath { get; set; }

    [Display("Match type", Description = "Specify how the URL should be matched with records in Google Analytics.")]
    [StaticDataSource(typeof(MatchTypeHandler))]
    public string? MatchType { get; set; }
}