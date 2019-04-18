using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QHSalesApp
{
    public partial class UserListPage : ContentPage
    {
        private List<SalesPerson> personList { get; set; }
        public UserListPage()
        {
            InitializeComponent();

            this.Title = "Sales Person List";
            sbSearch.Placeholder = "Search by Sales Person Code";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            LookupListView.ItemSelected += LookupListView_ItemSelected;
            BindingContext = this;
        }

        private void LookupListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            var selectedItem = e.SelectedItem as SalesPerson;

            Navigation.PushAsync(new UnloadReturnPage(selectedItem.SalesPersoncode));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    if (personList == null)
                    {
                        personList = new List<SalesPerson>();
                        DataManager manager = new DataManager();
                        personList = await manager.GetSalesPersonList();
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LookupListView.ItemsSource = personList != null ? personList : null;
                        LookupListView.Unfocus();
                        LookupListView.SelectedItem = null;
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
            List<SalesPerson> filterItems = new List<SalesPerson>();
            if (personList != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    LookupListView.ItemsSource = personList;
                }
                else
                {
                    filterItems = personList.Where(x => x.SalesPersoncode.ToLower().Contains(filter.ToLower())).ToList();
                    LookupListView.ItemsSource = filterItems;
                }
            }

        }
    }
}
