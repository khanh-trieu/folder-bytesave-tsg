using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace TransferWorker.UI.Utility
{
   public class UIExportViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ExportFactory<IView>)value).CreateExport().Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
