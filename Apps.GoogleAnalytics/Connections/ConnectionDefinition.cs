using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleAnalytics.Connections
{
    public class ConnectionDefinition : IConnectionDefinition
    {
        public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
        {
            new ConnectionPropertyGroup
            {
                Name = "OAuth",
                AuthenticationType = ConnectionAuthenticationType.OAuth2,
                ConnectionUsage = ConnectionUsage.Actions,
                ConnectionProperties = new List<ConnectionProperty>()
                {
                    new ConnectionProperty("viewId")
                }
            },
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

        public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(Dictionary<string, string> values)
        {
            var accessToken = values.First(v => v.Key == "access_token");
            yield return new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.None,
                "Authorization",
                accessToken.Value
            );
            //var serviceAccountConfString = values.First(v => v.Key == "serviceAccountConfString");
            //yield return new AuthenticationCredentialsProvider(
            //    AuthenticationCredentialsRequestLocation.None,
            //    serviceAccountConfString.Key,
            //    serviceAccountConfString.Value
            //);

            var viewId = values.First(v => v.Key == "viewId");
            yield return new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.None,
                viewId.Key,
                viewId.Value
            );
        }
    }
}
