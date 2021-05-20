using System;
using System.Collections.Generic;
using System.Net;
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
    /// Interaction logic for ListFolderRestoreView.xaml
    /// </summary>
    public partial class ListFolderRestoreView : UserControl
    {
        public ListFolderRestoreView()
        {
            
                InitializeComponent();
            
        }
        public async void Browse_Clicked(object sender, RoutedEventArgs args)
        {
            var context = this.DataContext as ListFolderRestoreViewModel;
            //string _path = await GetPath(context.LocalFolderPath);
            //context.LocalFolderPath = _path;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;

                context.PathRestore = path;
            }

        }
        
    }
}
