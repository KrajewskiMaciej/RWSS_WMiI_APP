using System.ComponentModel;

namespace RWSS_WMiI.ViewModels
{
     public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        

    }
}
