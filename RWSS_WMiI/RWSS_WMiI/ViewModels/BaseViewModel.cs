using System.ComponentModel;

namespace RWSS_WMiI.ViewModels
{
     public class BaseViewModel : INotifyPropertyChanged
    {
#pragma warning disable CS0067 // Zdarzenie „BaseViewModel.PropertyChanged” nie jest nigdy używane
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067 // Zdarzenie „BaseViewModel.PropertyChanged” nie jest nigdy używane
        

    }
}
