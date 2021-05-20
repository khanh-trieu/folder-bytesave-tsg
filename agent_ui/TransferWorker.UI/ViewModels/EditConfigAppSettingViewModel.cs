using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Windows;
using TransferWorker.UI.Models;

namespace TransferWorker.UI.ViewModels
{
   public class EditConfigAppSettingViewModel :ViewModelBase
    {
        private int timer;
        private int idAppSetting;
        private string idAppSettingString;
        private string storageConnectionString;
        private string nameAppSetting;
        private string accountName;
        private bool is_display_connect_string;
        private int maxConcurrency;
        private connect_bytesave appSettings;
        
        public connect_bytesave AppSetting
        {
            get => appSettings;
            set => this.RaiseAndSetIfChanged(ref appSettings, value);
        }
        public int Timer
        {
            get => timer;
            set => this.RaiseAndSetIfChanged(ref timer, value);
        }
        public bool Is_display_connect_string
        {
            get => is_display_connect_string;
            set => this.RaiseAndSetIfChanged(ref is_display_connect_string, value);
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
                loadAccAsync();
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
        public string AccountName
        {
            get => accountName;
            set
            {
                this.RaiseAndSetIfChanged(ref accountName, value);
            }
        }

        private async System.Threading.Tasks.Task loadAccAsync()
        {
            if (StorageConnectionString != null)
            {
                try
                {
                    var connect = StorageConnectionString.Split(';');
                    var accname = connect[1].Split('=');
                    AccountName = accname[1];
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Incorrect connection string format!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        public EditConfigAppSettingViewModel(connect_bytesave app)
        {
            this.appSettings = app;
            NameAppSetting = app.name;
            Is_display_connect_string = app.type == 0 ? true : false;
            Ok = ReactiveCommand.Create(OkClick);
            Cancel = ReactiveCommand.Create(() => { });
        }
        public connect_bytesave OkClick()
        {
            if (string.IsNullOrWhiteSpace(NameAppSetting))
            {
                MessageBox.Show("Bạn chưa nhập tên kết nối", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            //if (string.IsNullOrWhiteSpace(StorageConnectionString))
            //{
            //    MessageBox.Show("Bạn chưa nhập chuỗi kết nối", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return null;
            //}
            return new connect_bytesave
            {
                //metric_service_username_account = AccountName,
                name = NameAppSetting,
                time_update_at = int.Parse(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
            };
        }
        public ReactiveCommand<Unit, connect_bytesave> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

    }
}
