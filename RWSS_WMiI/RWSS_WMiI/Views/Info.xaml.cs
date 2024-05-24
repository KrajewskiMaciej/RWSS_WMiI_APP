

namespace RWSS_WMiI.Views
{
    public partial class Info : ContentPage
    {
        public Info()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            LoadEvents();
            ButtonVis();
        }

        public async Task LoadEvents()
        {
            var eventsList = await GetEventList();
            eventsListView.ItemsSource = eventsList;
        }

        public async Task<List<SetEvent>> GetEventList()
        {
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
                                        Wydarzenia.Data_Od;";

            var eventList = new List<SetEvent>();

            await using var re = await command.ExecuteReaderAsync();
            if (re.HasRows)
            {
                while (await re.ReadAsync())
                {
                    var eventId = re.GetInt32(re.GetOrdinal("ID"));
                    var eventName = re.GetString(re.GetOrdinal("Nazwa"));
                    DateTime eventDateStart = re.GetDateTime(re.GetOrdinal("Data_Od"));
                    DateTime? eventDateEnd = !re.IsDBNull(re.GetOrdinal("Data_Do")) ? re.GetDateTime(re.GetOrdinal("Data_Do")) : (DateTime?)null;
                    var eventText = re.GetString(re.GetOrdinal("Opis"));
                    var eventLIW = re.IsDBNull(re.GetOrdinal("Link_Do_Wydarzenia")) ? string.Empty : re.GetString(re.GetOrdinal("Link_Do_Wydarzenia"));
                    var eventIm_D = re.IsDBNull(re.GetOrdinal("Im_D")) ? string.Empty : re.GetString(re.GetOrdinal("Im_D"));
                    var eventNa_D = re.IsDBNull(re.GetOrdinal("Na_D")) ? string.Empty : re.GetString(re.GetOrdinal("Na_D"));
                    var eventP_D = re.IsDBNull(re.GetOrdinal("Pseudonim")) ? string.Empty : re.GetString(re.GetOrdinal("Pseudonim"));
                    var eventWA_ID = re.GetInt32(re.GetOrdinal("Kto_Dodal"));

                    var IMBV = false;
                    var MCW = 0;

                    if (App.PUA >= 1 && App.PUA <= 7)
                    {
                            IMBV = true;
                            MCW = 35;
                    }
                    else if( App.ADMIN == 1)
                    {
                        IMBV = true;
                        MCW = 35;
                    }

                    if(eventId == 1)
                    {
                        if(App.PUA == 1 || App.PUA ==2 || App.ADMIN == 1) 
                        {
                            IMBV = true;
                            MCW = 35;
                        }
                        else
                        {
                            IMBV = false;
                            MCW = 0;
                        }
                    }

                    var ILBV = false;
                    var LCW = 0;

                    if (!eventLIW.Equals(string.Empty))
                    {
                        ILBV = true;
                        LCW = 35;
                    }

                    var eventWA = CreateWA(eventIm_D, eventNa_D, eventP_D);
                    var event1 = new SetEvent { Id = eventId, Name = eventName, DateRange ="Data: " + SetDateRange(eventDateStart, eventDateEnd), Text = eventText, LIW = eventLIW, WA = eventWA,WA_ID = eventWA_ID, IsMoreButtonVisible = IMBV, MoreColumnWidth = MCW, LinkColumnWidth = LCW, IsLinkButtonVisible = ILBV};

                    eventList.Add(event1);
                }
            }
            re.Close();


            return eventList;
        }

        private string SetDateRange(DateTime startDate, DateTime? endDate)
        {
            if (startDate.Date == endDate.GetValueOrDefault().Date)
            {
                return _ = $"{startDate:dd.MM.yyyy HH:mm} - {endDate:HH:mm}";
            }
            else if (endDate == null)
            {
                return _ = $"{startDate:dd.MM.yyyy HH:mm}";
            }
            else
            {
                return _ = $"{startDate:dd.MM.yyyy HH:mm} - {endDate:dd.MM.yyyy HH:mm}";
            }
        }
        private string CreateWA(string name, string lastName, string nickname)
        {
            if(!nickname.Equals(string.Empty))
            {
                return MoF(name)+ " "+ name + " " + '"' + nickname + '"' + " " + lastName;
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

                // Odznacz element listy
                eventsListView.SelectedItem = null;
            }
        }

        private void ButtonVis()
        {
            AddButtonFrame.IsVisible = false;

            if (App.PUA >= 1 && App.PUA <= 7)
            {
                AddButtonFrame.IsVisible = true;
            }
            else if (App.ADMIN == 1)
            {
                AddButtonFrame.IsVisible = true;
            }
        }

        private async void AddNewEvent(object sender, EventArgs e)
        {
            if (sender is ImageButton clickedButton && clickedButton.Equals(AddButtonVis))
            {
                var shell = (Shell)App.Current.MainPage;
                var navigation = shell.CurrentItem?.CurrentItem?.CurrentItem?.Navigation;
                if (navigation != null)
                {
                    await navigation.PushAsync(new AddNewEvent());
                }
            }
        }

        public async void ButtonMoreClicked(object sender, EventArgs e)
        {
            var shell = (Shell)App.Current.MainPage;

            var rwssTab = shell.FindByName<Tab>("InfoTab");

            rwssTab.CurrentItem = rwssTab.Items[0];

            var navigation = shell.CurrentItem?.CurrentItem?.CurrentItem?.Navigation;

            var button = (ImageButton)sender;
            var selectedEvent = (SetEvent)button.BindingContext;

            var MoreClicked = await DisplayActionSheet($"Edycja Wydarzenia", "Anuluj", null, $"Edytuj Wydarzenie", $"Usuń Wydarzenie");

            if (MoreClicked == $"Edytuj Wydarzenie")
            {
                if (navigation != null)
                {
                    await navigation.PushAsync(new EditEvent(selectedEvent.Id));
                }
            }
            else if(MoreClicked == $"Usuń Wydarzenie")
            {
                var IsSure = await DisplayAlert($"Usunąć wydarzenie {selectedEvent.Name}?", "Tej operacji nie można cofnąć", "Tak", "Nie");
                if(IsSure == true)
                {
                    DeleteEvent(selectedEvent.Id, selectedEvent.Name);
                }
            }
        }

        private async void DeleteEvent(int IID, string SEN)
        {
            await using var cmd = App.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM Wydarzenia WHERE ID= @EID;";
            cmd.Parameters.AddWithValue("@EID", IID);

            await using var rd = await cmd.ExecuteReaderAsync();

            rd.Close();
            await DisplayAlert($"Usunięto wydarzenie {SEN}", "Pomyślnie usunięto wydarzenie", "OK");

            await LoadEvents();
        }

    }

        public class SetEvent
    {
        // Właściwości wydarzenia
        public int MoreColumnWidth { get; set; }
        public bool IsMoreButtonVisible { get; set; }
        public int LinkColumnWidth { get; set; }
        public bool IsLinkButtonVisible { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string DateRange { get; set; }
        public string Text { get; set; } = "";
        public string LIW { get; set; } = "";
        public string WA { get; set; } = "";
        public int WA_ID { get; set; }
    }
}
