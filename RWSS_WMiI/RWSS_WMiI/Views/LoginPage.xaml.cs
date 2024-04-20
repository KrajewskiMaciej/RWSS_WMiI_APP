using RWSS_WMiI.ViewModels;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace RWSS_WMiI.Views
{
    public partial class LoginPage : ContentPage
    {
        public Command LoginCheckCommand { get; }

        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginPageModel();
        }
        
        void SaveLogin(object sender, EventArgs e)
        {
            login = ((Entry)sender).Text;
        }
        void SavePassword(object sender, EventArgs e)
        {
            passwd = ((Entry)sender).Text;
        }
        public string login="";
        public string passwd="";

        private async void CheckLoginCommand(object obj, EventArgs e)
        {
            if ((login == "Test") && (passwd == "zaq1@WSX"))
            {
                CheckLogin.Text = $"Logowanie udane";
                await Shell.Current.GoToAsync(nameof(LoginPageModel.OnClickLogin));
            }
            else
            {
                CheckLogin.Text = $"Błędna nazwa użytkownika lub hasło";
            }
            SemanticScreenReader.Announce(CheckLogin.Text);
        }
    }
}
