using DynamicData;
using Nito.AsyncEx;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TransferWorker.UI.Helpers;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;

namespace TransferWorker.UI.ViewModels
{
    public class EditConfigBackupViewModel : ViewModelBase
    {
        private string localFolderPath;
        private string containerName;
        private int deleteTimer;
        private int coolTier;
        private int archiveTier;
        private backup_bytesave folder;


        private int idAppSetting;
        private int timer;
        private string storageConnectionString;
        private int maxConcurrency;
        public backup_bytesave Folder
        {
            get => folder;
            set => this.RaiseAndSetIfChanged(ref folder, value);
        }
        public INotifyTaskCompletion _initializationNotifier;

        public INotifyTaskCompletion InitializationNotifier
        {
            get => _initializationNotifier;
            set
            {
                this.RaiseAndSetIfChanged(ref _initializationNotifier, value);
            }
        }
        public string LocalFolderPath
        {
            get => localFolderPath;
            set => this.RaiseAndSetIfChanged(ref localFolderPath, value);
        }

        private int time_delete_file_in_lastversion;
        public int Time_delete_file_in_lastversion
        {
            get => time_delete_file_in_lastversion;
            set => this.RaiseAndSetIfChanged(ref time_delete_file_in_lastversion, value);
        }

        private bool isFolder;

        public bool IsFolder
        {
            get => isFolder;
            set => this.RaiseAndSetIfChanged(ref isFolder, value);
        }
        public string ContainerName
        {
            get => containerName;
            set => this.RaiseAndSetIfChanged(ref containerName, value);
        }

        public int DeleteTimer
        {
            get => deleteTimer;
            set => this.RaiseAndSetIfChanged(ref deleteTimer, value);
        }

        public int ArchiveTier
        {
            get => archiveTier;
            set => this.RaiseAndSetIfChanged(ref archiveTier, value);
        }

        public int CoolTier
        {
            get => coolTier;
            set => this.RaiseAndSetIfChanged(ref coolTier, value);
        }
        private string jobName;
        public string JobName
        {
            get => jobName;
            set => this.RaiseAndSetIfChanged(ref jobName, value);
        }

        private string nameAcc;
        public string NameAcc
        {
            get => nameAcc;
            set => this.RaiseAndSetIfChanged(ref nameAcc, value);
        }
        private ComboContainerModel containerNa;
        public ComboContainerModel ContainerNa
        {
            get => containerNa;
            set => this.RaiseAndSetIfChanged(ref containerNa, value);
        }
        private string textTime;
        public string TextTime
        {
            get => textTime;
            set => this.RaiseAndSetIfChanged(ref textTime, value);
        }
        private ComboModel oneNum;

        public ComboModel OneNum
        {
            get => oneNum;
            set => this.RaiseAndSetIfChanged(ref oneNum, value);
        }
        private string email;

        public string Email
        {
            get => email;
            set => this.RaiseAndSetIfChanged(ref email, value);
        }
        private ComboModel multiNum;

        public ComboModel MultiNum
        {
            get => multiNum;
            set => this.RaiseAndSetIfChanged(ref multiNum, value);
        }

        private bool isWeekly;

        public bool IsWeekly
        {
            get => isWeekly;
            set => this.RaiseAndSetIfChanged(ref isWeekly, value);
        }

        private bool isOnce = true;

        public bool IsOnce
        {
            get => isOnce;
            set => this.RaiseAndSetIfChanged(ref isOnce, value);
        }

        private bool isMulti = true;

        public bool IsMulti
        {
            get => isMulti;
            set => this.RaiseAndSetIfChanged(ref isMulti, value);
        }
        private string timeSelect;
        public string TimeSelect
        {
            get => timeSelect;
            set => this.RaiseAndSetIfChanged(ref timeSelect, value);
        }
        private ComboModel daily;

        public ComboModel Daily
        {
            get => daily;
            set
            {
                this.RaiseAndSetIfChanged(ref daily, value);
                switch (daily.Id)
                {
                    case 1:
                        IsWeekly = false;
                        break;

                    case 2:
                        IsWeekly = true;
                        break;

                    default:
                        break;
                }
            }
        }
        private ObservableCollection<ComboModel> itemsEmail = new ObservableCollection<ComboModel>();

