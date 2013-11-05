using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    public class SurveyScoreCard
    {
        #region Properties

        public Guid ID { get; set; }
        public string Name { get; set; }

        public List<SurveyScoreCardRange> Ranges { get; set; }

        #endregion

        #region Constructor

        public SurveyScoreCard()
        {
            Ranges = new List<SurveyScoreCardRange>();
        }

        #endregion
    }
}
