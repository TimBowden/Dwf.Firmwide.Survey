using Microsoft.SharePoint;
using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using System.Text;


namespace Dwf.Firmwide.Survey
{
    public partial class SurveyHandler : IHttpHandler
    {
        #region Constants

        public const string mcstrTemplateList = "_DWFSurveyTemplate";
        public const string mcstrScoreCardList = "_DWFSurveyScorecard";
        public const string mcstrResponseList = "_DWFSurveyResponse";

        #endregion

        #region IHttpHandler Members

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {

            string json = "";
            JavaScriptSerializer ser = new JavaScriptSerializer(new SimpleTypeResolver());

            //prevent caching
            context.Response.Clear();
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.MinValue);

            switch (context.Request["Action"])
            {
                case "SaveTemplate":
                    json = ser.Serialize(SaveTemplate(context));
                    break;

                case "GetTemplate":
                    json = ser.Serialize(GetCurrentTemplate(context));
                    break;

                case "SaveResponse":
                    json = ser.Serialize(SaveResponse(context));
                    break;

                case "GetScore":
                    json = ser.Serialize(GetScore(context));
                    break;

                case "GetPreviousResponses":
                    json = ser.Serialize(GetPreviousResponses(context));
                    break;

                case "GetResponses":
                    json = ser.Serialize(GetResponses(context));
                    break;

                case "GetResponse":
                    json = ser.Serialize(GetResponse(context));
                    break;

                case "GetScoreCard":
                    json = ser.Serialize(GetScoreCard(context));
                    break;

                case "SaveScoreCard":
                    json = ser.Serialize(SaveScoreCard(context));
                    break;

                case "GetTemplates":
                    ser = new JavaScriptSerializer();
                    json = ser.Serialize(GetTemplates(context));
                    break;

                case "RunFSharpScript":
                    ser = new JavaScriptSerializer();
                    json = RunFSharpScript(context);
                    break;

                case "GetCurrentTemplate":
                    ser = new JavaScriptSerializer();
                    json = ser.Serialize(GetCurrentTemplate(context));
                    break;

                case "CreateTestTemplate":
                    ser = new JavaScriptSerializer();
                    json = ser.Serialize(CreateTestTemplate("IDTheRegime", new Guid()));
                    break;

                default:
                    break;
            }

            context.Response.ContentType = "application/json";

            context.Response.Write(json);

            context.Response.Flush();

            context.Response.End();

        }

        #endregion

        #region Private Methods

        private string RunFSharpScript(HttpContext context)
        {

            SurveyTemplateAdmin st = GetCurrentTemplate(context);

           
            return CompileExecutable(st);
        }

