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
using Newtonsoft.Json.Linq;

namespace Service.Tests.Controllers
{
    [TestClass]
    public class CompanyDocumentControllerTest
    {
        private int _testDataCount = 100000;
        private List<CompanyDocument> _companyDocumentList = new List<CompanyDocument>();

        [TestMethod]
        public void CompanyDocumentControllerPostTest()
        {
            try
            {
                CompanyDocumentController productCompanyController = new CompanyDocumentController();
                GenerateTestData();
                productCompanyController.Post(_companyDocumentList.Cast<IElasticDocument>().ToList());

                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void CompanyDocumentControllerPostDeleteTest()
        {
            try
            {
                CompanyDocumentController productCompanyController = new CompanyDocumentController();
                GenerateTestData();
                productCompanyController.Post(_companyDocumentList.Cast<IElasticDocument>().ToList(), delete: true);

                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void CompanyDocumentControllerDeleteTest()
        {
            try
            {
                CompanyDocumentController productCompanyController = new CompanyDocumentController();
                GenerateTestData();
                
                var jArray = new JArray();

                foreach (var document in _companyDocumentList)
                {
                    jArray.Add(document.Id.ToString());
                }

                productCompanyController.Delete(jArray.ToString());

                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void CompanyDocumentControllerDeleteIndexTest()
        {
            try
            {
                CompanyDocumentController productCompanyController = new CompanyDocumentController();

                productCompanyController.Delete(ids: null, indexDelete: true);

                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void GenerateTestData()
        {
            var date = DateTime.Now.Date;

            int testDataSellerCount         = _testDataCount / 3;
            int testDataBuyerCount          = _testDataCount / 3;
            int testDataSellerBuyerCount    = _testDataCount / 3 + 1;

            for (int i = 1; i <= testDataSellerBuyerCount; i++)
            {
                this._companyDocumentList.Add(
                    new CompanyDocument()
                    {
                        Id = i.ToString(),
                        Company = new Company()
                        {
                            Roles = new List<CompanyRoleEnum>() { CompanyRoleEnum.Seller, CompanyRoleEnum.Buyer },

                            Id = "Id" + i.ToString(),
                            UpdateDate = date,

                            FullName = "FullName " + i,
                            Inn = "Inn " + i,
                            Ogrn = "Ogrn " + i,
                            OgrnRegDate = date.AddMinutes(i),
                            RegionOkatoCode = "RegionOkatoCode " + i,
                            RegionOkatoCodes = new List<string>(),
                            RegionTaxId = i,
                            RegionTaxName = "RegionTaxName " + i,
                            SearchName = "SearchName " + i,
                            ShortName = "ShortName " + i,
                            SectorOkvedCode = "SectorOkvedCode " + i,
                            SectorOkvedCodes = new List<string>(),

                            SectorOkvedАdditionalCodes = new List<string>(),
                            RevenueValue = new decimal(1000000.01) + i,
                            RevenueYear = 2016
                        },
                        SellerProducts = new List<Product>()
                        {
                            new Product()
                            {
                                Name = "Product " + i + ".1",
                                Description = "Description " + i + ".1",
                                Code = "Code " + i + ".1",
                                Measure = "Measure " + i + ".1"
                            },
                            new Product()
                            {
                                Name = "Product " + i + ".2",
                                Description = "Description " + i + ".2",
                                Code = "Code " + i + ".2",
                                Measure = "Measure " + i + ".2"
                            }
                        },
                        BuyerProducts = new List<Product>()
                        {
                            new Product()
                            {
                                Name = "Product " + i + ".1",
                                Description = "Description " + i + ".1",
                                Code = "Code " + i + ".1",
                                Measure = "Measure " + i + ".1"
                            },
                            new Product()
                            {
                                Name = "Product " + i + ".2",
                                Description = "Description " + i + ".2",
                                Code = "Code " + i + ".2",
                                Measure = "Measure " + i + ".2"
                            }
                        }
                    }
                    );
            }

            for (int i = 1; i <= testDataSellerCount; i++)
            {
                this._companyDocumentList.Add(
                    new CompanyDocument()
                    {
                        Id = i.ToString(),
                        UpdateDate = date,
                        Company = new Company()
                        {
                            Roles = new List<CompanyRoleEnum>() { CompanyRoleEnum.Seller},

                            Id = "Id" + i.ToString(),
                            FullName = "FullName " + i,
                            Inn = "Inn " + i,
                            Ogrn = "Ogrn " + i,
                            OgrnRegDate = date.AddMinutes(i),
                            RegionOkatoCode = "RegionOkatoCode " + i,
                            RegionOkatoCodes = new List<string>(),
                            RegionTaxId = i,
                            RegionTaxName = "RegionTaxName " + i,
                            SearchName = "SearchName " + i,
                            ShortName = "ShortName " + i,
                            SectorOkvedCode = "SectorOkvedCode " + i,
                            SectorOkvedCodes = new List<string>(),

                            SectorOkvedАdditionalCodes = new List<string>(),
                            RevenueValue = new decimal(1000000.01) + i,
                            RevenueYear = 2016

                        },
                        SellerProducts = new List<Product>()
                        {
                            new Product()
                            {
                                Name = "Product " + i + ".1",
                                Description = "Description " + i + ".1",
                                Code = "Code " + i + ".1",
                                Measure = "Measure " + i + ".1"
                            },
                            new Product()
                            {
                                Name = "Product " + i + ".2",
                                Description = "Description " + i + ".2",
                                Code = "Code " + i + ".2",
                                Measure = "Measure " + i + ".2"
                            }
                        }
                    }
                    );
            }

            for (int i = 1; i <= testDataBuyerCount; i++)
            {
                this._companyDocumentList.Add(
                    new CompanyDocument()
                    {
                        Id = i.ToString(),
                        UpdateDate = date,
                        Company = new Company()
                        {
                            Roles = new List<CompanyRoleEnum>() {CompanyRoleEnum.Buyer},

                            Id = "Id" + i.ToString(),
                            FullName = "FullName " + i,
                            Inn = "Inn " + i,
                            Ogrn = "Ogrn " + i,
                            OgrnRegDate = date.AddMinutes(i),
                            RegionOkatoCode = "RegionOkatoCode " + i,
                            RegionOkatoCodes = new List<string>(),
                            RegionTaxId = i,
                            RegionTaxName = "RegionTaxName " + i,
                            SearchName = "SearchName " + i,
                            ShortName = "ShortName " + i,
                            SectorOkvedCode = "SectorOkvedCode " + i,
                            SectorOkvedCodes = new List<string>(),

                            SectorOkvedАdditionalCodes = new List<string>(),
                            RevenueValue = new decimal(1000000.01) + i,
                            RevenueYear = 2016

                        },
                        BuyerProducts = new List<Product>()
                        {
                            new Product()
                            {
                                Name = "Product " + i + ".1",
                                Description = "Description " + i + ".1",
                                Code = "Code " + i + ".1",
                                Measure = "Measure " + i + ".1"
                            },
                            new Product()
                            {
                                Name = "Product " + i + ".2",
                                Description = "Description " + i + ".2",
                                Code = "Code " + i + ".2",
                                Measure = "Measure " + i + ".2"
                            }
                        }
                    }
                    );
            }

        }
    }
}
