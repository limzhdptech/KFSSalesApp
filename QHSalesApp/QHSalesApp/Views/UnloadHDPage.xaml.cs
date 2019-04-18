using Acr.UserDialogs;
using PCLBluetooth;
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
    public partial class UnloadHDPage : ContentPage
    {
        private IBluetoothClient _bluetoothClient;
        private ObservableCollection<UnloadHeader> recHeaders { get; set; }
        private bool isEnablePrintBtn { get; set; }
        public UnloadHDPage()
        {
            InitializeComponent();
            //NavigationPage.SetHasBackButton(this, false);
            this.BackgroundColor = Color.FromHex("#dddddd");
            DataLayout.IsVisible = false;
            Emptylayout.IsVisible = true;
            listview.ItemTapped += Listview_ItemTapped;

            sbSearch.Placeholder = "Search by Unload Doc No,Date";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            this.Title = "To Unload List";
            App.gCurStatus = "loaded";
           // this.ToolbarItems.Add(new ToolbarItem { Text = "Loaded List", Command = new Command(this.ChangeDocumentStatus) });
            BindingContext = this;
        }

        private async void ChangeDocumentStatus()
        {
            this.ToolbarItems.Clear();

            if (App.gCurStatus == "loaded")
            {
                this.Title = "Unloaded List";
                App.gCurStatus = "unloaded";
                this.ToolbarItems.Add(new ToolbarItem { Text = "Loaded List", Command = new Command(this.ChangeDocumentStatus) });
            }
            else
            {
                this.Title = "Loaded List";
                App.gCurStatus = "loaded";
                this.ToolbarItems.Add(new ToolbarItem { Text = "Unloaded List", Command = new Command(this.ChangeDocumentStatus) });
            }

            await LoadData();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            isEnablePrintBtn = true;
            //if (App.gCurStatus == "loaded")
            //    await SyncData();
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
            var item = (UnloadHeader)e.Item;
            Navigation.PushAsync(new UnloadLinePage(item.UnloadDocNo));
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
                listview.ItemsSource = recHeaders.Where(x => x.UnloadDocNo.ToLower().Contains(filter.ToLower()) ||
                x.UnloadDate.ToString().ToLower().Contains(filter.ToLower()));
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
                    recHeaders = new ObservableCollection<UnloadHeader>();
                    DataManager manager = new DataManager();
                    recHeaders = await manager.GetSQLite_UnloadHeader();

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (recHeaders != null)
                        {
                            if(recHeaders.Count>0)
                            {
                                listview.ItemsSource = recHeaders.OrderByDescending(x => x.ID);
                                DataLayout.IsVisible = true;
                                Emptylayout.IsVisible = false;
                            }
                           
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            //UserDialogs.Instance.ShowError("No Data", 3000);
                            DataLayout.IsVisible = false;
                            Emptylayout.IsVisible = true;
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

        async Task SyncData()
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
                string retval = await manager.SaveSQLite_PopulatedPick("picked");
                if (retval == "Success")
                {
                    await LoadData();
                }
                //else
                //{
                //    UserDialogs.Instance.ShowError("No picked data!", 3000);
                //}
            }
            catch (OperationCanceledException ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
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

                DataManager manager = new DataManager();
                string retval = await manager.SaveSQLite_PopulatedPick("picked");
                if (retval == "Success")
                {
                    await LoadData();
                }
                else
                {
                    UserDialogs.Instance.ShowError("No picked data!", 3000);
                }
            }
            catch (OperationCanceledException ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }

        private void BagLabelButton_Clicked(object sender, EventArgs e)
        {
            App.gPageTitle = "Scan Bag Label to Unload";
            var item = (Button)sender;
            Navigation.PushAsync(new BagLabelScanPage("unloaded",string.Empty));
        }

        private void LoadItemsButton_Clicked(object sender, EventArgs e)
        {
            var item = (Button)sender;
            if (App.gCurStatus == "unloaded")
            {
                App.gPageTitle = "Unloaded Items";
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
            if(isEnablePrintBtn)
            {
                isEnablePrintBtn = false;
                var item = (Button)sender;
                //item.CommandParameter.ToString()
                string retval = string.Empty;
                string retmsg = string.Empty;
                DataManager manager = new DataManager();
                UnloadHeader head = new UnloadHeader();
                ObservableCollection<UnloadLine> recs = new ObservableCollection<UnloadLine>();
                DeviceInfo info = new DeviceInfo();
                bool canPrint = false;
                try
                {
                    var answer = await DisplayAlert("Print Unload Stock", "Are you sure to print Unload Stock?", "Yes", "No");
                    if (answer)
                    {
                        canPrint = true;
                        //_bluetoothClient = DependencyService.Get<IBluetoothClient>();
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
                        isEnablePrintBtn = true;
                        return;

                    }
                    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                    Task.Run(async () =>
                    {
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
                                        head = await manager.GetUnloadHeaderbyID(int.Parse(item.CommandParameter.ToString())); //GetRequestLinesbyRequestNo
                                        if (head != null)
                                        {
                                            recs = await manager.GetUnloadLinesbyDocNo(head.UnloadDocNo);
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

                    }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                    {

                        UserDialogs.Instance.HideLoading();
                        if (retmsg == "Success")
                        {
                            var a = Utils.Print_CheckOut(info.DeviceName, head, recs, App.gCompanyName, App.gSalesPersonCode + "/" + App.gSalesPersonName); //head, recs, customer, sellTo, App.gSalesPersonCode + "/" + App.gSalesPersonName);
                            UserDialogs.Instance.Alert(a);
                            isEnablePrintBtn = true;
                            //  Navigation.PopAsync();
                        }
                        else
                        {
                            UserDialogs.Instance.ShowError(retmsg, 3000);
                            isEnablePrintBtn = true;
                        }
                    }));
                }
                catch (OperationCanceledException ex)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    isEnablePrintBtn = true;
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    isEnablePrintBtn = true;
                }
            }
            
        }
    }
}