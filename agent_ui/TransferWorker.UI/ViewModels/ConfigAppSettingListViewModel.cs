using Microsoft.WindowsAzure.Storage;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;

namespace TransferWorker.UI.ViewModels
{
    public class ConfigAppSettingListViewModel : ViewModelBase
    {

        private int timer;
        private int idAppSetting;
        private string idAppSettingString;
        private string storageConnectionString;
        private string nameAppSetting;
        private string accountName;
        private string lastCheck;
        private string img;
        private string status;
        private int maxConcurrency;
        private AppSetting appSettings;

        public AppSetting AppSetting
        {
            get => appSettings;
            set => this.RaiseAndSetIfChanged(ref appSettings, value);
        }
        public int Timer
        {
            get => timer;
            set => this.RaiseAndSetIfChanged(ref timer, value);
        }
        public int IdAppSetting
        {
            get => idAppSetting;
            set => this.RaiseAndSetIfChanged(ref idAppSetting, value);
        }
        public string IdAppSettingString
        {
            get => idAppSettingString;
            set => this.RaiseAndSetIfChanged(ref idAppSettingString, value);
        }
        public string Img
        {
            get => img;
            set => this.RaiseAndSetIfChanged(ref img, value);
        }
        public string DisplayedImage
        {
            get { return @"D:\Code\Transfer_New\transferworker\TransferWorker.UI\Assets\delete.png"; }
        }
        public string AccountName
        {
            get => accountName;
            set => this.RaiseAndSetIfChanged(ref accountName, value);
        }
        public string Status
        {
            get => status;
            set => this.RaiseAndSetIfChanged(ref status, value);
        }
        public string LastCheck
        {
            get => lastCheck;
            set => this.RaiseAndSetIfChanged(ref lastCheck, value);
        }

        public int MaxConcurrency
        {
            get => maxConcurrency;
            set => this.RaiseAndSetIfChanged(ref maxConcurrency, value);
        }

        public string StorageConnectionString
        {
            get => storageConnectionString;
            set
            {
                this.RaiseAndSetIfChanged(ref storageConnectionString, value);
                
            }
        }
        public string NameAppSetting
        {
            get => nameAppSetting;
            set
            {
                this.RaiseAndSetIfChanged(ref nameAppSetting, value);
            }
        }
        private bool isEnable;
        public bool IsEnable
        {
            get => isEnable;
            set => this.RaiseAndSetIfChanged(ref isEnable, value);
        }
        private BitmapSource _bitmapSource;

        public BitmapSource ButtonSource
        {
            get { return _bitmapSource; }
        }

        public ObservableCollection<AppSetting> Items { get; }

        public ConfigAppSettingListViewModel(List<AppSetting> configs)
        {
            IsEnable = true;
            var okEnabled = this.WhenAnyValue(
                       x => x.IsEnable,
                       x => x == true);
            Items = new ObservableCollection<AppSetting>(configs);
           //Delete = ReactiveCommand.Create(RunTheThing);
           Delete = ReactiveCommand.CreateFromTask<AppSetting, AppSetting>(DeleteItem);
            Edit = ReactiveCommand.Create<ComboModel, AppSetting>(EditItem, okEnabled);
            Test = ReactiveCommand.Create<AppSetting, AppSetting>(TestItem, okEnabled);
       
        }
        public ConfigAppSettingListViewModel(AppSetting configs)
        {
            appSettings = configs;
            IsEnable = true;
            var okEnabled = this.WhenAnyValue(
                       x => x.IsEnable,
                       x => x == true);
            nameAppSetting = configs.NameAppSetting;
            accountName = configs.AccountName;
            storageConnectionString = configs.StorageConnectionString;
            lastCheck = configs.LastCheck;
            //Test chuỗi kết nối
            CloudStorageAccount storageAccount;

            if (CloudStorageAccount.TryParse(configs.StorageConnectionString, out storageAccount))
            {
                Status = "Success!";
            }
            else
            {
                Status = "False!";
            }

            long size = 0;
            //var list = container.ListBlobs();
            //foreach (CloudBlockBlob blob in list)
            //{
            //    size += blob.Properties.Length;
            //}


            img = "/Assets/icon.png";
            //Bitmap bitmap = (Bitmap)Bitmap.FromFile(@"D:\Code\Transfer_New\transferworker\TransferWorker.UI\Assets\delete.png", true);
            //_bitmapSource = BitmapConversion.BitmapToBitmapSource(bitmap);
            //Delete = ReactiveCommand.Create(RunTheThing);
            Delete = ReactiveCommand.CreateFromTask<AppSetting, AppSetting>(DeleteItem);
            Edit = ReactiveCommand.Create<ComboModel, AppSetting>(EditItem, okEnabled);
            Test = ReactiveCommand.Create<AppSetting, AppSetting>(TestItem, okEnabled);
            Detail = ReactiveCommand.Create<ComboModel, ComboModel>(DetailItem, okEnabled);

        }
        public static class BitmapConversion
        {
            public static BitmapSource BitmapToBitmapSource(Bitmap source)
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                              source.GetHbitmap(),
                              IntPtr.Zero,
                              Int32Rect.Empty,
                              BitmapSizeOptions.FromEmptyOptions());
            }
        }
        private AppSetting EditItem(ComboModel item)
        {
            var setting = new MainUtility().LoadConfig().AppSettings;
            var app = setting.FirstOrDefault(x => x.IdAppSetting == item.Id);
            return app;
            // Code for executing the command here.
        }
        private ComboModel DetailItem(ComboModel item)
        {
            return item;
            // Code for executing the command here.
        }
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
        private async Task<AppSetting> DeleteItem(AppSetting item)
        {
            IsEnable = false;
            //var msBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            //{
            //    ButtonDefinitions = ButtonEnum.YesNo,
            //    ContentTitle = "Thông báo",
            //    ContentMessage = "Bạn có muốn xóa tài khoản "+item.NameAppSetting+"  không?",
            //    Icon = MessageBox.Avalonia.Enums.Icon.Plus,
            //    //Style = Style.UbuntuLinux,
            //    //WindowStartupLocation = WindowStartupLocation.CenterScreen
            //});
            //var result = await msBoxStandardWindow.Show();
            //if (result == ButtonResult.No)
            //{
            //    IsEnable = true;
            //    return null;
            //}
            IsEnable = true;
            return item;
            // Code for executing the command here.
        }


     
        public ReactiveCommand<AppSetting, AppSetting> Delete { get; }
        public ReactiveCommand<ComboModel, AppSetting> Edit { get; }
        public ReactiveCommand<AppSetting, AppSetting> Test { get; }
        public ReactiveCommand<ComboModel, ComboModel> Detail { get; }
    }
}
