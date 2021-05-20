
using NetCore.Utils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TransferWorker.Utility
{
    public class SendMail
    {
        public string SendMailTSG(string MailNhan, string MailGui, string Pwd, string Host, string Post, string Content, string Subject)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(Host);

                SmtpServer.UseDefaultCredentials = true;
                SmtpServer.Port = int.Parse(Post);
                SmtpServer.Credentials = new System.Net.NetworkCredential(MailGui, Pwd);
                SmtpServer.EnableSsl = true;

                mail.From = new MailAddress(MailGui);
                mail.Subject = Subject;
                mail.Body = Content;
                mail.IsBodyHtml = true;


                mail.To.Add(new MailAddress(MailNhan));

                SmtpServer.Send(mail);
                return "OK";
            }
            catch (Exception ex)
            {
                NLogManager.LogError("Sendmail Error" + ex.ToString());
                return ex.ToString();
            }
          
        }
    }
}
