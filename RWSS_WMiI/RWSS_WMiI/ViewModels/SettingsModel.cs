using RWSS_WMiI.Views;
using System.Windows.Input;

namespace RWSS_WMiI.ViewModels
{

    public partial class SettingsModel : BaseViewModel
    {
        public Command LogoutCommand { get; }
        public Command LoginCommand { get; }
        public Command AboutCommand { get; }
        public ICommand ChangeThemeCommand { get; }

        public SettingsModel()
        {
            LogoutCommand = new Command(async () => await OnClickLogout());
            AboutCommand = new Command(async () => await OnClickAbout());
            LoginCommand = new Command(async () => await OnClickLogin());
            ChangeThemeCommand = new ChangeThemeCommand();

        }

        private async Task OnClickLogout()
        {
            App.ADMIN = 0;
            App.PUA = 0;
            App.UID = 0;
            App.UN = "";
            App.UE = "";
            Main.stan = "";
            Main.miejsce = "";
            Main.WithWho = "";
            Main.osoba = "";
            Main.LID = 0;
            Main.LIW = "";
            Main.WA_ID = 0;
            Main.TimeCompare = DateTime.Now;
            await Shell.Current.GoToAsync($"//{nameof(Start)}");
        }

        private async Task OnClickLogin()
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");

        }

        private async Task OnClickAbout()
        {
                await Shell.Current.GoToAsync($"//{nameof(About)}");

        }
    }
    public class ChangeThemeCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true; // Zawsze możemy wykonać zmianę motywu
        }

        public void Execute(object parameter)
        {
            if (parameter is string theme)
            {
                // Tutaj wykonaj logikę zmiany motywu na podstawie wartości 'theme'
                // Na przykład możesz użyć DependencyService do ustawienia motywu
                // w zależności od wybranej opcji.
                // Upewnij się, że masz implementacje zmiany motywu dla każdej opcji.
                // Poniżej znajduje się przykładowa implementacja zmiany motywu:

                if (theme == "Jasny")
                {
                    // Ustaw motyw na jasny
                    // np. ThemeHelper.SetTheme(Theme.Light);
                }
                else if (theme == "Ciemny")
                {
                    // Ustaw motyw na ciemny
                    // np. ThemeHelper.SetTheme(Theme.Dark);
                }
                else if (theme == "Systemowy")
                {
                    // Ustaw motyw na motyw systemowy
                    // np. ThemeHelper.SetTheme(Theme.System);
                }
            }
        }
    }
}
