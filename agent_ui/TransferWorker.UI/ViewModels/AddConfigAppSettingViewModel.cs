using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using TransferWorker.UI.Utility;
using TransferWorker.UI.Models;
using System.Reactive;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using System.Resources;
using System.Globalization;
using System.Windows.Markup;
using System.Net;
using System.IO;
using TransferWorker.UI.Views;
using Newtonsoft.Json.Linq;

namespace TransferWorker.UI.ViewModels
{
    public class AddConfigAppSettingViewModel : ViewModelBase
    {
        private string idAppSettingString;
        private string storageConnectionString;
        private string nameAppSetting;
        private string accountName;
        private Settings _settings;
        public string IdAppSettingString
        {
            get => idAppSettingString;
            set => this.RaiseAndSetIfChanged(ref idAppSettingString, value);
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
            set =>this.RaiseAndSetIfChanged(ref nameAppSetting, value);
        }
        public string AccountName
        {
            get => accountName;
            set => this.RaiseAndSetIfChanged(ref accountName, value);
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
                    MessageBox.Show("Chuỗi kết nối sai định dạng!","Thông báo",MessageBoxButton.OK,MessageBoxImage.Information);
                }
            }
        }
        public ICommand ClickSave {
            get;
            private set;
        }
        public AddConfigAppSettingViewModel()
        {
            //var okEnabled = this.WhenAnyValue(
            //          x => x.StorageConnectionString,
            //          x => x.NameAppSetting,
            //          (x,y) => !string.IsNullOrWhiteSpace(x)&&!string.IsNullOrWhiteSpace(y));

            Ok = ReactiveCommand.Create(OkClick
                   );
            Cancel = ReactiveCommand.Create(() => { });
        }
        public connect_bytesave OkClick()
        {
            if (string.IsNullOrWhiteSpace(NameAppSetting))
            {
                //string getString = (string)Application.Current.Resources["NotInputName"];
                //string localizedMessage = (string)Application.Current.FindResource("NotInputName");
               
                //new ShowMessages("Bạn cần nhập đủ thông tin: </br> Tên kết nối, chuỗi kết nối" ,"My App", MessageBoxButton.OK, MessageBoxImage.Information);
                MessageBox.Show("Bạn chưa nhập tên kết nối", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            if (string.IsNullOrWhiteSpace(storageConnectionString))
            {
                //new ShowMessages("Bạn cần nhập đủ thông tin: </br> Tên kết nối, chuỗi kết nối" ,"My App", MessageBoxButton.OK, MessageBoxImage.Information);
                 MessageBox.Show("Bạn chưa nhập chuỗi kết nối", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            var check = new MainUtility().check_connect(NameAppSetting);

            //var _settings = new TransferWorker.UI.Utility.MainUtility().LoadConfig();
            //var check = _settings.Settings.AppSettings.FirstOrDefault(x => x.AccountName == AccountName || x.NameAppSetting == NameAppSetting);


            if (check == false)
            {
                return null;
            }
            return new connect_bytesave
            {
                metric_service_username_account = AccountName,
                name = NameAppSetting,
                metric_service_information_connect = StorageConnectionString,
                metric_service_max_storage = "1000",
                //id_metric_service= System.Configuration.ConfigurationSettings.AppSettings["id_service"],
        };
        }
        
        public ReactiveCommand<Unit, connect_bytesave> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReactiveCommand<connect_bytesave, connect_bytesave> Test { get; }
    }
}
