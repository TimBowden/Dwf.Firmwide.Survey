using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    public class SurveyAnswer
    {
        private Guid _ID;

        public Guid ID
        {
            get { return _ID; }
            set { _ID = value;

            if (_ID == Guid.Empty)
            {
                _ID = Guid.NewGuid();
            }
            
            }
        }        

        public Guid ResponseID { get; set; }
        public QuestionType Type { get; set; }
        public string Text { get; set; }
    }
}
