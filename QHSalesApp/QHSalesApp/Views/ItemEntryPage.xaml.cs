using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemEntryPage : ContentPage
    {
        readonly Database database;
        private SalesLine data { get; set; }
        private int EntryNo { get; set; }
        private string DocNo { get; set; }
        private string DocType { get; set; }
        private int cmdPara { get; set; }
        private decimal oldLineAmt { get; set; }
        private decimal LastQty { get; set; }
        private decimal LastBadQty { get; set; }
        private string LastItemType { get; set; }
        private ObservableCollection<SalesLine> records { get; set; }

        private string _itemType { get; set; }
        private string GoodReasonCode { get; set; }
        private string BadReasonCode { get; set; }
        private bool isItemExisted { get; set; }
        private bool IsBack { get; set; }

        private string ReasonCodeType { get; set;}
        // ContainerInfo BagInfo { get; set; }
        ContainerInfo tmpInfo { get; set; }
        //RequestLine ScanRequest { get; set; }
        private decimal MaxSoldQty { get; set; }

        private decimal CurQty { get; set; }
        private string _ItemNo { get; set; }

        private bool _isEnableSaveBtn { get; set; }
        private bool _isEnableDeleteBtn { get; set; }

        public bool _isEnableItemBtn { get; set; }


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
        private string SearchReasonCode { get; set; }
        public ItemEntryPage(int id, string docno)
        {
            InitializeComponent();

            Title = App.gPageTitle;
            App.gIsExchange = false;
            database = new Database(Constants.DatabaseName);
            database.CreateTable<SalesLine>();
            EntryNo = id;
            DocNo = docno;
            MaxSoldQty = 0;
            BarcodeEntry.Completed += BarcodeEntry_Completed;
            //BarcodeEntry.Unfocused += BarcodeEntry_Unfocused;

            ItemNolookUpButton.ButtonIcon = AwasomeIcon.FASearch;
            ItemNolookUpButton.ButtonColor = Color.FromHex("#EC2029");
            ItemNolookUpButton.ButtonFontSize = 16;
            ItemNolookUpButton.OnTouchesEnded += ItemlookUpButton_OnTouchesEnded;

            GoodReasonCodelookUpButton.ButtonIcon = AwasomeIcon.FASearch;
            GoodReasonCodelookUpButton.ButtonColor = Color.FromHex("#EC2029");
            GoodReasonCodelookUpButton.ButtonFontSize = 16;
            GoodReasonCodelookUpButton.OnTouchesEnded += GoodReasonCodelookUpButton_OnTouchesEnded;

            BadReasonCodelookUpButton.ButtonIcon = AwasomeIcon.FASearch;
            BadReasonCodelookUpButton.ButtonColor = Color.FromHex("#EC2029");
            BadReasonCodelookUpButton.ButtonFontSize = 16;
            BadReasonCodelookUpButton.OnTouchesEnded += BadReasonCodelookUpButton_OnTouchesEnded;

            if (EntryNo>0)
            {
                // BarcodeEntry.IsVisible = false;
                BarcodeEntry.IsEnabled = false;
                ItemNolookUpButton.IsVisible = false;
            }

            if (App.gDocType == "SO")
            {
                GoodReasonCodeLabel.IsVisible = false;
                GoodReasonCodeLayout.IsVisible = false;
                BadReasonCodeLabel.IsVisible = false;
                BadReasonCodeLayout.IsVisible = false;
                BadQuantityEntry.IsVisible = false;
                BadQuantityLabel.IsVisible = false;
                ExchangeItemLayout.IsVisible = true;
                FOCItemCheckbox.IsVisible = true;
               
            }
            else
            {
                GoodReasonCodeLabel.IsVisible = true;
                GoodReasonCodeLayout.IsVisible = true;
                BadReasonCodeLabel.IsVisible = true;
                BadReasonCodeLayout.IsVisible = true;
                BadQuantityEntry.IsVisible = true;
                BadQuantityLabel.IsVisible = true;
                ExchangeItemLayout.IsVisible = false;
                FOCItemCheckbox.IsVisible = false;
                
            }

            //if (App.gDocType == "SO")
            //{
            //    ItemNoEntry.IsEnabled = false;
            //    ItemNolookUpButton.IsVisible = false;
            //    if (EntryNo != 0) ScanBagEntry.IsEnabled = false;
            //}
            //else
            //{
            //    ScanBagLabel.IsVisible = false;
            //    ScanBagLayout.IsVisible = false;
            //}


            //UnitPricelookUpButton.ButtonIcon = AwasomeIcon.FASearch;
            //UnitPricelookUpButton.ButtonColor = Color.FromHex("#EC2029");
            //UnitPricelookUpButton.OnTouchesEnded += UnitPricelookUpButton_OnTouchesEnded; 

            QuantityEntry.Completed += QuantityEntry_Completed;
            QuantityEntry.Unfocused += QuantityEntry_Unfocused;

            BadQuantityEntry.Completed += BadQuantityEntry_Completed;
            BadQuantityEntry.Unfocused += BadQuantityEntry_Unfocused;

            UnitPriceEntry.Completed += UnitPriceEntry_Completed;
            UnitPriceEntry.Unfocused += UnitPriceEntry_Unfocused;
            // FOCQtyEntry.Completed += FOCQtyEntry_Completed;
            FOCItemCheckbox.CheckedChanged += FOCItemCheckbox_CheckedChanged;
            ExchangeItemCheckbox.CheckedChanged += ExchangeItemCheckbox_CheckedChanged;
           // ScanBagEntry.Completed += ScanBagEntry_Completed;
            this.BackgroundColor = Color.FromHex("#dddddd");
            saveButton.Clicked += SaveButton_Clicked;
            DeleteButton.Clicked += DeleteButton_Clicked;
            IsBack = false;
            IsLoading = false;
            BindingContext = this;
        }

        private void GoodReasonCodelookUpButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            //var obj = (ActionButton)sender;
            //cmdPara = int.Parse(obj.CommandParameter.ToString());

            //var page = new ItemsPage(_filter);
            //page.listview.ItemSelected += ItemListview_ItemSelected;
            //Navigation.PushAsync(page);
            if(App.gIsEnableReasonCode)
            {
                App.gIsEnableReasonCode = false;
                if (App.gDocType == "SO")
                {
                    if (FOCItemCheckbox.Checked)
                        SearchReasonCode = "INV-1";
                    if (ExchangeItemCheckbox.Checked)
                        SearchReasonCode = "INV-2";
                }
                if (App.gDocType == "CN") SearchReasonCode = "CN-1";
                ReasonCodeType = "Good";
                var obj = (ActionButton)sender;
                var page = new ReasonCodePopupPage(SearchReasonCode);
                page.Reasonlistview.ItemSelected += ReasonListview_ItemSelected;
                Navigation.PushPopupAsync(page);
            }
            
        }
        private void BadReasonCodelookUpButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            if(App.gIsEnableReasonCode)
            {
                App.gIsEnableReasonCode = false;
                ReasonCodeType = "Bad";
                if (App.gDocType == "SO")
                {
                    if (ExchangeItemCheckbox.Checked)
                        SearchReasonCode = "INV-3";
                }

                if (App.gDocType == "CN") SearchReasonCode = "CN-2";
                var obj = (ActionButton)sender;
                var page = new ReasonCodePopupPage(SearchReasonCode);
                page.Reasonlistview.ItemSelected += ReasonListview_ItemSelected;
                Navigation.PushPopupAsync(page);
            }
            
        }

        private async void ReasonListview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            IsBack = true;
            var selectedItem = e.SelectedItem as ReasonCode;
            if(ReasonCodeType=="Good")
            {
                GoodReasonCode = selectedItem.Code;
                GoodReasonCodeEntry.Text = selectedItem.Description;
            }   
            else
            {
                BadReasonCode = selectedItem.Code;
                BadReasonCodeEntry.Text = selectedItem.Description;
            }
            App.gIsEnableReasonCode = true;
            await PopupNavigation.PopAsync();
        }
        private void ExchangeItemCheckbox_CheckedChanged(object sender, XLabs.EventArgs<bool> e)
        {
            if (ExchangeItemCheckbox.Checked == true)
            {
                GoodReasonCodeLabel.Text = "Good Reason Code";
                App.gIsExchange = true;
              
                FOCItemCheckbox.Checked = false;
                FOCItemCheckbox.IsEnabled = false;
                UnitPriceEntry.IsEnabled = false;
                GoodReasonCodeLabel.IsVisible = true;
                GoodReasonCodeLayout.IsVisible = true;
                BadReasonCodeLabel.IsVisible = true;
                BadReasonCodeLayout.IsVisible = true;
                BadQuantityEntry.IsVisible = true;
                BadQuantityLabel.IsVisible = true;
                UnitPriceEntry.Text = string.Format("{0:0.00}", 0);
                ItemtotalLabel.Text = string.Format("{0:0.00}", 0);
            }
            else
            {
                GoodReasonCodeLabel.Text = "Good Reason Code";
                App.gIsExchange = false;
                SearchReasonCode = string.Empty;
                GoodReasonCodeLabel.IsVisible = false;
                GoodReasonCodeLayout.IsVisible = false;
                BadReasonCodeLabel.IsVisible = false;
                BadReasonCodeLayout.IsVisible = false;
                BadQuantityEntry.IsVisible = false;
                BadQuantityLabel.IsVisible = false;
                FOCItemCheckbox.IsEnabled = true;
                UnitPriceEntry.IsEnabled = true;
                GoodReasonCode = string.Empty;
                BadReasonCode = string.Empty;
                GoodReasonCodeEntry.Text = string.Empty;
                BadReasonCodeEntry.Text = string.Empty;
                // bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsNumber(x) || x == '-');
                decimal excQty = 0;
                var isnumeric=  decimal.TryParse(QuantityEntry.Text,out  excQty);
                if(isnumeric)
                {
                    if (excQty < 0)
                    {
                        QuantityEntry.Text = "0";
                    }
                }
                else
                    QuantityEntry.Text = "0";

                if (!string.IsNullOrEmpty(_ItemNo))
                    LoadDefaultPrice(_ItemNo, App.gCustPriceGroup);

                if (LastItemType == "EXC")
                {
                    MaxSoldQty = MaxSoldQty + LastBadQty-LastQty;
                }
            }
        }
        private void FOCItemCheckbox_CheckedChanged(object sender, XLabs.EventArgs<bool> e)
        {
            if(FOCItemCheckbox.Checked==true)
            {
                SearchReasonCode = "INV-1";
                ExchangeItemCheckbox.Checked = false;
                ExchangeItemCheckbox.IsEnabled = false;
                GoodReasonCodeLabel.IsVisible = true;
                GoodReasonCodeLayout.IsVisible = true;
                BadReasonCodeLabel.IsVisible = false;
                BadReasonCodeLayout.IsVisible = false;
                UnitPriceEntry.IsEnabled = false;
                UnitPriceEntry.Text = string.Format("{0:0.00}", 0);
                ItemtotalLabel.Text = string.Format("{0:0.00}", 0);
            }
            else
            {
                GoodReasonCodeLabel.IsVisible = false;
                GoodReasonCodeLayout.IsVisible = false;
                BadReasonCodeLabel.IsVisible = false;
                BadReasonCodeLayout.IsVisible = false;
                ExchangeItemCheckbox.IsEnabled = true;
                GoodReasonCode = string.Empty;
                BadReasonCode = string.Empty;
                GoodReasonCodeEntry.Text = string.Empty;
                BadReasonCodeEntry.Text = string.Empty;
                UnitPriceEntry.IsEnabled = true;
                if (!string.IsNullOrEmpty(_ItemNo))
                    LoadDefaultPrice(_ItemNo, App.gCustPriceGroup);
            }
        }

        protected override  void OnAppearing()
        {
            base.OnAppearing();
            _isEnableSaveBtn = true;
            _isEnableDeleteBtn = true;
            _isEnableItemBtn = true;
            App.gIsEnableReasonCode = true;
            DisplayData();
            BarcodeEntry.Focus();
        }

         void DisplayData()
        {
            try
            {
                data = new SalesLine();
                Item item = new Item();
                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {
                    if(EntryNo!=0)
                    {
                        DataManager manager = new DataManager();
                        data = await manager.GetSalesLinebyID(EntryNo);
                        if(data!=null)
                            item = manager.GetSQLite_ItembyItemNo(data.ItemNo);
                    }
                    else
                    {
                        data = null;
                    }
                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {
                    
                    if (!IsBack)
                    {
                        if (data != null)
                        {
                            string strImage = Regex.Replace(item.Str64Img, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                            ItemImage.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(strImage)));
                           
                            EntryNo = data.ID;
                            //DocumentNoEntry.Text = dt.Rows[0]["DocumentNo"].ToString();
                            ItemNoLabel.Text = data.ItemNo;
                            _ItemNo = data.ItemNo;
                            LastItemType = data.ItemType;
                            _itemType = data.ItemType;
                            DescLabel.Text = data.Description;
                            UomEntry.Text = data.UnitofMeasurementCode;
                            QuantityEntry.Text = data.Quantity.ToString();
                            CurQty = data.Quantity;
                            BadQuantityEntry.Text = data.BadQuantity.ToString();
                            LastQty = data.Quantity;
                            LastBadQty = data.BadQuantity;
                            BarcodeEntry.Text = data.BagNo;
                            //  FOCQtyEntry.Text = data.FOCQty.ToString();
                            UnitPriceEntry.Text = string.Format("{0:0.00}", data.UnitPrice);
                            oldLineAmt = data.LineAmount;
                            ItemtotalLabel.Text = string.Format("{0:0.00}", data.LineAmount);
                            CalculateMaxSoldQty();
                            if (data.ItemType == "FOC")
                            {
                                FOCItemCheckbox.Checked = true;
                            }
                            else if (data.ItemType == "EXC")
                            {
                                
                                ExchangeItemCheckbox.Checked = true;
                                CurQty = data.Quantity + data.BadQuantity;
                                //App.gIsExchange = true;
                                //LastQty = 0;
                                //if(data.Quantity<0)
                                //{
                                //    LastQty = data.Quantity ;
                                //}
                            }
                            else
                            {
                                FOCItemCheckbox.Checked = false;
                                ExchangeItemCheckbox.Checked = false;
                            }
                            DataManager manager = new DataManager();
                            GoodReasonCode = data.GoodReasonCode;
                            string goodReasonDesc= manager.GetSQLite_ReasonDesc(GoodReasonCode);
                            GoodReasonCodeEntry.Text = goodReasonDesc;
                            BadReasonCode = data.BadReasonCode;
                            string BadReasonDesc = manager.GetSQLite_ReasonDesc(BadReasonCode);
                            BadReasonCodeEntry.Text = BadReasonDesc;
                            isItemExisted = true;
                        }
                        else
                        {
                            string strImage = Regex.Replace(Constants.BlankImgStr, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                            ItemImage.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(strImage)));
                            ItemNoLabel.Text = "Scan or select Item";
                            _ItemNo = string.Empty;
                            _itemType = string.Empty;
                            LastItemType = string.Empty;
                            DescLabel.Text = string.Empty;
                            UomEntry.Text = string.Empty;
                            GoodReasonCode = string.Empty;
                            BadReasonCode = string.Empty;
                            GoodReasonCodeEntry.Text = string.Empty;
                            BadReasonCodeEntry.Text = string.Empty;
                            QuantityEntry.Text = "0";
                            BadQuantityEntry.Text = "0";
                            //  FOCQtyEntry.Text = "0";
                            UnitPriceEntry.Text = string.Format("{0:0.00}", 0);
                            // UnitPriceEntry.Text = "0";
                            ItemtotalLabel.Text = string.Format("{0:0.00}", 0);
                            oldLineAmt = 0;
                            LastQty = 0;
                            LastBadQty = 0;
                            CurQty = 0;
                            MaxSoldQty = 0;
                            isItemExisted = false;
                            FOCItemCheckbox.Checked = false;
                            ExchangeItemCheckbox.Checked = false;
                            App.gIsExchange = false;
                            DeleteButton.IsEnabled = false;
                        }
                    }
                    UserDialogs.Instance.HideLoading();
                }));
            }
            catch (OperationCanceledException ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert","OK");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert","OK");
            }
        }
        private async void BarcodeEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (!string.IsNullOrEmpty(BarcodeEntry.Text))
            {
                DataManager manager = new DataManager();
                Item item = new Item();
                item = await manager.GetSQLite_ItembyBarCode(BarcodeEntry.Text);
                if (item != null)
                {
                    ItemNoLabel.Text = item.ItemNo;
                    DescLabel.Text = item.Description;
                    string strImage = Regex.Replace(item.Str64Img, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                    ItemImage.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(strImage)));
                    UomEntry.Text = item.BaseUOM;

                    LoadDefaultPrice(item.ItemNo, App.gCustPriceGroup);
                    MaxSoldQty = item.LoadQty - item.SoldQty;
                    isItemExisted = true;
                    QuantityEntry.Focus();
                }
                else
                {
                    UserDialogs.Instance.Alert("Wrong barcode No or Item does not existed!", "Alert","OK");

                    BarcodeEntry.Text = "Scan or select Item";
                    DescLabel.Text = string.Empty;
                    MaxSoldQty = 0;
                    isItemExisted = false;
                    BarcodeEntry.Focus();
                }
            }
        }

         void CalculateMaxSoldQty()
        {
            DataManager manager = new DataManager();
            Item item = new Item();
            item = manager.GetSQLite_ItembyItemNo(_ItemNo);
            if(item!=null)
            {
                //BalQty => LoadQty + ReturnQty + BadQty - SoldQty;
                MaxSoldQty = (item.LoadQty+CurQty)+item.ReturnQty - item.SoldQty;
            }
            else
            {
                MaxSoldQty = CurQty+0;
            }
        }

        //private async void BarcodeEntry_Completed(object sender, EventArgs e)
        //{
        //    DataManager manager = new DataManager();
        //    VanItem vitem = new VanItem();
        //    Item item = new Item();
        //    try
        //    {
        //        if (string.IsNullOrEmpty(BarcodeEntry.Text))
        //        {
        //            return;
        //        }
        //        UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
        //        if (App.gDocType == "SO")
        //        {
        //            vitem = await manager.GetSQLite_VanItembyBarCode(BarcodeEntry.Text);
        //        }
        //        else
        //        {
        //            item = await manager.GetSQLite_ItembyBarCode(BarcodeEntry.Text);
        //        }

        //        if (App.gDocType == "SO")
        //        {
        //            if (vitem != null)
        //            {
        //                ItemNoLabel.Text = vitem.ItemNo;
        //                _ItemNo = vitem.ItemNo;
        //                DescLabel.Text = vitem.Description;
        //                ItemImage.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(vitem.Str64Img)));
        //                UomEntry.Text = vitem.BaseUOM;
        //                LoadDefaultPrice(vitem.ItemNo, App.gCustPriceGroup);
        //                CalculateMaxSoldQty();
        //                isItemExisted = true;

        //                //  QuantityEntry.Focus();
        //            }
        //            else
        //            {
        //                ItemNoLabel.Text = "Scan or select Item";
        //                _itemType = string.Empty;
        //                BarcodeEntry.Text = string.Empty;
        //                DescLabel.Text = string.Empty;
        //                MaxSoldQty = 0;
        //                isItemExisted = false;
        //                UserDialogs.Instance.ShowError("Wrong barcode No or Item does not existed!", "Alert","OK");
        //            }
        //        }
        //        else
        //        {
        //            if (item != null)
        //            {
        //                ItemNoLabel.Text = item.ItemNo;
        //                _ItemNo = item.ItemNo;
        //                DescLabel.Text = item.Description;
        //                ItemImage.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(item.Str64Img)));
        //                UomEntry.Text = item.BaseUOM;
        //                LoadDefaultPrice(item.ItemNo, App.gCustPriceGroup);
        //                isItemExisted = true;

        //                //  QuantityEntry.Focus();
        //            }
        //            else
        //            {
        //                ItemNoLabel.Text = "Scan or select Item";
        //                _itemType = string.Empty;
        //                BarcodeEntry.Text = string.Empty;
        //                DescLabel.Text = string.Empty;
        //                isItemExisted = false;
        //                UserDialogs.Instance.ShowError("Wrong barcode No or Item does not existed!", "Alert","OK");
        //            }
        //        }
        //        UserDialogs.Instance.HideLoading();
        //    }
        //    catch (OperationCanceledException ex)
        //    {
        //        Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
        //        UserDialogs.Instance.ShowError(ex.Message.ToString(), "Alert","OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        UserDialogs.Instance.HideLoading(); //IsLoading = false;
        //        UserDialogs.Instance.ShowError(ex.Message.ToString(), "Alert","OK");
        //    }
        //}

        private void BarcodeEntry_Completed(object sender, EventArgs e)
        {
           
            if (string.IsNullOrEmpty(BarcodeEntry.Text))
            {
                return;
            }
            try
            {
                DataManager manager = new DataManager();
                VanItem vitem = new VanItem();
                Item item = new Item();
                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {
                    if (App.gDocType == "SO")
                    {
                        vitem = await manager.GetSQLite_VanItembyBarCode(BarcodeEntry.Text);
                    }
                    else
                    {
                        item = await manager.GetSQLite_ItembyBarCode(BarcodeEntry.Text);
                    }
                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {
                    if (App.gDocType == "SO")
                    {

                        if (vitem != null)
                        {
                            ItemNoLabel.Text = vitem.ItemNo;
                            _ItemNo = vitem.ItemNo;
                            DescLabel.Text = vitem.Description;
                            string strImage = Regex.Replace(vitem.Str64Img, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                            ItemImage.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(strImage)));
                            UomEntry.Text = vitem.BaseUOM;
                            LoadDefaultPrice(vitem.ItemNo, App.gCustPriceGroup);
                            CalculateMaxSoldQty();
                            isItemExisted = true;

                            //  QuantityEntry.Focus();
                        }
                        else
                        {
                            ItemNoLabel.Text = "Scan or select Item";
                            _itemType = string.Empty;
                            BarcodeEntry.Text = string.Empty;
                            DescLabel.Text = string.Empty;
                            MaxSoldQty = 0;
                            isItemExisted = false;
                            UserDialogs.Instance.Alert("Wrong barcode No or Item does not existed!", "Alert","OK");
                        }
                    }
                    else
                    {
                        if (item != null)
                        {
                            ItemNoLabel.Text = item.ItemNo;
                            _ItemNo = item.ItemNo;
                            DescLabel.Text = item.Description;
                            string strImage = Regex.Replace(item.Str64Img, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                            ItemImage.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(strImage)));
                            UomEntry.Text = item.BaseUOM;
                            LoadDefaultPrice(item.ItemNo, App.gCustPriceGroup);
                            isItemExisted = true;

                            //  QuantityEntry.Focus();
                        }
                        else
                        {
                            ItemNoLabel.Text = "Scan or select Item";
                            _itemType = string.Empty;
                            BarcodeEntry.Text = string.Empty;
                            DescLabel.Text = string.Empty;
                            isItemExisted = false;
                            UserDialogs.Instance.Alert("Wrong barcode No or Item does not existed!", "Alert","OK");
                        }
                    }
                    UserDialogs.Instance.HideLoading();
                }));
            }
            catch (OperationCanceledException ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert","OK");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert","OK");
            }
        }  
        private void UnitPriceEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(QuantityEntry.Text))
            {
                //QuantityEntry.Focus();
                //DependencyService.Get<IMessage>().LongAlert("Not allow blank  quantity!");
                UserDialogs.Instance.Alert("Not allow blank  quantity!", "Alert","OK");
                return;
            }

            if (App.gDocType == "SO")
            {
                if (decimal.Parse(QuantityEntry.Text) == 0)
                {
                    //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                    UserDialogs.Instance.Alert("Not allow 0 quantity!", "Alert","OK");
                    // QuantityEntry.Focus();
                    return;
                }

                if (FOCItemCheckbox.Checked == false && ExchangeItemCheckbox.Checked == false)
                {
                    if (decimal.Parse(UnitPriceEntry.Text) == 0)
                    {
                        UserDialogs.Instance.Alert("Unit price should not be 0!", "Alert","OK");
                        // UnitPriceEntry.Focus();
                        return;
                    }
                }

            }

            

            if (!string.IsNullOrEmpty(UnitPriceEntry.Text))
            {
                
                decimal QtyTotal = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                UnitPriceEntry.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text));
                ItemtotalLabel.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text) * QtyTotal);
                saveButton.Focus();
            }
            else
            {
                //DependencyService.Get<IMessage>().LongAlert("Empty Unit Price!");
                UserDialogs.Instance.Alert("Empty Unit Price!", "Alert","OK");
               // this.Focus();
            }
        }
        private void UnitPriceEntry_Completed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(QuantityEntry.Text))
            {
               // QuantityEntry.Focus();
                //DependencyService.Get<IMessage>().LongAlert("Not allow blank  quantity!");
                UserDialogs.Instance.Alert("Not allow blank  quantity!", "Alert","OK");
                return;
            }

            if (App.gDocType == "SO")
            {
                if (decimal.Parse(QuantityEntry.Text) == 0)
                {
                    //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                    UserDialogs.Instance.Alert("Not allow 0 quantity!", "Alert","OK");
                    // QuantityEntry.Focus();
                    return;
                }

                if (FOCItemCheckbox.Checked == false && ExchangeItemCheckbox.Checked == false)
                {
                    if (decimal.Parse(UnitPriceEntry.Text) == 0)
                    {
                        UserDialogs.Instance.Alert("Unit price should not be 0!", "Alert","OK");
                        // UnitPriceEntry.Focus();
                        return;
                    }
                }
            }

            if (!string.IsNullOrEmpty(UnitPriceEntry.Text))
            {
                decimal QtyTotal = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                UnitPriceEntry.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text));
                ItemtotalLabel.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text) * QtyTotal);
                saveButton.Focus();
            }
            else
            {
                //DependencyService.Get<IMessage>().LongAlert("Empty Unit Price!");
                UserDialogs.Instance.Alert("Unit Price is empty!", "Alert","OK");
               // this.Focus();
            }
        }
        private void FOCQtyEntry_Completed(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(FOCQtyEntry.Text))
            //{
            //    LoadDefaultPrice(ItemNoEntry.Text, App.gCustCode);
            //}
            //else
            //{
            //    DependencyService.Get<IMessage>().LongAlert("Not allow blank foc quantity!");
            //    FOCQtyEntry.Focus();
            //}
        }
        private void UnitPricelookUpButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            var obj = (ActionButton)sender;
            cmdPara = int.Parse(obj.CommandParameter.ToString());
            var page = new PriceLookupPage(_ItemNo,App.gCustCode);
            page.pricelistview.ItemSelected += UnitPriceListview_ItemSelected;
            Navigation.PushAsync(page);
        }
        private void UomlookUpButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            var obj = (ActionButton)sender;
            string itemNo = ItemNoLabel.Text;
            var page = new LookupUOMPage(itemNo);
            page.listview.ItemSelected += Listview_ItemSelected;
            Navigation.PushAsync(page);
        }
        private void ItemlookUpButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            if(_isEnableItemBtn)
            {
                _isEnableItemBtn = false;
                var obj = (ActionButton)sender;

                cmdPara = int.Parse(obj.CommandParameter.ToString());
                if (App.gDocType == "SO")
                {
                    var page = new VanItemPage();
                    page.listview.ItemSelected += VanItemListview_ItemSelected;
                    Navigation.PushAsync(page);
                }
                else
                {
                    var page = new ItemsPage();
                    page.listview.ItemSelected += ItemListview_ItemSelected;
                    Navigation.PushAsync(page);
                }
            }
           
        }
        private void ItemListview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            IsBack = true;
            var selectedItem = e.SelectedItem as Item;
            ItemNoLabel.Text = selectedItem.ItemNo;
            _ItemNo = selectedItem.ItemNo;
            
            string strImage = Regex.Replace(selectedItem.Str64Img, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
            ItemImage.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(strImage)));
            BarcodeEntry.Text = selectedItem.BarCode;
            DescLabel.Text = selectedItem.Description;
            UomEntry.Text = selectedItem.BaseUOM;
            LoadDefaultPrice(selectedItem.ItemNo, App.gCustPriceGroup);
            isItemExisted = true;
            CalculateMaxSoldQty();
            //UnitPricePicker.IsEnabled = true;
            Navigation.PopAsync();
        }
        private void VanItemListview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            IsBack = true;
            var selectedItem = e.SelectedItem as VanItem;
            ItemNoLabel.Text = selectedItem.ItemNo;
            _ItemNo = selectedItem.ItemNo;
    
            string strImage = Regex.Replace(selectedItem.Str64Img, @"^data:image\/[a-zA-Z]+;base64,", string.Empty); 
            ItemImage.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(strImage)));
            BarcodeEntry.Text = selectedItem.BarCode;
            DescLabel.Text = selectedItem.Description;
            UomEntry.Text = selectedItem.BaseUOM;
            LoadDefaultPrice(selectedItem.ItemNo, App.gCustPriceGroup);
            isItemExisted = true;
            CalculateMaxSoldQty();
            //UnitPricePicker.IsEnabled = true;
            Navigation.PopAsync();
        }
        private void UnitPriceListview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            IsBack = true;
            var selectedItem = e.SelectedItem as SalesPrice;
            decimal qty = decimal.Parse(QuantityEntry.Text);
            //  decimal foc = decimal.Parse(FOCQtyEntry.Text);
            UnitPriceEntry.Text = selectedItem.UnitPrice.ToString(); //CalculateSalesPrice(selectedItem.UnitPrice).ToString();
            ItemtotalLabel.Text = string.Format("{0:0.00}", selectedItem.UnitPrice * qty);
          //  LoadSalesPricePicker(selectedItem.ItemNo, App.gCustCode);
            //UnitPricePicker.IsEnabled = true;
            Navigation.PopAsync();
        }
        private decimal CalculateSalesPrice(decimal unitprice)
        {
            decimal qty = decimal.Parse(QuantityEntry.Text);
            decimal foc = 0;// decimal.Parse(FOCQtyEntry.Text);
            decimal saleprice = 0;
            if (qty != 0)
                saleprice = (unitprice * (qty + foc)) / qty;
            else
                saleprice = unitprice;
            return saleprice;
        }
        private  void LoadDefaultPrice(string itemNo, string itemPriceGroup)
        {
            DataManager dm = new DataManager();
            List<SalesPrice> salesPrices = new List<SalesPrice>();
            salesPrices =  dm.GetItemPricebyItemPriceGroup(itemNo, itemPriceGroup);

            List<SalesPrice> query = new List<SalesPrice>();
            query = salesPrices.Where(x => x.StartDate <= DateTime.Today && x.EndDate >= DateTime.Today).OrderBy(x => (DateTime.Today-x.EndDate).TotalDays + (DateTime.Today-x.StartDate).TotalDays).ToList();
            //SalesPrice price = salesPrices.OrderBy(x => x.UnitPrice).FirstOrDefault();
            SalesPrice price = query.FirstOrDefault();
            if (price != null)
            {
                if (ExchangeItemCheckbox.Checked == true || FOCItemCheckbox.Checked==true)
                    UnitPriceEntry.Text  = "0.00";
                else
                    UnitPriceEntry.Text = price.UnitPrice.ToString(); 
                //CalculateSalesPrice(price.UnitPrice).ToString();
                if(!string.IsNullOrEmpty(QuantityEntry.Text))
                {
                    decimal qty = decimal.Parse(QuantityEntry.Text);
                    ItemtotalLabel.Text = string.Format("{0:0.00}", decimal.Parse(price.UnitPrice.ToString()) * qty);
                }
            }

            
        }
        private void Listview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            IsBack = true;
            var selectedItem = e.SelectedItem as ItemUOM;
            UomEntry.Text = selectedItem.UOMCode;
            Navigation.PopAsync();
        }
        private void QuantityEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(QuantityEntry.Text))
            {
                QuantityEntry.Text = "0";
                //QuantityEntry.Focus();
                UserDialogs.Instance.Alert("Not allow blank  quantity!", "Alert","OK");
                //  DependencyService.Get<IMessage>().LongAlert("Not allow blank  quantity!");
                return;
            }

            if (App.gDocType == "SO")
            {
                if (ExchangeItemCheckbox.Checked)
                {
                    decimal excTotalQty = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                    if (excTotalQty == 0)
                    {
                        //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                        UserDialogs.Instance.Alert("Quantity is not allow 0!", "Alert","OK");
                        //QuantityEntry.Focus();
                        return;
                    }

                    if (excTotalQty > MaxSoldQty)
                    {
                        UserDialogs.Instance.Alert("Quantity is more than maximum quantity = "+ MaxSoldQty.ToString() +"!", "Alert","OK");
                        return;
                    }
                }
                else
                {
                    if (decimal.Parse(QuantityEntry.Text) == 0)
                    {
                        //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                        UserDialogs.Instance.Alert("Not allow 0 quantity!", "Alert","OK");
                        //QuantityEntry.Focus();
                        return;
                    }

                    if (decimal.Parse(QuantityEntry.Text) > MaxSoldQty)
                    {
                        UserDialogs.Instance.Alert("Quantity is more than maximum quantity = " + MaxSoldQty.ToString() + "!", "Alert","OK");
                        //QuantityEntry.Text = MaxSoldQty.ToString();
                        //QuantityEntry.Focus();
                        return;
                    }
                }
            }

            if (!string.IsNullOrEmpty(UnitPriceEntry.Text))
            {
                decimal QtyTotal = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                UnitPriceEntry.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text));
                ItemtotalLabel.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text) * QtyTotal);
                if (App.gDocType != "SO")
                {
                    BadQuantityEntry.Focus();
                }
                else
                    saveButton.Focus();
            }
            else
            {
                decimal QtyTotal = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                UnitPriceEntry.Text = string.Format("{0:#,##0.00}", 0);
                ItemtotalLabel.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text) * QtyTotal);
            }
        }
        private void QuantityEntry_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(QuantityEntry.Text))
            {
                if (App.gDocType == "SO")
                {
                    if (ExchangeItemCheckbox.Checked)
                    {
                        decimal excTotalQty = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                        if (excTotalQty == 0)
                        {
                            //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                            UserDialogs.Instance.Alert("Quantity is not allow 0!", "Alert","OK");
                            //QuantityEntry.Focus();
                            return;
                        }

                        if (excTotalQty > MaxSoldQty)
                        {
                            UserDialogs.Instance.Alert("Quantity is more than maximum quantity = " + MaxSoldQty.ToString() + "!", "Alert","OK");
                            return;
                        }
                    }
                    else
                    {
                        if (decimal.Parse(QuantityEntry.Text) == 0)
                        {
                            //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                            UserDialogs.Instance.Alert("Not allow 0 quantity!", "Alert","OK");
                            //QuantityEntry.Focus();
                            return;
                        }

                        if (decimal.Parse(QuantityEntry.Text) > MaxSoldQty)
                        {
                            UserDialogs.Instance.Alert("Quantity is more than maximum quantity = " + MaxSoldQty.ToString() + "!", "Alert","OK");
                            //QuantityEntry.Text = MaxSoldQty.ToString();
                            //QuantityEntry.Focus();
                            return;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(UnitPriceEntry.Text))
                {
                    decimal QtyTotal = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                    UnitPriceEntry.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text));
                    ItemtotalLabel.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text) * QtyTotal);
                    //ItemtotalLabel.Text = string.Format("{0:0.00}", decimal.Parse(UnitPriceEntry.Text) * decimal.Parse(QuantityEntry.Text));
                }
                else
                {
                    decimal QtyTotal = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                    UnitPriceEntry.Text = string.Format("{0:#,##0.00}", 0);
                    ItemtotalLabel.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text) * QtyTotal);
                }

                if (App.gDocType != "SO")
                {
                    BadQuantityEntry.Focus();
                }
                else
                    saveButton.Focus();
            }
            else
            {
                QuantityEntry.Text = "0";
                //DependencyService.Get<IMessage>().LongAlert("Not allow blank  quantity!");
                UserDialogs.Instance.Alert("Not allow blank  quantity!", "Alert","OK");
                //QuantityEntry.Focus();
            }
        }
        private void BadQuantityEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(BadQuantityEntry.Text))
            {
                BadQuantityEntry.Text = "0";
                //UnitPriceEntry.Focus();
            }
            else
            {
                if (App.gDocType == "SO")
                {
                    if (ExchangeItemCheckbox.Checked)
                    {

                        decimal excTotalQty = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                        if (excTotalQty == 0)
                        {
                            //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                            UserDialogs.Instance.Alert("Quantity is not allow 0!", "Alert","OK");
                            //QuantityEntry.Focus();
                            return;
                        }

                        if (excTotalQty > MaxSoldQty)
                        {
                            UserDialogs.Instance.Alert("Quantity is more than maximum quantity = " + MaxSoldQty.ToString() + "!", "Alert", "OK");
                            return;
                        }
                    }

                }

                if (!string.IsNullOrEmpty(UnitPriceEntry.Text))
                {
                    decimal QtyTotal = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                    UnitPriceEntry.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text));
                    ItemtotalLabel.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text) * QtyTotal);
                    saveButton.Focus();
                }
            }
        }
        private void BadQuantityEntry_Completed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(BadQuantityEntry.Text))
            {
                BadQuantityEntry.Text = "0";
                // UnitPriceEntry.Focus();
            }
            else
            {
                if (App.gDocType == "SO")
                {
                    if (ExchangeItemCheckbox.Checked)
                    {

                        decimal excTotalQty = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                        if (excTotalQty == 0)
                        {
                            //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                            UserDialogs.Instance.Alert("Quantity is not allow 0!", "Alert","OK");
                            //QuantityEntry.Focus();
                            return;
                        }
                        if (excTotalQty > MaxSoldQty)
                        {
                            UserDialogs.Instance.Alert("Quantity is more than maximum quantity = " + MaxSoldQty.ToString() + "!", "Alert", "OK");
                            return;
                        }
                    }

                }

                if (!string.IsNullOrEmpty(UnitPriceEntry.Text))
                {
                    decimal QtyTotal = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                    UnitPriceEntry.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text));
                    ItemtotalLabel.Text = string.Format("{0:#,##0.00}", decimal.Parse(UnitPriceEntry.Text) * QtyTotal);
                    saveButton.Focus();
                }
            }
        }
        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if(_isEnableSaveBtn)
            {
                _isEnableSaveBtn = false;
                if (string.IsNullOrEmpty(_ItemNo))
                {
                    //DependencyService.Get<IMessage>().LongAlert("Not allow blank Item!");
                    UserDialogs.Instance.Alert("Not allow blank Item!", "Alert","OK");
                    _isEnableSaveBtn = true;
                    //BarcodeEntry.Focus();
                    return;
                }

                if (!isItemExisted)
                {
                    UserDialogs.Instance.Alert("Wrong Item No or Item does not existed!", "Alert","OK");
                    _isEnableSaveBtn = true;
                    //BarcodeEntry.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(QuantityEntry.Text))
                {
                    //DependencyService.Get<IMessage>().LongAlert("Not allow blank quantity!");
                    UserDialogs.Instance.Alert("Not allow blank quantity!", "Alert","OK");
                    _isEnableSaveBtn = true;
                    // QuantityEntry.Focus();
                    return;
                }

                if (App.gDocType == "SO")
                {
                    if (FOCItemCheckbox.Checked)
                    {
                        _itemType = "FOC";

                        if (string.IsNullOrEmpty(GoodReasonCodeEntry.Text))
                        {
                            UserDialogs.Instance.Alert("Required Reason Code!", "Alert","OK");
                            _isEnableSaveBtn = true;
                            return;
                        }

                        if (decimal.Parse(QuantityEntry.Text) > MaxSoldQty)
                        {
                            UserDialogs.Instance.Alert("FOC quanity is more than  maximum quantity =" + MaxSoldQty, "Alert","OK");
                            _isEnableSaveBtn = true;
                            //QuantityEntry.Focus();
                            return;
                        }
                    }
                    else if (ExchangeItemCheckbox.Checked)
                    {
                        _itemType = "EXC";
                        decimal excTotalQty = decimal.Parse(QuantityEntry.Text) + decimal.Parse(BadQuantityEntry.Text);
                        if (excTotalQty == 0)
                        {
                            //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                            UserDialogs.Instance.Alert("Exchanged quantity is not allow 0 quantity!", "Alert","OK");
                            _isEnableSaveBtn = true;
                            //QuantityEntry.Focus();
                            return;
                        }

                        if (decimal.Parse(QuantityEntry.Text) > 0)
                        {
                            if (string.IsNullOrEmpty(GoodReasonCodeEntry.Text))
                            {
                                UserDialogs.Instance.Alert("Required Good Reason Code!", "Alert","OK");
                                _isEnableSaveBtn = true;
                                return;
                            }
                        }

                        if (decimal.Parse(BadQuantityEntry.Text) > 0)
                        {
                            if (string.IsNullOrEmpty(BadReasonCodeEntry.Text))
                            {
                                UserDialogs.Instance.Alert("Required Bad Reason Code!", "Alert","OK");
                                _isEnableSaveBtn = true;
                                return;
                            }
                        }

                        if (excTotalQty > MaxSoldQty)
                        {
                            UserDialogs.Instance.Alert("Quantity is more than maximum quantity " + MaxSoldQty, "Alert","OK");
                            _isEnableSaveBtn = true;
                            return;
                        }
                    }
                    else
                    {
                        _itemType = App.gDocType;
                    }

                    if (ExchangeItemCheckbox.Checked == false)
                    {
                        if (decimal.Parse(QuantityEntry.Text) == 0)
                        {
                            //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                            UserDialogs.Instance.Alert("Not allow 0 quantity!", "Alert","OK");
                            _isEnableSaveBtn = true;
                            //QuantityEntry.Focus();
                            return;
                        }

                        if (decimal.Parse(QuantityEntry.Text) > MaxSoldQty)
                        {
                            UserDialogs.Instance.Alert("Sold quanity is more than maximum quantity "+ MaxSoldQty, "Alert","OK");
                            _isEnableSaveBtn = true;
                            //QuantityEntry.Focus();
                            return;
                        }
                    }

                    if (FOCItemCheckbox.Checked == false && ExchangeItemCheckbox.Checked == false)
                    {

                        if (decimal.Parse(UnitPriceEntry.Text) == 0)
                        {
                            UserDialogs.Instance.Alert("Unit price should not be 0!", "Alert","OK");
                            _isEnableSaveBtn = true;
                            //UnitPriceEntry.Focus();
                            return;
                        }
                    }


                }
                else
                {
                    if (decimal.Parse(QuantityEntry.Text) == 0 && decimal.Parse(BadQuantityEntry.Text) == 0)
                    {
                        //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                        UserDialogs.Instance.Alert("Not allow 0 quantity!", "Alert","OK");
                        _isEnableSaveBtn = true;
                        //QuantityEntry.Focus();
                        return;
                    }

                    if (decimal.Parse(QuantityEntry.Text) > 0)
                    {

                        if (string.IsNullOrEmpty(GoodReasonCodeEntry.Text))
                        {
                            UserDialogs.Instance.Alert("Required Good Reason Code!", "Alert","OK");
                            _isEnableSaveBtn = true;
                            return;
                        }
                    }

                    if (decimal.Parse(BadQuantityEntry.Text) > 0)
                    {

                        if (string.IsNullOrEmpty(BadReasonCodeEntry.Text))
                        {
                            UserDialogs.Instance.Alert("Required Bad Reason Code!", "Alert","OK");
                            _isEnableSaveBtn = true;
                            return;
                        }
                    }
                }

                if (string.IsNullOrEmpty(BadQuantityEntry.Text))
                    BadQuantityEntry.Text = "0";

                if (string.IsNullOrEmpty(UnitPriceEntry.Text))
                {
                    UserDialogs.Instance.Alert("Not allow blank unit price!", "Alert","OK");
                    _isEnableSaveBtn = true;
                    //UnitPriceEntry.Focus();
                    return;
                }



                string retval = string.Empty;
                try
                {
                    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                    Task.Run(async () =>
                    {
                        DataManager manager = new DataManager();
                        Item item = new Item();
                        item = manager.GetSQLite_ItembyItemNo(_ItemNo);
                        decimal badQty = decimal.Parse(BadQuantityEntry.Text);
                        if (App.gDocType == "SO")
                        {
                            if (!ExchangeItemCheckbox.Checked)
                            {
                                badQty = 0;
                            }
                        }
                        SalesLine line = new SalesLine()
                        {
                            ID = EntryNo,
                            DocumentNo = DocNo,
                            ItemNo = _ItemNo,
                            Description = DescLabel.Text,
                            UnitofMeasurementCode = UomEntry.Text,
                            Quantity = decimal.Parse(QuantityEntry.Text),
                            BadQuantity = badQty,
                            FOCQty = 0,
                            UnitPrice = decimal.Parse(UnitPriceEntry.Text),
                            GoodReasonCode = GoodReasonCode,
                            BadReasonCode = BadReasonCode,
                            ItemType = _itemType,
                            BagNo = item.BarCode
                            //IsSync = "false",
                            //SyncDateTime = string.Empty
                        };
                        retval = manager.SaveSQLite_SalesLine(line);
                        if (retval == "Success")
                        {
                            SalesHeader head = new SalesHeader();
                            head = manager.GetSalesHeaderbyDocNo(DocNo);
                            decimal new_amt = (head.TotalAmount - oldLineAmt) + line.LineAmount;

                            ObservableCollection<SalesLine> lines = new ObservableCollection<SalesLine>();
                            lines = manager.GetSalesLinesbyDocNo(DocNo);
                            retval = manager.UpdateSalesHeaderTotalAmount(new_amt, DocNo, lines);


                            if (App.gDocType == "SO")
                            {
                                decimal soldQty = 0;
                                decimal excBadQty = 0;
                                if (ExchangeItemCheckbox.Checked)
                                {

                                    if (LastItemType != "EXC")
                                    {
                                        soldQty = item.SoldQty - LastQty + decimal.Parse(BadQuantityEntry.Text);
                                        excBadQty = item.BadQty + decimal.Parse(BadQuantityEntry.Text);
                                    }
                                    else
                                    {
                                        soldQty = item.SoldQty - LastBadQty + decimal.Parse(BadQuantityEntry.Text);
                                        excBadQty = item.BadQty - LastBadQty + decimal.Parse(BadQuantityEntry.Text);
                                    }

                                    manager.UpdateSQLite_SOInventory(_ItemNo, soldQty);
                                    manager.UpdateSQLite_ExchangeInventory(_ItemNo, excBadQty);
                                }
                                else
                                {
                                    if (LastItemType == "EXC")
                                    {
                                        soldQty = (item.SoldQty - LastBadQty) + decimal.Parse(QuantityEntry.Text);
                                        manager.UpdateSQLite_SOInventory(_ItemNo, soldQty);

                                        excBadQty = item.BadQty - LastBadQty;
                                        manager.UpdateSQLite_ExchangeInventory(_ItemNo, excBadQty);
                                    }
                                    else
                                    {
                                        soldQty = item.SoldQty - LastQty + decimal.Parse(QuantityEntry.Text);
                                        manager.UpdateSQLite_SOInventory(_ItemNo, soldQty);
                                    }
                                }
                                App.gIsExchange = false;
                            }
                            else
                            {
                                decimal returnQty = item.ReturnQty - LastQty + decimal.Parse(QuantityEntry.Text);
                                badQty = item.BadQty - LastBadQty + decimal.Parse(BadQuantityEntry.Text);
                                manager.UpdateSQLite_ReturnInventory(_ItemNo, returnQty, badQty);

                                VanItem vnitm = new VanItem();
                                vnitm = await manager.GetSQLite_VanItembyItemNo(_ItemNo);
                                decimal vanQty = 0;

                                if (vnitm != null)
                                {
                                    vanQty = vnitm.LoadQty - LastQty + decimal.Parse(QuantityEntry.Text);
                                    await manager.UpdateSQLite_VanItem(_ItemNo, vanQty);
                                }
                                else
                                {
                                    //Item itm = new Item();
                                    //itm= manager.GetSQLite_ItembyItemNo(_ItemNo);
                                    vanQty = decimal.Parse(QuantityEntry.Text);
                                    vnitm = new VanItem()
                                    {
                                        ID = 0,
                                        ItemNo = _ItemNo,
                                        Description = DescLabel.Text,
                                        BarCode = item.BarCode,
                                        BaseUOM = item.BaseUOM,
                                        UnitPrice = item.UnitPrice,
                                        Str64Img = item.Str64Img,
                                        LoadQty = vanQty,
                                        SoldQty = 0,
                                        ReturnQty = 0,
                                        BadQty = 0,
                                        UnloadQty = 0,
                                        Balance = 0
                                    };

                                    await manager.SaveSQLite_VanItem(vnitm);
                                }

                            }


                        }
                    }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        if (retval == "Success")
                            UserDialogs.Instance.ShowSuccess(retval,3000);
                        else
                            UserDialogs.Instance.Alert(retval, "Alert","OK");

                        Navigation.PopAsync();
                    }));
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert","OK");
                    _isEnableSaveBtn = true;
                }
            }
           
        }
        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            if(_isEnableDeleteBtn)
            {
                _isEnableDeleteBtn = false;
                var item = (Button)sender;
                string retval = string.Empty;
                DataManager manager = new DataManager();
                try
                {
                    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                    Task.Run(async () =>
                    {
                        var answer = await UserDialogs.Instance.ConfirmAsync("Are you sure to delete?", "Delete", "Yes", "No");
                        if (answer)
                        {
                            if (EntryNo != 0)
                            {
                                manager.DeleteSalesLinebyID(EntryNo);
                                SalesHeader head = new SalesHeader();
                                head = manager.GetSalesHeaderbyDocNo(DocNo);
                                decimal new_amt = (head.TotalAmount - oldLineAmt);
                                ObservableCollection<SalesLine> lines = new ObservableCollection<SalesLine>();
                                lines = manager.GetSalesLinesbyDocNo(DocNo);
                                retval = manager.UpdateSalesHeaderTotalAmount(new_amt, DocNo, lines);
                                Item iobj = new Item();
                                iobj = manager.GetSQLite_ItembyItemNo(_ItemNo);
                                if (App.gDocType == "SO")
                                {
                                    decimal soldQty = iobj.SoldQty - LastQty;

                                    if (_itemType == "EXC")
                                    {
                                        soldQty = iobj.SoldQty - LastBadQty;
                                        decimal excBadQty = iobj.BadQty - LastBadQty;
                                        manager.UpdateSQLite_ExchangeInventory(_ItemNo, excBadQty);
                                    }

                                    manager.UpdateSQLite_SOInventory(_ItemNo, soldQty);
                                }
                                else
                                {
                                    decimal returnQty = iobj.ReturnQty - LastQty;
                                    decimal badQty = iobj.BadQty - LastBadQty;
                                    manager.UpdateSQLite_ReturnInventory(_ItemNo, returnQty, badQty);

                                    VanItem vnitm = new VanItem();
                                    vnitm = await manager.GetSQLite_VanItembyItemNo(_ItemNo);
                                    decimal vanQty = 0;
                                    if (vnitm != null)
                                    {
                                        vanQty = vnitm.LoadQty - LastQty;
                                        await manager.UpdateSQLite_VanItem(_ItemNo, vanQty);
                                    }
                                }

                                //DependencyService.Get<IMessage>().LongAlert(retval);

                            }

                        }
                    }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        if (EntryNo != 0)
                        {
                            UserDialogs.Instance.ShowSuccess(retval, 3000);
                            Navigation.PopAsync();
                        }
                        else
                        {
                            UserDialogs.Instance.Alert("Can not delete new item!", "Alert","OK");
                            _isEnableDeleteBtn = true;
                        }
                    }));
                }
                catch (OperationCanceledException ex)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert","OK");
                    _isEnableDeleteBtn = true;
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert","OK");
                    _isEnableDeleteBtn = true;
                }
            }
            
        }
    }
}