using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    public class SurveyScore
    {
        public enum RAGRating
        {
            Red = 0,
            Amber = 1,
            Green = 2,
            Information = 3

        }

        public string Message { get; set; }
        public string Color { get; set; }
        public RAGRating Rating { get; set; }
    }
}
