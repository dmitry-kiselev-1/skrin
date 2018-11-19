using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Domain.CompanyDocument
{
    public class CompanyDocumentWriter : ElasticWriter
    {
        /// <summary> Конструктор </summary>
        /// <param name="elasticConnectionString">Адрес и порт сервера Elastic</param>
        public CompanyDocumentWriter(string elasticConnectionString)
            : base(elasticConnectionString)
        {}

        /// <summary> 
        /// Создание новых и обновление существующих документов (если индекса нет, он будет создан).
        /// </summary>
        /// <param name="documentList"> Список документов </param>
        public override async Task BulkCreateOrUpdate(List<IElasticDocument> documentList)
        {
            try
            {
                using (var connectionSettings = new ConnectionSettings(this.ElasticEndPoint))
                {
                    var client = new ElasticClient(connectionSettings);

                    if (!documentList.Any()) return;

                    var document = documentList.First();

                    var bulkResponse = await client.BulkAsync(descriptor =>
                        new BulkDescriptor()
                            .IndexMany(documentList)
                            .Index(document.IndexName)
                            .Type(document.TypeName)
                        );

                    if (!bulkResponse.IsValid)
                    {
                        throw new Exception(bulkResponse.DebugInformation);
                    }
                }

                // bulkResponse.IsValid (false при неверном ответе)
                // bulkResponse.DebugInformation (подробное описание ошибки)
                // bulkResponse.Errors
                // bulkResponse.ItemsWithErrors
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString(), e);
            }
        }

        /// <summary> 
        /// Удаление документов (если индекса нет, он будет создан).
        /// </summary>
        /// <param name="documentList"> Список документов </param>
        public override async Task BulkDelete(List<IElasticDocument> documentList)
        {
            try
            {
                using (var connectionSettings = new ConnectionSettings(this.ElasticEndPoint))
                {
                    var client = new ElasticClient(connectionSettings);

                    if (!documentList.Any()) return;

                    var document = documentList.First();

                    var bulkResponse = await client.BulkAsync(descriptor =>
                        new BulkDescriptor()
                            .DeleteMany(documentList)
                            .Index(document.IndexName)
                            .Type(document.TypeName)
                        );

                    if (!bulkResponse.IsValid)
                    {
                        throw new Exception(bulkResponse.DebugInformation);
                    }
                }

                // bulkResponse.IsValid (false при неверном ответе)
                // bulkResponse.DebugInformation (подробное описание ошибки)
                // bulkResponse.Errors
                // bulkResponse.ItemsWithErrors
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString(), e);
            }
        }

    }
}
