using RWSS_WMiI.ViewModels;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace RWSS_WMiI.Views
{
    public partial class LoginPage : ContentPage
    {
        public Command LoginCheckCommand { get; }
        StartModel startModel = new StartModel();

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
        public string login = "";
        public string passwd = "";

        private async void CheckLoginCommand(object obj, EventArgs e)
        {
            LoginPageModel viewModel = (LoginPageModel)this.BindingContext;
            if (login == "Test")
            {
                if (passwd == "test")
                {
                    CheckLogin.Text = $"Logowanie udane";
                    App.PUA = 1;
                    startModel.UASV();
                    await viewModel.OnClickLogin();
                }
            }
            else
            {
                CheckLogin.Text = $"Błędna nazwa użytkownika lub hasło";
            }
            SemanticScreenReader.Announce(CheckLogin.Text);
        }
    }
}
