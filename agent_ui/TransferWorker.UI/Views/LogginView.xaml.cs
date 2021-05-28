using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Management;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Windows.Forms;
using NetCore.Utils.Log;
using System.Diagnostics;
using TransferWorker.UI.Utility;
using System.ServiceProcess;
using System.ComponentModel;

namespace TransferWorker.UI.Views
{
    /// <summary>
    /// Interaction logic for LogginView.xaml
    /// </summary>
    /// 
    public partial class LogginView : Window
    {
        public LogginView()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InstallMT();
            DoWorkAsync();
            // check_is_logged();
            //check_logged_bytesaveAsync();
            InitializeComponent();
        }
        private void check_is_logged()
        {
            try
            {
                var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
                string api_check_logged = System.Configuration.ConfigurationSettings.AppSettings["api_check_logged"];
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_check_logged + Get_Serial_number());
                WebReq.Method = "GET";

                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    JObject json = JObject.Parse(jsonString);
                    var msg = json["msg"].ToString();
                    var status = json["status"].ToString();
                    if (status == "none")
                    {
                        this.Show();
                        return;
                    }
                    if (status == "true")
                    {
                        new MainUtility().WriteConfig_Loggin();
                        MainWindowView objPopupwindow = new MainWindowView();
                        objPopupwindow.Show();
                        this.Close();
                        //System.Windows.Forms.MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                NLogManager.LogError("[check_is_logged] -> " + ex);
                this.Show();
                return;
            }
        }
        public async Task<bool> check_logged_bytesaveAsync()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
                string api_check_logged = System.Configuration.ConfigurationSettings.AppSettings["api_check_logged"];
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + api_check_logged + Get_Serial_number());
                WebReq.Method = "GET";
                WebReq.Timeout = 10000;

                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                    JObject json = JObject.Parse(jsonString);
                    var msg = json["msg"].ToString();
                    var status = json["status"].ToString();
                    if (status == "none")
                    {
                        return false;
                        this.Show();
                    }
                    if (status == "true")
                    {
                        return true;
                        //System.Windows.Forms.MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return false;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return false;
            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }
        //public bool is_indete { get; set; }
        private bool is_indete;

        public bool Is_indete
        {
            get => is_indete;
            set => this.OnPropertyChanged(ref is_indete, value);
        }
        private string nameValue;

        public string Name
        {
            get { return nameValue; }
            set { nameValue = value; }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }
        private void OnPropertyChanged(ref bool is_indete, bool value)
        {
            throw new NotImplementedException();
        }
        private async Task DoWorkAsync()
        {
            try
            {
                bool isfa = false;
                await Task.Run(() =>
                {
                    //do some work HERE
                    isfa = check_logged_bytesaveAsync().Result;
                });
                if (isfa == true)
                {
                    new MainUtility().WriteConfig_Loggin();
                    MainWindowView objPopupwindow = new MainWindowView();
                    objPopupwindow.Show();
                    this.Close();
                }
                pb.IsIndeterminate = false;
                grid_form.Opacity = 1;
            }
            catch (Exception ex)
            {

                pb.IsIndeterminate = false;
                grid_form.Opacity = 1;
            }
            
        }
        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Name = "khánh";
                if (textBoxEmail.Text.Length == 0)
                {
                    //errormessage.Text = "Bạn chưa nhập Email!";
                    System.Windows.Forms.MessageBox.Show("Bạn chưa nhập Email!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else if (!Regex.IsMatch(textBoxEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                {
                    //errormessage.Text = "Email nhận sai định dạng";
                    System.Windows.Forms.MessageBox.Show("Email nhận sai định dạng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxEmail.Select(0, textBoxEmail.Text.Length);
                    //textBoxEmail.Focus();
                }
                else if (passwordBox1.Password.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Bạn chưa nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    NLogManager.LogError("[textBoxEmail.Text] -> " + textBoxEmail.Text);
                    string email = textBoxEmail.Text;
                    string password = passwordBox1.Password;
                    pb.IsIndeterminate = true;
                    grid_form.Opacity = 0.6;
                    bool isfa = false;
                    await Task.Run(() =>
                    {
                        //do some work HERE
                        isfa = loggin_api(email, password).Result;
                    });
                    if (isfa == true)
                    {
                        new MainUtility().write_email(email);
                        new MainUtility().WriteConfig_Loggin();
                        MainWindowView objPopupwindow = new MainWindowView();
                        objPopupwindow.Show();
                        this.Close();
                    }
                    pb.IsIndeterminate = false;
                    grid_form.Opacity = 1;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Lỗi hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NLogManager.LogError("[Submit_Click] -> " + ex);
                pb.IsIndeterminate = false;
                grid_form.Opacity = 1;
            }
        }
        public async Task<bool> loggin_api(string email, string password)
        {
            var BYTESAVE_API_PBL = System.Configuration.ConfigurationManager.AppSettings["BYTESAVE_API_PBL"];
            string api_loggin = System.Configuration.ConfigurationManager.AppSettings["api_loggin"];
            //HttpResponseMessage response = await client.GetAsync(BYTESAVE_API_PBL + "dang-nhap/" + email + "/" + pwd + "/" + GetOsName() + "/" + Get_Serial_number() + "/" + GetIPAddress() + "/" + GetIPPrivate() + "/" + GetNameCompany());
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create( BYTESAVE_API_PBL + api_loggin + email + "/" + password + "/" + GetOsName() + "/" + Get_Serial_number() + "/" + GetIPAddress() + "/" + GetIPPrivate() + "/" + GetNameCompany());
            NLogManager.LogError("[WebReq] -> " + WebReq.RequestUri.OriginalString.ToString());
            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
                JObject json = JObject.Parse(jsonString);
                var msg = json["msg"].ToString();
                var status = json["status"].ToString();
                if (status == "false")
                {
                    System.Windows.Forms.MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public async Task loggin_bytesaveAsync(string email, string pwd)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
                HttpResponseMessage response = await client.GetAsync(BYTESAVE_API_PBL + "dang-nhap/" + email + "/" + pwd + "/" + GetOsName() + "/" + Get_Serial_number() + "/" + GetIPAddress() + "/" + GetIPPrivate() + "/" + GetNameCompany());
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);
                var a = 0;
                JObject json = JObject.Parse(responseBody);
                var msg = json["msg"].ToString();
                var status = json["status"].ToString();
                if (status == "false")
                {
                    System.Windows.Forms.MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MainWindowView objPopupwindow = new MainWindowView();
                    objPopupwindow.Show();
                    this.Close();

                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }
        private string GetOsName()
        {
            OperatingSystem os_info = System.Environment.OSVersion;
            string version = os_info.Version.Major.ToString() + "." + os_info.Version.Minor.ToString();
            switch (version)
            {
                case "10.0": return "Windows 10";
                case "6.3": return "Windows 8.1";
                case "6.2": return "Windows 8";
                case "6.1": return "Windows 7";
                case "6.0": return "Windows Vista";
                case "5.2": return "Windows XP 64-Bit Edition";
                case "5.1": return "Windows XP";
                case "5.0": return "Windows 2000";
            }
            return "Unknown";
        }
        public string GetNameCompany()
        {
            string hostName = Environment.MachineName;

            return hostName;
        }
        public string GetIPPrivate()
        {
            List<string> ips = new List<string>();

            System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            foreach (System.Net.IPAddress ip in entry.AddressList)
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    ips.Add(ip.ToString());
            return ips[0];
        }
        public string GetIPAddress()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);

            return address;
        }
        public string Get_Serial_number()
        {
            string serial_number = "";
            try
            {
                ManagementObjectSearcher mSearcher = new ManagementObjectSearcher("SELECT SerialNumber, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS");
                ManagementObjectCollection collection = mSearcher.Get();
                foreach (ManagementObject obj in collection)
                {
                    serial_number = (string)obj["SerialNumber"];
                    break;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return serial_number;
        }

        public bool RunAsync()
        {
            using (var client = new HttpClient())
            {
                var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
                var SerialNumber = "";
                ManagementObjectSearcher mSearcher = new ManagementObjectSearcher("SELECT SerialNumber, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS");
                ManagementObjectCollection collection = mSearcher.Get();
                foreach (ManagementObject obj in collection)
                {
                    SerialNumber = (string)obj["SerialNumber"];
                }

                // HTTP GET
                //HttpResponseMessage response = client.GetAsync("sao-luu/danh-sach/" + SerialNumber);
                //var responseTask = client.GetAsync("/dang-nhap/''/''/''/SerialNumber/''/''/''");
                //responseTask.Wait();
                //var result = responseTask.Result;
                //IEnumerable<json_backup_bytesave> students = null;
                //if (response.IsSuccessStatusCode)
                //{
                //    //Product product = await response.Content.ReadAsAsync<Product>();
                //    //Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                //    var jsonstring = response.Content.ReadAsStringAsync();

                //    var readTask = result.Content.ReadAsAsync<IList<json_backup_bytesave>>();
                //    readTask.Wait();

                //    students = readTask.Result;
                //    return true;

                //}




                //// HTTP POST
                //var gizmo = new Product() { Name = "Gizmo", Price = 100, Category = "Widget" };
                //response = await client.PostAsJsonAsync("api/products", gizmo);
                //if (response.IsSuccessStatusCode)
                //{
                //    Uri gizmoUrl = response.Headers.Location;

                //    // HTTP PUT
                //    gizmo.Price = 80;   // Update price
                //    response = await client.PutAsJsonAsync(gizmoUrl, gizmo);

                //    // HTTP DELETE
                //    response = await client.DeleteAsync(gizmoUrl);
                //}
            }
            return true;
        }
        static readonly HttpClient client = new HttpClient();

        private void btn_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public async void InstallMT()
        {
            NLogManager.LogInfo("InstallMT ");
            ServiceController sc = new ServiceController(System.Configuration.ConfigurationSettings.AppSettings["name_service"]);
            try
            {
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        return;
                    case ServiceControllerStatus.Stopped:
                        sc.Start();
                        return;
                }
            }
            catch (Exception e)
            {
                var localFilePath2 = Path.GetFullPath("SetupWorker.bat");
                try
                {
                    ProcessStartInfo procInfo = new ProcessStartInfo();
                    procInfo.UseShellExecute = true;
                    procInfo.FileName = localFilePath2;  //The file in that DIR.
                    procInfo.WorkingDirectory = @""; //The working DIR.
                    procInfo.Verb = "runas";
                    Process.Start(procInfo);  //Start that process.
                }
                catch (Exception ex)
                {
                    NLogManager.LogError("InstallMT " + ex.ToString());
                    new MainUtility().save_log_agent(ex.ToString(), "InstallMT", 0, 0, "");
                }
            }
        }
    }
}
