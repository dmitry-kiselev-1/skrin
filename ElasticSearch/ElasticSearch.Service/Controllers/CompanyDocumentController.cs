using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ElasticSearch.Domain;
using ElasticSearch.Domain.CompanyDocument;

namespace ElasticSearch.Service.Controllers
{
    public class CompanyDocumentController : ApiController
    {
        // GET: api/CompanyDocument
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CompanyDocument/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CompanyDocument
        public void Post([FromBody]List<IElasticDocument> value)
        {
            //string elasticEndPoint = ConfigurationManager.AppSettings["elasticsearcher_server"];

            Domain.CompanyDocument.CompanyDocumentWriter productCompanyWriter =
                new CompanyDocumentWriter(new Uri("http://172.16.18.206:9200"));

            productCompanyWriter.BulkCreate(value);
        }

        // PUT: api/CompanyDocument/5
        public void Put(int id, [FromBody]List<CompanyDocument> value)
        {
        }

        // DELETE: api/CompanyDocument/5
        public void Delete(int id)
        {
        }
    }
}
