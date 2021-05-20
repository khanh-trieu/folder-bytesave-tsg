using DynamicData;
using Microsoft.Toolkit.Extensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Nito.AsyncEx;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TransferWorker.UI.Helpers;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;
using TransferWorker.UI.Views;

namespace TransferWorker.UI.ViewModels
{
    public class AddConfigBackupViewModel : ViewModelBase
    {
        private int time_delete_file_in_lastversion;
        private string localFolderPath;
        private string containerName;
        private int deleteTimer;
        private int coolTier;
        private int archiveTier;

        private int idAppSetting;
        private int timer;
        private string storageConnectionString;
        private int maxConcurrency;

        private List<ComboModel> listItemHour;
        private List<ComboModel> listEmails;

        private List<connect_bytesave> _connect_list;


        public INotifyTaskCompletion _initializationNotifier;

        public INotifyTaskCompletion InitializationNotifier
        {
            get => _initializationNotifier;
            set
            {
                this.RaiseAndSetIfChanged(ref _initializationNotifier, value);
            }
        }

        private ComboModel oneNum;

        public ComboModel OneNum
        {
            get => oneNum;
            set => this.RaiseAndSetIfChanged(ref oneNum, value);
        }


        private ObservableCollection<ComboModel> itemsHour = new ObservableCollection<ComboModel>();

