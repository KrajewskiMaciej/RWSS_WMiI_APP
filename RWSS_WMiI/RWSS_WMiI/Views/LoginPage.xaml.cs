using RWSS_WMiI.ViewModels;
using MySqlConnector;
using System.Data;

#pragma warning disable CS8600 // Konwertowanie literału null lub możliwej wartości null na nienullowalny typ.
#pragma warning disable CS8601 // Możliwe przypisanie odwołania o wartości null.

namespace RWSS_WMiI.Views
{
    public partial class LoginPage : ContentPage
    {
        public string login { get; set; } = "";
        public string passwd { get; set; } = "";
        public static string PL { get; set; } = "";

        public LoginPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }

        void SaveLogin(object sender, EventArgs e)
        {
            login = ((Entry)sender).Text;
            CheckLogin.Text = $"";
            SemanticScreenReader.Announce(CheckLogin.Text);
        }

        void SavePassword(object sender, EventArgs e)
        {
            passwd = ((Entry)sender).Text;
            CheckLogin.Text = $"";
            SemanticScreenReader.Announce(CheckLogin.Text);
        }

        private async void CheckLoginCommand(object obj, EventArgs e)
        {
            using var commandd = App.Connection.CreateCommand();
            commandd.CommandText = @"SELECT * FROM Użytkownicy WHERE `Nazwa Użytkownika` = @Username";
            commandd.Parameters.AddWithValue("@Username", login);

            using var re = await commandd.ExecuteReaderAsync();
            if (re.HasRows)
            {
                while (await re.ReadAsync())
                {

                    string storedPassword = re["Hasło"].ToString();


                    if (PasswordHasher.VerifyPassword(passwd, storedPassword))
                    {
                        CheckLogin.Text = $"Logowanie udane";
                        CheckLogin.TextColor = Colors.Green;

                        App.UID = re.GetInt32("ID");
                        App.UN = re["Imię"].ToString() + " " + re["Nazwisko"].ToString();
                        App.UE = re["E-mail"].ToString();

                        PL = re["PierwszeLogowanie"].ToString();

                        re.Close();

                            App.ADMIN = 0;
                            using var getPUA = App.Connection.CreateCommand();
                            getPUA.CommandText = @"SELECT ID_Uprawnienia FROM Użytkownicy_Uprawnienia WHERE ID_Użytkownika = @Uid ORDER BY ID_Uprawnienia;";
                            getPUA.Parameters.AddWithValue("@Uid", App.UID);

                            using MySqlDataReader read = getPUA.ExecuteReader();
                            if (read.HasRows)
                            {
                                while (await read.ReadAsync())
                                {
                                    int idUprawnienia = read.GetInt32("ID_Uprawnienia");
                                    if(App.PUA == 0)
                                    {
                                        App.PUA = read.GetInt32("ID_Uprawnienia");
                                    }

                                    if (idUprawnienia == 5)
                                    {
                                        App.ADMIN = 1;
                                    }
                                    else if (idUprawnienia < App.PUA)
                                    {
                                        App.PUA = idUprawnienia;
                                    }
                                }
                            }
                            read.Close();

                            await UserPreferences.SaveUserTypeAsync("RWSS");

                            await UserPreferences.SavePUAAsync(App.PUA);
                            await UserPreferences.SaveUIDAsync(App.UID);
                            await UserPreferences.SaveUNAsync(App.UN);
                            await UserPreferences.SaveADMINAsync(App.ADMIN);

                            await UserPreferences.SaveLoginStateAsync(true);

                        await Navigation.PushAsync(new Main());
                    }
                    else
                    {
                        CheckLogin.Text = $"Błędne hasło";
                        CheckLogin.TextColor = Colors.Red;

                        SemanticScreenReader.Announce(CheckLogin.Text);
                    }
                }
            }
            else
            {
                CheckLogin.Text = $"Błędna nazwa użytkownika";
                CheckLogin.TextColor = Colors.Red;

                SemanticScreenReader.Announce(CheckLogin.Text);
            }



        }

        private async void BackToStart(object obj, EventArgs e)
        {

            await Navigation.PushAsync(new Start());

        }

    }
}