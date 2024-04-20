using RWSS_WMiI.Views;

namespace RWSS_WMiI.ViewModels
{
    
    public partial class AccountModel : BaseViewModel
    {
        public Command LogoutCommand { get; }
        public Command AboutCommand { get; }
        public AccountModel()
        {
            LogoutCommand = new Command(OnClickLogout);
            AboutCommand = new Command(OnClickAbout);
        }

        private async void OnClickLogout(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(Start)}");
        }

        private async void OnClickAbout(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(About)}");
        }
    }
}
