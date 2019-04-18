using Acr.UserDialogs;
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
    public partial class ReleaseOrderPage : ContentPage
    {
        private ObservableCollection<SalesHeader> recItems { get; set; }
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

        public ReleaseOrderPage()
        {
            InitializeComponent();
            if(App.gDocType=="SO")
            this.Title = "Sales Order (Released)";
            else
                this.Title = "Credit Memo (Released)";
            NavigationPage.SetHasBackButton(this, false);
            this.BackgroundColor = Color.FromHex("#dddddd");
            this.ToolbarItems.Add(new ToolbarItem { Text = "Open", Command = new Command(this.LoadOpenSO) });
           ///this.ToolbarItems.Add(new ToolbarItem { Text = "Release", Command = new Command(this.LoadReleasedSO) });
            listview.ItemTapped += Listview_ItemTapped;
            sbSearch.Placeholder = "Search by Order No,Posting Date";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);

            EmptyDataLayout.IsVisible = false;
            IsLoading = false;
            BindingContext = this;
        }

         void LoadOpenSO()
        {
            App.gSOStatus = "Open";
            if (App.gDocType == "SO")
                this.Title = "Sales Order (Open)";
            else
                this.Title = "Credit Memo (Open)";
            //this.Title = "Released Sales Orders";
            Navigation.PushAsync(new MainPage(1));
            
            //   await LoadData(App.gSOStatus);
        }

        private void Listview_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
            var item = (SalesHeader)e.Item;

            if (App.gDocType == "SO")
                App.gPageTitle = "Items List (Sales Order)";
            else
                App.gPageTitle = "Items List (Credit Memo)";
            SalesHeader head = new SalesHeader();
            head = recItems.Where(x => x.DocumentNo == item.DocumentNo).FirstOrDefault();
            App.gCustCode = head.SellToCustomer;
            Navigation.PushAsync(new ReleaseLinePage(item.DocumentNo));
        }

        protected  override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData(App.gSOStatus);
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(0));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        async Task LoadData(string status)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
                Task.Run(async () =>
                {
                    try
                    {
                        recItems = new ObservableCollection<SalesHeader>();
                        DataManager manager = new DataManager();
                        recItems = await manager.GetSQLite_SalesHeaderbyStatus(status, App.gDocType);

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (recItems != null)
                            {
                                listview.ItemsSource = recItems.OrderByDescending(x => x.ID);
                            }
                            else
                            {
                                listview.ItemsSource = null;
                                //ShowDataLayout.IsVisible = false;
                                //EmptyDataLayout.IsVisible = true;
                            }


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
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }

        private void FilterKeyword(string filter)
        {
            if (recItems == null) return;
            listview.BeginRefresh();
            if (string.IsNullOrWhiteSpace(filter))
            {
                listview.ItemsSource = recItems.OrderByDescending(x => x.ID);

            }
            else
            {
                listview.ItemsSource = recItems.Where(x => x.DocumentNo.ToLower().Contains(filter.ToLower()) ||
                x.DocumentDate.ToString().ToLower().Contains(filter.ToLower()) || x.SellToCustomer.ToLower().Contains(filter.ToLower()));
            }
            listview.EndRefresh();
        }

        
    }
}