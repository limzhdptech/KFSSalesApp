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
    public partial class UnloadReturnPage : ContentPage
    {
        private ObservableCollection<UnloadReturn> recReturns { get; set; }

        private string SalesPersonCode { get; set; }
        public UnloadReturnPage(string salesPersonCode)
        {
            InitializeComponent();

            this.Title = "Unload Return";
            this.BackgroundColor = Color.FromHex("#dddddd");
            SalesPersonCode = salesPersonCode;
            sbSearch.Placeholder = "Search by Item No,Description";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            this.ToolbarItems.Add(new ToolbarItem { Text = "Confirm", Command = new Command(this.SyncBack) });
            BindingContext = this;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }
        async void SyncBack()
        {
            string confirmText = string.Empty;

            DependencyService.Get<INetworkConnection>().CheckNetworkConnection();
            if (DependencyService.Get<INetworkConnection>().IsConnected)
            {
                string result = DependencyService.Get<INetworkConnection>().IsServiceOnline(Helpers.Settings.GeneralSettings);
                if (result != "true")
                {
                    UserDialogs.Instance.ShowError("Error : Service is offline. [" + result + "]", 3000);
                    return;
                }
            }
            else
            {
                UserDialogs.Instance.ShowError("Error : No internet connection", 3000);
                return;
            }

            try
            {
                DataManager manager = new DataManager();

                if (recReturns != null)
                {
                    foreach (UnloadReturn itm in recReturns)
                    {
                        //App.svcManager.ExportUnloadHistory(deviceIdentifier, itm.ItemNo, itm.UnloadQty, itm.SoldQty, itm.ReturnQty, itm.BalQty, App.gSalesPersonCode);
                        //Sync back to middle tier with update
                       confirmText= App.svcManager.ExportUnloadReturn(SalesPersonCode, itm.ItemNo, itm.QSReturnQty, itm.ToBin, "ToNAV");
                    }

                    if (confirmText == "Success")
                    {
                        confirmText = App.svcManager.UnloadQSReclasstoNAV(SalesPersonCode);
                        if(confirmText=="Success")
                        {
                           confirmText=await manager.DeleteSQLite_UnloadReturnAndScannedDoc();
                            if (confirmText == "Success")
                                UserDialogs.Instance.ShowSuccess(confirmText, 3000);
                            else
                                UserDialogs.Instance.ShowError(confirmText, 3000);
                        }   
                        else
                            UserDialogs.Instance.ShowError(confirmText, 3000);
                    }
                    else
                    {
                        UserDialogs.Instance.ShowError(confirmText, 3000);
                    }
                }
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
        }

        async Task LoadData()
        {

            DependencyService.Get<INetworkConnection>().CheckNetworkConnection();
            if (DependencyService.Get<INetworkConnection>().IsConnected)
            {
                string result = DependencyService.Get<INetworkConnection>().IsServiceOnline(Helpers.Settings.GeneralSettings);
                if (result != "true")
                {
                    UserDialogs.Instance.ShowError("Error : Service is offline. [" + result + "]", 3000);
                    return;
                }
            }
            else
            {
                UserDialogs.Instance.ShowError("Error : No internet connection", 3000);
                return;
            }

            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    recReturns = new ObservableCollection<UnloadReturn>();
                    DataManager manager = new DataManager();
                    string retmsg= await manager.SaveSQLite_UnloadReturn(App.gSalesPersonCode);
                    if (retmsg == "Success")
                        recReturns = await manager.GetSQLite_UnloadReturn();
                    else
                        recReturns = null; //await manager.GetSQLite_ItemtoUnload(); // To get unload return list by Sales Person code
                    Device.BeginInvokeOnMainThread(() =>
                    {

                        if (recReturns != null)
                        {
                            listview.BeginRefresh();
                            listview.ItemsSource = recReturns;
                            listview.EndRefresh();
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            UserDialogs.Instance.ShowError("No Data", 3000);
                        }

                        listview.Unfocus();
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    });

                }
                catch (OperationCanceledException ex)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
            });
        }

        private void FilterKeyword(string filter)
        {
            if (recReturns == null) return;
            listview.BeginRefresh();
            if (string.IsNullOrWhiteSpace(filter))
            {
                listview.ItemsSource = recReturns.OrderByDescending(x => x.EntryNo);

            }
            else
            {
                listview.ItemsSource = recReturns.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) ||
                x.ItemDesc.ToString().ToLower().Contains(filter.ToLower()));
            }
            listview.EndRefresh();
        }


        private void ScanButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            App.gPageTitle = "Scan Bag Label to Unload";
            Navigation.PushAsync(new UnloadBagScanPage(string.Empty));
        }

        private async void ChangeButton_Clicked(object sender, EventArgs e)
        {
            var obj = (Button)sender;
            //UnloadReturn item = new UnloadReturn();
            //item = recReturns.Where(x => x.ItemNo == obj.CommandParameter.ToString()).FirstOrDefault();
            Navigation.PushAsync(new UnloadBagScanPage(obj.CommandParameter.ToString()));
           
        }

        private async void ClearButton_Clicked(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Title = "Clear Scanned",
                    Message = "Are you sure to clear?",
                    CancelText = "No",
                    OkText = "Yes"
                });
                if (result)
                {
                    try
                    {
                        var obj = (Button)sender;
                        DataManager manager = new DataManager();
                        string retstr = await manager.ClearSQLite_ScannedUnloadReturn(obj.CommandParameter.ToString());
                        LoadData();
                        UserDialogs.Instance.ShowSuccess("Success", 3000);
                    }
                    catch (Exception ex)
                    {
                        UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    }
                    
                }
            });
           

        }
    }
}
