using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Acr.UserDialogs;

namespace QHSalesApp
{
    public partial class EditQtyPage : PopupPage
    {
        private RequestLine ln { get; set; }
        private Item item { get; set; }
        private string ParentPage { get; set; }
        private string textValidate = string.Empty;
        public EditQtyPage(object obj,string page)
        {
            InitializeComponent();
            ParentPage = page;
            if(ParentPage=="Load")
            {
                ln = new RequestLine();
                ln = (RequestLine)obj;
            }
            else
            {
                item = new Item();
                item = (Item)obj;
            }
            QuantityEntry.Completed += QuantityEntry_Completed;
            QuantityEntry.Unfocused += QuantityEntry_Unfocused;
        }

        private void OnLoadValidated()
        {
            string textValidate = "Success";
            if (string.IsNullOrEmpty(QuantityEntry.Text))
            {
                textValidate = "Not allow blank quantity!";
            }

            //if (decimal.Parse(QuantityEntry.Text) == 0)
            //{
            //    textValidate="Not allow 0 quantity!";
            //    QuantityEntry.Text = ln.PickQty.ToString();
            //}

            if (decimal.Parse(QuantityEntry.Text) > ln.Quantity)
            {
                textValidate = "Pick quantity must not greater than requested quantity!";
                QuantityEntry.Text = ln.Quantity.ToString();
            }

            if (textValidate!= "Success")
            {
                UserDialogs.Instance.ShowError(textValidate, 3000);
                QuantityEntry.Focus();
                return;
            }
        }

        private void OnUnloadValidated()
        {
            string textValidate = "Success";
            if (string.IsNullOrEmpty(QuantityEntry.Text))
            {
                textValidate = "Not allow blank quantity!";
            }

            if (decimal.Parse(QuantityEntry.Text) > item.BalQty)
            {
                textValidate = "Unload quantity must not greater than balance quantity!";
                QuantityEntry.Text = item.BalQty.ToString();
            }

            if (decimal.Parse(QuantityEntry.Text) ==0)
            {
                textValidate = "Unload quantity must not 0!";
                QuantityEntry.Text = item.BalQty.ToString();
            }

            if (textValidate != "Success")
            {
                UserDialogs.Instance.ShowError(textValidate, 3000);
                QuantityEntry.Focus();
                return;
            }
        }
        private void QuantityEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (ParentPage == "Load")
                OnLoadValidated();
            else
                OnUnloadValidated();
        }

        private void QuantityEntry_Completed(object sender, EventArgs e)
        {
            if (ParentPage == "Load")
                OnLoadValidated();
            else
                OnUnloadValidated();
        }

        protected override  void OnAppearing()
        {
            base.OnAppearing();
            if (ParentPage == "Load")
                QuantityEntry.Text = ln.PickQty.ToString();
            else
                QuantityEntry.Text = item.BalQty.ToString();
        }

        private async void OnClose(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }
        private async void UpdateButtonOnClicked(object sender, EventArgs e)
        {
            try
            {
                string retval = string.Empty;
                DataManager manager = new DataManager();
                if (ParentPage == "Load")
                {
                    OnLoadValidated();
                    RequestLine line = new RequestLine()
                    {
                        ID = ln.ID,
                        EntryNo = ln.EntryNo,
                        HeaderEntryNo = ln.HeaderEntryNo,
                        ItemNo = ln.ItemNo,
                        ItemDesc = ln.ItemDesc,
                        QtyperBag = ln.QtyperBag,
                        NoofBags = ln.NoofBags,
                        Quantity = ln.Quantity,
                        PickQty = decimal.Parse(QuantityEntry.Text),
                        LoadQty = ln.LoadQty,
                        SoldQty = ln.SoldQty,
                        UnloadQty = ln.UnloadQty,
                        UomCode = ln.UomCode,
                        VendorNo = ln.VendorNo,
                        RequestNo = ln.RequestNo,
                        UserID = ln.UserID,
                        InHouse = ln.InHouse,
                        IsSync = ln.IsSync,
                        SyncDateTime = ln.SyncDateTime
                    };
                    retval = await manager.SaveSQLite_RequestLine(line);
                    if (retval == "Success")
                    {
                        MessagingCenter.Send<App>((App)Application.Current, "OnLoadData");
                        await PopupNavigation.PopAsync();
                    }
                   
                }
                else
                {
                    OnUnloadValidated();
                    var record = new Item
                    {
                        ID = item.ID,
                        EntryNo = item.EntryNo,
                        ItemNo = item.ItemNo,
                        Description = item.Description,
                        Description2 = item.Description2,
                        BaseUOM = item.BaseUOM,
                        Str64Img=item.Str64Img,
                        UnitPrice = item.UnitPrice,
                        CategoryCode = item.CategoryCode,
                        BarCode=item.BarCode,
                        InvQty = item.InvQty,
                        LoadQty = item.LoadQty,
                        SoldQty = item.SoldQty,
                        ReturnQty = item.ReturnQty,
                        BadQty=item.BadQty,
                        UnloadQty = decimal.Parse(QuantityEntry.Text)
                    };
                    retval = await manager.SaveSQLite_ItemInfo(record);
                    if (retval == "Success")
                    {
                        int id = 0;
                        ChangedItem itm = new ChangedItem();
                        itm = manager.GetSQLite_ChangedItembyItemNo(item.ItemNo);
                        if(itm!=null)
                        {
                            id = itm.ID;
                        }
                        var cObj = new ChangedItem
                        {
                            ID = id,
                            ItemNo = item.ItemNo,
                            Quantity = decimal.Parse(QuantityEntry.Text)
                        };
                        retval = await manager.SaveSQLite_ChangedItem(cObj);
                        MessagingCenter.Send<App>((App)Application.Current, "OnUnLoadData");
                        await PopupNavigation.PopAsync();
                    }
                }

                
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            
        }
        //protected override Task OnAppearingAnimationEndAsync()
        //{
        //    return Content.FadeTo(0.5);
        //}

        //protected override Task OnDisappearingAnimationBeginAsync()
        //{
        //    return Content.FadeTo(1);
        //}
    }
}
