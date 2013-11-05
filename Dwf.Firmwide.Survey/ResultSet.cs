using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    public class ResultSet
    {
        public int Score { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return "Your score is: " + Score.ToString() + " and the message is: " + Message;
        }

        public ResultSet(int intScore, string strMessage)
        {
            Score = intScore;
            Message = strMessage;
        }

        public ResultSet()
        {
           
        }
    }
}
