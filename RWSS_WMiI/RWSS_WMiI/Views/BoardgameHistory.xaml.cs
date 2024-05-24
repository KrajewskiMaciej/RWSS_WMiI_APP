using DocumentFormat.OpenXml.Spreadsheet;
using RWSS_WMiI.ViewModels;
using System.Data;

namespace RWSS_WMiI.Views
{

    public partial class BoardgameHistory : ContentPage
    {
        private int BG_ID;
        public BoardgameHistory()
        {
            InitializeComponent();

        }

        public BoardgameHistory(int bgID) : this()
        {
            this.BG_ID = bgID;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            LoadBoardgameHistory();
        }

        public async Task LoadBoardgameHistory()
        {
            var BoardgameHistoryList = await GetBoardgameHistory();
            BoardgameHistoryListView.ItemsSource = BoardgameHistoryList;
        }

        private async Task<List<SetBoardgameHistory>> GetBoardgameHistory()
        {

            await using var command = App.Connection.CreateCommand();
            command.CommandText = @"SELECT 
                                        PH.Osoba_Wypozyczajaca,
                                        P.Nazwa AS Nazwa_Planszowki,
                                        CONCAT(UW.Imię, ' ', UW.Nazwisko) AS Kto_Wypozyczyl,
                                        PH.Wypozyczenie_Start,
                                        PH.Wypozyczenie_Koniec,
                                        CONCAT(UP.Imię, ' ', UP.Nazwisko) AS Kto_Przyjal
                                    FROM 
                                        Planszowki_Historia PH
                                    JOIN 
                                        Planszowki P ON PH.ID_Planszowki = P.ID
                                    JOIN 
                                        Użytkownicy UW ON PH.Kto_Wypozyczyl = UW.ID
                                    JOIN 
                                        Użytkownicy UP ON PH.Kto_Przyjal = UP.ID
                                    WHERE P.ID = @PID;";
            command.Parameters.AddWithValue("@PID", BG_ID);

            var BoardgameHistoryList = new List<SetBoardgameHistory>();

            await using var re = await command.ExecuteReaderAsync();
            if (re.HasRows)
            {
                while (await re.ReadAsync())
                {
                    // Pobierz dane użytkownika z czytnika
                    var RentedTo = re.GetString(re.GetOrdinal("Osoba_Wypozyczajaca"));
                    BG_Name.Text ="Historia Wypożyczeń " + re.GetString(re.GetOrdinal("Nazwa_Planszowki"));
                    var Who_Rent = re.GetString(re.GetOrdinal("Kto_Wypozyczyl"));
                    var Rent_Start = re.GetDateTime(re.GetOrdinal("Wypozyczenie_Start"));
                    var Rent_End = re.GetDateTime(re.GetOrdinal("Wypozyczenie_Koniec"));
                    var Who_Get = re.GetString(re.GetOrdinal("Kto_Przyjal"));

                    var Boardgame = new SetBoardgameHistory { BG_Rented_To = RentedTo, BG_Who_Rent = Who_Rent, BG_Rent_Start = Rent_Start, BG_Rent_End = Rent_End, BG_Who_Get = Who_Get};

                    BoardgameHistoryList.Add(Boardgame);
                }
            }
            re.Close();

            return BoardgameHistoryList;
        }

    }

    public class SetBoardgameHistory
    {
        public string BG_Rented_To { get; set; }
        public string BG_Who_Rent { get; set; }
        public DateTime BG_Rent_Start { get; set; }
        public DateTime BG_Rent_End { get; set; }
        public string BG_Who_Get { get; set; }
    }




}
