using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QHSalesApp
{
    public partial class ReportPage : ContentPage
    {
        private string printReport { get; set; }
        public ReportPage()
        {
            InitializeComponent();
            //var semephone = new SemaphoreSlim(1);
            //MessagingCenter.Subscribe<App>((App)Application.Current, "OnPrintData", async (sender) => {
            //    await semephone.WaitAsync();
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        Print_Report();
            //    });
            //    semephone.Release();
            //});
            this.Title = "Report";
            NavigationPage.SetHasBackButton(this, false);
            this.BackgroundColor = Color.FromHex("#dddddd");
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(0));
            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private async void ActivityButton_Clicked(object sender, EventArgs e)
        {
            printReport = "Activity";
            //Navigation.PushPopupAsync(new EditQtyPage(item,"Unload"));
            try
            {
                var answer = await DisplayAlert("Print", "Are you sure to print Activity Report?", "Yes", "No");
                if (answer)
                {
                     Navigation.PushPopupAsync(new DatePopupPage(printReport));
                }     
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }

        private async void DailySalesInvButton_Clicked(object sender, EventArgs e)
        {
            printReport = "SalesSummary";
            //Navigation.PushPopupAsync(new EditQtyPage(item,"Unload"));
            try
            {
                var answer = await DisplayAlert("Print", "Are you sure to print Daily Sales Invoices Summary Report?", "Yes", "No");
                if (answer)
                {
                    Navigation.PushPopupAsync(new DatePopupPage(printReport));
                    //MessagingCenter.Unsubscribe<App>((App)Application.Current, "OnPrintData");
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }

        private async void VoidReportButton_Clicked(object sender, EventArgs e)
        {
            //Navigation.PushPopupAsync(new EditQtyPage(item,"Unload"));
            printReport = "Void";
            try
            {

                var answer = await DisplayAlert("Print", "Are you sure to print Void Report?", "Yes", "No");
                if (answer)
                {
                    Navigation.PushPopupAsync(new DatePopupPage(printReport));
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }
        private async void DailySalesReturnButton_Clicked(object sender, EventArgs e)
        {
            //Navigation.PushPopupAsync(new EditQtyPage(item,"Unload"));
            printReport = "SalesReturn";
            try
            {

                var answer = await DisplayAlert("Print", "Are you sure to print Daily Sales Return Report?", "Yes", "No");
                if (answer)
                {
                    Navigation.PushPopupAsync(new DatePopupPage(printReport));
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }
        
      
    }
}
