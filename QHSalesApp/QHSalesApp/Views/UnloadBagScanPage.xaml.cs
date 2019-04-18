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
    public partial class UnloadBagScanPage : ContentPage
    {

        ContainerInfo BagInfo { get; set; }
        ContainerInfo tmpInfo { get; set; }
        RequestLine ScanRequest { get; set; }

        UnloadReturn item { get; set; }

        string ItemNo { get; set; }
        private string CanScanText { get; set; }
        public UnloadBagScanPage(string itemno)
        {
            InitializeComponent();
            ItemNo = itemno;
            if (!string.IsNullOrEmpty(ItemNo))
                this.Title = "Scan Bag Label";
            else
                this.Title = "Add Item";
            ScanRequest = new RequestLine();
            item = new UnloadReturn();
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
                retval = "Not allow 0 quantity!";
                QuantityEntry.Focus();
                return retval;
            }

            if(string.IsNullOrEmpty(ItemNo))
            {
                //decimal bagQty = BagInfo.LoadQty - BagInfo.SoldQty;
                if (decimal.Parse(QuantityEntry.Text) > BagInfo.Quantity)
                {
                    retval = " Quantity is greater than scanned bag quantity!";
                    return retval;
                }
            }
            return retval;
        }
        private void QuantityEntry_Unfocused(object sender, FocusEventArgs e)
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
                if (result)
                {
                    BagLabelEntry.Text = string.Empty;
                    ItemNoEntry.Text = string.Empty;
                    QuantityEntry.Text = string.Empty;
                    FromBinEntry.Text = string.Empty;
                    ToBinEntry.Text = string.Empty;
                }
            });
        }

        private void BagLabelEntry_Focused(object sender, FocusEventArgs e)
        {
            //UserDialogs.Instance.ShowSuccess("Bag Label focus", 3000);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if(!string.IsNullOrEmpty(ItemNo))
            {
                BagLabelEntry.IsVisible = false;
               LoadItems();
            }
            else
            {
                BagLabelEntry.Focus();
                CanScanText = "Scan not yet!";
            }
            

        }
        private void BagLabelEntry_Unfocused(object sender, FocusEventArgs e)
        {

        }

        private async void LoadItems()
        {
            try
            {
                DataManager manager = new DataManager();
                item = await manager.GetSQLite_UnloadReturnbyItemNo(ItemNo);
                ItemNoEntry.Text = ItemNo;
                QuantityEntry.Text = item.Quantity.ToString();
                FromBinEntry.Text = item.FromBin;
               // ToBinEntry.Text = BagInfo.BinCode;
                CanScanText = "Success";
            }
            catch (Exception ex)
            {

                CanScanText = ex.Message.ToString();
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
          
        }
        private async void BagLabelEntry_Completed(object sender, EventArgs e)
        {
            try
            {
                DataManager manager = new DataManager();

                ScannedUnloadReturnDoc scanDoc = new ScannedUnloadReturnDoc();
                scanDoc = await manager.GetSQLite_ScannedReturnUnloadDocByBagNo(BagLabelEntry.Text);
                if (scanDoc != null)
                {
                    CanScanText = "Already scanned for this item!";
                    UserDialogs.Instance.ShowError(CanScanText, 3000);
                    return;
                }

                BagInfo = new ContainerInfo();
                BagInfo = await manager.GetContainerInfobyBagLabelDirect(BagLabelEntry.Text);

                if (BagInfo != null)
                {
                    
                    manager = new DataManager();
                    item = await manager.GetSQLite_UnloadReturnbyItemNo(BagInfo.ItemNo);
                    if(item!=null)
                    {
                        ItemNoEntry.Text = BagInfo.ItemNo;
                        QuantityEntry.Text = BagInfo.Quantity.ToString();
                        FromBinEntry.Text = item.FromBin;
                        ToBinEntry.Text = BagInfo.BinCode;
                        CanScanText = "Success";
                    }
                    else
                    {
                        CanScanText = "Item not found in Unload Return!";
                        UserDialogs.Instance.ShowError(CanScanText, 3000);
                        return;
                    }
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

        private async void ConfirmButton_Clicked(object sender, EventArgs e)
        {

            try
            {
                if (CanScanText != "Success")
                {
                    UserDialogs.Instance.ShowError(CanScanText, 3000);
                    return;
                }

                if (string.IsNullOrEmpty(ToBinEntry.Text))
                {
                    UserDialogs.Instance.ShowError("To Bin Code can not be blank", 3000);
                    return;
                }

                if (string.IsNullOrEmpty(QuantityEntry.Text))
                {
                    UserDialogs.Instance.ShowError("Not allow blank quantity", 3000);
                    return;
                }
                else
                {
                    string retval = string.Empty;
                    
                    DataManager manager = new DataManager();
                    if (item!=null)
                    {
                        decimal QSRetQty=item.QSReturnQty+ decimal.Parse(QuantityEntry.Text); // Need to confirm QS Return formula

                        retval = await manager.UpdateSQLite_UnloadReturn(item.ItemNo,QSRetQty, ToBinEntry.Text);
                        if (retval == "Success")
                        {
                            if(string.IsNullOrEmpty(ItemNo))
                                await manager.SaveSQLite_ScannedUnloadReturnDoc(BagInfo.BoxNo, BagInfo.RefDocNo, BagInfo.MobileEntryNo, BagInfo.Quantity,BagInfo.ItemNo,ToBinEntry.Text);
                            //   await manager.UpdateSQLite_Inventory(tmpInfo.ItemNo, 0, 0, 0, enterQty);
                            //
                            //ObservableCollection<ContainerInfo> lstInfo = new ObservableCollection<ContainerInfo>();
                            //lstInfo = await manager.GetSQLite_ContainerInfobyRequestLineNo(BagInfo.MobileEntryNo);

                            if (retval == "Success")
                            {
                                UserDialogs.Instance.ShowSuccess("Scan success!", 3000);
                                BagLabelEntry.Text = string.Empty;
                                ItemNoEntry.Text = string.Empty;
                                QuantityEntry.Text = string.Empty;
                                FromBinEntry.Text = string.Empty;
                                ToBinEntry.Text = string.Empty;
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
                    else
                    {
                        UserDialogs.Instance.ShowError("No scanned Item!", 3000);
                    }
                    
                }

            }
            catch (Exception ex)
            {

                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }
    }
}
