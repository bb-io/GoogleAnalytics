﻿using Apps.GoogleAnalytics.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleAnalytics.Models.Responses
{
    public class ConversionRateResponse
    {
        public IEnumerable<GoalConversionRateDto> ConvertionRates { get; set; }
    }
}