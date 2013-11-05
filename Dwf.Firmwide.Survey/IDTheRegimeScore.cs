using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwf.Firmwide.Survey
{
    public class IDTheRegimeScore
    {
        public ResultSet Main(SurveyTemplate st, SurveyResponse rs)
        {
            ResultSet rsReturn = new ResultSet(0, "");

            StringBuilder sbMessage = new StringBuilder();
            int intScore = 0;

            double dblDamage;            
            
            if (Double.TryParse(rs.GetAnswerText(st.GetQuestionFromNumber("Q1").ID), out dblDamage))
            {

                if (dblDamage <= 2000)
                {
                    sbMessage.AppendLine("Small Claims Track");  
                }
                
                             
            }

            double dblScale;

            

            if (Double.TryParse(rs.GetAnswerText(st.GetQuestionFromNumber("Q2").ID), out dblScale))
            {

                if (dblScale < 100)
                {
                    sbMessage.AppendLine("Outside the portal");    
                }
                
                
            }

            rsReturn.Message = sbMessage.ToString();
            rsReturn.Score = intScore;
            return rsReturn;

        }
    }
}
