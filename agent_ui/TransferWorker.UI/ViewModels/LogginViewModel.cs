using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace TransferWorker.UI.ViewModels
{
   public class LogginViewModel
    {
        ICommand btnloggin { get; set; }
        public LogginViewModel()
        {

            this.btnloggin = new Models.DelegateCommand(o => this.loggin());
        }
        public void loggin()
        {

        }
    }
}
