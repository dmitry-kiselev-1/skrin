using System;
using Newtonsoft.Json;

namespace ElasticSearch.Domain.CompanyDocument
{
    /// <summary> Параметры операций загрузки </summary>
    public class CompanyDocumentReaderSettings
    {
        /// <summary> Количество записей для загрузки </summary>
        public int BufferCount { get; set; }

        /// <summary> Последний идентификатор в загруженном буфере </summary>
        public int LastId { get; set; }

        /// <summary> Дата начала последней завершённой загрузки </summary>
        public DateTime LastCompleted { get; set; }

        /// <summary> Дата начала последней загрузки </summary>
        public DateTime? LastStarted { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
