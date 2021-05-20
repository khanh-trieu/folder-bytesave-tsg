using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TransferWorker.UI.Models;
using TransferWorker.UI.Utility;

namespace TransferWorker.UI.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        //private List<log_agent_bytesave> _log;
        public Historys _logs;

        public ObservableCollection<LogContent> Logs { get; }
        public HistoryViewModel()
        {
            try
            {
                var lst_log = new MainUtility().LoadHistory();
                if (lst_log == null)
                {
                    Logs = new ObservableCollection<LogContent>();
                    return;
                }
                _logs = lst_log;
                List<LogContent> logss = new List<LogContent>();
                float time_day30 = (Int32)(DateTime.UtcNow.AddDays(-30).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var item_day30 = _logs.history.history_bytesave.Where(x => x.time_log < time_day30);
                _logs.history.history_bytesave.Remove(item_day30);
                new MainUtility().WriteHistory(_logs.history);
                foreach (var item in _logs.history.history_bytesave)
                {
                    logss.Add(new LogContent
                    {
                        Content = item.log_content,
                        TimeDisplay = new DateTime(1970, 1, 1).AddSeconds(item.time_log).ToLocalTime().ToString("dd/MM/yyyy hh:MM:ss tt"),
                        Time = item.time_log.ToString(),
                        Tittle = item.function,
                        StatusSuccess = item.status == 1 ? "Visible" : "Hidden",
                        StatusFalse = item.status == 1 ? "Hidden" : "Visible"
                    });
                }


                //foreach (var item in _log)
                //{
                //    var a = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(float.Parse(item.time_log)).ToLocalTime().ToString("dd/MM/yyyy hh:MM:ss tt");

                //    List<LogContent> lst = new List<LogContent>();
                //    lst.Add(new LogContent() { Content = item.function });
                //    Logs _logss = new Logs();
                //    _logss.LogContents.Add(lst);
                //    //_logs.LogContents.Add(new LogContent()
                //    //{
                //    //    Content = item.log_content,
                //    //    TimeDisplay = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(float.Parse(item.time_log)).ToLocalTime().ToString("dd/MM/yyyy hh:MM:ss tt"),
                //    //    Time = item.time_log,
                //    //    Tittle = item.function.ToString(),
                //    //    StatusFalse = "Visible",
                //    //    StatusSuccess = "Hidden",
                //    //    //});
                //    //    //_logs.LogContents.Add(
                //    //    //new LogContent
                //    //    //{
                //    //    //    Tittle = "",
                //    //    //    //Time = DateTime.Now,
                //    //    //    TimeDisplay = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                //    //    //    Content = "",
                //    //    //    StatusSuccess = 1 == 1 ? "Visible" : "Hidden",
                //    //    //    StatusFalse = 0 == 1 ? "Hidden" : "Visible"
                //    //    //}
                //    //});
                //}
                //_log.LogContents.RemoveAll(x => x.Time < (DateTime.Now.AddDays(-30)));
                //new MainUtility().WriteLog(_log);
                Logs = new ObservableCollection<LogContent>(logss.OrderByDescending(x => x.Time));
            }
            catch (Exception)
            {

                Logs = new ObservableCollection<LogContent>(new List<LogContent>());
            }
        }

    }
}
