
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ElasticSearch.Domain.CompanyDocument
{
    /// <summary> Коллекция "товар - список поставщиков" </summary>
    public class CompanyDocument : IElasticDocument
    {
        /// <summary> Уникальный идентификатор документа в индексе (идентификатор компании) </summary>
        public string Id { get; set; }

        /// <summary> Дата обновления в Elastic </summary>
        public DateTime? UpdateDate { get; set; }

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
        public List<Product> SellerProducts { get; set; }

        /// <summary> Список товаров, приобретаемых данной компанией </summary>
        public List<Product> BuyerProducts { get; set; }
    }
}
