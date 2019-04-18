using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QHSalesApp
{
    public partial class ScanChoosePage : PopupPage
    {
        public ScanChoosePage()
        {
            InitializeComponent();
        }

        private async void OnClose(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }
        //protected override Task OnAppearingAnimationEndAsync()
        //{
        //    return Content.FadeTo(0.5);
        //}

        //protected override Task OnDisappearingAnimationBeginAsync()
        //{
        //    return Content.FadeTo(1);
        //}
    }
}
