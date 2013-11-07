using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint;

namespace Dwf.Firmwide.Survey.UnitTests
{
    [TestClass]
    public class SurveyTest
    {
        private const string mcstrSPSite = "http://sp2010devtbo/sites/survey";

        [TestMethod]
        [TestCategory("Basic Framework")]
        [Owner("Tim Bowden")]
        [Priority(1)]
        public void CheckSiteCollectionAndWeb()
        {
            //make sure site and web exist
            using (SPSite site = new SPSite(mcstrSPSite))
            {
                Assert.IsNotNull(site);

                using (SPWeb web = site.OpenWeb())
                {
                    Assert.IsNotNull(web);
                }

            }
        }
    }
}
