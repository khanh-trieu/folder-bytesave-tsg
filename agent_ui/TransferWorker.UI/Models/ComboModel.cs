using System.Collections.ObjectModel;

namespace TransferWorker.UI.Models
{
    public class ComboModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
    public class ComboHourModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
    public class ComboContainerModel
    {
        public int Id { get; set; }
        public string ContainerName { get; set; }
    }
    public class KetNoi
    {
        public KetNoi()
        {
            this.LstContainer = new ObservableCollection<Container>();
        }

        public string NameAppsetting { get; set; }
        public int IdAppsetting { get; set; }

        public ObservableCollection<Container> LstContainer { get; set; }
    }
    public class Container
    {
        public string NameContainer { get; set; }

        public int Id { get; set; }
    }
}