using System;
using System.Xml;

namespace WIPReportMot.Configuration
{
    public class Config
    {
        //Thread Safe Singleton without using locks and no lazy instantiation
        private static readonly Config instance = new Config();
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Config() { }
        private Config() { }

        private static string DataSource { get; set; }
        private static string InitialCatalog { get; set; }
        private static string Manufacturer { get; set; }
        private static string ConfigFile { get; set; }
        private static string FileSavedPath { get; set; }
        //instance variables for email
        private static string Recipient { get; set; }
        private static string RecipientAdmin { get; set; }
        private static string Sender { get; set; }
        private static string SmtpServer { get; set; }
        private static int SmtpPort { get; set; }
        private static string SenderUserName { get; set; }
        private static string SenderUserPass { get; set; }

        //constructor
        public static Config Instance
        {
            get
            {
                ConfigFile = System.AppDomain.CurrentDomain.BaseDirectory + @"Config.xml";
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigFile);
                //config for server
                DataSource = doc.DocumentElement.SelectSingleNode("/config/dataSource").InnerText;
                InitialCatalog = doc.DocumentElement.SelectSingleNode("/config/initialCatalog").InnerText;
                Manufacturer = doc.DocumentElement.SelectSingleNode("/config/manafacturer").InnerText;
                FileSavedPath = doc.DocumentElement.SelectSingleNode("/config/fileSavedPath").InnerText;
                //config for email
                Recipient = doc.DocumentElement.SelectSingleNode("/config/email/recipient").InnerText;
                RecipientAdmin = doc.DocumentElement.SelectSingleNode("/config/email/recipientAdmin").InnerText;
                Sender = doc.DocumentElement.SelectSingleNode("/config/email/sender").InnerText;
                SmtpServer = doc.DocumentElement.SelectSingleNode("/config/email/smtpServer").InnerText;
                SmtpPort = Convert.ToInt32(doc.DocumentElement.SelectSingleNode("/config/email/smtpPort").InnerText);
                SenderUserName = doc.DocumentElement.SelectSingleNode("/config/email/senderUserName").InnerText;
                SenderUserPass = doc.DocumentElement.SelectSingleNode("/config/email/senderUserPass").InnerText;

                return instance;
            }
        }
        /// <summary>
        /// Get Data Source 
        /// </summary>
        /// <returns></returns>
        public string GetDataSource()
        {
            return DataSource;
        }

        /// <summary>
        /// Get InitialCatlog
        /// </summary>
        /// <returns></returns>
        public string GetInitialCatalog()
        {
            return InitialCatalog;
        }

        /// <summary>
        /// Get Manufacturer
        /// </summary>
        /// <returns></returns>
        public string GetManufacturer()
        {
            return Manufacturer;
        }

        /// <summary>
        /// Get File Saved Path
        /// </summary>
        /// <returns></returns>
        public string GetFileSavedPath()
        {
            return FileSavedPath;
        }

        /// <summary>
        /// Get Recipient
        /// </summary>
        /// <returns></returns>
        public string GetRecipient()
        {
            return Recipient;
        }

        /// <summary>
        /// Get Recipient Admin
        /// </summary>
        /// <returns></returns>
        public string GetRecipientAdmin()
        {
            return RecipientAdmin;
        }

        /// <summary>
        /// Get Sender
        /// </summary>
        /// <returns></returns>
        public string GetSender()
        {
            return Sender;
        }

        /// <summary>
        /// Get Smtp Server
        /// </summary>
        /// <returns></returns>
        public string GetSmtpServer()
        {
            return SmtpServer;
        }

        /// <summary>
        /// Get Smtp Port
        /// </summary>
        /// <returns></returns>
        public int GetSmtpPort()
        {
            return SmtpPort;
        }

        /// <summary>
        /// Get Sender User Name
        /// </summary>
        /// <returns></returns>
        public string GetSenderUserName()
        {
            return SenderUserName;
        }

        /// <summary>
        /// Get Sender User Pass
        /// </summary>
        /// <returns></returns>
        public string GetSenderUserPass()
        {
            return SenderUserPass;
        }
    }
}
