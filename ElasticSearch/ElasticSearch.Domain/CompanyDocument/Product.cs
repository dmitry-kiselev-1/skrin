
namespace ElasticSearch.Domain.CompanyDocument
{
    /// <summary> Товар (на основе ООС Госзакупки и Росаккредитации) </summary>
    public class Product
    {
        #region search attributes

        /// <summary> Товар - наименование </summary>
        public string Name { get; set; }

        #endregion

        /// <summary> Товар - цитата из описания </summary>
        public string Description { get; set; }
    }
}
