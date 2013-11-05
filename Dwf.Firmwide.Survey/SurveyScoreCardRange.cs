using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    public class SurveyScoreCardRange
    {
        #region Properties

        public Guid ID { get; set; }
        public string Message { get; set; }
        public Survey.SurveyScore.RAGRating RAGRating { get; set; }

        public int? LowerBound { get; set; }
        public int? UpperBound { get; set; }
       
        #endregion
    }
}
