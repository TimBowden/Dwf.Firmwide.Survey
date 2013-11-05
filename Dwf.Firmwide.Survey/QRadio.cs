using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    public class QRadio: SurveyQuestion
    {
        public List<SurveyQuestionResponse> Responses { get; set; }
        public bool Inline { get; set; }
    }
}
