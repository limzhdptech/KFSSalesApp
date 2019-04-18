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
    public partial class SalesOrderEntryPage : ContentPage
    {
        readonly Database database;
        private SalesHeader data { get; set; }
        private int EntryNo { get; set; }
        private string DocNo { get; set; }
        private string DocType { get; set; }
        private decimal SubTotal { get; set; }
        private decimal GSTAmount { get; set; }
        public decimal NetTotal { get; set; }
        private string CurStatus { get; set; }
        private string paraButton { get; set; }
        private string BillToCustNo { get; set; }
        private string BilltoName { get; set; }
        private string originCust { get; set; }
        private ObservableCollection<SalesHeader> records { get; set; }

        //private bool IsNewEntry { get; set; }
        private bool IsBack { get; set; }
        //  bool hasClicked { get; set; }
        //  Action<object, EventArgs> _setClick;
        private bool _isloading;
        private bool _isEnableSaveBtn { get; set; }
        private bool _isEnableCustomerBtn { get; set; }
        public bool IsLoading
        {
            get { return this._isloading; }
            set
            {
                this._isloading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public SalesOrderEntryPage(int entryNo)
        {
            InitializeComponent();
            database = new Database(Constants.DatabaseName);
            database.CreateTable<SalesHeader>();
            Title = App.gPageTitle;
            EntryNo = entryNo;
            SellToCustomerEntry.Completed += SellToCustomerEntry_Completed;
            SellToCustomerEntry.Unfocused += SellToCustomerEntry_Unfocused;
            SelltoCustomerlookUpButton.ButtonIcon = AwasomeIcon.FASearch;
            SelltoCustomerlookUpButton.ButtonColor = Color.FromHex("#EC2029");
            SelltoCustomerlookUpButton.ButtonFontSize = 16;
            SelltoCustomerlookUpButton.OnTouchesEnded += lookUpButton_OnTouchesEnded;

            //BillToCustomerlookUpButton.ButtonIcon = AwasomeIcon.FASearch;
            //BillToCustomerlookUpButton.ButtonColor = Color.FromHex("#EC2029");
            //BillToCustomerlookUpButton.OnTouchesEnded += lookUpButton_OnTouchesEnded;

            this.BackgroundColor = Color.FromHex("#dddddd");
            saveButton.Clicked += SaveButton_Clicked;
            IsBack = false;
            IsLoading = false;
            BindingContext = this;
        }

        private void SellToCustomerEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (!string.IsNullOrEmpty(SellToCustomerEntry.Text))
            {
                DataManager manager = new DataManager();
                Customer customer = new Customer();
                customer = manager.GetSQLite_CustomerbyCustNo(SellToCustomerEntry.Text);
                if (customer != null)
                {
                    SellToCustomerEntry.Text = customer.CustomerNo;
                    SellToNameLabel.Text = customer.Name;
                    App.gCustPriceGroup = customer.CustomerPriceGroup;
                    manager = new DataManager();
                    Customer objCust = new Customer();
                    objCust = manager.GetSQLite_CustomerbyCustNo(customer.billtoCustNo);
                    if (objCust != null)
                    {
                        BillToCustNo = objCust.CustomerNo;
                        BilltoName = objCust.Name;
                    }
                    else
                    {
                        BillToCustNo = customer.CustomerNo;
                        BilltoName = customer.Name;
                    }
                }
                else
                {
                    UserDialogs.Instance.ShowError("Wrong customer no or customer does not existed!", 3000);

                    SellToCustomerEntry.Text = string.Empty;
                    SellToNameLabel.Text = string.Empty;
                    BillToCustNo = string.Empty;
                    BilltoName = string.Empty;
                    App.gCustPriceGroup = string.Empty;
                    SellToCustomerEntry.Focus();
                }
            } 
        }

        private void SellToCustomerEntry_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SellToCustomerEntry.Text))
            {
                DataManager manager = new DataManager();
                Customer customer = new Customer();
                customer = manager.GetSQLite_CustomerbyCustNo(SellToCustomerEntry.Text);
                if (customer != null)
                {
                    SellToCustomerEntry.Text = customer.CustomerNo;
                    SellToNameLabel.Text = customer.Name;
                    App.gCustPriceGroup = customer.CustomerPriceGroup;

                    manager = new DataManager();
                    Customer objCust = new Customer();
                    objCust = manager.GetSQLite_CustomerbyCustNo(customer.billtoCustNo);
                    if (objCust != null)
                    {
                        BillToCustNo = objCust.CustomerNo;
                        BilltoName = objCust.Name;
                    }
                    else
                    {
                        BillToCustNo = customer.CustomerNo;
                        BilltoName = customer.Name;
                    }
                }
                else
                {
                    UserDialogs.Instance.ShowError("Wrong customer no or customer does not existed!", 3000);

                    SellToCustomerEntry.Text = string.Empty;
                    SellToNameLabel.Text = string.Empty;
                    BillToCustNo = string.Empty;
                    BilltoName = string.Empty;
                    App.gCustPriceGroup = string.Empty;
                    SellToCustomerEntry.Focus();

                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _isEnableSaveBtn = true;
            _isEnableCustomerBtn = true;
            data = new SalesHeader();
            if (!IsBack)
            {
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
                try
                {
                    if (EntryNo!=0)
                    {
                        DataManager manager = new DataManager();
                        data = manager.GetSalesHeaderbyID(EntryNo);
                    } 
                    else
                        data = null;

                        DisplayData(data);
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;

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
            }
        }

        void DisplayData(SalesHeader record)
        {
            if (record != null)
            {
                EntryNo = record.ID;
                InvoiceNoLabel.Text = record.DocumentNo;
                //DocumentDatePicker.Date =Convert.ToDateTime(record.DocumentDate);
                DocDateTimeLabel.Text = record.DocumentDate;
                SellToCustomerEntry.Text = record.SellToCustomer;
                SellToNameLabel.Text = record.SellToName;
                ExternalDocNoEntry.Text = record.ExternalDocNo;
                BillToCustNo = record.BillToCustomer;
                BilltoName = record.BillToName;
                SubTotal = record.TotalAmount;
                GSTAmount = record.GSTAmount;
                NetTotal = record.NetAmount;
                CurStatus = record.Status;
                originCust = record.SellToCustomer;
            }
            else
            {
                EntryNo = 0;
                DataManager manager = new DataManager();
                if(App.gDocType=="SO")
                    InvoiceNoLabel.Text = manager.GetLastNoSeries(App.gSOPrefix); 
                else
                    InvoiceNoLabel.Text = manager.GetLastNoSeries(App.gCRPrefix);
                DocDateTimeLabel.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt");
                ExternalDocNoEntry.Text = string.Empty;
               // DocumentDatePicker.Date = DateTime.Today;
                SubTotal = 0;
                GSTAmount = 0;
                NetTotal = 0;
               // StatusPicker.SelectedItem = "Open";
                CurStatus = "Open";
                originCust = string.Empty;
                // StatusPicker.IsEnabled = false;
                // //Status: Open (same as in NAV), Released (same as in NAV), Completed (when Transfer Order is posted and deleted)
            }
        }

        private void lookUpButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            if(_isEnableCustomerBtn)
            {
                _isEnableCustomerBtn = false;
                var page = new CustomerPage();
                var obj = (ActionButton)sender;
                paraButton = obj.CommandParameter.ToString();
                page.listview.ItemSelected += Listview_ItemSelected;
                Navigation.PushAsync(page);
            }
        }

        private void Listview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            IsBack = true;
            var selectedItem = e.SelectedItem as Customer;
            if (paraButton == "SelltoCustomer")
            {
                SellToCustomerEntry.Text = selectedItem.CustomerNo;
                SellToNameLabel.Text = selectedItem.Name;
                App.gCustPriceGroup = selectedItem.CustomerPriceGroup;

                DataManager manager = new DataManager();
                Customer objCust = new Customer();
                objCust = manager.GetSQLite_CustomerbyCustNo(selectedItem.billtoCustNo);
                if(objCust != null)
                {
                    BillToCustNo = objCust.CustomerNo;
                    BilltoName = objCust.Name;
                }
                else
                {
                    BillToCustNo = selectedItem.CustomerNo;
                    BilltoName = selectedItem.Name;
                }
               // GetSQLite_CustomerbyCustNo(string custno)
                //BillToCustomerEntry.Text = selectedItem.CustomerNo;
                //BillToNameLabel.Text = selectedItem.Name;
            } 
            else
            {
                //BillToCustomerEntry.Text = selectedItem.CustomerNo;
                //BillToNameLabel.Text = selectedItem.Name;
            }
                
            Navigation.PopAsync();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if(_isEnableSaveBtn)
            {
                _isEnableSaveBtn = false;
                if (CurStatus == "Open")
                {
                    if (!string.IsNullOrEmpty(SellToCustomerEntry.Text))
                    {
                        DataManager manager = new DataManager();
                        string retval = await manager.SaveSQLite_SalesHeader(new SalesHeader
                        {
                            ID = EntryNo,
                            DocumentNo = InvoiceNoLabel.Text,
                            DocumentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"),//DocumentDatePicker.Date.ToString("yyyy-MM-dd"),
                            SellToCustomer = SellToCustomerEntry.Text,
                            SellToName = SellToNameLabel.Text,
                            BillToCustomer = BillToCustNo,
                            BillToName = BilltoName,
                            Status = "Open",
                            TotalAmount = SubTotal,
                            GSTAmount = GSTAmount,
                            NetAmount = NetTotal,
                            DocumentType = App.gDocType,
                            Note = string.Empty,
                            IsVoid = "false",
                            IsSync = "false",
                            SyncDateTime = string.Empty,
                            ExternalDocNo = ExternalDocNoEntry.Text
                        });

                        if (retval == "Success")
                        {
                            // Edit Sales line if Customer Changed
                            if (!string.IsNullOrEmpty(originCust))
                            {
                                if (originCust != SellToCustomerEntry.Text)
                                {
                                    ObservableCollection<SalesLine> prevLines = new ObservableCollection<SalesLine>();
                                    manager = new DataManager();
                                    prevLines = manager.GetSalesLinesbyDocNo(InvoiceNoLabel.Text);
                                    if (prevLines != null)
                                    {
                                        if (prevLines.Count > 0)
                                        {

                                            foreach (SalesLine s in prevLines)
                                            {

                                                SalesLine line = new SalesLine()
                                                {
                                                    ID = s.ID,
                                                    DocumentNo = s.DocumentNo,
                                                    ItemNo = s.ItemNo,
                                                    Description = s.Description,
                                                    UnitofMeasurementCode = s.UnitofMeasurementCode,
                                                    Quantity = s.Quantity,
                                                    BadQuantity = s.BadQuantity,
                                                    FOCQty = s.FOCQty,
                                                    UnitPrice = GetDefaultPrice(s.ItemNo),
                                                    GoodReasonCode = s.GoodReasonCode,
                                                    BadReasonCode = s.BadReasonCode,
                                                    ItemType = s.ItemType,
                                                    // BagNo=ScanBagEntry.Text
                                                    //IsSync = "false",
                                                    //SyncDateTime = string.Empty
                                                };
                                                retval = manager.SaveSQLite_SalesLine(line);
                                            }
                                            if (retval == "Success")
                                            {
                                                ObservableCollection<SalesLine> lines = new ObservableCollection<SalesLine>();
                                                lines = manager.GetSalesLinesbyDocNo(InvoiceNoLabel.Text);
                                                decimal new_amt = lines.Sum(x => x.LineAmount);
                                                retval = manager.UpdateSalesHeaderTotalAmount(new_amt, InvoiceNoLabel.Text, lines);
                                            }

                                        } // end check line count
                                    } // line!=null
                                }
                            }
                            if(EntryNo==0) //DD #284
                            {
                                if (App.gDocType == "SO")
                                    manager.IncreaseNoSeries(App.gSOPrefix, InvoiceNoLabel.Text, "SO");
                                else
                                    manager.IncreaseNoSeries(App.gCRPrefix, InvoiceNoLabel.Text, "CR");
                            }

                            UserDialogs.Instance.ShowSuccess(retval, 3000);
                            Navigation.PopAsync();
                        }
                        else
                        {
                            UserDialogs.Instance.ShowError(retval, 3000);
                            _isEnableSaveBtn = true;
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.ShowError("Required Sell to Customer!", 3000);
                        SellToCustomerEntry.Focus();
                        _isEnableSaveBtn = true;
                    }

                }
                else
                {
                    UserDialogs.Instance.ShowError("Not allow to save released Sales Order!", 3000);
                    _isEnableSaveBtn = true;
                }
            }
            
        }

        private decimal GetDefaultPrice(string itemNo)
        {
            decimal price=0;
            DataManager dm = new DataManager();
            List<SalesPrice> salesPrices = new List<SalesPrice>();
            salesPrices = dm.GetItemPricebyItemPriceGroup(itemNo, App.gCustPriceGroup);
            SalesPrice obj = salesPrices.OrderBy(x => x.UnitPrice).FirstOrDefault();
            if (obj != null)
            {
                price = obj.UnitPrice; //CalculateSalesPrice(price.UnitPrice).ToString();
                //if (!string.IsNullOrEmpty(QuantityEntry.Text))
                //{
                //    decimal qty = decimal.Parse(QuantityEntry.Text);
                //    ItemtotalLabel.Text = string.Format("{0:0.00}", decimal.Parse(price.UnitPrice.ToString()) * qty);
                //}

            }
            return price;
        }
    }
}