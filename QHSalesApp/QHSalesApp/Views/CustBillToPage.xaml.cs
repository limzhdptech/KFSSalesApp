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
    public partial class CustBillToPage : ContentPage
    {
        public CustBillToPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Customer customer = new Customer();
            customer = App.gCustomer;
            AddressLabel.Text = customer.Address;
            Address2Label.Text = customer.Address2;
            CityLabel.Text = customer.City;
            PostCodeLabel.Text = customer.Postcode;
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
    }
}