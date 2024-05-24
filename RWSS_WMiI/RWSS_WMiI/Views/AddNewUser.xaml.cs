

namespace RWSS_WMiI.Views
{
    public partial class AddNewUser : ContentPage
    {

        public string Imie { get; set; } = "";
        public string Nazwisko { get; set; } = "";
        public int Nr_Albumu { get; set; } = 0;
        public string Email { get; set; } = "";
        public string Stopien { get; set; } = "";
        public string Kierunek { get; set; } = "";
        public string Rok { get; set; } = "";
        public string Forma { get; set; } = "";
        public int Rola { get; set; } = 0;
        public AddNewUser()
        {
            InitializeComponent();

            StopienPick.SelectedIndexChanged += (sender, e) =>
            {
                if (StopienPick.SelectedIndex != -1)
                {
                    Stopien = StopienPick.SelectedItem.ToString();
                }
            };

            KierunekPick.SelectedIndexChanged += (sender, e) =>
            {
                if (KierunekPick.SelectedIndex != -1)
                {
                    Kierunek = KierunekPick.SelectedItem.ToString();
                }
            };

            RokPick.SelectedIndexChanged += (sender, e) =>
            {
                if (RokPick.SelectedIndex != -1)
                {
                    Rok = RokPick.SelectedItem.ToString();
                }
            };
            FormaPick.SelectedIndexChanged += (sender, e) =>
            {
                if (FormaPick.SelectedIndex != -1)
                {
                    Forma = FormaPick.SelectedItem.ToString();
                }
            };

            RolaPick.SelectedIndexChanged += (sender, e) =>
            {
                if (RolaPick.SelectedIndex != -1)
                {
                    Rola = (int)RolaPick.SelectedIndex;
                }
            };


        }

        void SaveImie(object sender, EventArgs e)
        {
            Imie = ((Entry)sender).Text;
        }

        void SaveNazwisko(object sender, EventArgs e)
        {
            Nazwisko = ((Entry)sender).Text;
        }

        void SaveNr_Albumu(object sender, EventArgs e)
        {
            if (int.TryParse(((Entry)sender).Text, out int result))
            {
                Nr_Albumu = result;
            }
        }

        void SaveEmail(object sender, EventArgs e)
        {
            Email = ((Entry)sender).Text;
        }


        public async void CheckNewUser(object obj, EventArgs e)
        {

            if (string.IsNullOrEmpty(Imie) || string.IsNullOrEmpty(Nazwisko) || string.IsNullOrEmpty(Stopien) || string.IsNullOrEmpty(Kierunek) || string.IsNullOrEmpty(Forma) || string.IsNullOrEmpty(Rok) || Nr_Albumu < 100000)
            {
                CheckNewUserPlaceholder.Text = "*Błąd przy dodawaniu nowego użytkownika! ";
                CheckNewUserPlaceholder.TextColor = Colors.Red;

            }
            else
            {
                string nazwa = GenerujNazweUzytkownika(Imie, Nazwisko, Nr_Albumu);

                if (string.IsNullOrEmpty(Email))
                {
                    Email = GenerujEmail(Nr_Albumu);
                }

                int setRole = SetRole(Rola);

                string haslo = PasswordGenerator.GeneratePassword();
                string hasloHashed = PasswordHasher.HashPassword(haslo); 

                await using var command = App.Connection.CreateCommand();
                command.CommandText = @"INSERT INTO Użytkownicy (`Nazwa Użytkownika`, Hasło, Imię, Nazwisko, `Nr Albumu`, Stopień, Kierunek, Rok, Forma, `E-mail`) VALUES (@USERNAME, @HASLO, @NAME, @LASTNAME, @ALBUM_NR, @STOPIEN, @KIERUNEK, @ROK, @FORMA, @EMAIL)";

                command.Parameters.AddWithValue("@USERNAME", nazwa);
                command.Parameters.AddWithValue("@HASLO", hasloHashed);
                command.Parameters.AddWithValue("@NAME", Imie);
                command.Parameters.AddWithValue("@LASTNAME", Nazwisko);
                command.Parameters.AddWithValue("@ALBUM_NR", Nr_Albumu);
                command.Parameters.AddWithValue("@STOPIEN", Stopien);
                command.Parameters.AddWithValue("@KIERUNEK", Kierunek);
                command.Parameters.AddWithValue("@ROK", Rok);
                command.Parameters.AddWithValue("@FORMA", Forma);
                command.Parameters.AddWithValue("@EMAIL", Email);

                await using var read = await command.ExecuteReaderAsync();

                read.Close();

                int tmp = 0;

                await using var command2 = App.Connection.CreateCommand();
                command2.CommandText = @"SELECT ID FROM Użytkownicy WHERE `Nr Albumu` = @NR_ALBUM;";
                command2.Parameters.AddWithValue("@NR_ALBUM", Nr_Albumu);

                await using var read2 = await command2.ExecuteReaderAsync();
                if (read2.HasRows)
                {
                    while (await read2.ReadAsync())
                    {
                        tmp = read2.GetInt32("ID");
                    }
                }
                read2.Close();

                await using var command3 = App.Connection.CreateCommand();
                command3.CommandText = @"INSERT INTO Użytkownicy_Uprawnienia (ID_Użytkownika, ID_Uprawnienia) VALUES (@TUID, @UprID);";
                command3.Parameters.AddWithValue("TUID", tmp);
                command3.Parameters.AddWithValue("UprID", setRole);
                await using var read3 = await command3.ExecuteReaderAsync();

                read3.Close();

                EmailSender.SendEmail(Imie, nazwa, haslo, App.UE, Email);

                CheckNewUserPlaceholder.Text = "Użytkownik dodany pomyślnie!";
                CheckNewUserPlaceholder.TextColor = Colors.Green;
            }
        }

        static string GenerujNazweUzytkownika(string imie, string nazwisko, int nrAlbumu)
        {
            string nrAlbumuString = nrAlbumu.ToString();
            string nazwaUzytkownika = imie[..Math.Min(3, imie.Length)].ToLower() +
                                      nazwisko[..Math.Min(3, nazwisko.Length)].ToLower() +
                                      nrAlbumuString[Math.Max(0, nrAlbumuString.Length - 3)..];
            return nazwaUzytkownika;
        }

        static string GenerujEmail(int nrAlbumu)
        {

            string email = nrAlbumu.ToString().ToLower().Replace(" ", "") + "@student.uwm.edu.pl";
            return email;
        }
        static int SetRole(int rola)
        {
            return rola switch
            {
                0 => 7,
                1 => 8,
                2 => 9,
                3 => 11,
                _ => 9,
            };
        }

    }
}
