using System.Data;
using System.Globalization;
using MySqlConnector;
using System.Xml;

#pragma warning disable CS8600 // Konwertowanie literału null lub możliwej wartości null na nienullowalny typ.
#pragma warning disable CS8601 // Możliwe przypisanie odwołania o wartości null.
#pragma warning disable CS8602 // Wyłuskanie odwołania, które może mieć wartość null.

namespace RWSS_WMiI.Views
{
    public partial class Main : ContentPage
    {
        public static string stan { get; set; } = "";
        public static string miejsce { get; set; } = "";
        public static string WithWho { get; set; } = "";
        public static string osoba { get; set; } = "";
        public static int LID { get; set; }
        public static string LIW { get; set;  }
        public static int WA_ID {  get; set; }

        public static DateTime TimeCompare { get; set; }
        public Main()
        {
            
            InitializeComponent();

        }

        protected override async void OnAppearing()
        {
            
            base.OnAppearing();
            await UpdateStatus();
            await GetRequest();


        }

        private async Task UpdateStatus()//tu może wypierdalać NullException
        {
            if (App.PUA >= 1 && App.PUA <= 12)
            {
                if(App.Connection.State == ConnectionState.Closed)
                {
                    App.Connection.OpenAsync();
                }

                ASKGrid.IsVisible = true;
                ZKGrid.IsVisible = true;

                Status.RowDefinitions[2].Height = new GridLength(50, GridUnitType.Absolute);
                Status.RowDefinitions[3].Height = new GridLength(50, GridUnitType.Absolute);

                ChckRequestsGrid.IsVisible = true;
                Status.RowDefinitions[1].Height = new GridLength(25, GridUnitType.Absolute);

                await using var cmdg = App.Connection.CreateCommand();
                cmdg.CommandText = @"SELECT Klucz.Stan, CONCAT(Użytkownicy.Imię, ' ', Użytkownicy.Nazwisko) AS Ostatnio_w_Posiadaniu, Klucz.Dodatkowe_Informacje FROM Klucz JOIN Użytkownicy ON Klucz.Ostatnio_W_Posiadaniu = Użytkownicy.ID WHERE Klucz.ID = 1;";
                    using var readStatus = await cmdg.ExecuteReaderAsync();
                    try
                    {
                    Console.WriteLine($"Status połączenia: {App.Connection.State}");

                    if (readStatus.HasRows)
                        {
                            while (readStatus.Read())
                            {
                                stan = readStatus["Stan"].ToString();
                                osoba = readStatus["Ostatnio_w_Posiadaniu"].ToString();
                                miejsce = readStatus["Dodatkowe_Informacje"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                    finally
                    {
                        readStatus?.Close();
                    }

                switch (stan)
                {
                    case "Biuro":
                        ChckStat.Text = $"Biuro otwarte";
                        break;

                    case "Bunkier":
                        ChckStat.Text = $"Klucz w bunkrze";
                        break;

                    case "Portiernia":
                        ChckStat.Text = $"Klucz na portierni";
                        break;

                    case "Osoba":
                        if (osoba.Equals(App.UN))
                        {
                            ChckStat.Text = $"Ty masz klucz";
                        }
                        else
                        {
                            ChckStat.Text = $"Klucz ma {osoba} w {miejsce}";
                        }
                        break;

                    default:
                        ChckStat.Text = $"Nieznane położenie klucza";
                        break;
                }
            }
            else
            {
                ASKGrid.IsVisible = false;
                ZKGrid.IsVisible = false;

                Status.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Absolute);
                Status.RowDefinitions[3].Height = new GridLength(0, GridUnitType.Absolute);

                ChckRequestsGrid.IsVisible = false;
                Status.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Absolute);

                await using var cmd = App.Connection.CreateCommand();
                cmd.CommandText = @"SELECT Stan FROM Klucz WHERE Klucz.ID = 1;";

                await using var rdg = await cmd.ExecuteReaderAsync();
                if (rdg.HasRows)
                {
                    while (await rdg.ReadAsync())
                    {
                        stan = rdg["Stan"].ToString();
                    }
                }
                rdg.Close();

                switch (stan)
                {
                    case "Biuro":
                        ChckStat.Text = $"Biuro otwarte";
                        break;
                    default:
                        ChckStat.Text = $"Biuro zamknięte";
                        break;
                }
            }

            SemanticScreenReader.Announce(ChckStat.Text);

            await using var command = App.Connection.CreateCommand();
            command.CommandText = @"SELECT
                                        Wydarzenia.ID,
                                        Wydarzenia.Nazwa,
                                        Wydarzenia.Data_Od,
                                        Wydarzenia.Data_Do,
                                        Wydarzenia.Opis,
                                        Wydarzenia.Link_Do_Wydarzenia,
                                        Użytkownicy.Imię AS Im_D,
                                        Użytkownicy.Nazwisko AS Na_D,
                                        Użytkownicy.Pseudonim,
                                        Wydarzenia.Kto_Dodal
                                    FROM
                                        Wydarzenia
                                    JOIN
                                        Użytkownicy
                                    ON
                                        Wydarzenia.Kto_Dodal = Użytkownicy.ID 
                                    ORDER BY
                                        Wydarzenia.Data_Od
                                    LIMIT 1;";

            await using var re = await command.ExecuteReaderAsync();
            if (re.HasRows)
            {
                while (await re.ReadAsync())
                {
                    var eventId = re.GetInt32("ID");
                    var eventName = re.GetString("Nazwa");
                    DateTime eventDateStart = re.GetDateTime("Data_Od");
                    DateTime? eventDateEnd = !re.IsDBNull("Data_Do") ? re.GetDateTime("Data_Do") : (DateTime?)null;
                    var eventText = re.GetString("Opis");
                    var eventLIW = re.IsDBNull("Link_Do_Wydarzenia") ? string.Empty : re.GetString("Link_Do_Wydarzenia");
                    var eventIm_D = re.IsDBNull("Im_D") ? string.Empty : re.GetString("Im_D");
                    var eventNa_D = re.IsDBNull("Na_D") ? string.Empty : re.GetString("Na_D");
                    var eventP_D = re.IsDBNull("Pseudonim") ? string.Empty : re.GetString("Pseudonim");
                    var eventWA_ID = re.GetInt32("Kto_Dodal");

                    MoreButtonGrid.IsVisible = false;

                    EventGrid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Absolute);

                    LinkButtonGrid.IsVisible = false;

                    EventGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Absolute);

                    if (!eventLIW.Equals(string.Empty))
                    {
                        LinkButtonGrid.IsVisible = true;

                        EventGrid.ColumnDefinitions[2].Width = new GridLength(40, GridUnitType.Absolute);
                    }

                    var eventWA = createWA(eventIm_D, eventNa_D, eventP_D);
                    Name.Text = eventName;
                    DateRange.Text = "Data: " + SetDateRange(eventDateStart, eventDateEnd);
                    Text.Text = eventText;
                    LIW = eventLIW;
                    WA.Text = eventWA;
                    WA_ID = eventWA_ID;

                }
            }
            re.Close();

        }

