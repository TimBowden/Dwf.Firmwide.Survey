using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{

    public enum QuestionType
    {
        Text = 0,
        Radio = 1,
        DropDown= 2,
        SectionBreak = 3,
        DateTime = 4,
        Number = 5,
        Percentage = 6,
        Currency = 7

    }

    public enum InputWidth
    {
        Mini= 0,
        Small = 1,
        Medium = 2,
        Large = 3,
        XLarge = 4,
        XXLarge = 5

    }
    
    public class SurveyQuestion
    {
        private Guid _ID;

        public Guid ID
        {
            get { return _ID; }
            set
            {
                _ID = value;

                if (_ID == Guid.Empty)
                {
                    _ID = Guid.NewGuid();
                }

            }
        }
        public string Number { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }
        public string ToolTip { get; set; }
        public int Order { get; set; }
        
        
    }
}
