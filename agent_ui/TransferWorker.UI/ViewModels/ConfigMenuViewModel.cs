using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAzure.Storage;
using NetCore.Utils.Log;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;

namespace TransferWorker.UI.ViewModels
{
    public class ConfigMenuViewModel : ViewModelBase
    {
        private bool isEnable;

        public bool IsEnable
        {
            get => isEnable;
            set => this.RaiseAndSetIfChanged(ref isEnable, value);
        }
        private string title;
        public string Title
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, value);
        }
        private string pathh;
        public string Pathh
        {
            get { return "/AssemblyName;component/Images/ImageName.jpg"; }
        }
        public string DisplayedImage
        {
            get { return "/AssemblyName;component/Images/ImageName.jpg"; }
        }
        private string imgAdd;
        public string ImgAdd
        {
            get { return "/TransferWorker.UI;Assets/plus.png"; }
        }
        private string checkConnect;
        public string CheckConnect
        {
            get => checkConnect;
            set => this.RaiseAndSetIfChanged(ref checkConnect, value);
        }

        //public ObservableCollection<FolderConfig> Items { get; }
        public ObservableCollection<connect_bytesave> Items { get; }
        public ConfigMenuViewModel(List<connect_bytesave> appSettings)
        {
            if(appSettings == null)
            {
                Items = new ObservableCollection<connect_bytesave>();
            }

            NLogManager.LogInfo("ConfigMenuViewModel  " + appSettings.ToList());
            IsEnable = true;
            var okEnabled = this.WhenAnyValue(
                       x => x.IsEnable,
                       x => x == true);
            Items = new ObservableCollection<connect_bytesave>(appSettings.OrderBy(x => x.id));

            Title = "Kết nối";
            Detail = ReactiveCommand.Create<connect_bytesave, connect_bytesave>(DetailItem, okEnabled);

            Delete = ReactiveCommand.Create<connect_bytesave, connect_bytesave>(DeleteItemApp, okEnabled);
            Test = ReactiveCommand.Create<connect_bytesave, connect_bytesave>(TestItem, okEnabled);
        }
       
        private connect_bytesave DeleteItemApp(connect_bytesave item)
        {
            IsEnable = false;
            string message = "Bạn có chắc muốn xóa kết nối: " + item.name + " không ?";
            string caption = "Thông báo";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.No)
            {
                IsEnable = true;
                return null;
            }
            IsEnable = true;
            return item;
        }
        private connect_bytesave DetailItem(connect_bytesave item)
        {
            return item;
            // Code for executing the command here.
        }
        private FolderConfig EditItem(FolderConfig item)
        {
            return item;
            // Code for executing the command here.
        }
        private connect_bytesave TestItem(connect_bytesave item)
        {
            return item;
        }


        public ReactiveCommand<connect_bytesave, connect_bytesave> Delete { get; }
        public ReactiveCommand<connect_bytesave, connect_bytesave> Detail { get; }
        public ReactiveCommand<connect_bytesave, connect_bytesave> Test { get; }
        public ReactiveCommand<connect_bytesave, connect_bytesave> EditApp { get; }
    }
}
