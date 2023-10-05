using Google.Analytics.Data.V1Beta;

namespace Apps.GoogleAnalytics.Constants;

public static class AnalyticsRequestData
{
    public static Metric[] Metrics => new[]
    {
        new Metric() { Name = "newUsers" },
        new() { Name = "totalUsers" },
        new() { Name = "activeUsers" },
        new() { Name = "userConversionRate" },
        new() { Name = "transactions" },
        new() { Name = "sessions" },
        new() { Name = "scrolledUsers" },
        new() { Name = "conversions" },
        new() { Name = "bounceRate" }
    };    
    
    public static Dimension[] Dimensions => new[]
    {
        new Dimension() { Name = "pagePath" }
    };
}