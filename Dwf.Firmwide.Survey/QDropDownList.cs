using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    class QDropDownList: SurveyQuestion
    {

        public InputWidth Width { get; set; }
        public List<SurveyQuestionResponse> Responses { get; set; }
        

    }
}
