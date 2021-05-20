using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using NetCore.Utils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TransferWorker.Helpers;
using TransferWorker.Models;
using TransferWorker.Utility;

namespace TransferWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptionsMonitor<Settings> _options;
        private int timerRun = 5;    //phút ==> ban đầu = 5 , cài lại = 60 để bug
        private string messageRunLate = "";
        public Worker(ILogger<Worker> logger, IOptionsMonitor<Settings> options)
        {
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    //if (await IsAllow())
                    //{
                    var tasks = new List<Task>();

                    foreach (var folder in _options.CurrentValue.Folders)
                    {
                        if (await IsAllow(folder))
                        {
                            var t = Task.Run(async () =>
                            {
                                try
                                {
                                    var appsetting = _options.CurrentValue.AppSettings.FirstOrDefault(x => x.IdAppSetting == folder.IdAppSetting);
                                    IAzureBlobHelper _azureBlobHelper = new AzureBlobHelper(appsetting, folder);
                                    await _azureBlobHelper.Compare();
                                }
                                catch (Exception e)
                                {
                                    NLogManager.PublishException(e);
                                }
                            });

                            tasks.Add(t);

                            await Task.WhenAll(tasks);
                            //await WriteLog("LastRunTime", DateTime.Now.ToString());

                            //}
                            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                            await new MainUtility().WriteLastRunTime("appsettings", DateTime.Now.ToString(), folder);
                            //await Task.Delay( 10000, stoppingToken);
                        }
                    }
                    // sau timerRun phút sẽ chạy lại 1 lần
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            //new TransferWorker.Utility.MainUtility().Write("appsettings", DateTime.Now.ToString());
            //new TransferWorker.Utility.MainUtility().LoadLog();
            //new TransferWorker.Utility.SendMail().kk();
            // var con = _appSettings.StorageConnectionString;
            // await _azureBlobHelper.UploadFile();

           
            //Authenticate and create a data factory management client
            //var context = new AuthenticationContext("https://login.windows.net/" + tenantID);
            //ClientCredential cc = new ClientCredential(applicationId, authenticationKey);
            //AuthenticationResult result = context.AcquireTokenAsync("https://management.azure.com/", cc).Result;
            //ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);
            //var client = new DataFactoryManagementClient(cred) { SubscriptionId = subscriptionId };
        }

        private async Task<bool> IsAllow(FolderConfig folder)
         {
           // return true;
            //if (GetIsRunNow("appsettings") == true)
            //{
            //    return false; // return đây để run worker  Warring
            //}

            var strTimer = folder.TimerString;
            var timerArray = strTimer.Split('|');
            var lastTimeRun = folder.LastRunTime;
            //if (Convert.ToInt32(timerArray[0]) == 1)  // hàng ngay
            //{
            //    var currentHour = DateTime.Now.Hour;
            //    var currentMinute = DateTime.Now.Minute;
            //    if (Convert.ToInt32(timerArray[2]) == 3) //theo nhiều giờ cố định
            //    {
            //        var listHour = timerArray[3].Split(',');
            //        foreach (var item in listHour)
            //        {
            //            if (currentHour == int.Parse(item) && currentMinute < 60)  
            //            {
            //                return true;
            //            }
            //        }
            //    }
            //    // xác định mốc giờ cố định?
            //    if (Convert.ToInt32(timerArray[2]) == 1)  // theo mốc cố định
            //    {
            //        var hour = Convert.ToInt32(timerArray[3]) % 24;
            //        if (currentHour == hour && currentMinute < timerRun) 
            //        {
            //            return true;
            //        }

            //    }
            //    if(Convert.ToInt32(timerArray[2]) == 2)
            //    {
            //        var hour = Convert.ToInt32(timerArray[3]) % 24;
            //        if (lastTimeRun.Hour + hour == currentHour && currentMinute < timerRun)
            //        {
            //            return true;
            //        }
            //    }
            //}
            //else  //theo tuan
            //{
            if (!string.IsNullOrEmpty(timerArray[1]) && folder.Id > 0)
            {
               //return true;

                var dayOfWeekArray = timerArray[1].Split(',').Select(s => Convert.ToInt32(s)).ToList();
                var dayOfWeek = (int)DateTime.Now.DayOfWeek;
                //xem còn mấy ngày đến lượt chạy tiếp
                var day = 7;
                for (int i = 0; i < dayOfWeekArray.Count(); i++)
                {
                    var dayTemp = 0;
                    if (dayOfWeekArray[i] > dayOfWeek)
                    {
                        dayTemp = dayOfWeekArray[i] - dayOfWeek;
                    }
                    else
                    {
                        dayTemp = 7 - (dayOfWeek - dayOfWeekArray[i]);
                    }
                    if (dayTemp > 0 && dayTemp < day)
                    {
                        day = dayTemp;
                    }
                }

                var currentHour = DateTime.Now;
                var currentMinute = DateTime.Now.Minute;
                if (Convert.ToInt32(timerArray[2]) == 3) //theo nhiều giờ cố định
                {
                    if (dayOfWeekArray.Contains(dayOfWeek))
                    {
                        var listHour = timerArray[3].Split(',');
                        foreach (var item in listHour)
                        {
                            var Time = DateTime.Parse(item);
                            if (currentHour.Hour == Time.Hour && currentHour.Minute == Time.Minute)
                            {
                                return true;
                            }
                        }
                    }
                }
                // xác định lặp lại sau bao lâu
                if (Convert.ToInt32(timerArray[2]) == 2)
                {
                    var hour = Convert.ToInt32(timerArray[3]) % 24;
                    //hour sẽ là sau bao nhiêu giờ chạy tiếp

                    var lastRun = DateTime.Parse(folder.LastRunTime == "" ? folder.CreateTime : folder.LastRunTime);
                    if (lastRun.AddHours(hour).Hour == currentHour.Hour)
                    {
                        return true;
                    }
                }
                //}
            }

            return false;
        }

        //private async Task WriteLog(string fileName, string content)
        //{
        //    try
        //    {
        //        //if (!Directory.Exists("Data"))
        //        //{
        //        //    Directory.CreateDirectory("Data");
        //        //}
        //        //string localFilePath = Path.Combine(fileName + ".txt");
        //        //await File.WriteAllTextAsync(localFilePath, content);
        //        //string localFilePath = Path.GetFullPath(fileName + ".txt");
        //        //File.WriteAllText(fileName + ".txt", content);

        //        string localFilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
        //      + "\\"+fileName + ".json";

        //        //   string localFilePath = Path.GetFullPath(fileName + ".json");
        //        var str = File.ReadAllText(localFilePath);
        //        var configs = JsonSerializer.Deserialize<AppJson>(str);
        //        configs.LastRunTime = content;
        //        File.WriteAllText(localFilePath, JsonSerializer.Serialize(configs));
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //    }
        //}


        //private async Task<DateTime> GetLog(string fileName)
        //{
        //    try
        //    {
        //        //string localFilePath = Path.Combine( fileName + ".txt");
        //        //content = await File.ReadAllTextAsync(localFilePath);
        //        //string localFilePath = Path.GetFullPath(fileName + ".txt");
        //        //content = File.ReadAllText(localFilePath);
        //        //return DateTime.Parse(content);
        //        //  string localFilePath = Path.GetFullPath(fileName + ".json");
        //        string localFilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
        //       + "\\" + fileName+ ".json";
        //        var str = File.ReadAllText(localFilePath);
        //        var configs = JsonSerializer.Deserialize<AppJson>(str);
        //        return DateTime.Parse(configs.LastRunTime);
        //    }
        //    catch (Exception ex)
        //    {
        //        await WriteLog("RunningTime", DateTime.Now.ToString());
        //    }
        //    return DateTime.Now;
        //}
       
    }
}