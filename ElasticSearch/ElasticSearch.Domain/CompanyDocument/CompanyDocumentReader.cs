using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ElasticSearch.Domain.CompanyDocument
{
    public class CompanyDocumentReader: SourceReader
    {
        /// <summary> Адрес SQL-сервера </summary>
        public string SqlConnectionString { get; private set; }

        /// <summary> Адрес SQL-сервера </summary>
        public CompanyDocumentReaderSettings ReaderSettings { get; private set; }

        /// <summary> Конструктор </summary>
        /// <param name="sqlConnectionString">Строка подключения к SQL-серверу</param>
        /// <param name="readerSettings">Параметры чтения</param>
        public CompanyDocumentReader(string sqlConnectionString, CompanyDocumentReaderSettings readerSettings)
        {
            this.SqlConnectionString = sqlConnectionString;
            this.ReaderSettings = readerSettings;
        }

        /// <summary>
        /// Читает из источника буфер и возвращает его в виде коллекции документов Elastic
        /// </summary>
        public override async Task<List<IElasticDocument>> Read()
        {
            return await Task.Factory.StartNew(() =>
            {
                using (SqlConnection sqlConnection = new SqlConnection(SqlConnectionString))
                {
                    sqlConnection.Open();

                    var filterRowsCount = GetBufferFilter(sqlConnection);

                    if (filterRowsCount == 0)
                    {
                        return null;
                    };

                    var companyDataSet = GetBuffer(sqlConnection, "Company");
                    var companyDataRows = companyDataSet.Tables[0].AsEnumerable();

                    var revenueLookup = GetBuffer(sqlConnection, "Revenue")
                        .Tables[0]
                        .AsEnumerable()
                        .AsParallel()
                            //.WithMergeOptions(ParallelMergeOptions.Default)
                            //.WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                        .ToLookup(
                            row => row.Field<int>("us_id"),
                            row => new
                            {
                                //us_id = row.Field<int>("us_id"),
                                year = row.Field<int?>("year"),
                                value = row.Field<decimal?>("value")
                            });

                    var okvedLookup = GetBuffer(sqlConnection, "Okved")
                        .Tables[0]
                        .AsEnumerable()
                        .AsParallel()
                        //.WithMergeOptions(ParallelMergeOptions.Default)
                        //.WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                        .ToLookup(
                            row => row.Field<int>("us_id"),
                            row => new 
                            {
                                //us_id = row.Field<int>("us_id"),
                                codes = row
                                    .Field<string>("codes")
                                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                    .Distinct().ToList()
                            });

                    var companyDocuments = companyDataRows
                        .AsParallel()
                            //.WithMergeOptions(ParallelMergeOptions.Default)
                            //.WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                        .Select(r =>
                        new CompanyDocument()
                        {
                            Id = r.Field<int>("us_id").ToString(),
                            Company = new Company()
                            {
                                Id = r.Field<int>("us_id").ToString(),
                                UpdateDate = r.Field<DateTime?>("us_update_date"),

                                Inn = r.Field<string>("inn"),
                                Ogrn = r.Field<string>("ogrn"),

                                OgrnRegDate = r.Field<DateTime?>("ogrn_reg_date"),

                                ShortName = r.Field<string>("short_name"),
                                FullName = r.Field<string>("full_name"),
                                SearchName = r.Field<string>("search_name"),

                                RegionOkatoCode = r.Field<string>("region_okato_code"),
                                RegionOkatoCodes =
                                    (r.Field<string>("region_okato_code_list") ?? string.Empty)
                                        .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
                                        .Distinct()
                                        .ToList(),

                                RegionTaxId = r.Field<int?>("region_tax_id"),
                                RegionTaxName = r.Field<string>("region_tax_name"),

                                SectorOkvedCode = r.Field<string>("sector_okved_code"),
                                SectorOkvedCodes =
                                    (r.Field<string>("sector_okved_code_parent_list") ?? string.Empty)
                                        .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                        .Distinct()
                                        .ToList(),

                                SectorOkvedАdditionalCodes =
                                    okvedLookup[r.Field<int>("us_id")].Any()
                                        ? okvedLookup[r.Field<int>("us_id")].First().codes
                                        : null,

                                RevenueValue =
                                    revenueLookup[r.Field<int>("us_id")].Any()
                                        ? revenueLookup[r.Field<int>("us_id")].First().value
                                        : null,

                                RevenueYear =
                                    revenueLookup[r.Field<int>("us_id")].Any()
                                        ? revenueLookup[r.Field<int>("us_id")].First().year
                                        : null,

                                Roles = null
                            }
                        });

                    return companyDocuments.Cast<IElasticDocument>().ToList();
                }
            });
        }

        private int GetBufferFilter(SqlConnection sqlConnection)
        {
            string queryString =
                File.ReadAllText(
                    Path.Combine(
                        Environment.CurrentDirectory, @"CompanyDocument\sql\Filter.sql"));

            var command = new SqlCommand(queryString, sqlConnection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 0
            };

            command.Parameters.AddWithValue("@BufferCount", this.ReaderSettings.BufferCount);
            command.Parameters.AddWithValue("@LastId", this.ReaderSettings.LastId);
            command.Parameters.AddWithValue("@LastCompleted", this.ReaderSettings.LastCompleted);

            return command.ExecuteNonQuery();
        }

        private DataSet GetBuffer(SqlConnection sqlConnection, string bufferName)
        {
            string queryString =
                File.ReadAllText(
                    Path.Combine(
                        Environment.CurrentDirectory,
                        String.Format(@"CompanyDocument\sql\{0}.sql", bufferName)));

            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(new SqlCommand()
            {
                Connection = sqlConnection,
                CommandType = CommandType.Text,
                CommandText = queryString,
                CommandTimeout = 0
            });

            sqlDataAdapter.Fill(dataSet);

            return dataSet;
        }

    }
}
