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
    public partial class PriceLookupPage : ContentPage
    {
        public ListView pricelistview { get { return LookupListView; } }
        List<SalesPrice> lstPrice = new List<SalesPrice>();
        private string ItemNo { get; set; }
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

        public PriceLookupPage(string item_no,string cust_no)
        {
            InitializeComponent();
            ItemNo = item_no;
            CustNo = cust_no;
            this.Title = "Price List";
            //sbSearch.Placeholder = "Search by Item No or Description";
            //sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            //sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            IsLoading = false;
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    //DataManager dm = new DataManager();
                    //ObservableCollection<SalesPrice> salesPrices = new ObservableCollection<SalesPrice>();
                    //salesPrices = await dm.GetItemPricebyItemNo(itemNo, salesCode);

                        lstPrice = new List<SalesPrice>();
                        DataManager manager = new DataManager();
                       lstPrice =   manager.GetItemPricebyItemNo(ItemNo,CustNo);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LookupListView.ItemsSource = lstPrice!= null ? lstPrice : null;
                        LookupListView.Unfocus();
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

        //private void SearchItemsFilter(string filter)
        //{
        //    List<Item> filterItems = new List<Item>();
        //    if (App.glstItem != null)
        //    {
        //        if (string.IsNullOrWhiteSpace(filter))
        //        {
        //            LookupListView.ItemsSource = App.glstItem;
        //        }
        //        else
        //        {
        //            filterItems = App.glstItem.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) || x.Description.ToLower().Contains(filter.ToLower())).ToList();
        //            LookupListView.ItemsSource = filterItems;
        //        }
        //    }

        //}
    }
}