using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Dwf.Firmwide.Survey
{
    class SurveyResponseSummary
    {
        #region Properties

        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Template { get; set; }
        public string User { get; set; }
        public Dwf.Firmwide.Survey.SurveyScore.RAGRating RAGRating { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
        public string DisplayDate { get; set; }

        #endregion

        #region Constructors

        public SurveyResponseSummary()
        {

        }

        public SurveyResponseSummary(SPListItem lsi, string pstrTemplateList) 
        {
            ID = lsi.UniqueId;
            Name = lsi.Title;
            Template = ""; // FTKHandler.GetTemplate(pstrTemplateList, new Guid(lsi["TemplateID"].ToString())).Title;
            SPFieldUserValue userValue =  new SPFieldUserValue(lsi.Web, lsi[SPBuiltInFieldId.Author].ToString());
            SPUser user = userValue.User;
            string email = user.Email;
            
            User = user.Name;
            DateCreated = (DateTime)lsi[SPBuiltInFieldId.Created];
            DateModified = (DateTime)lsi[SPBuiltInFieldId.Modified];

            DisplayDate = DateModified.ToString("dd/MM/yyyy");

            PopulateScoreDisplay(Int32.Parse(lsi["Score"].ToString()));
        }
        


        #endregion

        #region Private Methods

        public void PopulateScoreDisplay(int pintScore)
        {
            SurveyScoreCardRange rngMatch = new SurveyScoreCardRange();
            SurveyScoreCard sscThis = new SurveyScoreCard(); // FTKHandler.GetScoreCard();

            foreach (SurveyScoreCardRange rngThis in sscThis.Ranges)
            {
                //From x --> infinity
                if (pintScore >= rngThis.LowerBound && rngThis.UpperBound == null)
                {
                    rngMatch = rngThis;
                    break;
                }
                //From x --> -infinity
                else if (pintScore <= rngThis.UpperBound && rngThis.LowerBound == null)
                {
                    rngMatch = rngThis;
                    break;
                }
                else if (pintScore >= rngThis.LowerBound && pintScore <= rngThis.UpperBound)
                {
                    rngMatch = rngThis;
                    break;
                }
            }

            //ScoreDescription = rngMatch.Message;
            //ScoreColor = rngMatch.RAGRating.ToString();
            RAGRating = rngMatch.RAGRating;
        }


        #endregion 

    }
}
