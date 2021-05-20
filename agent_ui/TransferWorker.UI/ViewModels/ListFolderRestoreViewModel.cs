using Nito.AsyncEx;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using TransferWorker.UI.Helpers;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;
using Nito.AsyncEx;
using System.Windows;
using System.Windows.Threading;
using System;
using System.Windows.Input;
using System.Reactive;
using Avalonia.Controls;
using System.Net;

namespace TransferWorker.UI.ViewModels
{
    public class ListFolderRestoreViewModel : ViewModelBase
    {
        private bool isEnable;
        private string nameContainer;

        public bool IsEnable
        {
            get => isEnable;
            set => this.RaiseAndSetIfChanged(ref isEnable, value);
        }
        public string NameContainer
        {
            get => nameContainer;
            set => this.RaiseAndSetIfChanged(ref nameContainer, value);
        }
        private bool isProgress;
        public bool IsProgress
        {
            get => isProgress;
            set
            {
                this.RaiseAndSetIfChanged(ref isProgress, value);

                switch (isProgress)
                {
                    case true:
                        HiddenIsProgress = "Visible";
                        break;

                    case false:
                        HiddenIsProgress = "Hidden";
                        break;

                    default:
                        break;
                }
            }
        }

        public string hiddenIsProgress;

        public string HiddenIsProgress
        {
            get => hiddenIsProgress;
            set
            {
                this.RaiseAndSetIfChanged(ref hiddenIsProgress, value);
            }
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

        public INotifyTaskCompletion _restoreNotifier;

        public INotifyTaskCompletion RestoreNotifier
        {
            get => _restoreNotifier;
            set
            {
                this.RaiseAndSetIfChanged(ref _restoreNotifier, value);
            }
        }

        private bool isVisible;

        public bool IsVisible
        {
            get => isVisible;
            set => this.RaiseAndSetIfChanged(ref isVisible, value);
        }

        private bool isChecked;

        public bool IsChecked
        {
            get => isChecked;
            set => this.RaiseAndSetIfChanged(ref isChecked, value);
        }
        private string isFirstText;

        public string IsFirstText
        {
            get => isFirstText;
            set => this.RaiseAndSetIfChanged(ref isFirstText, value);
        }

        private backup_bytesave folder;

        public backup_bytesave Folder
        {
            get => folder;
            set
            {
                this.RaiseAndSetIfChanged(ref folder, value);
                InitializationNotifier = NotifyTaskCompletion.Create(LoadData());
            }
        }

        public ObservableCollection<backup_bytesave> Items { get; }
        private List<FileSyncInfo> _allFiles;
        private List<FileViewInfo> _allViewFiles;
        private List<FileViewInfo> _selectedFiles = new List<FileViewInfo>();

        private ObservableCollection<FileViewInfo> files = new ObservableCollection<FileViewInfo>();

        public ObservableCollection<FileViewInfo> Files
        {
            get { return files; }
            set
            {
                this.RaiseAndSetIfChanged(ref files, value);
            }
        }
        public string pathRestore;

        public string PathRestore
        {
            get => pathRestore;
            set
            {
                this.RaiseAndSetIfChanged(ref pathRestore, value);
            }
        }
        public List<connect_bytesave> _connects { get; }
        public List<backup_bytesave> _backups { get; }

        private readonly Settings _settings;
        public ICommand btnSelectAll { get; private set; }
        public ICommand btnRestore { get; private set; }
        public ICommand btnSelect { get; private set; }
        public ListFolderRestoreViewModel()
        {

            HiddenIsProgress = "hidden";
            IsFirstText = "Visible";

            // LoadData();
        }
        public ListFolderRestoreViewModel(List<connect_bytesave> setting, List<backup_bytesave> folders)
        {
            _connects = setting;
            _backups = folders;
            IsProgress = true;
            //_settings = settings;
            Items = new ObservableCollection<backup_bytesave>(folders);
            folder = folders.FirstOrDefault();
            nameContainer = folder != null ? folder.container_name : "";
            InitializationNotifier = NotifyTaskCompletion.Create(LoadData());
            this.btnSelectAll = new Models.DelegateCommand(o => this.SelectAll(1));
            this.btnRestore = new Models.DelegateCommand(o => this.Restore());
            Tickss = ReactiveCommand.Create<FileViewInfo>(SelectFile);
            SelectFolder = ReactiveCommand.Create<FileViewInfo>(LoadFolderDetail);
            IsFirstText = "Hidden";
            // LoadData();
        }

        public ListFolderRestoreViewModel(backup_bytesave folders, List<connect_bytesave> setting)
        {
            // var a = GetIP();
            IsFirstText = "Hidden";
            IsProgress = true;
            _connects = setting;
            //_settings = new MainUtility().LoadConfig().Settings;
            folder = folders;
            NameContainer = folders.container_name;
            InitializationNotifier = NotifyTaskCompletion.Create(LoadData());
            this.btnSelectAll = new Models.DelegateCommand(o => this.SelectAll(1));
            this.btnRestore = new Models.DelegateCommand(o => this.Restore());
            Tickss = ReactiveCommand.Create<FileViewInfo>(SelectFile);
            SelectFolder = ReactiveCommand.Create<FileViewInfo>(LoadFolderDetail);
            // LoadData();
        }
        /// <summary>
        /// load list meta file từ cloud rồi chia thư mục
        /// </summary>
        /// <returns></returns>
        private async Task LoadData()
        {
            IsVisible = true;
            var result = await GetListFile();
            if (result != null)
            {
                _allFiles = result;
                var lstFile = new List<FileViewInfo>();
                int id = 0;
                foreach (var item in _allFiles)
                {
                    var name = Path.GetFileName(item.FileName);
                    var folderNamePath = Path.GetDirectoryName(item.FileName);
                    if (string.IsNullOrEmpty(folderNamePath))
                    {
                        lstFile.Add(new FileViewInfo
                        {
                            Id = id++,
                            FileName = name,
                            IsFolder = "Hidden",
                            IsFile = "Visible",
                            Level = 0,
                            CreatedTime = item.CreatedTime,
                            LastModified = item.LastModified,
                            LastModifiedDisplay = item.LastModified.ToString("dd/MM/yyyy hh:mm:ss tt"),
                            Parent = string.Empty,
                            //Ticked = ReactiveCommand.Create<FileViewInfo>(SelectFile),
                            Select = null,
                            OriginalPath = item.FileName
                        });
                    }
                    else
                    {
                        var strArray = folderNamePath.Split('\\');
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            if (lstFile.Exists(f => f.Level == i && f.FileName == strArray[i]))
                            {
                                continue;
                            }
                            lstFile.Add(new FileViewInfo
                            {
                                Id = id++,
                                FileName = strArray[i],
                                IsFolder = "Visible",
                                IsFile = "Hidden",
                                Level = i,
                                CreatedTime = item.CreatedTime,
                                LastModified = item.LastModified,
                                LastModifiedDisplay = item.LastModified.ToString("dd/MM/yyyy hh:mm:ss tt"),
                                Parent = i == 0 ? "" : strArray[i - 1],
                                //Select = ReactiveCommand.Create<FileViewInfo>(LoadFolderDetail),
                                //Ticked = ReactiveCommand.Create<FileViewInfo>(SelectFile),
                                OriginalPath = item.FileName
                            });
                        }
                        if (!string.IsNullOrEmpty(name))
                        {
                            lstFile.Add(new FileViewInfo
                            {
                                Id = id++,
                                FileName = name,
                                IsFolder = "Hidden",
                                IsFile = "Visible",
                                Level = strArray.Length,
                                CreatedTime = item.CreatedTime,
                                LastModified = item.LastModified,
                                LastModifiedDisplay = item.LastModified.ToString("dd/MM/yyyy hh:mm:ss tt"),
                                Parent = strArray[strArray.Length - 1],
                                //Ticked = ReactiveCommand.Create<FileViewInfo>(SelectFile),
                                OriginalPath = item.FileName
                            });
                        }
                    }
                }
                _allViewFiles = lstFile;
                LoadFolderDetail();
                IsVisible = false;
                IsProgress = false;
            }
            else
            {
                IsVisible = false;
                IsProgress = false;
                //  MessageBox.Show("Containner trống");
                //new Messages().ThongBaoAsync("Thông báo", "Container đang trống!", Icon.Folder, ButtonEnum.Ok);
                return;
            }
        }