        public ObservableCollection<ComboModel> ItemsHour
        {
            get { return itemsHour; }
            set
            {
                this.RaiseAndSetIfChanged(ref itemsHour, value);
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
        private string email;

        public string Email
        {
            get => email;
            set
            {
                this.RaiseAndSetIfChanged(ref email, value);

            }
        }
        private bool isFolder;

        public bool IsFolder
        {
            get => isFolder;
            set => this.RaiseAndSetIfChanged(ref isFolder, value);
        }

        private bool isOnce = false;

        public bool IsOnce
        {
            get => isOnce;
            set => this.RaiseAndSetIfChanged(ref isOnce, value);
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
        private bool isMultiHour = true;

        public bool IsMultiHour
        {
            get => isMultiHour;
            set
            {
                this.RaiseAndSetIfChanged(ref isMultiHour, value);
            }
        }

        private bool isMulti = true;

        public bool IsMulti
        {
            get => isMulti;
            set => this.RaiseAndSetIfChanged(ref isMulti, value);
        }

        public ObservableCollection<ComboModel> Items { get; }
        public ObservableCollection<ComboModel> ItemsNumber { get; }
        public ObservableCollection<ComboModel> ItemsNumberMulti { get; }
        public ObservableCollection<ComboContainerModel> ItemsContainer { get; }
        public ObservableCollection<ComboModel> ItemsAccount { get; }
        public ObservableCollection<ComboModel> DayOfWeeks { get; }
        public json_connect_bytesave data_json { get; }
        public string LocalFolderPath
        {
            get => localFolderPath;
            set => this.RaiseAndSetIfChanged(ref localFolderPath, value);
        }
        public int Time_delete_file_in_lastversion
        {
            get => time_delete_file_in_lastversion;
            set => this.RaiseAndSetIfChanged(ref time_delete_file_in_lastversion, value);
        }
        private string timeSelect;
        public string TimeSelect
        {
            get => timeSelect;
            set => this.RaiseAndSetIfChanged(ref timeSelect, value);
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
        public int Timer
        {
            get => timer;
            set => this.RaiseAndSetIfChanged(ref timer, value);
        }
        public string StorageConnectionString
        {
            get => storageConnectionString;
            set => this.RaiseAndSetIfChanged(ref storageConnectionString, value);
        }
        private string jobName;
        public string JobName
        {
            get => jobName;
            set => this.RaiseAndSetIfChanged(ref jobName, value);
        }


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
        private List<string> listEmail;
        public List<string> ListEmail
        {
            get => listEmail;
            set => this.RaiseAndSetIfChanged(ref listEmail, value);
        }
        //private ComboModel daily;

        //public ComboModel Daily
        //{
        //    get => daily;
        //    set
        //    {
        //        this.RaiseAndSetIfChanged(ref daily, value);
        //        switch (daily.Id)
        //        {
        //            case 1:
        //                IsWeekly = false;
        //                break;

        //            case 2:
        //                IsWeekly = true;
        //                break;

        //            default:
        //                break;
        //        }
        //    }
        //}
        public Settings _setting;
        public AddConfigBackupViewModel(Settings setting)
        {
            _connect_list = setting.connect_bytesaves;
            _setting = setting;
            this.btnAddHour = new Models.DelegateCommand(o => this.AddHourItem());
            this.btnAddEmail = new Models.DelegateCommand(o => this.AddEmailItem());
            DelHour = ReactiveCommand.Create<ComboModel>(DelHourItem);
            DelEmail = ReactiveCommand.Create<ComboModel>(DelEmailItem);
            ItemsAccount = new ObservableCollection<ComboModel>();
            ItemsContainer = new ObservableCollection<ComboContainerModel>();
            ItemsAccount.Add(new ComboModel
            {
                Id = 0,
                Name = "--Chọn--"
            });
            ItemsContainer.Add(new ComboContainerModel
            {
                Id = 0,
                ContainerName = "--Chọn--"
            });

            foreach (var item in _connect_list)
            {
                ItemsAccount.Add(new ComboModel
                {
                    Id = item.id,
                    Name = item.name
                });
            }
            container = ItemsContainer.OrderBy(x => x.Id).FirstOrDefault();
            nameApp = ItemsAccount.OrderBy(x => x.Id).FirstOrDefault();
            NotifyTaskCompletion.Create(LoadData());
            ItemsNumber = new ObservableCollection<ComboModel>();
            for (int i = 1; i < 25; i++)
            {
                ItemsNumber.Add(new ComboModel
                {
                    Id = i,
                    Name = "Number"
                });
            }
            oneNum = ItemsNumber[3];
            ItemsNumberMulti = new ObservableCollection<ComboModel>();
            for (int i = 1; i < 25; i++)
            {
                ItemsNumberMulti.Add(new ComboModel
                {
                    Id = i,
                    Name = "Number"
                });
            }

            multiNum = ItemsNumberMulti[3];

            //Items = new ObservableCollection<ComboModel>();

            //Items.Add(new ComboModel
            //{
            //    Id = 1,
            //    Name = "Ngày"
            //});
            //Items.Add(new ComboModel
            //{
            //    Id = 2,
            //    Name = "Tuần"
            //});
            //daily = Items.FirstOrDefault();
            IsWeekly = false;
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
            listItemHour = new List<ComboModel>();

            listEmails = new List<ComboModel>();

            var okEnabled = this.WhenAnyValue(
                        x => x.LocalFolderPath,
                        y => y.JobName,
                        (x, y) => !string.IsNullOrWhiteSpace(x) && !string.IsNullOrWhiteSpace(y));


            Time_delete_file_in_lastversion = 30;
            DeleteTimer = 30;
            ArchiveTier = 10;
            CoolTier = 10;

            Ok = ReactiveCommand.Create(
                    OkClickFolder);
            Cancel = ReactiveCommand.Create(() => { });
        }

        public void AddEmailItem()
        {
            if (Email == null)
            {
                MessageBox.Show("Bạn chưa nhập Email!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            if (regex.IsMatch(Email) == false)
            {
                MessageBox.Show("Sai định dạng email, bạn vui lòng nhập lại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            listEmails.Add(new ComboModel { Id = listEmails.Count == 0 ? 1 : listEmails.Max(x => x.Id) + 1, Name = Email });
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
                ItemsHour = new ObservableCollection<ComboModel>(listItemHour.OrderBy(x => x.Name));
            }
            else
            {
                listItemHour.Remove(hour);
                ItemsHour = new ObservableCollection<ComboModel>(listItemHour.OrderBy(x => x.Name));
            }
        }
        private async Task LoadData()
        {
            var listapp = _connect_list;
            var storageConnet = listapp.FirstOrDefault(x => x.name == nameApp.Name);
            var listContainerOLD = ItemsContainer.Where(x => x.Id >= 1);
            if (listContainerOLD.Count() > 0)
                ItemsContainer.Remove(listContainerOLD.ToList());
            if (storageConnet != null)
            {
                var containers = await AzureBlobHelper.ListContainersAsync(storageConnet.metric_service_information_connect);
                if (containers == null)
                {

                    MessageBox.Show("Connection string problem! Please check again", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (Container.Id == 0)
            {
                MessageBox.Show("Bạn chưa chọn vùng chứa đích ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            //var _settings = new TransferWorker.UI.Utility.MainUtility().LoadConfig();
            var check = _setting.backup_bytesaves.Where(x => x.name == jobName);
            //_settings.Settings.Folders.FirstOrDefault(x => x.JobName == jobName);
            //if (check == false)
            //{
            //    return null;
            //}

            if (check.Count() > 0)
            {
                MessageBox.Show("Tác vụ sao lưu đã tồn tại!  ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
            timerString = timerString +  hourlst;
            string emaillst = string.Join(',', listEmails.Select(x => x.Name));
            return new backup_bytesave
            {
                id_connect_bytesave = NameApp.Id,
                connect_bytesave_name = NameApp.Name,
                name = JobName,
                local_path = LocalFolderPath,
                container_name = Container.ContainerName,
                email = emaillst,
                time_delete = DeleteTimer,
                time_delete_file_in_LastVersion = Time_delete_file_in_lastversion,
                time = timerString,
                is_folder = IsFolder == true ? 1 : 0,
                time_create_at = int.Parse(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                time_update_at = int.Parse(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),


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
            //CreateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
            //UpdateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
            //Timer = Timer
        };
        }

        public void Tick(int value)
        {
        }



        public async Task getllist(string connectstring)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectstring);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainers = new List<CloudBlobContainer>();
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var containerSegment = await blobClient.ListContainersSegmentedAsync(blobContinuationToken);
                blobContainers.AddRange(containerSegment.Results);
                blobContinuationToken = containerSegment.ContinuationToken;

            } while (blobContinuationToken != null);
        }


        public async Task ContainersCleanup(string connectString)
        {
            // Get blob client
            var storageAccount = CloudStorageAccount.Parse(connectString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Remove all test containers
            var testContainers = await this.ListContainers(blobClient, "")
                .ConfigureAwait(false);

            foreach (var container in testContainers)
            {
                await container.DeleteIfExistsAsync()
                    .ConfigureAwait(false);
            }
        }
        private async Task<IEnumerable<CloudBlobContainer>> ListContainers(CloudBlobClient blobClient, string prefix)
        {
            BlobContinuationToken continuationToken = null;
            var containers = new List<CloudBlobContainer>();

            do
            {
                var response = await blobClient
                    .ListContainersSegmentedAsync(prefix, continuationToken)
                    .ConfigureAwait(false);

                containers.AddRange(response.Results);
                continuationToken = response.ContinuationToken;
            }
            while (continuationToken != null);

            return containers;
        }
        public ICommand AddHour { get; set; }
        public ICommand DelHour { get; set; }
        public ICommand DelEmail { get; set; }
        public ICommand btnAddHour { get; private set; }
        public ICommand btnAddEmail { get; private set; }
        public ReactiveCommand<Unit, backup_bytesave> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        //public ICommand btnDelEmail { get; private set; }
    }
}
