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
    public partial class SalesLinePage : ContentPage
    {
        private ObservableCollection<SalesLine> recItems { get; set; }
        private string DocNo { get; set; }
        private string pagefrom { get; set; }
        private bool _isloading;
        private bool _EnableNextBtn { get; set; }

        private bool _EnableAddBtn { get; set; }
        public bool IsLoading
        {
            get { return this._isloading; }
            set
            {
                this._isloading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public SalesLinePage(string docNo,string from)
        {
            InitializeComponent();
          
           
            this.BackgroundColor = Color.FromHex("#dddddd");

            pagefrom = from;
            if (App.gDocType == "SO")
                App.gPageTitle = "SO - " + docNo;
            else
                App.gPageTitle = "CR - " + docNo;
            this.Title = App.gPageTitle;
            //if (docStatus != "Open")
            //{
            //    AddButton.IsVisible = false;
            //}
            //else
            listview.ItemTapped += Listview_ItemTapped;
            // this.ToolbarItems.Add(new ToolbarItem { Text = "Add", Icon = "add.png", Command = new Command(this.GoNextPage) });

            DocNo = docNo;
           // DocumentNoLabel.Text = docNo;
            //CustomerNoLabel.Text = App.gCustCode;
            sbSearch.Placeholder = "Search by Item No, Desc, Barcode";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            IsLoading = false;
            BindingContext = this;
        }

        protected  override async void OnAppearing()
        {
            base.OnAppearing();
            _EnableAddBtn = true;
            _EnableNextBtn = true;
            await LoadData();
            //if(pagefrom=="Released")
            //{
            //    AddButton.IsVisible = false;
            //}
        }

        private void Listview_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
            var item = (SalesLine)e.Item;
            if(pagefrom!="Released")
            {
                if(App.gDocType=="SO")
                    App.gPageTitle = "Edit Item (SO)";
                else
                    App.gPageTitle = "Edit Item (CR)";
                App.gIsExchange = false;
                Navigation.PushAsync(new ItemEntryPage(item.ID, DocNo));
            }
        }

        async Task LoadData()
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    recItems = new ObservableCollection<SalesLine>();
                    DataManager manager = new DataManager();
                    recItems = manager.GetSalesLinesbyDocNo(DocNo);
                    
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        TotalAmountNameLabel.Text = "Sub Total :";
                        GSTAmountNameLabel.Text = "GST :";
                        NetAmountNameLabel.Text = "Total :";
                        if (recItems != null)
                        {
                            listview.ItemsSource = recItems.OrderBy(x => x.Description);
                            
                            decimal total = recItems.Sum(x => x.LineAmount);
                            decimal gstamt = 0;
                            if (recItems != null)
                            {
                                if (recItems.Count > 0)
                                {
                                    foreach (SalesLine s in recItems)
                                    {
                                        gstamt += (s.LineAmount * decimal.Parse(App.gPercentGST)) / 100;
                                    }
                                    lblRecCount.Text ="record count : " + recItems.Count.ToString();
                                }
                                else
                                    lblRecCount.Text = "record count : 0";
                            }
                            decimal netamt = total + gstamt;

                            TotalAmountLabel.Text = string.Format("{0:0.00}", total) ;
                            GSTAmountLabel.Text =  string.Format("{0:0.00}", gstamt);
                            NetAmountLabel.Text= string.Format("{0:0.00}", netamt);
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            TotalAmountLabel.Text =string.Format("{0:0.00}", 0);
                            GSTAmountLabel.Text = string.Format("{0:0.00}", 0);
                            NetAmountLabel.Text = string.Format("{0:0.00}", 0);
                        }

                        listview.Unfocus();
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

        //async Task BindRecords()
        //{
        //    DataTable dt = new DataTable();
        //    dt = await App.svcManager.RetrieveTfLines(DocNo);
        //    if (dt.Rows.Count > 0)
        //    {
        //        recItems = new ObservableCollection<SalesLine>();
        //        // int i = 0;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            recItems.Add(new SalesLine
        //            {
        //                ID = int.Parse(dr["EntryNo"].ToString()),
        //                DocumentNo = dr["DocumentNo"].ToString(),
        //                ItemNo = dr["ItemNo"].ToString(),
        //                // LoginEntryNo = int.Parse(dr["LoginEntryNo"].ToString()),
        //                Description = dr["Description"].ToString(),
        //                UOM = dr["UOM"].ToString(),
        //                Quantity = decimal.Parse(dr["Quantity"].ToString())
        //            });
        //        }
        //    }
        //}

        private void AddButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            if(_EnableAddBtn)
            {
                _EnableAddBtn = false;
                if (App.gDocType == "SO")
                    App.gPageTitle = "New Item (SO)";
                else
                    App.gPageTitle = "New Item (CR)";
                App.gIsExchange = false;
                Navigation.PushAsync(new ItemEntryPage(0, DocNo));

            } 
        }

        private void FilterKeyword(string filter)
        {
            try
            {
                if (recItems == null) return;
                listview.BeginRefresh();
                if (string.IsNullOrWhiteSpace(filter))
                {
                    listview.ItemsSource = recItems;
                }
                else
                {
                    listview.ItemsSource = recItems.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) || x.Description.ToLower().Contains(filter.ToLower()) || x.BagNo.ToLower().Contains(filter.ToLower()));
                }
                listview.EndRefresh();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            
        }

        private void NextButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            if(_EnableNextBtn)
            {
                _EnableNextBtn = false;
                if (recItems != null)
                {
                    if (recItems.Count > 0)
                    {
                        if (App.gDocType == "SO")
                            App.gPageTitle = "Confirmation (Sales Order)";
                        else
                            App.gPageTitle = "Confirmation (Credit Memo)";
                        Navigation.PushAsync(new ConfirmOrderPage(DocNo, recItems, pagefrom));
                    }
                    else
                    {
                        // DependencyService.Get<IMessage>().LongAlert("Required to add Items");
                        _EnableNextBtn = true;
                        UserDialogs.Instance.ShowError("Required to add Items", 3000);
                    }
                }
                else
                {
                    //DependencyService.Get<IMessage>().LongAlert("Required to add Items");
                    UserDialogs.Instance.ShowError("Required to add Items", 3000);
                    _EnableNextBtn = true;
                }
            }

            
        }
    }
}