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
    public partial class RequestLinePage : ContentPage
    {
        private ObservableCollection<RequestLine> recItems { get; set; }
        private string HeaderNo { get; set; }
        private string ReqNo { get; set; }
        public RequestLinePage(string headerno,string requestno)
        {
            InitializeComponent();

            this.Title = App.gPageTitle;
            this.BackgroundColor = Color.FromHex("#dddddd");
            Datalayout.IsVisible = false;
            Emptylayout.IsVisible = true;
            //this.ToolbarItems.Add(new ToolbarItem { Text = "Add", Icon = "add.png", Command = new Command(this.GoNextPage) });

            HeaderNo = headerno;
            ReqNo = requestno;
            if (App.gCurStatus == "request")
            {
                listview.ItemTapped += Listview_ItemTapped;
                //this.ToolbarItems.Add(new ToolbarItem { Text = "Send Request", Command = new Command(this.SendRequest) });
            }
            else
            {
                AddButton.IsVisible = false;
            }

            //DocumentNoLabel.Text = docNo;
            //CustomerNoLabel.Text = App.gCustCode;
            //sbSearch.Placeholder = "Search by Item No,Description";
            //sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await LoadData();
            //if(pagefrom=="Released")
            //{
            //    AddButton.IsVisible = false;
            //}
        }

        async void SendRequest()
        {
            try
            {
                if (recItems != null)
                {
                    if(recItems.Count==0)
                    {
                        UserDialogs.Instance.ShowError("No items for send request", 3000);
                        return;
                    }

                    UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
                    string alertmsg = string.Empty;
                    DataManager manager = new DataManager();
                    RequestHeader objhead = new RequestHeader();
                    objhead = await manager.GetSQLite_RequestHeadebyKey(HeaderNo);
                    if (objhead != null)
                    {
                        string retmsg = "Success";
                        //string retmsg = App.svcManager.ExportRequestStock(objhead.EntryNo, App.gSalesPersonCode, objhead.RequestNo, objhead.RequestDate, "topick");
                        if (retmsg == "Success")
                        {
                            manager = new DataManager();
                            string retval = await manager.SaveSQLite_RequestHeader(new RequestHeader
                            {
                                ID = objhead.ID,
                                EntryNo = objhead.EntryNo,
                                SalesPersonCode = objhead.SalesPersonCode,
                                RequestNo = objhead.RequestNo,
                                RequestDate = objhead.RequestDate,
                                IsSync = "true",
                                SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"),
                                CurStatus = "picking"
                            });
                        }
                        else
                        {
                            alertmsg = "Can not able to sync doc No " + objhead.RequestNo + ". Err message: " + retmsg;
                            UserDialogs.Instance.ShowError(alertmsg, 3000);
                        }

                        ObservableCollection<RequestLine> lstline = new ObservableCollection<RequestLine>();
                        lstline = await manager.GetRequestLinesbyDocNo(objhead.EntryNo);
                        if (lstline != null && lstline.Count > 0)
                        {
                            foreach (RequestLine l in lstline)
                            {
                                //retmsg = App.svcManager.ExportRequestLine(l.EntryNo, l.HeaderEntryNo, l.UserID, l.ItemNo, l.QtyperBag, l.NoofBags, l.Quantity, l.PickQty, l.LoadQty,l.UnloadQty,l.UomCode, l.VendorNo, l.InHouse, objhead.RequestNo);
                                if (retmsg == "Success")
                                {
                                    manager = new DataManager();
                                    RequestLine line = new RequestLine()
                                    {
                                        ID = l.ID,
                                        EntryNo = l.EntryNo,
                                        HeaderEntryNo = l.HeaderEntryNo,
                                        UserID = l.UserID,
                                        ItemNo = l.ItemNo,
                                        ItemDesc = l.ItemDesc,
                                        QtyperBag = l.QtyperBag,
                                        NoofBags = l.NoofBags,
                                        Quantity = l.Quantity,
                                        PickQty = l.Quantity, //l.PickQty,
                                        LoadQty = l.LoadQty,
                                        UomCode = l.UomCode,
                                        VendorNo = l.VendorNo,
                                        InHouse = l.InHouse,
                                        RequestNo = l.RequestNo,
                                        IsSync = "picking",
                                        SyncDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt")
                                    };
                                    await manager.SaveSQLite_RequestLine(line);

                                }
                                else
                                {
                                    alertmsg = "Can not able to sync doc no " + objhead.RequestNo + " -> Item No" + l.ItemNo + ". Err message: " + retmsg;
                                    UserDialogs.Instance.ShowError(alertmsg, 3000);
                                    return;
                                } 
                            }
                           // await Task.Delay(10000);
                            retmsg = "Success";// App.svcManager.ImportDataToNAV("SalesRequest", "picking");
                            if(retmsg=="Success")
                            {
                                //DependencyService.Get<INetworkConnection>().CheckNetworkConnection();
                                //if (DependencyService.Get<INetworkConnection>().IsConnected)
                                //{
                                //    string result = DependencyService.Get<INetworkConnection>().IsServiceOnline(Helpers.Settings.GeneralSettings);
                                //    if (result != "true")
                                //    {
                                //        //DependencyService.Get<IMessage>().LongAlert("Error : Service is offline. [" + result + "]");
                                //        UserDialogs.Instance.ShowError("Error : Service is offline. [" + result + "]", 3000);
                                //        return;
                                //    }
                                //}
                                //else
                                //{
                                //    UserDialogs.Instance.ShowError("Error : No internet connection", 3000);
                                //    return;
                                //}
                                // SyncNumberSeries();
                                UserDialogs.Instance.HideLoading();
                                alertmsg = "Request Stock sync success!";
                                UserDialogs.Instance.ShowSuccess(alertmsg, 3000);
                                Navigation.PopAsync();
                            }
                            else
                            {
                                UserDialogs.Instance.HideLoading();
                                UserDialogs.Instance.ShowError(retmsg, 3000);
                                return;
                            }
                        }
                        else
                        {
                            UserDialogs.Instance.HideLoading();
                            retmsg = "No line records";
                        }
                        // Sync Num series
                        //SyncNumberSeries();


                    }
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.ShowError("No items for send request", 3000);
                }
                  
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }

        private async void SyncNumberSeries()
        {
            DataManager dm = new DataManager();
            List<NumberSeries> seriesList = new List<NumberSeries>();
            seriesList = await dm.GetSQLite_NumberSeries();
            string SOLastCode = string.Empty;
            string CRLastCode = string.Empty;
            string MPLastCode = string.Empty;
            string RSLastCode = string.Empty;
            string ULLastCode = string.Empty;
            string SOLastSeries = string.Empty;
            string CRLastSeries = string.Empty;
            string MPLastSeries = string.Empty;
            string RSLastSeries = string.Empty;
            string ULLastSeries = string.Empty;
            if (seriesList != null && seriesList.Count > 0)
            {
                foreach (NumberSeries s in seriesList)
                {
                    switch (s.Description)
                    {
                        case "SO":
                            SOLastCode = s.LastNoCode;
                            SOLastSeries = s.LastNoSeries.ToString();
                            break;
                        case "CR":
                            CRLastCode = s.LastNoCode;
                            CRLastSeries = s.LastNoSeries.ToString();
                            break;
                        case "CP":
                            MPLastCode = s.LastNoCode;
                            MPLastSeries = s.LastNoSeries.ToString();
                            break;
                        case "RS":
                            RSLastCode = s.LastNoCode;
                            RSLastSeries = s.LastNoSeries.ToString();
                            break;
                        case "UL":
                            ULLastCode = s.LastNoCode;
                            ULLastSeries = s.LastNoSeries.ToString();
                            break;
                    }
                }
                string ret = App.svcManager.ExportNumSeries(App.gDeviceId, App.gSalesPersonCode, SOLastCode, CRLastCode, MPLastCode, RSLastCode,ULLastCode, SOLastSeries, CRLastSeries, MPLastSeries, RSLastSeries,ULLastSeries);
                //  UserDialogs.Instance.ShowError(App.gDeviceId +"- Message :" +ret, 5000);
            }

        }

        private void Listview_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
            var item = (RequestLine)e.Item;
             App.gPageTitle = "Edit  Request Item";
 
             Navigation.PushAsync(new RequestLnEntryPage(item.ID, item.HeaderEntryNo,ReqNo));
        }

        async Task LoadData()
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    
                    recItems = new ObservableCollection<RequestLine>();
                    DataManager manager = new DataManager();
                    recItems = await manager.GetRequestLinesbyRequestNo(ReqNo);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        
                        if (recItems != null)
                        {
                            if (recItems.Count > 0)
                            {
                                listview.ItemsSource = recItems.OrderBy(x => x.ItemDesc);
                                Datalayout.IsVisible = true;
                                Emptylayout.IsVisible = false;
                            }
                            else
                            {
                                listview.ItemsSource = null;
                                Datalayout.IsVisible = false;
                                Emptylayout.IsVisible = true;
                            }
                            
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            Datalayout.IsVisible = false;
                            Emptylayout.IsVisible = true;
                        }

                        listview.Unfocus();
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    });

                }
                catch (OperationCanceledException ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
            });
        }

        private void AddButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
             App.gPageTitle = "Add Request Item";
            Navigation.PushAsync(new RequestLnEntryPage(0, HeaderNo,ReqNo));
        }

        private void FilterKeyword(string filter)
        {
            if (recItems == null) return;
            listview.BeginRefresh();
            if (string.IsNullOrWhiteSpace(filter))
            {
                listview.ItemsSource = recItems;
            }
            else
            {
                listview.ItemsSource = recItems.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) || x.ItemDesc.ToLower().Contains(filter.ToLower()));
            }
            listview.EndRefresh();
        }
    }
}