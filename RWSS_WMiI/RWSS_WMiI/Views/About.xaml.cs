using RWSS_WMiI.ViewModels;

namespace RWSS_WMiI.Views
{
    public partial class About : ContentPage
    {
        public About()
        {
            InitializeComponent();
            this.BindingContext = new AboutModel();
        }
    }
}
