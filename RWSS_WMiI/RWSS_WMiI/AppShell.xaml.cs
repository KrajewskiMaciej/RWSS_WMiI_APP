using RWSS_WMiI.Views;

namespace RWSS_WMiI
{
    
    public partial class AppShell : Shell
    {
#pragma warning disable CA1041 // Określ komunikat dla atrybutu ObsoleteAttribute
        [Obsolete]
#pragma warning restore CA1041 // Określ komunikat dla atrybutu ObsoleteAttribute
        public AppShell()
        {
            InitializeComponent();
            GetPageName();
            UsersTab.Appearing += RWSSTab_Tapped;
            InfoTab.Appearing += InfoTab_Tapped;
            Settings.Appearing += Settings_Tapped;
            BoardgamesTab.Appearing += Boardgames_Tapped;


        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            
            base.OnNavigated(args);
            UASV();
            GetPageName();

        }

        private async void RWSSTab_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//UsersCard");
        }

        private async void InfoTab_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Info");
        }
        private async void Settings_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Settings");
        }
        private async void Boardgames_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Boardgames");
        }

        public void GetPageName()
        {
            if (CurrentPage == null)
            {
                return;
            }

            var titleView = new StackLayout
                {
                    Padding = 10
                };

                var titleLabel = new Label
                {
                    FontSize = 32,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Text = CurrentPage.Title
                };

                var descriptionLabel = new Label
                {
                    Text = "Trwają prace nad aplikacją",
                    FontSize = 8,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                titleView.Children.Add(titleLabel);
                titleView.Children.Add(descriptionLabel);

                Shell.SetTitleView(this, titleView);
        }
        public static void UASV()
        {
            (((Tab)Current.FindByName("UsersTab")).Items[1]).IsVisible = false;
            (((Tab)Current.FindByName("UsersTab")).Items[2]).IsVisible = false;
            (((Tab)Current.FindByName("UsersTab")).Items[3]).IsVisible = false;
            (((Tab)Current.FindByName("InfoTab")).Items[1]).IsVisible = false;
            (((Tab)Current.FindByName("InfoTab")).Items[2]).IsVisible = false;
            (((Tab)Current.FindByName("Settings")).Items[1]).IsVisible = false;
            (((Tab)Current.FindByName("BoardgamesTab")).Items[1]).IsVisible = false;
        }
    }

}
