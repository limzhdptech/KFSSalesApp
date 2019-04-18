using Acr.UserDialogs;
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
    public partial class CustomerTabbedPage : TabbedPage
    {
        public CustomerTabbedPage()
        {
            InitializeComponent();

            try
            {
                var navigationPage = new NavigationPage(new CustBillToPage());
                navigationPage.Icon = "billto.png";
                navigationPage.Title = "BillTo";

                Children.Add(new CustDetailPage());
                Children.Add(navigationPage);
                Children.Add(new CustFinancePage());
            }
            catch (Exception ex)
            {

                //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
           

            //var naviPage = new NavigationPage(new CustDetailPage());
            //naviPage.Icon = "info.png";
            //naviPage.Title = "Customer Detail";

            //Children.Add(new CustFinancePage());// { Icon = "finance.png", Title = "Finance" }
            //this.Children[0].Title = "Finance";
            //this.Children[0].Icon = "finance.png";
            //Children.Add(new CustBillToPage());
            //this.Children[1].Title = "Bill To";
            //this.Children[1].Icon = "billto.png";
            //Children.Add(new CustUnpaidBillPage());
            //this.Children[2].Title = "Unpaid Bill";
            //this.Children[2].Icon = "invoice.png";
            //Children.Add(new CustPaidHistoryPage());
            //this.Children[3].Title = "Payment History";
            //this.Children[3].Icon = "history.png";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
           
        }
    }
}