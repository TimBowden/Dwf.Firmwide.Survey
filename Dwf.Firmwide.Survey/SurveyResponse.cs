using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Web.Script.Serialization;

namespace Dwf.Firmwide.Survey
{
    public class SurveyResponse
    {

        #region Properties


        public Guid ID { get; private set; }

        public string ResponseName { get; set; }
        public Guid TemplateID { get; set; }
        public List<SurveyAnswer> ResponseData { get; set; }
        public string ScoreDescription { get; set; }
        public string ScoreColor { get; set; }

        public SurveyScoreCard ScoreCard { get; set; }

        public SurveyScore.RAGRating RAG { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public bool AlternateScore { get; set; }


        #endregion

        #region Public Methods

        public int Score(string pstrList){

            return 0; // Score(FTKHandler.GetTemplate(pstrList));

        }

        public int Score(string pstrList, Guid pguidTemplateID)
        {
            return 0;// Score(FTKHandler.GetTemplate(pstrList, pguidTemplateID));
        }

        public int Score(SurveyTemplate pstTemplate)
        {

            return 0;

        }

        public int Score(SurveyTemplate pstTemplate, SurveyScoreCard sscScoreCard)
        {
            int intAnswerCount = 0;
            int intAnswerTotal = 0;
            int intReturnScore = 0;

            TemplateID = pstTemplate.TemplateID;
            ScoreCard = sscScoreCard;

            foreach (SurveyAnswer ansThis in ResponseData)
            {
                if (ansThis.Type == QuestionType.Radio || ansThis.Type == QuestionType.DropDown)
                {
                    int? intValue = pstTemplate.GetResponseValue(ansThis.ResponseID);

                    if (intValue != null)
                    {
                        intAnswerCount++;
                        Guid[] strReversibleAnswers = { };                        

                        if (AlternateScore && strReversibleAnswers.Contains(ansThis.ID))
                        {
                            intAnswerTotal += 10 - intValue.Value;
                        }
                        else
                        {
                            intAnswerTotal += intValue.Value;
                        }

                    }

                }
            }

            if (intAnswerCount != 0)
            {
                intReturnScore = intAnswerTotal / intAnswerCount;
            }
            else
            {
                intReturnScore = 0;
            }



            PopulateScoreDisplay(intReturnScore, sscScoreCard);

            return intReturnScore;

        }

        public void PopulateScoreDisplay(int pintScore, SurveyScoreCard sscThis)
        {

            SurveyScoreCardRange rngMatch = new SurveyScoreCardRange();

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

            ScoreDescription = rngMatch.Message;
            ScoreColor = rngMatch.RAGRating.ToString();
            RAG = rngMatch.RAGRating;
        }

        public string GetAnswerText(Guid pguidID)
        {

            string strReturn = String.Empty;            
            
            foreach (SurveyAnswer ans in ResponseData)
            {
                if (ans.ID == pguidID)
                {
                    return ans.Text;
                }
            }

            return strReturn;
        }


        #endregion

        #region Constructors

        public SurveyResponse()
        {
            ID = Guid.Empty;
        }

        public SurveyResponse(SPListItem lsi)
        {
            ID = lsi.UniqueId;
            ResponseName = lsi["ResponseName"].ToString();
            TemplateID = new Guid(lsi["TemplateID"].ToString());
            if (lsi["ResponseData"] != null)
            {
                JavaScriptSerializer ser = new JavaScriptSerializer(new SimpleTypeResolver());
                ResponseData = ser.Deserialize<List<SurveyAnswer>>(lsi["ResponseData"].ToString());
            }
            DateCreated = (DateTime)lsi[SPBuiltInFieldId.Created];
            DateModified = (DateTime)lsi[SPBuiltInFieldId.Modified];
            ScoreCard = new SurveyScoreCard(); // FTKHandler.GetScoreCard();
            PopulateScoreDisplay(Int32.Parse(lsi["Score"].ToString()), ScoreCard);
        }

        #endregion

        #region Private Methods



        #endregion
    }
}
