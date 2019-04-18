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
    public partial class CustUnpaidBillPage : ContentPage
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

        public CustUnpaidBillPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasBackButton(this, false);
           // this.ToolbarItems.Add(new ToolbarItem { Text = "Back", Command = new Command(this.BackPage) });
            //if (EnableBackButtonOverride)
            //{
            //    this.CustomBackButtonAction = async () =>
            //    {
            //        var result = await this.DisplayAlert(null,
            //            "Hey wait now! are you sure " +
            //            "you want to go back?",
            //            "Yes go back", "Nope");

            //        if (result)
            //        {
            //            await Navigation.PopAsync(true);
            //        }
            //    };
            //}

            Customer customer = new Customer();
            customer = App.gCustomer;

            CustNo = customer.CustomerNo;
            //this.Title = customer.CustomerNo + " - Unpaid Bills";
            TitleLabel.Text = customer.CustomerNo + " - Unpaid Bills";
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
                        listview.ItemsSource = objList != null ? objList.OrderByDescending(x => x.TransDate) : null;
                        listview.Unfocus();
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    });
                }
                catch (OperationCanceledException ex)
                {
                    UserDialogs.Instance.HideLoading();  //IsLoading = false;
                    //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading();  //IsLoading = false;
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