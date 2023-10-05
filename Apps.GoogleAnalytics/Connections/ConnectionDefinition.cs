using Apps.GoogleAnalytics.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.GoogleAnalytics.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
    {
        new()
        {
            Name = "OAuth",
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = new List<ConnectionProperty>()
            {
                new(CredsNames.PropertyId) { DisplayName = "Property ID" }
            }
        }
        //new ConnectionPropertyGroup
        //{
        //    Name = "Developer API token",
        //    AuthenticationType = ConnectionAuthenticationType.Undefined,
        //    ConnectionUsage = ConnectionUsage.Actions,
        //    ConnectionProperties = new List<ConnectionProperty>()
        //    {
        //        new ConnectionProperty("serviceAccountConfString"),
        //        new ConnectionProperty("viewId")
        //    }
        //}
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            CredsNames.AccessToken,
            values[CredsNames.AccessToken]
        );
        //var serviceAccountConfString = values.First(v => v.Key == "serviceAccountConfString");
        //yield return new AuthenticationCredentialsProvider(
        //    AuthenticationCredentialsRequestLocation.None,
        //    serviceAccountConfString.Key,
        //    serviceAccountConfString.Value
        //);

        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            CredsNames.PropertyId,
           values[CredsNames.PropertyId]
        );
    }
}