        public async Task GetRequest()
        {
            if (App.PUA >= 1 && App.PUA <= 12)
            {
                if (App.Connection.State == ConnectionState.Closed)
                {
                    App.Connection.OpenAsync();
                }

                string kto = "";
                DateTime dataIczas = DateTime.Now;

                using var grk = App.Connection.CreateCommand();
                grk.CommandText = @"SELECT CONCAT(Użytkownicy.Imię, ' ', Użytkownicy.Nazwisko) AS Kto, Prośby.Data_i_Czas FROM Prośby JOIN Użytkownicy ON Prośby.Kto = Użytkownicy.ID WHERE Prośby.ID = 1;";

                using MySqlDataReader requestGet = await grk.ExecuteReaderAsync();
                    if (requestGet.HasRows)
                    {
                        while (await requestGet.ReadAsync())
                        {
                            kto = requestGet["Kto"].ToString();
                            dataIczas = requestGet.GetDateTime("Data_i_Czas");
                        }
                    }

                    requestGet?.Close();

                if (!string.IsNullOrEmpty(kto))
                {

                    if (dataIczas > DateTime.Now)
                    {
                        string tmp2 = dataIczas.ToString("HH:mm");
                        ChckRequests.Text = kto + " będzie potrzebował klucza do " + tmp2;
                    }
                    else
                    {
                        ChckRequests.Text = "Nikt nie potrzebuje klucza";
                    }
                }
                else
                {
                    ChckRequests.Text = "Nikt nie potrzebuje klucza";
                }

            }
            SemanticScreenReader.Announce(ChckRequests.Text);
        }

