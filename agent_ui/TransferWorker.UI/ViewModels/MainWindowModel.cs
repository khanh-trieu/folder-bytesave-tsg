using Microsoft.WindowsAzure.Storage;
using NetCore.Utils.Log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;
using TransferWorker.UI.Views;

namespace TransferWorker.UI.ViewModels
{
    public class MainWindowModel : ViewModelBase
    {
        private bool isVisible;

        public bool IsVisible
        {
            get => isVisible;
            set => this.RaiseAndSetIfChanged(ref isVisible, value);
        }
        private bool isLicense;

        public bool IsLicense
        {
            get => isLicense;
            set => this.RaiseAndSetIfChanged(ref isLicense, value);
        }
        private ViewModelBase contentMenu;

        public ViewModelBase ContentMenu
        {
            get => contentMenu;
            private set => this.RaiseAndSetIfChanged(ref contentMenu, value);
        }
        private ViewModelBase contentDetail;

        public ViewModelBase ContentDetail
        {
            get => contentDetail;
            private set => this.RaiseAndSetIfChanged(ref contentDetail, value);
        }

        private ViewModelBase content;
        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }
        private ObservableCollection<LogContent> _listlogs = new ObservableCollection<LogContent>();

        public ObservableCollection<LogContent> _Listlogs
        {
            get { return _listlogs; }
            set
            {
                this.RaiseAndSetIfChanged(ref _listlogs, value);
            }
        }
        public ObservableCollection<LogContent> Logs { get; }
        //private List<LogContent> _listlogs;
        public ICommand AddConfigAppSetting { get; set; }
        public ICommand btnAppSettingConfigView { get; private set; }
        public ICommand btnFolderConfigView { get; private set; }
        public ICommand btnRestoreView { get; private set; }
        public ICommand btnAddConfigApp { get; private set; }
        public ICommand btnEditConfigApp { get; private set; }
        public ICommand btnAddItemBakup { get; private set; }
        public ICommand btnEditItemConfigView { get; private set; }
        public ICommand btnInfoView { get; private set; }
        public ICommand btnHistoryView { get; private set; }
        public ICommand btnSettingView { get; private set; }
        public ICommand Cmd { get; set; }

        private connect_bytesave _connect_bytesave;
        private List<connect_bytesave> _list_connect_bytesave;


        private Settings _settings;
        private AppSetting _appsetting;

        private backup_bytesave _folder;

        private string _email_loggin;

        private Logs _logs;
        public ObservableCollection<FolderConfig> Items { get; }


        //public ObservableCollection<FolderConfig> ContentJob { get; }
        public MainWindowModel()
        {
            try
            {
                if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 2) { App.Current.Shutdown(0); App.Current.Run(); }
                NLogManager.LogInfo("MainWindowModel");
                var appjson = new MainUtility().LoadConfig();
                _settings = appjson.Settings;
                _email_loggin = appjson.email_loggin;

                int date_now = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var ex_date = _settings.bytesave_info.information.bytesave_expiration_date;
                if (date_now > ex_date)
                {
                    ContentDetail = new InfoViewModel(_settings, _email_loggin);
                    ContentMenu = null;
                    
                    //LogginView objPopupwindow = new LogginView();
                    //objPopupwindow.Show();
                    MessageBox.Show("Tài khoản của bạn đã hết hạn sử dụng! bạn vui lòng gia hạn để có thể tiếp tục sử dụng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    return;

                }
                string mac = new MainUtility().GetMac();
                IsLicense = true;
                string comp = "";
                ListBackup();
                ListConnect();
                //kiểm tra connect khi mở app
                AutoConnect();

                this.btnRestoreView = new Models.DelegateCommand(o => this.RestoreView());
                this.btnFolderConfigView = new Models.DelegateCommand(o => this.FolderConfigView());
                this.btnAppSettingConfigView = new Models.DelegateCommand(o => this.AppSettingConfigView());
                //  this.btnAddConfigApp = new Models.DelegateCommand(o => this.AddItemConfigView());
                this.btnEditConfigApp = new Models.DelegateCommand(o => this.EditItemConfigView(_connect_bytesave));
                btnAddItemBakup = new Prism.Commands.DelegateCommand(AddItemBakup);
                btnAddConfigApp = new Prism.Commands.DelegateCommand(AddItemConfigView);
                this.btnEditItemConfigView = new Models.DelegateCommand(o => this.EditItemBackup(_folder));
                this.btnInfoView = new Models.DelegateCommand(o => this.InfoView());
                this.btnHistoryView = new Models.DelegateCommand(o => this.HistoryView());
                this.btnSettingView = new Models.DelegateCommand(o => this.SettingView());
            }
            catch (Exception ex)
            {
                NLogManager.LogInfo("Error MainWin" + ex.ToString());
                new MainUtility().save_log_agent(ex.ToString(), "MainWindowModel", 0, 0, _email_loggin);
                IsLicense = false;
                ContentDetail = new InfoViewModel(_settings, _email_loggin);
                ContentMenu = null;
                //LogginView window = new LogginView();
                //window.Show();
                //App.Current.Shutdown(0); App.Current.Run();
                return;
                
            }
        }
       
