using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QHSalesApp
{
    public partial class VanItemPage : ContentPage
    {
        private List<VanItem> itemList { get; set; }
        // private int intPageId { get; set; }
        public ListView listview { get { return LookupListView; } }

        private string curfilter { get; set; }
        //private string _filter;
        public VanItemPage()
        {
            InitializeComponent();
            this.Title = "Items List";
            sbSearch.Placeholder = "Search by Item No or Description";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            this.ToolbarItems.Add(new ToolbarItem { Text = "ALL", Command = new Command(this.ChangeFilter) });
            curfilter = "Default";
            BindingContext = this;
        }

        private async void ChangeFilter()
        {
            this.ToolbarItems.Clear();
            if (curfilter != "ALL")
            {
                this.ToolbarItems.Add(new ToolbarItem { Text = "Default", Command = new Command(this.ChangeFilter) });
                curfilter = "ALL";
            }
            else
            {
                this.ToolbarItems.Add(new ToolbarItem { Text = "ALL", Command = new Command(this.ChangeFilter) });
                curfilter = "Default";
            }
            LoadData();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        private void LoadData()
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    // if (itemList == null)
                    //{
                    itemList = new List<VanItem>();
                    string ifilter = "ALL";
                    DataManager manager = new DataManager();
                    if (curfilter == "Default") ifilter = App.gCustPriceGroup;
                    itemList = await manager.GetSQLite_VanItems(ifilter);
                    //}
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LookupListView.ItemsSource = itemList != null ? itemList.OrderBy(x=> x.Description) : null;
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
            List<VanItem> filterItems = new List<VanItem>();
            if (itemList != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    LookupListView.ItemsSource = itemList.OrderBy(x => x.Description);
                }
                else
                {
                    filterItems = itemList.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) || x.Description.ToLower().Contains(filter.ToLower())).ToList();
                    LookupListView.ItemsSource = filterItems.OrderBy(x => x.Description); 
                }
            }

        }
    }
}
