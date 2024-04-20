using RWSS_WMiI.ViewModels;

namespace RWSS_WMiI.Views
{
    public partial class GuestAbout : ContentPage
    {
        public GuestAbout()
        {
            InitializeComponent();
            this.BindingContext = new GuestAboutModel();
        }
    }
}
