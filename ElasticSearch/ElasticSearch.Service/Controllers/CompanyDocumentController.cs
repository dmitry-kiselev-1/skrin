using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.ModelBinding;
using System.Web.SessionState;
using ElasticSearch.Domain;
using ElasticSearch.Domain.CompanyDocument;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Service.Controllers
{
    //[SessionState(SessionStateBehavior.Disabled)]
    public class CompanyDocumentController : ApiController
    {
        private readonly string _elasticConnectionString = ConfigurationManager.AppSettings["elasticConnectionString"];

        /// <summary>
        /// Обновление коллекции документов
        /// </summary>
        /// <param name="documentList">Список документов</param>
        /// <param name="delete">
        /// Если true, то операция удаления, иначе операция создания новых и обновления существующих
        /// </param>
        /// <example>
        /// POST: api/CompanyDocument
        /// POST: api/CompanyDocument?delete=true
        /// </example>
        public void Post([FromBody] List<IElasticDocument> documentList, [FromUri] bool delete = false)
        {
            Domain.CompanyDocument.CompanyDocumentWriter productCompanyWriter =
                new CompanyDocumentWriter(_elasticConnectionString);
            /*
            Domain.CompanyDocument.CompanyDocumentWriter productCompanyWriter =
                //new CompanyDocumentWriter(new Uri("http://172.16.18.206:9200"));
                //new CompanyDocumentWriter(new Uri("https://search-promomash-elastic-domain-26y5ho4yiiwvd4pevfjkaqpyzy.us-west-2.es.amazonaws.com/"));
                //new CompanyDocumentWriter(new Uri("https://63gl8vi55e:c8jvqsvdv3@test-7782594285.us-west-2.bonsaisearch.net/"));
                new CompanyDocumentWriter(new Uri("http://127.0.0.1:9200/"));
            */

            if (delete)
            {
                productCompanyWriter.BulkDelete(documentList);
            }
            else
            {
                productCompanyWriter.BulkCreateOrUpdate(documentList);    
            }
        }

        /// <summary>
        /// Удаление документов
        /// </summary>
        /// <param name="ids">Идентификаторы в формате массива Json ["1","2","3"]</param>
        /// <param name="indexDelete">Если true, то операция удаления индекса целиком</param>
        /// <example>
        /// DELETE: api/CompanyDocument/["5","6","7"]
        /// </example>
        public void Delete([FromUri] string ids, [FromUri] bool indexDelete = false)
        {
            Domain.CompanyDocument.CompanyDocumentWriter сompanyDocumentWriter =
                new CompanyDocumentWriter(_elasticConnectionString);

            if (indexDelete)
            {
                сompanyDocumentWriter.IndexDelete(indexName: new CompanyDocument().IndexName, existingConnection: null);
            }
            else
            {
                JArray jArray = JArray.Parse(ids);

                var documentList =
                    jArray.Select(item => new CompanyDocument() { Id = item.ToString() }).ToList<IElasticDocument>();

                сompanyDocumentWriter.BulkDelete(documentList);    
            }
        }

        /*
        // GET: api/CompanyDocument/{...}
        public List<IElasticDocument> Get(object filterObject)
        {
            return new List<IElasticDocument>();
        }
        */ 
    }
}
