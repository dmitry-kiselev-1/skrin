using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.Domain;
using ElasticSearch.Domain.CompanyDocument;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElasticSearch.ElasticLoader.CompanyDocument
{
    /// <summary>
    /// Выборка компаний для загрузки в Elastic.
    
    /// Загружаемые сущности:
    ///     Компания
    ///     Список ОКВЭД для компании
    ///     Выручка компании
    ///     Список товаров - на продажу и для покупки
    /// 
    /// 0) Обновить параметры загрузки: 
    ///     @LastStarted (дата начала загрузки)

    /// 1) Считать параметры загрузки: 
    ///     @BufferCount (количество записей в буфере загрузки)
    ///     @LastId (последний из ранее загруженных id, начальное значение 0) 
    ///     @LastCompleted (дата последней завершённой загрузки, начальное значение '1753-01-01T00:00:00.000')

    /// 2) Выгрузить в Elastic 
    ///     топ @BufferCount записей 
    ///     после @LastId (us_id > @LastId)
    ///     после @LastCompleted (us_update_date > @LastCompleted)

    /// 3) Обновить параметры загрузки: 
    ///     @LastId присвоить последний us_id из шага 2

    /// 4) Повторять шаги 1-3, если записи закончились, перейти к 5

    /// 5) Обновить параметры загрузки: 
    ///     @LastId присвоить 0
    ///     @LastCompleted присвоить @LastStarted
    ///     @LastStarted присвоить null

    /// </summary>
    public class CompanyDocumentLoader
    {
        /// <summary> Адрес SQL-сервера </summary>
        public string SqlConnectionString { get; private set; }

        /// <summary> Адрес Elastic-сервера </summary>
        public string ElasticConnectionString { get; private set; }

        /// <summary> Конструктор </summary>
        /// <param name="sqlConnectionString">Строка подключения к SQL-серверу</param>
        /// <param name="elasticConnectionString">Строка подключения к Elastic-серверу</param>
        public CompanyDocumentLoader(string sqlConnectionString, string elasticConnectionString)
        {
            SqlConnectionString = sqlConnectionString;
            ElasticConnectionString = elasticConnectionString;
        }

        /// <summary> Обновление индекса "company-index" в Elastic </summary>
        /// <param name="maxIterations">Ограничитель числа итераций</param>
        public async Task Load(int? maxIterations = null)
        {
            Stopwatch stopwatchTotal = new Stopwatch();
            stopwatchTotal.Start();

            Stopwatch stopwatchReadBuffer = new Stopwatch();
            Stopwatch stopwatchWriteBuffer = new Stopwatch();

            TimeSpan timeSpanReadBufferTotal = new TimeSpan();
            TimeSpan timeSpanWriteBufferTotal = new TimeSpan();

            ElasticLoaderLog.Write("Чтение конфигурации...");

            var readerSettings = this.GetReaderSettings();
                readerSettings.LastStarted = DateTime.Now;

            ElasticLoaderLog.Write(readerSettings.ToString());

            int iteration = 0;
           
            string stopwatchFormat = @"hh\:mm\:ss";
            //string stopwatchFormat = @"hh\:mm\:ss\.fff";

            while (true)
            {
                iteration++;
                
                stopwatchReadBuffer.Restart();
                
                var buffer = await ReadBuffer(readerSettings);
                
                stopwatchReadBuffer.Stop();
                timeSpanReadBufferTotal = timeSpanReadBufferTotal.Add(stopwatchReadBuffer.Elapsed);

                ElasticLoaderLog.Write("");
                ElasticLoaderLog.Write(
                    "{0}. Read  {1}, time {2}, total time {3},\t id> {4}",
                    iteration, 
                    readerSettings.BufferCount,
                    stopwatchReadBuffer.Elapsed.ToString(stopwatchFormat),
                    timeSpanReadBufferTotal.ToString(stopwatchFormat),
                    readerSettings.LastId);

                if (buffer != null && buffer.Any())
                {
                    stopwatchWriteBuffer.Restart();
                    
                    await this.WriteBuffer(buffer, readerSettings);
                    
                    stopwatchWriteBuffer.Stop();
                    timeSpanWriteBufferTotal = timeSpanWriteBufferTotal.Add(stopwatchWriteBuffer.Elapsed);

                    ElasticLoaderLog.Write(
                        "{0}. Write {1}, time {2}, total time {3},\t id: {4} - {5}",
                        iteration, buffer.Count,
                        stopwatchWriteBuffer.Elapsed.ToString(stopwatchFormat),
                        timeSpanWriteBufferTotal.ToString(stopwatchFormat),
                        buffer.OrderBy(d => int.Parse(d.Id)).First().Id,
                        buffer.OrderBy(d => int.Parse(d.Id)).Last().Id);
                }
                else
                    break;

                if (maxIterations.HasValue && maxIterations.Value == iteration)
                    break;

                ElasticLoaderLog.Write(
                    "{0}. Total elapsed: {1}, total rows: {2}",
                    iteration,
                    stopwatchTotal.Elapsed.ToString(stopwatchFormat),
                    readerSettings.BufferCount * iteration);
            }
            stopwatchTotal.Stop();

            // успешное завершение загрузки:
            ElasticLoaderLog.Write("Запись конфигурации...");
            ElasticLoaderLog.Write(readerSettings.ToString());

            // отметка о завершении выборки не ставится в случае ограничения итераций:
            if (!maxIterations.HasValue)
            {
                readerSettings.LastId = 0;
                readerSettings.LastCompleted = readerSettings.LastStarted.Value;
                readerSettings.LastStarted = null;
            }

            this.SaveReaderSettings(readerSettings);

            stopwatchTotal.Stop();
            ElasticLoaderLog.Write("Обработка завершена в: {0}", DateTime.Now);
            ElasticLoaderLog.Write("Длительность обработки: {0}", stopwatchTotal.Elapsed.ToString(stopwatchFormat));
        }

        /// <summary> Чтение буфера в SQL </summary>
        private async Task<List<IElasticDocument>> ReadBuffer(CompanyDocumentReaderSettings readerSettings)
        {
            var reader = new CompanyDocumentReader(SqlConnectionString, readerSettings);
            var buffer = await reader.Read();
            return buffer;
        }

        /// <summary> Запись буфера в Elastic </summary>
        private async Task WriteBuffer(List<IElasticDocument> buffer, CompanyDocumentReaderSettings readerSettings)
        {
            foreach (var doc in buffer)
            { doc.UpdateDate = readerSettings.LastStarted; }

            var writer = new CompanyDocumentWriter(ElasticConnectionString);
            await writer.BulkCreateOrUpdate(buffer);

            // корректировка параметров для следующей операции чтения:
            readerSettings.LastId = int.Parse(
                buffer.OrderByDescending(d => int.Parse(d.Id)).First().Id);

            this.SaveReaderSettings(readerSettings);
        }

        private CompanyDocumentReaderSettings GetReaderSettings(bool getDefaultSettings = false)
        {
            CompanyDocumentReaderSettings defaultSettings = new CompanyDocumentReaderSettings()
            {
                BufferCount = 10000,
                LastId = 0,
                LastCompleted = DateTime.Parse("1753-01-01T00:00:00.000"),
                LastStarted = null
            };

            if (getDefaultSettings)
            {
                return defaultSettings;
            }

            try
            {
                CompanyDocumentReaderSettings settings;

                string settingsString
                    = ElasticLoaderSettings.Default.CompanyDocumentReaderSettings;

                if (String.IsNullOrWhiteSpace(settingsString))
                {
                    settings = defaultSettings;
                }
                else
                {
                    settings =
                        (CompanyDocumentReaderSettings)
                            JsonConvert.DeserializeObject(settingsString, typeof(CompanyDocumentReaderSettings));
                }

                return settings;
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка чтения файла конфигурации CompanyDocumentLoaderSettings", e);
            }
        }

        private void SaveReaderSettings(CompanyDocumentReaderSettings readerSettings)
        {
            try
            {
                string settingsString =
                    ElasticLoaderSettings.Default.CompanyDocumentReaderSettings =
                        JsonConvert.SerializeObject(readerSettings, Formatting.Indented);

                ElasticLoaderSettings.Default.Save();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка записи файла конфигурации CompanyDocumentLoaderSettings", e);
            }
        }

        /// <summary>
        /// Сбрасывает параметры чтения до стартовых
        /// </summary>
        public void SetDefaultReaderSettings()
        {
            SaveReaderSettings(this.GetReaderSettings(getDefaultSettings: true));
        }
    }
}
