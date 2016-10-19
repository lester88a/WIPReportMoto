using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WIPReportMot.Configuration;

namespace WIPReportMot.EmailService
{
    public class Email
    {
        //default setting of the email environment
        //private string _recipient;
        private string _sender;
        private string _smtpServer;
        private int _smtpPort;
        private string _senderUserName;
        private string _senderUserPass;
        private Attachment attachment;

        MailMessage message = new MailMessage();

        public Email()
        {
            //this._recipient = Config.Instance.GetRecipient();
            this._sender = Config.Instance.GetSender();
            this._smtpServer = Config.Instance.GetSmtpServer();
            this._smtpPort = Convert.ToInt32(Config.Instance.GetSmtpPort());
            this._senderUserName = Config.Instance.GetSenderUserName();
            this._senderUserPass = Config.Instance.GetSenderUserPass();
        }

        public void SendEmailMethod(string fileName, string recipient, string subject, string body)
        {
            message.To.Add(recipient);
            message.Subject = subject;
            message.From = new MailAddress(_sender);
            message.Body = body;
            SmtpClient smtp = new SmtpClient(_smtpServer, _smtpPort);

            //configure the client 
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(_senderUserName, _senderUserPass);


            //attachment
            attachment = new Attachment(fileName);
            message.Attachments.Add(attachment);

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
            attachment.Dispose();
        }
    }
}
