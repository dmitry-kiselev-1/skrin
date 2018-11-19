
namespace ElasticSearch.Domain.CompanyDocument
{
    /// <summary> Роль компании </summary>
    public enum CompanyRoleEnum: byte
    {
        /// <summary> Продавец </summary>
        Seller  = 1,

        /// <summary> Покупатель </summary>
        Buyer   = 2
    }
}