        private async void KeyStatus(object obj, EventArgs e)
        {
            using var command = App.Connection.CreateCommand();
            command.CommandText = @"SELECT Klucz.Stan, CONCAT(Użytkownicy.Imię, ' ', Użytkownicy.Nazwisko) AS Posiadane, Klucz.Ostatnio_W_Posiadaniu, Klucz.Dodatkowe_Informacje FROM Klucz JOIN Użytkownicy;";

            using var read = await command.ExecuteReaderAsync();
            if (read.HasRows)
            {
                while (await read.ReadAsync())
                {
                    LID = read.GetInt32("Ostatnio_W_Posiadaniu");

                    osoba = read["Posiadane"].ToString();

                    stan = read["Stan"].ToString();
                }
            }

            if(stan.Equals("Portiernia"))
            {
                if (App.PUA > 1 && App.PUA < 4)
                {
                    CheckStatus(stan, LID);
                }
                else if(App.PUA ==11)
                {
                    CheckStatus(stan, LID);
                }
                else
                {
                    RequestKeyGet();
                }
                
            }
            else
            {
                CheckStatus(stan, LID);

            }
            read.Close();


        }

        private async void CheckStatus(string stan, int LID)
        {
            if (stan.Equals("Osoba") && LID != App.UID)
            {
                var result = await DisplayActionSheet($"Czy wziąłeś klucz od <{osoba}>", "Tak", "Nie");
                if (result != null && result != "Nie")
                {
                    StatusKey();
                }
            }
            else
            {
                StatusKey();
            }
        }

