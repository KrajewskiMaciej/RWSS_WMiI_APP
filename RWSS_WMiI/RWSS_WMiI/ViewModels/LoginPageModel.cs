using RWSS_WMiI.Views;

namespace RWSS_WMiI.ViewModels
{
    public partial class LoginPageModel : BaseViewModel
    {
        public Command LoginCommand { get; }

        public LoginPageModel()
        {
            LoginCommand = new Command(async () => await OnClickLogin());

        }

        public async Task OnClickLogin()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}
