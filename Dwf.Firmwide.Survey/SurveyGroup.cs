using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    public class SurveyGroup
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
        public string Name { get; set; }
        public string ToolTip { get; set; }
        public int Order { get; set; }
        public List<SurveyQuestion> Questions { get; set; }
    }
}