        public void RestoreView()
        {
            Load_config();
            ListContainer();
            //GetService();
            //if (_settings == null)
            //{
            //    new ShowMessages("Thông báo", "Error file");
            //    return;
            //}
            //else
            //{
            //    var list = new ListFolderRestoreViewModel();
            //    ContentDetail = list;
            //    ContentMenu = ListMenuRestore;
            //}
            // _Listlogs = new ObservableCollection<LogContent>(new MainUtility().LoadConfig().Logs.LogContents);
        }
        public void ListBackup()
        {
            Load_config();
            var list_backup = _settings.backup_bytesaves;
            if (list_backup == null)
            {
                var DetailBackup_null = new ConfigDetailBackupViewModel(null);
                var listMenubackup_null = new ConfigMenuBackupViewModel(null);
                ContentMenu = listMenubackup_null;
                ContentDetail = DetailBackup_null;
                return;
            }
            _folder = list_backup.FirstOrDefault();
            //foreach (var item in _settings.AppSettings)
            //{
            //    if (string.IsNullOrWhiteSpace(item.StorageConnectionString))
            //    {
            //        MessageBox.Show("Data null", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}
            var listMenubackup = new ConfigMenuBackupViewModel(list_backup);
            Observable.Merge(
                listMenubackup.Detail)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        BackupDetaiSelect(model);
                    }
                });
            Observable.Merge(
                listMenubackup.Delete)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        try
                        {
                            _settings.backup_bytesaves.Remove(model);
                            //var is_del = new MainUtility().del_backup_from_agent(model.id);
                            new MainUtility().WriteConfig(_settings);
                            _folder = _settings.backup_bytesaves.FirstOrDefault();
                            new MainUtility().AddHistory("Sao lưu", "Xóa tác vụ sao lưu: " + model.name, 1);
                            MessageBox.Show("Xóa thành công tác vụ sao lưu:" + model.name, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            ListBackup();
                            new MainUtility().del_backup_from_agent(model.id);
                            //App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                            //        {
                            //            ListBackup();
                            //            //_folder = _settings.Folders.FirstOrDefault();
                            //            new MainUtility().WriteConfig(_settings);
                            //            new MainUtility().AddLog("Sao lưu", "Xóa tác vụ sao lưu: " + model.name, 1);
                            //        });
                        }
                        catch (Exception)
                        {
                            new MainUtility().AddHistory("Sao lưu", "Xóa tác vụ sao lưu: " + model.name, 0);
                            MessageBox.Show("Xóa không thành công tác vụ sao lưu:" + model.name, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                });
            var DetailBackup = new ConfigDetailBackupViewModel(_folder != null ? _folder : list_backup.FirstOrDefault());
            ContentMenu = listMenubackup;
            ContentDetail = DetailBackup;
        }
        public void FolderConfigView()
        {
            ListBackup();
            //GetService();
            //foreach (var item in _settings.AppSettings)
            //{
            //    if (string.IsNullOrWhiteSpace(item.StorageConnectionString))
            //    {
            //        MessageBox.Show("Data null", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}
            //var listAppSetting = new ConfigDetailBackupViewModel(_settings.Folders.Where(x => x.Id > 0).Count() > 0 ? _settings.Folders.Where(x => x.Id > 0).FirstOrDefault() : _settings.Folders.FirstOrDefault());
            //_folder = _settings.Folders.Where(x => x.Id > 0).FirstOrDefault();
            //ContentMenu = ListMenuBackup;
            //ContentDetail = listAppSetting;
        }
        public void BackupDetaiSelect(backup_bytesave folder)
        {
            var vm = new ConfigDetailBackupViewModel(folder);
            _folder = folder;
            ContentDetail = vm;
        }
        public void AddItemBakup()
        {
            //var list_data = new MainUtility().get_list_data_connect();
            var vm = new AddConfigBackupViewModel(_settings);
            Observable.Merge(
                vm.Ok)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        var maxId = 0;
                        if (_settings.backup_bytesaves.Count == 0)
                            maxId = 1;
                        else maxId = _settings.backup_bytesaves.Max(f => f.id);

                        model.id = maxId + 1;
                        _settings.backup_bytesaves.Add(model);
                        new MainUtility().WriteConfig(_settings);
                        new MainUtility().AddHistory("Sao lưu", "Thêm mới tác vụ sao lưu: " + model.name, 1);
                        MessageBox.Show("Thêm mới thành công tác vụ sao lưu:" + model.name, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        new MainUtility().create_backup_to_server(0, model.id_connect_bytesave, model.name, model.local_path, model.container_name, model.email, model.time_delete, model.time_delete_file_in_LastVersion, model.time, model.is_folder, _email_loggin);

                        //var is_create = new MainUtility().create_backup_to_server(0,model.id_connect_bytesave,model.name,
                        //    model.local_path,model.container_name,model.email,model.time_delete,model.time_delete_file_in_LastVersion,model.time,model.is_folder);
                        //var maxId = 0;
                        //if (_settings.Folders.Count == 0)
                        //    maxId = 1;
                        //else maxId = _settings.Folders.Max(f => f.Id);

                        //model.Id = maxId + 1;
                        //_settings.Folders.Add(model);
                        //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => ListMenuBackup.Items.Add(model)
                        //));
                        ListBackup();
                        //new MainUtility().WriteConfig(_settings);
                        //var Folder = new ConfigDetailBackupViewModel(model);
                        //ContentDetail = Folder;
                        //ContentMenu = ListMenuBackup;
                        //    if (ListMenuRestore.Departments.Count == 0 || ListMenuRestore.Departments.FirstOrDefault(x => x.IdAppsetting == model.IdAppSetting).LstContainer.FirstOrDefault(x => x.Id == model.Id) == null)
                        //    {
                        //        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => AddContainerRestore(model, true)
                        //));
                        //    }
                        _folder = model;
                        //new MainUtility().AddLog("Sao lưu", "Thêm mới tác vụ kết nối: " + model.name, 1);
                        //  _Listlogs = new ObservableCollection<LogContent>(new MainUtility().LoadConfig().Logs.LogContents);
                    }
                    else
                    {
                        ContentDetail = vm;
                    }
                });
            Observable.Merge(
               vm.Cancel)
               .Subscribe(model =>
               {
                   ContentDetail = new ConfigDetailBackupViewModel(_folder);
               });
            ContentDetail = vm;
        }
        public void ListContainer()
        {
            //var list_connect = new MainUtility().get_list_data_connect().data;
            //var list_backup = new MainUtility().get_list_data_backup().data;

            var listMenuContainer = new ListMenuRestoreViewModel(_settings.connect_bytesaves, _settings.backup_bytesaves);
            Observable.Merge(
                        listMenuContainer.Detail)
                        .Subscribe(model =>
                        {
                            if (model != null)
                            {
                                ContainerSelect(model, _settings.connect_bytesaves);
                            }
                        });
            var list = new ListFolderRestoreViewModel();
            ContentDetail = list;
            ContentMenu = listMenuContainer;
        }
        public void AddContainerRestore(FolderConfig folder, bool IsAddContainer)
        {
            ListContainer();
            //var ketNoi = ListMenuRestore.Departments.FirstOrDefault(x => x.IdAppsetting == folder.IdAppSetting);
            //var container = ketNoi == null ? 0 : ketNoi.LstContainer.Where(x => x.NameContainer == folder.ContainerName).Count();
            //if (IsAddContainer == true)
            //{
            //    if (ketNoi == null || container == 0)
            //    {
            //        ketNoi.LstContainer.Add(new Container() { NameContainer = folder.ContainerName, Id = folder.Id });
            //    }
            //}
            //else
            //{
            //    if (container > 0)
            //    {
            //        ketNoi.LstContainer.Remove(ketNoi.LstContainer.FirstOrDefault(x => x.Id == folder.Id));
            //    }
            //}

            //ListMenuRestore.Departments.Remove(ketNoi);
            //ListMenuRestore.Departments.Add(ketNoi);
        }
        public void EditContainer(FolderConfig folder)
        {
            ListContainer();
            //var ketNoi = ListMenuRestore.Departments.FirstOrDefault(x => x.IdAppsetting == folder.IdAppSetting);
            //var container = ketNoi.LstContainer.FirstOrDefault(x => x.Id == folder.Id);

            //if (container != null)
            //{
            //    ketNoi.LstContainer.Remove(container);
            //}
            //ketNoi.LstContainer.Add(new Container() { NameContainer = folder.ContainerName, Id = folder.Id });
            //ListMenuRestore.Departments.Remove(ketNoi);
            //ListMenuRestore.Departments.Add(ketNoi);
        }
        public void EditItemBackup(backup_bytesave folder)
        {
            //var list_connect = new MainUtility().get_list_data_connect().data;
            var vm = new EditConfigBackupViewModel(folder, _settings.connect_bytesaves);
            Observable.Merge(
                vm.Ok)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        //var is_create = new MainUtility().create_backup_to_server(model.id, model.id_connect_bytesave, model.name,
                        //    model.local_path, model.container_name, model.email, model.time_delete, model.time_delete_file_in_LastVersion, model.time, model.is_folder);

                        if (folder != null)
                        {
                            folder.id_connect_bytesave = model.id_connect_bytesave;
                            folder.name = model.name;
                            folder.local_path = model.local_path;
                            folder.container_name = model.container_name;
                            folder.email = model.email;
                            folder.time_delete = model.time_delete;
                            folder.time_delete_file_in_LastVersion = model.time_delete_file_in_LastVersion;
                            folder.time = model.time;
                            folder.is_folder = model.is_folder;
                            folder.time_update_at = model.time_update_at;
                            new MainUtility().WriteConfig(_settings);
                        }
                        new MainUtility().AddHistory("Sao lưu", "Chỉnh sửa tác vụ sao lưu: " + model.name, 1);
                        MessageBox.Show("Chỉnh sửa thành công tác vụ sao lưu:" + model.name, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ListBackup();
                        new MainUtility().create_backup_to_server(model.id, model.id_connect_bytesave, model.name, model.local_path, model.container_name, model.email, model.time_delete, model.time_delete_file_in_LastVersion, model.time, model.is_folder, _email_loggin);
                        //_folder = folder;
                        //    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => ListMenuBackup.Items.Remove(ListMenuBackup.Items.FirstOrDefault(x => x.Id == folder.Id))
                        //));
                        //  Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => ListMenuBackup.Items.Add(model)
                        //));
                        //ContentMenu = ListMenuBackup;
                        //ContentDetail = new ConfigDetailBackupViewModel(model);
                        //App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        //{
                        //    // EditContainer(model);
                        //    new MainUtility().AddLog("Sao lưu", "Chỉnh sửa tác vụ sao lưu: " + model.name, 1);
                        //});



                        _folder = model;
                        //_Listlogs = new ObservableCollection<LogContent>(new MainUtility().LoadConfig().Logs.LogContents);
                    }
                });
            Observable.Merge(
                vm.Cancel)
                .Subscribe(model =>
                {
                    ContentDetail = new ConfigDetailBackupViewModel(folder);
                });

            ContentDetail = vm;
        }
        public void ContainerSelect(backup_bytesave folder, List<connect_bytesave> connects)
        {
            ContentDetail = new ListFolderRestoreViewModel(folder, connects);
        }
        public void AutoConnect()
        {
            NLogManager.LogInfo("AutoConnect");
            CloudStorageAccount storageAccount;

            //foreach (var appSettiing in _settings.AppSettings)
            //{
            //    if (CloudStorageAccount.TryParse(appSettiing.StorageConnectionString, out storageAccount))
            //    {
            //        appSettiing.LastCheck = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //        appSettiing.CheckConnectTrue = "0.8";
            //        appSettiing.CheckConnectFalse = "0";
            //        new MainUtility().WriteConfig(_settings);
            //    }
            //    else
            //    {
            //        appSettiing.LastCheck = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //        appSettiing.CheckConnectTrue = "0";
            //        appSettiing.CheckConnectFalse = "0.8";
            //        new MainUtility().WriteConfig(_settings);
            //    }
            //}

        }
        public void CheckConnect(connect_bytesave appSettiing)
        {
            CloudStorageAccount storageAccount;

            if (CloudStorageAccount.TryParse(appSettiing.metric_service_information_connect, out storageAccount))
            {
                MessageBox.Show("Kết nối thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                //appSettiing. = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                //appSettiing.CheckConnectTrue = "0.8";
                //appSettiing.CheckConnectFalse = "0";
                //new MainUtility().WriteConfig(_settings);
                _connect_bytesave = appSettiing;
                ListConnect();
            }
            else
            {
                MessageBox.Show("Kết nối không thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                //var aaaa = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                //appSettiing.CheckConnectTrue = "0";
                //appSettiing.CheckConnectFalse = "0.8";
                //appSettiing.LastCheck = aaaa.ToString();
                //new MainUtility().WriteConfig(_settings);
                _connect_bytesave = appSettiing;
                ListConnect();

            }
        }

        public void ListConnect()
        {
            Load_config();
            //var list_connect = new MainUtility().get_list_data_connect();
            var list_connect = _settings.connect_bytesaves;
            if (list_connect == null)
            {
                var listMenu_null = new ConfigMenuViewModel(null);
                var listAppSetting_null = new ConfigDetailViewModel(null);
                ContentMenu = listMenu_null;
                ContentDetail = listAppSetting_null;
            }
            _list_connect_bytesave = list_connect;
            _connect_bytesave = list_connect.FirstOrDefault();

            var listMenu = new ConfigMenuViewModel(_list_connect_bytesave);
            // Detail kết nối[Kết nôí]
            Observable.Merge(
                listMenu.Detail)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        AppSettingDetaiSelect(model);
                    }
                });
            Observable.Merge(
                listMenu.Test)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        CheckConnect(model);
                    }
                });
            Observable.Merge(
                listMenu.Delete)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        //var CheckBackup = _settings.backup_bytesaves.FirstOrDefault(x => x.id_connect_bytesave == model.id);
                        //if (CheckBackup != null)
                        //{
                        //    MessageBox.Show("Kết nối đang có tác vụ sao lưu sử dụng, không thể xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        //    return;
                        //}
                        //var is_del_connect = new MainUtility().del_connect_from_agent(model.id);
                        //if (is_del_connect == false)
                        //{
                        //    return;
                        //}
                        //if (is_del_connect == true)
                        //{
                        //    MessageBox.Show("Xóa thành công kết nối " + model.name, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        //}
                        //var is_del = new MainUtility().del_connect_from_agent(model.id);

                        //_settings.AppSettings.Remove(model);
                        //_s
                        //new MainUtility().WriteConfig(_settings);
                        ////new MainUtility().AddLog("Kết nối", "Xóa kết nối: " + model.name, 1);
                        //_appsetting = _settings.AppSettings.FirstOrDefault();

                        ListConnect();
                    }
                });

            var listAppSetting = new ConfigDetailViewModel(_connect_bytesave == null ? _list_connect_bytesave.FirstOrDefault() : _connect_bytesave);
            ContentMenu = listMenu;
            ContentDetail = listAppSetting;
        }

        public void EditItemConfigView(connect_bytesave appSetting)
        {
            var appSettiing = appSetting == null ? _connect_bytesave : appSetting;
            var vm = new EditConfigAppSettingViewModel(appSettiing);
            Observable.Merge(
                vm.Ok)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        if (appSettiing != null)
                        {
                            //var id_service = System.Configuration.ConfigurationSettings.AppSettings["id_service"];
                            appSettiing.name = model.name;
                            appSettiing.time_update_at = model.time_update_at;

                            new MainUtility().WriteConfig(_settings);
                            MessageBox.Show("Chỉnh sửa thành công kết nối " + model.name, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                            new MainUtility().create_connect_to_server(appSettiing.id, "0", "0", "0", "0", model.name, _email_loggin);

                            //if (is_create_connect == true)
                            //{
                            //    MessageBox.Show("Chỉnh sửa thành công kết nối " + model.name, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            //    new MainUtility().save_log_agent("Chỉnh sửa thành công  kết nối " + model.name, "Kết nối",1, 1);
                            //}
                        }
                        _connect_bytesave = appSettiing;
                        ListConnect();
                        //   ContentDetail = new ConfigDetailViewModel(model);
                        //App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        //{
                        //    //var a = ListMenuApp.Items.FirstOrDefault(x => x.IdAppSetting == appSettiing.IdAppSetting);
                        //    //  a.AccountName = model.AccountName;
                        //    //  a.NameAppSetting = model.NameAppSetting;
                        //    //  a.UpdateTime = model.UpdateTime;
                        //    //  a.LastCheck = model.LastCheck;
                        //    //  a.StorageConnectionString = model.StorageConnectionString;
                        //    ListMenuApp.Items.Remove(ListMenuApp.Items.FirstOrDefault(x => x.IdAppSetting == appSettiing.IdAppSetting));
                        //    ListMenuApp.Items.Add(appSettiing);
                        //});
                        //new MainUtility().WriteConfig(_settings);
                        ////  ContentMenu = ListMenuApp;
                        ////   _Listlogs = new ObservableCollection<LogContent>(new MainUtility().LoadConfig().Logs.LogContents);
                        //new MainUtility().AddLog("Kết nối", "Chỉnh sửa kết nối: " + model.name, 1);
                    }
                });

            Observable.Merge(
              vm.Cancel)
              .Subscribe(model =>
              {
                  ContentDetail = new ConfigDetailViewModel(appSettiing);
              });

            ContentDetail = vm;
        }
        public void AddItemConfigView()
        {
            var id_service = System.Configuration.ConfigurationSettings.AppSettings["id_service"];
            var vm = new AddConfigAppSettingViewModel();
            Observable.Merge(
                vm.Ok)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        // var is_create_connect = new MainUtility().create_connect_to_server(0, id_service, model.metric_service_information_connect, model.metric_service_max_storage, model.metric_service_username_account, model.name);
                        //var is_create_connect = new MainUtility().create_connect_to_server(0, id_service, model.metric_service_information_connect, model.metric_service_max_storage, model.metric_service_username_account, model.name);
                        //if (is_create_connect == true)
                        //{
                        //    MessageBox.Show("Thêm mới thành công kết nối " + model.name, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        //    new MainUtility().save_log_agent("Thêm mới thành công  kết nối " + model.name, "Kết nối",1, 1);
                        //}

                        //var maxId = 0;
                        //if (_settings.AppSettings.Count == 0)
                        //    maxId = 1;
                        //else maxId = _settings.AppSettings.Max(f => f.IdAppSetting);
                        //model.IdAppSetting = maxId + 1;
                        //_settings.AppSettings.Add(model);
                        //_appsetting = model;

                        //new MainUtility().WriteConfig(_settings);
                        CheckConnect(model);
                        _connect_bytesave = model;
                        ListConnect();
                        //  ListMenuRestore.Departments.Add(new KetNoi { IdAppsetting = model.IdAppSetting, NameAppsetting = model.NameAppSetting });
                        //   _Listlogs = new ObservableCollection<LogContent>(new MainUtility().LoadConfig().Logs.LogContents);
                        //new MainUtility().AddLog("Kết nối", "Thêm mới kết nối: " + model.NameAppSetting, 1);

                    }
                });
            Observable.Merge(
            vm.Cancel)
            .Subscribe(model =>
            {
                ContentDetail = new ConfigDetailViewModel(_list_connect_bytesave.FirstOrDefault());
            });

            ContentDetail = vm;
        }
        public void AppSettingDetaiSelect(connect_bytesave appsetting)
        {
            var vm = new ConfigDetailViewModel(appsetting);
            //_appsetting = appsetting;
            _connect_bytesave = appsetting;
            ContentDetail = vm;
        }
        public void AppSettingConfigView()
        {

            ListConnect();
            //GetService();
            //var abc = _settings.AppSettings.Where(x => x.IdAppSetting > 0).Count();
            //var listAppSetting = new ConfigDetailViewModel(abc > 0 ? _settings.AppSettings.Where(x => x.IdAppSetting > 0).FirstOrDefault() : _settings.AppSettings.FirstOrDefault());
            //_appsetting = abc > 0 ? _settings.AppSettings.Where(x => x.IdAppSetting > 0).FirstOrDefault() : _settings.AppSettings.FirstOrDefault();
            //ContentMenu = ListMenuApp;
            //ContentDetail = listAppSetting;
        }
        public void InfoView()
        {
            try
            {
                ContentDetail = new InfoViewModel(_settings, _email_loggin);
                ContentMenu = null;
            }
            catch (Exception ex)
            {
                NLogManager.LogError("Info " + ex.ToString());
            }
        }
        public void SettingView()
        {
            Load_config();
            ContentDetail = new SettingViewModel(_settings, _email_loggin);
            ContentMenu = null;
        }
        public void HistoryView()
        {
            ContentDetail = new HistoryViewModel();
            ContentMenu = null;
        }
        public void GetService()
        {
            try
            {
                //WriteToFile("Restart: " + _options.CurrentValue.NameService);
                ServiceController sc = new ServiceController("TransferWorker");
                if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
                     (sc.Status.Equals(ServiceControllerStatus.StopPending)))
                {
                    // Start the service if the current status is stopped.
                    sc.Start();
                    // Refresh and display the current service status.
                    sc.Refresh();
                    NLogManager.LogInfo("Restart service");
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogInfo("Error service" + ex.ToString());
            }
        }
        public void InstallMT()
        {
            var localFilePath2 = Path.GetFullPath("SetupWorker.bat");
            System.Diagnostics.Process.Start(localFilePath2);
            new MainUtility().WriteAppjson("1", 1);

        }
        public Companys GetLicense(string license)
        {
            int parameter = 1;
            string[] name = new string[parameter];
            object[] values = new object[parameter];
            name[0] = "@license"; values[0] = license;
            //name[1] = "@mac"; values[1] = mac;
            var json = JsonConvert.SerializeObject(new Connection().LoadDataParameter("LoadCompany", name, values, parameter));
            var Company = JsonConvert.DeserializeObject<List<Companys>>(json);
            return Company.FirstOrDefault();
        }

        public void Load_config()
        {
            var appjson = new MainUtility().LoadConfig();
            _settings = appjson.Settings;
            _email_loggin = appjson.email_loggin;
        }
    }
}
