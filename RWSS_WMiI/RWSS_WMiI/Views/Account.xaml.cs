using RWSS_WMiI.ViewModels;

namespace RWSS_WMiI.Views
{
    public partial class Account : ContentPage
    { 
        int count = 0;

        public Account()
        {
            InitializeComponent();
            this.BindingContext = new AccountModel();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
