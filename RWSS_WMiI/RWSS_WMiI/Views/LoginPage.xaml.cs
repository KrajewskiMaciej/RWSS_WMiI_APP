using RWSS_WMiI.ViewModels;

namespace RWSS_WMiI.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginPageModel();
            
        }
    }
}
