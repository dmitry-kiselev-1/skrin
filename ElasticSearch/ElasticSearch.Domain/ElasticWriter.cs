using System;
using System.Collections.Generic;
using Nest;

namespace ElasticSearch.Domain
{
    public abstract class ElasticWriter
    {
        /// <summary> Адрес сервера Elastic </summary>
        protected Uri ElasticEndPoint { get; private set; }

        /// <param name="elasticEndPoint">Адрес и порт сервера Elastic</param>
        protected ElasticWriter(Uri elasticEndPoint)
        {
            this.ElasticEndPoint = elasticEndPoint;
        }

        /// <summary> Обновление коллекции документов </summary>
        /// <param name="documentList"> Список для удаления </param>
        public abstract void BulkRemove(List<IElasticDocument> documentList);

        /// <summary> Обновление коллекции документов </summary>
        /// <param name="documentList"> Список для добавления </param>
        public abstract void BulkCreate(List<IElasticDocument> documentList);

        /// <summary> Создание индекса </summary>
        /// <param name="document">Тип документа для извлечения метаданных</param>
        protected void IndexCreate(IElasticDocument document)
        {
            var connectionSettings = new ConnectionSettings(this.ElasticEndPoint);
            var client = new ElasticClient(connectionSettings);

            var createIndexResponse = client.Index(document,
                i => i.Index(document.IndexName).Type(document.TypeName).Id(document.Id));

            var deleteDocumentResponse =
                client.Delete(new DeleteRequest(document.IndexName, document.TypeName, document.Id));
        }

        /// <summary> Удаление индекса </summary>
        /// <param name="indexName">Название удаляемого индекса</param>
        protected void IndexRemove(string indexName)
        {
            var connectionSettings = new ConnectionSettings(this.ElasticEndPoint);
            var client = new ElasticClient(connectionSettings);

            var deleteIndexResponse = client.DeleteIndex(Indices.Index(indexName));
        }
    }
}
