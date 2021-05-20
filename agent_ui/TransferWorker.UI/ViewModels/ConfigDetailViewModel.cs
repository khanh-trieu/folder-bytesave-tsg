using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TransferWorker.UI.Helpers;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;

namespace TransferWorker.UI.ViewModels
{
    public class ConfigDetailViewModel : ViewModelBase
    {
        private int timer;
        private int idAppSetting;
        private string idAppSettingString;
        private string storageConnectionString;
        private string nameAppSetting;
        private string max_used;
        private string type_connect;
        private string accountName;
        private string lastCheck;
        private string img;
        private string ttt;
        private string status;
        private int maxConcurrency;
        private AppSetting appSetting;
        private decimal leghtSize;
        public AppSetting AppSetting
        {
            get => appSetting;
            set
            {
                this.RaiseAndSetIfChanged(ref appSetting, value);
            }
        }
        public string Type_connect
        {
            get => type_connect;
            set { this.RaiseAndSetIfChanged(ref type_connect, value); }
        }
        public int Timer
        {
            get => timer;
            set { this.RaiseAndSetIfChanged(ref timer, value); }
        }
        public int IdAppSetting
        {
            get => idAppSetting;
            set { this.RaiseAndSetIfChanged(ref timer, value); }
        }
        public string IdAppSettingString
        {
            get => idAppSettingString;
            set { this.RaiseAndSetIfChanged(ref idAppSettingString, value); }
        }
        public string Ttt
        {
            get => ttt;
            set { this.RaiseAndSetIfChanged(ref ttt, value); }
        }
        public string Img
        {
            get => img;
            set { this.RaiseAndSetIfChanged(ref img, value); }
        }
        public string DisplayedImage
        {
            //   get { return "/TransferWorker.UI;/Assets/delete.png"; }
            get { return @"../../Assets/settings.png"; }
        }
        public string Max_used
        {
            get => max_used;
            set { this.RaiseAndSetIfChanged(ref max_used, value); }
        }
        public string AccountName
        {
            get => accountName;
            set { this.RaiseAndSetIfChanged(ref accountName, value); }
        }
        public string Status
        {
            get => status;
            set { this.RaiseAndSetIfChanged(ref status, value); }
        }
        public string LastCheck
        {
            get => lastCheck;
            set { this.RaiseAndSetIfChanged(ref lastCheck, value); }
        }
        private string isStatusTrue;
        public string IsStatusTrue
        {
            get => isStatusTrue;
            set { this.RaiseAndSetIfChanged(ref isStatusTrue, value); }
        }
        private string isStatusFalse;
        public string IsStatusFalse
        {
            get => isStatusFalse;
            set { this.RaiseAndSetIfChanged(ref isStatusFalse, value); }
        }

        public int MaxConcurrency
        {
            get => maxConcurrency;
            set { this.RaiseAndSetIfChanged(ref maxConcurrency, value); }
        }

        public string StorageConnectionString
        {
            get => storageConnectionString;
            set { this.RaiseAndSetIfChanged(ref storageConnectionString, value); }
        }
        public string NameAppSetting
        {
            get => nameAppSetting;
            set { this.RaiseAndSetIfChanged(ref nameAppSetting, value); }
        }
        private bool isEnable;
        public bool IsEnable
        {
            get => isEnable;
            set { this.RaiseAndSetIfChanged(ref isEnable, value); }
        }
        private string useLevel;
        public string UseLevel
        {
            get => useLevel;
            set { this.RaiseAndSetIfChanged(ref useLevel, value); }
        }
        private string isHave;

        public string IsHave
        {
            get => isHave;
            set => this.RaiseAndSetIfChanged(ref isHave, value);
        }
        private string isNoHave;

        public string IsNoHave
        {
            get => isNoHave;
            set => this.RaiseAndSetIfChanged(ref isNoHave, value);
        }
        public ObservableCollection<AppSetting> Items { get; }

