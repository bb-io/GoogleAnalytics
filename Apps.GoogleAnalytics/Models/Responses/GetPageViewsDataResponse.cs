using Apps.GoogleAnalytics.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleAnalytics.Models.Responses
{
    public class GetPageViewsDataResponse
    {
        public IEnumerable<PageViewsDataDto> PageViewsData { get; set; }
    }
}
