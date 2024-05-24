

namespace RWSS_WMiI.Views
{
    public partial class Start : ContentPage
    {
        public Start()
        {
                InitializeComponent();
            
        }

        private async void OnStudentButtonClicked(object sender, EventArgs e)
        {
            await UserPreferences.SaveUserTypeAsync("Student");
            await Shell.Current.GoToAsync($"//{nameof(Main)}");
        }

        private async void OnRwssButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

    }

}
