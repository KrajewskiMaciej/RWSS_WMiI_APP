
using Microsoft.Extensions.Logging;

namespace RWSS_WMiI.Views
{

    public partial class EditAccount : ContentPage
    {
        
        public string Imie { get; set; } = "";
        public string Nazwisko { get; set; } = "";
        public string Pseudonim { get; set; } = "";
        public string Link { get; set; } = "";
        public string Opis { get; set; } = "";
        public string Stopien { get; set; }
        public string Kierunek { get; set; }
        public string Rok { get; set; }
        public string Forma { get; set; }

        public int userID { get; set; }
        public string OldImie { get; set; } = "";
        public string OldNazwisko { get; set; } = "";
        public string OldPseudonim { get; set; } = "";
        public string OldLink { get; set; } = "";
        public string OldOpis { get; set; } = "";
        public string OldStopien { get; set; }
        public string OldKierunek { get; set; }
        public string OldRok { get; set; }
        public string OldForma { get; set; }
        public EditAccount()
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


        }

        public EditAccount(int userID) : this()
        {
            this.userID = userID;

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            LoadEditedUser(userID);
        }

        void SaveImie(object sender, EventArgs e)
        {
            Imie = ((Entry)sender).Text;
        }

        void SaveNazwisko(object sender, EventArgs e)
        {
            Nazwisko = ((Entry)sender).Text;
        }

        void SavePseudonim(object sender, EventArgs e)
        {
            Pseudonim = ((Entry)sender).Text;
        }

        void SaveLink(object sender, EventArgs e)
        {
            Link = ((Entry)sender).Text;
        }

        void OnEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            string oldText = e.OldTextValue;
            string newText = e.NewTextValue;
            string myText = SavedOpis.Text;
        }

        public async Task LoadEditedUser(int EUID)
        {
            await using var cmd = App.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Użytkownicy WHERE ID = @EUID;";
            cmd.Parameters.AddWithValue("@EUID", EUID);

            await using var rd = await cmd.ExecuteReaderAsync();
            if (rd.HasRows)
            {
                while (await rd.ReadAsync())
                {
                        OldImie = rd["Imię"].ToString();
                        SavedImie.Text = OldImie;

                        OldNazwisko = rd["Nazwisko"].ToString();
                        SavedNazwisko.Text = OldNazwisko;

                        OldLink = rd["Link_Do_Messenger"].ToString();
                        SavedLink.Text = OldLink;

                        OldOpis = rd["Opis"].ToString();
                        SavedOpis.Text = OldOpis;

                    OldPseudonim = rd["Pseudonim"].ToString();
                    SavedPseudonim.Text = OldPseudonim;

                    OldForma = rd["Forma"].ToString();
                        FormaPick.SelectedItem = OldForma;

                        OldRok = rd["Rok"].ToString();
                        RokPick.SelectedItem = OldRok;

                        OldKierunek = rd["Kierunek"].ToString();
                        KierunekPick.SelectedItem = OldKierunek;

                        OldStopien = rd["Stopień"].ToString();
                        StopienPick.SelectedItem = OldStopien;
                }
            }
            rd.Close();
        }

        public async void CheckEditedUser(object obj, EventArgs e)
        {
            Opis = SavedOpis.Text;

            if (string.IsNullOrEmpty(Imie) || string.IsNullOrEmpty(Nazwisko) || string.IsNullOrEmpty(Stopien) || string.IsNullOrEmpty(Kierunek) || string.IsNullOrEmpty(Forma) || string.IsNullOrEmpty(Rok))
            {
                CheckEditedUserPlaceholder.Text = "*Błąd przy edytowaniu użytkownika! ";
                CheckEditedUserPlaceholder.TextColor = Colors.Red;

            }
            else
            {
                await using var command = App.Connection.CreateCommand();
                command.CommandText = @"UPDATE Użytkownicy SET Imię = @IMIE, Nazwisko = @NAZWISKO, Pseudonim = @PSEUDONIM, Opis = @OPIS, Link_Do_Messenger = @LINK, Stopień = @STOPIEN, Kierunek = @KIERUNEK, Rok = @ROK, Forma = @FORMA WHERE ID = @EUID;";

                if (OldImie == Imie) { command.Parameters.AddWithValue("@IMIE", OldImie); }
                else { command.Parameters.AddWithValue("@IMIE", Imie); }

                if (OldNazwisko == Nazwisko) { command.Parameters.AddWithValue("@NAZWISKO", OldNazwisko); }
                else { command.Parameters.AddWithValue("@NAZWISKO", Nazwisko); }

                if (OldPseudonim == Pseudonim) { command.Parameters.AddWithValue("@PSEUDONIM", OldPseudonim); }
                else { command.Parameters.AddWithValue("@PSEUDONIM", Pseudonim); }

                if (OldOpis == Opis) { command.Parameters.AddWithValue("@OPIS", OldOpis); }
                else { command.Parameters.AddWithValue("@OPIS", Opis); }

                if (OldLink == Link) { command.Parameters.AddWithValue("@LINK", OldLink); }
                else { command.Parameters.AddWithValue("@LINK", Link); }

                command.Parameters.AddWithValue("@STOPIEN", Stopien);
                command.Parameters.AddWithValue("@KIERUNEK", Kierunek);
                command.Parameters.AddWithValue("@ROK", Rok);
                command.Parameters.AddWithValue("@FORMA", Forma);

                command.Parameters.AddWithValue("@EUID", userID);

                await using var read = await command.ExecuteReaderAsync();

                read.Close();

                CheckEditedUserPlaceholder.Text = "Zapisano zmiany!";
                CheckEditedUserPlaceholder.TextColor = Colors.Green;
            }
        }

        public async Task OnChangePasswordClicked(object obj, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(PasswdChg)}");
        }

    }
}