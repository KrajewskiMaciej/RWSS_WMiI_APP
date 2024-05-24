

namespace RWSS_WMiI.Views
{
    public partial class UsersCard : ContentPage
    {
        public UsersCard()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            LoadUsers();
            ButtonVis();
        }

        public async Task LoadUsers()
        {
            var usersList = await GetUsersList();
            usersListView.ItemsSource = usersList;
        }

        public async Task<List<SetUser>> GetUsersList()
        {
            await using var command = App.Connection.CreateCommand();
            command.CommandText = @"SELECT
                                        U.ID,
                                        U.Imię,
                                        U.Nazwisko,
                                        U.Kierunek,
                                        U.Rok,
                                        U.Link_Do_Messenger,
                                        U.`Zdjęcie Profilowe` AS Zdjęcie,
                                        GROUP_CONCAT(UPR.Nazwa ORDER BY UPR.ID SEPARATOR ', ') AS Funkcje
                                    FROM
                                        Użytkownicy U
                                    JOIN
                                        Użytkownicy_Uprawnienia UU ON U.ID = UU.ID_Użytkownika
                                    JOIN
                                        Uprawnienia UPR ON UPR.ID = UU.ID_Uprawnienia
                                    GROUP BY
                                        U.ID
                                    ORDER BY
                                        (SELECT MIN(UPR2.ID)
                                         FROM Użytkownicy_Uprawnienia UU2
                                         JOIN Uprawnienia UPR2 ON UU2.ID_Uprawnienia = UPR2.ID
                                         WHERE UU2.ID_Użytkownika = U.ID);";
            command.Parameters.AddWithValue("@UID", App.UID);

            var usersList = new List<SetUser>(); // Tworzymy listę użytkowników

            await using var re = await command.ExecuteReaderAsync();
            if (re.HasRows)
            {
                while (await re.ReadAsync())
                {
                    // Pobierz dane użytkownika z czytnika
                    var userId = re.GetInt32(re.GetOrdinal("Id"));
                    var userName = re.GetString(re.GetOrdinal("Imię")) + " " + re.GetString(re.GetOrdinal("Nazwisko"));
                    var userRole = re.GetString(re.GetOrdinal("Funkcje"));
                    var userKiR = re.GetString(re.GetOrdinal("Kierunek")) + ", " + re.GetString(re.GetOrdinal("Rok")) + " Rok";
                    var userLIMS = re.IsDBNull(re.GetOrdinal("Link_Do_Messenger")) ? string.Empty : re.GetString(re.GetOrdinal("Link_Do_Messenger"));

                    var IMBV = false;
                    var CW = 0;

                    if (App.PUA >= 1 && App.PUA <= 4)
                    {
                        if (App.UID != userId)
                        {
                            IMBV = true;
                            CW = 35;
                        }
                    }
                    else if (App.ADMIN == 1)
                    {
                        IMBV = true;
                        CW = 35;
                    }

                    var ILBV = false;
                    if (!userLIMS.Equals(string.Empty))
                    {
                        if (App.UID != userId)
                        {
                            ILBV = true;
                        }
                    }

                    if (App.UID == userId)
                    {
                        userName += (" (TY)");
                    }

                    var imagePath = $"Images/avatar.png";
                    var userImage = re.IsDBNull(re.GetOrdinal("Zdjęcie")) ? imagePath : re.GetString(re.GetOrdinal("Zdjęcie"));


                    var user = new SetUser { Id = userId, Name = userName, Role = userRole, KiR = userKiR, Image = userImage, LIMS = userLIMS, IsMoreButtonVisible = IMBV, ColumnWidth = CW, IsMessageButtonVisible = ILBV };

                    usersList.Add(user);
                }
            }
            re.Close();

            return usersList;
        }

