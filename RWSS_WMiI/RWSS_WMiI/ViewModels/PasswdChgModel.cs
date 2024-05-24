using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWSS_WMiI.Views;

namespace RWSS_WMiI.ViewModels
{
    public partial class PasswdChgModel : BaseViewModel
    {
        public Command GoToMainPage { get; }
        public PasswdChgModel() 
        {
            GoToMainPage = new Command(async () => await OnClickMainPage());
        }

        public async Task OnClickMainPage()
        {
            await Shell.Current.GoToAsync($"//{nameof(Main)}");
        }
    }
}