        private async void StatusKey()
        {

            var result = await DisplayActionSheet("Zmień status klucza", "Anuluj", null, "Otwieram biuro", "Wkładam do bunkra", "Oddaję na portiernię", "Biorę ze sobą");
            if (result != null && result != "Anuluj")
            {
                switch (result)
                {
                    case "Otwieram biuro":
                        miejsce = "Biuro";
                        stan = "Biuro";
                        break;

                    case "Wkładam do bunkra":
                        miejsce = "Bunkier";
                        stan = "Bunkier";
                        break;

                    case "Oddaję na portiernię":
                        if(ChckRequests.Text == "Nikt nie potrzebuje klucza")
                        {
                            miejsce = "Portiernia";
                            stan = "Portiernia";
                        }
                        else
                        {
                            string kto = "";
                            string dataIczas = null;

                            await using var cmd = App.Connection.CreateCommand();
                            cmd.CommandText = @"SELECT CONCAT(Użytkownicy.Imię, ' ', Użytkownicy.Nazwisko) AS Kto, Prośby.Kto, Prośby.Data_i_Czas FROM Prośby INNER JOIN Użytkownicy ON Prośby.Kto = Użytkownicy.ID WHERE Prośby.ID = 1;";

                            using var rdRQ = await cmd.ExecuteReaderAsync();
                            if (rdRQ.HasRows)
                            {
                                while (await rdRQ.ReadAsync())
                                {

                                    kto = rdRQ["Kto"].ToString();

                                    dataIczas = rdRQ["Data_i_Czas"].ToString();

                                }


                            }

                            rdRQ.Close();
                            if (!string.IsNullOrEmpty(kto))
                            {
                                DateTime tmp = DateTime.ParseExact(dataIczas, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                if (tmp > DateTime.Now)
                                {
                                    string tmp2 = tmp.ToString("HH:mm");
                                     string request= kto + " będzie potrzebował klucza do " + tmp2;

                                    await DisplayAlert("Nie możesz oddać klucza", request, "OK");
                                }

                            }
                        }
                        
                        break;

                    case "Biorę ze sobą":
                        var result1 = await DisplayPromptAsync("Gdzie będziesz", "", "Zapisz", "Anuluj", placeholder: "Wprowadź nazwę sali");

                        if (!string.IsNullOrWhiteSpace(result1) && result1 != "Anuluj")
                        {
                            miejsce = result1;
                            stan = "Osoba";
                            SemanticScreenReader.Announce(ChckStat.Text);
                        }
                        break;
                }
                using var command = App.Connection.CreateCommand();
                command.CommandText = @"UPDATE Klucz SET Stan = @stan, Ostatnio_W_Posiadaniu = @osoba, Dodatkowe_Informacje = @miejsce WHERE ID = 1";
                command.Parameters.AddWithValue("@stan", stan);
                command.Parameters.AddWithValue("@osoba", App.UID);
                command.Parameters.AddWithValue("@miejsce", miejsce);

                using var read = await command.ExecuteReaderAsync();

                read.Close();
                await UpdateStatus();

            }
        }

        private async void RequestKeyGet()
        {
            var result = await DisplayAlert("Nie możesz wziąć klucza z Portiernii", "Poprosić o klucz?", "Tak", "Nie");

            if (result.Equals(true))
            {
                await DisplayAlert("Jakotako", "Cośtam", "OK");
            }
        }
        private async void RequestKey(object obj, EventArgs e)
        {
            int person = 0;

            await using var cmd = App.Connection.CreateCommand();
            cmd.CommandText = @"SELECT Kto FROM Prośby WHERE ID = 1;";

            await using var rdRK = await cmd.ExecuteReaderAsync();
            if (rdRK.HasRows)
            {
                while (await rdRK.ReadAsync())
                {
                    if (!rdRK.IsDBNull("Kto"))
                    {
                        person = rdRK.GetInt32("Kto");
                    }
                    
                }
            }

            rdRK.Close();

            if (person == App.UID && ChckRequests.Text != "Nikt nie potrzebuje klucza")
            {
                var WhatToDo = await DisplayActionSheet("Co chcesz zrobić", "Zamknij", null, "Zmień rezerwację", "Anuluj rezerwację");
                if(WhatToDo.Equals("Zmień rezerwację"))
                {
                    SetReservationTime();
                }
                else if(WhatToDo.Equals("Anuluj rezerwację"))
                {
                    DeleteReservation();
                }
            }
            else 
            {
                SetReservationTime();
            }

        }

        private async void SetReservationTime()
        {
            var selectedTime = await DisplayPromptAsync("Podaj Godzinę", "Do której potrzebujesz klucza?", "Zapisz", "Anuluj", placeholder: "1623 = 16:23", keyboard: Keyboard.Numeric, maxLength: 4);

            if (selectedTime != null)
            {
                int time = int.Parse(selectedTime);
                int hours = 00;
                int minutes = 00;

                if (selectedTime.Length == 2)
                {
                    time *= 100;
                }
                else if (selectedTime.Length == 3)
                {
                    time = time * 10;
                }

                    hours = time / 100;
                    minutes = time % 100;


                    DateTime currentDate = DateTime.Now.Date;
                    DateTime selectedDateTime = currentDate.AddHours(hours);
                    selectedDateTime = selectedDateTime.AddMinutes(minutes);

                    int OpenHours = 20;
                    int OpenMinutes = 30;
                    DateTime OpenWMiI = currentDate.AddHours(OpenHours);
                    OpenWMiI = OpenWMiI.AddMinutes(OpenMinutes);

                

                    if (selectedDateTime > OpenWMiI)
                    {
                        await DisplayAlert("Nie możesz zarezerwować klucza", "Klucz należy oddać na portiernię przed 20:30", "OK");
                    }
                    else if(CompareTime(selectedDateTime))
                    {
                        string person ="";

                        await using var cmd = App.Connection.CreateCommand();
                        cmd.CommandText = @"SELECT Kto FROM Prośby WHERE ID = 1;";

                        await using var rdSD = await cmd.ExecuteReaderAsync();
                        if (rdSD.HasRows)
                        {
                            while (await rdSD.ReadAsync())
                            {
                                person = rdSD["Kto"].ToString();
                            }
                        }
                        rdSD.Close();

                        var ifSave = await DisplayAlert($"{person} potrzebuje klucza dłużej niż ty",$"Czy chcesz zapisać godzinę gdyby {person} nie potrzebował dłużej klucza?", "Tak", "Nie");

                        if(ifSave.Equals("Tak"))
                    {
                        AddReservationForLater(selectedDateTime);
                    }
                    }
                    else
                    {
                        var okTime = await DisplayAlert("Wybrana data i godzina", selectedDateTime.ToString(), "OK", "Anuluj");
                        if (okTime.Equals(true))
                        {
                            await using var command = App.Connection.CreateCommand();
                            command.CommandText = @"UPDATE Prośby SET Kto = @osoba, Data_i_Czas = @dataIczas WHERE ID = 1";
                            command.Parameters.AddWithValue("@osoba", App.UID);
                            command.Parameters.AddWithValue("@dataIczas", selectedDateTime);

                            await using var read = await command.ExecuteReaderAsync();

                            read.Close();
                        }
                    }
            }
        }

        private bool CompareTime(DateTime selected)
        {
            GetCompareTime();
            if(selected < TimeCompare)
            {
                return true;
            }

            return false;
        }

        private async void GetCompareTime()
        {
            await using var cmd = App.Connection.CreateCommand();
            cmd.CommandText = @"SELECT Data_i_Czas FROM Prośby WHERE ID = 1;";

            await using var rdGCT = await cmd.ExecuteReaderAsync();
            if (rdGCT.HasRows)
            {
                while (await rdGCT.ReadAsync())
                {
                    if (!rdGCT.IsDBNull(rdGCT.GetOrdinal("Data_i_Czas")))
                    {
                        TimeCompare = rdGCT.GetDateTime("Data_i_Czas");
                    }
                }
            }
            rdGCT.Close();
        }

        private async void DeleteReservation()
        {

            await using var command = App.Connection.CreateCommand();
            command.CommandText = @"UPDATE Prośby SET Kto = null, Data_i_Czas = null WHERE ID = 1";

            await using var read = await command.ExecuteReaderAsync();

            read.Close();

            await using var command1 = App.Connection.CreateCommand();
            command1.CommandText = @"SELECT Kto, Data_i_Czas FROM Prośby WHERE Zdarzenie = 'Będe potrzebować do' ORDER BY Data_i_Czas DESC Limit 1;";
            await using var latestRecord = await command1.ExecuteReaderAsync();

            if (latestRecord.HasRows)
            {
                int tmp = latestRecord.GetInt32("Kto");
                DateTime tmpDT = latestRecord.GetDateTime("Data_i_Czas");
                // Ustaw Kto i Data_i_Czas na wartości z najpóźniejszego rekordu
                await using var command2 = App.Connection.CreateCommand();
                command2.CommandText = @"UPDATE Prośby SET Kto = @Kto, Data_i_Czas = @Data_i_Czas WHERE Zdarzenie = 'Będe potrzebować do'";
                command2.Parameters.AddWithValue("@Data_i_Czas", tmpDT);
                command2.Parameters.AddWithValue("@Kto", tmp);
                await command2.ExecuteNonQueryAsync();

                await DisplayAlert("Usunięto rezerwację", "Twoja rezerwacja została usunięta", "OK");
            }

            latestRecord.Close();
        }

        private async void AddReservationForLater(DateTime selectedDateTime)
        {
            var okTime = await DisplayAlert("Wybrana data i godzina", selectedDateTime.ToString(), "OK", "Anuluj");
            if (okTime.Equals(true))
            {
                await using var command = App.Connection.CreateCommand();
                command.CommandText = @"INSERT INTO Prośby SET Kto = @osoba, Data_i_Czas = @dataIczas";
                command.Parameters.AddWithValue("@osoba", App.UID);
                command.Parameters.AddWithValue("@dataIczas", selectedDateTime);

                await using var read = await command.ExecuteReaderAsync();

                read.Close();
            }
        }


        private string SetDateRange(DateTime startDate, DateTime? endDate)
        {
            string dateRange;
            if (startDate.Date == endDate.GetValueOrDefault().Date)
            {
                return dateRange = $"{startDate:dd.MM.yyyy HH:mm} - {endDate:HH:mm}";
            }
            else if (endDate == null)
            {
                return dateRange = $"{startDate:dd.MM.yyyy HH:mm}";
            }
            else
            {
                return dateRange = $"{startDate:dd.MM.yyyy HH:mm} - {endDate:dd.MM.yyyy HH:mm}";
            }
        }
        private string createWA(string name, string lastName, string nickname)
        {
            if (!nickname.Equals(string.Empty))
            {
                return MoF(name) + " " + name + " " + '"' + nickname + '"' + " " + lastName;
            }
            else
            {
                return MoF(name) + " " + name + " " + lastName;
            }
        }

        private string MoF(string name)
        {
            if (name.EndsWith("a", StringComparison.OrdinalIgnoreCase))
            {
                return "Dodała";
            }
            else
            {
                return "Dodał";
            }
        }

        private async void OnEventSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem is SetEvent selectedEvent)
            {
                var shell = (Shell)App.Current.MainPage;

                var navigation = shell.CurrentItem?.CurrentItem?.CurrentItem?.Navigation;

                if (navigation != null)
                {
                    if (selectedEvent.LIW != null && selectedEvent.LIW != "")
                    {
                        await Launcher.OpenAsync(new Uri(selectedEvent.LIW));
                    }
                }

            }
        }

        private async void ButtonMoreClicked(object sender, EventArgs e)
        {
            // Adres URL, który chcesz otworzyć
            string url = "https://www.example.com";

            // Otwarcie linku w przeglądarce systemowej
            await Launcher.OpenAsync(new Uri(url));
        }


    }


}
