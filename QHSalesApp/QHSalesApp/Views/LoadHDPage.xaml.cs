using Acr.UserDialogs;
using PCLBluetooth;
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
    public partial class LoadHDPage : ContentPage
    {
        readonly Database database;
        private IBluetoothClient _bluetoothClient;
        private ObservableCollection<RequestHeader> recHeaders { get; set; }
        private bool _isEnablePrintBtn { get; set; }
        public LoadHDPage()
        {
            InitializeComponent();
            database = new Database(Constants.DatabaseName);
            database.CreateTable<DeviceInfo>();
            DataLayout.IsVisible = false;
            Emptylayout.IsVisible = true;
            NavigationPage.SetHasBackButton(this, false);
            this.BackgroundColor = Color.FromHex("#dddddd");

            listview.ItemTapped += Listview_ItemTapped;

            sbSearch.Placeholder = "Search by Request No,Request Date";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            this.Title = "To Load List";
            App.gCurStatus = "request";
            this.ToolbarItems.Add(new ToolbarItem { Text = "Loaded List", Command = new Command(this.ChangeDocumentStatus) });
            BindingContext = this;
        }

        private async void ChangeDocumentStatus()
        {
            this.ToolbarItems.Clear();
            _isEnablePrintBtn = true;
            if (App.gCurStatus == "request")
            {
                this.Title = "Loaded List";
                App.gCurStatus = "loaded";
               // LoadButton.IsVisible = false;
                this.ToolbarItems.Add(new ToolbarItem { Text = "To Load List", Command = new Command(this.ChangeDocumentStatus) });
            }
            else
            {
                this.Title = "To Load List";
                App.gCurStatus = "request";
               // LoadButton.IsVisible = true;
                this.ToolbarItems.Add(new ToolbarItem { Text = "Loaded List", Command = new Command(this.ChangeDocumentStatus) });
            }

            await LoadData();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            _isEnablePrintBtn = true;
            //if (App.gCurStatus == "picking")
            //    await PopulateData();
            //else
                await LoadData();
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(0));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private void Listview_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
            var item = (RequestHeader)e.Item;
            if (App.gCurStatus == "picking")
            {
                App.gPageTitle = "Picked Items";
            }
            else
                App.gPageTitle = "Loaded Items";
            Navigation.PushAsync(new LoadItemPage(item.RequestNo)); 
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }

        private void FilterKeyword(string filter)
        {
            if (recHeaders == null) return;
            listview.BeginRefresh();
            if (string.IsNullOrWhiteSpace(filter))
            {
                listview.ItemsSource = recHeaders.OrderByDescending(x => x.ID);
            }
            else
            {
                listview.ItemsSource = recHeaders.Where(x => x.RequestNo.ToLower().Contains(filter.ToLower()) ||
                x.RequestDate.ToString().ToLower().Contains(filter.ToLower()));
            }
            listview.EndRefresh();
        }

        async Task LoadData()
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    recHeaders = new ObservableCollection<RequestHeader>();
                    DataManager manager = new DataManager();
                    recHeaders = await manager.GetRequestHeaderbyStatus(App.gCurStatus);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                       
                        if (recHeaders != null)
                        {
                            if (recHeaders.Count > 0)
                            {
                                listview.ItemsSource = recHeaders.OrderByDescending(x => x.ID);
                                DataLayout.IsVisible = true;
                                Emptylayout.IsVisible = false;
                            }
                            else
                            {
                                listview.ItemsSource = null;
                                DataLayout.IsVisible = false;
                                Emptylayout.IsVisible = true;
                                UserDialogs.Instance.ShowError("No Data", 3000);
                            }
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            DataLayout.IsVisible = false;
                            Emptylayout.IsVisible = true;
                            UserDialogs.Instance.ShowError("No Data", 3000);
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
                catch (Exception ex1)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                    UserDialogs.Instance.ShowError(ex1.Message.ToString(), 3000);
                }
            });
        }

        async Task PopulateData()
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
            Task.Run(async () =>
            {
                try
                {
                    //Populate data from asmx (direct download)
                    DependencyService.Get<INetworkConnection>().CheckNetworkConnection();
                    if (DependencyService.Get<INetworkConnection>().IsConnected)
                    {
                        string result = DependencyService.Get<INetworkConnection>().IsServiceOnline(Helpers.Settings.GeneralSettings);
                        if (result != "true")
                        {
                            //DependencyService.Get<IMessage>().LongAlert("Error : Service is offline. [" + result + "]");
                            UserDialogs.Instance.ShowError("Error : Service is offline. [" + result + "]", 3000);
                            return;
                        }

                    }
                    else
                    {
                        UserDialogs.Instance.ShowError("Error : No internet connection", 3000);
                        return;
                    }

                    DataManager manager = new DataManager();
                    string retval = await manager.SaveSQLite_PopulatedPick("picking");
                    // UserDialogs.Instance.Toast(retval, null);
                    if (retval == "Success")
                    {
                        await LoadData();
                    }
                    //else
                    //{
                    //    UserDialogs.Instance.ShowError("No picked data!", 3000);
                    //}

                    Device.BeginInvokeOnMainThread(() =>
                    {
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

        private async void LoadButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            try
            {
            //Populate data from asmx (direct download)
            DependencyService.Get<INetworkConnection>().CheckNetworkConnection();
            if (DependencyService.Get<INetworkConnection>().IsConnected)
            {
                string result = DependencyService.Get<INetworkConnection>().IsServiceOnline(Helpers.Settings.GeneralSettings);
                if (result != "true")
                {
                    //DependencyService.Get<IMessage>().LongAlert("Error : Service is offline. [" + result + "]");
                    UserDialogs.Instance.ShowError("Error : Service is offline. [" + result + "]", 3000);
                    return;
                }

            }
            else
            {
                UserDialogs.Instance.ShowError("Error : No internet connection", 3000);
                return;
            }

                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
                DataManager manager = new DataManager();
                string retval = await manager.SaveSQLite_PopulatedPick("picking");
                if(retval=="Success")
                {
                     await LoadData();
                    UserDialogs.Instance.HideLoading();
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.ShowError("No picked data!", 3000);
                }
            }
            catch (OperationCanceledException ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }

        private void BagLabelButton_Clicked(object sender, EventArgs e)
        {
            App.gPageTitle = "Scan Bag Label to Load";
            var item = (Button)sender;
            Navigation.PushAsync(new BagLabelScanPage("loaded", item.CommandParameter.ToString()));
        }
        private void LoadItemsButton_Clicked(object sender, EventArgs e)
        {
            var item = (Button)sender;
            if (App.gCurStatus == "picking")
            {
                App.gPageTitle = "Picking Items";
            }
            else
                App.gPageTitle = "Loaded Items";
            //SalesHeader head = new SalesHeader();
            //head = recItems.Where(x => x.DocumentNo == item.CommandParameter.ToString()).FirstOrDefault();
            //App.gCustCode = head.SellToCustomer;
            Navigation.PushAsync(new LoadItemPage(item.CommandParameter.ToString()));
        }

        private async void PrintButton_Clicked(object sender, EventArgs e)
        {
            if(_isEnablePrintBtn)
            {
                _isEnablePrintBtn = false;

                var item = (Button)sender;
                //item.CommandParameter.ToString()
                string retval = string.Empty;
                string retmsg = string.Empty;
                DataManager manager = new DataManager();
                RequestHeader head = new RequestHeader();
                ObservableCollection<RequestLine> recs = new ObservableCollection<RequestLine>();
                DeviceInfo info = new DeviceInfo();
                bool canPrint = false;
                try
                {
                    var answer = await DisplayAlert("Print", "Are you sure to print Check In Report?", "Yes", "No");
                    UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
                    if (answer)
                    {
                        canPrint = true;
                        //var devices = await _bluetoothClient.GetPairedDevices();
                        //if (devices != null)
                        //{
                        //    if (devices.Count > 0)
                        //    {
                        //        canPrint = true;
                        //    }
                        //    else
                        //    {
                        //        UserDialogs.Instance.AlertAsync("Not found bluetooth device!", "Alert");
                        //        return;
                        //    }
                        //}
                        //else
                        //{
                        //    UserDialogs.Instance.AlertAsync("Not found bluetooth device!", "Alert");
                        //    return;
                        //}
                    }
                    else
                    {
                        UserDialogs.Instance.HideLoading();
                        _isEnablePrintBtn = true;
                        return;
                    }

                    if (canPrint)
                    {
                        info = await manager.GetDeviceInfo();
                        if (info != null)
                        {
                            if (!string.IsNullOrEmpty(info.DeviceName))
                            {
                                try
                                {
                                    manager = new DataManager();
                                    head = await manager.GetRequestHeaderbyID(int.Parse(item.CommandParameter.ToString())); //GetRequestLinesbyRequestNo
                                    if (head != null)
                                    {
                                        recs = await manager.GetRequestLinesbyRequestNo(head.RequestNo);
                                        if (recs != null)
                                        {
                                            if (recs.Count > 0)
                                            {
                                                retmsg = "Success";
                                            }
                                            else
                                                retmsg = "No request lines";
                                        }
                                        else
                                            retmsg = "No request lines";
                                    }
                                    else
                                    {
                                        retmsg = "No request document";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    retmsg = ex.Message.ToString();
                                }
                            }
                            else
                                retmsg = "Required to setup bluetooth printer!";
                        }
                        else
                        {
                            retmsg = "Required to setup bluetooth printer!";
                        }
                    }

                    UserDialogs.Instance.HideLoading();
                    if (retmsg == "Success")
                    {
                        bool isBeforeConfirm = true;
                        if (App.gCurStatus == "loaded")
                        {
                            isBeforeConfirm = false;
                        }
                        var a = Utils.Print_CheckIn(info.DeviceName, head, recs, App.gCompanyName, App.gSalesPersonCode + "/" + App.gSalesPersonName, isBeforeConfirm); //head, recs, customer, sellTo, App.gSalesPersonCode + "/" + App.gSalesPersonName);
                        UserDialogs.Instance.Alert(a);
                        //  Navigation.PopAsync();
                    }
                    else
                    {
                        UserDialogs.Instance.ShowError(retmsg, 3000);
                        _isEnablePrintBtn = true;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    _isEnablePrintBtn = true;
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    _isEnablePrintBtn = true;
                }
            }
            
        }
    }
}