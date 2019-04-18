using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerPage : ContentPage
    {
        private List<Customer> objList { get; set; }
        // private int intPageId { get; set; }
        public ListView listview { get { return LookupListView; } }

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

        public CustomerPage()
        {
            InitializeComponent();
            // intPageId = pageId;
            this.Title = "Search Customer";
            sbSearch.Placeholder = "Search by Customer No or Name";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            DeleteButton.Clicked += DeleteButton_Clicked;
            IsLoading = false;
            BindingContext = this;
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            DataManager dm = new DataManager();
            dm.DeleteItem();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    objList = new List<Customer>();
                    DataManager manager = new DataManager();
                    objList = await manager.GetSQLite_Customers();
                    //if (App.gCustomers == null)
                    //    App.gCustomers = await manager.GetSQLite_Customers();
                    //objList = App.gCustomers;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LookupListView.ItemsSource = objList != null ? objList.OrderBy(x=>x.Name) : null;
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

        private void SearchItemsFilter(string filter)
        {
            List<Customer> filterItems = new List<Customer>();
            if (objList != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    LookupListView.ItemsSource = objList.OrderBy(x => x.Name);

                }
                else
                {
                    filterItems = objList.Where(x => x.CustomerNo.ToLower().Contains(filter.ToLower()) || x.Name.ToLower().Contains(filter.ToLower())).ToList();
                    LookupListView.ItemsSource = filterItems.OrderBy(x=>x.Name);
                }
            }

        }
    }
}