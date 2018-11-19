using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Domain
{
    /// <summary>
    /// Обязанность класса - читать источники и преобразовывать их в коллекции докуметов Elastic
    /// </summary>
    public abstract class SourceReader
    {
        /// <summary>
        /// Читает из источника буфер и возвращает его в виде коллекцию документов Elastic
        /// </summary>
        public abstract Task<List<IElasticDocument>> Read();
    }
}
