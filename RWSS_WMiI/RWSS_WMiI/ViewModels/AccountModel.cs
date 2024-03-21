using RWSS_WMiI.Views;

namespace RWSS_WMiI.ViewModels
{
    
    public partial class AccountModel : BaseViewModel
    {
        public Command LogoutCommand { get; }
        public AccountModel()
        {
            LogoutCommand = new Command(OnClickLogout);
        }

        private async void OnClickLogout(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
