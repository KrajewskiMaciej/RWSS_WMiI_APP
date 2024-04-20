using RWSS_WMiI.Views;
using Microsoft.Maui.Controls;

namespace RWSS_WMiI
{
    
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        public void DUT()
        {
            ((Tab)this.FindByName("MainPageTab")).IsVisible = true;
            ((Tab)this.FindByName("InfoTab")).IsVisible = true;
            ((Tab)this.FindByName("AccountTab")).IsVisible = true;
            ((Tab)this.FindByName("GuestAboutTab")).IsVisible = false;
            ((Tab)this.FindByName("BoardgamesTab")).IsVisible = false;
        }

        public void DGT()
        {
            ((Tab)this.FindByName("AccountTab")).IsVisible = false;
            ((Tab)this.FindByName("GuestAboutTab")).IsVisible = true;
            ((Tab)this.FindByName("BoardgamesTab")).IsVisible = false;
        }
    }
}
