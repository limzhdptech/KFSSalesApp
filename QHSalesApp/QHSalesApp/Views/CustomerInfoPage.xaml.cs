﻿using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerInfoPage : ContentPage
    {

        private List<Customer> custList { get; set; }
        private List<Customer> filteredCustList { get; set; }
        private enum FilterStates { FILTERED, UNFILTERED }
        private FilterStates currentState;

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

        public CustomerInfoPage()
        {
            InitializeComponent();
            DataLayout.IsVisible = false;
            EmptyLayout.IsVisible = true;
            this.BackgroundColor = Color.FromHex("#dddddd");
            // intPageId = pageId;
            this.Title = "Customers";
            sbSearch.Placeholder = "Search by Customer No or Name";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            listview.ItemTapped += Listview_ItemTapped;
            IsLoading = false;
            BindingContext = this;
            EmptyDataLayout.IsVisible = false;
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
                    //custList = App.gCustomers;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (filteredCustList != null)
                        {
                            if (filteredCustList.Count > 0)
                            {
                                DataLayout.IsVisible = true;
                                EmptyLayout.IsVisible = false;
                                listview.ItemsSource = filteredCustList;
                            }
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            DataLayout.IsVisible = false;
                            EmptyLayout.IsVisible = true;
                        }

                        //listview.ItemsSource = custList != null ? custList : null;
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

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(0));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
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

        private void Listview_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
            var item = (Customer)e.Item;
            App.gCustomer = new Customer();
            App.gCustomer = item;

            // if(App.gfromMenu=="Customers")
            Navigation.PushAsync(new CustDetailPage());
        }
    }
}