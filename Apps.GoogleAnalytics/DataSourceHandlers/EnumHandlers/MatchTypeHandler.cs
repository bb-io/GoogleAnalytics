using Blackbird.Applications.Sdk.Common.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleAnalytics.DataSourceHandlers.EnumHandlers
{
    public class MatchTypeHandler : IStaticDataSourceHandler
    {
        protected Dictionary<string, string> EnumValues => new()
        {
            {"1", "Exact"},
            {"2", "Begins with"},
            {"3", "Ends with"},
            {"4", "Contains"},
            {"5", "Full regex"},
            {"6", "Partial regex"},
        };

        public Dictionary<string, string> GetData()
        {
            return EnumValues;
        }
    }
}
