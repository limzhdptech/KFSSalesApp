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
        private List<Customer> custList { get; set; }
        private List<Customer> filteredCustList { get; set; }
        private enum FilterStates { FILTERED, UNFILTERED }
        private FilterStates currentState;
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
        private void ChangeFilter()
        {
            this.ToolbarItems.Clear();
            switch (currentState)
            {
                case FilterStates.FILTERED:
                    this.ToolbarItems.Add(new ToolbarItem { Text = "DEFAULT", Command = new Command(this.ChangeFilter) });
                    currentState = FilterStates.UNFILTERED;
                    listview.ItemsSource = custList;
                    break;
                case FilterStates.UNFILTERED:
                    this.ToolbarItems.Add(new ToolbarItem { Text = "ALL", Command = new Command(this.ChangeFilter) });
                    currentState = FilterStates.FILTERED;
                    listview.ItemsSource = filteredCustList;
                    break;
            }
        }
        protected override void OnAppearing()
        {
            this.ToolbarItems.Clear();
            this.ToolbarItems.Add(new ToolbarItem { Text = "ALL", Command = new Command(this.ChangeFilter) });
            base.OnAppearing();
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    custList = new List<Customer>();
                    DataManager manager = new DataManager();
                    custList = await manager.GetSQLite_Customers();
                    custList = custList.OrderBy(x => x.Name).ToList();
                    filteredCustList = custList.Where(x => x.SalesPersonCode.ToLower().Contains(App.gSalesPersonCode.ToLower())).ToList();
                    //if (App.gCustomers == null)
                    //    App.gCustomers = await manager.GetSQLite_Customers();
                    //objList = App.gCustomers;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LookupListView.ItemsSource = filteredCustList != null ? filteredCustList : null;
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
            switch (currentState)
            {
                case FilterStates.FILTERED:
                    if (filteredCustList != null)
                    {
                        if (string.IsNullOrWhiteSpace(filter))
                        {
                            listview.ItemsSource = filteredCustList;
                        }
                        else
                        {
                            filterItems = filteredCustList.Where(x => x.CustomerNo.ToLower().Contains(filter.ToLower()) || x.Name.ToLower().Contains(filter.ToLower())).ToList();
                            listview.ItemsSource = filterItems;
                        }
                    }
                    break;
                case FilterStates.UNFILTERED:
                    if (custList != null)
                    {
                        if (string.IsNullOrWhiteSpace(filter))
                        {
                            listview.ItemsSource = custList;
                        }
                        else
                        {
                            filterItems = custList.Where(x => x.CustomerNo.ToLower().Contains(filter.ToLower()) || x.Name.ToLower().Contains(filter.ToLower())).ToList();
                            listview.ItemsSource = filterItems;
                        }
                    }
                    break;
            }

        }
    }
}