using RWSS_WMiI.ViewModels;

namespace RWSS_WMiI.Views
{
    public partial class Account : ContentPage
    {
        public Account()
        {
            InitializeComponent();
            this.BindingContext = new AccountModel();

        }
    }
}
