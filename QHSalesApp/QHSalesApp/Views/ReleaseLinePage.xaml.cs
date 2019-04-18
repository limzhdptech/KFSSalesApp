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
    public partial class ReleaseLinePage : ContentPage
    {
        private ObservableCollection<SalesLine> recLines { get; set; }
        private string DocNo { get; set; }
        
        private bool _isloading;
        private bool _EnableNextBtn { get; set; }
        public bool IsLoading
        {
            get { return this._isloading; }
            set
            {
                this._isloading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public ReleaseLinePage(string docno)
        {
            InitializeComponent();
            this.Title = App.gPageTitle;
            this.BackgroundColor = Color.FromHex("#dddddd");
            DocNo = docno;
            sbSearch.Placeholder = "Search by Item No,Description";
           // this.ToolbarItems.Add(new ToolbarItem { Text = "Open",Icon="refresh.png", Command = new Command(this.DataLoading)});
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            IsLoading = false;
            BindingContext = this;
        }
        private void DataLoading()
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);  //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    recLines = new ObservableCollection<SalesLine>();
                    DataManager manager = new DataManager();
                    recLines = manager.GetSalesLinesbyDocNo(DocNo);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        TotalAmountNameLabel.Text = "Sub Total :";
                        GSTAmountNameLabel.Text = "GST :";
                        NetAmountNameLabel.Text = "Total :";
                        if (recLines!=null)
                        {
                            listview.ItemsSource = recLines != null ? recLines.OrderBy(x=>x.Description) : null;
                            decimal total = recLines.Sum(x => x.LineAmount);
                            decimal gstamt = 0;
                            if (recLines != null)
                            {
                                if (recLines.Count > 0)
                                {
                                    foreach (SalesLine s in recLines)
                                    {
                                        gstamt += (s.LineAmount * decimal.Parse(App.gPercentGST)) / 100;
                                    }
                                    lblRecCount.Text = "Record Count : " + recLines.Count.ToString();
                                }
                                else
                                    lblRecCount.Text = "Record Count : 0";
                            }
                            decimal netamt = total + gstamt;
                            TotalAmountLabel.Text = string.Format("{0:0.00}", total);
                            GSTAmountLabel.Text = string.Format("{0:0.00}", gstamt);
                            NetAmountLabel.Text = string.Format("{0:0.00}", netamt);
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            TotalAmountLabel.Text = string.Format("{0:0.00}", 0);
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
        protected  override  void OnAppearing()
        {
            base.OnAppearing();
            _EnableNextBtn = true;
            DataLoading();
        }

      //async void DataLoading()
      //  {
      //      IsLoading = true;
      //      try
      //      {
      //          DataManager manager = new DataManager();
      //          recLines = await  manager.GetSalesLinesbyDocNo(DocNo);

      //          if (recLines != null)
      //          {
      //              listview.ItemsSource = recLines;
      //          }
      //          else
      //          {
      //              listview.ItemsSource = null;

      //              //  EmptyDataLayout.IsVisible = true;
      //          }

      //          listview.Unfocus();
      //          IsLoading = false;
      //      }
      //      catch (OperationCanceledException ex)
      //      {
      //          IsLoading = false;
      //          DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
      //      }
      //      catch (Exception ex)
      //      {
      //          IsLoading = false;
      //          DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
      //      }
      //  }

       
      

        private void FilterKeyword(string filter)
        {
            if (recLines == null) return;
            listview.BeginRefresh();
            if (string.IsNullOrWhiteSpace(filter))
            {
                listview.ItemsSource = recLines;
            }
            else
            {
                listview.ItemsSource = recLines.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) || x.Description.ToLower().Contains(filter.ToLower()));
            }
            listview.EndRefresh();
        }

        private void NextButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            if(_EnableNextBtn)
            {
                _EnableNextBtn = false;
                if (recLines != null)
                {
                    if (recLines.Count > 0)
                    {
                        if (App.gDocType == "SO")
                            App.gPageTitle = "Confirmation (Sales Order)";
                        else
                            App.gPageTitle = "Confirmation (Credit Memo)";
                        Navigation.PushAsync(new ConfirmOrderPage(DocNo, recLines, "Released"));
                    }
                    else
                    {
                        //DependencyService.Get<IMessage>().LongAlert("Required to add Items");
                        UserDialogs.Instance.ShowError("Required to add Items", 3000);
                        _EnableNextBtn = true;
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