using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    class QPercentage : SurveyQuestion
    {
        public double MaxPercentage { get; set; }
        public bool ShowAsSlider { get; set; }

        public override string RegEx
        {
            get
            {
                return @"^(((\d{1,3})(,\d{3})*)|(\d+))(.\d+)%?";
            }
        }
    }
}