        public ObservableCollection<ComboModel> ItemsEmail
        {
            get { return itemsEmail; }
            set
            {
                this.RaiseAndSetIfChanged(ref itemsEmail, value);
            }
        }
        private List<ComboModel> listEmails;
        private ComboModel nameApp;
        public ComboModel NameApp
        {
            get => nameApp;
            set
            {
                this.RaiseAndSetIfChanged(ref nameApp, value);
                InitializationNotifier = NotifyTaskCompletion.Create(LoadData());
            }
        }
        private ComboContainerModel container;
        public ComboContainerModel Container
        {
            get => container;
            set => this.RaiseAndSetIfChanged(ref container, value);
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

        public int MaxConcurrency
        {
            get => maxConcurrency;
            set => this.RaiseAndSetIfChanged(ref maxConcurrency, value);
        }

        public ObservableCollection<ComboModel> ItemsNumber { get; }
        public ObservableCollection<ComboModel> ItemsNumberMulti { get; }

        public string StorageConnectionString
        {
            get => storageConnectionString;
            set => this.RaiseAndSetIfChanged(ref storageConnectionString, value);
        }
        private List<ComboModel> listItemHour;
        private ComboContainerModel itemContainer;
        public ObservableCollection<ComboModel> Items { get; }
        public ObservableCollection<ComboContainerModel> ItemsContainer { get; }
        public ObservableCollection<ComboModel> ItemsAccount { get; }
        public ObservableCollection<ComboModel> DayOfWeeks { get; }
        private ObservableCollection<ComboModel> itemsHour = new ObservableCollection<ComboModel>();
        // public ObservableCollection<ComboModel> ItemsHour { get; }
        public ObservableCollection<ComboModel> ItemsAddHour { get; }
        public List<connect_bytesave> _connect_list { get; }
        public ObservableCollection<ComboModel> ItemsHour
        {
            get { return itemsHour; }
            set
            {
                this.RaiseAndSetIfChanged(ref itemsHour, value);
            }
        }
        private bool isOnceLate = false;

        public bool IsOnceLate
        {
            get => isOnceLate;
            set
            {
                this.RaiseAndSetIfChanged(ref isOnceLate, value);
            }
        }
        private bool isMultiHour = false;

        public bool IsMultiHour
        {
            get => isMultiHour;
            set
            {
                this.RaiseAndSetIfChanged(ref isMultiHour, value);
            }
        }
        public EditConfigBackupViewModel(backup_bytesave folder, List<connect_bytesave> list_connect)
        {
            _connect_list = list_connect;
            this.btnAddHour = new Models.DelegateCommand(o => this.AddHourItem());
            this.btnAddEmail = new Models.DelegateCommand(o => this.AddEmailItem());
            DelHour = ReactiveCommand.Create<ComboModel>(DelHourItem);
            DelEmail = ReactiveCommand.Create<ComboModel>(DelEmailItem);
            this.folder = folder;
            var okEnabled = this.WhenAnyValue(
                        x => x.LocalFolderPath,
                        (x) => !string.IsNullOrWhiteSpace(x));

            Ok = ReactiveCommand.Create(
                () => OkClickFolder());

            JobName = folder.name;

            LocalFolderPath = folder.local_path;


            ItemsContainer = new ObservableCollection<ComboContainerModel>();
            ItemsContainer.Add(new ComboContainerModel
            {
                Id = 0,
                ContainerName = folder.container_name
            });
            container = ItemsContainer.FirstOrDefault();


            ItemsAccount = new ObservableCollection<ComboModel>();
            ItemsAccount.Add(new ComboModel
            {
                Id = 0,
                Name = "--Chọn--"
            });
            var listapp = _connect_list;

            foreach (var item in listapp)
            {
                ItemsAccount.Add(new ComboModel
                {
                    Id = item.id,
                    Name = item.name
                });
            }
            nameApp = ItemsAccount.FirstOrDefault(x => x.Id == folder.id_connect_bytesave);

            if (nameApp == null)
                nameApp = ItemsAccount.OrderBy(x => x.Id).FirstOrDefault();

            IsFolder = folder.is_folder == 0 ? false : true;
            //ContainerNa = itemContainer;

            DayOfWeeks = new ObservableCollection<ComboModel>();
            for (int i = 0; i < 6; i++)
            {
                DayOfWeeks.Add(new ComboModel
                {
                    Id = i,
                    Name = "Thứ " + (i + 2)
                });
            }
            DayOfWeeks.Add(new ComboModel
            {
                Id = 6,
                Name = "Chủ nhật"
            });
            IsWeekly = false;
            var timerString = folder.time;
            if (!string.IsNullOrEmpty(timerString))
            {
                var timerArray = timerString.Split('|');
                //if (type == 2)
                //{
                var dayOfWeekArray = timerArray[0].Split(',').Select(s => Convert.ToInt32(s)).ToList();
                foreach (var day in DayOfWeeks)
                {
                    if (dayOfWeekArray.Contains(day.Id))
                    {
                        day.IsChecked = true;
                    }
                }
                //}
                ItemsNumber = new ObservableCollection<ComboModel>();
                for (int i = 1; i < 25; i++)
                {
                    ItemsNumber.Add(new ComboModel
                    {
                        Id = i,
                        Name = "Number"
                    });
                }

                ItemsNumberMulti = new ObservableCollection<ComboModel>();
                for (int i = 1; i < 25; i++)
                {
                    ItemsNumberMulti.Add(new ComboModel
                    {
                        Id = i,
                        Name = "Number"
                    });
                }
                listItemHour = new List<ComboModel>();
                listEmails = new List<ComboModel>();
                IsMultiHour = false;
                IsOnceLate = false;
                IsOnce = false;
                multiNum = ItemsNumberMulti.FirstOrDefault();
                //if (Convert.ToInt32(timerArray[2]) == 1)
                //{
                //    IsOnce = true;
                //    oneNum = ItemsNumber.FirstOrDefault(x => x.Id == Convert.ToInt32(timerArray[3]));
                //}
                //else if (Convert.ToInt32(timerArray[2]) == 2)
                //{
                //    IsOnceLate = true;
                //    multiNum = ItemsNumberMulti.FirstOrDefault(x => x.Id == Convert.ToInt32(timerArray[3]));
                //}
                //else
                //{
                var listhour = timerArray[1].Split(",");
                int y = 0;
                foreach (var item in listhour)
                {
                    listItemHour.Add(new ComboModel { Id = y, Name = item });
                    y++;
                }

                ItemsHour = new ObservableCollection<ComboModel>(listItemHour.OrderBy(x => x.Id));
                IsMultiHour = true;
                //}
            }

            //CoolTier = folder.CoolTier;
            //ArchiveTier = folder.ArchiveTier;
            DeleteTimer = folder.time_delete;
            Time_delete_file_in_lastversion = folder.time_delete_file_in_LastVersion;
            //MaxConcurrency = folder.MaxConcurrency;
            var listemail = folder.email.Split(",");
            int z = 0;
            foreach (var item in listemail)
            {
                listEmails.Add(new ComboModel { Id = z, Name = item });
                z++;
            }
            ItemsEmail = new ObservableCollection<ComboModel>(listEmails.OrderBy(x => x.Id));
            Cancel = ReactiveCommand.Create(() => { });
        }
        private async Task LoadData()
        {
            var listapp = _connect_list;

            var storageConnet = listapp.FirstOrDefault(x => x.id == NameApp.Id);

            var listContainerOLD = ItemsContainer.Where(x => x.Id >= 1);
            if (listContainerOLD.Count() > 0)
                ItemsContainer.Remove(listContainerOLD.ToList());

            if (storageConnet != null)
            {
                var containers = await AzureBlobHelper.ListContainersAsync(storageConnet.metric_service_information_connect);
                if (containers == null)
                {
                    new ShowMessages("Thông báo", "Error");
                    return;
                }
                int ii = 1;
                foreach (var item in containers)
                {
                    ItemsContainer.Add(new ComboContainerModel
                    {
                        Id = ii,
                        ContainerName = item.Name
                    });
                    ii++;
                }
            }
        }
        public void AddEmailItem()
        {
            if (Email == null)
            {
                MessageBox.Show("Bạn chưa nhập email!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            if (regex.IsMatch(Email) == false)
            {
                MessageBox.Show("Sai định dạng email, bạn vui lòng nhập lại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            listEmails.Add(new ComboModel { Id = 1, Name = Email });
            Email = string.Empty;
            ItemsEmail = new ObservableCollection<ComboModel>(listEmails.OrderBy(x => x.Id));
        }
        public void DelEmailItem(ComboModel email)
        {
            string message = "Bạn có chắc muốn xóa: " + email.Name + " ?";
            string caption = "Thông báo";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.No)
            {
                ItemsEmail = new ObservableCollection<ComboModel>(listEmails.OrderBy(x => x.Id));
            }
            else
            {
                listEmails.Remove(email);
                ItemsEmail = new ObservableCollection<ComboModel>(listEmails.OrderBy(x => x.Id));
            }
            Email = string.Empty;
            ItemsEmail = new ObservableCollection<ComboModel>(listEmails.OrderBy(x => x.Id));
        }
        public void AddHourItem()
        {
            if (timeSelect == null)
            {
                MessageBox.Show("Bạn chưa chọn thời gian! " + timeSelect, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (listItemHour != null && listItemHour.FirstOrDefault(x => x.Name == timeSelect) != null)
            {
                MessageBox.Show("Thời gian đã tồn tại " + timeSelect, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            listItemHour.Add(new ComboModel { Id = 1, Name = TimeSelect });
            ItemsHour = new ObservableCollection<ComboModel>(listItemHour.OrderBy(x => x.Name));
        }
        public void DelHourItem(ComboModel hour)
        {
            string message = "Bạn có chắc muốn xóa: " + hour.Name + " ?";
            string caption = "Thông báo";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.No)
            {
                ItemsHour = new ObservableCollection<ComboModel>(listItemHour.OrderBy(x => x.Id));
            }
            else
            {
                listItemHour.Remove(hour);
                ItemsHour = new ObservableCollection<ComboModel>(listItemHour);
            }

        }
        public backup_bytesave OkClickFolder()
        {
            if (string.IsNullOrWhiteSpace(LocalFolderPath))
            {
                MessageBox.Show("Bạn chưa nhập nguồn  ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            if (string.IsNullOrWhiteSpace(JobName))
            {
                MessageBox.Show("Bạn chưa nhập tên tác vụ ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            if (DayOfWeeks.Where(x => x.IsChecked).Count() == 0 && IsMultiHour == true)
            {
                MessageBox.Show("Bạn chưa chọn ngày trong tuần!  ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            if (listItemHour.Count == 0 && IsMultiHour == true)
            {
                MessageBox.Show("Bạn chưa chọn thời gian! ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            var timerString = string.Empty;

            string k = string.Join(',', DayOfWeeks.Where(x => x.IsChecked).Select(d => d.Id));
            timerString = timerString + k + "|";

            string hourlst = string.Join(',', listItemHour.OrderBy(x => x.Name).Select(x => x.Name));
            timerString = timerString + hourlst;
            string emaillst = string.Join(',', listEmails.Select(x => x.Name));

            return new backup_bytesave
            {
                id = this.folder.id,
                id_connect_bytesave = NameApp.Id,
                name = JobName,
                local_path = LocalFolderPath,
                container_name = Container.ContainerName,
                email = emaillst,
                time_delete = DeleteTimer,
                time_delete_file_in_LastVersion = Time_delete_file_in_lastversion,
                time = timerString,
                is_folder = IsFolder == true ? 1 : 0,
                time_update_at = int.Parse(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                //Id = folder.Id,
                //ContainerName = Container.ContainerName,
                //IdAppSetting = NameApp.Id,
                //JobName = JobName,
                //NameAppSetting = NameApp.Name,
                //LocalFolderPath = LocalFolderPath,
                //ArchiveTier = DeleteTimer >= 30 ? 10 : DeleteTimer >= 10 && DeleteTimer < 30 ? 5 : 0,
                //CoolTier = DeleteTimer >= 30 ? 10 : DeleteTimer >= 10 && DeleteTimer < 30 ? 3 : 0,
                //DeleteTimer = DeleteTimer,
                //MaxConcurrency = 10,
                //TimerString = timerString,
                //IsFolder = IsFolder,
                //Email = emaillst,
                //CreateTime = folder.CreateTime,
                //UpdateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                //Timer = Timer
            };
        }
        public void Tick(int value)
        {
        }
        public ReactiveCommand<Unit, backup_bytesave> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ICommand AddHour { get; set; }
        public ICommand DelHour { get; set; }
        public ICommand DelEmail { get; set; }
        public ICommand btnAddHour { get; private set; }
        public ICommand btnAddEmail { get; private set; }
    }
}
