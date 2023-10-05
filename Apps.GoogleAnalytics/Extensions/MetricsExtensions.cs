using Google.Analytics.Data.V1Beta;
using Google.Protobuf.Collections;

namespace Apps.GoogleAnalytics.Extensions;

public static class MetricsExtensions
{
    public static string GetSummary(this RepeatedField<MetricValue> metrics)
        => @$"
                    New users: {metrics[0].Value}
                    Total users: {metrics[1].Value}
                    Active users: {metrics[2].Value}
                    Conversion rate: {metrics[3].Value}
                    Transactions: {metrics[4].Value}
                    Sessions: {metrics[5].Value}
                    Scrolled users: {metrics[6].Value}
                    Conversions: {metrics[7].Value}
                    Bounce rate: {metrics[8].Value}
                ";
}