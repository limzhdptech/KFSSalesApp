using Acr.UserDialogs;
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
    public partial class RequestLnEntryPage : ContentPage
    {
        readonly Database database;
        private RequestLine data { get; set; }
        private int LineID { get; set; }
        private string EntryNo { get; set; }
        private string HeaderNo { get; set; }
        private int cmdPara { get; set; }
        private decimal oldLineAmt { get; set; }
        private ObservableCollection<RequestLine> records { get; set; }
        private bool isItemExisted { get; set; }
        private bool IsBack { get; set; }
        private string VendorNo { get; set; }
        private string _RequestNo { get; set; }
        private bool InHouse { get; set; }

        private string _ItemNo { get; set; }

        private bool _isEnableSaveBtn { get; set; }
        private bool _isEnableDeleteBtn { get; set; }
        private bool _isEnableItemBtn { get; set; }
        public RequestLnEntryPage(int id,string hdno,string requestno)
        {
            InitializeComponent();
            Title = App.gPageTitle;
            database = new Database(Constants.DatabaseName);
            database.CreateTable<RequestLine>();

            LineID = id;
            HeaderNo = hdno;
            _RequestNo = requestno;
            InHouse = true;
            BarCodeEntry.Completed += ItemNoEntry_Completed;
            //BarCodeEntry.Unfocused += ItemNoEntry_Unfocused;
            ItemNolookUpButton.ButtonIcon = AwasomeIcon.FASearch;
            ItemNolookUpButton.ButtonFontSize = 16;
            ItemNolookUpButton.ButtonColor = Color.FromHex("#EC2029");
            ItemNolookUpButton.OnTouchesEnded += ItemlookUpButton_OnTouchesEnded;

            //VendorlookUpButton.ButtonIcon = AwasomeIcon.FASearch;
            //VendorlookUpButton.ButtonColor = Color.FromHex("#EC2029");
            //VendorlookUpButton.OnTouchesEnded += VendorlookUpButton_OnTouchesEnded;

            QtyEntry.Completed += QtyperBagEntry_Completed;
            QtyEntry.Unfocused += QtyperBagEntry_Unfocused;

            //NoofBagsEntry.Completed += NoofBagsEntry_Completed;
            //NoofBagsEntry.Unfocused += NoofBagsEntry_Unfocused; 

            this.BackgroundColor = Color.FromHex("#dddddd");
            saveButton.Clicked += SaveButton_Clicked;
            DeleteButton.Clicked += DeleteButton_Clicked;
            IsBack = false;
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _isEnableSaveBtn = true;
            _isEnableDeleteBtn = true;
            _isEnableItemBtn = true;
            data = new RequestLine();
            try
            {
                if(!IsBack)
                {
                    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                    Task.Run(async () =>
                    {
                        if (LineID != 0)
                        {
                            DataManager manager = new DataManager();
                            data = await manager.GetRequestItemLinebyID(LineID);
                        }
                        else
                            data = null;
                    }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        DisplayData();
                        BarCodeEntry.Focus();
                    }));
                }
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

        void DisplayData()
        {
            if (LineID != 0)
            {
                if (data != null)
                {
                    LineID = data.ID;
                    EntryNo = data.EntryNo;
                    HeaderNo = data.HeaderEntryNo;
                    //DocumentNoEntry.Text = dt.Rows[0]["DocumentNo"].ToString();
                    ItemNoLabel.Text = data.ItemNo;
                    _ItemNo = data.ItemNo;
                    DescEntry.Text = data.ItemDesc;
                    UomEntry.Text = data.UomCode;
                    QtyEntry.Text = data.Quantity.ToString();
                   // NoofBagsEntry.Text = data.NoofBags.ToString();
                   // QuantityLabel.Text = data.Quantity.ToString(); //string.Format("{0:0.00}", data.LineAmount);
                   // VendorEntry.Text = data.VendorNo;
                    InHouse = data.InHouse;
                    isItemExisted = true;
                }
            }
            else
            {
                LineID = 0;
                EntryNo = Guid.NewGuid().ToString();
                HeaderNo = HeaderNo;
                //DocumentNoEntry.Text = dt.Rows[0]["DocumentNo"].ToString();
                ItemNoLabel.Text = "Scan or select Item";
                _ItemNo = string.Empty;
                DescEntry.Text = string.Empty;
                UomEntry.Text = string.Empty;
                QtyEntry.Text = "1";
               // NoofBagsEntry.Text = "1";
               // QuantityLabel.Text = "1"; //string.Format("{0:0.00}", data.LineAmount);
               // VendorEntry.Text = string.Empty;
                InHouse = true;
                isItemExisted = false;
            }
        }

        private void NoofBagsEntry_Unfocused(object sender, FocusEventArgs e)
        {
            //if (string.IsNullOrEmpty(QtyperBagEntry.Text))
            //{
            //    UserDialogs.Instance.ShowError("Not allow blank quantity!", 3000);
            //    QtyperBagEntry.Focus();
            //    return;
            //}

            //if (!string.IsNullOrEmpty(NoofBagsEntry.Text))
            //{
            //    QuantityLabel.Text = (decimal.Parse(QtyperBagEntry.Text) * decimal.Parse(NoofBagsEntry.Text)).ToString();
            //    saveButton.Focus();
            //}
            //else
            //{
            //    UserDialogs.Instance.ShowError("Empty No of Bags!", 3000);
            //    NoofBagsEntry.Focus();
            //}
        }

        private void NoofBagsEntry_Completed(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(QtyperBagEntry.Text))
            //{
            //    UserDialogs.Instance.ShowError("Not allow blank quantity!", 3000);
            //    QtyperBagEntry.Focus();
            //    return;
            //}

            //if (!string.IsNullOrEmpty(NoofBagsEntry.Text))
            //{
            //    QuantityLabel.Text = (decimal.Parse(QtyperBagEntry.Text) * decimal.Parse(NoofBagsEntry.Text)).ToString();
            //    saveButton.Focus();
            //}
            //else
            //{
            //    UserDialogs.Instance.ShowError("Empty No of Bags!", 3000);
            //    NoofBagsEntry.Focus();
            //}
        }

        private void QtyperBagEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(QtyEntry.Text))
            {
                UserDialogs.Instance.ShowError("Not allow blank quantity!", 3000);
                QtyEntry.Focus();
                return;
            }

            //if (!string.IsNullOrEmpty(NoofBagsEntry.Text))
            //{
            //    QuantityLabel.Text = (decimal.Parse(QtyperBagEntry.Text) * decimal.Parse(NoofBagsEntry.Text)).ToString();
            //    NoofBagsEntry.Focus();
            //}
            //else
            //{
            //    UserDialogs.Instance.ShowError("Empty No of Bags!", 3000);
            //    NoofBagsEntry.Focus();
            //}
        }

        private void QtyperBagEntry_Completed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(QtyEntry.Text))
            {
                UserDialogs.Instance.ShowError("Not allow blank quantity!", 3000);
                QtyEntry.Focus();
                return;
            }

            //if (!string.IsNullOrEmpty(NoofBagsEntry.Text))
            //{
            //    QuantityLabel.Text = (decimal.Parse(QtyperBagEntry.Text) * decimal.Parse(NoofBagsEntry.Text)).ToString();
            //    NoofBagsEntry.Focus();
            //}
            //else
            //{
            //    UserDialogs.Instance.ShowError("Empty No of Bags!", 3000);
            //    NoofBagsEntry.Focus();
            //}
        }

        private void VendorlookUpButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            var page = new VendorPage();
            var obj = (ActionButton)sender;

            page.listview.ItemSelected += Listview_ItemSelected;
            Navigation.PushAsync(page);
        }

        private void Listview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            IsBack = true;
            var selectedItem = e.SelectedItem as Vendor;

           // VendorEntry.Text = selectedItem.VendorNo;
           // CustomerNameLabel.Text = selectedItem.Name;
            Navigation.PopAsync();
        }

        private  void ItemNoEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (!string.IsNullOrEmpty(BarCodeEntry.Text))
            {
                DataManager manager = new DataManager();
                Item item = new Item();
                item =  manager.GetSQLite_ItembyItemNo(BarCodeEntry.Text);
                if (item != null)
                {
                    ItemNoLabel.Text = item.ItemNo;
                    _ItemNo = item.ItemNo;
                    DescEntry.Text = item.Description;
                    UomEntry.Text = item.BaseUOM;
                    isItemExisted = true;
                    QtyEntry.Focus();
                }
                else
                {
                    ItemNoLabel.Text ="Scan or select Item";
                    _ItemNo = string.Empty;
                    DescEntry.Text = string.Empty;
                    isItemExisted = false;
                    UserDialogs.Instance.ShowError("Wrong Item No or Item does not existed!", 3000);
                }
            }
        }

        private void ItemNoEntry_Completed(object sender, EventArgs e)
        {
            try
            {
                DataManager manager = new DataManager();
                Item item = new Item();
                if (string.IsNullOrEmpty(BarCodeEntry.Text))
                {
                    return;
                }

                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {
                    item = await manager.GetSQLite_ItembyBarCode(BarCodeEntry.Text);

                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    if (item != null)
                    {
                        ItemNoLabel.Text = item.ItemNo;
                        _ItemNo = item.ItemNo;
                        DescEntry.Text = item.Description;
                        UomEntry.Text = item.BaseUOM;
                        isItemExisted = true;
                        BarCodeEntry.Unfocus();
                    }
                    else
                    {
                        UserDialogs.Instance.ShowError("Wrong Item No or Item does not existed!", 3000);
                        BarCodeEntry.Text = string.Empty;
                        ItemNoLabel.Text = "Scan  barcode or select Item No";
                        _ItemNo = string.Empty;
                        DescEntry.Text = string.Empty;
                        isItemExisted = false;
                        //BarCodeEntry.Focus();
                    }
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

        private void ItemlookUpButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            if(_isEnableItemBtn)
            {
                _isEnableItemBtn = false;
                var obj = (ActionButton)sender;
                cmdPara = int.Parse(obj.CommandParameter.ToString());
                var page = new ItemsPage();
                page.listview.ItemSelected += ItemListview_ItemSelected;
                Navigation.PushAsync(page);
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
            DescEntry.Text = selectedItem.Description;
            UomEntry.Text = selectedItem.BaseUOM;
            isItemExisted = true;
            //UnitPricePicker.IsEnabled = true;
            Navigation.PopAsync();
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            if(_isEnableDeleteBtn)
            {
                _isEnableDeleteBtn = false;
                var item = (Button)sender;

                var answer = await UserDialogs.Instance.ConfirmAsync("Are you sure to delete?", "Delete", "Yes", "No");
                // var answer = await DisplayAlert("Delete Sales Order", "Are you sure to delete?", "Yes", "No");
                if (answer)
                {
                    DataManager manager = new DataManager();
                    manager.DeleteSingleRequestLine(EntryNo);
                    UserDialogs.Instance.ShowSuccess("Deleted!", 3000);
                    Navigation.PopAsync();
                }
            }
           
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if(_isEnableSaveBtn)
            {
                _isEnableSaveBtn = false;
                try
                {
                    if (string.IsNullOrEmpty(_ItemNo))
                    {
                        //DependencyService.Get<IMessage>().LongAlert("Not allow blank Item!");
                        UserDialogs.Instance.ShowError("Not allow blank Item!", 3000);
                        BarCodeEntry.Focus();
                        return;
                    }

                    if (!isItemExisted)
                    {
                        UserDialogs.Instance.ShowError("Wrong Item No or Item does not existed!", 3000);
                        BarCodeEntry.Focus();
                        return;
                    }

                    if (string.IsNullOrEmpty(QtyEntry.Text))
                    {
                        //DependencyService.Get<IMessage>().LongAlert("Not allow blank quantity!");
                        UserDialogs.Instance.ShowError("Not allow blank quantity!", 3000);
                        QtyEntry.Focus();
                        return;
                    }

                    if (decimal.Parse(QtyEntry.Text) == 0)
                    {
                        //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                        UserDialogs.Instance.ShowError("Not allow 0 quantity!", 3000);
                        QtyEntry.Focus();
                        return;
                    }

                    //if (string.IsNullOrEmpty(NoofBagsEntry.Text))
                    //{
                    //    //DependencyService.Get<IMessage>().LongAlert("Not allow blank quantity!");
                    //    UserDialogs.Instance.ShowError("Not allow blank unit price!", 3000);
                    //    NoofBagsEntry.Focus();
                    //    return;
                    //}

                    //if (decimal.Parse(NoofBagsEntry.Text) == 0)
                    //{
                    //    //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                    //    UserDialogs.Instance.ShowError("Not allow unit price is 0!", 3000);
                    //    NoofBagsEntry.Focus();
                    //    return;
                    //}

                    // if (!string.IsNullOrEmpty(VendorEntry.Text)) InHouse = false;
                    DataManager manager = new DataManager();
                    RequestLine line = new RequestLine()
                    {
                        ID = LineID,
                        EntryNo = EntryNo,
                        HeaderEntryNo = HeaderNo,
                        ItemNo = _ItemNo,
                        ItemDesc = DescEntry.Text,
                        QtyperBag = 0,
                        // NoofBags= decimal.Parse(NoofBagsEntry.Text),
                        Quantity = decimal.Parse(QtyEntry.Text),
                        PickQty = decimal.Parse(QtyEntry.Text),
                        LoadQty = 0,
                        UomCode = UomEntry.Text,
                        VendorNo = string.Empty,//VendorEntry.Text,
                        RequestNo = _RequestNo,
                        UserID = App.gSalesPersonCode,
                        InHouse = InHouse,
                        IsSync = "request",
                        SyncDateTime = string.Empty
                    };
                    string retval = await manager.SaveSQLite_RequestLine(line);
                    if (retval == "Success")
                    {
                        UserDialogs.Instance.ShowSuccess(retval, 3000);
                        Navigation.PopAsync();
                    }
                    else
                    {
                        //DependencyService.Get<IMessage>().LongAlert(retval);
                        UserDialogs.Instance.ShowError(retval, 3000);
                        _isEnableSaveBtn = true;
                    }
                }
                catch (Exception ex)
                {

                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    _isEnableSaveBtn = true;
                }
            }
        }
    }
}