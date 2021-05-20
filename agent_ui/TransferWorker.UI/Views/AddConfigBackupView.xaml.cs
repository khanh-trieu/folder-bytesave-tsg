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
    /// Interaction logic for AddConfigBackupView.xaml
    /// </summary>
    public partial class AddConfigBackupView : UserControl
    {
        public AddConfigBackupView()
        {
            InitializeComponent();
        }
        public async void Browse_Clicked(object sender, RoutedEventArgs args)
        {
            var context = this.DataContext as AddConfigBackupViewModel;
            //string _path = await GetPath(context.LocalFolderPath);
            //context.LocalFolderPath = _path;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                context.IsFolder = true;
                context.LocalFolderPath = path;
            }


        }
        public async void Browse_ClickedFile(object sender, RoutedEventArgs args)
        {
            var context = this.DataContext as AddConfigBackupViewModel;
            //string _path = await GetPath(context.LocalFolderPath);
            //context.LocalFolderPath = _path;
            context.IsFolder = false;
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            openFileDlg.Multiselect = true;
            openFileDlg.Filter = "All files (*.*)|*.*";
            if (result == true)
            {
                string path = openFileDlg.FileName;

                context.LocalFolderPath = path;
            }
        }
        
        private void PresetTimePicker_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            var context = this.DataContext as AddConfigBackupViewModel;
            DateTime selectedTime = PresetTimePicker.SelectedTime.Value;
            string formatted = selectedTime.ToString("HH:mm");
            context.TimeSelect = formatted;

            // var a = PresetTimePicker;
        }
        private void CoolButton_Click(object sender, RoutedEventArgs e)
        {
            // CoolButton Clicked! Let's show our InputBox.
            InputBox.Visibility = System.Windows.Visibility.Visible;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as AddConfigBackupViewModel;
            // YesButton Clicked! Let's hide our InputBox and handle the input text.
            InputBox.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            // NoButton Clicked! Let's hide our InputBox.
            InputBox.Visibility = System.Windows.Visibility.Collapsed;

        }

    }
}
