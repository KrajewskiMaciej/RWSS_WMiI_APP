using Microsoft.Maui.Controls;

namespace RWSS_WMiI
{
    public class DateTimePicker : StackLayout
    {
        DatePicker datePicker;
        TimePicker timePicker;

        public DateTimePicker()
        {
            datePicker = new DatePicker();
            timePicker = new TimePicker();
            datePicker.Date = DateTime.Now;
            Orientation = StackOrientation.Horizontal;



            // Dodaj kontrolki do DateTimePicker (czyli do StackLayout)
            Children.Add(datePicker);
            Children.Add(timePicker);

        }

        public DateTime SelectedDate
        {
            get { return datePicker.Date + timePicker.Time; }
            set { datePicker.Date = value; }
        }

    }
}
