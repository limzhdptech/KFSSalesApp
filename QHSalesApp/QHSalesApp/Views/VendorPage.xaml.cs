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
    public partial class VendorPage : ContentPage
    {
        private List<Vendor> objList { get; set; }
        public ListView listview { get { return LookupListView; } }
        public VendorPage()
        {
            InitializeComponent();
            this.Title = "Vendor List";
            sbSearch.Placeholder = "Search by Vendor No or Name";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
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
                    objList = new List<Vendor>();
                    DataManager manager = new DataManager();
                    objList = await manager.GetSQLite_Vendors();

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LookupListView.ItemsSource = objList != null ? objList : null;
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
            List<Vendor> filterItems = new List<Vendor>();
            if (objList != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    LookupListView.ItemsSource = objList;

                }
                else
                {
                    filterItems = objList.Where(x => x.VendorNo.ToLower().Contains(filter.ToLower()) || x.VendorName.ToLower().Contains(filter.ToLower())).ToList();
                    LookupListView.ItemsSource = filterItems;
                }
            }

        }
    }
}