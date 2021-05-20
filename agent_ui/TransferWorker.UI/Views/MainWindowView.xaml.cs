using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using TransferWorker.UI.ViewModels;

namespace TransferWorker.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
            ApplyLanguage();
        }
        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
        }
        private void ApplyLanguage(string cultureName = null)
        {
            if (cultureName != null)
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);

            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri("..\\Lang\\VN.xaml", UriKind.Relative);
           // dict.Source = new Uri("..\\Lang\\EL.xaml", UriKind.Relative);
            //switch (Thread.CurrentThread.CurrentCulture.ToString())
            //{
            //    case "vi-VN":
            //        dict.Source = new Uri("..\\Lang\\VN.xaml", UriKind.Relative);
            //        break;
            //    // ...
            //    default:
            //        dict.Source = new Uri("..\\Lang\\EL.xaml", UriKind.Relative);
            //        break;
            //}
            this.Resources.MergedDictionaries.Add(dict);
        }
    }
}
