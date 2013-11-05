using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    public class SurveyQuestionResponse
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
        public string Text { get; set; }
        public int? Value { get; set; }
        //public T ResponseValue;
        public double? CurrencyValue { get; set; }
        public DateTime? DateValue { get; set; }
        public double? PercentageValue { get; set; }
        public double? NumberValue { get; set; }
    }
}
