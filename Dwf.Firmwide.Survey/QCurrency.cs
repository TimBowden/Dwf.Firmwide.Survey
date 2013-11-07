using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    class QCurrency : SurveyQuestion
    {
        public double MaxAmount { get; set; }
        public double MinAmount { get; set; }
        public string CurrencySymbol { get; set; }

        public override string RegEx
        {
            get
            {
                return @"^-?(((\d{1,3})(,\d{3})*)|(\d+))(.\d+)?";
            }
        }
    }
}
