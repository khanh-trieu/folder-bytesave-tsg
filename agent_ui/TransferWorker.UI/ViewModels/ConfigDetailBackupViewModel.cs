using DynamicData;
using Nito.AsyncEx;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TransferWorker.UI.Helpers;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;

namespace TransferWorker.UI.ViewModels
{
    public class ConfigDetailBackupViewModel : ViewModelBase
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
        private string email;

        public string Email
        {
            get => email;
            set => this.RaiseAndSetIfChanged(ref email, value);
        }
        private string nameAcc;
        public string NameAcc
        {
            get => nameAcc;
            set => this.RaiseAndSetIfChanged(ref nameAcc, value);
        }
        private string containerNa;
        public string ContainerNa
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

        private ComboModel multiNum;

        public ComboModel MultiNum
        {
            get => multiNum;
            set => this.RaiseAndSetIfChanged(ref multiNum, value);
        }

        private string isWeekly;

        public string IsWeekly
        {
            get => isWeekly;
            set => this.RaiseAndSetIfChanged(ref isWeekly, value);
        }

        private string isDay;

        public string IsDay
        {
            get => isDay;
            set => this.RaiseAndSetIfChanged(ref isDay, value);
        }

        private string createTime;

        public string CreateTime
        {
            get => createTime;
            set => this.RaiseAndSetIfChanged(ref createTime, value);
        }
        private string name_connect;

        public string Name_connect
        {
            get => name_connect;
            set => this.RaiseAndSetIfChanged(ref name_connect, value);
        }
        private string updateTime;

        public string UpdateTime
        {
            get => updateTime;
            set => this.RaiseAndSetIfChanged(ref updateTime, value);
        }
        private string nextTime;

        public string NextTime
        {
            get => nextTime;
            set => this.RaiseAndSetIfChanged(ref nextTime, value);
        }
        private string nextRunTime;

