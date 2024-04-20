using RWSS_WMiI.Views;

namespace RWSS_WMiI.ViewModels
{
    public partial class StartModel : BaseViewModel
    {
        public Command LoginPageCommand { get; }
        public Command MainPageCommand { get; }

        public StartModel()
        {
            LoginPageCommand = new Command(OnClickLogin);
            MainPageCommand = new Command(OnClickMain);
        }

        private async void OnClickLogin(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        private async void OnClickMain(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}
