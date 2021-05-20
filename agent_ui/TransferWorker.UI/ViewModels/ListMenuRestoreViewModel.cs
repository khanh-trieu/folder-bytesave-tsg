using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCore.Utils.Extensions;
using NetCore.Utils.Log;
using ReactiveUI;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;

namespace TransferWorker.UI.ViewModels
{
   public class ListMenuRestoreViewModel : ViewModelBase
    {
        private List<connect_bytesave> _settings;
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

        public class MenuItem
        {
            public MenuItem()
            {
                this.Items = new ObservableCollection<FolderConfig>();
            }

            public string Title { get; set; }

            public ObservableCollection<FolderConfig> Items { get; set; }
        }
      
        private List<KetNoi> departments;
        public List<KetNoi> Departments
        {
            get
            {
                return departments;
            }
            set
            {
                departments = value;
                
            }
        }
        //public ObservableCollection<FolderConfig> Items { get; }
        public ObservableCollection<FolderConfig> Items { get; }
        public List<connect_bytesave> _connects { get; }
        public List<backup_bytesave> _backups { get; }
        public ListMenuRestoreViewModel(List<connect_bytesave> setting, List<backup_bytesave> folders)
        {
            _backups = folders;
            IsEnable = true;
            var okEnabled = this.WhenAnyValue(
                       x => x.IsEnable,
                       x => x == true);
            _settings = setting;
            List<KetNoi> LstKetNoi = new List<KetNoi>();
            foreach (var item in setting)
            {
                KetNoi ketNoi = new KetNoi() { NameAppsetting = item.name,IdAppsetting = item.id };

                var lstfolder = folders.Where(x=>x.id_connect_bytesave == ketNoi.IdAppsetting);
                foreach (var itemFolder in lstfolder)
                {
                    if (ketNoi.LstContainer.Count == 0 || ketNoi.LstContainer.FirstOrDefault(x=>x.NameContainer == itemFolder.container_name) == null)
                    {
                        ketNoi.LstContainer.Add(new Container() { NameContainer = itemFolder.container_name, Id = itemFolder.id });
                    }
                }
                LstKetNoi.Add(ketNoi);
            }   
            Departments = LstKetNoi;
            Detail = ReactiveCommand.Create<Container, backup_bytesave>(DetailItem, okEnabled);
        }
      
        private backup_bytesave DetailItem(Container item)
        {
            return _backups.FirstOrDefault(x=>x.id == item.Id);
        }

        public ReactiveCommand<Container, backup_bytesave> Detail { get; }
    }
}
