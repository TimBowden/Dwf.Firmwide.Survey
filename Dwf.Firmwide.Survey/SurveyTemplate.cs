using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Web.Script.Serialization;

namespace Dwf.Firmwide.Survey
{
    public class SurveyTemplate
    {        

        #region Properties

        public string Title { get; set; }
        public Guid TemplateID { get; set; }
        public List<SurveyGroup> Groups { get; set; }
        public List<SurveyQuestion> QuestionData { get; set; }

        //The dynamic scripting
        public string ScoreFunction { get; set; }
        public string ClientFunctionStackJS { get; set; }

        #endregion

        #region Contructors

        private Microsoft.SharePoint.SPListItem lsi;

        public SurveyTemplate()
        {

        }

        public SurveyTemplate(SPListItem lsi)
        {
            // TODO: Complete member initialization
            JavaScriptSerializer ser = new JavaScriptSerializer(new SimpleTypeResolver());

            SurveyTemplate hld = ser.Deserialize<SurveyTemplate>(lsi["TemplateData"].ToString());

            hld.ScoreFunction = lsi["ScoreFunction"].ToString();

            hld.ClientFunctionStackJS = lsi["ClientFunctionStackJS"].ToString();

            this.Title = hld.Title;
            this.TemplateID = hld.TemplateID;
            this.Groups = hld.Groups;
            this.QuestionData = hld.QuestionData;
            this.ScoreFunction = hld.ScoreFunction;
            this.ClientFunctionStackJS = hld.ClientFunctionStackJS;

        }

        #endregion

        #region Public Methods

        public int? GetResponseValue(Guid pguidResponseID)
        {
            int? intReturn = null;
            bool blnFound = false;
            SurveyQuestionResponse sqrThis = new SurveyQuestionResponse();

            if (QuestionData != null)
            {

                blnFound = TryGetQuestionResponseFromGuid(QuestionData, pguidResponseID, out sqrThis);
                
            }

            if (!blnFound)
            {
                foreach (SurveyGroup grp in Groups)
                {
                    blnFound = TryGetQuestionResponseFromGuid(grp.Questions, pguidResponseID, out sqrThis);
                    if (blnFound)
	                {
                        break;
	                }
                }
            }

            if (blnFound)
            {
                intReturn = sqrThis.Value;
            }
            

            return intReturn;
        }

        public SurveyQuestion GetQuestionFromNumber(string pstrNumber)
        {
            SurveyQuestion qstReturn = null;

            foreach (SurveyQuestion qst in QuestionData)
            {
                if (qst.Number == pstrNumber)
                {
                    return qst;
                }
            }

            foreach (SurveyGroup grp in Groups)
            {
                foreach (SurveyQuestion qst in grp.Questions)
                {
                     if (qst.Number == pstrNumber)
                    {
                        return qst;
                    }
                }
            }

            return qstReturn;
        }

        public SurveyQuestion GetQuestion(Guid pguidID)
        {
            SurveyQuestion qstReturn = null;

            foreach (SurveyQuestion qst in QuestionData)
            {
                if (qst.ID == pguidID)
                {
                    return qst;
                }
            }

            foreach (SurveyGroup grp in Groups)
            {
                foreach (SurveyQuestion qst in grp.Questions)
                {
                    if (qst.ID == pguidID)
                    {
                        return qst;
                    }
                }
            }

            return qstReturn;
        }

        public SurveyQuestion GetQuestion(string pstrText)
        {
            SurveyQuestion qstReturn = null;

            foreach (SurveyQuestion qst in QuestionData)
            {
                if (qst.Text == pstrText)
                {
                    return qst;
                }
            }

            foreach (SurveyGroup grp in Groups)
            {
                foreach (SurveyQuestion qst in grp.Questions)
                {
                    if (qst.Text == pstrText)
                    {
                        return qst;
                    }
                }
            }

            return qstReturn;
        }

        #endregion

        #region Private Methods

        private bool TryGetQuestionResponseFromGuid(List<SurveyQuestion> plstThis, Guid pguidResponseID, out SurveyQuestionResponse psvrThis)
        {
            bool blnFound = false;
            psvrThis = new SurveyQuestionResponse();

            foreach (SurveyQuestion quest in plstThis)
            {
                
                if (quest.GetType() == typeof(QDropDownList))
                {
                        QDropDownList qddl = quest as QDropDownList;

                        foreach (SurveyQuestionResponse rspThis in qddl.Responses)
                        {
                            if (rspThis.ID == pguidResponseID)
                            {
                                psvrThis = rspThis;
                                blnFound = true;
                                break;
                            }
                        }
                    }
                    else if (quest.GetType() == typeof(QRadio))
                    {
                        QRadio qrad = quest as QRadio;

                        foreach (SurveyQuestionResponse rspThis in qrad.Responses)
                        {
                            if (rspThis.ID == pguidResponseID)
                            {
                                psvrThis = rspThis;
                                blnFound = true;
                                break;
                            }
                        }
                    }

            }

            return blnFound;
        }

        #endregion

    }
}
