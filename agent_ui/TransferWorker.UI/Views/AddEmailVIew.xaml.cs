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
using System.Windows.Shapes;
using TransferWorker.UI.ViewModels;

namespace TransferWorker.UI.Views
{
    /// <summary>
    /// Interaction logic for AddEmailVIew.xaml
    /// </summary>
    public partial class AddEmailVIew : Window
    {
        public AddEmailVIew()
        {
            InitializeComponent();
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as AddConfigBackupViewModel;
            context.Email = txtEmail.Text;
        }
    }
}
