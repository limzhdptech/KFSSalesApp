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
    public partial class SyncPage : ContentPage
    {
        Dictionary<string, string> dict { get; set; }
        readonly Database database;
        private bool isItemSync { get; set; }
        private bool isCustomerSync { get; set; }
       // private bool isVendorSync { get; set; }
        //private bool isCustPricHisSync { get; set; }
        private bool isSalePriceSync { get; set; }

        private bool isPaymentMethodSync { get; set; }
        private bool isUomSync { get; set; }
        private bool isSalesOrderSync { get; set; }
        private bool isRequestOrderSync { get; set; }
       // private bool isPaymentSync { get; set; }
        private bool isCreditMemoSync { get; set; }

        private bool isUnloadStockSync { get; set; }
        // private bool isCustPriceHistSync { get; set; }
        private bool isMasterSync { get; set; }

        private string itemMsg { get; set; }
        private string priceMsg { get; set; }
        private string customerMsg { get; set; }
        //private string vendorMsg { get; set; }
        private string methodMsg { get; set; }
        private string RequestStockMsg { get; set; }
        private string salesMsg { get; set; }
       // private string paymentMsg { get; set; }
        //private string custLdgMsg { get; set; }
        //private string custPriceMsg { get; set; }
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

        public SyncPage()
        {
            InitializeComponent();
            this.Title = "Sync Data";
            isItemSync = false;
            isCustomerSync = false;
           // isVendorSync = false;
            //isCustPricHisSync = false;
            isSalePriceSync = false;
            isPaymentMethodSync = false;
            isSalesOrderSync = false;
           // isPaymentSync = false;

            isMasterSync = false;
            database = new Database(Constants.DatabaseName);
            SyncButton.Clicked += SyncButton_Clicked;
            SyncAllSwitch.Toggled += SyncAllSwitch_Toggled;
            SyncItemSwitch.Toggled += SyncItemSwitch_Toggled;
            SyncPriceSwitch.Toggled += SyncPriceSwitch_Toggled;
            SyncPaymentMethodsSwitch.Toggled += SyncPaymentMethodsSwitch_Toggled;
            SyncCustomereSwitch.Toggled += SyncCustomereSwitch_Toggled;


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

        private void SyncCustomereSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            //SyncItemSwitch.IsEnabled = false;
            //SyncPriceSwitch.IsEnabled = false;
            //SyncPaymentMethodsSwitch.IsEnabled = false;
            //SyncSalesOrderSwitch.IsEnabled = false;
            //SyncPaymentSwitch.IsEnabled = false;
        }

        private void SyncPaymentMethodsSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            //SyncItemSwitch.IsEnabled = false;
            //SyncCustomereSwitch.IsEnabled = false;
            //SyncPriceSwitch.IsEnabled = false;
            
            //SyncSalesOrderSwitch.IsEnabled = false;
            //SyncPaymentSwitch.IsEnabled = false;
        }

        private void SyncPriceSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            //SyncItemSwitch.IsEnabled = false;
            //SyncCustomereSwitch.IsEnabled = false;
            
            //SyncPaymentMethodsSwitch.IsEnabled = false;
            //SyncSalesOrderSwitch.IsEnabled = false;
            //SyncPaymentSwitch.IsEnabled = false;
        }

        private void SyncAllSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if(SyncAllSwitch.IsToggled==true)
            {
                SyncItemSwitch.IsEnabled = false;
               // SyncVendorSwitch.IsEnabled = false;
                SyncCustomereSwitch.IsEnabled = false;
                SyncPriceSwitch.IsEnabled = false;
                SyncPaymentMethodsSwitch.IsEnabled = false;
                //SyncCustPriceHisSwitch.IsEnabled = false;
            }
            else
            {
                SyncItemSwitch.IsEnabled = true;
               // SyncVendorSwitch.IsEnabled = true;
                SyncCustomereSwitch.IsEnabled = true;
                SyncPriceSwitch.IsEnabled = true;
                SyncPaymentMethodsSwitch.IsEnabled = true;
               // SyncCustPriceHisSwitch.IsEnabled = true;
            }

        }

        private void SyncItemSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            
            //SyncCustomereSwitch.IsEnabled = false;
            //SyncPriceSwitch.IsEnabled = false;
            //SyncPaymentMethodsSwitch.IsEnabled = false;
            //SyncSalesOrderSwitch.IsEnabled = false;
            //SyncPaymentSwitch.IsEnabled = false;
        }

        private void SyncButton_Clicked(object sender, EventArgs e)
        {
            string retmsg = string.Empty;
            string alertmsg = string.Empty;
            string syncflag = string.Empty;
            DataManager manager = new DataManager();
            dict = new Dictionary<string, string>();
            DependencyService.Get<INetworkConnection>().CheckNetworkConnection();
            if (DependencyService.Get<INetworkConnection>().IsConnected)
            {
                string result = DependencyService.Get<INetworkConnection>().IsServiceOnline(Helpers.Settings.GeneralSettings);
                if (result != "true")
                {
                    //DependencyService.Get<IMessage>().LongAlert("Error : Service is offline. [" + result + "]");
                    UserDialogs.Instance.ShowError("Error : Service is offline. [" + result + "]", 3000);
                    return;
                }
            }
            else
            {
                UserDialogs.Instance.ShowError("Error : No internet connection", 3000);
                return;
            }

            try
            {
                if (SyncAllSwitch.IsToggled == false && SyncItemSwitch.IsToggled == false && SyncPriceSwitch.IsToggled == false && SyncCustomereSwitch.IsToggled == false 
                    && SyncPaymentMethodsSwitch.IsToggled==false && SyncSalesOrderSwitch.IsToggled==false && SyncCreditMemoSwitch.IsToggled==false && SyncRequestOrderSwitch.IsToggled==false && SyncUnloadStockSwitch.IsToggled==false)//&& SyncPaymentSwitch.IsToggled== false && SyncVendorSwitch.IsToggled==false && SyncCustPriceHisSwitch.IsToggled==false 
                {
                    UserDialogs.Instance.ShowError("Required to select option(s) to sync!", 3000);
                    return;
                }
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
                Task.Run(async () =>
                {
                    try
                    {
                        if (SyncAllSwitch.IsToggled == true)
                        {
                            syncflag = "All";
                            database.CreateTable<Item>();
                            database.DatabaseInitialize<BarCodeInfo>();
                            database.DatabaseInitialize<ReasonCode>();
                            //database.DatabaseInitialize<ItemUOM>();
                            //database.DatabaseInitialize<Vendor>();
                            database.DatabaseInitialize<Customer>();
                            //database.DatabaseInitialize<CustomerPriceHistory>();
                            //database.DatabaseInitialize<CustLedgerEntry>();
                            database.DatabaseInitialize<SalesPrice>();
                            database.DatabaseInitialize<PaymentMethod>();
                            App.gItems = null;
                            itemMsg = await manager.SaveSQLite_Items();
                            await manager.SaveSQLite_BarCodeInfo();
                            await manager.SaveSQLite_ReasonCodes();
                            // retmsg = await manager.SaveSQLite_ItemUOms();
                            priceMsg = await manager.SaveSQLite_SalesPrices(App.gSalesPersonCode);
                            customerMsg = await manager.SaveSQLite_Customers(App.gSalesPersonCode);
      
                            methodMsg = await manager.SaveSQLite_PaymentMethods();
                            if (itemMsg == "Success" && priceMsg == "Success" && customerMsg == "Success" && methodMsg == "Success") //& custLdgMsg == "Success" && vendorMsg == "Success" && custPriceMsg=="Success" 
                            {
                                alertmsg = "Master data sync success!";
                                isMasterSync = true;
                                retmsg = "Success";
                            }
                            else
                            {
                                if (itemMsg != "Success")
                                    alertmsg = "Item failed! Err message: " + itemMsg;
                                if (priceMsg != "Success")
                                    alertmsg = "Sales Price failed! Err message: " + priceMsg;
                                if (customerMsg != "Success")
                                    alertmsg = "Customers failed! Err message: " + customerMsg;
                                //if (vendorMsg != "Success")
                                //    alertmsg = "Vendor failed! Err message: " + vendorMsg;
                                //if (custPriceMsg!= "Success")
                                //    alertmsg = "Customers Price History failed! Err message: " + custPriceMsg;
                                if (methodMsg != "Success")
                                    alertmsg = "Payment Method failed! Err message: " + methodMsg;
                                //if (custLdgMsg != "Success")
                                //    alertmsg = "Customer Ledger Entry failed! Err message: " + custLdgMsg;
                                isMasterSync = false;
                                retmsg = "Failed";
                            }
                            dict.Add(syncflag, retmsg);
                        }

                        if (SyncItemSwitch.IsToggled == true)
                        {
                            syncflag = "Item";
                            database.CreateTable<Item>();
                            database.DatabaseInitialize<BarCodeInfo>();
                            database.DatabaseInitialize<ReasonCode>();
                            App.gItems = null;
                            retmsg = await manager.SaveSQLite_Items();
                            await manager.SaveSQLite_BarCodeInfo();
                            await manager.SaveSQLite_ReasonCodes();
                            if (retmsg == "Success")
                            {
                                alertmsg = "Items sync success!";
                                isItemSync = true;
                            }
                            else
                            {
                                alertmsg = "Sync Item failed! Err message :" + retmsg;
                                retmsg = "Failed!";
                                isItemSync = false;
                            }
                            dict.Add(syncflag, retmsg);
                        }

                        if (SyncPriceSwitch.IsToggled == true)
                        {
                            syncflag = "SalesPrice";
                            database.DatabaseInitialize<SalesPrice>();
                            retmsg = await manager.SaveSQLite_SalesPrices(App.gSalesPersonCode);
                            if (retmsg == "Success")
                            {
                                alertmsg = "Sales Price sync success!";
                                isSalePriceSync = true;
                            }
                            else
                            {
                                retmsg = "Failed!";
                                alertmsg = "Sync sales price failed! Err message :" + retmsg;
                                isSalePriceSync = false;
                            }
                            dict.Add(syncflag, retmsg);
                        }

                        if (SyncCustomereSwitch.IsToggled == true)
                        {
                           // string custMsg = string.Empty;
                            syncflag = "Customer";
                            database.DatabaseInitialize<Customer>();
                            database.DatabaseInitialize<CustLedgerEntry>();
                            retmsg = await manager.SaveSQLite_Customers(App.gSalesPersonCode);
                            if (retmsg == "Success")
                            {
                                alertmsg = "Customer sync success!";
                                isCustomerSync = true;
                            }
                            else
                            {
                                alertmsg = "Sync customer failed! Err message :" + retmsg;
                                retmsg = "Failed!";
                                isCustomerSync = false;
                            }
                            dict.Add(syncflag, retmsg);
                        }

                        if (SyncPaymentMethodsSwitch.IsToggled == true)
                        {
                            syncflag = "PaymentMethod";
                            database.DatabaseInitialize<PaymentMethod>();
                            retmsg = await manager.SaveSQLite_PaymentMethods();
                            if (retmsg == "Success")
                            {
                                alertmsg = "Payment Method sync success!";
                                isPaymentMethodSync = true;
                            }
                            else
                            {
                                alertmsg = "Sync payment methods failed! Err message :" + retmsg;
                                retmsg = "Failed!";
                                isPaymentMethodSync = false;
                            }
                            //  IsLoading = false;
                            dict.Add(syncflag, retmsg);
                        }
                        // ----------------- Sync Transaction ----------------------------------------------------
                        //----------------------------------------------------------------------------------------
                        if (SyncRequestOrderSwitch.IsToggled == true)
                        {
                            ObservableCollection<RequestHeader> lsthd = new ObservableCollection<RequestHeader>();
                            lsthd = await manager.GetSQLite_RequestHeaderNotSync();
                            syncflag = "RequestOrder";
                            if (lsthd != null && lsthd.Count > 0)
                            {
                                foreach (RequestHeader objhead in lsthd)
                                {
                                    retmsg = App.svcManager.ExportRequestStock(objhead.EntryNo, App.gSalesPersonCode, objhead.RequestNo, objhead.RequestDate, "topick");
                                    if (retmsg == "Success")
                                    {
                                        manager = new DataManager();
                                        string retval = await manager.SaveSQLite_RequestHeader(new RequestHeader
                                        {
                                            ID = objhead.ID,
                                            EntryNo = objhead.EntryNo,
                                            SalesPersonCode = objhead.SalesPersonCode,
                                            RequestNo = objhead.RequestNo,
                                            RequestDate = objhead.RequestDate,
                                            IsSync = "true",
                                            SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"),
                                            CurStatus = objhead.CurStatus
                                        });

                                        ObservableCollection<RequestLine> lstline = new ObservableCollection<RequestLine>();
                                        lstline = await manager.GetRequestLinesbyDocNo(objhead.EntryNo);
                                        if (lstline != null && lstline.Count > 0)
                                        {
                                            foreach (RequestLine l in lstline)
                                            {
                                                retmsg = App.svcManager.ExportRequestLine(l.EntryNo, l.HeaderEntryNo, l.UserID, l.ItemNo, l.QtyperBag, l.NoofBags, l.Quantity, l.PickQty, l.LoadQty, l.UnloadQty, l.UomCode, l.VendorNo, l.InHouse, objhead.RequestNo);
                                                if (retmsg == "Success")
                                                {
                                                    manager = new DataManager();
                                                    RequestLine line = new RequestLine()
                                                    {
                                                        ID = l.ID,
                                                        EntryNo = l.EntryNo,
                                                        HeaderEntryNo = l.HeaderEntryNo,
                                                        UserID = l.UserID,
                                                        ItemNo = l.ItemNo,
                                                        ItemDesc = l.ItemDesc,
                                                        QtyperBag = l.QtyperBag,
                                                        NoofBags = l.NoofBags,
                                                        Quantity = l.Quantity,
                                                        PickQty = l.PickQty,
                                                        LoadQty = l.LoadQty,
                                                        UomCode = l.UomCode,
                                                        VendorNo = l.VendorNo,
                                                        InHouse = l.InHouse,
                                                        RequestNo = l.RequestNo,
                                                        IsSync = l.IsSync,
                                                        SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt")
                                                    };
                                                    retval = await manager.SaveSQLite_RequestLine(line);
                                                    alertmsg = "Request Order sync success!";
                                                    isRequestOrderSync = true;
                                                }
                                                else
                                                {
                                                    alertmsg = "Can not able to sync doc no " + objhead.RequestNo + " -> Item No" + l.ItemNo + ". Err message: " + retmsg;
                                                    retmsg = "Failed!";
                                                    isRequestOrderSync = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            retmsg = "No line records";
                                            alertmsg = retmsg;
                                            isRequestOrderSync = false;
                                        }
                                        // Sync Num series
                                        SyncNumberSeries();
                                    }
                                    else
                                    {
                                        alertmsg = "Can not able to sync doc No " + objhead.RequestNo + ". Err message: " + retmsg;
                                        retmsg = "Failed!";
                                        isRequestOrderSync = false;
                                        break;
                                    }
                                } //Loop End.
                            }
                            else
                            {
                                retmsg = "No records";
                                alertmsg = retmsg;
                                isRequestOrderSync = false;
                            }
                            dict.Add(syncflag, retmsg);
                        }

                        if (SyncSalesOrderSwitch.IsToggled == true)
                        {
                            ObservableCollection<SalesHeader> lsthd = new ObservableCollection<SalesHeader>();
                            lsthd = await manager.GetSQLite_UnsyncSalesHeaderbyStatus("Released", "SO");
                            syncflag = "SalesOrder";
                            if (lsthd != null && lsthd.Count > 0)
                            {
                                foreach (SalesHeader s in lsthd)
                                {
                                    retmsg = App.svcManager.ExportSalesHeader(s.DocumentNo, s.SellToCustomer, s.SellToName, s.BillToCustomer, s.BillToName, s.DocumentDate, s.Status, s.PaymentMethod, s.TotalAmount, s.DocumentType, s.Note, s.StrSignature,App.gSalesPersonCode,App.gDeviceId,s.Comment,s.IsVoid,s.ExternalDocNo);
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
                                            Comment=s.Comment,
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
                                                retmsg = App.svcManager.ExportSalesLine(l.ID.ToString(),l.DocumentNo, l.ItemNo, l.LocationCode, l.Quantity, l.FOCQty,l.BadQuantity, l.UnitofMeasurementCode, l.UnitPrice, l.LineDiscountPercent, l.LineDiscountAmount, l.LineAmount,l.GoodReasonCode,l.BadReasonCode,l.ItemType);
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
                                                        BadQuantity = l.BadQuantity,
                                                        FOCQty = l.FOCQty,
                                                        UnitPrice = l.UnitPrice,
                                                        GoodReasonCode = l.GoodReasonCode,
                                                        BadReasonCode = l.BadReasonCode,
                                                        ItemType = l.ItemType
                                                        //IsSync = "true",
                                                        //SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt")
                                                    };
                                                   retval = manager.SaveSQLite_SalesLine(line);
                                                    alertmsg = "Sales Order sync success!";
                                                    isSalesOrderSync = true;
                                                }
                                                else
                                                {
                                                    alertmsg = "Can not able to sync doc no " + l.DocumentNo + " -> Item No" + l.ItemNo + ". Err message: " + retmsg;
                                                    retmsg = "Failed!";
                                                    isSalesOrderSync = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            retmsg = "No line records";
                                            alertmsg = retmsg;
                                            isSalesOrderSync = false;
                                        }
                                        // Sync Num series
                                        SyncNumberSeries();
                                    }
                                    else
                                    {
                                        alertmsg = "Can not able to sync doc No " + s.DocumentNo + ". Err message: " + retmsg;
                                        retmsg = "Failed!";
                                        isSalesOrderSync = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                retmsg = "No records";
                                alertmsg = retmsg;
                                isSalesOrderSync = false;
                            }
                            dict.Add(syncflag, retmsg);
                        }

                        if (SyncCreditMemoSwitch.IsToggled == true)
                        {
                            ObservableCollection<SalesHeader> lsthd = new ObservableCollection<SalesHeader>();
                            lsthd = await manager.GetSQLite_UnsyncSalesHeaderbyStatus("Released", "CN");
                            syncflag = "CreditMemo";
                            if (lsthd != null && lsthd.Count > 0)
                            {
                                foreach (SalesHeader s in lsthd)
                                {
                                    retmsg = App.svcManager.ExportSalesHeader(s.DocumentNo, s.SellToCustomer, s.SellToName, s.BillToCustomer, s.BillToName, s.DocumentDate, s.Status, s.PaymentMethod, s.TotalAmount, s.DocumentType, s.Note, s.StrSignature,App.gSalesPersonCode,App.gDeviceId,s.Comment,s.IsVoid,s.ExternalDocNo);
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
                                            Comment=s.Comment,
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
                                                retmsg = App.svcManager.ExportSalesLine(l.ID.ToString(),l.DocumentNo, l.ItemNo, l.LocationCode, l.Quantity, l.FOCQty,l.BadQuantity, l.UnitofMeasurementCode, l.UnitPrice, l.LineDiscountPercent, l.LineDiscountAmount, l.LineAmount,l.GoodReasonCode,l.BadReasonCode,l.ItemType);
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
                                                        BadQuantity=l.BadQuantity,
                                                        FOCQty = l.FOCQty,
                                                        UnitPrice = l.UnitPrice,
                                                        GoodReasonCode=l.GoodReasonCode,
                                                        BadReasonCode=l.BadReasonCode,
                                                        ItemType=l.ItemType
                                                        //IsSync = "true",
                                                        //SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt")
                                                    };
                                                    retval = manager.SaveSQLite_SalesLine(line);
                                                    alertmsg = "Credit Memo sync success!";
                                                    isCreditMemoSync = true;
                                                }
                                                else
                                                {
                                                    alertmsg = "Can not able to sync doc no " + l.DocumentNo + " -> Item No" + l.ItemNo + ". Err message: " + retmsg;
                                                    retmsg = "failed!";
                                                    isCreditMemoSync = false;
                                                    break;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            retmsg = "No line records";
                                            isCreditMemoSync = false;
                                        }
                                        // Sync Num series
                                        SyncNumberSeries();
                                    }
                                    else
                                    {
                                        alertmsg = "Can not able to sync " + s.DocumentNo + ". Err message: " + retmsg;
                                        retmsg = "Failed!";
                                        isCreditMemoSync = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                retmsg = "No records";
                                alertmsg = retmsg;
                                isCreditMemoSync = false;
                            }
                            dict.Add(syncflag, retmsg);
                        }

                        if (SyncUnloadStockSwitch.IsToggled == true)
                        {
                            ObservableCollection<UnloadHeader> lsthd = new ObservableCollection<UnloadHeader>();
                            lsthd = await manager.GetSQLite_UnloadHeaderNotSync();
                            syncflag = "UnloadStock";
                            if (lsthd != null && lsthd.Count > 0)
                            {
                                foreach (UnloadHeader objhead in lsthd)
                                {
                                    retmsg = App.svcManager.ExportUnloadHeader(objhead.EntryNo, App.gSalesPersonCode,objhead.UnloadDocNo,objhead.UnloadDate,objhead.CurStatus);
                                    if (retmsg == "Success")
                                    {
                                        manager = new DataManager();
                                        string retval = await manager.SaveSQLite_UnloadHeader(new UnloadHeader
                                        {
                                            ID = objhead.ID,
                                            EntryNo = objhead.EntryNo,
                                            SalesPersonCode = objhead.SalesPersonCode,
                                            UnloadDocNo=objhead.UnloadDocNo,
                                            UnloadDate=objhead.UnloadDate,
                                            CurStatus = objhead.CurStatus,
                                            IsSync = "true",
                                            SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt")
                                        });

                                        ObservableCollection<UnloadLine> lstline = new ObservableCollection<UnloadLine>();
                                        lstline = await manager.GetUnloadLinesbyDocNo(objhead.UnloadDocNo);
                                        if (lstline != null && lstline.Count > 0)
                                        {
                                            foreach (UnloadLine l in lstline)
                                            {
                                                retmsg = App.svcManager.ExportUnloadLine(l.EntryNo, l.HeaderEntryNo, l.UserID, l.ItemNo,l.ItemDesc,l.ItemUom,l.GoodQty,l.BadQty);
                                                if (retmsg == "Success")
                                                {
                                                    manager = new DataManager();
                                                    UnloadLine line = new UnloadLine()
                                                    {
                                                        ID = l.ID,
                                                        EntryNo = l.EntryNo,
                                                        HeaderEntryNo = l.HeaderEntryNo,
                                                        UserID = l.UserID,
                                                        ItemNo = l.ItemNo,
                                                        ItemDesc = l.ItemDesc,
                                                        Quantity =l.Quantity,
                                                        GoodQty = l.GoodQty,
                                                        BadQty = l.BadQty,
                                                        ItemUom = l.ItemUom,
                                                        IsSync = "true",
                                                        SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt")
                                                    };
                                                    retval = await manager.SaveSQLite_UnloadLine(line);
                                                    alertmsg = "Request Order sync success!";
                                                    isUnloadStockSync = true;
                                                }
                                                else
                                                {
                                                    alertmsg = "Can not able to sync doc no " + objhead.UnloadDocNo + " -> Item No" + l.ItemNo + ". Err message: " + retmsg;
                                                    retmsg = "Failed!";
                                                    isUnloadStockSync = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            retmsg = "No line records";
                                            alertmsg = retmsg;
                                            isUnloadStockSync = false;
                                        }
                                        // Sync Num series
                                        SyncNumberSeries();
                                    }
                                    else
                                    {
                                        alertmsg = "Can not able to sync doc No " + objhead.UnloadDocNo + ". Err message: " + retmsg;
                                        retmsg = "Failed!";
                                        isUnloadStockSync = false;
                                        break;
                                    }
                                } //Loop End.
                            }
                            else
                            {
                                retmsg = "No records";
                                alertmsg = retmsg;
                                isUnloadStockSync = false;
                            }
                            dict.Add(syncflag, retmsg);
                        }
                        //  IsLoading = false;

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            //strtime = strtime + " >> Sync End Time : " + DateTime.Now.ToString("hh:mm ss");
                            //timeLabel.Text = strtime;

                            
                            //DependencyService.Get<IMessage>().LongAlert(alertmsg);

                            UserDialogs.Instance.HideLoading(); //IsLoading = false;
                            if (dict.Count > 1)
                            {
                                if (dict.Any(x=> x.Value.Equals("Success")))
                                {
                                    alertmsg = "Sync data success!";
                                    UserDialogs.Instance.ShowSuccess(alertmsg, 3000);
                                }
                                else
                                {
                                    alertmsg = "Sync data failed!";
                                    UserDialogs.Instance.ShowError(alertmsg, 3000);
                                }
                            }
                            else
                            {
                                if (retmsg == "Success")
                                {
                                    //if (dict.Count > 1)
                                    //    alertmsg = "Sync data success!";
                                    UserDialogs.Instance.ShowSuccess(alertmsg, 3000);
                                }
                                else
                                    UserDialogs.Instance.ShowError(alertmsg, 3000);
                            }
                           
                            ShowSyncStatus();
                        });
                    }
                    catch (OperationCanceledException ex)
                    {

                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                        ShowSyncStatus();
                        //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                        UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    }
                    catch (Exception ex)
                    {
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                        ShowSyncStatus();
                        //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                        UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    }
                });

            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
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
            if(seriesList!=null && seriesList.Count>0)
            {
                foreach (NumberSeries s in seriesList)
                {
                    switch(s.Description)
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
               string ret= App.svcManager.ExportNumSeries(App.gDeviceId, App.gSalesPersonCode, SOLastCode, CRLastCode,MPLastCode,RSLastCode,ULLastCode,SOLastSeries,CRLastSeries,MPLastSeries,RSLastSeries,ULLastSeries);
              //  UserDialogs.Instance.ShowError(App.gDeviceId +"- Message :" +ret, 5000);
            }

        }
        private async void  SyncButton1_Clicked(object sender, EventArgs e)
        {
            string syncflag = string.Empty;
            string retmsg = string.Empty;
            string itemMsg, custMsg, methodMsg, priceMsg;
            try
            {
                DependencyService.Get<INetworkConnection>().CheckNetworkConnection();
                if (DependencyService.Get<INetworkConnection>().IsConnected)
                {
                    string result = DependencyService.Get<INetworkConnection>().IsServiceOnline(Helpers.Settings.GeneralSettings);
                    if (result != "true")
                    {
                        UserDialogs.Instance.ShowError("Error : Service is offline. [" + result + "]", 3000);
                        return;
                    }
                }
                else
                {
                    UserDialogs.Instance.ShowError("Error : No internet connection", 3000);
                    return;
                }
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
                if (SyncAllSwitch.IsToggled == true)
                {
                    syncflag = "All";
                    itemMsg = await SyncItem();
                    priceMsg = await SyncSalesPrice();
                    custMsg = await SyncCustomer();
                    methodMsg = await SyncPaymentMethod();
                    if(itemMsg=="Success" && priceMsg == "Success" && customerMsg == "Success" && methodMsg == "Success")
                    {
                        isMasterSync = true;
                        retmsg = "Success";
                        ShowSyncStatus();
                    }
                    else
                    {
                        if (itemMsg != "Success")
                            retmsg = "Item failed! Err message: " + itemMsg;
                        if (priceMsg != "Success")
                            retmsg = "Sales Price failed! Err message: " + priceMsg;
                        if (customerMsg != "Success")
                            retmsg = "Customers failed! Err message: " + customerMsg;
                        if (methodMsg != "Success")
                            retmsg = "Payment Method failed! Err message: " + methodMsg;
                       
                        isMasterSync = false;
                        retmsg = "Failed";
                        ShowSyncStatus();
                    }
                }

                if (SyncItemSwitch.IsToggled == true)
                {
                    syncflag = "Item";
                    retmsg = await SyncItem();
                    ShowSyncStatus();
                }

                if (SyncPriceSwitch.IsToggled == true)
                {
                    syncflag = "SalesPrice";
                    retmsg = await SyncSalesPrice();
                    ShowSyncStatus();
                }

                if (SyncCustomereSwitch.IsToggled == true)
                {
                    syncflag = "Customer";
                    retmsg = await SyncCustomer();
                    ShowSyncStatus();
                }
                if (SyncPaymentMethodsSwitch.IsToggled == true)
                {
                    syncflag = "PaymentMethod";
                    retmsg = await SyncPaymentMethod();
                    ShowSyncStatus();
                }

                if (SyncSalesOrderSwitch.IsToggled == true)
                {
                    syncflag = "SalesOrder";
                    retmsg = await SyncSOCR("SO");
                    ShowSyncStatus();
                }

                //if (SyncPaymentSwitch.IsToggled == true)
                //{
                //    syncflag = "Payment";
                //    retmsg = await SyncPayment();
                //    ShowSyncStatus();
                //}

                if (SyncCreditMemoSwitch.IsToggled == true)
                {
                    syncflag = "CreditMemo";
                    retmsg = await SyncSOCR("CR");
                    ShowSyncStatus();
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }

        #region Sync get data methods
        private async Task<string> SyncItem()
        {
            string alertmsg = string.Empty;
            string retmsg = string.Empty;
            try
            {
                //UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
                //syncflag = "Item";
                DataManager manager = new DataManager();
                database.DatabaseInitialize<Item>();
                retmsg = await manager.SaveSQLite_Items();
                UserDialogs.Instance.HideLoading();
                if (retmsg == "Success")
                {
                    alertmsg = "Items sync success!";
                    UserDialogs.Instance.ShowSuccess(alertmsg, 3000);
                    isItemSync = true;
                }
                else
                {
                    alertmsg = "Sync Item failed! Err message :" + retmsg;
                    UserDialogs.Instance.ShowError(alertmsg, 3000);
                    retmsg = "Failed!";
                    isItemSync = false;
                }
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                retmsg = "Failed!";
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                isItemSync = false;
            }
            
            return retmsg;
        }

        private async Task<string> SyncSalesPrice()
        {
            string alertmsg = string.Empty;
            string retmsg = string.Empty;
            try
            {
                
                //syncflag = "Item";
                DataManager manager = new DataManager();
                database.DatabaseInitialize<SalesPrice>();
                retmsg = await manager.SaveSQLite_SalesPrices(App.gSalesPersonCode);
                UserDialogs.Instance.HideLoading();

                if (retmsg == "Success")
                {
                    alertmsg = "Sales Price sync success!";
                    UserDialogs.Instance.ShowSuccess(alertmsg, 3000);
                    isSalePriceSync = true;
                }
                else
                {
                    alertmsg = "Sync sales price failed! Err message :" + retmsg;
                    UserDialogs.Instance.ShowError(alertmsg, 3000);
                    retmsg = "Failed!";
                    isSalePriceSync = false;
                }
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                retmsg = "Failed!";
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                isSalePriceSync = false;
            }

            return retmsg;
        }

        private async Task<string> SyncCustomer()
        {
            string alertmsg = string.Empty;
            string retmsg = string.Empty;
            string CLEmsg = string.Empty;
            try
            {
               
                //syncflag = "Item";
                DataManager manager = new DataManager();
                database.DatabaseInitialize<Customer>();
                database.DatabaseInitialize<CustLedgerEntry>();
                retmsg = await manager.SaveSQLite_Customers(App.gSalesPersonCode);
                CLEmsg = await manager.SaveSQLite_CustLederEntry(string.Empty);
                UserDialogs.Instance.HideLoading();

                if (retmsg == "Success")
                {
                    if (CLEmsg == "Success")
                    {
                        alertmsg = "Customer and CLE sync success!";
                        UserDialogs.Instance.ShowSuccess(alertmsg, 3000);
                        isCustomerSync = true;
                    }
                    else
                    {
                        alertmsg = "Sync Customer success and CLE is failed! Err message :" + retmsg;
                        retmsg = "Failed!";
                        isCustomerSync = false;
                    }
                }
                else
                {
                    alertmsg = "Sync customer failed! Err message :" + retmsg;
                    UserDialogs.Instance.ShowError(alertmsg, 3000);
                    retmsg = "Failed!";
                    isCustomerSync = false;
                }
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                retmsg = "Failed!";
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                isCustomerSync = false;
            }

            return retmsg;
        }

        private async Task<string> SyncPaymentMethod()
        {
            string alertmsg = string.Empty;
            string retmsg = string.Empty;
            try
            {
                
                //syncflag = "Item";
                DataManager manager = new DataManager();
                database.DatabaseInitialize<PaymentMethod>();
               retmsg = await manager.SaveSQLite_PaymentMethods();
                UserDialogs.Instance.HideLoading();

                if (retmsg == "Success")
                {
                    alertmsg = "Payment Method sync success!";
                    UserDialogs.Instance.ShowSuccess(alertmsg, 3000);
                    isPaymentMethodSync = true;
                }
                else
                {
                    alertmsg = "Sync payment methods failed! Err message :" + retmsg;
                    UserDialogs.Instance.ShowError(alertmsg, 3000);
                    retmsg = "Failed!";
                    isPaymentMethodSync = false;
                }
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                retmsg = "Failed!";
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                isPaymentMethodSync = false;
            }

            return retmsg;
        }
        #endregion

        #region Sync set Data methods
        private async Task<string> SyncSOCR(string syncType)
        {
            string syncTitle = string.Empty;
            string alertmsg = string.Empty;
            string retmsg = string.Empty;
            try
            {
                if (syncType == "SO")
                    syncTitle = "Sales Order";
                else
                    syncTitle = "Credit Memo";
               
                DataManager manager = new DataManager();
                ObservableCollection<SalesHeader> lsthd = new ObservableCollection<SalesHeader>();
                    lsthd = await manager.GetSQLite_SalesHeaderbyStatus("Released",syncType);
              //  lsthd = await manager.GetSQLite_SalesHeaderbyStatus("Released", "CN");
                if (lsthd != null && lsthd.Count > 0)
                {
                    foreach (SalesHeader s in lsthd)
                    {
                        retmsg = App.svcManager.ExportSalesHeader(s.DocumentNo, s.SellToCustomer, s.SellToName, s.BillToCustomer, s.BillToName, s.DocumentDate, s.Status, s.PaymentMethod, s.TotalAmount, s.DocumentType, s.Note, s.StrSignature,App.gSalesPersonCode,App.gDeviceId,s.Comment,s.IsVoid,s.ExternalDocNo);
                        if (retmsg == "Success")
                        {
                            ObservableCollection<SalesLine> lstline = new ObservableCollection<SalesLine>();
                            lstline = manager.GetSalesLinesbyDocNo(s.DocumentNo);
                            if (lstline != null && lstline.Count > 0)
                            {
                                foreach (SalesLine l in lstline)
                                {
                                    retmsg = App.svcManager.ExportSalesLine(l.ID.ToString(),l.DocumentNo, l.ItemNo, l.LocationCode, l.Quantity, l.FOCQty,l.BadQuantity, l.UnitofMeasurementCode, l.UnitPrice, l.LineDiscountPercent, l.LineDiscountAmount, l.LineAmount,l.GoodReasonCode,l.BadReasonCode,l.ItemType);
                                    if (retmsg == "Success")
                                    {
                                        alertmsg = syncTitle+" sync success!";
                                        retmsg = "success";
                                        isSalesOrderSync = true;
                                    }
                                    else
                                    {
                                        alertmsg = "Can not able to sync doc no " + l.DocumentNo + " -> Item No" + l.ItemNo + ". Err message: " + retmsg;
                                        retmsg = "failed!";
                                        isSalesOrderSync = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                retmsg = "No line records";
                                isSalesOrderSync = false;
                            }

                        }
                        else
                        {
                            alertmsg = "Can not able to sync doc No " + s.DocumentNo + ". Err message: " + retmsg;
                            retmsg = "failed!";
                            isSalesOrderSync = false;
                            break;
                        }
                    }
                }
                    
                UserDialogs.Instance.HideLoading();
                if (retmsg == "Success")
                {
                    
                    UserDialogs.Instance.ShowSuccess(alertmsg, 3000);
                    isSalesOrderSync = true;
                }
                else
                {
                    alertmsg = "Sync "+ syncTitle +" failed! Err message :" + retmsg;
                    UserDialogs.Instance.ShowError(alertmsg, 3000);
                    retmsg = "Failed!";
                    isSalesOrderSync = false;
                }
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                retmsg = "Failed!";
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                isSalesOrderSync = false;
            }

            return retmsg;
        }

        #endregion

        private void ShowSyncStatus()
        {
            string showtext = string.Empty;
          foreach(KeyValuePair<string,string> pair in dict)
          {
            showtext = pair.Value.ToString();

            switch (pair.Key.ToString())
            {
                case "Item":
                    if (isItemSync)
                    {
                        syncItemLabel.Text = showtext;
                        syncItemLabel.TextColor = Color.Green;
                    }
                    else
                    {
                        syncItemLabel.Text = showtext;
                        syncItemLabel.TextColor = Color.Red;
                    }
                    syncItemLabel.IsVisible = true;
                    SyncItemSwitch.IsToggled = false;
                    break;
                case "SalesPrice":

                    if (isSalePriceSync)
                    {
                        syncPriceLabel.Text = showtext;
                        syncPriceLabel.TextColor = Color.Green;
                    }
                    else
                    {
                        syncPriceLabel.Text = showtext;
                        syncPriceLabel.TextColor = Color.Red;
                    }
                    syncPriceLabel.IsVisible = true;
                    SyncPriceSwitch.IsToggled = false;
                    break;
                case "PaymentMethod":
                    if (isPaymentMethodSync)
                    {
                        syncPaymentMethodsLabel.Text = showtext;
                        syncPaymentMethodsLabel.TextColor = Color.Green;
                    }
                    else
                    {
                        syncPaymentMethodsLabel.Text = showtext;
                        syncPaymentMethodsLabel.TextColor = Color.Red;
                    }
                    syncPaymentMethodsLabel.IsVisible = true;
                    SyncPaymentMethodsSwitch.IsToggled = false;
                    break;
                case "Customer":
                    if (isCustomerSync)
                    {
                        syncCustomerLabel.Text = showtext;
                        syncCustomerLabel.TextColor = Color.Green;
                    }
                    else
                    {
                        syncCustomerLabel.Text = showtext;
                        syncCustomerLabel.TextColor = Color.Red;
                    }
                    syncCustomerLabel.IsVisible = true;
                    SyncCustomereSwitch.IsToggled = false;
                    break;
                    //case "Vendor":
                    //    if (isVendorSync)
                    //    {
                    //        syncVendorLabel.Text = showtext;
                    //        syncVendorLabel.TextColor = Color.Green;
                    //    }
                    //    else
                    //    {
                    //        syncVendorLabel.Text = showtext;
                    //        syncVendorLabel.TextColor = Color.Red;
                    //    }
                    //    syncVendorLabel.IsVisible = true;
                    //    SyncVendorSwitch.IsToggled = false;
                    //    break;
                    //case "CustomerPriceHistory":
                    //    if (isCustPriceHistSync)
                    //    {
                    //        syncCustPriceHisLabel.Text = showtext;
                    //        syncCustPriceHisLabel.TextColor = Color.Green;
                    //    }
                    //    else
                    //    {
                    //        syncCustPriceHisLabel.Text = showtext;
                    //        syncCustPriceHisLabel.TextColor = Color.Red;
                    //    }
                    //    syncCustPriceHisLabel.IsVisible = true;
                    //    SyncCustPriceHisSwitch.IsToggled = false;
                    //    break;
                    case "RequestOrder":
                        if (isRequestOrderSync)
                        {
                            syncRequestOrderLabel.Text = showtext;
                            syncRequestOrderLabel.TextColor = Color.Green;
                        }
                        else
                        {
                            syncRequestOrderLabel.Text = showtext;
                            syncRequestOrderLabel.TextColor = Color.Red;
                        }
                        syncRequestOrderLabel.IsVisible = true;
                        SyncRequestOrderSwitch.IsToggled = false;
                        break;
                    case "SalesOrder":
                    if (isSalesOrderSync)
                    {
                        syncSalesOrderLabel.Text = showtext;
                        syncSalesOrderLabel.TextColor = Color.Green;
                    }
                    else
                    {
                        syncSalesOrderLabel.Text = showtext;
                        syncSalesOrderLabel.TextColor = Color.Red;
                    }
                    syncSalesOrderLabel.IsVisible = true;
                    SyncSalesOrderSwitch.IsToggled = false;
                    break;
                //case "Payment":
                //    if (isPaymentSync)
                //    {
                //        syncPaymentLabel.Text = showtext;
                //        syncPaymentLabel.TextColor = Color.Green;
                //    }
                //    else
                //    {
                //        syncPaymentLabel.Text = showtext;
                //        syncPaymentLabel.TextColor = Color.Red;
                //    }
                //    syncPaymentLabel.IsVisible = true;
                //    SyncPaymentSwitch.IsToggled = false;
                //    break;
                case "CreditMemo":
                    if (isCreditMemoSync)
                    {
                        syncCreditMemotLabel.Text = showtext;
                        syncCreditMemotLabel.TextColor = Color.Green;
                    }
                    else
                    {
                        syncCreditMemotLabel.Text = showtext;
                        syncCreditMemotLabel.TextColor = Color.Red;
                    }
                    syncCreditMemotLabel.IsVisible = true;
                    SyncCreditMemoSwitch.IsToggled = false;
                    break;
                    case "UnloadStock":
                        if (isUnloadStockSync)
                        {
                            syncUnloadStockLabel.Text = showtext;
                            syncUnloadStockLabel.TextColor = Color.Green;
                        }
                        else
                        {
                            syncUnloadStockLabel.Text = showtext;
                            syncUnloadStockLabel.TextColor = Color.Red;
                        }
                        syncUnloadStockLabel.IsVisible = true;
                        SyncUnloadStockSwitch.IsToggled = false;
                        break;
                    case "All":
                    if (isMasterSync)
                    {
                        syncAllLabel.Text = showtext;
                        syncAllLabel.TextColor = Color.Green;
                    }
                    else
                    {
                        if (itemMsg != "Success")
                            syncItemLabel.TextColor = Color.Red;
                        syncItemLabel.IsVisible = true;
                        syncItemLabel.Text = itemMsg;
                        if (priceMsg != "Success")
                            syncPriceLabel.TextColor = Color.Red;
                        syncPriceLabel.IsVisible = true;
                        syncPriceLabel.Text = priceMsg;
                        if (customerMsg != "Success")
                            syncCustomerLabel.TextColor = Color.Red;
                        syncCustomerLabel.IsVisible = true;
                        syncCustomerLabel.Text = customerMsg;
                        if (methodMsg != "Success")
                            syncPaymentMethodsLabel.TextColor = Color.Red;
                        syncPaymentMethodsLabel.IsVisible = true;
                        syncPaymentMethodsLabel.Text = methodMsg;

                        //syncAllLabel.Text = showtext;
                        //syncAllLabel.TextColor = Color.Red;
                    }
                    syncAllLabel.IsVisible = true;
                    SyncAllSwitch.IsToggled = false;
                    break;
                }
            }            
        }

        //private DataTable GetSaleHeaderTable(ObservableCollection<SalesHeader> hd)
        //{
        //    // Here we create a DataTable with four columns.
        //    DataTable table = new DataTable();

        //    table.Columns.Add("DocumentNo", typeof(string));
        //    table.Columns.Add("SellToCustomer", typeof(string));
        //    table.Columns.Add("SellToName", typeof(string));
        //    table.Columns.Add("BillToCustomer", typeof(string));
        //    table.Columns.Add("BillToName", typeof(string));
        //    table.Columns.Add("DocumentDate", typeof(string));
        //    table.Columns.Add("Status", typeof(string));
        //    table.Columns.Add("PaymentMethod", typeof(string));
        //    table.Columns.Add("TotalAmount", typeof(string));
        //    //table.Columns.Add("Date", typeof(DateTime));

        //    // Here we add five DataRows.
        //    if (hd != null && hd.Count > 0)
        //    {
        //        foreach(SalesHeader s in hd)
        //        {
        //            table.Rows.Add(s.DocumentNo, s.SellToCustomer, s.SellToName, s.BillToCustomer,s.BillToName,s.DocumentDate,s.Status, s.PaymentMethod, s.TotalAmount);
        //        }
        //    }
        //    return table;
        //}

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }
        private void ShowSyncedLabel(Label label, string msg)
        {
            label.IsVisible = true;
            if (msg == "Success")
            { 
                label.TextColor = Color.Green;
            }
            else
            {
               
                syncPaymentMethodsLabel.TextColor = Color.Red;
            }
            label.Text = msg;
        }
    }
}