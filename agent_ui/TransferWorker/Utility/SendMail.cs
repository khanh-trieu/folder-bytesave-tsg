using Limilabs.Client.SMTP;
using NetCore.Utils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TransferWorker.Models;

namespace TransferWorker.Utility
{
    public class SendMail
    {
        public async Task SendMailTSG(string MailNhan, string JobName, string ListFile, bool IsSuccess)
        {
            NLogManager.LogError("Sendmail");
            var GetEmail = GetAppJson("appsettings");
            try
            {
                if (GetEmail.host == "" || GetEmail.port == "" || GetEmail.email == "")
                {
                    return;
                }
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(GetEmail.host);
                var password = DecryptGenLicense(GetEmail.pwd);
                SmtpServer.UseDefaultCredentials = true;
                SmtpServer.Port = int.Parse(GetEmail.port);
                SmtpServer.Credentials = new System.Net.NetworkCredential(GetEmail.email, password);
                SmtpServer.EnableSsl = true;
                if (IsSuccess == true)
                {
                    mail.From = new MailAddress(GetEmail.email);
                    mail.Subject = GetEmail.subject;
                    mail.Body = "[Thành công] " + DateTime.Now.ToString("dd/MM/yyyy hh:mm tt") + " Tác vụ sao lưu: " + JobName + " : " + ListFile;
                    mail.IsBodyHtml = true;
                }
                else
                {
                    mail.From = new MailAddress(GetEmail.email);
                    mail.Subject = GetEmail.subject;
                    mail.Body = "[Không thành công] " + DateTime.Now.ToString("dd/MM/yyyy hh:mm tt") + " Tác vụ sao lưu: " + JobName;
                    mail.IsBodyHtml = true;
                }
                var sendMail = new List<Task>();
                var lstEmail = MailNhan.Split(",");
                foreach (var item in lstEmail)
                {
                    mail.To.Add(new MailAddress(item));
                    NLogManager.LogError("Sendmail to " + item);
                    //SmtpServer.SendMailAsync(mailMessage);
                    var t = Task.Run(async () =>
                    {
                        SmtpServer.Send(mail);
                    });
                    sendMail.Add(t);
                }
                await Task.WhenAll(sendMail);
            }
            catch (Exception ex)
            {
                NLogManager.LogError("Sendmail Error" + ex);    
            }
        }
        private AppJson GetAppJson(string fileName)
        {
            string localFilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
             + "\\" + fileName + ".json";
            var str = File.ReadAllText(localFilePath);
            var configs = JsonSerializer.Deserialize<AppJson>(str);
            return configs;
        }
        public string DecryptGenLicense(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("khanh.trieu"));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        try
                        {
                            byte[] cipherBytes = Convert.FromBase64String(cipher.Substring(0, cipher.Length - 3));
                            byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                            return UTF8Encoding.UTF8.GetString(bytes);
                        }
                        catch (Exception ex)
                        {
                            return "";
                        }

                    }
                }
            }
        }
    }
}
