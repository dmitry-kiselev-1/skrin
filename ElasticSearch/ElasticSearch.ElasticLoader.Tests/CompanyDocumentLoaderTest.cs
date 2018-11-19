using System;
using System.Configuration;
using ElasticSearch.ElasticLoader.CompanyDocument;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElasticSearch.ElasticLoader.Tests
{
    [TestClass]
    public class CompanyDocumentLoaderTest
    {
        private string _sqlConnectionString = ConfigurationManager.AppSettings["sqlConnectionString"];
        private string _elasticConnectionString = ConfigurationManager.AppSettings["elasticConnectionString"];

        [TestMethod]
        public void CompanyDocumentLoaderLoadTest()
        {
            try
            {
                var loader = new CompanyDocumentLoader(
                    sqlConnectionString: _sqlConnectionString, 
                    elasticConnectionString: _elasticConnectionString);

                loader.Load(maxIterations: 10);

                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void CompanyDocumentLoaderSetDefaultReaderSettingsTest()
        {
            try
            {
                var loader = new CompanyDocumentLoader(
                    sqlConnectionString: _sqlConnectionString, 
                    elasticConnectionString: _elasticConnectionString);

                loader.SetDefaultReaderSettings();

                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
