using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LookupUOMPage : ContentPage
    {
        private ObservableCollection<ItemUOM> objList { get; set; }
        // private int intPageId { get; set; }
        public ListView listview { get { return LookupListView; } }
        public string ItemNo { get; set; }
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

        public LookupUOMPage(string itemNo)
        {
            InitializeComponent();
            this.Title = "Customer List";
            sbSearch.Placeholder = "Search by Customer No or Name";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            
            ItemNo = itemNo;
            IsLoading = false;
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
                    objList = new ObservableCollection<ItemUOM>();
                    DataManager manager = new DataManager();
                    objList = await manager.GetItemUOMbyItemNo(ItemNo);

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
                   // DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
            });

        }

        private void SearchItemsFilter(string filter)
        {
            List<ItemUOM> filterItems = new List<ItemUOM>();
            if (objList != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    LookupListView.ItemsSource = objList;

                }
                else
                {
                    filterItems = objList.Where(x => x.UOMCode.ToLower().Contains(filter.ToLower())).ToList();
                    LookupListView.ItemsSource = filterItems;
                }
            }

        }
    }
}