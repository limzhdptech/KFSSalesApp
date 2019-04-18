using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadQtyEntryPage : ContentPage
    {
        readonly Database database;
        private RequestLine data { get; set; }
        private int ItemId { get; set; }
        private string EntryNo { get; set; }
        private string HeaderNo { get; set; }
        private string VendorNo { get; set; }
        private string _RequestNo { get; set; }
        private string UomCode { get; set; }
        private bool InHouse { get; set; }

        public LoadQtyEntryPage(int itemId)
        {
            InitializeComponent();
            this.Title = "Enter Loaded Quantity";
            ItemId = itemId;
            database = new Database(Constants.DatabaseName);
            database.CreateTable<RequestLine>();
            this.BackgroundColor = Color.FromHex("#dddddd");
            saveButton.Clicked += SaveButton_Clicked;
            LoadQtyEntry.Completed += LoadQtyEntry_Completed;
            LoadQtyEntry.Unfocused += LoadQtyEntry_Unfocused;
            BindingContext = this;
        }

        private void LoadQtyEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(LoadQtyEntry.Text))
            {
                UserDialogs.Instance.ShowError("Not allow blank quantity!", 3000);
                LoadQtyEntry.Focus();
                return;
            }
        }

        private void LoadQtyEntry_Completed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LoadQtyEntry.Text))
            {
                UserDialogs.Instance.ShowError("Not allow blank quantity!", 3000);
                LoadQtyEntry.Focus();
                return;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            data = new RequestLine();
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
                Task.Run(async () =>
                {
                    try
                    {
                        if (ItemId != 0)
                        {
                            DataManager manager = new DataManager();
                            data = await manager.GetRequestItemLinebyID(ItemId);
                        }
                        else
                            data = null;

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayData();
                            UserDialogs.Instance.HideLoading(); //IsLoading = false;
                        });

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
                });
        }

        void DisplayData()
        {
            if (ItemId != 0)
            {
                if (data != null)
                {
                    ItemId = data.ID;
                    EntryNo = data.EntryNo;
                    HeaderNo = data.HeaderEntryNo;
                    //DocumentNoEntry.Text = dt.Rows[0]["DocumentNo"].ToString();
                    ItemNoLabel.Text = data.ItemNo;
                    DescLabel.Text = data.ItemDesc;
                    UomCode = data.UomCode;
                    QtyperBagLabel.Text = data.QtyperBag.ToString();
                    NoofBagsLabel.Text = data.NoofBags.ToString();
                    ReqQtyLabel.Text = data.Quantity.ToString(); //string.Format("{0:0.00}", data.LineAmount);
                    PickQtyLabel.Text = data.PickQty.ToString();
                    LoadQtyEntry.Text = data.LoadQty.ToString();
                    VendorNo = data.VendorNo;
                    InHouse = data.InHouse;
                }
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            try
            {
  
                if (string.IsNullOrEmpty(LoadQtyEntry.Text))
                {
                    //DependencyService.Get<IMessage>().LongAlert("Not allow blank quantity!");
                    UserDialogs.Instance.ShowError("Not allow blank quantity!", 3000);
                    LoadQtyEntry.Focus();
                    return;
                }

                if (decimal.Parse(LoadQtyEntry.Text) == 0)
                {
                    //DependencyService.Get<IMessage>().LongAlert("Not allow 0 quantity!");
                    UserDialogs.Instance.ShowError("Not allow 0 quantity!", 3000);
                    LoadQtyEntry.Focus();
                    return;
                }

                DataManager manager = new DataManager();
                RequestLine line = new RequestLine()
                {
                    ID = data.ID,
                    EntryNo = data.EntryNo,
                    HeaderEntryNo = data.HeaderEntryNo,
                    ItemNo = data.ItemNo,
                    ItemDesc = data.ItemDesc,
                    QtyperBag = data.QtyperBag,
                    NoofBags = data.NoofBags,
                    Quantity = data.Quantity,
                    PickQty = data.PickQty,
                    LoadQty = decimal.Parse(LoadQtyEntry.Text),
                    SoldQty=data.SoldQty,
                    UnloadQty=data.UnloadQty,
                    UomCode = data.UomCode,
                    VendorNo = data.VendorNo,
                    RequestNo = data.RequestNo,
                    UserID = data.UserID,
                    InHouse = data.InHouse,
                    IsSync = data.IsSync,
                    SyncDateTime = data.SyncDateTime
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
                }
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }

        }
    }
}