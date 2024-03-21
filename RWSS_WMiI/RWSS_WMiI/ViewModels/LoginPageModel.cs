using RWSS_WMiI.Views;

namespace RWSS_WMiI.ViewModels
{
    public partial class LoginPageModel : BaseViewModel
    {
        public Command LoginCommand { get; }

        public LoginPageModel()
        {
            LoginCommand = new Command(OnClickLogin);
        }

        private async void OnClickLogin(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}
