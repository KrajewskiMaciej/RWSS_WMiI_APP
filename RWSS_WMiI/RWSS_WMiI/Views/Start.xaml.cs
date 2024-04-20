using RWSS_WMiI.ViewModels;

namespace RWSS_WMiI.Views
{
    public partial class Start : ContentPage
    {
        public Start()
        {
            InitializeComponent();
            this.BindingContext = new StartModel();
        }
    }

}
