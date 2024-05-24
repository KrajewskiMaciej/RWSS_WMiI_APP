using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWSS_WMiI.Views
{
    public partial class AddNewEvent : ContentPage
    {
        public string EventName { get; set; }
        public string EventText { get; set; }
        public string EventLink {  get; set; }
        public string Reminder { get; set; }
        public DateTime EventStart {  get; set; }
        public DateTime EventEnd {  get; set; }
        public AddNewEvent()
        {
            InitializeComponent();

            SendReminder.SelectedIndexChanged += (sender, e) =>
            {
                if (SendReminder.SelectedIndex != -1)
                {
                    Reminder = SendReminder.SelectedItem.ToString();
                }
            };

        }

        void SaveName(object sender, EventArgs e)
        {
            EventName = ((Entry)sender).Text;
        }

        void EventLinkSave(object sender, EventArgs e)
        {
            EventLink = ((Entry)sender).Text;
        }

        void SaveStartDate()
        {
            EventStart = EventStartDate.SelectedDate;
        }

        void SaveEndDate()
        {
            EventEnd = EventEndDate.SelectedDate;
        }

        void EventTextChanged(object sender, TextChangedEventArgs e)
        {
            string oldText = e.OldTextValue;
            string newText = e.NewTextValue;
            string myText = EventTextAdd.Text;
        }

        public async void CheckNewEvent(object obj, EventArgs e)
        {
            EventText = EventTextAdd.Text;

            SaveStartDate();
            SaveEndDate();

            if (string.IsNullOrEmpty(EventName) || string.IsNullOrEmpty(EventText) || EventStart.Date == DateTime.Now.Date)
            {
                CheckNewEventPlaceholder.Text = "*Błąd przy dodawaniu nowego wydarzenia! ";
                CheckNewEventPlaceholder.TextColor = Colors.Red;

                if(string.IsNullOrEmpty(EventName))
                {
                    CheckNewEventPlaceholder.Text += "\n Nazwa nie może być pusta";
                }

                if(string.IsNullOrEmpty(EventText))
                {
                    CheckNewEventPlaceholder.Text += "\n Opis nie może być pusty";
                }

                if(EventStart.Date == DateTime.Now.Date)
                {
                    CheckNewEventPlaceholder.Text += "\n Wydarzenie nie może odbywać się dzisiaj";
                }

            }
            else
            {
                if (EventEnd.Date == DateTime.Now.Date)
                {
                    await using var command = App.Connection.CreateCommand();
                    command.CommandText = @"INSERT INTO Wydarzenia (Nazwa, Opis, Data_Od, Link_Do_Wydarzenia, Kto_Dodal, Czy_Przypomniec) VALUES (@EVENT_NAME, @EVENT_TEXT, @DATE_START, @LINK, @WHO_ADD, @SEND_REMINDER)";

                    command.Parameters.AddWithValue("@EVENT_NAME", EventName);
                    command.Parameters.AddWithValue("@EVENT_TEXT", EventText);
                    command.Parameters.AddWithValue("@DATE_START", EventStart);
                    command.Parameters.AddWithValue("@LINK", EventLink);
                    command.Parameters.AddWithValue("@WHO_ADD", App.UID);
                    command.Parameters.AddWithValue("@SEND_REMINDER", Reminder);

                    await using var read = await command.ExecuteReaderAsync();

                    read.Close();

                }
                else
                {
                    await using var command = App.Connection.CreateCommand();
                    command.CommandText = @"INSERT INTO Wydarzenia (Nazwa, Opis, Data_Od, Data_Do, Link_Do_Wydarzenia, Kto_Dodal, Czy_Przypomniec) VALUES (@EVENT_NAME, @EVENT_TEXT, @DATE_START, @DATE_END, @LINK, @WHO_ADD, @SEND_REMINDER)";

                    command.Parameters.AddWithValue("@EVENT_NAME", EventName);
                    command.Parameters.AddWithValue("@EVENT_TEXT", EventText);
                    command.Parameters.AddWithValue("@DATE_START", EventStart);
                    command.Parameters.AddWithValue("@DATE_END", EventEnd);
                    command.Parameters.AddWithValue("@LINK", EventLink);
                    command.Parameters.AddWithValue("@WHO_ADD", App.UID);
                    command.Parameters.AddWithValue("@SEND_REMINDER", Reminder);

                    await using var read = await command.ExecuteReaderAsync();

                    read.Close();
                }
                CheckNewEventPlaceholder.Text = "Wydarzenie dodane pomyślnie!";
                CheckNewEventPlaceholder.TextColor = Colors.Green;
                await Shell.Current.GoToAsync($"//{nameof(Info)}");
            }
        }

    }
}
