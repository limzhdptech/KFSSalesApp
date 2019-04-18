using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnloadVanPage : ContentPage
    {
        private ObservableCollection<Item> recItems { get; set; }
        public UnloadVanPage()
        {
            InitializeComponent();
            this.Title = "Unload Stock";
            this.BackgroundColor = Color.FromHex("#dddddd");
            sbSearch.Placeholder = "Search by Item No,Description";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            this.ToolbarItems.Add(new ToolbarItem { Text = "Confirm", Command = new Command(this.ConfrimUnloaded)});
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        async void ConfrimUnloaded()
        {
            string confirmText = string.Empty;

            try
            {
                DataManager manager = new DataManager();
                // to Sync SO and Credit Memo - HNN
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
                
                    ObservableCollection<Item> unloadItems = new ObservableCollection<Item>();
                    unloadItems = manager.GetSQLite_ItemtoUnload();
                    if (unloadItems != null)
                    {
                        if (unloadItems.Count > 0)
                        {
                            IDevice device = DependencyService.Get<IDevice>();
                            string deviceIdentifier = device.GetIdentifier();
                            foreach (Item itm in unloadItems)
                            {
                                confirmText = manager.ResetSqlite_Invenotry(itm.ItemNo);
                            }

                            confirmText = App.svcManager.UnloadHistoryReclasstoNAV(App.gSalesPersonCode, deviceIdentifier);
                            if (confirmText == "Success")
                            {

                                confirmText = manager.DeleteAllScanDocTables();
                                if (confirmText == "Success")
                                {
                                    UserDialogs.Instance.HideLoading();
                                    UserDialogs.Instance.ShowSuccess(confirmText, 3000);
                                    await LoadData();
                                }
                            }
                            else
                            {
                                UserDialogs.Instance.HideLoading();
                                UserDialogs.Instance.ShowError(confirmText, 3000);
                            }

                        }
                        else
                        {
                            UserDialogs.Instance.HideLoading();
                            UserDialogs.Instance.ShowError("No scanned items to unload!", 3000);
                        }
                    }
                
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
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
        private  void ScanButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            App.gPageTitle = "Scan Bag Label to Unload";
            Navigation.PushAsync(new BagLabelScanPage("Unloaded",string.Empty));
            //await Navigation.PushPopupAsync(new ScanChoosePage());
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
                string ret = App.svcManager.ExportNumSeries(App.gDeviceId, App.gSalesPersonCode, SOLastCode, CRLastCode, MPLastCode, RSLastCode, ULLastCode, SOLastSeries, CRLastSeries, MPLastSeries, RSLastSeries, ULLastSeries);
                //  UserDialogs.Instance.ShowError(App.gDeviceId +"- Message :" +ret, 5000);
            }

        }
        private void UnloadButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {

        }

        async Task LoadData()
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    recItems = new ObservableCollection<Item>();
                    DataManager manager = new DataManager();
                    recItems = manager.GetSQLite_ItemtoUnload();
                    Device.BeginInvokeOnMainThread(() =>
                    {

                        if (recItems != null)
                        {
                            listview.BeginRefresh();
                            listview.ItemsSource = recItems;
                            listview.EndRefresh();
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            UserDialogs.Instance.ShowError("No Data", 3000);
                        }

                        listview.Unfocus();
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    });

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
            });
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

        private async void ChangeButton_Clicked(object sender, EventArgs e)
        {
            var obj = (Button)sender;
            Item item = new Item();
            item = recItems.Where(x => x.ItemNo == obj.CommandParameter.ToString()).FirstOrDefault();
            if(item.ReturnQty>0)
            {
                Navigation.PushAsync(new CRQtyEntryPage(obj.CommandParameter.ToString()));
            }
            else
            {
                UserDialogs.Instance.ShowError("Not allow CR blance 0 to unload item!", 3000);
            }
        }
    }
}