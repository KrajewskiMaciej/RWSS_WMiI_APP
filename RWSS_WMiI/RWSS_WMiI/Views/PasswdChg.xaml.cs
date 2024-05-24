using RWSS_WMiI.ViewModels;
using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RWSS_WMiI.Views
{
    public partial class PasswdChg : ContentPage
    {
        public Command GoToMainPagekCommand { get; }
        public static string oldPasswd { get; set; } = "";
        public static string newPasswd { get; set; } = "";
        public static string tmpPasswd { get; set; } = "";
        public PasswdChg() 
        {
            InitializeComponent();
            this.BindingContext = new PasswdChgModel();
        }

        void SaveOldPassword(object sender, EventArgs e)
        {
            oldPasswd = ((Entry)sender).Text;
        }

        void SaveNewPassword(object sender, EventArgs e)
        {
            newPasswd = ((Entry)sender).Text;
        }

        void SaveTmpPassword(object sender, EventArgs e)
        {
            tmpPasswd = ((Entry)sender).Text;
        }

        public async void CheckPasswdAccurate(object obj, EventArgs e)
        {
            PasswdChgModel viewModel = (PasswdChgModel)this.BindingContext;

            if (App.Connection.State == ConnectionState.Closed)
            {
                await App.Connection.OpenAsync();
            }
            string storedPassword = "";

            using var command = App.Connection.CreateCommand();
            command.CommandText = @"SELECT Hasło FROM Użytkownicy WHERE ID = @UID";
            command.Parameters.AddWithValue("@UID", App.UID);

            using var read = await command.ExecuteReaderAsync();
            if (read.HasRows)
            {
                while (await read.ReadAsync())
                {
                    storedPassword = read["Hasło"].ToString();
                }

            }
            read.Close();

            if (PasswordHasher.VerifyPassword(oldPasswd, storedPassword))
            {
                string chck1 = newPasswd;
                string chck2 = tmpPasswd;
                
                if(newPasswd == tmpPasswd) 
                {
                    if(IsPasswordSecure(newPasswd))
                    {
                        var isOK = await DisplayAlert("Hasło spełnia zasady bezpieczeństwa", "Zapisać nowe hasło?", "Tak", "Nie");
                        if(isOK.Equals(true))
                        {
                            DateTime firstLogin = DateTime.Now;

                            string newPasswdHashed = PasswordHasher.HashPassword(newPasswd);

                            using var cmd = App.Connection.CreateCommand();
                            cmd.CommandText = @"UPDATE Użytkownicy SET Hasło = @noweHasło, PierwszeLogowanie = @pierwszaData WHERE ID = @UID;";
                            cmd.Parameters.AddWithValue("@UID", App.UID);
                            cmd.Parameters.AddWithValue("@noweHasło", newPasswdHashed);
                            cmd.Parameters.AddWithValue("@pierwszaData", firstLogin);

                            using var rd = await cmd.ExecuteReaderAsync();

                            rd.Close();
                            await viewModel.OnClickMainPage();
                        }

                    }
                    else
                    {
                        DisplayPasswordAlert();
                   }
                }
                else
                {
                    CheckPasswd.Text = $"Hasła nie są takie same";
                }
            }
            else
            {
                CheckPasswd.Text = $"Stare hasło jest niepoprawne";
            }


            SemanticScreenReader.Announce(CheckPasswd.Text);
        }

        static bool IsPasswordSecure(string password)
        {
            // Sprawdzenie długości hasła
            if (password.Length < 8)
                return false;

            // Sprawdzenie obecności wielkich liter
            bool hasUpperCase = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c))
                {
                    hasUpperCase = true;
                    break;
                }
            }
            if (!hasUpperCase)
                return false;

            // Sprawdzenie obecności małych liter
            bool hasLowerCase = false;
            foreach (char c in password)
            {
                if (char.IsLower(c))
                {
                    hasLowerCase = true;
                    break;
                }
            }
            if (!hasLowerCase)
                return false;

            // Sprawdzenie obecności cyfr
            bool hasDigit = false;
            foreach (char c in password)
            {
                if (char.IsDigit(c))
                {
                    hasDigit = true;
                    break;
                }
            }
            if (!hasDigit)
                return false;

            // Sprawdzenie obecności znaków specjalnych
            string specialChars = "!@#$%^&*()-_=+";
            bool hasSpecialChar = false;
            foreach (char c in password)
            {
                if (specialChars.Contains(c))
                {
                    hasSpecialChar = true;
                    break;
                }
            }
            if (!hasSpecialChar)
                return false;

            return true;
        }

        public async void DisplayPasswordAlert()
        {
            await DisplayAlert("Hasło nie spełnia wymogów bezpieczeństwa", "1. Ma co najmniej 8 znaków.\r\n2. Zawiera co najmniej jedną wielką literę.\r\n3.Zawiera co najmniej jedną małą literę.\r\n4. Zawiera co najmniej jedną cyfrę.\r\n5. Zawiera co najmniej jeden znak specjalny (np. !@#$%^&*()-_=+).", "OK");
        }
    }
}
