using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.ElasticLoader
{
    internal static class ElasticLoaderLog
    {
        private static string _path = Path.Combine(
            Environment.CurrentDirectory, @"ElasticSearch.ElasticLoader.log");

        internal static void Clear()
        {
            try
            {
                Console.Clear();
                File.Delete(_path);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка очистки ElasticSearch.ElasticLoader.log", e);
            }
        }

        internal static void Write(string message)
        {
            try
            {
                Console.WriteLine(message);
                File.AppendAllText(_path, message + "\n");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка записи в ElasticSearch.ElasticLoader.log", e);
            }
        }

        internal static void Write(string message, params object[] messageParameters)
        {
            string logMessage = String.Format(message, messageParameters);
            Write(logMessage);
        }

        internal static void Error(string message, Exception exception)
        {
            string errorSubject = "Ошибка ElasticSearch.ElasticLoader. " + message;
            
            Write("");
            Write(errorSubject);
            Write(exception.ToString());

            SendEmail(message: exception.ToString(), subject: errorSubject);
        }

        internal static void Error(Exception exception, string message, params object[] messageParameters)
        {
            string logMessage = String.Format(message, messageParameters);
            Error(logMessage, exception);
        }

        internal static void SendEmail(string message, string subject)
        {
            var emailList = ConfigurationManager.AppSettings["emailList"];

            string fromAddress = "service@skrin.ru";
            string toAddress = emailList;
            SmtpClient smpt = new SmtpClient("mail.skrin.ru");
            try
            {
                MailMessage msg = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = message,
                    Priority = MailPriority.High,
                    CC = {"kd0001@yandex.ru"}
                };
                
                smpt.Send(msg);
            }
            
            catch (Exception e)
            {
                Write("");
                Write("Не удалось отправить Email об ошибке:");
                Write(e.ToString());
                Write("");
            }
        }
    }
}
