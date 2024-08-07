﻿using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleAnalytics.Models.Responses;

public class PageDataResponse
{
    [Display("Matched paths")]
    public IEnumerable<string> MatchedPaths { get; set; }

    [Display("New users")]
    public int NewUsers { get; set; }

    [Display("Total users")]
    public int TotalUsers { get; set; }

    [Display("Active users")]
    public int ActiveUsers { get; set; }

    [Display("Conversion rate")]
    public double UserConversionRate { get; set; }

    [Display("Transactions")]
    public int Transactions { get; set; }

    [Display("Sessions")]
    public int Sessions { get; set; }

    [Display("Scrolled users")]
    public int ScrolledUsers { get; set; }

    [Display("Conversions")]
    public int Conversions { get; set; }

    [Display("Bounce rate")]
    public double BounceRate { get; set; }

    [Display("Summary")]
    public string Summary { get; set; }
}