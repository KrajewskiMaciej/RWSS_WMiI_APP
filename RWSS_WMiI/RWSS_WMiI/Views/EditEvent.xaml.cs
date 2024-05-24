using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWSS_WMiI.Views
{
    public partial class EditEvent : ContentPage
    {
        public string EventName { get; set; }
        public string EventText { get; set; }
        public string EventLink { get; set; }
        public string Reminder { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
        public int eventID {  get; set; }
        public string NewEventName { get; set; }
        public string NewEventText { get; set; }
        public string NewEventLink { get; set; }
        public string NewReminder { get; set; }
        public DateTime NewEventStart { get; set; }
        public DateTime NewEventEnd { get; set; }
        public EditEvent()
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

        public EditEvent(int eventID) : this()
        {
            this.eventID = eventID;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            LoadEditedEvent(eventID);
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

        void OnEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            string oldText = e.OldTextValue;
            string newText = e.NewTextValue;
            string myText = EventTextAdd.Text;
        }

        public async Task LoadEditedEvent(int EEID)
        {
            await using var cmd = App.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Wydarzenia WHERE ID = @EEID;";
            cmd.Parameters.AddWithValue("@EEID", EEID);

            await using var rd = await cmd.ExecuteReaderAsync();
            if (rd.HasRows)
            {
                while (await rd.ReadAsync())
                {
                        NewEventName = rd["Nazwa"].ToString();
                        SavedName.Text = NewEventName;
                        NewEventText = rd["Opis"].ToString();
                        EventTextAdd.Text = NewEventText;
                        NewEventLink = rd["Link_Do_Wydarzenia"].ToString();
                        SavedLink.Text = NewEventLink;
                        NewEventStart = rd.GetDateTime("Data_Od");
                        EventStartDate.SelectedDate = NewEventStart;
                        NewEventEnd = rd.GetDateTime("Data_Do");
                        EventEndDate.SelectedDate = NewEventEnd;
                }
            }
            rd.Close();
        }

        public async void CheckEditedEvent(object obj, EventArgs e)
        {
            EventText = EventTextAdd.Text;

            SaveStartDate();
            SaveEndDate();

            if (string.IsNullOrEmpty(EventName) || string.IsNullOrEmpty(EventText))
            {
                CheckEditedEventPlaceholder.Text = "*Błąd przy edytowaniu wydarzenia! ";
                CheckEditedEventPlaceholder.TextColor = Colors.Red;

                if (string.IsNullOrEmpty(EventName))
                {
                    CheckEditedEventPlaceholder.Text += "\n Nazwa nie może być pusta";
                }

                if (string.IsNullOrEmpty(EventText))
                {
                    CheckEditedEventPlaceholder.Text += "\n Opis nie może być pusty";
                }


            }
            else
            {
                if (EventEnd.Date == DateTime.Now.Date)
                {
                    await using var command = App.Connection.CreateCommand();
                    command.CommandText = @"UPDATE Wydarzenia SET Nazwa = @EVENT_NAME, Opis = @EVENT_TEXT, Data_Od = @DATE_START, Link_Do_Wydarzenia = @LINK WHERE ID = @EEID;";
                    
                    if(NewEventName ==EventName) { command.Parameters.AddWithValue("@EVENT_NAME", NewEventName); }
                    else { command.Parameters.AddWithValue("@EVENT_NAME", EventName); }

                    if(NewEventText == EventText) { command.Parameters.AddWithValue("@EVENT_TEXT", NewEventText); }
                    else { command.Parameters.AddWithValue("@EVENT_TEXT", EventText); }
                    
                    if(NewEventStart == EventStart) { command.Parameters.AddWithValue("@DATE_START", NewEventStart); }
                    else { command.Parameters.AddWithValue("@DATE_START", EventStart); }
                    
                    if(NewEventLink == EventLink) { command.Parameters.AddWithValue("@LINK", NewEventLink); }
                    else { command.Parameters.AddWithValue("@LINK", EventLink); }

                    command.Parameters.AddWithValue("@EEID", eventID);

                    await using var read = await command.ExecuteReaderAsync();

                    read.Close();

                }
                else
                {
                    await using var command = App.Connection.CreateCommand();
                    command.CommandText = @"UPDATE Wydarzenia SET Nazwa = @EVENT_NAME, Opis = @EVENT_TEXT, Data_Od = @DATE_START, Data_Do = @DATE_END, Link_Do_Wydarzenia = @LINK WHERE ID = @EEID;";

                    if (NewEventName == EventName) { command.Parameters.AddWithValue("@EVENT_NAME", NewEventName); }
                    else { command.Parameters.AddWithValue("@EVENT_NAME", EventName); }

                    if (NewEventText == EventText) { command.Parameters.AddWithValue("@EVENT_TEXT", NewEventText); }
                    else { command.Parameters.AddWithValue("@EVENT_TEXT", EventText); }

                    if (NewEventStart == EventStart) { command.Parameters.AddWithValue("@DATE_START", NewEventStart); }
                    else { command.Parameters.AddWithValue("@DATE_START", EventStart); }

                    if (NewEventEnd == EventEnd) { command.Parameters.AddWithValue("@DATE_END", NewEventEnd); }
                    else { command.Parameters.AddWithValue("@DATE_END", EventEnd); }

                    if (NewEventLink == EventLink) { command.Parameters.AddWithValue("@LINK", NewEventLink); }
                    else { command.Parameters.AddWithValue("@LINK", EventLink); }

                    command.Parameters.AddWithValue("@EEID", eventID);

                    await using var read = await command.ExecuteReaderAsync();

                    read.Close();
                }
                CheckEditedEventPlaceholder.Text = "Wydarzenie edytowane pomyślnie!";
                CheckEditedEventPlaceholder.TextColor = Colors.Green;
                await Shell.Current.GoToAsync($"//{nameof(Info)}");
            }
        }

    }
}
