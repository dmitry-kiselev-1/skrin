using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace ElasticSearch.Domain
{
    /// <summary>
    /// Обязанность класса - записывать коллекции докуметов в Elastic
    /// </summary>
    public abstract class ElasticWriter
    {
        /// <summary> Адрес и порт сервера Elastic </summary>
        protected Uri ElasticEndPoint { get; private set; }

        /// <summary> Конструктор </summary>
        /// <param name="elasticConnectionString">Адрес и порт сервера Elastic</param>
        protected ElasticWriter(string elasticConnectionString)
        {
            this.ElasticEndPoint = new Uri(elasticConnectionString);
        }

        /// <summary> Удаление документов </summary>
        /// <param name="documentList"> Список документов </param>
        public abstract Task BulkDelete(List<IElasticDocument> documentList);

        /// <summary> Создание новых и обновление существующих документов </summary>
        /// <param name="documentList"> Список документов </param>
        public abstract Task BulkCreateOrUpdate(List<IElasticDocument> documentList);

        /// <summary> Создание индекса </summary>
        /// <param name="document">Тип документа для извлечения метаданных</param>
        /// <param name="existingConnection">Открытое подключение - если его нет, будет создано новое</param>
        public async Task IndexCreate(IElasticDocument document, ConnectionSettings existingConnection)
        {
            try
            {

                var connectionSettings = existingConnection ?? new ConnectionSettings(this.ElasticEndPoint);
                var client = new ElasticClient(connectionSettings);

                var createIndexResponse = client.Index(document,
                    i => i.Index(document.IndexName).Type(document.TypeName).Id(document.Id));

                var response =
                    await client.DeleteAsync(new DeleteRequest(document.IndexName, document.TypeName, document.Id));

                if (!response.IsValid)
                {
                    throw new Exception(response.DebugInformation);
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

        /// <summary> Удаление индекса </summary>
        /// <param name="indexName">Название удаляемого индекса</param>
        /// <param name="existingConnection">Открытое подключение - если его нет, будет создано новое</param>
        public async Task IndexDelete(string indexName, ConnectionSettings existingConnection)
        {
            try
            {
                var connectionSettings = existingConnection ?? new ConnectionSettings(this.ElasticEndPoint);
                var client = new ElasticClient(connectionSettings);

                var response = 
                    await client.DeleteIndexAsync(Indices.Index(indexName));

                if (!response.IsValid)
                {
                    throw new Exception(response.DebugInformation);
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
