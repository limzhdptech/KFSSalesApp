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
    public partial class SalesHeaderPage : ContentPage
    {
        
        private ObservableCollection<SalesHeader> recItems { get; set; }
        private bool _isloading;
        private bool _isDetail { get; set; }
        private bool _isDelete { get; set; }
        private bool _isAdded { get; set; }
        public bool IsLoading
        {
            get { return this._isloading; }
            set
            {
                this._isloading = value;
                OnPropertyChanged("IsLoading");
            }
        }
        public SalesHeaderPage()
        {
            InitializeComponent();
            
            if (App.gDocType == "SO")
                this.Title = "Sales Order (Open)";
            else
                this.Title = "Credit Memo (Open)";
            DataLayout.IsVisible = false;
            Emptylayout.IsVisible = true;
            NavigationPage.SetHasBackButton(this, false);
            this.BackgroundColor = Color.FromHex("#dddddd");
            //this.ToolbarItems.Add(new ToolbarItem { Text = "Open", Command = new Command(this.LoadOpenSO) });
            this.ToolbarItems.Add(new ToolbarItem { Text = "Released", Command = new Command(this.LoadReleasedSO) });
            listview.ItemTapped += Listview_ItemTapped;
            sbSearch.Placeholder = "Search by Order No,Posting Date";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            
           // EmptyLayout.IsVisible = false;
            IsLoading = false;
            BindingContext = this;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(0));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }
        //async void LoadOpenSO()
        //{
        //    App.gSOStatus = "Open";
        //    this.Title = "Open Orders";
        //    await LoadData(App.gSOStatus);
        //}

        void LoadReleasedSO()
        {
           App.gSOStatus = "Released";
           //this.Title = "Released Sales Orders";
            Navigation.PushAsync(new MainPage(2));
        }

        private void Listview_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
            var item = (SalesHeader)e.Item;
            if (App.gDocType == "SO")
                App.gPageTitle = "Edit Sales Order (" + item.DocumentNo+")";
            else
                App.gPageTitle = "Edit Credit Memo (" + item.DocumentNo + ")";
            Navigation.PushAsync(new SalesOrderEntryPage(item.ID));
        }

        protected  override void OnAppearing()
        {
            base.OnAppearing();
            _isAdded = true;
            _isDetail = true;
            _isDelete = true;
            LoadData(App.gSOStatus);
       
            //if(listview.ItemsSource==null)
            //{
            //    DataLayout.IsVisible = false;
            //   EmptyLayout.IsVisible = true;
            //}
            //else
            //{
            //    DataLayout.IsVisible = true;
            //    EmptyLayout.IsVisible = false;
            //}
        }

        //async Task BindRecords(string docNo)
        //{
        //    DataTable dt = new DataTable();
        //    dt = await App.svcManager.RetrieveTfHeaders(docNo, App.DocuType);

        //    if (dt.Rows.Count > 0)
        //    {
        //        recItems = new ObservableCollection<TransferHeader>();
        //        //int i = 0;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            recItems.Add(new TransferHeader
        //            {
        //                ID = int.Parse(dr["EntryNo"].ToString()),
        //                DocumentNo = (dr["DocumentNo"].ToString()),
        //                DocumentType = dr["DocumentType"].ToString(),
        //                DocumentDate = dr["DocumentDate"].ToString(),
        //                FromLoc = dr["FromLoc"].ToString(),
        //                ToLoc = dr["ToLoc"].ToString(),
        //                Status = dr["Status"].ToString(),
        //            });
        //        }

        //    }
        //}

         void LoadData(string status)
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    recItems = new ObservableCollection<SalesHeader>();
                    DataManager manager = new DataManager();
                    recItems = await manager.GetSQLite_SalesHeaderbyStatus(status,App.gDocType);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (recItems != null)
                        {
                            if(recItems.Count>0)
                            {
                                listview.ItemsSource = recItems.OrderByDescending(x => x.ID);
                                DataLayout.IsVisible = true;
                                Emptylayout.IsVisible = false;
                            }
                            else
                            {
                                listview.ItemsSource = null;
                                DataLayout.IsVisible = false;
                                Emptylayout.IsVisible = true;
                            } 
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            DataLayout.IsVisible = false;
                            Emptylayout.IsVisible = true;
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
                catch (Exception ex1)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                    UserDialogs.Instance.ShowError(ex1.Message.ToString(), 3000);
                }
            });
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
                x.DocumentDate.ToString().ToLower().Contains(filter.ToLower()));
            }
            listview.EndRefresh();
        }

        private void DtlButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            var item = (ActionButton)sender;
            if (App.gDocType == "SO")
                App.gPageTitle = "Items List (Sales Order)";
            else
                App.gPageTitle = "Items List (Credit Memo)";
            SalesHeader head = new SalesHeader();
            head = recItems.Where(x => x.DocumentNo == item.CommandParameter.ToString()).FirstOrDefault();
            App.gCustCode = head.SellToCustomer;
            Navigation.PushAsync(new SalesLinePage(item.CommandParameter.ToString(),"Open"));
        }

        private void AddButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            if(_isAdded)
            {
                _isAdded = false;
                if (App.gDocType == "SO")
                    App.gPageTitle = "Add New Sales Order";
                else
                    App.gPageTitle = "Add New Credit Memo";
                Navigation.PushAsync(new SalesOrderEntryPage(0));
            }
            
        }

        private  void DeleteButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            var item = (ActionButton)sender;
            bool isConfirm = false;
            Task.Run(async () =>
            {
                var answer = await UserDialogs.Instance.ConfirmAsync("Are you sure to delete?", "Delete", "Yes", "No");
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (answer)
                    {
                        isConfirm = true;
                    }
                    else
                    {
                        isConfirm = false;
                    }
                });
            });
            if (isConfirm)
            {
                DataManager manager = new DataManager();
                    
                SalesHeader hd = new SalesHeader();
                hd = manager.GetSalesHeaderbyID(int.Parse(item.CommandParameter.ToString()));
                ObservableCollection<SalesLine> recItems = new ObservableCollection<SalesLine>();
                recItems =  manager.GetSalesLinesbyDocNo(hd.DocumentNo);
                if (recItems != null)
                {
                    if(recItems.Count>0)
                    {
                        foreach(SalesLine s in recItems)
                        {
                            manager.DeleteScannedSoldDocbyBagNo(s.BagNo);
                        }
                        manager.DeleteSalesLinebyDocNo(hd.DocumentNo);
                    }
                }
                manager.DeleteSalesOrderbyID(int.Parse(item.CommandParameter.ToString()));
                LoadData(App.gSOStatus);
            }
        }

        private void DetailTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //var item = (GestureRecognizer)sender;
            //App.gPageTitle = "Sales Items";
            //SalesHeader head = new SalesHeader();
            //head = recItems.Where(x => x.DocumentNo == item.CommandParameter.ToString()).FirstOrDefault();
            //App.gCustCode = head.SellToCustomer;
            //Navigation.PushAsync(new SalesLinePage(head.DocumentNo));
        }

        private async Task DeleteTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //var answer = await DisplayAlert("Delete Sales Order", "Are you sure to delete?", "Yes", "No");
            //if (answer)
            //{
            //    DataManager manager = new DataManager();
            //    manager.DeleteSalesOrderbyID(id);
            //}
        }

        private void DetailButton_Clicked(object sender, EventArgs e)
        {
           if(_isDetail)
            {
                _isDetail = false;
                var item = (Button)sender;

                if (App.gDocType == "SO")
                    App.gPageTitle = "Items List (Sales Order)";
                else
                    App.gPageTitle = "Items List (Credit Memo)";
                SalesHeader head = new SalesHeader();
                head = recItems.Where(x => x.DocumentNo == item.CommandParameter.ToString()).FirstOrDefault();
                App.gCustCode = head.SellToCustomer;
                DataManager manager = new DataManager();
                Customer customer = new Customer();
                customer = manager.GetSQLite_CustomerbyCustNo(head.SellToCustomer);
                if (customer != null)
                {
                    App.gCustPriceGroup = customer.CustomerPriceGroup;
                }
                else
                {
                    App.gCustPriceGroup = string.Empty;
                }
                Navigation.PushAsync(new SalesLinePage(item.CommandParameter.ToString(), "Open"));
            }
            
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            if(_isDelete)
            {
                var item = (Button)sender;
                try
                {
                    _isDelete = false;
                    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                    Task.Run(async () =>
                    {
                        var answer = await UserDialogs.Instance.ConfirmAsync("Are you sure to delete?", "Delete", "Yes", "No");
                        if (answer)
                        {

                            DataManager manager = new DataManager();

                            SalesHeader hd = new SalesHeader();
                            hd = manager.GetSalesHeaderbyID(int.Parse(item.CommandParameter.ToString()));
                            ObservableCollection<SalesLine> recItems = new ObservableCollection<SalesLine>();
                            recItems = manager.GetSalesLinesbyDocNo(hd.DocumentNo);
                            if (recItems != null)
                            {
                                if (recItems.Count > 0)
                                {
                                    //if (App.gDocType == "SO")
                                    //{
                                    foreach (SalesLine s in recItems)
                                    {
                                        Item iobj = new Item();
                                        iobj = manager.GetSQLite_ItembyItemNo(s.ItemNo);
                                        if (App.gDocType == "SO")
                                        {
                                            decimal soldQty = 0;
                                            if (s.ItemType == "EXC")
                                            {
                                                soldQty = iobj.SoldQty - s.BadQuantity;
                                                decimal excBadQty = iobj.BadQty - s.BadQuantity;
                                                manager.UpdateSQLite_ExchangeInventory(s.ItemNo, excBadQty);
                                            }
                                            else
                                                soldQty = iobj.SoldQty - s.Quantity;

                                            manager.UpdateSQLite_SOInventory(s.ItemNo, soldQty);
                                        }
                                        else
                                        {
                                            decimal returnQty = iobj.ReturnQty - s.Quantity;
                                            decimal badQty = iobj.BadQty - s.BadQuantity;
                                            manager.UpdateSQLite_ReturnInventory(s.ItemNo, returnQty, badQty);
                                        }
                                    }
                                    // }
                                    manager.DeleteSalesLinebyDocNo(hd.DocumentNo);
                                }
                            }
                            manager.DeleteSalesOrderbyID(int.Parse(item.CommandParameter.ToString()));
                            
                        }
                        _isDelete = true;
                    }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        LoadData(App.gSOStatus);
                    }));

                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
            }
            
        }

        private async Task<string> UpdateContainerInfoSoldQty(string bagno,decimal soldqty)
        {
            string retval = string.Empty;
            try
            {
                DataManager manager = new DataManager();
                ContainerInfo obj = new ContainerInfo();
                obj = await manager.GetContainerInfobyBagLabel(bagno);
                ContainerInfo tmpInfo = new ContainerInfo
                {
                    ID = obj.ID,
                    EntryNo = obj.EntryNo,
                    PalletNo = obj.PalletNo,
                    CartonNo = obj.CartonNo,
                    BoxNo = obj.BoxNo,
                    LineNo = obj.LineNo,
                    ItemNo = obj.ItemNo,
                    VariantCode = obj.VariantCode,
                    Quantity = obj.Quantity,
                    LoadQty = obj.LoadQty,
                    SoldQty = obj.LoadQty- soldqty,
                    UnloadQty = obj.UnloadQty,
                    LocationCode = obj.LocationCode,
                    BinCode = obj.BinCode,
                    RefDocNo = obj.RefDocNo,
                    RefDocLineNo = obj.RefDocLineNo,
                    RefDocType = obj.RefDocType,
                    MobileEntryNo = obj.MobileEntryNo
                };

                retval = await manager.UpdateSQLite_ContainerInfo(tmpInfo);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }

        }
    }
}

