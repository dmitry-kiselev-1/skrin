using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Domain.CompanyDocument
{
    public class CompanyDocumentWriter: ElasticWriter
    {
        public CompanyDocumentWriter(Uri elasticEndPoint): base(elasticEndPoint) {}

        public override void BulkCreate(List<IElasticDocument> documentList)
        {
            var connectionSettings = new ConnectionSettings(this.ElasticEndPoint);
            var client = new ElasticClient(connectionSettings);

            if (!documentList.Any()) return;

            var document = documentList.First();

            //this.IndexRemove(document.IndexName);

            if (!client.IndexExists(Indices.Index(document.IndexName)).Exists)
            {
                this.IndexCreate(document);    
            }

            var bulkResponse = client.Bulk(descriptor =>
                new BulkDescriptor()
                    .CreateMany(documentList)
                        .Index(document.IndexName)
                        .Type(document.TypeName)
                );
        }

        public override void BulkRemove(List<IElasticDocument> documentList)
        {
            var connectionSettings = new ConnectionSettings(this.ElasticEndPoint);
            var client = new ElasticClient(connectionSettings);

            var bulkResponse = client.Bulk(descriptor =>
                new BulkDescriptor()
                    //.DeleteMany<CompanyDocument>(new[] { "1", "2" })
                );
        }
    }
}
