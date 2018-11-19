
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ElasticSearch.Domain.CompanyDocument
{
    /// <summary> Компания </summary>
    public class Company : IEquatable<Company>
    {
        #region search attributes

        /// <summary> Регион, код региона по регистрации в налоговых органах </summary>
        public int? RegionTaxId { get; set; }

        /// <summary> Регион, наименование региона по регистрации в налоговых органах </summary>
        public string RegionTaxName { get; set; }

        /// <summary> Регион, ОКАТО-код </summary>
        public String RegionOkatoCode { get; set; }

        /// <summary> Регион, список: ОКАТО-код и все его parent-коды</summary>
        public List<String> RegionOkatoCodes { get; set; }

        /// <summary> Отрасль, ОКВЭД-код </summary>
        public String SectorOkvedCode { get; set; }

        /// <summary> Отрасль, список: ОКВЭД-код и цепочка его parent-кодов</summary>
        public List<String> SectorOkvedCodes { get; set; }

        /// <summary> Отрасль, список: Дополнительные ОКВЭД-коды плюс цепочки parent-кодов каждого из них</summary>
        [JsonIgnore]
        public List<String> SectorOkvedАdditionalCodes { get; set; }

        /// <summary> Выручка, значение (возможен поиск по интервалу) </summary>
        public decimal? RevenueValue { get; set; }

        /// <summary> Выручка, год</summary>
        public int? RevenueYear { get; set; }

        /// <summary> Дата ОГРН/ОГРНИП (возможен поиск по интервалу) </summary>
        public DateTime? OgrnRegDate { get; set; }

        #endregion

        /// <summary> Уникальный идентификатор компании</summary>
        public string Id { get; set; }

        /// <summary> ИНН </summary>
        public string Inn { get; set; }

        /// <summary> ОГРН </summary>
        public string Ogrn { get; set; }

        /// <summary> Полное наименование ЮЛ/ИП </summary>
        // ToDo: исключить из индексации
        public string FullName { get; set; }

        /// <summary> Полное наименование ЮЛ/ИП </summary>
        // ToDo: исключить из индексации
        public string ShortName { get; set; }
        
        /// <summary> Поисковое наименование ЮЛ/ИП </summary>
        public string SearchName { get; set; }

        /// <summary> Список ролей (покупатель, продавец) </summary>
        public List<CompanyRoleEnum> Roles { get; set; }

        /// <summary> Дата обновления из источника (sql) </summary>
        public DateTime? UpdateDate { get; set; }

        /*
        /// <summary> Компания - оценочный индикатор (светофор) </summary>
        public string Indicator { get; set; }
        */

        public bool Equals(Company other)
        {
            return this.Id == other.Id;
        }
    }
}
