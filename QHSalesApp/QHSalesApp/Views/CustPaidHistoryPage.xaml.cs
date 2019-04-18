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
    public partial class CustPaidHistoryPage : ContentPage
    {
        private List<CustLedgerEntry> objList { get; set; }
        // private int intPageId { get; set; }
        private string CustNo { get; set; }
        private bool _isloading;
        public bool IsLoading
        {
            get { return this._isloading; }
            set
            {
                this._isloading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public CustPaidHistoryPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            //this.ToolbarItems.Add(new ToolbarItem { Text = "Back", Command = new Command(this.BackPage) });
            Customer customer = new Customer();
            customer = App.gCustomer;

            CustNo = customer.CustomerNo;
           // this.Title = customer.CustomerNo + " - Payment History";
            TitleLabel.Text = customer.CustomerNo + " - Payment History";
            this.BackgroundColor = Color.FromHex("#dddddd");
            sbSearch.Placeholder = "Search by Document No,Date";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            IsLoading = false;
            BindingContext = this;
        }

        void BackPage()
        {
            Navigation.PushAsync(new MainPage(5));
        }

        private void TapGestureRecognizer_Tapped_5(object sender, EventArgs e)
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

        private void FilterKeyword(string filter)
        {
            if (objList == null) return;
            listview.BeginRefresh();
            if (string.IsNullOrWhiteSpace(filter))
            {
                listview.ItemsSource = objList.OrderBy(x => x.TransDate);

            }
            else
            {
                listview.ItemsSource = objList.Where(x => x.DocNo.ToLower().Contains(filter.ToLower()) ||
                x.TransDate.ToString().ToLower().Contains(filter.ToLower()));
            }
            listview.EndRefresh();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    objList = new List<CustLedgerEntry>();
                    DataManager manager = new DataManager();
                    objList = await manager.GetSQLite_CustomerLedgerEntry(CustNo);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        listview.ItemsSource = objList != null ? objList.OrderBy(x => x.TransDate) : null;
                        listview.Unfocus();
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    });
                }
                catch (OperationCanceledException ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
            });
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