        private async void OnUserSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem is SetUser selectedUser)
            {
                var shell = Application.Current.MainPage as Shell;

                var rwssTab = shell.FindByName<Tab>("RWSSTab");

                rwssTab.CurrentItem = rwssTab.Items[0];

                var navigation = shell.CurrentItem?.CurrentItem?.CurrentItem?.Navigation;

                if (navigation != null)
                {
                    await navigation.PushAsync(new Account(selectedUser.Id));
                }

                // Odznacz element listy
                usersListView.SelectedItem = null;
            }
        }

        private static async void ButtonMessageClicked(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            var selectedUser = (SetUser)button.BindingContext;

            var url = selectedUser.LIMS;

            // Otwarcie linku w przeglądarce systemowej
            await Launcher.OpenAsync(new Uri(url));
        }

        public async void ButtonMoreClicked(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            var selectedUser = (SetUser)button.BindingContext;

            var MoreClicked = await DisplayActionSheet($"Edycja Użytkownika {selectedUser.Name}", "Anuluj", null, $"Edytuj rolę Użytkownikowi {selectedUser.Name}");

            if (MoreClicked == $"Edytuj rolę Użytkownikowi {selectedUser.Name}")
            {
                var AOD = await DisplayActionSheet($"Edytuj rolę Użytkownikowi {selectedUser.Name}", "Anuluj", null, "Dodaj Rolę", "Usuń Rolę");
                if (AOD == "Dodaj Rolę")
                {
                    var WR = "";
                    int PR;

                    if (App.ADMIN == 1)
                    {
                        WR = await DisplayActionSheet($"Dodaj Rolę Użytkownikowi {selectedUser.Name}", "Anuluj", null, "Przewodniczący", "Wiceprzewodniczący", "Członek Prezydium", "Delegat do RUSS", "Koordynator Sportu", "Osoba odpowiedzialna za Sociale", "Starosta Roku", "Wicestarosta Roku", "Członek RWSS", "Członek RWSS z możliwością wzięcia klucza z portierni", "Absolwent", "Admin");
                    }
                    else if (App.PUA == 1)
                    {
                        WR = await DisplayActionSheet($"Dodaj Rolę Użytkownikowi {selectedUser.Name}", "Anuluj", null, "Przewodniczący", "Wiceprzewodniczący", "Członek Prezydium", "Delegat do RUSS", "Koordynator Sportu", "Osoba odpowiedzialna za Sociale", "Starosta Roku", "Wicestarosta Roku", "Członek RWSS", "Członek RWSS z możliwością wzięcia klucza z portierni", "Absolwent");
                    }

                    else if (App.PUA == 2)
                    {
                        WR = await DisplayActionSheet($"Dodaj Rolę Użytkownikowi {selectedUser.Name}", "Anuluj", null, "Członek Prezydium", "Delegat do RUSS", "Koordynator Sportu", "Osoba odpowiedzialna za Sociale", "Starosta Roku", "Wicestarosta Roku", "Członek RWSS", "Członek RWSS z możliwością wzięcia klucza z portierni", "Absolwent");
                    }
                    else if (App.PUA == 3 || App.PUA == 4)
                    {
                        WR = await DisplayActionSheet($"Dodaj Rolę Użytkownikowi {selectedUser.Name}", "Anuluj", null, "Koordynator Sportu", "Osoba odpowiedzialna za Sociale", "Starosta Roku", "Wicestarosta Roku", "Członek RWSS", "Absolwent");
                    }


                    switch (WR)
                    {
                        case "Przewodniczący":
                            bool ZR;
                            if (App.PUA == 1)
                            {
                                ZR = await DisplayAlert("Uwaga!", $"Czy chcesz przekazać pozycję Przewodniczącego dla {selectedUser.Name}? \n Tej zmiany nie można cofnąć", "Tak", "Nie");
                                if (ZR == true)
                                {
                                    PR = 1;
                                    ChangePresident(selectedUser.Id, PR);
                                }
                            }
                            else if (App.ADMIN == 1)
                            {
                                ZR = await DisplayAlert("Uwaga!", $"Czy chcesz przekazać pozycję Przewodniczącego dla {selectedUser.Name}? \n Tej zmiany nie można cofnąć", "Tak", "Nie");
                                if (ZR == true)
                                {
                                    var CN = await DisplayAlert("Czy na Pewno?", "Czy na Pewno?", "Tak", "Nie");
                                    if (CN == true)
                                    {
                                        var JP = await DisplayAlert("Jesteś Pewien?", $"Jesteś Pewien?", "Tak", "Nie");
                                        if (JP == true)
                                        {
                                            var NC = await DisplayAlert("Uwaga!", $"Tej zmiany nie można cofnąć", "Tak", "Nie");
                                            if (NC == true)
                                            {
                                                var OK = await DisplayAlert("OK", $"OK", "OK", "Nie");
                                                if (OK == true)
                                                {
                                                    PR = 1;
                                                    ChangePresident(selectedUser.Id, PR);
                                                }
                                                else if (OK == false)
                                                {
                                                    var AHA = await DisplayAlert("MAM CIĘ!", $"Nie kliknąłeś OK", "OK", "Nie");
                                                    if (AHA == true)
                                                    {
                                                        var CJP = await DisplayAlert("Czy NA PEWNO chcesz zmienić przewodniczącego?", $"Tej zmiany nie można cofnąć (a przynajmniej inni)", "Tak", "Nie");
                                                        if (CJP == true)
                                                        {
                                                            PR = 1;
                                                            ChangePresident(selectedUser.Id, PR);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }

                            break;
                        case "Wiceprzewodniczący":
                            PR = 2;
                            ChangeRole(selectedUser.Id, PR, selectedUser.Name, WR);
                            break;
                        case "Członek Prezydium":
                            PR = 3;
                            ChangeRole(selectedUser.Id, PR, selectedUser.Name, WR);
                            break;
                        case "Delegat do RUSS":
                            PR = 4;
                            ChangeRole(selectedUser.Id, PR, selectedUser.Name, WR);
                            break;
                        case "Koordynator Sportu":
                            PR = 6;
                            ChangeRole(selectedUser.Id, PR, selectedUser.Name, WR);
                            break;
                        case "Osoba odpowiedzialna za Sociale":
                            PR = 7;
                            ChangeRole(selectedUser.Id, PR, selectedUser.Name, WR);
                            break;
                        case "Starosta Roku":
                            PR = 8;
                            ChangeRole(selectedUser.Id, PR, selectedUser.Name, WR);
                            break;
                        case "Wicestarosta Roku":
                            PR = 9;
                            ChangeRole(selectedUser.Id, PR, selectedUser.Name, WR);
                            break;
                        case "Członek RWSS":
                            PR = 10;
                            ChangeRole(selectedUser.Id, PR, selectedUser.Name, WR);
                            break;
                        case "Członek RWSS z możliwością wzięcia klucza z portierni":
                            PR = 11;
                            ChangeRole(selectedUser.Id, PR, selectedUser.Name, WR);
                            break;
                        case "Absolwent":
                            PR = 12;
                            ChangeRole(selectedUser.Id, PR, selectedUser.Name, WR);
                            break;
                        case "Admin":
                            if (App.ADMIN == 1)
                            {
                                bool ZA = await DisplayAlert("Uwaga!", $"Czy chcesz przekazać pozycję Admina dla {selectedUser.Name}? \n Tej zmiany nie można cofnąć", "Tak", "Nie");
                                if (ZA == true)
                                {
                                    var CN = await DisplayAlert("Czy na Pewno?", "Czy na Pewno?", "Tak", "Nie");
                                    if (CN == true)
                                    {
                                        var JP = await DisplayAlert("Jesteś Pewien?", $"Jesteś Pewien?", "Tak", "Nie");
                                        if (JP == true)
                                        {
                                            var NC = await DisplayAlert("Uwaga!", $"Tej zmiany nie można cofnąć", "Tak", "Nie");
                                            if (NC == true)
                                            {
                                                var OK = await DisplayAlert("OK", $"OK", "OK", "Nie");
                                                if (OK == true)
                                                {
                                                    PR = 5;
                                                    ChangeAdmin(selectedUser.Id, PR);
                                                }
                                                else if (OK == false)
                                                {
                                                    var AHA = await DisplayAlert("MAM CIĘ!", $"Nie kliknąłeś OK", "OK", "Nie");
                                                    if (AHA == true)
                                                    {
                                                        var CJP = await DisplayAlert("Czy NA PEWNO chcesz zmienić Admina?", $"Tej zmiany nie można cofnąć (Serio)", "Tak", "Nie");
                                                        if (CJP == true)
                                                        {
                                                            PR = 5;
                                                            ChangeAdmin(selectedUser.Id, PR);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            break;

                    }
                }
                else if (AOD == "Usuń Rolę")
                {
                    await using var cmd = App.Connection.CreateCommand();
                    cmd.CommandText = @"SELECT UPR.Nazwa AS Funkcje FROM Użytkownicy_Uprawnienia UU JOIN Uprawnienia UPR ON UPR.ID = UU.ID_Uprawnienia  WHERE UU.ID_Użytkownika = @SUID ORDER BY UPR.ID;";
                    cmd.Parameters.AddWithValue("@SUID", selectedUser.Id);

                    await using var rd = await cmd.ExecuteReaderAsync();

                    List<string> roles = [];

                    while (await rd.ReadAsync())
                    {
                        if (rd["Funkcje"].ToString() != "Administrator Aplikacji" && App.ADMIN != 1)
                        {
                            roles.Add(rd["Funkcje"].ToString());
                        }
                        else if (rd["Funkcje"].ToString() != "Przewodniczący" && (App.ADMIN != 1 || App.PUA != 1))
                        {
                            roles.Add(rd["Funkcje"].ToString());
                        }
                        else
                        {
                            roles.Add(rd["Funkcje"].ToString());
                        }
                    }

                    rd.Close();

                    var selectedRole = await DisplayActionSheet($"Którą Rolę Usunąć Użytkownikowi {selectedUser.Name}", "Anuluj", null, [.. roles]);

                    if (selectedRole != "Anuluj")
                    {
                        int DPR;

                        switch (selectedRole)
                        {
                            case "Wiceprzewodniczący":
                                DPR = 2;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name);
                                break;
                            case "Członek Prezydium":
                                DPR = 3;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name);
                                break;
                            case "Delegat do RUSS":
                                DPR = 4;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name);
                                break;
                            case "Administrator Aplikacji":
                                DPR = 5;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name);
                                break;
                            case "Koordynator Sportu":
                                DPR = 6;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name);
                                break;
                            case "Osoba odpowiedzialna za Sociale":
                                DPR = 7;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name); ;
                                break;
                            case "Starosta Roku":
                                DPR = 8;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name);
                                break;
                            case "Wicestarosta Roku":
                                DPR = 9;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name);
                                break;
                            case "Członek RWSS":
                                DPR = 10;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name);
                                break;
                            case "Członek RWSS z możliwością wzięcia klucza z portierni":
                                DPR = 11;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name);
                                break;
                            case "Absolwent":
                                DPR = 12;
                                DeleteRole(selectedUser.Id, DPR, selectedUser.Name);
                                break;
                        }

                    }
                }
            }
        }



        public async void ButtonVis()
        {
            AddButtonFrame.IsVisible = false;

            if (App.PUA >= 1 && App.PUA <= 4)
            {
                AddButtonFrame.IsVisible = true;
            }
            else if (App.ADMIN == 1)
            {
                AddButtonFrame.IsVisible = true;
            }
        }

        private async void AddNewUser(object sender, EventArgs e)
        {
            if (sender is ImageButton clickedButton && clickedButton.Equals(AddButtonVis))
            {
                var shell = (Shell)App.Current.MainPage;
                var navigation = shell.CurrentItem?.CurrentItem?.CurrentItem?.Navigation;
                if (navigation != null)
                {
                    await navigation.PushAsync(new AddNewUser());
                }
            }
        }

        private async void ChangePresident(int CUR, int PR)
        {
            await using var cdp = App.Connection.CreateCommand();
            cdp.CommandText = @"DELETE FROM Użytkownicy_Uprawnienia WHERE ID_Uprawnienia = 1 OR ID_Uprawnienia = 2 OR ID_Uprawnienia = 3 OR ID_Uprawnienia = 4;";
            using var readStatus = await cdp.ExecuteReaderAsync();

            readStatus.Close();

            await using var csnp = App.Connection.CreateCommand();
            csnp.CommandText = @"INSERT INTO Użytkownicy_Uprawnienia (ID_Użytkownika, ID_Uprawnienia) VALUES (@NPUID, @PID);";
            csnp.Parameters.AddWithValue("@NPUID", CUR);
            csnp.Parameters.AddWithValue("@PID", PR);

            using var sendNP = await csnp.ExecuteReaderAsync();

            sendNP.Close();

            await DisplayAlert("Zmieniono przewodniczącego", "Zmiana przewodniczącego zakończona pomyślnie", "OK");
        }

        private async void ChangeRole(int CUR, int RID, string name, string NR)
        {
            await using var csnp = App.Connection.CreateCommand();
            csnp.CommandText = @"INSERT INTO Użytkownicy_Uprawnienia (ID_Użytkownika, ID_Uprawnienia) VALUES (@NPUID, @PID);";
            csnp.Parameters.AddWithValue("@NPUID", CUR);
            csnp.Parameters.AddWithValue("@PID", RID);

            using var sendNP = await csnp.ExecuteReaderAsync();

            sendNP.Close();

            await DisplayAlert("Dodano rolę dla " + name, $"Dodano rolę {NR} dla Użytkownika " + name, "OK");
        }

        private async void ChangeAdmin(int CUR, int PR)
        {
            await using var cdp = App.Connection.CreateCommand();
            cdp.CommandText = @"DELETE FROM Użytkownicy_Uprawnienia WHERE ID_Uprawnienia = 5;";
            using var readStatus = await cdp.ExecuteReaderAsync();

            readStatus.Close();

            await using var csnp = App.Connection.CreateCommand();
            csnp.CommandText = @"INSERT INTO Użytkownicy_Uprawnienia (ID_Użytkownika, ID_Uprawnienia) VALUES (@NPUID, @PID);";
            csnp.Parameters.AddWithValue("@NPUID", CUR);
            csnp.Parameters.AddWithValue("@PID", PR);

            using var sendNP = await csnp.ExecuteReaderAsync();

            sendNP.Close();

            await DisplayAlert("Zmieniono Admina", "Już nie zarządzasz aplikacją HEHE ;P", "OK");
        }

        private async void DeleteRole(int CUR, int DPR, string name)
        {
            await using var cdp = App.Connection.CreateCommand();
            cdp.CommandText = @"DELETE FROM Użytkownicy_Uprawnienia WHERE ID_Użytkownika = @NPUID AND ID_Uprawnienia = @DPR;";
            cdp.Parameters.AddWithValue("@NPUID", CUR);
            cdp.Parameters.AddWithValue("@DPR", DPR);

            using var readStatus = await cdp.ExecuteReaderAsync();

            readStatus.Close();

            await DisplayAlert("Usunięto Rolę", $"Usunięto Rolę Użytkownikowi {name}", "OK");
        }



    }

    public class SetUser
    {
        // Właściwości użytkownika
        public int ColumnWidth { get; set; }
        public bool IsMoreButtonVisible { get; set; }
        public bool IsMessageButtonVisible { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Role { get; set; } = "";
        public string KiR { get; set; } = "";
        public string Image { get; set; } = "";
        public string LIMS { get; set; } = "";
    }


}
