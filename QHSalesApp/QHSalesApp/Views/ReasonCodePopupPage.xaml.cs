using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace QHSalesApp
{
    public partial class ReasonCodePopupPage : PopupPage
    {
        private List<ReasonCode> itemList { get; set; }
        public ListView Reasonlistview { get { return LookupListView; } }
        private string strSearch { get; set; }
        public ReasonCodePopupPage(string code)
        {
            InitializeComponent();
            strSearch = code;
            this.Title = "Reason Code List";
            sbSearch.Placeholder = "Search by Code or Description";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            App.gIsEnableReasonCode = true;
            await PopupNavigation.PopAsync();
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
                    itemList = new List<ReasonCode>();
                    DataManager manager = new DataManager();
                    itemList = await manager.GetSQLite_ReasonCodes(strSearch);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading(); 
                        LookupListView.ItemsSource = itemList != null ? itemList : null;
                        LookupListView.Unfocus();
                    });
                }
                catch (OperationCanceledException ex)
                {
                    UserDialogs.Instance.HideLoading(); 
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); 
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
            });
        }

        private void SearchItemsFilter(string filter)
        {
            List<ReasonCode> filterItems = new List<ReasonCode>();
            if (itemList != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    LookupListView.ItemsSource = itemList;
                }
                else
                {
                    filterItems = itemList.Where(x => x.Code.ToLower().Contains(filter.ToLower()) || x.Description.ToLower().Contains(filter.ToLower())).ToList();
                    LookupListView.ItemsSource = filterItems;
                }
            }

        }
    }
}
