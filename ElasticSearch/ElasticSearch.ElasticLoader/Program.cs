using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using ElasticSearch.ElasticLoader.CompanyDocument;

namespace ElasticSearch.ElasticLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            ElasticLoaderLog.Clear();

            DateTime starDateTime = DateTime.Now;

            Stopwatch stopwatchTotal = new Stopwatch();
            string stopwatchFormat = @"hh\:mm\:ss";
            //string stopwatchFormat = @"hh\:mm\:ss\.fff";

            stopwatchTotal.Start();

            if (args.Length != 0)
            {
                switch (args[0])
                {
                    case "-company":
                        try
                        {
                            stopwatchTotal.Start();
                            ElasticLoaderLog.Write("Старт обработки: {0}", starDateTime);
                            
                            //throw new Exception("Ошибка", new Exception("Внутренняя ошибка"));

                            var companyDocumentLoader = new CompanyDocumentLoader(
                                sqlConnectionString: ConfigurationManager.AppSettings["sqlConnectionString"],
                                elasticConnectionString: ConfigurationManager.AppSettings["elasticConnectionString"]);

                            Task.Factory.StartNew(() => companyDocumentLoader.Load()).Wait();
                        }
                        catch (Exception e)
                        {
                            ElasticLoaderLog.Error(e.Message, e);
                            Console.ReadLine();
                        }
                        break;
                    default:
                        ElasticLoaderLog.Write("Неизвестная команда.");
                        return;
                }
            }
            else
            {
                ElasticLoaderLog.Write("Ключи запуска:");
                ElasticLoaderLog.Write("-company    запуск загрузки компаний с товарами");
                
                Console.ReadLine();
                return;
            }

            //ElasticLoaderLog.Write("Загрузка завершена.");
            
            Console.ReadLine();
        }
    }
}
