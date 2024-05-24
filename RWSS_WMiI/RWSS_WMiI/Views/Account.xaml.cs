

namespace RWSS_WMiI.Views
{

    public partial class Account : ContentPage
    {
        private int userID;
        public Account()
        {
            InitializeComponent();

        }

        public Account(int userID) : this()
        {
            this.userID = userID;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
                SetUser();
        }

        private async void SetUser()
        {
            if (App.PUA < 1 || App.PUA > 12)
            {
                EditButton.IsVisible = false;
            }
            else if(App.UID != userID)
            {
                EditButton.IsVisible = false;
            }

            await using var command = App.Connection.CreateCommand();
            command.CommandText = @"SELECT Użytkownicy.ID, Użytkownicy.Imię, Użytkownicy.Nazwisko, Użytkownicy.Kierunek, Użytkownicy.Rok, Użytkownicy.Forma, Użytkownicy.Stopień, Użytkownicy.Opis, Użytkownicy.`Zdjęcie Profilowe` AS Zdjęcie, GROUP_CONCAT(Uprawnienia.Nazwa ORDER BY Uprawnienia.ID DESC SEPARATOR ', ') AS Funkcje FROM Użytkownicy JOIN Użytkownicy_Uprawnienia ON Użytkownicy.ID = Użytkownicy_Uprawnienia.ID_Użytkownika JOIN Uprawnienia ON Uprawnienia.ID = Użytkownicy_Uprawnienia.ID_Uprawnienia WHERE Użytkownicy.ID = @UID;";
            command.Parameters.AddWithValue("@UID", userID);

            await using var re = await command.ExecuteReaderAsync();
            if (re.HasRows)
            {
                while (await re.ReadAsync())
                {
                    // Pobierz dane użytkownika z czytnika
                    User.Text = re.GetString(re.GetOrdinal("Imię")) + " " + re.GetString(re.GetOrdinal("Nazwisko"));
                    Role.Text = re.GetString(re.GetOrdinal("Funkcje"));
                    KiR.Text = re.GetString(re.GetOrdinal("Kierunek")) + ", " + re.GetString(re.GetOrdinal("Rok")) + " Rok" + ", " + re.GetString(re.GetOrdinal("Forma"));
                    UserText.Text = re.IsDBNull(re.GetOrdinal("Opis")) ? UserText.Text : re.GetString(re.GetOrdinal("Opis"));
                    Stopien.Text = re.GetString(re.GetOrdinal("Stopień"));

                    var imagePath = $"{Application.Current.Resources}/Images/{"avatar.png"}";
                    var userImage = re.IsDBNull(re.GetOrdinal("Zdjęcie")) ? imagePath : re.GetString(re.GetOrdinal("Zdjęcie"));


                }
            }
            re.Close();
            SemanticScreenReader.Announce(User.Text);
        }

        private async void EditAccountClicked(object sender, EventArgs e)
        {
            var clickedButton = sender as Button;
            if (clickedButton != null)
            {
                var shell = (Shell)App.Current.MainPage;
                var navigation = shell.CurrentItem?.CurrentItem?.CurrentItem?.Navigation;
                if (navigation != null)
                {
                    await navigation.PushAsync(new EditAccount(userID));
                }
            }
        }
    }




    }