        private SurveyTemplate SaveTemplate(HttpContext context)
        {

            SurveyTemplate stThis = null;
            string strList = context.Request["List"] == null ? mcstrTemplateList : context.Request["List"];

            try
            {
                if (context.Request.Form.Count > 0)
                {

                    if (true)
                    {
                        JavaScriptSerializer ser = new JavaScriptSerializer(new SimpleTypeResolver());

                        stThis = ser.Deserialize<SurveyTemplate>(context.Request.Form[0].ToString());

                        using (SPWeb web = SPContext.Current.Web)
                        {

                            SPList lst = web.Lists.TryGetList(strList);

                            if (lst != null)
                            {

                                SPListItem lsi;

                                if (stThis.TemplateID == Guid.Empty)
                                {
                                    lsi = lst.Items.Add();

                                }
                                else
                                {
                                    lsi = lst.GetItemByUniqueId(stThis.TemplateID);
                                }
                                lsi["Title"] = stThis.Title;
                                lsi["QuestionData"] = ser.Serialize(stThis);
                                web.AllowUnsafeUpdates = true;
                                lsi.Update();
                                web.AllowUnsafeUpdates = false;


                            }


                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "DWF Survey: Specified List Not Found";
                }

            }
            catch (Exception ex)
            {

                context.Response.StatusCode = 500;
                context.Response.StatusDescription = ex.Message;
            }

            return stThis;
        }

        private SurveyTemplateAdmin GetCurrentTemplate(HttpContext context)
        {
            SurveyTemplateAdmin stThis = null;
            string strList = context.Request["List"] == null ? mcstrTemplateList : context.Request["List"];



            using (SPWeb web = SPContext.Current.Web)
            {
                try
                {
                    SPList lst = web.Lists.TryGetList(strList);

                    if (lst != null)
                    {

                        SPListItem lsi = null;

                        if (context.Request["TemplateID"] == null)
                        {
                            SPQuery qry = new SPQuery();
                            qry.RowLimit = 1;
                            qry.Query = @"<OrderBy><FieldRef Name='Created' Ascending='False' /></OrderBy>";
                            SPListItemCollection lic = lst.GetItems(qry);
                            if (lic.Count == 1)
                            {
                                lsi = lic[0];
                            }
                        }
                        else
                        {
                            lsi = lst.GetItemByUniqueId(new Guid(context.Request["TemplateID"]));
                        }

                        if (lsi != null)
                        {

                            
                            stThis = new SurveyTemplateAdmin(lsi);
                        }


                    }
                    else
                    {
                        context.Response.StatusCode = 500;
                        context.Response.StatusDescription = "DWF Survey: Specified List Not Found";
                    }


                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "DWF Survey: " + ex.Message;
                }


            }

            return stThis;

        }

        private SurveyScoreCard GetScoreCard(HttpContext context)
        {
            SurveyScoreCard scdReturn = new SurveyScoreCard();

            string strList = context.Request["List"] == null ? mcstrScoreCardList : context.Request["List"];

            scdReturn.ID = Guid.NewGuid();
            scdReturn.Name = "Fraud Toolkit";

            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 0, UpperBound = 1, RAGRating = SurveyScore.RAGRating.Red, Message = "One to Settle" });
            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 2, UpperBound = 3, RAGRating = SurveyScore.RAGRating.Red, Message = "Weak Case, Likely to Settle" });
            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 4, UpperBound = 5, RAGRating = SurveyScore.RAGRating.Amber, Message = "Investigation Warranted, may be One To Settle" });
            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 6, UpperBound = 7, RAGRating = SurveyScore.RAGRating.Amber, Message = "Investigation Warranted,50:50 Prospects" });
            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 8, UpperBound = 9, RAGRating = SurveyScore.RAGRating.Amber, Message = "Investigation Warranted, Looks Like One To Fight" });
            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 10, UpperBound = 100, RAGRating = SurveyScore.RAGRating.Green, Message = "Good Case One To Fight" });


            //using (SPWeb web = SPContext.Current.Web)
            //{
            //    try
            //    {
            //        SPList lst = web.Lists.TryGetList(context.Request["List"]);

            //        if (lst != null)
            //        {

            //            SPListItem lsi = null;

            //            if (context.Request["ScoreCardID"] == null)
            //            {
            //                SPQuery qry = new SPQuery();
            //                qry.RowLimit = 1;
            //                qry.Query = @"<OrderBy><FieldRef Name='Created' Ascending='False' /></OrderBy>";
            //                SPListItemCollection lic = lst.GetItems(qry);
            //                if (lic.Count == 1)
            //                {
            //                    lsi = lic[0];
            //                }
            //            }
            //            else
            //            {
            //                lsi = lst.GetItemByUniqueId(new Guid(context.Request["TemplateID"]));
            //            }

            //            if (lsi != null)
            //            {

            //                JavaScriptSerializer ser = new JavaScriptSerializer(new SimpleTypeResolver());

            //                scdReturn = ser.Deserialize<SurveyScoreCard>(lsi["ScoreCardData"].ToString());
            //            }
            //            else
            //            {
            //                context.Response.StatusCode = 500;
            //                context.Response.StatusDescription = "DWF Survey: Scorecard Not Found";
            //            }


            //        }
            //        else
            //        {
            //            context.Response.StatusCode = 500;
            //            context.Response.StatusDescription = "DWF Survey: Specified List Not Found";
            //        }


            //    }
            //    catch (Exception ex)
            //    {
            //        context.Response.StatusCode = 500;
            //        context.Response.StatusDescription = "DWF Survey: " + ex.Message;
            //    }

            return scdReturn;



        }

        private Dictionary<string, string> GetTemplates(HttpContext context)
        {
            Dictionary<string, string> dicReturn = new Dictionary<string, string>();
            string strList = context.Request["List"] == null ? mcstrTemplateList : context.Request["List"];



            using (SPWeb web = SPContext.Current.Web)
            {
                try
                {
                    SPList lst = web.Lists.TryGetList(strList);

                    if (lst != null)
                    {


                        SPQuery qry = new SPQuery();
                        qry.Query = @"<OrderBy><FieldRef Name='Created' Ascending='False' /></OrderBy>";
                        SPListItemCollection lic = lst.GetItems(qry);

                        foreach (SPListItem lsi in lic)
                        {
                            dicReturn.Add(lsi.UniqueId.ToString(), lsi.Title);
                        }


                    }
                    else
                    {
                        context.Response.StatusCode = 500;
                        context.Response.StatusDescription = "DWF Survey: Specified List Not Found";
                    }


                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "DWF Survey: " + ex.Message;
                }


            }

            return dicReturn;
        }

        private DWFResponse SaveResponse(HttpContext context)
        {
            DWFResponse rspReturn = new DWFResponse();
            string strList = context.Request["List"] == null ? mcstrResponseList : context.Request["List"];

            rspReturn.IsError = true;

            if (context.Request["ResponseData"] != null)
            {

                JavaScriptSerializer ser = new JavaScriptSerializer(new SimpleTypeResolver());

                SurveyResponse srThis = new SurveyResponse();

                srThis.ResponseData = ser.Deserialize<List<SurveyAnswer>>(context.Request["ResponseData"]);
                srThis.ResponseName = context.Request["ResponseName"];
                srThis.TemplateID = new Guid(context.Request["TemplateID"]);


                using (SPWeb web = SPContext.Current.Web)
                {
                    try
                    {
                        SPList lst = web.Lists.TryGetList(strList);

                        if (lst != null)
                        {

                            SPListItem lsi;

                            if (context.Request["ID"] == "-1")
                            {
                                lsi = lst.Items.Add();

                            }
                            else
                            {
                                lsi = lst.GetItemByUniqueId(new Guid(context.Request["ID"]));
                            }
                            lsi["Title"] = srThis.ResponseName;
                            lsi["ResponseName"] = srThis.ResponseName;
                            int intScore = srThis.Score(SurveyHandler.GetTemplate(context.Request["TemplateList"], srThis.TemplateID), SurveyHandler.GetScoreCard());
                            lsi["Score"] = intScore;
                            lsi["TemplateID"] = srThis.TemplateID;
                            lsi["ResponseData"] = context.Request["ResponseData"];
                            web.AllowUnsafeUpdates = true;
                            SPSecurity.RunWithElevatedPrivileges(delegate()
                            {
                                lsi.Update();
                            });
                            web.AllowUnsafeUpdates = false;

                            rspReturn.IsError = false;
                            rspReturn.Message = "<h2>" + intScore + " out of 10</h2>";
                            rspReturn.Message += "<p>" + srThis.ScoreDescription + "</p>";
                        }
                    }
                    catch (Exception ex)
                    {

                        context.Response.StatusCode = 500;
                        context.Response.StatusDescription = "DWF Survey: " + ex.Message;
                    }

                }
            }
            else
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "DWF Survey: Specified Responses List Not Found";
            }


            return rspReturn;
        }

        private SurveyScore GetScore(HttpContext context)
        {

            SurveyScore scoReturn = new SurveyScore();

            try
            {
                if (context.Request["TemplateList"] != null &&
                    context.Request["ResponseData"] != null &&
                    context.Request["ScoreCardList"] != null &&
                    context.Request["TemplateID"] != null &&
                    context.Request["ScoreCardID"] != null)
                {

                    JavaScriptSerializer ser = new JavaScriptSerializer(new SimpleTypeResolver());

                    SurveyResponse srThis = new SurveyResponse();

                    if (context.Request["Suspicion"] == null)
                    {
                        srThis.AlternateScore = false;
                    }
                    else
                    {
                        srThis.AlternateScore = (context.Request["Suspicion"] == "Yes");
                    }

                    srThis.ResponseData = ser.Deserialize<List<SurveyAnswer>>(context.Request["ResponseData"]);

                    SurveyTemplate tmpThis = GetTemplate(context.Request["TemplateList"], new Guid(context.Request["TemplateID"]));
                    //SurveyScoreCard sscThis = GetSurveyScoreCard(context.Request["ScoreCardList"], new Guid(context.Request["ScoreCardID"]));
                    SurveyScoreCard sscThis = GetScoreCard(context);

                    srThis.Score(tmpThis, sscThis);
                    scoReturn.Message = srThis.ScoreDescription;
                    scoReturn.Color = srThis.ScoreColor;
                    scoReturn.Rating = srThis.RAG;



                }

                else
                {
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "DWF Survey: Incorrect score request";
                }


            }
            catch (ArgumentException argEx)
            {
                context.Response.StatusCode = 500;

                context.Response.StatusDescription = "DWF Survey: Guid does not match with a list item in the list. " + argEx.Message + ". Data:" + argEx.Data.ToString();
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "DWF Survey: " + ex.Message;
            }

            return scoReturn;
        }

        private List<SurveyResponse> GetPreviousResponses(HttpContext context)
        {

            List<SurveyResponse> lstReturn = new List<SurveyResponse>();

            if (context.Request["List"] != null)
            {

                using (SPWeb web = SPContext.Current.Web)
                {
                    try
                    {
                        SPList lst = web.Lists.TryGetList(context.Request["List"]);

                        if (lst != null)
                        {

                            SPView vwSpecified;
                            SPListItemCollection items;

                            if (context.Request["ListView"] != null)
                            {
                                vwSpecified = lst.Views[context.Request["ListView"]];

                            }
                            else
                            {
                                vwSpecified = lst.DefaultView;

                            }

                            items = lst.GetItems(vwSpecified);

                            foreach (SPListItem item in items)
                            {
                                SurveyResponse rspThis = new SurveyResponse(item);
                                lstReturn.Add(rspThis);

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                        context.Response.StatusCode = 500;
                        context.Response.StatusDescription = "DWF Survey: " + ex.Message;

                    }

                }

            }


            return lstReturn;

        }

        private SurveyResponse GetResponse(HttpContext context)
        {
            SurveyResponse rspReturn = new SurveyResponse();

            if (context.Request["List"] != null && context.Request["ResponseID"] != null)
            {

                using (SPWeb web = SPContext.Current.Web)
                {
                    try
                    {
                        SPList lst = web.Lists.TryGetList(context.Request["List"]);
                        Guid guidResponse = new Guid(context.Request["ResponseID"]);

                        if (lst != null)
                        {

                            SPListItem lsi = lst.GetItemByUniqueId(guidResponse);

                            rspReturn = new SurveyResponse(lsi);

                        }

                    }
                    catch (Exception ex)
                    {

                        context.Response.StatusCode = 500;
                        context.Response.StatusDescription = "DWF Survey: " + ex.Message;

                    }

                }

            }


            return rspReturn;

        }

        private List<SurveyResponseSummary> GetResponses(HttpContext context)
        {
            List<SurveyResponseSummary> lstReturn = new List<SurveyResponseSummary>();
            string strList = context.Request["List"] == null ? mcstrResponseList : context.Request["List"];
            string strTemplateList = context.Request["TemplateList"] == null ? mcstrTemplateList : context.Request["TemplateList"];

            using (SPWeb web = SPContext.Current.Web)
            {
                try
                {
                    SPList lst = web.Lists.TryGetList(strList);

                    if (lst != null)
                    {

                        SPView vwSpecified;
                        SPListItemCollection items;

                        if (context.Request.QueryString["ListView"] != null)
                        {
                            vwSpecified = lst.Views[context.Request.QueryString["ListView"]];

                        }
                        else
                        {
                            vwSpecified = lst.DefaultView;

                        }

                        items = lst.GetItems(vwSpecified);

                        foreach (SPListItem item in items)
                        {
                            SurveyResponseSummary rspThis = new SurveyResponseSummary(item, strTemplateList);
                            lstReturn.Add(rspThis);

                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 500;
                        context.Response.StatusDescription = "DWF Survey: Specified Responses List Not Found";
                    }

                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "DWF Survey: " + ex.Message;
                }

            }


            return lstReturn;

        }

        private SurveyTemplate CreateTestTemplate(string strTitle, Guid guidID)
        {
            //JavaScriptSerializer ser = new JavaScriptSerializer();
            SurveyTemplate stThis = new SurveyTemplate();
            stThis.Title = strTitle;
            stThis.TemplateID = guidID;



            List<SurveyQuestionResponse> resp = new List<SurveyQuestionResponse>();

            SurveyQuestionResponse rs = new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 10 };

            resp.Add(rs);

            rs = new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 };

            resp.Add(rs);

            rs = new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "N/A" };

            resp.Add(rs);

            List<SurveyQuestion> lstQ = new List<SurveyQuestion>();

            QText qt = new QText { ID = Guid.NewGuid(), Lines = 1, Width = InputWidth.Small, Type = QuestionType.Text, Number = "Q1", Order = 0, Text = "Value of Damages", ToolTip = "This is interlinked with the sliding scale" };

            lstQ.Add(qt);

            qt = new QText { ID = Guid.NewGuid(), Lines = 1, Width = InputWidth.Small, Type = QuestionType.Text, Number = "Q2", Order = 0, Text = "Sliding Scale", ToolTip = "Indicator of how much responsibility to admit" };

            lstQ.Add(qt);

            List<SurveyQuestionResponse> respToC = new List<SurveyQuestionResponse>();

            rs = new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Motor", Value = 1 };

            respToC.Add(rs);

            rs = new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Employer's Liability", Value = 2 };

            respToC.Add(rs);

            rs = new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Public Liability", Value = 3 };

            respToC.Add(rs);

            QRadio qr = new QRadio { ID = Guid.NewGuid(), Inline = true, Number = "Q3", Order = 1, Text = "Type of Claim", Type = QuestionType.Radio, ToolTip = "Type of Claim", Responses = respToC };

            lstQ.Add(qr);

            qt = new QText { ID = Guid.NewGuid(), Lines = 1, Width = InputWidth.Small, Type = QuestionType.Text, Number = "Q4", Order = 0, Text = "Date of Event", ToolTip = "Date of Event" };

            lstQ.Add(qt);

            qt = new QText { ID = Guid.NewGuid(), Lines = 1, Width = InputWidth.Small, Type = QuestionType.Text, Number = "Q5", Order = 0, Text = "Date of Claim", ToolTip = "Date of Claim" };

            lstQ.Add(qt);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 });

            QDropDownList qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q6", Order = 2, Text = "Is there a London uplift?", Type = QuestionType.DropDown, ToolTip = "Is there a London uplift?", Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "CFA", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Non-CFA", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q7", Order = 2, Text = "Type of Funding", Type = QuestionType.DropDown, ToolTip = "Type of Funding", Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q8", Order = 2, Text = "Is there an ATE premium", Type = QuestionType.DropDown, ToolTip = "Is there an ATE premium", Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Case Litigated", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Pre-Litigated", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q9", Order = 2, Text = "Is the Case Litigated or Pre-Litigated", Type = QuestionType.DropDown, ToolTip = "Is the Case Litigated or Pre-Litigated", Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q10", Order = 2, Text = "Did the claim start in the portal?", Type = QuestionType.DropDown, ToolTip = "Did the claim start in the portal?", Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Pre-allocation", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Post-allocation/Pre-listing", Value = 2 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Post-listing/Pre-trial", Value = 3 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q11", Order = 2, Text = "State of Litigation?", Type = QuestionType.DropDown, ToolTip = "State of Litigation?" , Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q12", Order = 2, Text = "Is the case going through to trial?", Type = QuestionType.DropDown, ToolTip = "Is the case going through to trial?", Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q13", Order = 2, Text = "Admitted failure to wear a seatbelt?", Type = QuestionType.DropDown, ToolTip = "Admitted failure to wear a seatbelt?", Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q14", Order = 2, Text = "Date of settlement > 01/04/2013?", Type = QuestionType.DropDown, ToolTip = "Date of settlement > 01/04/2013?", Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q15", Order = 2, Text = "Was the ATE purchased < 01/04/2013?", Type = QuestionType.DropDown, ToolTip = "Was the ATE purchased < 01/04/2013?", Responses = resp };

            lstQ.Add(qd);


            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q16", Order = 2, Text = "Date of CFA < 01/04/2013?", Type = QuestionType.DropDown, ToolTip = "Date of CFA < 01/04/2013?", Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q17", Order = 2, Text = "Has the case settled within stages 1&2?", Type = QuestionType.DropDown, ToolTip = "Has the case settled within stages 1&2?", Responses = resp };

            lstQ.Add(qd);

            resp = new List<SurveyQuestionResponse>();

            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "Yes", Value = 1 });
            resp.Add(new SurveyQuestionResponse { ID = Guid.NewGuid(), Text = "No", Value = 0 });

            qd = new QDropDownList { ID = Guid.NewGuid(), Width = InputWidth.Small, Number = "Q18", Order = 2, Text = "Does the user want to stay within the portal?", Type = QuestionType.DropDown, ToolTip = "Does the user want to stay within the portal?", Responses = resp };

            lstQ.Add(qd);

            qt = new QText { ID = Guid.NewGuid(), Lines = 1, Width = InputWidth.Small, Type = QuestionType.Text, Number = "Q19", Order = 0, Text = "Disbursements including VAT", ToolTip = "Disbursements including VAT" };

            lstQ.Add(qt);
          
            List<SurveyGroup> grps = new List<SurveyGroup>();

            grps.Add(new SurveyGroup { ID = Guid.NewGuid(), Name = "IDTheRegime", Order = 0, Questions = lstQ, ToolTip = "ID The Regime"});
            
            stThis.Groups = grps;

            return stThis;

        }

        private SurveyScoreCard SaveScoreCard(HttpContext context)
        {
            string strList = context.Request["List"] == null ? mcstrScoreCardList : context.Request["List"];

            SurveyScoreCard sscReturn = new SurveyScoreCard();

            try
            {
                if (context.Request["ScoreCardData"] != null)
                {

                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    sscReturn = ser.Deserialize<SurveyScoreCard>(context.Request["ScoreCardData"]);

                    using (SPWeb web = SPContext.Current.Web)
                    {

                        SPList lst = web.Lists.TryGetList(strList);

                        if (lst != null)
                        {

                            SPListItem lsi;

                            if (sscReturn.ID == Guid.Empty)
                            {
                                lsi = lst.Items.Add();

                            }
                            else
                            {
                                lsi = lst.GetItemByUniqueId(sscReturn.ID);
                            }
                            lsi["Title"] = sscReturn.Name;
                            lsi["ScoreCardData"] = ser.Serialize(sscReturn);
                            web.AllowUnsafeUpdates = true;
                            lsi.Update();
                            web.AllowUnsafeUpdates = false;


                        }
                        else
                        {
                            context.Response.StatusCode = 500;
                            context.Response.StatusDescription = "DWF Survey: Scorecard List not found";
                        }


                    }



                }
                else
                {
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "DWF Survey: Scorecard data not found";
                }
            }
            catch (Exception ex)
            {

                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "DWF Survey: " + ex.Message;
            }


            return sscReturn;
        }

        public string CompileExecutable(SurveyTemplateAdmin st)
        {
            CodeDomProvider provider = null;            
            
            provider = CodeDomProvider.CreateProvider("CSharp");
           
            if (provider != null)
            {

                // Format the executable file name. 
                // Build the output assembly path using the current directory 
                // and <source>_cs.exe or <source>_vb.exe.               
                
               
                CompilerParameters cp = new CompilerParameters();

                //string strReference = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), references[i]);

                cp.ReferencedAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly().Location);

                // Generate an executable instead of  
                // a class library.
                cp.GenerateExecutable = false;

                // Specify the assembly file name to generate.
                //cp.OutputAssembly = exeName;

                // Save the assembly as a physical file.
                cp.GenerateInMemory = true;

                // Set whether to treat all warnings as errors.
                cp.TreatWarningsAsErrors = false;

                // Invoke compilation of the source file.
                CompilerResults cr = provider.CompileAssemblyFromSource(cp, 
                    st.ScoreFunction);

                if(cr.Errors.Count > 0)
                {

                    StringBuilder sb = new StringBuilder();
                    
                    // Display compilation errors.
                    foreach(CompilerError ce in cr.Errors)
                    {
                        sb.AppendLine(ce.ErrorText);
                    }

                    return sb.ToString();
                }
                else
                {
                    Assembly a = cr.CompiledAssembly;
                    var o = a.CreateInstance("SurveyFunction");
                    Type t = o.GetType();
                    MethodInfo mi = t.GetMethod("Main");


                    object[] oParams = new object[2];
                    oParams[0] = st as SurveyTemplate;
                    oParams[1] = new SurveyResponse();
                    object s = mi.Invoke(o, oParams);
                    return s.ToString();



                }
                
            }

            return "Code has not run";

        }
	
        

        #endregion

        #region Static Methods

        public static SurveyScoreCard GetScoreCard()
        {
            SurveyScoreCard scdReturn = new SurveyScoreCard();

            scdReturn.ID = Guid.NewGuid();
            scdReturn.Name = "Fraud Toolkit";

            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 0, UpperBound = 1, RAGRating = SurveyScore.RAGRating.Red, Message = "One to Settle" });
            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 2, UpperBound = 3, RAGRating = SurveyScore.RAGRating.Red, Message = "Weak Case, Likely to Settle" });
            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 4, UpperBound = 5, RAGRating = SurveyScore.RAGRating.Amber, Message = "Investigation Warranted, may be One To Settle" });
            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 6, UpperBound = 7, RAGRating = SurveyScore.RAGRating.Amber, Message = "Investigation Warranted,50:50 Prospects" });
            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 8, UpperBound = 9, RAGRating = SurveyScore.RAGRating.Amber, Message = "Investigation Warranted, Looks Like One To Fight" });
            scdReturn.Ranges.Add(new SurveyScoreCardRange { ID = Guid.NewGuid(), LowerBound = 10, UpperBound = 100, RAGRating = SurveyScore.RAGRating.Green, Message = "Good Case One To Fight" });

            return scdReturn;

        }

        public static SurveyTemplate GetTemplate(string pstrList)
        {

            SurveyTemplate stThis = null;


            //using (SPWeb web = SPContext.Current.Web)
            SPWeb web = SPContext.Current.Web;
            {
                try
                {
                    SPList lst = web.Lists.TryGetList(pstrList);

                    if (lst != null)
                    {

                        SPListItem lsi = null;

                        SPQuery qry = new SPQuery();
                        qry.RowLimit = 1;
                        qry.Query = @"<OrderBy><FieldRef Name='Created' Ascending='False' /></OrderBy>";
                        SPListItemCollection lic = lst.GetItems(qry);
                        if (lic.Count == 1)
                        {
                            lsi = lic[0];
                        }

                        JavaScriptSerializer ser = new JavaScriptSerializer(new SimpleTypeResolver());
                        stThis = ser.Deserialize<SurveyTemplate>(lsi["QuestionData"].ToString());

                    }

                }
                catch (Exception ex)
                {
                    stThis.Title = ex.Message;
                }

            }

            return stThis;
        }

        public static SurveyTemplate GetTemplate(string pstrList, Guid pguidTemplate)
        {

            SurveyTemplate stThis = null;

            SPWeb web = SPContext.Current.Web;
            {
                try
                {
                    SPList lst = web.Lists.TryGetList(pstrList);

                    if (lst != null)
                    {

                        SPListItem lsi = lst.GetItemByUniqueId(pguidTemplate);



                        JavaScriptSerializer ser = new JavaScriptSerializer(new SimpleTypeResolver());
                        stThis = new SurveyTemplate();
                        stThis.Title = lsi.Title;
                        stThis.TemplateID = lsi.UniqueId;
                        stThis = ser.Deserialize<SurveyTemplate>(lsi["QuestionData"].ToString());

                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return stThis;
        }

        public static SurveyScoreCard GetSurveyScoreCard(string pstrList, Guid pguidScoreCard)
        {

            SurveyScoreCard sscThis = null;

            SPWeb web = SPContext.Current.Web;
            {
                try
                {
                    SPList lst = web.Lists.TryGetList(pstrList);

                    if (lst != null)
                    {

                        SPListItem lsi = null;

                        lsi = lst.GetItemByUniqueId(pguidScoreCard);

                        JavaScriptSerializer ser = new JavaScriptSerializer();
                        sscThis = ser.Deserialize<SurveyScoreCard>(lsi["ScoreCardData"].ToString());



                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return sscThis;
        }

        #endregion
    }
}
