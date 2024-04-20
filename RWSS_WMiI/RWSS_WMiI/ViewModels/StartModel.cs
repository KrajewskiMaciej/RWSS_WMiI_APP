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
            App.PUA = 1;
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            UASV();
        }

        private async void OnClickMain(object obj)
        {
            App.PUA = 10;
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            UASV();
        }
        public void UASV()
        {
            if (Application.Current.MainPage is AppShell appShell)
            {
                if (App.PUA >= 1 && App.PUA <= 9)
                {
                    appShell.DUT();
                }
                else if (App.PUA == 10)
                {
                    appShell.DGT();
                }
            }
        }
    }
}
