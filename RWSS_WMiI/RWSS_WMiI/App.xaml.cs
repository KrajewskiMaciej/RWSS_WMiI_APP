namespace RWSS_WMiI
{
    public partial class App : Application
    {
        public static int PUA { get; set; }
        
        public App()
        {
            InitializeComponent();
            PUA = 10;
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }
    }
}
