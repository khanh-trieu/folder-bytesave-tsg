using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.ServiceProcess;
using TransferWorker.UI.Utility;
using TransferWorker.Utility;
using System.Windows;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Linq;
using TransferWorker.UI.Models;

namespace TransferWorker.UI.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        private string host;
        public string Host
        {
            get => host;
            set => this.RaiseAndSetIfChanged(ref host, value);
        }
        private string port;
        public string Port
        {
            get => port;
            set => this.RaiseAndSetIfChanged(ref port, value);
        }
        private string email;
        public string Email
        {

            get => email;
            set
            {
                this.email = value;
                this.RaisePropertyChanged();
            }
        }
        private string pwd;
        public string Pwd
        {
            get => pwd;
            set => this.RaiseAndSetIfChanged(ref pwd, value);
        }
        private string isOK;
        public string IsOK
        {
            get => isOK;
            set => this.RaiseAndSetIfChanged(ref isOK, value);
        }
        private string subject;
        public string Subject
        {
            get => subject;
            set => this.RaiseAndSetIfChanged(ref subject, value);
        }
        private bool isInput;
        public bool IsInput
        {
            get => isInput;
            set => this.RaiseAndSetIfChanged(ref isInput, value);
        }
        private string contentTest;
        public string ContentTest
        {
            get => contentTest;
            set => this.RaiseAndSetIfChanged(ref contentTest, value);
        }
        private string emailTest;
        public string EmailTest
        {
            get => emailTest;
            set => this.RaiseAndSetIfChanged(ref emailTest, value);
        }
        private string textError;
        public string TextError
        {
            get => textError;
            set
            {
                this.RaiseAndSetIfChanged(ref textError, value);
            }
        }
        private string textSuccess;
        public string TextSuccess
        {
            get => textSuccess;
            set => this.RaiseAndSetIfChanged(ref textSuccess, value);
        }
        public Settings _setting;
        public ICommand btnEdit { get; private set; }
        public ICommand btnOK { get; private set; }
        public ICommand btnCancel { get; private set; }
        public ICommand btnTestEmail { get; private set; }
        public int id_mail = 0;
        public int id_agent = 0;
        public string _email_loggin;
        public SettingViewModel(Settings settings,string email_loggin)
        {
            _setting = settings;
            _email_loggin = email_loggin;
            this.btnEdit = new Models.DelegateCommand(o => this.Edit());
            this.btnOK = new Models.DelegateCommand(o => this.OK());
            this.btnCancel = new Models.DelegateCommand(o => this.Cancel());
            this.btnTestEmail = new Models.DelegateCommand(o => this.TestSendMail());
            //var setting = new MainUtility().get_setting_to_server() ;
            var app = settings.bytesave_setting.set_bytesave;
            IsOK = "Hidden";
            IsInput = false;
            Host = app.server_mail;
            Port = app.port;
            Email = app.mail_send;
            Pwd = app.mail_send_pwd != null ? new MainUtility().Base64Decode(app.mail_send_pwd): "";
            Subject = app.subject;
            id_mail = app.id;
            id_agent = app.id_agent;
        }
        public void Edit()
        {
            IsOK = "Visible";
            IsInput = true;
        }
        public void OK()
        {
            if(Email == "" && Host == "" && Port == "")
            {
                IsOK = "Hidden";
                IsInput = false;
                return;
            }
            if (Email == "")
            {
                MessageBox.Show("Bạn chưa nhập email!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            if (regex.IsMatch(Email) == false)
            {
                MessageBox.Show("Mail nhập sai định dạng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Host == "")
            {
                MessageBox.Show("Bạn chưa nhập server mail!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Port == "")
            {
                MessageBox.Show("Bạn chưa nhập port!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Pwd == "")
            {
                MessageBox.Show("Bạb chưa nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            IsOK = "Visible";
            IsInput = true;
            //new TransferWorker.UI.Utility.MainUtility().WriteServerMail(Host,Port,Email,Pwd,Subject);
            //var setting = new MainUtility().get_setting_to_server().data;
            //var setting_mail = setting.FirstOrDefault();
            //id_mail = setting.Count() == 0 ? 0 : setting_mail.id;
            
            var setting_mail = _setting.bytesave_setting.set_bytesave;
            setting_mail.server_mail = Host;
            setting_mail.mail_send = Email;
            setting_mail.port = Port;
            setting_mail.mail_send_pwd = new MainUtility().Base64Encode(Pwd);
            setting_mail.subject = Subject;
            setting_mail.id = id_mail;
            setting_mail.id_agent = id_agent;
            new MainUtility().WriteConfig(_setting);
            MessageBox.Show("Chỉnh sửa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            new MainUtility().create_setting_to_server(id_mail,id_agent, _email_loggin, Host, Port, Email, new MainUtility().Base64Encode(Pwd), Subject);
            
            IsOK = "Hidden";
            IsInput = false;
        }
        public void Cancel()
        {
            //var setting = new MainUtility().get_setting_to_server().data;
            var setting_mail = _setting.bytesave_setting.set_bytesave;

            //var app = new MainUtility().LoadServerMail();

            IsOK = "Hidden";
            IsInput = false;
            Host = setting_mail.server_mail;
            Port = setting_mail.port;
            Email = setting_mail.mail_send;
            Pwd = new MainUtility().Base64Decode(setting_mail.mail_send_pwd);
            Subject = setting_mail.subject;
        }
        public void CheckService()
        {

        }
        public void TestSendMail()
        {
            try
            {
                if (EmailTest == null)
                {
                    //MessageBox.Show("Bạn vui lòng nhập Email!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    //  MessageBox.Show(Application.Current.MainWindow, "Can't login with given names.", "Login Failure", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Cancel);

                    TextError = "Please enter your email address!";
                    TextSuccess = "";
                    return;
                }
                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
                if (regex.IsMatch(EmailTest) == false)
                {
                    //  MessageBox.Show("Sai định dạng email! Vui lòng nhập lại", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    TextError = "Wrong email format! Please re-enter!";
                    TextSuccess = "";
                    return;
                }

                var TestSendMail = new SendMail().SendMailTSG(EmailTest, Email, Pwd, Host, Port, ContentTest, "Test Mail");

                if (TestSendMail.Contains("OK"))
                {
                    // MessageBox.Show("Gửi mail thành công!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    TextError = "";
                    TextSuccess = "Sent email success!";
                }
                else
                {
                    //MessageBox.Show("Gửi mail không thành công! Bạn vui lòng kiểm tra lại tất cả thông tin", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TextError = "Sent email failed! Please check all the information";
                    TextSuccess = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mail sent failed!" + ex.ToString(), "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