        public void Restore()
        {
            if (PathRestore == null)
            {
                MessageBox.Show("Bạn chưa chọn đường dẫn thư mục tại máy", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            RestoreNotifier = NotifyTaskCompletion.Create(RestoreSeletedFile(PathRestore));
        }

        public async Task RestoreSeletedFile(string path)
        {
            try
            {
                if (_selectedFiles.Count == 0)
                {
                    MessageBox.Show("Chưa có file được chọn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                IsVisible = true;
                await Task.Run(async () =>
                {
                    var option = _connects.FirstOrDefault(x => x.id == folder.id_connect_bytesave);

                    IAzureBlobHelper _azureBlobHelper = new AzureBlobHelper(option, folder);
                    await _azureBlobHelper.Download(_selectedFiles, path);
                    new MainUtility().AddHistory("Phục hồi", "Thành công: " + _selectedFiles.Count + " file", 1);
                    MessageBox.Show("Phục hồi thành công", "Thông báo");
                });

                IsVisible = false;
                //xóa hết file đã chọn
                IsChecked = true;
                _allViewFiles.ConvertAll(f => f.isChecked = false);
                SelectAll(0);

            }
            catch (Exception)
            {
                new MainUtility().AddHistory("Phục hồi", "Phục hồi không thành công: " + _selectedFiles.Count + " file", 0);
            }

        }


        /// <summary>
        /// chọn hoặc bỏ chọn tất
        /// </summary>
        public void SelectAll(int? reSelect)
        {
            // isChecked = !isChecked;
            //if (reSelect == 0)
            //{
            //    isChecked = false;
            //}
            foreach (var item in Files)
            {
                item.isChecked = reSelect != 0?!isChecked : false;
                //item.isChecked = true;
                CheckChild(item);
            }
            //gán lại để cho view update lại giao diện
            var tempFiles = Files;
            Files = new ObservableCollection<FileViewInfo>();
            Files = tempFiles;

            _selectedFiles.Clear();
            if (_allViewFiles != null)
            {
                _selectedFiles.AddRange(_allViewFiles.Where(f => f.isChecked));
            }
            isChecked = !isChecked;
        }

        public void SelectFile(FileViewInfo info = null)
        {
            if (info != null && info.IsFolder == "Visible")
            {
                var file = _allViewFiles.FirstOrDefault(f => f.Id == info.Id);
                if (file != null)
                {
                    file.isChecked = !info.isChecked;
                }
                CheckChild(info);
            }
            else
            {
                if (info != null)
                {
                    var file = _allViewFiles.FirstOrDefault(f => f.Id == info.Id);
                    if (file != null)
                    {
                        file.isChecked = !info.isChecked;
                    }
                }
            }
            //gán lại để cho view update lại giao diện
            //gán lại để cho view update lại giao diện
            var tempFiles = Files;
            Files = new ObservableCollection<FileViewInfo>(tempFiles.OrderByDescending(x => x.IsFolder));

            _selectedFiles.Clear();
            _selectedFiles.AddRange(_allViewFiles.Where(f => f.isChecked));
        }

        private void CheckChild(FileViewInfo info)
        {
            var childs = _allViewFiles.Where(f => f.Parent == info.FileName && f.Level == info.Level + 1);
            if (childs == null || childs.Count() <= 0)
            {
                return;
            }
            foreach (var item in childs)
            {
                item.isChecked = info.isChecked;

                if (item.IsFolder == "Visible")
                {
                    CheckChild(item);
                }
            }
        }

        /// <summary>
        /// chia thư mục file
        /// </summary>
        /// <param name="info"></param>
        public void LoadFolderDetail(FileViewInfo info = null)
        {
            //Files.Clear();
            if (info != null && info.IsFile == "Visible")
            {
                return;
            }
            var folder = info == null ? "" : info.FileName;
            var lstFile = new List<FileViewInfo>();
            var level = 0;
            if (!string.IsNullOrEmpty(folder))
            {
                //quay lại thư mục cha
                if (folder == "[...]")
                {
                    level = info.Level;
                    folder = info.Parent;
                    if (level > 0)       // tìm thư mục cha của cha
                    {
                        var currentFolder = _allViewFiles.FirstOrDefault(f => f.Id == info.Id);
                        if (currentFolder != null)
                        {
                            var parent = _allViewFiles.FirstOrDefault(f => f.FileName == currentFolder.Parent && f.Level == currentFolder.Level - 1);

                            if (parent != null)
                            {
                                lstFile.Add(new FileViewInfo
                                {
                                    Id = parent.Id,
                                    FileName = "[...]",
                                    IsFolder = "Visible",
                                    IsFile = "Hidden",
                                    Level = parent.Level,
                                    Select = parent.Select,
                                    Parent = parent.Parent
                                });
                            }
                            folder = parent.FileName;
                            level = currentFolder.Level;
                        }
                    }
                }
                else
                {
                    //thêm thư mục cha để quay lại
                    lstFile.Add(new FileViewInfo
                    {
                        Id = info.Id,
                        FileName = "[...]",
                        IsFolder = "Visible",
                        IsFile = "Hidden",
                        Level = info.Level,
                        Select = info.Select,
                        Parent = info.Parent
                    });

                    level = info.Level + 1;
                }
            }

            lstFile.AddRange(_allViewFiles.Where(f => f.Level == level &&
            (string.IsNullOrEmpty(folder) ? true : (f.Parent == folder))).OrderByDescending(o => o.IsFolder).ThenBy(o => o.FileName).ToList());
            Files = new ObservableCollection<FileViewInfo>(lstFile.OrderByDescending(x => x.IsFolder));

        }

        private Task<List<FileSyncInfo>> GetListFile()
        {
            var item_connect = _connects.FirstOrDefault(x => x.id == folder.id_connect_bytesave);
            if (item_connect == null)
                return null;
            IAzureBlobHelper _azureBlobHelper = new AzureBlobHelper(item_connect, folder);
            if (folder != null)
                return _azureBlobHelper.GetListFileFromCloud(folder.container_name, folder.id_connect_bytesave,item_connect.metric_service_information_connect);
            return _azureBlobHelper.GetListFileFromCloud();
            //foreach (var item in _connects)
            //{
            //    IAzureBlobHelper _azureBlobHelper = new AzureBlobHelper(item, folder);
            //    if (folder != null)
            //        return _azureBlobHelper.GetListFileFromCloud(folder.container_name, folder.id_connect_bytesave);
            //    return _azureBlobHelper.GetListFileFromCloud();
            //}
            //return null;
        }
        public string GetIP()
        {
            //String hostName = string.Empty;
            //hostName = Dns.GetHostName();
            //IPHostEntry myIP = Dns.GetHostEntry(hostName);
            //IPAddress[] address = myIP.AddressList;
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            //HostNameTextLabel.Text = hostName;
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress[] addr = hostEntry.AddressList;
            var ip = addr.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                         .FirstOrDefault();
            return ip.ToString() ?? "";
            //return myIP;

        }
        public ICommand Tickss { get; set; }
        public ICommand SelectFolder { get; set; }
    }
}

