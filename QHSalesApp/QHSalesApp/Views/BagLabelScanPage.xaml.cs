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
    public partial class BagLabelScanPage : ContentPage
    {
        private string Scanfrom { get; set; }
        private string HeadEntryNo { get; set; }
        ContainerInfo BagInfo { get; set; }
        ContainerInfo tmpInfo { get; set; }
        RequestLine ScanRequest { get; set; }
        private decimal NewLoadedQty { get; set; }
        private decimal NewUnloadedQty { get; set; }

        private string CanScanText { get; set; }
        public BagLabelScanPage(string scanf,string hdkey)
        {
            InitializeComponent();
            this.Title = "Scan Bag Label";
            HeadEntryNo = hdkey;
            Scanfrom = scanf;
            ScanRequest = new RequestLine();
            // ScannedItemButton.Clicked += ScannedItemButton_Clicked; ;
            if(scanf== "loaded")
            this.ToolbarItems.Add(new ToolbarItem { Text = "Loaded", Command = new Command(this.GotoScannedItemList) });

            BagLabelEntry.Focused += BagLabelEntry_Focused;
            BagLabelEntry.Completed += BagLabelEntry_Completed;
            BagLabelEntry.Unfocused += BagLabelEntry_Unfocused;

            QuantityEntry.Completed += QuantityEntry_Completed;
            QuantityEntry.Unfocused += QuantityEntry_Unfocused;
            ConfirmButton.Clicked += ConfirmButton_Clicked;
            ClearButton.Clicked += ClearButton_Clicked;

            
        }

        private string ValidateFields()
        {
            string retval = "Success";
            if (string.IsNullOrEmpty(QuantityEntry.Text))
            {
                //DependencyService.Get<IMessage>().LongAlert("Not allow blank quantity!");
                retval = "Not allow blank quantity!";
                QuantityEntry.Focus();
                return retval;
            }

            if (decimal.Parse(QuantityEntry.Text) == 0)
            {
                //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                //UserDialogs.Instance.ShowError("Not allow 0 quantity!", 3000);
                retval = "Not allow 0 quantity!";
                QuantityEntry.Focus();
                return retval;
            }

            if (Scanfrom == "loaded")
            {
                if (decimal.Parse(QuantityEntry.Text) > ScanRequest.PickQty)
                {
                    retval = "Loaded quantity is greater than picked quantity!";
                    return retval;
                }
            }
            else
            {
                decimal bagQty = BagInfo.LoadQty - BagInfo.SoldQty;
                if (decimal.Parse(QuantityEntry.Text) > bagQty)
                {
                    retval = "Unloaded quantity is greater than loaded quantity!";
                    return retval;
                }
            }
            return retval;
        }
        private void QuantityEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if(CanScanText!="Success")
            {
                UserDialogs.Instance.ShowError(CanScanText, 3000);
                return;
            }

            CanScanText = ValidateFields();
            if (CanScanText != "Success")
            {
                UserDialogs.Instance.ShowError(CanScanText, 3000);
                return;
            }
        }

        private void QuantityEntry_Completed(object sender, EventArgs e)
        {
            if (CanScanText != "Success")
            {
                UserDialogs.Instance.ShowError(CanScanText, 3000);
                return;
            }

            CanScanText = ValidateFields();
            if (CanScanText != "Success")
            {
                UserDialogs.Instance.ShowError(CanScanText, 3000);
                return;
            }
        }

        private void ClearButton_Clicked(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Title = "Label Scan",
                    Message = "Are you sure to clear?",
                    CancelText = "No",
                    OkText = "Yes"
                });
                //  var answer = await DisplayAlert("Logout", "Are you sure to logout?", "Yes", "No");
                if (result)
                {
                    //DataManager manager = new DataManager();
                    //BagInfo = new ContainerInfo();
                    //BagInfo = await manager.GetContainerInfobyBagLabel(BagLabelEntry.Text);
                    //ScanRequest = await manager.GetRequestLinebyEntryNo(BagInfo.MobileEntryNo);
                    //RequestLine line = new RequestLine()
                    //{
                    //    ID = ScanRequest.ID,
                    //    EntryNo = ScanRequest.EntryNo,
                    //    HeaderEntryNo = ScanRequest.HeaderEntryNo,
                    //    ItemNo = ScanRequest.ItemNo,
                    //    ItemDesc = ScanRequest.ItemDesc,
                    //    QtyperBag = ScanRequest.QtyperBag,
                    //    NoofBags = ScanRequest.NoofBags,
                    //    Quantity = ScanRequest.Quantity,
                    //    PickQty = ScanRequest.PickQty,
                    //    LoadQty = NewLoadedQty,
                    //    SoldQty = ScanRequest.SoldQty,
                    //    UnloadQty = 0,
                    //    UomCode = ScanRequest.UomCode,
                    //    VendorNo = ScanRequest.VendorNo,
                    //    RequestNo = ScanRequest.RequestNo,
                    //    UserID = App.gSalesPersonCode,
                    //    InHouse = ScanRequest.InHouse,
                    //    IsSync = ScanRequest.IsSync,
                    //    SyncDateTime = string.Empty
                    //};

                    //string retval = await manager.SaveSQLite_RequestLine(line);

                    //Item item = new Item();
                    //item = await manager.GetSQLite_ItembyItemNo(ScanRequest.ItemNo);

                    //var record = new Item
                    //{
                    //    ID = item.ID,
                    //    EntryNo = item.EntryNo,
                    //    ItemNo = item.ItemNo,
                    //    Description = item.Description,
                    //    Description2 = item.Description2,
                    //    BaseUOM = item.BaseUOM,
                    //    UnitPrice = item.UnitPrice,
                    //    CategoryCode = item.CategoryCode,
                    //    InvQty = item.InvQty,
                    //    LoadQty = item.LoadQty,
                    //    SoldQty = item.SoldQty,
                    //    ReturnQty = item.ReturnQty,
                    //    UnloadQty = 0
                    //};
                    //manager.SaveSQLite_ItemInfo(record);
                    //manager.DeleteUnload();
                    BagLabelEntry.Text = string.Empty;
                    ItemNoEntry.Text = string.Empty;
                    QuantityEntry.Text = string.Empty;
                }
            });
        }

        private void BagLabelEntry_Focused(object sender, FocusEventArgs e)
        {
            //UserDialogs.Instance.ShowSuccess("Bag Label focus", 3000);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BagLabelEntry.Focus();
            CanScanText = "Scan not yet!";
        }
        private void BagLabelEntry_Unfocused(object sender, FocusEventArgs e)
        {
            
        }

        private async void BagLabelEntry_Completed(object sender, EventArgs e)
        {
            try
            {
                DataManager manager = new DataManager();
                BagInfo = new ContainerInfo();
                BagInfo = await manager.GetContainerInfobyBagLabel(BagLabelEntry.Text);

                if (BagInfo != null)
                {
                    manager = new DataManager();
                    ScanRequest = await manager.GetRequestLinebyEntryNo(BagInfo.MobileEntryNo);
                     NewLoadedQty = ScanRequest.LoadQty;
                     NewUnloadedQty = ScanRequest.UnloadQty;
                    ItemNoEntry.Text = BagInfo.ItemNo;
                    if (Scanfrom == "loaded")
                    {
                        QuantityEntry.Text = BagInfo.Quantity.ToString();
                        ScannedLoadDoc loadscan = new ScannedLoadDoc();
                        loadscan = await manager.GetSQLite_ScannedLoadDoc(BagLabelEntry.Text);
                        if (loadscan!=null)
                        {
                            CanScanText = "Already Scanned for this item!";
                            UserDialogs.Instance.ShowError(CanScanText, 3000);
                            
                            return;
                        }
                    }
                    else
                    {

                        if((BagInfo.LoadQty - BagInfo.SoldQty) == 0)
                        {
                            CanScanText = "No remaining quantity!";
                            BagLabelEntry.Text = string.Empty;
                            ItemNoEntry.Text = string.Empty;
                            QuantityEntry.Text = string.Empty;
                            UserDialogs.Instance.ShowError(CanScanText, 3000);
                            return;
                        }
                        QuantityEntry.Text = (BagInfo.LoadQty - BagInfo.SoldQty).ToString();
                        ScannedUnloadDoc unloadscan = new ScannedUnloadDoc();
                        unloadscan = await manager.GetSQLite_ScannedUnloadDocByBagNo(BagLabelEntry.Text);
                        if (unloadscan != null)
                        {
                            CanScanText = "Already scanned for this item!";
                            UserDialogs.Instance.ShowError(CanScanText, 3000);
                            return;
                        }
                    }
                    CanScanText = "Success";
                }
                else
                {
                    CanScanText = "Item not found!";
                    UserDialogs.Instance.ShowError(CanScanText, 3000);
                    ItemNoEntry.Text = string.Empty;
                    QuantityEntry.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                CanScanText = ex.Message.ToString();
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }
        
        private async void  ConfirmButton_Clicked(object sender, EventArgs e)
        {
            
            try
            {
                if(CanScanText!="Success")
                {
                    UserDialogs.Instance.ShowError(CanScanText, 3000);
                    return;
                }

                decimal actualLoadQty = 0;
                decimal actualUnloadQty = 0;
                if (string.IsNullOrEmpty(QuantityEntry.Text))
                {
                    UserDialogs.Instance.ShowError("Not allow blank quantity", 3000);
                    return;
                }
                else
                {
                    decimal enterQty = decimal.Parse(QuantityEntry.Text);
                    if (Scanfrom == "loaded")
                    {
                        NewLoadedQty = ScanRequest.LoadQty + enterQty;

                    }
                    else
                    {
                        NewUnloadedQty = ScanRequest.UnloadQty + enterQty;
                        
                    }
                    DataManager manager = new DataManager();
                    RequestLine line = new RequestLine()
                    {
                        ID = ScanRequest.ID,
                        EntryNo = ScanRequest.EntryNo,
                        HeaderEntryNo = ScanRequest.HeaderEntryNo,
                        ItemNo = ScanRequest.ItemNo,
                        ItemDesc = ScanRequest.ItemDesc,
                        QtyperBag = ScanRequest.QtyperBag,
                        NoofBags = ScanRequest.NoofBags,
                        Quantity = ScanRequest.Quantity,
                        PickQty = ScanRequest.PickQty,
                        LoadQty = NewLoadedQty,
                        SoldQty = ScanRequest.SoldQty,
                        UnloadQty = NewUnloadedQty,
                        UomCode = ScanRequest.UomCode,
                        VendorNo = ScanRequest.VendorNo,
                        RequestNo = ScanRequest.RequestNo,
                        UserID = App.gSalesPersonCode,
                        InHouse = ScanRequest.InHouse,
                        IsSync = ScanRequest.IsSync,
                        SyncDateTime = string.Empty
                    };

                    string retval = await manager.SaveSQLite_RequestLine(line);
                    if (retval == "Success")
                    {
                        if (Scanfrom == "loaded")
                        {
                            await manager.SaveSQLite_ScannedLoadDoc(BagInfo.BoxNo, BagInfo.RefDocNo, BagInfo.MobileEntryNo, enterQty,BagInfo.ItemNo);
                            actualLoadQty = enterQty;
                            actualUnloadQty = BagInfo.UnloadQty;
                        }
                        else
                        {
                            await manager.SaveSQLite_ScannedUnloadDoc(BagInfo.BoxNo, BagInfo.RefDocNo, BagInfo.MobileEntryNo, BagInfo.Quantity);
                            actualLoadQty = BagInfo.LoadQty;
                            actualUnloadQty = enterQty;
                        }

                        HeadEntryNo = BagInfo.RefDocNo;

                        tmpInfo = new ContainerInfo
                        {
                            ID = BagInfo.ID,
                            EntryNo = BagInfo.EntryNo,
                            PalletNo = BagInfo.PalletNo,
                            CartonNo = BagInfo.CartonNo,
                            BoxNo = BagInfo.BoxNo,
                            LineNo = BagInfo.LineNo,
                            ItemNo = BagInfo.ItemNo,
                            VariantCode = BagInfo.VariantCode,
                            Quantity = BagInfo.Quantity,
                            LoadQty = actualLoadQty,
                            SoldQty = BagInfo.SoldQty,
                            UnloadQty = actualUnloadQty,
                            LocationCode = BagInfo.LocationCode,
                            BinCode = BagInfo.BinCode,
                            RefDocNo = BagInfo.RefDocNo,
                            RefDocLineNo = BagInfo.RefDocLineNo,
                            RefDocType = BagInfo.RefDocType,
                            MobileEntryNo = BagInfo.MobileEntryNo
                        };

                        retval=await manager.UpdateSQLite_ContainerInfo(tmpInfo);
                        if (Scanfrom != "loaded")
                        {
                            decimal loadedqty = await manager.GetSQLite_SumLoadedItems(tmpInfo.ItemNo);
                             manager.UpdateSQLite_Inventory(tmpInfo.ItemNo,loadedqty, 0, 0,0, enterQty);
                        }
                            
                        //
                        ObservableCollection<ContainerInfo> lstInfo = new ObservableCollection<ContainerInfo>();
                        lstInfo = await manager.GetSQLite_ContainerInfobyRequestLineNo(BagInfo.MobileEntryNo);

                        if (retval == "Success")
                        {
                            UserDialogs.Instance.ShowSuccess("Scan success!", 3000);
                            BagLabelEntry.Text = string.Empty;
                            ItemNoEntry.Text = string.Empty;
                            QuantityEntry.Text = string.Empty;
                        }
                        else
                        {
                            UserDialogs.Instance.ShowError(retval, 3000);
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.ShowError(retval, 3000);
                    }
                }
                
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }

        private void GotoScannedItemList()
        {
            if (Scanfrom == "loaded")
            {
                App.gPageTitle = "Loaded Items";
                Navigation.PushAsync(new LoadItemPage(HeadEntryNo));
            }               
            else
                Navigation.PushAsync(new UnloadVanPage());
        }
    }
}