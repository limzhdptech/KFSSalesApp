using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignaturePad.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using Acr.UserDialogs;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmOrderPage : ContentPage
    {
        SalesHeader head { get; set; }
        Customer customer { get; set; }
        string DocumentNo { get; set; }
        string DocumentDate { get; set; }
        string pagefrom { get; set; }
        string _signature64str { get; set; }
        string isvoid { get; set; }
        int EntryNo { get; set; }
        ObservableCollection<SalesLine> recs { get; set; }
        List<PaymentMethod> paymethods { get; set; }
        decimal GSTPercent { get; set; }
        decimal TotalAmount { get; set; }
        decimal GSTAmount { get; set; }
        decimal NetAmount { get; set; }
        private bool _isEnableVoidBtn { get; set; }
        private bool _isEnableCopyBtn { get; set; }
        private bool _isEnableConfirmBtn { get; set; }
        private bool _isEnablePrintBtn { get; set; }
        string ExchangeDocNo { get; set; }
        int _limit = 50;     //Enter text limit

        public ConfirmOrderPage(string docno,ObservableCollection<SalesLine> records,string from)
        {
            InitializeComponent();
            DocumentNo = docno;
            isvoid = "false";
            this.Title = App.gPageTitle;
            recs = new ObservableCollection<SalesLine>();
            pagefrom = from;
            recs = records;

            CommentEntry.TextChanged += (sender, args) =>
            {
                string _text = CommentEntry.Text;      //Get Current Text
                if (_text.Length > _limit)       //If it is more than your character restriction
                {
                    _text = _text.Remove(_text.Length - 1);  // Remove Last character
                    CommentEntry.Text = _text;        //Set the Old value
                }
            };
        }

        public decimal CalculateGSTAmount()
        {
            //decimal SubTotal = decimal.Parse(lines.Sum(x => x.LineAmount).ToString());
            //decimal GSTAmount = (SubTotal * 7) / 100;
            decimal gstamt = 0;
            if(recs!=null)
            {
                if(recs.Count>0)
                {
                    foreach (SalesLine s in recs)
                    {
                        gstamt += (s.LineAmount * decimal.Parse(App.gPercentGST)) / 100;
                    }
                }
            }

            return gstamt;
        }

        void ShowData()
        {
            _isEnableVoidBtn = true;
            _isEnableCopyBtn = true;
            _isEnableConfirmBtn = true;
            _isEnablePrintBtn = true;
            DocNoLabel.Text = DocumentNo;
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    //LoadPaymentMethods();

                    TotalAmount = recs.Sum(x => x.LineAmount);
                    GSTAmount = CalculateGSTAmount();
                    NetAmount = TotalAmount + GSTAmount;
                    

                    // Get header info 
                    head = new SalesHeader();
                    DataManager manager = new DataManager();
                    head = manager.GetSalesHeaderbyDocNo(DocumentNo);

                    customer = new Customer();
                    customer = await manager.GetCustomerbyCode(head.SellToCustomer);

                    Customer billcustomer = new Customer();
                    billcustomer = await manager.GetCustomerbyCode(head.BillToCustomer);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        SubAmountNameLabel.Text = "SUB TOTAL :";
                        GSTAmountNameLabel.Text = "GST (" + App.gPercentGST + " % ) : ";
                        NetAmountNameLabel.Text = "TOTAL :";
                        TotalAmountLabel.Text = string.Format("{0:0.00}", TotalAmount);
                        GSTAmountLabel.Text = string.Format("{0:0.00}", GSTAmount);
                        NetAmountLabel.Text = string.Format("{0:0.00}", NetAmount);

                        if (customer != null)
                        {
                            SellToNameLabel.Text = head.SellToName;
                            SellToLine1Label.Text = customer.Address + "," + customer.Address2 + "," + customer.City + "  " + customer.Postcode;
                            SellToLine2Label.Text = customer.PhoneNo + "," + customer.MobileNo;
                            //Line4Label.Text = string.Empty;
                        }

                        if (billcustomer != null)
                        {
                            BillToNameLabel.Text = head.BillToName;
                            Line1Label.Text = billcustomer.Address + "," + billcustomer.Address2 + "," + billcustomer.City + "  " + billcustomer.Postcode;
                            Line2Label.Text = billcustomer.PhoneNo + "," + billcustomer.MobileNo;
                            //Line4Label.Text = string.Empty;
                        }
                        if (pagefrom == "Released")
                        {
                            ConfirmButton.IsVisible = false;
                            if (App.gDocType == "SO")
                            {
                                if (head.IsVoid == "true" || head.IsSync == "true")
                                {
                                    //VoidButton.IsVisible = false;
                                    VoidButton.Text = "Copy";
                                    VoidButton.IsVisible = true;
                                    VoidButton.Clicked += CopyButton_OnClicked;

                                }
                                else
                                {
                                    VoidButton.Text = "Void";
                                    VoidButton.IsVisible = true;
                                    VoidButton.Clicked += VoidButton_OnClicked;
                                }

                            }
                            else
                                VoidButton.IsVisible = false;

                            CustomerSignaturePad.IsVisible = false;
                            CommentEntry.IsEnabled = false;
                            imgSignature.IsVisible = true;
                            //PaymentMethodsPicker.IsEnabled = false;
                            //PaymentMethodsPicker.SelectedItem = head.PaymentMethod;
                            CommentEntry.Text = head.Comment;
                            imgSignature.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(head.StrSignature)));
                            _signature64str = head.StrSignature;
                            // imgSignature.Source = head.StrSignature;
                            //CustomerSignaturePad.
                        }
                        else
                        {
                            VoidButton.IsVisible = false;
                            CustomerSignaturePad.IsVisible = true;
                            imgSignature.IsVisible = false;
                            _signature64str = string.Empty;
                        }
                    });

                }
                catch (OperationCanceledException ex)
                {
                    UserDialogs.Instance.HideLoading();

                    UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert", "OK");
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading();

                    UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert", "OK");
                }
            });

            
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            ShowData();
            //_isEnableVoidBtn = true;
            //_isEnableCopyBtn = true;
            //_isEnableConfirmBtn = true;
            //_isEnablePrintBtn = true;

            //DocNoLabel.Text = DocumentNo;
            ////LoadPaymentMethods();

            //TotalAmount = recs.Sum(x => x.LineAmount);
            //GSTAmount = CalculateGSTAmount();
            //NetAmount = TotalAmount + GSTAmount;
            //SubAmountNameLabel.Text = "SUB TOTAL :";
            //GSTAmountNameLabel.Text = "GST (" + App.gPercentGST + " % ) : ";
            //NetAmountNameLabel.Text = "TOTAL :";
            //TotalAmountLabel.Text = string.Format("{0:0.00}", TotalAmount);
            //GSTAmountLabel.Text= string.Format("{0:0.00}", GSTAmount);
            //NetAmountLabel.Text= string.Format("{0:0.00}", NetAmount);

            //// Get header info 
            //head = new SalesHeader();
            //DataManager manager = new DataManager();
            //head =manager.GetSalesHeaderbyDocNo(DocumentNo);

            //customer = new Customer();
            //customer = await manager.GetCustomerbyCode(head.SellToCustomer);
            //if (customer != null)
            //{
            //    SellToNameLabel.Text = head.SellToName;
            //    SellToLine1Label.Text = customer.Address + "," + customer.Address2 + "," + customer.City + "  " + customer.Postcode;
            //    SellToLine2Label.Text = customer.PhoneNo + "," + customer.MobileNo;
            //    //Line4Label.Text = string.Empty;
            //}

            //customer = new Customer();
            //customer = await manager.GetCustomerbyCode(head.BillToCustomer);

            //if(customer!=null)
            //{
            //    BillToNameLabel.Text = head.BillToName;
            //    Line1Label.Text = customer.Address + "," + customer.Address2 + "," + customer.City + "  " + customer.Postcode;
            //    Line2Label.Text = customer.PhoneNo + "," + customer.MobileNo;
            //    //Line4Label.Text = string.Empty;
            //}

            //if (pagefrom == "Released")
            //{
            //    ConfirmButton.IsVisible = false;
            //    if (App.gDocType == "SO")
            //    {
            //        if (head.IsVoid == "true" || head.IsSync == "true")
            //        {
            //            //VoidButton.IsVisible = false;
            //            VoidButton.Text = "Copy";
            //            VoidButton.IsVisible = true;
            //            VoidButton.Clicked += CopyButton_OnClicked;
                        
            //        }    
            //        else
            //        {
            //            VoidButton.Text = "Void";
            //            VoidButton.IsVisible = true;
            //            VoidButton.Clicked += VoidButton_OnClicked;
            //        }
                        
            //    }
            //    else
            //        VoidButton.IsVisible = false;

            //    CustomerSignaturePad.IsVisible = false;
            //    CommentEntry.IsEnabled = false;
            //    imgSignature.IsVisible = true;
            //    //PaymentMethodsPicker.IsEnabled = false;
            //    //PaymentMethodsPicker.SelectedItem = head.PaymentMethod;
            //    CommentEntry.Text = head.Comment;
            //    imgSignature.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(head.StrSignature)));
            //    _signature64str = head.StrSignature;
            //   // imgSignature.Source = head.StrSignature;
            //    //CustomerSignaturePad.
            //}
            //else
            //{
            //    VoidButton.IsVisible = false;
            //    CustomerSignaturePad.IsVisible = true;
            //    imgSignature.IsVisible = false;
            //    _signature64str = string.Empty;
            //}
        }

        private void Clear_Button_OnClicked(object sender, EventArgs e)
        {
            CustomerSignaturePad.Clear();
        }

        private async void ConfirmButton_OnClicked(object sender, EventArgs e)
        {
            if(_isEnableConfirmBtn)
            {
                _isEnableConfirmBtn = false;
                string msg = string.Empty;
                string retval = string.Empty;
                isvoid = "false";
                try
                {
                    if (App.gDocType == "SO")
                        msg = "Are you sure to release Sales Order?";
                    else
                        msg = "Are you sure to release Credit Memo?";
                    var answer = await DisplayAlert("Confirm", msg, "Yes", "No");
                    if (answer)
                    {
                        Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                        Task.Run(async () =>
                        {

                            retval = await ConfirmedOrder();
                        }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                        {
                            UserDialogs.Instance.HideLoading();
                            if (!string.IsNullOrEmpty(retval))
                            {
                                if (retval == "Success")
                                {
                                    //DependencyService.Get<IMessage>().LongAlert(retval);
                                    UserDialogs.Instance.ShowSuccess(retval, 3000);
                                    //var navipages = Navigation.NavigationStack.ToList();
                                    //foreach(var pg in navipages)
                                    //{
                                    //    Navigation.RemovePage(pg);
                                    //}

                                    App.gSOStatus = "Released";
                                    Navigation.PushAsync(new MainPage(2));
                                }
                                else
                                {
                                    // DependencyService.Get<IMessage>().LongAlert(retval);
                                    UserDialogs.Instance.ShowError(retval, 3000);
                                    _isEnableConfirmBtn = true;
                                }
                            }

                        }));
                    }
                }
                catch (OperationCanceledException ex)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    _isEnableConfirmBtn = true;
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    _isEnableConfirmBtn = true;
                }
            }
           
        }
        private async void PrintButton_Clicked(object sender, EventArgs e)
        {
            if(_isEnablePrintBtn)
            {
                _isEnablePrintBtn = false;
                string msg = string.Empty;
                string msgTitle = string.Empty;
                string retval = string.Empty;
                isvoid = "false";
                try
                {
                    if (App.gDocType == "SO")
                    {
                        msgTitle = "Print Sales Order";
                        msg = "Are you sure to print Sales Order?";
                    }

                    else
                    {
                        msgTitle = "Print Credit Memo";
                        msg = "Are you sure to print Credit Memo?";
                    }

                    var answer = await DisplayAlert(msgTitle, msg, "Yes", "No");
                    if (answer)
                    {
                        DataManager manager = new DataManager();
                        DeviceInfo info = new DeviceInfo();
                        info = await manager.GetDeviceInfo();
                        if (info != null)
                        {
                            if (!string.IsNullOrEmpty(info.DeviceName))
                            {
                                if (pagefrom != "Released")
                                {
                                    retval = await ConfirmedOrder();
                                }
                                else
                                    retval = "Success";

                                if (retval == "Success")
                                {
                                    UserDialogs.Instance.ShowSuccess(retval, 3000);
                                    PrintText(info.DeviceName);
                                    App.gSOStatus = "Released";
                                    Navigation.PushAsync(new MainPage(2));
                                }
                                else
                                {
                                    // DependencyService.Get<IMessage>().LongAlert(retval);
                                    UserDialogs.Instance.ShowError(retval, 3000);
                                    _isEnablePrintBtn = true;
                                }

                            }

                            else
                            {
                                UserDialogs.Instance.ShowError("Bluetooth printer name not found!", 3000);
                                _isEnablePrintBtn = true;
                            }
                                
                        }
                        else
                        {
                            UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);
                            _isEnablePrintBtn = true;
                        }
                    }
                    else
                    {
                        _isEnablePrintBtn = true;
                    }
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    _isEnablePrintBtn = true;
                }
            }
            
        }
        private async void VoidButton_OnClicked(object sender, EventArgs e)
        {
            if(_isEnableVoidBtn)
            {
                _isEnableVoidBtn = false;
                string msg = string.Empty;
                string retval = string.Empty;
                isvoid = "true";
                DataManager manager = new DataManager();
                ObservableCollection<SalesLine> recItems = new ObservableCollection<SalesLine>();
                try
                {
                    //if (App.gDocType == "SO")
                    //    msg = "Are you sure to void Sales Order?";
                    //else
                    //    msg = "Are you sure to void Credit Memo?";

                    var answer = await DisplayAlert("Confirm", "Are you sure to void Sales Order?", "Yes", "No");
                    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                    Task.Run(async () =>
                    {
                        if (answer)
                        {
                            retval = await ConfirmedOrder();

                            recItems = manager.GetSalesLinesbyDocNo(DocumentNo);
                            if (recItems != null)
                            {
                                if (recItems.Count > 0)
                                {
                                    foreach (SalesLine s in recItems)
                                    {
                                        Item iobj = new Item();
                                        iobj = manager.GetSQLite_ItembyItemNo(s.ItemNo);
                                        //decimal soldQty = 0;
                                        //if (s.ItemType!="EXC")
                                        //    soldQty = iobj.SoldQty - s.Quantity;
                                        //else
                                        //    soldQty= iobj.SoldQty - s.BadQuantity;
                                        //manager.UpdateSQLite_SOInventory(s.ItemNo, soldQty);

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
                                }
                            }
                        }

                    }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        if (!string.IsNullOrEmpty(retval))
                        {
                            if (retval == "Success")
                            {
                                //DependencyService.Get<IMessage>().LongAlert(retval);
                                UserDialogs.Instance.ShowSuccess(retval, 3000);
                                //var navipages = Navigation.NavigationStack.ToList();
                                //foreach(var pg in navipages)
                                //{
                                //    Navigation.RemovePage(pg);
                                //}

                                App.gSOStatus = "Released";
                                Navigation.PushAsync(new MainPage(2));
                            }
                            else
                            {
                                // DependencyService.Get<IMessage>().LongAlert(retval);
                                UserDialogs.Instance.ShowError(retval, 3000);
                                _isEnableVoidBtn = true;
                            }
                        }

                    }));
                }
                catch (OperationCanceledException ex)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    _isEnableVoidBtn = true;
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    _isEnableVoidBtn = true;
                }
            }
        }

        private bool CheckItemBalance(ObservableCollection<SalesLine> lines)
        {
            bool retval = true;
            DataManager manager = new DataManager();
            if (lines != null)
            {
                if (lines.Count > 0)
                {
                    foreach (SalesLine s in lines)
                    {
                        Item item = new Item();
                        item = manager.GetSQLite_ItembyItemNo(s.ItemNo);
                        if (item != null)
                        {
                           decimal MaxSoldQty = item.LoadQty + item.ReturnQty - item.SoldQty;
                          if (MaxSoldQty<s.Quantity)
                            {
                                retval = false;
                                break;
                            }
                        }
                    }
                }
            }
            return retval;
        }
        private async void CopyButton_OnClicked(object sender, EventArgs e)
        {
            if(_isEnableCopyBtn)
            {
                _isEnableCopyBtn = false;
                string msg = string.Empty;
                string retval = string.Empty;
                isvoid = "true";
                DataManager manager = new DataManager();
                ObservableCollection<SalesLine> recItems = new ObservableCollection<SalesLine>();
                try
                {
                    //if (App.gDocType == "SO")
                    //    msg = "Are you sure to void Sales Order?";
                    //else
                    //    msg = "Are you sure to void Credit Memo?";

                    var answer = await DisplayAlert("Confirm", "Are you sure to copy void Order?", "Yes", "No");
                    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                    Task.Run(async () =>
                    {
                        if (answer)
                        {
                            recItems = manager.GetSalesLinesbyDocNo(DocumentNo);

                            if (CheckItemBalance(recItems) == true)
                            {
                                manager = new DataManager();
                                string newDocNo = manager.GetLastNoSeries(App.gSOPrefix);
                                retval = await manager.SaveSQLite_SalesHeader(new SalesHeader
                                {
                                    ID = 0,
                                    DocumentNo = newDocNo,
                                    DocumentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"),//DocumentDatePicker.Date.ToString("yyyy-MM-dd"),
                                    SellToCustomer = head.SellToCustomer,
                                    SellToName = head.SellToName,
                                    BillToCustomer = head.BillToCustomer,
                                    BillToName = head.BillToName,
                                    Status = "Open",
                                    TotalAmount = head.TotalAmount,
                                    GSTAmount = head.GSTAmount,
                                    NetAmount = head.NetAmount,
                                    DocumentType = head.DocumentType,
                                    Note = head.Note,
                                    IsVoid = "false",
                                    IsSync = "false",
                                    SyncDateTime = string.Empty,
                                    ExternalDocNo = head.ExternalDocNo
                                });
                                if (retval == "Success")
                                {
                                    if (recItems != null)
                                    {
                                        if (recItems.Count > 0)
                                        {
                                            foreach (SalesLine s in recItems)
                                            {
                                                SalesLine line = new SalesLine()
                                                {
                                                    ID = 0,
                                                    DocumentNo = newDocNo,
                                                    ItemNo = s.ItemNo,
                                                    Description = s.Description,
                                                    UnitofMeasurementCode = s.UnitofMeasurementCode,
                                                    Quantity = s.Quantity,
                                                    BadQuantity = s.BadQuantity,
                                                    FOCQty = 0,
                                                    UnitPrice = s.UnitPrice,
                                                    GoodReasonCode = s.GoodReasonCode,
                                                    BadReasonCode = s.BadReasonCode,
                                                    ItemType = s.ItemType,
                                                    BagNo = s.BagNo
                                                };
                                                string retline = manager.SaveSQLite_SalesLine(line);
                                                if (retline == "Success")
                                                {
                                                    Item iobj = new Item();
                                                    iobj = manager.GetSQLite_ItembyItemNo(s.ItemNo);
                                                    //decimal soldQty = 0;
                                                    //if (s.ItemType != "EXC")
                                                    //    soldQty = iobj.SoldQty + s.Quantity;
                                                    //else
                                                    //    soldQty = iobj.SoldQty + s.BadQuantity;
                                                    //manager.UpdateSQLite_SOInventory(s.ItemNo, soldQty);

                                                    if (App.gDocType == "SO")
                                                    {
                                                        decimal soldQty = 0;
                                                        if (s.ItemType == "EXC")
                                                        {
                                                            soldQty = iobj.SoldQty + s.BadQuantity;
                                                            decimal excBadQty = iobj.BadQty + s.BadQuantity;
                                                            manager.UpdateSQLite_ExchangeInventory(s.ItemNo, excBadQty);
                                                        }
                                                        else
                                                            soldQty = iobj.SoldQty + s.Quantity;

                                                        manager.UpdateSQLite_SOInventory(s.ItemNo, soldQty);
                                                    }
                                                    else
                                                    {
                                                        decimal returnQty = iobj.ReturnQty - s.Quantity;
                                                        decimal badQty = iobj.BadQty - s.BadQuantity;
                                                        manager.UpdateSQLite_ReturnInventory(s.ItemNo, returnQty, badQty);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    manager.IncreaseNoSeries(App.gSOPrefix, newDocNo, "SO");
                                }
                            }
                            else
                            {
                                retval = "Not enough quantity!";
                            }
                        }

                    }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        if (!string.IsNullOrEmpty(retval))
                        {
                            if (retval == "Success")
                            {

                                UserDialogs.Instance.ShowSuccess(retval, 3000);
                                App.gSOStatus = "Open";
                                Navigation.PushAsync(new MainPage(1));
                            }
                            else
                            {
                                UserDialogs.Instance.ShowError(retval, 3000);
                                _isEnableCopyBtn = true;
                            }
                        }

                    }));
                }
                catch (OperationCanceledException ex)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    _isEnableCopyBtn = true;
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    _isEnableCopyBtn = true;
                }
            }
            
        }
        private async Task<string> ConfirmedOrder()
        {
            try
            {
                string retval = string.Empty;
                ExchangeDocNo = string.Empty;
                if(string.IsNullOrEmpty(_signature64str))
                {
                    try
                    {
                        var signedImageStream = await CustomerSignaturePad.GetImageStreamAsync(SignatureImageFormat.Png);
                        if (signedImageStream is MemoryStream)
                        {
                            var signatureMemoryStream = signedImageStream as MemoryStream;
                            byte[] bytes = signatureMemoryStream.ToArray();
                            _signature64str = Convert.ToBase64String(bytes);
                        }
                    }
                    catch (ArgumentException)
                    {
                        _signature64str = string.Empty;
                    }
                }
               
                DataManager manager = new DataManager();
                retval = await manager.SaveSQLite_SalesHeader(new SalesHeader
                {
                    ID = head.ID,
                    DocumentNo = head.DocumentNo,
                    DocumentDate = head.DocumentDate,
                    BillToCustomer = head.BillToCustomer,
                    BillToName = head.BillToName,
                    SellToCustomer = head.SellToCustomer,
                    SellToName = head.SellToName,
                    TotalAmount = head.TotalAmount,
                    GSTAmount=GSTAmount,
                    NetAmount=NetAmount,
                    Status = "Released",
                    PaymentMethod = string.Empty,
                    StrSignature = _signature64str,
                    DocumentType = head.DocumentType,
                    Note = head.Note,
                    Comment= CommentEntry.Text,
                    IsVoid=isvoid,
                    IsSync="false",
                    SyncDateTime=string.Empty,
                    ExternalDocNo=head.ExternalDocNo
                });

                if (retval == "Success")
                {
                    if (App.gDocType == "SO")
                    {
                        if (!string.IsNullOrEmpty(ExchangeDocNo))
                        {
                            ObservableCollection<SalesLine> lines = new ObservableCollection<SalesLine>();
                            lines = manager.GetSalesLinesbyDocNo(ExchangeDocNo);
                            decimal new_amt = lines.Sum(x => x.LineAmount);
                            retval = manager.UpdateSalesHeaderTotalAmount(new_amt, ExchangeDocNo, lines);
                        }
                     }
                    else
                    {
                       
                        ObservableCollection<SalesLine> lines = new ObservableCollection<SalesLine>();
                        lines = manager.GetSalesLinesbyDocNo(DocumentNo);
                        foreach (SalesLine ln in lines)
                        {
                            VanItem ckitm = new VanItem();
                            ckitm = await manager.GetSQLite_VanItembyItemNo(ln.ItemNo);
                            if (ckitm != null)
                            {
                                decimal vanqty = ckitm.ReturnQty + ln.Quantity;
                                await manager.UpdateSQLite_VanItem(ln.ItemNo, vanqty);
                            }
                            else
                            {
                                Item itm = new Item();
                                itm = manager.GetSQLite_ItembyItemNo(ln.ItemNo);
                                VanItem vitm = new VanItem()
                                {
                                    ID = 0,
                                    ItemNo = ln.ItemNo,
                                    Description = ln.Description,
                                    BarCode = itm.BarCode,
                                    BaseUOM = ln.UnitofMeasurementCode,
                                    UnitPrice = itm.UnitPrice,
                                    Str64Img = itm.Str64Img,
                                    LoadQty = itm.LoadQty,
                                    SoldQty = itm.SoldQty,
                                    ReturnQty = ln.Quantity,
                                    BadQty = itm.BadQty,
                                    UnloadQty = itm.UnloadQty,
                                    Balance = itm.Balance
                                };
                                await manager.SaveSQLite_VanItem(vitm);
                            }
                        }
                    }

                }

                return retval;
            }
            catch (Exception ex)
            {
               // UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
               return ex.Message.ToString();
            }
        }

        private void PrintText(string SelectedBthDevice)
        {

            //{PRINT, Global Options:@row,column:Name,Field Options|data|}
            //PRINT,->	Use a comma after the PRINT command only if there are global options.
            //Global Options: ->	Global Options include BACK, DEMAND, QSTOP, QUANTITY, ROTATION, and STOP. If you use more than one global option, separate each option with a comma.
            //Name ->	Specifies the name of the bar code, font, graphic, or line to print. The Name must be five characters.
            //Field Options	-> Field options increase the size of fonts, graphics, or lines. Each field option is separated from the next by a comma.
            //var s = "{PRINT:@10,30:PE203,HMULT2,VMULT2 | Total:$13.15 |@60,30:PE203,HMULT2,VMULT2 | 01 - 01 - 05 |}";

            //var s = "\x1B\x21\x30HelloWord\n test1\n test2\n";
            //string esc = "\x1B";


            // string ESC;// = "\x1B\x45"; //ESC byte in hex notation
            //LF byte in hex notation
            //\x21\x08 is Bold.
            //string cmds = "\x1B \x45 \x5A Testing \x0D \x0A \x1B \x12 test2";

            //x1b\x61\x01 = Text Center Alignment (x00 - Left,x02 right)
            //x1b\x45\x01 - Bold Letter mode
            //x1b\x2d\x02 - Underline mode
            //x1b\x21\x10 - Enabled double - height mode
            //x1b\x21\x20 = Enabled double - width mode
            ////ESC = "\x1B\x46\x1B\x38";
            // "\x00"= Character font A selected (ESC ! 0)
            //// "\x18"; //Emphasized + Double-height mode selected (ESC ! (16 + 8)) 24 dec => 18 hex

            try
            {

                //var a = Utils.Print_SalesOrder(SelectedBthDevice, head, recs, customer);
                //UserDialogs.Instance.Alert(a);

                DataManager manager = new DataManager();
                Customer sellTo = manager.GetCustomerbyCode(head.SellToCustomer).Result;

                ObservableCollection<SalesLine> RecsCopy = new ObservableCollection<SalesLine>();
               
                

                ObservableCollection<SalesLine> RecsNotExc = new ObservableCollection<SalesLine>(recs.Where(x => x.ItemType != "EXC"));
                foreach (SalesLine s in RecsNotExc)
                {
                    RecsCopy.Add(s);
                }

                ObservableCollection<SalesLine> RecsExc = new ObservableCollection<SalesLine>(recs.Where(x => x.ItemType == "EXC"));

                List<SalesLine> result = RecsExc.GroupBy(x => x.ItemNo)
                        .Select(f => new SalesLine
                        {
                            DocumentNo = f.First().DocumentNo,
                            ItemNo = f.First().ItemNo,
                            Description = f.First().Description,
                            UnitofMeasurementCode = f.First().UnitofMeasurementCode,
                            Quantity = f.Sum(c => c.Quantity + c.BadQuantity),
                            UnitPrice = f.First().UnitPrice,
                            GoodReasonCode = f.First().GoodReasonCode,
                            BadReasonCode = f.First().BadReasonCode,
                            ItemType = "EXC"
                        }).ToList();

                foreach (SalesLine sl in result)
                {
                    RecsCopy.Add(sl);
                }

                ObservableCollection<SalesLine> ascRecs = new ObservableCollection<SalesLine>(RecsCopy.OrderBy(i => i.Description));
                //foreach(SalesLine x in RecsCopy)
                //{
                //    UserDialogs.Instance.Alert(x.ItemNo +" || " + x.Quantity, "Title", "OK");
                //}

                var a = Utils.Print_TaxInvoice(SelectedBthDevice, head, ascRecs, customer, sellTo, App.gSalesPersonCode + "/" + App.gSalesPersonName);
                UserDialogs.Instance.Alert(a);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }

        }

        private async Task<string> SaveExchangeItems(string docno,SalesLine record,decimal unitprice)
        {
            try
            {
                DataManager manager = new DataManager();
                SalesLine line = new SalesLine()
                {
                    ID = 0,
                    DocumentNo = docno,
                    ItemNo = record.ItemNo,
                    Description = record.Description,
                    UnitofMeasurementCode = record.UnitofMeasurementCode,
                    Quantity = record.Quantity,
                    BadQuantity = record.BadQuantity,
                    FOCQty = 0,
                    UnitPrice = unitprice,
                    ItemType = record.ItemType,
                };
                string retval = manager.SaveSQLite_SalesLine(line);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
            
        }
        private async Task<string> SaveExchangeDoc(string DocDate,string selltoCutomer,string custname,decimal subtotal, decimal gstAmount, decimal nettotal)
        {
            try
            {
                DataManager manager = new DataManager();
                

                string retval = await manager.SaveSQLite_SalesHeader(new SalesHeader
                {
                    ID = 0,
                    DocumentNo = ExchangeDocNo,
                    DocumentDate = DocDate,
                    BillToCustomer = selltoCutomer,
                    BillToName = selltoCutomer,
                    SellToCustomer = selltoCutomer,
                    SellToName = custname,
                    Status = "Released",
                    TotalAmount = subtotal,
                    GSTAmount = GSTAmount,
                    NetAmount = nettotal,
                    PaymentMethod = string.Empty,
                    StrSignature = _signature64str,
                    DocumentType = "CN",
                    Note = head.Note,
                    Comment = CommentEntry.Text
                    //IsSync = "false",
                    //SyncDateTime = string.Empty
                });
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
            
        }
    }
}