
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ElasticSearch.Domain.CompanyDocument
{
    /// <summary> Коллекция "товар - список поставщиков" </summary>
    public class CompanyDocument : IElasticDocument
    {
        /// <summary> Уникальный идентификатор документа в индексе (идентификатор компании) </summary>
        public int Id { get; set; }

        /// <summary> Уникальное наименование индекса Elastic (аналог базы данных) </summary>
        [JsonIgnore]
        public string IndexName
        {
            get { return "company-index"; }
        }

        /// <summary> Уникальное наименование типа Elastic (аналог таблицы в базе данных) </summary>
        [JsonIgnore]
        public string TypeName
        {
            get { return "company-document"; }
        }

        /// <summary> Компания </summary>
        public Company Company { get; set; }

        /// <summary> Список товаров, поставляемых данной компанией </summary>
        public List<Product> ProductList { get; set; }
    }
}
