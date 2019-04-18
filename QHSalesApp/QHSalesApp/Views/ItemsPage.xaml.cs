using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        private List<Item> itemList { get; set; }
        // private int intPageId { get; set; }
        public ListView listview { get { return LookupListView; } }

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

        public ItemsPage()
        {
            InitializeComponent();
            this.Title = "Items List";
            sbSearch.Placeholder = "Search by Item No or Description";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            //DeleteButton.Clicked += DeleteButton_Clicked;
            
            IsLoading = false;
            BindingContext = this;
        }

       

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            DataManager dm = new DataManager();
            dm.DeleteItem();
        }

        //private async void ChangeDocumentStatus()
        //{
        //    this.ToolbarItems.Clear();

        //    if (App.gCurStatus == "picking")
        //    {
        //        this.Title = "Loaded List";
        //        App.gCurStatus = "loaded";
        //        // LoadButton.IsVisible = false;
        //        this.ToolbarItems.Add(new ToolbarItem { Text = "To Load List", Command = new Command(this.ChangeDocumentStatus) });
        //    }
        //    else
        //    {
        //        this.Title = "To Load List";
        //        App.gCurStatus = "picking";
        //        // LoadButton.IsVisible = true;
        //        this.ToolbarItems.Add(new ToolbarItem { Text = "Loaded List", Command = new Command(this.ChangeDocumentStatus) });
        //    }

        //    await LoadData();
        //}
        
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
                        itemList = new List<Item>();
                        DataManager manager = new DataManager();
                    if (App.gItems == null)
                        App.gItems = await manager.GetSQLite_Items();
                    itemList = App.gItems;
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
            List<Item> filterItems = new List<Item>();
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