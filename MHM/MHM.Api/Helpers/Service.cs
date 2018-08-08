using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MHM.Api.Helpers
{
    public static class Service
    {
        public static void SendMail(string emailid, string subject, string body)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Host = ConfigurationManager.AppSettings["SmtpClient"];
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);

                MailAddress frmAddress = new MailAddress(ConfigurationManager.AppSettings["FromMailAddress"], "MHM");

                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["FromMailAddress"], ConfigurationManager.AppSettings["Password"]);
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;

                MailMessage msg = new MailMessage();
                msg.From = frmAddress;
                msg.To.Add(new MailAddress(emailid));

                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.Body = body;

                client.Send(msg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMailWithAttaitchment(string ApplicantEmail, string AgentEmail, string ReportEmail, string subject, string body, byte[] file, string caseTitle)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Host = ConfigurationManager.AppSettings["SmtpClient"];
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);

                MailAddress frmAddress = new MailAddress(ConfigurationManager.AppSettings["FromMailAddress"], "MyHealthMath");

                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["FromMailAddress"], ConfigurationManager.AppSettings["Password"]);
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;

                MailMessage msg = new MailMessage();
                msg.From = frmAddress;
                msg.To.Add(new MailAddress(WebUtility.UrlDecode(ApplicantEmail)));
                msg.CC.Add(new MailAddress(WebUtility.UrlDecode(AgentEmail)));
                msg.Bcc.Add(new MailAddress(WebUtility.UrlDecode(ReportEmail)));
                msg.Attachments.Add(new Attachment(new MemoryStream(file), caseTitle));

                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.Body = body;

                client.Send(msg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LogError(string fileName, string content)
        {
            // Create a file to write to.
            //string createText = "Hello and Welcome" + Environment.NewLine;
            var path = HttpContext.Current.Server.MapPath("~/Logs/") + fileName + ".txt";
            File.WriteAllText(path, content);
        }
    }
}
