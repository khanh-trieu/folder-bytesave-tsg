
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TransferWorker.UI.Views
{
    /// <summary>
    /// Interaction logic for ConfigDetaiBackupView.xaml
    /// </summary>
    public partial class ConfigDetaiBackupView : UserControl
    {
        public ConfigDetaiBackupView()
        {
            InitializeComponent();
        }
        public async Task<string> GetPath(string path)
        {
            //var dialog = new OpenFolderDialog();
            //dialog.Directory = path;
            //var result = await dialog.ShowAsync((Window)(Parent.GetVisualRoot()));
            //return result;
            return null;
        }

        public async void Browse_Clicked(object sender, RoutedEventArgs args)
        {
            //var context = this.DataContext as EditItemViewModel;
            ////string _path = await GetPath(context.LocalFolderPath);
            ////context.LocalFolderPath = _path;
            //var dialog = new System.Windows.Forms.FolderBrowserDialog();
            //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string path = dialog.SelectedPath;

            //    context.LocalFolderPath = path;
            //}
            
        }
    }
}
