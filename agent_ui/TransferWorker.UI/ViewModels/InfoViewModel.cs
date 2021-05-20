using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;

namespace TransferWorker.UI.ViewModels
{
    public class InfoViewModel : ViewModelBase
    {
        private AppJson App;
        private string isbtnUpdate;
        public string IsbtnUpdate
        {
            get => isbtnUpdate;
            set => this.RaiseAndSetIfChanged(ref isbtnUpdate, value);
        }
        private string isUpdate;
        public string IsUpdate
        {
            get => isUpdate;
            set => this.RaiseAndSetIfChanged(ref isUpdate, value);
        }
        private string license;
        public string License
        {
            get => license;
            set => this.RaiseAndSetIfChanged(ref license, value);
        }
        private string dateEnd;
        public string DateEnd
        {
            get => dateEnd;
            set => this.RaiseAndSetIfChanged(ref dateEnd, value);
        }
        private string version_bytesave;
        public string Version_bytesave
        {
            get => version_bytesave;
            set => this.RaiseAndSetIfChanged(ref version_bytesave, value);
        }
        Settings _setting;
        public InfoViewModel(Settings settings,string email_loggin)
        {
            try
            {
                _setting = settings;
                //var info_to_server = new MainUtility().get_info_to_server();

                IsUpdate = "Hidden";
                IsbtnUpdate = "Hidden";
               // this.btnUpdateLicense = new Models.DelegateCommand(o => this.UpdateLicense());
                //App = new MainUtility().LoadConfig();
                //var comp =GetLicense(App.license);
                ////DateEnd = DateTime.Parse(new MainUtility().DecryptGenLicense(App.license)).ToString("dd/MM/yyyy");
                //var datee = DateTime.Parse(comp.ExpirationTime).ToString("dd/MM/yyyy");

                DateEnd = new DateTime(1970, 1, 1, 7, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(_setting.bytesave_info.information.bytesave_expiration_date).ToLocalTime().ToString("dd/MM/yyyy");
                Version_bytesave = "ByteSave Backup -- "+ _setting.bytesave_info.information.name_version;
            }
            catch (Exception ex)
            {
                IsbtnUpdate = "Visible";
                new MainUtility().save_log_agent(ex.ToString(), "InfoViewModel", 0, 0, email_loggin);

            }
            
            //if (comp.MACAddress.Trim() == mac)
            //{
            //    DateEnd = DateTime.Parse(comp.ExpirationTime).ToString("dd/MM/yyyy");

            //    if (DateTime.Parse(comp.ExpirationTime) <= DateTime.Now.AddDays(5))
            //    {
            //        IsbtnUpdate = "Visible";
            //    }
            //}
            //else
            //{
            //    IsbtnUpdate = "Visible";
            //}
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
        public void UpdateLicense()
        {
            if (License == null)
            {
                System.Windows.MessageBox.Show("Bạn chưa nhập mã bản quyền!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                string mac = new MainUtility().GetMac();
                int parameter = 1;
                string[] name = new string[parameter];
                object[] values = new object[parameter];
                name[0] = "@license"; values[0] = License;
                //name[1] = "@mac"; values[1] = mac;

                var json = JsonConvert.SerializeObject(new Connection().LoadDataParameter("LoadCompany", name, values, parameter));
                var Company = JsonConvert.DeserializeObject<List<Companys>>(json);
                if( Company.FirstOrDefault().DaDung >= Company.FirstOrDefault().SoLuong)
                {
                    System.Windows.MessageBox.Show("Vượt quá số lượng máy đã sử dụng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                var i = Company.FirstOrDefault().DaDung + 1;
                var lstMac = Company.FirstOrDefault().MACAddress.Split(",");
                if(Array.Exists(lstMac, element => element.Trim() == mac.Trim())){
                    System.Windows.MessageBox.Show("ByteSave đang sử dụng mã bản quyền này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                UpdateLicensetoData(i);
                //  var lis = new MainUtility().DecryptGenLicense(App.license);
                // int hasdcode = new MainUtility().GetHash(mac + License);

                var datee = DateTime.Parse(Company.FirstOrDefault().ExpirationTime).ToString("dd/MM/yyyy");
                DateEnd = datee;
                IsUpdate = "Hidden";
                new MainUtility().AddLog("Thông báo", "Cập nhật bản quyền đến: " + DateEnd, 1);
                new MainUtility().WriteAppjson(License, 0);
                System.Windows.MessageBox.Show("Cập nhật bản quyền thành công! Bạn làm ơn tắt và khởi động lại phần mềm để tiếp tục sử dụng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Mã bản quyền không hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public void UpdateLicensetoData(int i)
        {
            string mac = new MainUtility().GetMac();
            int parameter = 3;
            string[] name = new string[parameter];
            object[] values = new object[parameter];
            name[0] = "@license"; values[0] = License;
            name[1] = "@mac"; values[1] = mac.Trim();
            name[2] = "@dadung"; values[2] = i;
            new Connection().Execute("UpdateLicense", name, values, parameter);
        }

        public ICommand btnUpdateLicense { get; private set; }
    }
}
