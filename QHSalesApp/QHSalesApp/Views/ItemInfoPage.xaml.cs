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
    public partial class ItemInfoPage : ContentPage
    {
        private List<Item> itemList { get; set; }
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
        public ItemInfoPage()
        {
            InitializeComponent();
            DataLayout.IsVisible = false;
            EmptyLayout.IsVisible = true;
            this.BackgroundColor = Color.FromHex("#dddddd");
            // intPageId = pageId;
            this.Title = "Items";
            sbSearch.Placeholder = "Search by Item No or Description";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            listview.ItemTapped += Listview_ItemTapped;
            IsLoading = false;
            BindingContext = this;
            EmptyDataLayout.IsVisible = false;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(0));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    itemList = new List<Item>();
                    DataManager manager = new DataManager();
                    if (App.gItems == null)
                        App.gItems = await manager.GetSQLite_Items();
                    itemList = App.gItems;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if(itemList!=null)
                        {
                            if (itemList.Count > 0)
                            {
                                DataLayout.IsVisible = true;
                                EmptyLayout.IsVisible = false;
                                listview.ItemsSource = itemList.OrderBy(x=> x.Description);
                            }
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            DataLayout.IsVisible = false;
                            EmptyLayout.IsVisible = true;
                        }
                       // listview.ItemsSource = itemList != null ? itemList : null;
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

        private void SearchItemsFilter(string filter)
        {
            List<Item> filterItems = new List<Item>();
            if (itemList != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    listview.ItemsSource = itemList.OrderBy(x => x.Description);

                }
                else
                {
                    filterItems = itemList.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) || x.Description.ToLower().Contains(filter.ToLower())).ToList();
                    listview.ItemsSource = filterItems.OrderBy(x => x.Description);
                }
            }

        }

        private void Listview_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
            var item = (Item)e.Item;
            App.gItem = new Item();
            App.gItem = item;
           Navigation.PushAsync(new ItemDetailPage());
        }
    }
}