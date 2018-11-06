
using System;

namespace ElasticSearch.Domain.CompanyDocument
{
    /// <summary> Компания </summary>
    public abstract class Company
    {
        #region search attributes

        /// <summary> Компания - регион </summary>
        public string Region { get; set; }

        /// <summary> Компания - отрасль (ОКВЭД) </summary>
        public string Sector { get; set; }

        /// <summary> Компания - выручка (возможен поиск по интервалу) </summary>
        public decimal Revenue { get; set; }

        /// <summary> Компания - дата ОГРН/ОГРНИП (возможен поиск по интервалу) </summary>
        public DateTime RegistrationDate { get; set; }

        #endregion

        /// <summary> Компания - код (ИНН или ОГРН/ОГРНИП) </summary>
        public string Code { get; set; }

        /// <summary> Компания - наименование ЮЛ/ИП </summary>
        public string Name { get; set; }

        /// <summary> Компания - оценочный индикатор (светофор) </summary>
        public string Indicator { get; set; }
    }
}
