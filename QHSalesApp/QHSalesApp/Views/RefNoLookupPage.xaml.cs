using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RefNoLookupPage : ContentPage
    {
        private List<PaymentReference> objList { get; set; }
        public List<PaymentReference> dataList { get; set; }
        // private int intPageId { get; set; }
        public ListView listview { get { return LookupListView; } }

        public Button button { get { return FinishButton; } }
        private string custNo { get; set; }
        public RefNoLookupPage(string custno)
        {
            InitializeComponent();
            custNo = custno;
            this.Title = "Search Reference No";
            sbSearch.Placeholder = "Search by Document No";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            BindingContext = this;
        }

       
        protected override void OnAppearing()
        {
            base.OnAppearing();
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    objList = new List<PaymentReference>();
                    DataManager manager = new DataManager();
                    objList = await GetReferenceData();

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LookupListView.ItemsSource = objList != null ? objList : null;
                        LookupListView.Unfocus();
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
        private async Task<List<PaymentReference>> GetReferenceData()
        {
            //GetSQLite_CLEUnpaidBill
            DataManager manager = new DataManager();
            List<CustLedgerEntry> unpaidBills = new List<CustLedgerEntry>();
            unpaidBills = await manager.GetSQLite_CLEUnpaidBill(custNo);
            //GetSQLite_SalesHeaders
            List<SalesHeader> salesHeaders = new List<SalesHeader>();
            salesHeaders = await manager.GetSQLite_SalesHeadersbyCustNo(custNo);

            List<PaymentReference>  lstRef  = new List<PaymentReference>();
            if (unpaidBills==null && salesHeaders==null)
            {
                lstRef = null;
            }
            else
            {
                if (unpaidBills != null)
                {
                    if (unpaidBills.Count > 0)
                    {
                        foreach (CustLedgerEntry c in unpaidBills)
                        {
                            lstRef.Add(new PaymentReference
                            {
                                DocumentNo = c.DocNo,
                                Amount = c.UnpaidAmount,
                                SourceType = "CLE"
                            });
                        }
                    }
                }

                if (salesHeaders != null)
                {
                    if (salesHeaders.Count > 0)
                    {
                        foreach (SalesHeader s in salesHeaders)
                        {
                            lstRef.Add(new PaymentReference
                            {
                                DocumentNo = s.DocumentNo,
                                Amount = string.Format("{0:0.00}", s.TotalAmount),
                                SourceType = "SO"
                            });
                        }
                    }
                }
            }
            return lstRef;
        }
        private void SearchItemsFilter(string filter)
        {
            List<PaymentReference> filterItems = new List<PaymentReference>();
            if (objList != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    LookupListView.ItemsSource = objList;
                }
                else
                {
                    filterItems = objList.Where(x => x.DocumentNo.ToLower().Contains(filter.ToLower())).ToList();
                    LookupListView.ItemsSource = filterItems;
                }
            }

        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {

        }
    }
}