using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TransferWorker.UI.ViewModels;

namespace TransferWorker.UI.Views
{
    /// <summary>
    /// Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();
            var app = new TransferWorker.UI.Utility.MainUtility().LoadConfig().Settings.bytesave_setting.set_bytesave;
            passwordBox.Password = app.mail_send_pwd;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as SettingViewModel;
            if (context != null)
            {
                context.Pwd = passwordBox.Password;
            }
        }

        private void CoolButton_Click(object sender, RoutedEventArgs e)
        {
            // InputBox.Visibility = System.Windows.Visibility.Visible;
            myPopup.IsOpen = true;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            //InputBox.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void DisplayPopup(object sender, RoutedEventArgs e)
        {
          //  myPopup.IsOpen = true;
        }

        private void NoButton1_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = false;
            error.Text = "";
            success.Text = "";
        }
    }
}
