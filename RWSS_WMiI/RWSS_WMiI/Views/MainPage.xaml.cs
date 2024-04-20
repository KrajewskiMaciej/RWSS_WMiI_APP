namespace RWSS_WMiI.Views
{
    public partial class MainPage : ContentPage
    {
        public int status = 0;
        string imie = "Maciek";
        string sala = " sala A3/17";

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            status++;

            if (status == 1)
                CounterBtn1.Text = $"Kliknięte {status} raz";
            else
                CounterBtn1.Text = $"Kliknięte {status} razy";

            if (status == 5)
                status = 0;

            SemanticScreenReader.Announce(CounterBtn1.Text);
        }

        private void KeyStatus(object obj, EventArgs e)
        {
            if (App.PUA >= 1 && App.PUA <= 9)
            {
                switch (status)
                {
                    case 0:
                        ChckStat.Text = $"Biuro otwarte";
                        break;

                    case 1:
                        ChckStat.Text = $"Klucz w bunkrze";
                        break;

                    case 2:
                        ChckStat.Text = $"Klucz na portierni";
                        break;

                    case 3:
                        ChckStat.Text = $"Klucz ma {imie} {sala}";
                        break;

                    default:
                        ChckStat.Text = $"Nieznane położenie klucza";
                        break;

                }
            }
            else
            {
                if(status >= 1)
                {
                    ChckStat.Text = $"Biuro zamknięte";
                }
                else
                {
                    ChckStat.Text = $"Biuro otwarte";
                }
                
            }
            SemanticScreenReader.Announce(ChckStat.Text);
        }
    }

}
