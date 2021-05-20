using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NetCore.Utils.Log;
using ReactiveUI;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;
namespace TransferWorker.UI.ViewModels
{
    public class ConfigMenuBackupViewModel : ViewModelBase
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
            get => pathh;
            set => this.RaiseAndSetIfChanged(ref pathh, value);
        }
        private string jobNameMaxLenghth;
        public string JobNameMaxLenghth
        {
            get => jobNameMaxLenghth;
            set => this.RaiseAndSetIfChanged(ref jobNameMaxLenghth, value);
        }

        //public ObservableCollection<FolderConfig> Items { get; }
        public ObservableCollection<backup_bytesave> Items { get; }
        public ConfigMenuBackupViewModel()
        {
            Items = new ObservableCollection<backup_bytesave>();
            Title = "Sao lưu";
            Detail = ReactiveCommand.Create<backup_bytesave, backup_bytesave>(DetailItem);

            Delete = ReactiveCommand.CreateFromTask<backup_bytesave, backup_bytesave>(DeleteItem);
        }
       
        public ConfigMenuBackupViewModel(List<backup_bytesave> configs)
        {
            if(configs == null)
            {
                Items = new ObservableCollection<backup_bytesave>();
                return;
            }
            IsEnable = true;
            var okEnabled = this.WhenAnyValue(
                       x => x.IsEnable,
                       x => x == true);
            Items = new ObservableCollection<backup_bytesave>(configs.OrderBy(x=>x.id));

            Title = "Sao lưu";
            Detail = ReactiveCommand.Create<backup_bytesave, backup_bytesave>(DetailItem, okEnabled);

            Delete = ReactiveCommand.CreateFromTask<backup_bytesave, backup_bytesave>(DeleteItem);
        }

        private async Task<backup_bytesave> DeleteItem(backup_bytesave item)
        {
            IsEnable = false;
            string message = "Bạn có chắc muốn xóa thư mục sao lưu : " + item.name + " không ?";
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
            // Code for executing the command here.
        }
        private backup_bytesave DetailItem(backup_bytesave item)
        {
            return item;
            // Code for executing the command here.
        }
        //private AppSetting TestItem(ComboModel item)
        //{
        //    var setting = new MainUtility().LoadConfig();
        //    var itemm = setting.Settings.AppSettings.FirstOrDefault(x => x.IdAppSetting == item.Id);

        //    return itemm;
        //}
        //private AppSetting EditAppItem(ComboModel item)
        //{
        //    var setting = new MainUtility().LoadConfig();
        //    var itemm = setting.Settings.AppSettings.FirstOrDefault(x => x.IdAppSetting == item.Id);

        //    return itemm;
        //}


        public ReactiveCommand<backup_bytesave, backup_bytesave> Delete { get; }
        public ReactiveCommand<backup_bytesave, backup_bytesave> Edit { get; }
        public ReactiveCommand<backup_bytesave, backup_bytesave> Detail { get; }
    }
}
