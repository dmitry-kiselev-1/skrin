
using System;

namespace ElasticSearch.Domain.CompanyDocument
{
    /// <summary> Товар (на основе ООС Госзакупки и Росаккредитации) </summary>
    public class Product : IEquatable<Product>
    {
        #region search attributes

        /// <summary> Товар - наименование </summary>
        public string Name { get; set; }

        /// <summary> Код товара </summary>
        public string Code { get; set; }

        /// <summary> Единица измерения </summary>
        public string Measure { get; set; }

        #endregion

        /// <summary> Уникальный идентификатор товара</summary>
        public string Id { get; set; }

        /// <summary> Товар - цитата из описания </summary>
        public string Description { get; set; }

        public bool Equals(Product other)
        {
            return this.Id == other.Id;
        }
    }
}
