namespace Apps.GoogleAnalytics.Constants;

public class ExceptionMessages
{
    public const string ReportNull = "Unable to fetch any data for this report request";
    public const string ReportEmptyRows = "The requested report returned no results. Wait atleast 48 hours after setting up a new property.";
    public const string ReportMultiplePages = "The path parameter provided matched multiple pages. Please specify your path to have it match a unique page.";
}