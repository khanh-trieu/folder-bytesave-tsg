using System;
using System.IO;
using System.Text.Json;
using TransferWorker.Models;
using System.Windows;
using NetCore.Utils.Log;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace TransferWorker.Utility
{
    public class MainUtility
    {

        public Settings LoadSettings()
        {
            try
            {
                string localFilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
          + "\\logContent.json";
                //string localFilePath = Path.GetFullPath("logContent.json");

             
               var stream = File.Open(localFilePath, FileMode.Open, FileAccess.Write, FileShare.Read);
                if (stream != null)
                {
                    var str = File.ReadAllText(localFilePath);
                    var configs = JsonSerializer.Deserialize<AppJson>(str);
                    stream.Dispose();
                    stream.Close();
                    return configs.Settings;
                }
                stream.Dispose();
                stream.Close();

                //  string localFilePath = Path.GetFullPath("logContent.json");
                return null;
            }
            catch (Exception ex)
            {
                NLogManager.LogInfo("LoadSettings" + ex.ToString());
                return null;
              
            }
        }

        public async Task WriteLastRunTime(string fileName, string content, FolderConfig folder)
        {
            try
            {
                string localFilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
              + "\\" + fileName + ".json";
                var str = File.ReadAllText(localFilePath);
                var configs = JsonSerializer.Deserialize<AppJson>(str);
                var fl = configs.Settings.Folders.FirstOrDefault(x => x.Id == folder.Id);
                fl.LastRunTime = content;
                File.WriteAllText(localFilePath, JsonSerializer.Serialize(configs));
            }
            catch (Exception ex)
            {
                NLogManager.LogInfo("WriteLastRunTime  ==>  "+ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tittle">Tiêu đề</param>
        /// <param name="content">Nội dung</param>
        /// <param name="status">Trạng thái: 0: false  1: Success</param>
        public async Task AddLogContentAsync(string tittle, string nameFile,int status)
        {
            try
            {
                var appLog = new List<Task>();
                var t = Task.Run(async () =>
                {
                    string localFilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
               + "\\appsettings.json";
                    var str = File.ReadAllText(localFilePath);
                    var configs = JsonSerializer.Deserialize<AppJson>(str);
                    string sc = "Tác vụ: " + tittle + " sao lưu thành công lúc: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm tt") + " (" + nameFile + ")";
                    string fl = "Tác vụ: " + tittle + " sao lưu gặp lỗi: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm tt") + nameFile;
                    configs.Logs.LogContents.Add(new LogContent
                    {
                        Tittle = "Sao lưu",
                        Time = DateTime.Now,
                        TimeDisplay = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        Content = status == 1 ? sc : fl,
                        StatusSuccess = status == 1 ? "Visible" : "Hidden",
                        StatusFalse = status == 1 ? "Hidden" : "Visible"
                    });
                    File.WriteAllText(localFilePath, JsonSerializer.Serialize(configs));
                });
                appLog.Add(t);
                await Task.WhenAll(appLog);
            }
            catch (Exception ex)
            {
                NLogManager.LogInfo("AddLogContentAsync ==> "+ex);
            }
           
        }

        public void WriteLog(Logs _logs)
        {
            // string localFilePath = Path.GetFullPath("logContent.json");

            string localFilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
              + "\\logContent.json";
            var stream = File.Open(localFilePath, FileMode.Open, FileAccess.Write, FileShare.Read);
            if (stream != null)
            {
                var str = File.ReadAllText(localFilePath);
                var configs = JsonSerializer.Deserialize<AppJson>(str);
                stream.Dispose();
                File.WriteAllText(localFilePath, JsonSerializer.Serialize(configs));
                stream.Dispose();
            }
        }
    }
}