        public string NextRunTime
        {
            get => nextRunTime;
            set => this.RaiseAndSetIfChanged(ref nextRunTime, value);
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
                        IsWeekly = "Hidden";
                        break;

                    case 2:
                        IsWeekly = "Visible";
                        break;

                    default:
                        break;
                }
            }
        }

        private ComboModel nameApp;
        public ComboModel NameApp
        {
            get => nameApp;
            set
            {
                this.RaiseAndSetIfChanged(ref nameApp, value);
                // InitializationNotifier = NotifyTaskCompletion.Create(LoadData());
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


        public ObservableCollection<ComboModel> ItemsNumberMulti { get; }

        public string StorageConnectionString
        {
            get => storageConnectionString;
            set => this.RaiseAndSetIfChanged(ref storageConnectionString, value);
        }
        public ObservableCollection<ComboModel> Items { get; }
        public ObservableCollection<ComboContainerModel> ItemsContainer { get; }
        public ObservableCollection<ComboModel> ItemsAccount { get; }
        public ObservableCollection<ComboModel> DayOfWeeks { get; }
        private ObservableCollection<string> itemsHour = new ObservableCollection<string>();

        public ObservableCollection<string> ItemsHour
        {
            get { return itemsHour; }
            set
            {
                this.RaiseAndSetIfChanged(ref itemsHour, value);
            }
        }
        private List<ComboModel> listItemHour;
        public ConfigDetailBackupViewModel()
        {
            IsHave = "Hidden";
            IsNoHave = "Visible";
        }
        public ConfigDetailBackupViewModel(backup_bytesave folder)
        {
            try
            {
                IsHave = "Visible";
                IsNoHave = "Hidden";
                if (folder == null)
                {
                    IsHave = "Hidden";
                    IsNoHave = "Visible";
                    return;
                }
                this.folder = folder;
                var okEnabled = this.WhenAnyValue(
                            x => x.LocalFolderPath,
                            (x) => !string.IsNullOrWhiteSpace(x));

                JobName = folder.name;
                LocalFolderPath = folder.local_path;


                CreateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(folder.time_create_at).ToLocalTime().ToString("dd/MM/yyyy hh:MM:ss tt");
                UpdateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(folder.time_update_at).ToLocalTime().ToString("dd/MM/yyyy hh:MM:ss tt");
                Name_connect = folder.connect_bytesave_name;
                NameAcc = folder.connect_bytesave_username_account;

                MaxConcurrency = 10;
                DeleteTimer = folder.time_delete;
                //ArchiveTier = folder.ArchiveTier;
                //CoolTier = folder.CoolTier;

                // NotifyTaskCompletion.Create(LoadData());
                ContainerNa = folder.container_name;
                Items = new ObservableCollection<ComboModel>();
                Items.Add(new ComboModel
                {
                    Id = 1,
                    Name = "Ngày"
                });
                Items.Add(new ComboModel
                {
                    Id = 2,
                    Name = "Tuần"
                });
                daily = Items.FirstOrDefault();
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
                IsWeekly = "Hidden";
                IsDay = "Visible";
                var timerString = folder.time;
                if (!string.IsNullOrEmpty(timerString))
                {
                    var timerArray = timerString.Split('|');
                    //var type = Convert.ToInt32(timerArray[0]);
                    //daily = Items.FirstOrDefault(t => t.Id == type);
                    //if (type == 2)
                    //{
                    //    IsWeekly = "Visible";
                    //    IsDay = "Hidden";
                    //    var dayOfWeekArray = timerArray[1].Split(',').Select(s => Convert.ToInt32(s)).ToList();

                    //    foreach (var day in DayOfWeeks)
                    //    {
                    //        if (dayOfWeekArray.Contains(day.Id))
                    //        {
                    //            day.IsChecked = true;
                    //        }
                    //    }
                    //}
                    IsWeekly = "Visible";
                    IsDay = "Hidden";
                    var dayOfWeekArray = timerArray[0].Split(',').Select(s => Convert.ToInt32(s)).ToList();

                    foreach (var day in DayOfWeeks)
                    {
                        if (dayOfWeekArray.Contains(day.Id))
                        {
                            day.IsChecked = true;
                        }
                    }
                    //if (Convert.ToInt32(timerArray[2]) == 2)
                    //{
                    //    IsWeekly = "Collapsed";
                    //    //multiNum = ItemsNumberMulti.FirstOrDefault(x => x.Id == Convert.ToInt32(timerArray[3]));
                    //    var lateHour = Convert.ToInt32(timerArray[3]);
                    //    TextTime = "chạy lặp lại sau mỗi " + lateHour + " giờ";
                    //    var timenext = Convert.ToDateTime(folder.LastRunTime == null ? folder.CreateTime : folder.LastRunTime).AddHours(lateHour);
                    //    //(folder.CreateTime).AddHours(multiNum.Id);
                    //    NextTime = timenext.ToString("[HH:mm] dd/MM/yyyy");
                    //    foreach (var day in DayOfWeeks)
                    //    {
                    //        day.IsChecked = true;
                    //    }
                    //}
                    //else
                    //{
                    IsWeekly = "Visible";
                    var listhour = timerArray[1].Replace(",", "] - [");
                    var listitem = timerArray[1].Split(",");
                    listItemHour = new List<ComboModel>();
                    TextTime = listhour;
                    var now = DateTime.Now.ToShortTimeString();
                    int i = 0;
                    foreach (var item in listitem)
                    {
                        var a = DateTime.Parse(item.ToString());
                        var b = DateTime.Parse(now.ToString());
                        if (a > b)
                        {
                            listItemHour.Add(new ComboModel { Id = i, Name = item });
                            i++;
                        }
                    }
                    NextTime = listItemHour.Count == 0 ? DateTime.Parse(listitem[0]).AddDays(1).ToString("HH:mm dd/MM/yyyy") : DateTime.Parse(listItemHour.OrderBy(x => x.Name).FirstOrDefault().Name).ToString("HH:mm dd/MM/yyyy");
                    // ItemsHour = new ObservableCollection<string>(listItemHour);
                    //}
                }
                var listEmail = folder.email.Replace(",", "] - [");
                Email = listEmail;
                Cancel = ReactiveCommand.Create(() => { });
            }
            catch (Exception ex)
            {

            }

        }
        //private async Task LoadData()
        //{
        //    var listapp = new MainUtility().LoadConfig();

        //    var storageConnet = listapp.Settings.AppSettings.FirstOrDefault(x => x.NameAppSetting == nameApp.Name);
        //    var listContainerOLD = ItemsContainer.Where(x => x.Id >= 1);
        //    if (listContainerOLD.Count() > 0)
        //        ItemsContainer.Remove(listContainerOLD.ToList());

        //    if (storageConnet != null)
        //    {
        //        var containers = await AzureBlobHelper.ListContainersAsync(storageConnet.StorageConnectionString);
        //        if (containers == null)
        //        {
        //            new ShowMessages("Thông báo", "Lỗi");
        //            return;
        //        }
        //        int ii = 1;
        //        foreach (var item in containers)
        //        {
        //            ItemsContainer.Add(new ComboContainerModel
        //            {
        //                Id = ii,
        //                ContainerName = item.Name
        //            });
        //            ii++;
        //        }
        //    }

        //}

        public void Tick(int value)
        {
        }
        public ReactiveCommand<Unit, FolderConfig> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
    }
}
