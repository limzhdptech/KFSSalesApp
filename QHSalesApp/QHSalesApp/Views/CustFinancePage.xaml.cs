using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustFinancePage : ContentPage
    {
        public CustFinancePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
           // this.ToolbarItems.Add(new ToolbarItem { Text = "Back", Command = new Command(this.BackPage) });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Customer customer = new Customer();

            customer = App.gCustomer;
            this.BackgroundColor = Color.FromHex("#dddddd");
            //this.Title = customer.CustomerNo + " - Finance";
            TitleLabel.Text = customer.CustomerNo + " - Finance";
            CreditLimitLabel.Text = customer.CreditLimit;
            InvoiceLimitLabel.Text = customer.InvoiceLimit;
            OutstandingLabel.Text = customer.Outstanding;
            PaymentTermsLabel.Text = customer.PaymentTerms;
            CurrencyLabel.Text = customer.CurrencyCode;
            SalesPersonCodeLabel.Text = customer.SalesPersonCode;
        }

        void BackPage()
        {
            Navigation.PushAsync(new MainPage(5));
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(5));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CustDetailPage());
        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CustFinancePage());
        }

        private void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CustUnpaidBillPage());
        }

        private void TapGestureRecognizer_Tapped_4(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CustPaidHistoryPage());
        }

        private void TapGestureRecognizer_Tapped_5(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(5));
        }
    }
}