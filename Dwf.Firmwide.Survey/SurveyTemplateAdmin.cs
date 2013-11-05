using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Web.Script.Serialization;

namespace Dwf.Firmwide.Survey
{
    public class SurveyTemplateAdmin : SurveyTemplate
    {
        #region Properties

        //The dynamic scripting
        public string ScoreFunction { get; set; }
        public string ClientFunctionStackJS { get; set; }

        #endregion


        #region Contructors

        public SurveyTemplateAdmin(SPListItem lsi)
        {
            // TODO: Complete member initialization
            JavaScriptSerializer ser = new JavaScriptSerializer(new SimpleTypeResolver());

            SurveyTemplate hld = ser.Deserialize<SurveyTemplate>(lsi["TemplateData"].ToString());

            this.Title = hld.Title;
            this.TemplateID = hld.TemplateID;
            this.Groups = hld.Groups;
            this.QuestionData = hld.QuestionData;
            this.ScoreFunction = lsi["ScoreFunction"].ToString();
            this.ClientFunctionStackJS = lsi["ClientFunctionStackJS"].ToString();

        }

        #endregion
    }
}
