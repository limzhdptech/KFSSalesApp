using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QHSalesApp
{
    public partial class CRQtyEntryPage : ContentPage
    {
        private string cmdPara { get; set; }
        private string CanScanText { get; set; }
        Item item { get; set; }
        public CRQtyEntryPage(string itemno)
        {
            InitializeComponent();
            this.Title = "Add Item Quantity";
            item = new Item();
            cmdPara = itemno;

            ConfirmButton.Clicked += ConfirmButton_Clicked;
        }

        protected override  void OnAppearing()
        {
            base.OnAppearing();
            DataManager manager = new DataManager();
            item = manager.GetSQLite_ItembyItemNo(cmdPara);
            if(item!=null)
            {
                ItemNoEntry.Text = item.ItemNo;
                DescEntry.Text = item.Description;
                QuantityEntry.Text = item.ReturnQty.ToString();
            }
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

            if (decimal.Parse(QuantityEntry.Text) > item.ReturnQty)
            {
                retval = "Quantity is greater than credit memo quantity!";
                return retval;
            }
            return retval;
        }
        private async void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            string retval = string.Empty;
            try
            {
                DataManager manager = new DataManager();
                CanScanText = ValidateFields();
                if (CanScanText != "Success")
                {
                    UserDialogs.Instance.ShowError(CanScanText, 3000);
                    return;
                }

                decimal enterQty = decimal.Parse(QuantityEntry.Text);
                decimal loadedqty = await manager.GetSQLite_SumLoadedItems(ItemNoEntry.Text);
                retval  = manager.UpdateSQLite_Inventory(ItemNoEntry.Text,loadedqty, 0, 0,0, enterQty);

                if (retval == "Success")
                {
                    UserDialogs.Instance.ShowSuccess(" Success!", 3000);
                    //ItemNoEntry.Text = string.Empty;
                    //DescEntry.Text = string.Empty;
                    //QuantityEntry.Text = string.Empty;
                    Navigation.PopAsync();
                }
                else
                {
                    UserDialogs.Instance.ShowError(retval, 3000);
                }

            }
            catch (Exception ex)
            {

                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }
    }
}
