using System.Data;
using MySqlConnector;
using RWSS_WMiI.Views;

namespace RWSS_WMiI
{
    public partial class App : Application
    {
        public static MySqlConnection Connection { get; private set; }
        public static int PUA { get; set; }
        public static int ADMIN { get; set; } = 0;
        public static int UID { get; set; }
        public static string UN { get; set; } = "";
        public static string UE { get; set; } = "";

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

            Connection = new MySqlConnection("Server=rwsswmiiapp-rwsswmii.e.aivencloud.com;Port=17446;User ID=rwsswmiiapp;Password=AVNS_Xbbzzk3o1DO_vO5reb6; Database=rwsswmiiapp");

            DoOnStart();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            if (Connection.State == ConnectionState.Closed)
            {
                await Connection.OpenAsync();
            }
            
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.OpenAsync();
                MainPage = new AppShell();
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();

        } 
        
        public async void DoOnStart()
        {
            MainPage = new AppShell();


            if (Connection.State == ConnectionState.Closed)
            {
                await Connection.OpenAsync();
            }

            string isLoggedIn = await UserPreferences.GetUserTypeAsync();
            if (isLoggedIn == "RWSS")
            {
                App.PUA = await UserPreferences.GetPUAAsync();
                App.ADMIN = await UserPreferences.GetADMINAsync();
                App.UID = await UserPreferences.GetUIDAsync();
                App.UN = await UserPreferences.GetUNAsync();

                await Shell.Current.GoToAsync($"//{nameof(Main)}");
            }
            else
            {
                App.PUA = 0;
                App.ADMIN = 0;
                App.UID = 0;
                App.UN = "";

                await Shell.Current.GoToAsync($"//{nameof(Start)}");
            }

        }
    }

    public class UserPreferences
    {
        private const string UserTypeKey = "UserType";
        private const string IsLoggedInKey = "IsLoggedIn";
        private const string UIDKey = "UID";
        private const string UNKey = "UN";
        private const string PUAKey = "PUA";
        private const string ADMINKey = "ADMIN";

        public static async Task SaveUserTypeAsync(string userType)
        {
            await SecureStorage.SetAsync(UserTypeKey, userType);
        }

        public static async Task<string> GetUserTypeAsync()
        {
            return await SecureStorage.GetAsync(UserTypeKey);
        }

        public static async Task SaveLoginStateAsync(bool isLoggedIn)
        {
            await SecureStorage.SetAsync(IsLoggedInKey, isLoggedIn.ToString());
        }

        public static async Task<bool> GetLoginStateAsync()
        {
            var isLoggedIn = await SecureStorage.GetAsync(IsLoggedInKey);
            return bool.TryParse(isLoggedIn, out bool result) && result;
        }

        public static async Task SaveUIDAsync(int uid)
        {
            await SecureStorage.SetAsync(UIDKey, uid.ToString());
        }

        public static async Task<int> GetUIDAsync()
        {
            var uid = await SecureStorage.GetAsync(UIDKey);
            return int.TryParse(uid, out int result) ? result : 0;
        }

        public static async Task SaveUNAsync(string un)
        {
            await SecureStorage.SetAsync(UNKey, un);
        }

        public static async Task<string> GetUNAsync()
        {
            return await SecureStorage.GetAsync(UNKey);
        }

        public static async Task SavePUAAsync(int pua)
        {
            await SecureStorage.SetAsync(PUAKey, pua.ToString());
        }

        public static async Task<int> GetPUAAsync()
        {
            var pua = await SecureStorage.GetAsync(PUAKey);
            return int.TryParse(pua, out int result) ? result : 0;
        }

        public static async Task SaveADMINAsync(int admin)
        {
            await SecureStorage.SetAsync(ADMINKey, admin.ToString());
        }

        public static async Task<int> GetADMINAsync()
        {
            var admin = await SecureStorage.GetAsync(ADMINKey);
            return int.TryParse(admin, out int result) ? result : 0;
        }
    }


}
