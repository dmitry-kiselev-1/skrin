﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ElasticSearch.Domain
{
    public interface IElasticDocument
    {
        /// <summary> Уникальный идентификатор документа в индексе </summary>
        int Id { get; set; }

        /// <summary> Уникальное наименование индекса Elastic (аналог базы данных) </summary>
        [JsonIgnore]
        string IndexName { get; }

        /// <summary> Уникальное наименование типа Elastic (аналог таблицы в базе данных) </summary>
        [JsonIgnore]
        string TypeName { get; }
    }
}
