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
    public partial class CustDetailPage : ContentPage
    {
        public CustDetailPage()
        {
            InitializeComponent();
            //NavigationPage.SetHasBackButton(this, false);
            //this.ToolbarItems.Add(new ToolbarItem { Text = "Back", Command = new Command(this.BackPage) });
            Customer customer = new Customer();
            customer = App.gCustomer;
           // NavigationPage.SetHasNavigationBar(this, false);
            
            if (customer!=null)
            {

                this.Title = customer.CustomerNo + " - Detail";
                //TitleLabel.Text = customer.CustomerNo + " - Detail";
                this.BackgroundColor = Color.FromHex("#dddddd");
                customerNoLabel.Text = customer.CustomerNo;
                NameLabel.Text = customer.Name;
                Name2Label.Text = customer.Name2;
                ContactLabel.Text = customer.Contact;
                AddressLabel.Text = customer.Address;
                Address2Label.Text = customer.Address2;
                cityLabel.Text = customer.City;
                PostCodeLabel.Text = customer.Postcode;
                CountryCodeLabel.Text = customer.CountryCode;
            }
        }
        void BackPage()
        {
            Navigation.PushAsync(new MainPage(5));
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
           
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
            // Navigation.PushAsync(new CustPriceHisPage(App.gCustomer.CustomerNo));
            Navigation.PushAsync(new CustPaidHistoryPage());
        }

        private void TapGestureRecognizer_Tapped_5(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(5));
        }
    }
}