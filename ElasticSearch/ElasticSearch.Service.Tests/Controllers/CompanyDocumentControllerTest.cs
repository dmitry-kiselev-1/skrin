using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using ElasticSearch.Domain;
using ElasticSearch.Domain.CompanyDocument;
using ElasticSearch.Service.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Service.Tests.Controllers
{
    [TestClass]
    public class CompanyDocumentControllerTest
    {
        [TestMethod]
        public void CompanyDocumentControllerPostTest()
        {
            int testDataCount = 100000;

            try
            {
                CompanyDocumentController productCompanyController = new CompanyDocumentController();

                var companyDocumentList = new List<CompanyDocument>();

                var date = DateTime.Now.Date;

                for (int i = 1; i <= testDataCount; i++)
                {
                    companyDocumentList.Add(
                        new CompanyDocument()
                        {
                            Id = i,
                            Company = new Customer()
                            {
                                Code = "Code " + i,
                                Indicator = "Indicator " + i,
                                Name = "Name " + i,
                                Region = "Region " + i,
                                RegistrationDate = date.AddMinutes(i),
                                Revenue = new decimal(1000000.01) + i,
                                Sector = "Sector " + i
                            },
                            ProductList = new List<Product>()
                            {
                                new Product()
                                {
                                    Name = "Product " + i + ".1",
                                    Description = "Description " + i + ".1"
                                },
                                new Product()
                                {
                                    Name = "Product " + i + ".2",
                                    Description = "Description " + i + ".2"
                                }
                            }
                        }
                        );
                }

                productCompanyController.Post(companyDocumentList.Cast<IElasticDocument>().ToList());

                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

    }
}
