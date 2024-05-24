

namespace RWSS_WMiI.Views
{
    public partial class Boardgames : ContentPage
    {
        private List<SetBoardgame> _allBoardgames;
        public Boardgames()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            LoadBoardgames();
            IsAddBoardgameVisible();   
        }

        public async Task LoadBoardgames()
        {
            _allBoardgames = await GetList();
            BoardgamesListView.ItemsSource = _allBoardgames;
        }
        public async Task<List<SetBoardgame>> GetList()
        {
            await using var command = App.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Planszowki;";

            var BoardgamesList = new List<SetBoardgame>();

            await using var re = await command.ExecuteReaderAsync();
            if (re.HasRows)
            {
                while (await re.ReadAsync())
                {
                    string BG_Rented_Person = string.Empty;
                    int BG_Rented_Nr_Album = 0 ;
                    // Pobierz dane użytkownika z czytnika
                    var BG_Id = re.GetInt32(re.GetOrdinal("Id"));
                    var BG_Name = re.GetString(re.GetOrdinal("Nazwa"));
                    var BG_IsRented = re.GetString(re.GetOrdinal("Czy_Wypozyczone"));
                    BG_Rented_Person = re.IsDBNull(re.GetOrdinal("Wypozyczone_Dla_Osoba")) ? BG_Rented_Person : re.GetString(re.GetOrdinal("Wypozyczone_Dla_Osoba"));
                    BG_Rented_Nr_Album = re.IsDBNull(re.GetOrdinal("Wypozyczone_Dla_NR_Albumu")) ? BG_Rented_Nr_Album : re.GetInt32(re.GetOrdinal("Wypozyczone_Dla_NR_Albumu"));

                    var IRBV = false;
                    var IHBV = false;
                    var CW = 0;

                    if (App.PUA >= 1 && App.PUA <= 12)
                    {
                            IRBV = true;
                        IHBV = true;
                        CW = 60;
                    }
                    else if (App.ADMIN == 1)
                    {
                        IRBV = true;
                        IHBV = true;
                        CW = 60;
                    }

                    string Status = "Brak danych";
                    Color colorStatus = Colors.White;
                    switch(BG_IsRented)
                    {
                        case "Nie":
                            Status = "Dostępne";
                            colorStatus = Colors.Green;
                            break;
                        case "Tak":
                            Status = "Niedostępne";
                            colorStatus = Colors.Red;
                            break;
                    }

                    string RoG = "Wypożycz";
                    if(Status == "Niedostępne")
                    {
                        RoG = "Zwróć";
                    }

                    string rentedToStatus = "";
                    string rentedTo = "";
                    if (!string.IsNullOrEmpty(BG_Rented_Person))
                    {
                        rentedTo = BG_Rented_Person+ "(" + BG_Rented_Nr_Album.ToString() + ")";
                        rentedToStatus = $"Wypożyczone dla: {BG_Rented_Person}({BG_Rented_Nr_Album})";
                    }


                    var Boardgame = new SetBoardgame { ID = BG_Id, Name = BG_Name, Status = Status, ROG = RoG, StatusColor = colorStatus, RentedToStatus = rentedToStatus, RentedTo = rentedTo , IsRentButtonVisible = IRBV, ColumnWidth = CW, IsHistoryButtonVisible = IHBV };

                    BoardgamesList.Add(Boardgame);
                }
            }
            re.Close();

            return BoardgamesList;
        }

