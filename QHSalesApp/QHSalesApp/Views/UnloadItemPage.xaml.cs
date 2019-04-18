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

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnloadItemPage : ContentPage
    {
        private ObservableCollection<Item> recItems { get; set; }
        public UnloadItemPage()
        {
            InitializeComponent();
            var semephone = new SemaphoreSlim(1);
            MessagingCenter.Subscribe<App>((App)Application.Current, "OnUnLoadData", async(sender) => {
                await semephone.WaitAsync();
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoadData();
                });
                semephone.Release();
            });
            this.Title = "Unload Stock";
            this.BackgroundColor = Color.FromHex("#dddddd");
            DataLayout.IsVisible = false;
            Emptylayout.IsVisible = true;
            sbSearch.Placeholder = "Search by Item No,Description";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            this.ToolbarItems.Add(new ToolbarItem { Text = "Unloaded", Command = new Command(this.ShowUnloaded) });
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
        }
        async Task<string> SyncSalesOrder()
        {
            try
            {
                ObservableCollection<SalesHeader> lsthd = new ObservableCollection<SalesHeader>();
                DataManager manager = new DataManager();
                string alertmsg = string.Empty;
                //lsthd = await manager.GetSQLite_SalesHeaderbyStatus("Released", "SO");
                // lsthd = await manager.GetSQLite_SalesHeaderbyStatus("Released", "CN");
                lsthd = await manager.GetSQLite_SalesOrderbyStatus("Released");
                if (lsthd != null && lsthd.Count > 0)
                {
                    foreach (SalesHeader s in lsthd)
                    {
                        string retmsg = App.svcManager.ExportSalesHeader(s.DocumentNo, s.SellToCustomer, s.SellToName, s.BillToCustomer, s.BillToName, s.DocumentDate, s.Status, s.PaymentMethod, s.TotalAmount, s.DocumentType, s.Note, s.StrSignature, App.gSalesPersonCode, App.gDeviceId, s.Comment,s.IsVoid,s.ExternalDocNo);
                        if (retmsg == "Success")
                        {
                            manager = new DataManager();
                            string retval = await manager.SaveSQLite_SalesHeader(new SalesHeader
                            {
                                ID = s.ID,
                                DocumentNo = s.DocumentNo,
                                DocumentDate = s.DocumentDate,
                                BillToCustomer = s.BillToCustomer,
                                BillToName = s.BillToName,
                                SellToCustomer = s.SellToCustomer,
                                SellToName = s.SellToName,
                                TotalAmount = s.TotalAmount,
                                GSTAmount = s.GSTAmount,
                                NetAmount = s.NetAmount,
                                Status = "Released",
                                PaymentMethod = s.PaymentMethod,
                                StrSignature = s.StrSignature,
                                DocumentType = s.DocumentType,
                                Note = s.Note,
                                Comment = s.Comment,
                                IsVoid=s.IsVoid,
                                IsSync = "true",
                                SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"),
                                ExternalDocNo=s.ExternalDocNo

                            });

                            ObservableCollection<SalesLine> lstline = new ObservableCollection<SalesLine>();
                            lstline = manager.GetSalesLinesbyDocNo(s.DocumentNo);
                            if (lstline != null && lstline.Count > 0)
                            {
                                foreach (SalesLine l in lstline)
                                {
                                    retmsg = App.svcManager.ExportSalesLine(l.ID.ToString(), l.DocumentNo, l.ItemNo, l.LocationCode, l.Quantity, l.FOCQty,l.BadQuantity, l.UnitofMeasurementCode, l.UnitPrice, l.LineDiscountPercent, l.LineDiscountAmount, l.LineAmount,l.GoodReasonCode,l.BadReasonCode,l.ItemType);
                                    if (retmsg == "Success")
                                    {
                                        manager = new DataManager();
                                        SalesLine line = new SalesLine()
                                        {
                                            ID = l.ID,
                                            DocumentNo = l.DocumentNo,
                                            ItemNo = l.ItemNo,
                                            Description = l.Description,
                                            UnitofMeasurementCode = l.UnitofMeasurementCode,
                                            Quantity = l.Quantity,
                                            FOCQty = l.FOCQty,
                                            UnitPrice = l.UnitPrice,
                                            //IsSync = "true",
                                            //SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt")
                                        };
                                        retval = manager.SaveSQLite_SalesLine(line);
                                        //alertmsg = "Sales Order sync success!";
                                        alertmsg = "Success";
                                    }
                                    else
                                    {
                                        alertmsg = "Can not able to sync doc no " + l.DocumentNo + " -> Item No" + l.ItemNo + ". Err message: " + retmsg;
                                    }
                                }
                            }
                            else
                            {
                                alertmsg = "No line records";
                                break;
                            }
                            // Sync Num series
                            SyncNumberSeries();
                        }
                        else
                        {
                            alertmsg = "Can not able to sync doc No " + s.DocumentNo + ". Err message: " + retmsg;
                            break;
                        }
                    }
                }
                else
                {
                    alertmsg = "No records";
                }
                return alertmsg;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                return ex.Message.ToString();
            }
        }
        async Task<string> SyncPayment()
        {
            try
            {
                string alertmsg = string.Empty;
                ObservableCollection<Payment> lsthd = new ObservableCollection<Payment>();
                DataManager manager = new DataManager();
                lsthd = await manager.GetSQLite_PaymentbyStatus("Released");
                if (lsthd != null && lsthd.Count > 0)
                {
                    foreach (Payment s in lsthd)
                    {
                        string retmsg = App.svcManager.ExportPayment(s.DocumentNo, s.OnDate, s.CustomerNo, s.PaymentMethod, s.Amount, s.CustomerSignature, s.Imagestr, s.SalesPersonCode, s.Note, s.RecStatus, s.RefDocumentNo, s.SourceType);
                        if (retmsg == "Success")
                        {
                            manager = new DataManager();
                            Dictionary<int, string> dicResult = new Dictionary<int, string>();
                            dicResult = await manager.SaveSQLite_Payment(new Payment
                            {
                                ID = s.ID,
                                DocumentNo = s.DocumentNo,
                                OnDate = s.OnDate,
                                CustomerNo = s.CustomerNo,
                                CustomerName = s.CustomerName,
                                PaymentMethod = s.PaymentMethod,
                                Amount = s.Amount,
                                CustomerSignature = s.CustomerSignature,
                                SalesPersonCode = s.SalesPersonCode,
                                Note = s.Note,
                                Imagestr = s.Imagestr,
                                RecStatus = s.RecStatus,
                                RefDocumentNo = s.RefDocumentNo,
                                SourceType = s.SourceType,
                                //IsSync = "true",
                                //SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt")
                            });

                            alertmsg = "Success";
                            SyncNumberSeries();
                        }
                        else
                        {
                            alertmsg = "Can not able to sync " + s.DocumentNo + ". Err message: " + retmsg;
                            break;
                        }
                    }
                }
                else
                {
                    alertmsg = "No records";
                }
                return alertmsg;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }

        }
        private async void SyncNumberSeries()
        {
            DataManager dm = new DataManager();
            List<NumberSeries> seriesList = new List<NumberSeries>();
            seriesList = await dm.GetSQLite_NumberSeries();
            string SOLastCode = string.Empty;
            string CRLastCode = string.Empty;
            string MPLastCode = string.Empty;
            string RSLastCode = string.Empty;
            string ULLastCode = string.Empty;
            string SOLastSeries = string.Empty;
            string CRLastSeries = string.Empty;
            string MPLastSeries = string.Empty;
            string RSLastSeries = string.Empty;
            string ULLastSeries = string.Empty;
            if (seriesList != null && seriesList.Count > 0)
            {
                foreach (NumberSeries s in seriesList)
                {
                    switch (s.Description)
                    {
                        case "SO":
                            SOLastCode = s.LastNoCode;
                            SOLastSeries = s.LastNoSeries.ToString();
                            break;
                        case "CR":
                            CRLastCode = s.LastNoCode;
                            CRLastSeries = s.LastNoSeries.ToString();
                            break;
                        case "CP":
                            MPLastCode = s.LastNoCode;
                            MPLastSeries = s.LastNoSeries.ToString();
                            break;
                        case "RS":
                            RSLastCode = s.LastNoCode;
                            RSLastSeries = s.LastNoSeries.ToString();
                            break;
                        case "UL":
                            ULLastCode = s.LastNoCode;
                            ULLastSeries = s.LastNoSeries.ToString();
                            break;
                    }
                }
                string ret = App.svcManager.ExportNumSeries(App.gDeviceId, App.gSalesPersonCode, SOLastCode, CRLastCode, MPLastCode, RSLastCode,ULLastCode, SOLastSeries, CRLastSeries, MPLastSeries, RSLastSeries,ULLastSeries);
                //  UserDialogs.Instance.ShowError(App.gDeviceId +"- Message :" +ret, 5000);
            }

        }
        private  void ConfirmButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            this.IsEnabled = false;
            string confirmText = string.Empty;
            try
            {
                DataManager manager = new DataManager();
                ObservableCollection<SalesHeader> shd = new ObservableCollection<SalesHeader>();
               
                ObservableCollection<Item> unloadItems = new ObservableCollection<Item>();
                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {
                    bool checkval = true;
                    int recCount = 0;
                    shd = await manager.GetSQLite_SalesOrderbyStatus("Open");
                    if(shd.Count>0)
                    {
                        foreach (SalesHeader h in shd)
                        {
                            recCount = await manager.GetSQLite_SalesLineCount(h.DocumentNo);
                            if (recCount > 0)
                            {
                                checkval = false;
                                break;
                            }   
                        }
                    }
                    else
                    {
                        checkval = true;
                    }
                    
                    if(!checkval)
                    {
                        confirmText = "Unable to unload! Outstanding SO/CR are remaining";
                    }
                    else
                    {
                        unloadItems = manager.GetSQLite_ItemtoUnload();
                        if (unloadItems != null)
                        {
                            if (unloadItems.Count > 0)
                            {
                                string UnloadDocNo = manager.GetLastNoSeries(App.gULPrefix);
                                IDevice device = DependencyService.Get<IDevice>();
                                string deviceIdentifier = device.GetIdentifier();
                                foreach (Item itm in unloadItems)
                                {
                                    //confirmText = App.svcManager.ExportUnloadHistory(deviceIdentifier, itm.ItemNo, itm.UnloadQty, itm.SoldQty, itm.ReturnQty, itm.BalQty, App.gSalesPersonCode);
                                    manager = new DataManager();
                                    UnloadLine line = new UnloadLine()
                                    {
                                        ID = 0,
                                        HeaderEntryNo = UnloadDocNo,
                                        EntryNo = Guid.NewGuid().ToString(),
                                        UserID = App.gUserID,
                                        ItemNo = itm.ItemNo,
                                        ItemDesc = itm.Description,
                                        Quantity = itm.UnloadQty,
                                        GoodQty = itm.LoadQty + itm.ReturnQty - itm.SoldQty,
                                        BadQty = itm.BadQty,
                                        ItemUom = itm.BaseUOM
                                    };
                                    await manager.SaveSQLite_UnloadLine(line);
                                    confirmText = manager.ResetSqlite_Invenotry(itm.ItemNo);
                                }
                                if (confirmText == "Success")
                                {
                                    manager = new DataManager();
                                    string retval = await manager.SaveSQLite_UnloadHeader(new UnloadHeader
                                    {
                                        ID = 0,
                                        EntryNo = Guid.NewGuid().ToString(),
                                        SalesPersonCode = App.gSalesPersonCode,
                                        UnloadDocNo = UnloadDocNo,
                                        UnloadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"),
                                        IsSync = "false",
                                        SyncDateTime = string.Empty,
                                        CurStatus = "unloaded"
                                    });
                                    await manager.DeleteSQLite_VanItem();
                                    manager.DeleteChangedItem();
                                    manager.IncreaseNoSeries(App.gULPrefix, UnloadDocNo, "UL");
                                }
                            }
                            else
                            {
                                confirmText = "No scanned items to unload!";
                            }
                        }
                    }
                    

                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    if (confirmText == "Success")
                    {
                        UserDialogs.Instance.ShowSuccess(confirmText, 3000);
                        Navigation.PushAsync(new UnloadHDPage());
                    }    
                    else
                        UserDialogs.Instance.ShowError(confirmText, 3000);
                    //LoadData();
                }));
                this.IsEnabled = true;
            }
            catch (OperationCanceledException ex)
            {
                this.IsEnabled = true;
                Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            catch (Exception ex)
            {
                this.IsEnabled = true;
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
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
                    recItems = manager.GetSQLite_ItemtoUnload();
                    if(recItems!=null)
                    {
                        if(recItems.Count>0)
                        {
                            foreach(Item i in recItems)
                            {
                                ChangedItem citm = new ChangedItem();
                                citm = manager.GetSQLite_ChangedItembyItemNo(i.ItemNo);
                                if(citm==null)
                                {
                                   await manager.UpdateSQLite_UnloadInventory(i.ItemNo, i.BalQty);
                                }
                            }
                        }
                        recItems.Clear();
                        recItems = manager.GetSQLite_ItemtoUnload();
                    }
                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {

                    UserDialogs.Instance.HideLoading();
                    if (recItems != null)
                    {
                        if (recItems.Count > 0)
                        {
                            listview.BeginRefresh();
                            listview.ItemsSource = recItems;
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
                listview.ItemsSource = recItems.OrderByDescending(x => x.ID);

            }
            else
            {
                listview.ItemsSource = recItems.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) ||
                x.Description.ToString().ToLower().Contains(filter.ToLower()));
            }
            listview.EndRefresh();
        }
        private async void UnloadButton_Clicked(object sender, EventArgs e)
        {

            var obj = (Button)sender;
            Item item = new Item();
            item = recItems.Where(x => x.ItemNo == obj.CommandParameter.ToString()).FirstOrDefault();
            if (item.BalQty > 0)
            {
                Navigation.PushPopupAsync(new EditQtyPage(item,"Unload"));
            }
            else
            {
                UserDialogs.Instance.ShowError("Not allow balance quanity 0 to unload!", 3000);
            }
        }

    }
}