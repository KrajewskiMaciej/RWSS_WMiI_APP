using RWSS_WMiI.Views;

namespace RWSS_WMiI.ViewModels
{
    public partial class GuestAboutModel : BaseViewModel
    {
        public Command LoginPageCommand { get; }

        public GuestAboutModel()
        {
            LoginPageCommand = new Command(OnClickLogin);
        }

        private async void OnClickLogin(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
