using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Xamarin.Forms;

namespace QHSalesApp
{
    public partial class VanInventoryPage : ContentPage
    {
        private ObservableCollection<Item> recItems { get; set; }
        public VanInventoryPage()
        {
            InitializeComponent();
            
            this.Title = "Van Inventory";
            this.BackgroundColor = Color.FromHex("#dddddd");
            DataLayout.IsVisible = false;
            Emptylayout.IsVisible = true;
            sbSearch.Placeholder = "Search by Item No,Description";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            BindingContext = this;
        }
        private void ShowUnloaded()
        {
            Navigation.PushAsync(new UnloadHDPage());
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
            sbSearch.Unfocus();
        }
        void LoadData()
        {
            try
            {
                string retmsg = string.Empty;
                recItems = new ObservableCollection<Item>();
                DataManager manager = new DataManager();
                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {
                    List<VanItem> vitem = new List<VanItem>();
                    vitem= await manager.GetSQLite_VanItem();
                    recItems = manager.GetSQLite_ItemtoUnload();
                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {

                    UserDialogs.Instance.HideLoading();
                    if (recItems != null)
                    {
                        if (recItems.Count > 0)
                        {
                            listview.ItemsSource = null;
                            listview.BeginRefresh();
                            listview.ItemsSource = recItems.OrderBy(x=>x.Description);//.Where(x=> x.Balance!=0);
                            listview.EndRefresh();
                            DataLayout.IsVisible = true;
                            Emptylayout.IsVisible = false;
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            retmsg = string.Empty;
                            DataLayout.IsVisible = false;
                            Emptylayout.IsVisible = true;
                        }
                    }
                    else
                    {
                        listview.ItemsSource = null;
                        retmsg = string.Empty;
                        DataLayout.IsVisible = false;
                        Emptylayout.IsVisible = true;
                    }
                    listview.Unfocus();
                    if (!string.IsNullOrEmpty(retmsg))
                        UserDialogs.Instance.ShowError(retmsg, 3000);
                }));
            }
            catch (OperationCanceledException ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }
        private void FilterKeyword(string filter)
        {
            if (recItems == null) return;
            listview.BeginRefresh();
            if (string.IsNullOrWhiteSpace(filter))
            {
                listview.ItemsSource = recItems.OrderBy(x => x.Description);

            }
            else
            {
                List<Item> filterItems = new List<Item>();
                filterItems= recItems.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) ||
                 x.Description.ToString().ToLower().Contains(filter.ToLower())).ToList();
                listview.ItemsSource = filterItems.OrderBy(x => x.Description);
            }
            listview.EndRefresh();
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(0));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

    }
}
