using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Net;
using System.Management;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Interactivity;
using System.Linq;
using TransferWorker.UI.Utility;

namespace TransferWorker.UI.Views
{
    /// <summary>
    /// Interaction logic for InfoView.xaml
    /// </summary>
    public partial class InfoView : UserControl
    {
        public InfoView()
        {
            InitializeComponent();
        }
        private void toWeb(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://www.tsg.net.vn/",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        //private void NoButton_Click(object sender, RoutedEventArgs e)
        //{
        //    InputBox.Visibility = System.Windows.Visibility.Collapsed;
        //}

        //private void CoolButton_Click(object sender, RoutedEventArgs e)
        //{
        //    InputBox.Visibility = System.Windows.Visibility.Visible;
        //}

        private void btn_logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Khi bạn đăng xuất thì các tác vụ sao lưu sẽ không hoạt động? Bạn có muốn đăng xuất không?", "Thông báo", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    var BYTESAVE_API_PBL = System.Configuration.ConfigurationSettings.AppSettings["BYTESAVE_API_PBL"];
                    HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(BYTESAVE_API_PBL + "dang-xuat/" + Get_Serial_number());
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
                        new MainUtility().write_logout();
                        LogginView window = new LogginView();
                        window.Show();
                        var myWindow = (Window)VisualParent.GetSelfAndAncestors().FirstOrDefault(a => a is Window);
                        myWindow.Close();
                        //System.Windows.Application.Current.Shutdown();
                    }
                    break;
                case MessageBoxResult.No:
                    break;
            }
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
    }
}
