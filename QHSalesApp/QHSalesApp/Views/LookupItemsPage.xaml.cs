using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LookupItemsPage : ContentPage
    {
        private List<Item> objList { get; set; }
        // private int intPageId { get; set; }
        public ListView listview { get { return LookupListView; } }
        private ItemViewModel itemService;

        public LookupItemsPage()
        {
            InitializeComponent();
            // intPageId = pageId;
            this.itemService = new ItemViewModel();
            this.Title = "Search Item";
            sbSearch.Placeholder = "Search by Item No or Description";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            DeleteButton.Clicked += DeleteButton_Clicked;
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            DataManager dm = new DataManager();
            dm.DeleteItem();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //this.BusyIndicator.IsVisible = true;
            //this.BusyIndicator.IsRunning = true;
            try
            {
                await this.itemService.PopulateDataAsync(false);
                // Data-binding:
                this.BindingContext = this.itemService.Items;
            }
            catch (InvalidOperationException ex)
            {
                await DisplayAlert("Error", "Check your network connection.", "OK");
                return;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
                return;
            }
            finally
            {
                //this.BusyIndicator.IsVisible = false;
                //this.BusyIndicator.IsRunning = false;
            }

        }

        private async Task LoadDataAsync()
        {
            //this.BusyIndicator.IsVisible = true;
            //this.BusyIndicator.IsRunning = true;
            try
            {
                await this.itemService.PopulateDataAsync(true);
                this.BindingContext = this.itemService.Items;
            }
            catch (InvalidOperationException ex)
            {
                await DisplayAlert("Error", "Check your network connection.", "OK");
                return;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
                return;
            }
            finally
            {
                //this.BusyIndicator.IsVisible = false;
                //this.BusyIndicator.IsRunning = false;
            }
        }
        //async Task RefreshData()
        // {
        //     IsLoading = true;
        //     Task.Run(async () =>
        //     {
        //         try
        //         {
        //             objList = new List<Item>();
        //             DataManager manager = new DataManager();
        //             objList = await manager.GetSQLite_Items();

        //             Device.BeginInvokeOnMainThread(() =>
        //             {
        //                 LookupListView.ItemsSource = objList != null ? objList : null;
        //                 LookupListView.Unfocus();
        //                 IsLoading = false;
        //             });
        //         }
        //         catch (OperationCanceledException ex)
        //         {
        //             IsLoading = false;
        //             DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
        //         }
        //         catch (Exception ex)
        //         {
        //             IsLoading = false;
        //             DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
        //         }
        //     });
        // }

        private void SearchItemsFilter(string filter)
        {
            List<Item> filterItems = new List<Item>();
            if (objList != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    LookupListView.ItemsSource = objList;

                }
                else
                {
                    filterItems = objList.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) || x.Description.ToLower().Contains(filter.ToLower())).ToList();
                    LookupListView.ItemsSource = filterItems;
                }
            }

        }

        private async void LookupListView_Refreshing(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }
    }
}