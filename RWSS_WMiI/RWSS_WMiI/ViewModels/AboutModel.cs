using RWSS_WMiI.Views;

namespace RWSS_WMiI.ViewModels
{
    public partial class AboutModel : BaseViewModel
    {
        public Command MainPageCommand { get; }

        public AboutModel()
        {
            MainPageCommand = new Command(OnClickLogin);
        }

        private async void OnClickLogin(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}
