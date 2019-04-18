using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustPriceHisPage : ContentPage
    {
        private int cmdPara { get; set; }
        private string ItemNo { get; set; }
        private CustomerPriceHistory data { get; set; }
        private List<CustomerPriceHistory> list { get; set; }
        private bool IsBack { get; set; }
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

        public CustPriceHisPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            //this.ToolbarItems.Add(new ToolbarItem { Text = "Back", Command = new Command(this.BackPage) });
            ItemNo = App.gItem.ItemNo;
            //this.Title = ItemNo + " - Price History";
            TitleLabel.Text = ItemNo + " - Price History";
            this.BackgroundColor = Color.FromHex("#dddddd");
            IsLoading = false;
            BindingContext = this;
        }

        void BackPage()
        {
            Navigation.PushAsync(new MainPage(6));
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(6));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CustNoLabel.Text = App.gCustomer.CustomerNo;
            CustNameLabel.Text = App.gCustomer.Name;
            ItemNoLabel.Text = App.gItem.ItemNo;
            ItemDescLabel.Text = App.gItem.Description;
            OnLoad(App.gCustomer.CustomerNo, App.gItem.ItemNo);
        }


        private void OnLoad(string custno,string itemno)
        {
            //data = new CustomerPriceHistory();
            if (!IsBack)
            {
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
                Task.Run(async () =>
                {
                    try
                    {
                        DataManager manager = new DataManager();
                        list = new List<CustomerPriceHistory>();
                        list = await manager.GetSQLite_CustomerPriceHistory(custno, itemno);
                        //DataTable dt = new DataTable();
                        //if (!string.IsNullOrEmpty(itemno))
                        //{
                        //    //GetSQLite_CustomerPriceHistory
                        //    dt = await App.svcManager.RetCustomerPriceHistory(custno,itemno);
                            
                        //}
                        //else
                        //    dt = null;

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayData(list);
                            UserDialogs.Instance.HideLoading(); //IsLoading = false;
                        });

                    }
                    catch (OperationCanceledException ex)
                    {
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                       // DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
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
        }

        //private void DisplayData(DataTable dt)
        //{
        //    if (dt.Rows.Count > 0)
        //    {
        //      //  DataLayout.IsVisible = true;
        //        //EmptyLayout.IsVisible = false;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TranDateLabel.Text = dr["TransDate"].ToString();
        //            PriceLabel.Text = dr["Currency"].ToString() + " " + string.Format("{0:0.00}", decimal.Parse(dr["unitPrice"].ToString()));
        //            qtyLabel.Text = dr["Qty"].ToString() + " " + dr["UOM"].ToString();

        //            TranDate2Label.Text = dr["TransDate2"].ToString();
        //            Price2Label.Text = dr["Currency"].ToString() + " " + string.Format("{0:0.00}", decimal.Parse(dr["unitPrice2"].ToString()));
        //            qty2Label.Text = dr["Qty2"].ToString() + " " + dr["UOM"].ToString();

        //            TranDate3Label.Text = dr["TransDate3"].ToString();
        //            Price3Label.Text = dr["Currency"].ToString() + " " + string.Format("{0:0.00}", decimal.Parse(dr["unitPrice3"].ToString()));
        //            qty3Label.Text = dr["Qty3"].ToString() + " " + dr["UOM"].ToString();
        //        }
        //    }
        //    else
        //        ResetData();

        //}

        private void DisplayData(List<CustomerPriceHistory> priceList)
        {
            if (priceList!=null)
            {
                //  DataLayout.IsVisible = true;
                //EmptyLayout.IsVisible = false;
                if (priceList.Count > 0)
                {
                    foreach (CustomerPriceHistory p in priceList)
                    {
                        TranDateLabel.Text = p.TransDate;
                        PriceLabel.Text = p.Currency + " " + string.Format("{0:0.00}", decimal.Parse(p.UnitPrice));// dr["Currency"].ToString() + " " + string.Format("{0:0.00}", decimal.Parse(dr["unitPrice"].ToString()));
                        qtyLabel.Text = p.Qty + " " + p.UOM;

                        TranDate2Label.Text = p.TransDate2;
                        Price2Label.Text = p.Currency + " " + string.Format("{0:0.00}", decimal.Parse(p.UnitPrice2));
                        qty2Label.Text = p.Qty2 + " " + p.UOM;

                        TranDate3Label.Text = p.TransDate3;
                        Price3Label.Text = p.Currency + " " + string.Format("{0:0.00}", decimal.Parse(p.unitPrice3));
                        qty3Label.Text = p.Qty3 + " " + p.UOM;
                    }
                }
                else
                    ResetData();
                
            }
            else
                ResetData();

        }

        private void ResetData()
        {
            TranDateLabel.Text = "-";
            PriceLabel.Text = string.Format("{0:0.00}", 0);
            qtyLabel.Text = "0";

            TranDate2Label.Text = "-";
            Price2Label.Text = string.Format("{0:0.00}", 0);
            qty2Label.Text = "0";

            TranDate3Label.Text = "-";
            Price3Label.Text = string.Format("{0:0.00}", 0);
            qty3Label.Text = "0";
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ItemDetailPage());
        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new ItemPurHistoryPage());
          //  Navigation.PushAsync(new CustPriceHisPage());
        }

        private void TapGestureRecognizer_Tapped_5(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(6));
        }
    }
}