        //public ConfigDetailViewModel(List<AppSetting> configs)
        //{
        //    IsEnable = true;
        //    var okEnabled = this.WhenAnyValue(
        //               x => x.IsEnable,
        //               x => x == true);
        //    Items = new ObservableCollection<AppSetting>(configs);
        //}
        public ConfigDetailViewModel()
        {

        }
        public ConfigDetailViewModel(connect_bytesave appSetting)
        {
            IsHave = "Visible";
            IsNoHave = "Hidden";
            if (appSetting == null)
            {
                IsHave = "Hidden";
                IsNoHave = "Visible";
                return;
            }
            CloudStorageAccount storageAccount;

            //   appSetting = appSetting;
            IsEnable = true;
            var okEnabled = this.WhenAnyValue(
                       x => x.IsEnable,
                       x => x == true);
            Type_connect = appSetting.type == 0 ? "Kết nối được tạo trên phần mềm ByteSave" : "Kết nối được tạo trên Website";
            Max_used = appSetting.metric_service_max_storage + " GB";
            NameAppSetting = appSetting.name;
            AccountName = appSetting.metric_service_username_account;
            StorageConnectionString = appSetting.metric_service_information_connect;
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(appSetting.time_check_at).ToLocalTime();
            LastCheck = dt.ToString("dd/MM/yyyy hh:MM:ss tt");
            GetUseLevelAsync(appSetting.metric_service_information_connect);
            CloudStorageAccount.TryParse(appSetting.metric_service_information_connect, out storageAccount);
            if (storageAccount != null)
            {
                IsStatusTrue = "Visible";
                IsStatusFalse = "Hidden";
            }
            else
            {
                IsStatusTrue = "Hidden";
                IsStatusFalse = "Visible";
            }
            //long size = 0;
            Img = "../../../Assets/settings.png";

        }
        public async Task GetlenghtAsync()
        {
            await new MainUtility().GetUseLevelAsync(appSetting.StorageConnectionString);
        }

        //private AppSetting EditItem(ComboModel item)
        //{
        //    var setting = new MainUtility().LoadConfig().Settings.AppSettings;
        //    var app = setting.FirstOrDefault(x => x.IdAppSetting == item.Id);
        //    return app;
        //    // Code for executing the command here.
        //}
        //private AppSetting DetailItem(ComboModel item)
        //{
        //    var setting = new MainUtility().LoadConfig();
        //    var itemm = setting.Settings.AppSettings.FirstOrDefault(x => x.IdAppSetting == item.Id);
        //    return itemm;
        //    // Code for executing the command here.
        //}
        private AppSetting TestItem(AppSetting item)
        {
            return item;
            // Code for executing the command here.
        }
        public void EditAppsetting()
        {
            //new MainWindowViewModel().EditConfigView(appSettings);
            return;
        }

        public async Task<long> GetListFileFromCloud(string storageConnectionString)
        {
            try
            {
                //var cloudFiles = new List<SizeFile>();
                var containers = await AzureBlobHelper.ListContainersAsync(storageConnectionString);
                long number = 0;
                foreach (var item in containers)
                {
                    var blobContainer = GetBlobContainerV2(item.Name, storageConnectionString).GetBlobsAsync();
                    await foreach (var blobItem in blobContainer)
                    {
                        number += blobItem.Properties.ContentLength.Value;
                        //var fsi = new SizeFile
                        //{
                        //    Lenght = blobItem.Properties.ContentLength
                        //};
                        //cloudFiles.Add(fsi);
                    }
                }
                return number;
            }
            catch (Exception)
            {
                return 0;
            }
            
        }
        private BlobContainerClient GetBlobContainerV2(string nameContainer, string storageConnectionString)
        {
            var container = new BlobContainerClient(storageConnectionString, nameContainer);
            return container;
        }
        public async Task GetUseLevelAsync(string storageConnectionString)
        {
            try
            {
                var number = await GetListFileFromCloud(storageConnectionString);
                double cum = double.Parse(number.ToString());
                var a =  cum * (0.000000000931);  //chuyển sang MB
                var b =  cum * (0.000000954); // chuyển sang GB
                if (cum == 0) UseLevel = "0 GB";
                else UseLevel = a > 1? String.Format("{0:#.##} GB", a) : String.Format("{0:#.#} MB",b);
            }
            catch (Exception)
            {

            }

        }


    }
}