        async void ButtonRentClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedBoardgame = (SetBoardgame)button.BindingContext;
            var RentClicked = "";
            int fwNA = 0;
            string rentedTotmp = selectedBoardgame.RentedTo;
            if (selectedBoardgame.Status == "Dostępne")
            {
                RentClicked = await DisplayPromptAsync($"Wypożycz {selectedBoardgame.Name}", "Podaj Imię i Nazwisko", "OK", "Anuluj");

                if (!string.IsNullOrWhiteSpace(RentClicked) && RentClicked != "Anuluj")
                {
                    var GetAN = await DisplayPromptAsync($"Wypożycz {selectedBoardgame.Name}", "Podaj Nr Albumu", "OK", "Anuluj", keyboard: Keyboard.Numeric);

                    if (!string.IsNullOrWhiteSpace(GetAN) && GetAN != "Anuluj")
                    {
                        fwNA = int.Parse(GetAN);
                        string rentedTo = RentClicked + "(" + fwNA.ToString() + ")";

                        
                            UpdateBoardgame(selectedBoardgame.ID, RentClicked, fwNA, selectedBoardgame.Status, rentedTo, selectedBoardgame.Name);
                    }
                }
            }
            else if(selectedBoardgame.Status == "Niedostępne")
            {
                var GetClicked = await DisplayAlert($"Zwrot '{selectedBoardgame.Name}'", $"Czy zwrócić grę '{selectedBoardgame.Name}' wypożyczoną przez {selectedBoardgame.RentedTo}?", "Tak", "Nie");
                if(GetClicked == true)
                {
                    UpdateBoardgame(selectedBoardgame.ID, RentClicked, fwNA, selectedBoardgame.Status, rentedTotmp, selectedBoardgame.Name);
                }
            }

        }

        async void ButtonHistoryClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedBoardgame = (SetBoardgame)button.BindingContext;

            await Navigation.PushAsync(new BoardgameHistory(selectedBoardgame.ID));


        }

        public async Task UpdateBoardgame(int ID, string FW_N, int FW_NA, string rog, string rTO, string BGName)
        {
            if(rog == "Niedostępne")
            {
                await using var command = App.Connection.CreateCommand();
                command.CommandText = @"UPDATE Planszowki SET Wypozyczone_Dla_Osoba = null, Wypozyczone_Dla_NR_Albumu = null, Czy_Wypozyczone = 'Nie' WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", ID);

                await using var ub = await command.ExecuteReaderAsync();

                ub.Close();

                await UpdateBoardgameHistoryGet(ID, rTO, BGName);
                
            }
            else if(rog == "Dostępne")
            {
                await using var command = App.Connection.CreateCommand();
                command.CommandText = @"UPDATE Planszowki SET Wypozyczone_Dla_Osoba = @FW_N, Wypozyczone_Dla_NR_Albumu = @FW_NA, Czy_Wypozyczone = 'Tak' WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@FW_N", FW_N);
                command.Parameters.AddWithValue("@FW_NA", FW_NA);

                await using var ub = await command.ExecuteReaderAsync();

                ub.Close();

                await UpdateBoardgameHistoryRent(ID, rTO, BGName);
            }
            
        }

        public async Task UpdateBoardgameHistoryRent(int ID, string rTo, string BGName)
        {
            string tmp = rTo;
            await using var commandh = App.Connection.CreateCommand();
            commandh.CommandText = @"INSERT INTO Planszowki_Historia (ID_Planszowki, Kto_Wypozyczyl, Osoba_Wypozyczajaca, Wypozyczenie_Start) VALUES (@ID, @UID, @RTO, @DTSR);";
            commandh.Parameters.AddWithValue("@ID", ID);
            commandh.Parameters.AddWithValue("@UID", App.UID);
            commandh.Parameters.AddWithValue("@RTO", rTo);
            commandh.Parameters.AddWithValue("@DTSR", DateTime.Now);

            await using var ubh = await commandh.ExecuteReaderAsync();

            ubh.Close();
            await DisplayAlert($"Wypożyczono '{BGName}'", "Gra została wypożyczona", "OK", "Anuluj");
            LoadBoardgames();
        }

        public async Task UpdateBoardgameHistoryGet(int ID, string rTo, string BGName)
        {
            await using var commandh = App.Connection.CreateCommand();
            commandh.CommandText = @"UPDATE Planszowki_Historia SET Wypozyczenie_Koniec = @DTER, Kto_Przyjal = @UID WHERE ID_Planszowki = @ID AND Osoba_Wypozyczajaca = @RTO AND Wypozyczenie_Koniec IS null AND Kto_Przyjal IS null";
            commandh.Parameters.AddWithValue("@ID", ID);
            commandh.Parameters.AddWithValue("@UID", App.UID);
            commandh.Parameters.AddWithValue("@RTO", rTo);
            commandh.Parameters.AddWithValue("@DTER", DateTime.Now);

            await using var ubh = await commandh.ExecuteReaderAsync();

            ubh.Close();
            await DisplayAlert($"Zwrócono '{BGName}'", "Gra została zwrócona", "OK", "Anuluj");
            LoadBoardgames();
        }

        public async void IsAddBoardgameVisible()
        {
            AddBoardgame.IsVisible = false;
            if (App.PUA >= 1 && App.PUA <= 12)
            {
                AddBoardgame.IsVisible = true;
            }
            else if (App.ADMIN == 1)
            {
                AddBoardgame.IsVisible = true;
            }
        }

        private async void AddBoardgameClicked(object sender, EventArgs e)
        {
            var AddIndex = await DisplayPromptAsync("Podaj Index Planszówki", "Wpisz jeżeli nie w kolejności", "Dalej", "Anuluj", keyboard: Keyboard.Numeric);
            if (AddIndex != "Anuluj")
            {
                var AddName = await DisplayPromptAsync("Podaj Nazwę Planszówki", "", "Dalej", "Anuluj");
                if (!string.IsNullOrWhiteSpace(AddName) && AddName != "Anuluj")
                {
                    string comm = "Auto";
                    if (!string.IsNullOrWhiteSpace(AddIndex))
                    {
                        comm = AddIndex;
                    }

                    var IsAllOK = await DisplayAlert("Czy dodać nową planszówkę?", $"Czy dodać Index: {comm}, Nazwa: '{AddName}' do Planszówek?", "Tak", "Nie");
                    if (IsAllOK == true)
                    {
                        int NewIndex = 0;

                        

                        string NameTMP = AddName;
                        if (NewIndex != 0)
                        {
                            NewIndex = int.Parse(AddIndex);

                            await using var command = App.Connection.CreateCommand();
                            command.CommandText = @"INSERT INTO Planszowki (ID, Nazwa) VALUES (@NBG_ID, @NBG_N);";
                            command.Parameters.AddWithValue("@NBG_ID", NewIndex);
                            command.Parameters.AddWithValue("@NBG_N", NameTMP);

                            await using var re = await command.ExecuteReaderAsync();

                            re.Close();
                        }
                        else if (NewIndex == 0)
                        {

                            using var commandADD = App.Connection.CreateCommand();
                            commandADD.CommandText = @"INSERT INTO Planszowki (Nazwa) VALUES (@nbgn);";
                            commandADD.Parameters.AddWithValue("@nbgn", NameTMP);

                            using var reADD = await commandADD.ExecuteReaderAsync();

                            reADD.Close();

                        }
                        await DisplayAlert("Planszówka Dodana", $"Planszówka '{NameTMP}' została dodana pomyślnie", "OK");

                    }
                }
            }

            LoadBoardgames();
        }

        void OnSearchChanged(object sender, EventArgs e)
        {
            SearchBar searchBar = (SearchBar)sender;
            BoardgamesListView.ItemsSource = DataService.GetSearchResults(_allBoardgames, searchBar.Text);
        }

    }

    public class SetBoardgame
    {
        public int ColumnWidth { get; set; }
        public bool IsRentButtonVisible { get; set; }
        public bool IsHistoryButtonVisible { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public Color StatusColor { get; set;  }
        public string RentedToStatus { get; set; }
        public string RentedTo { get; set; }
        public string ROG { get; set; }
    }

    public static class DataService
    {
        public static IEnumerable<SetBoardgame> GetSearchResults(IEnumerable<SetBoardgame> allBoardgames, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return allBoardgames;
            }

            return allBoardgames.Where(bg =>
                bg.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                bg.ID.ToString().Contains(query));
        }
    }
}
