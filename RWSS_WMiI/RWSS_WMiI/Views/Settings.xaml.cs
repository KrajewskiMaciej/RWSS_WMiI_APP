using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RWSS_WMiI.ViewModels;

namespace RWSS_WMiI.Views
{
    public partial class Settings : ContentPage
    {
        public Command LogoutCommand { get; }
        public Command LoginCommand { get; }
        public Command AboutCommand { get; }

        public Settings()
        {
            InitializeComponent();
            LoadSettings();
            this.BindingContext = new SettingsModel();
        }
        public class SettingView
        {
            public string Name { get; set; }
            public string Details { get; set; }
        }

        public async Task LoadSettings()
        {
            var settingSet = await GetSettingsList();
            SettingsVisibility.ItemsSource = settingSet;
        }

        public async Task<List<SettingView>> GetSettingsList()
        {
            var settingSet = new List<SettingView>
            {
                new SettingView()
                {
                    Name = "Zmień motyw",
                    Details = "Kliknij by zmienić motyw (na razie nie działa)"
                },
                new SettingView()
                {
                    Name = "O aplikacji",
                    Details = ""
                }
            };
            if(App.PUA ==0)
            {
                settingSet.Add(new SettingView()
                {
                    Name = "Zaloguj",
                    Details = "Jeżeli posiadasz konto, zaloguj się by używać pełnej wersji aplikacji"
                });
                
            }
            else
            {
                settingSet.Add(new SettingView()
                {
                    Name = "Wyloguj",
                    Details = ""
                });
            }

            return settingSet;
        }

        private async void OnSettingSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem is SettingView selectedSetting)
            {
                switch (selectedSetting.Name)
                {
                    case "Zmień motyw":
                        await DisplayActionSheetForTheme();
                        break;
                    case "O aplikacji":
                            var shell = (Shell)App.Current.MainPage;

                            var rwssTab = shell.FindByName<Tab>("RWSSTab");

                            rwssTab.CurrentItem = rwssTab.Items[0];

                            var navigation = shell.CurrentItem?.CurrentItem?.CurrentItem?.Navigation;

                            if (navigation != null)
                            {
                                await navigation.PushAsync(new About());
                            }

                            // Odznacz element listy
                            SettingsVisibility.SelectedItem = null;
                        break;
                    case "Zaloguj":
                        ((SettingsModel)BindingContext).LoginCommand.Execute(null);
                        break;
                    case "Wyloguj":
                        // Wywołanie bezpośrednie metody LogoutCommand
                        ((SettingsModel)BindingContext).LogoutCommand.Execute(null);
                        break;
                    default:
                        break;
                }

                // Zresetuj wybór, aby można było ponownie wybrać element listy
                SettingsVisibility.SelectedItem = null;
            }
        }

        private async Task DisplayActionSheetForTheme()
        {
            string action = await DisplayActionSheet("Wybierz motyw", "Anuluj", null, "Jasny", "Ciemny", "Systemowy");

            // Tutaj wykonaj logikę zmiany motywu na podstawie wybranej akcji
            ((SettingsModel)BindingContext).ChangeThemeCommand.Execute(action);
        }


    }

    
}
