using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleAnalytics.Dtos
{
    public class PageViewsDataDto
    {
        public int Sessions { get; set; }

        public int PageViews { get; set; }

        public string AverageTimeOnPage { get; set; }

        public int Exits { get; set; }
    }
}
