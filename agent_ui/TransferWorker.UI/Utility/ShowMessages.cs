using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TransferWorker.UI.Utility
{
    public class ShowMessages
    {
        public  ShowMessages(string title, string mes)
        {
            MessageBox.Show(title, mes);
        }
    }
}
