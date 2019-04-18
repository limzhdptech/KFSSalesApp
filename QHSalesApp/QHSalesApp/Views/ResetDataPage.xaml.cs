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
    public partial class ResetDataPage : ContentPage
    {
        readonly Database database;
        public ResetDataPage()
        {
            InitializeComponent();
            database = new Database(Constants.DatabaseName);
            this.Title = "Reset Data";
            NavigationPage.SetHasBackButton(this, false);
            this.BackgroundColor = Color.FromHex("#dddddd");
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(0));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private async void ResetTransButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                DependencyService.Get<INetworkConnection>().CheckNetworkConnection();
                if (!DependencyService.Get<INetworkConnection>().IsConnected)
                {
                    UserDialogs.Instance.ShowError("Error : No internet connection", 3000);
                    return;
                }
                    DataManager manager = new DataManager();
                string retmsg = string.Empty;

                var answer = await DisplayAlert("Reset", "Are you sure to Reset Transaction?", "Yes", "No");
                if (!answer)
                {
                    return;
                }
                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {
                    manager.ResetTransData();
                    IDevice device = DependencyService.Get<IDevice>();
                    string deviceIdentifier = device.GetIdentifier();
                    manager.SaveSQLite_Setup(deviceIdentifier);

                    // Number Series
                    Setup setup = new Setup();
                    setup = manager.GetSQLite_Setup();
                    if (setup != null)
                    {
                        User user = new User();
                        DataManager dm = new DataManager();
                        user = dm.LoadSQLite_UserbyEmail(Helpers.Settings.UserEmail);
                        if (user == null)
                        {
                            UserDialogs.Instance.ShowError("User Email not found!", 3000);
                            return;
                        }

                        DataTable dt = new DataTable();
                        dt = App.svcManager.RetNumSeries(deviceIdentifier, App.gSalesPersonCode);
                        database.CreateTable<NumberSeries>();
                        ObservableCollection<NumberSeries> numSeries = new ObservableCollection<NumberSeries>();

                        if (dt.Rows.Count > 0)
                        {
                            App.gUserEntryNo = user.EntryNo; ;
                            App.gSalesPersonCode = user.SalesPersonCode;

                            string codePart = App.gSalesPersonCode; //App.gUserEntryNo.ToString();
                            App.gSOPrefix = setup.SOPrefix + codePart;
                            App.gCRPrefix = setup.CRPrefix + codePart;
                            App.gCPPrefix = setup.CPPrefix + codePart;
                            App.gRSPrefix = setup.RSPrefix + codePart;
                            App.gULPrefix = setup.ULPrefix + codePart;

                            string lastSONo = dt.Rows[0]["SOLastNoCode"].ToString();
                            string lastCRNo = dt.Rows[0]["CRLastNoCode"].ToString();
                            string lastMPNo = dt.Rows[0]["CPLastNoCode"].ToString();
                            string lastRSNo = dt.Rows[0]["RSLastNoCode"].ToString(); //MSO6MIX-10012
                            string LastULNo = dt.Rows[0]["ULLastNoCode"].ToString();

                            int LastSONumSeries = int.Parse(dt.Rows[0]["SOLastNoSeries"].ToString());
                            int LastCRNumSeries = int.Parse(dt.Rows[0]["CRLastNoSeries"].ToString());
                            int LastMPNumSeries = int.Parse(dt.Rows[0]["CPLastNoSeries"].ToString());
                            int LastRSNumSeries = int.Parse(dt.Rows[0]["RSLastNoSeries"].ToString());
                            int LastULNumSeries = int.Parse(dt.Rows[0]["ULLastNoSeries"].ToString());

                            numSeries.Add(new NumberSeries() { Code = App.gSOPrefix, Description = "SO", Increment = int.Parse(setup.Increment), LastNoCode = lastSONo, LastNoSeries = LastSONumSeries });
                            numSeries.Add(new NumberSeries() { Code = App.gCRPrefix, Description = "CR", Increment = int.Parse(setup.Increment), LastNoCode = lastCRNo, LastNoSeries = LastCRNumSeries });
                            numSeries.Add(new NumberSeries() { Code = App.gCPPrefix, Description = "CP", Increment = int.Parse(setup.Increment), LastNoCode = lastMPNo, LastNoSeries = LastMPNumSeries });
                            numSeries.Add(new NumberSeries() { Code = App.gRSPrefix, Description = "RS", Increment = int.Parse(setup.Increment), LastNoCode = lastRSNo, LastNoSeries = LastRSNumSeries });
                            numSeries.Add(new NumberSeries() { Code = App.gULPrefix, Description = "UL", Increment = int.Parse(setup.Increment), LastNoCode = LastULNo, LastNoSeries = LastULNumSeries });

                            manager.SaveSQLite_NumberSeries(numSeries);
                        }
                        else
                        {

                            App.gUserEntryNo = user.EntryNo; ;
                            App.gSalesPersonCode = user.SalesPersonCode;

                            string codePart = App.gSalesPersonCode; //App.gUserEntryNo.ToString();
                            App.gSOPrefix = setup.SOPrefix + codePart;
                            App.gCRPrefix = setup.CRPrefix + codePart;
                            App.gCPPrefix = setup.CPPrefix + codePart;
                            App.gRSPrefix = setup.RSPrefix + codePart;
                            App.gULPrefix = setup.ULPrefix + codePart;
                            numSeries.Add(new NumberSeries() { Code = App.gSOPrefix, Description = "SO", Increment = int.Parse(setup.Increment), LastNoCode = App.gSOPrefix + "-" + setup.StartNum, LastNoSeries = int.Parse(setup.StartNum) });
                            numSeries.Add(new NumberSeries() { Code = App.gCRPrefix, Description = "CR", Increment = int.Parse(setup.Increment), LastNoCode = App.gCRPrefix + "-" + setup.StartNum, LastNoSeries = int.Parse(setup.StartNum) });
                            numSeries.Add(new NumberSeries() { Code = App.gCPPrefix, Description = "CP", Increment = int.Parse(setup.Increment), LastNoCode = App.gCPPrefix + "-" + setup.StartNum, LastNoSeries = int.Parse(setup.StartNum) });
                            numSeries.Add(new NumberSeries() { Code = App.gRSPrefix, Description = "RS", Increment = int.Parse(setup.Increment), LastNoCode = App.gRSPrefix + "-" + setup.StartNum, LastNoSeries = int.Parse(setup.StartNum) });
                            numSeries.Add(new NumberSeries() { Code = App.gULPrefix, Description = "UL", Increment = int.Parse(setup.Increment), LastNoCode = App.gULPrefix + "-" + setup.StartNum, LastNoSeries = int.Parse(setup.StartNum) });

                            App.svcManager.ExportNumSeries(deviceIdentifier, App.gSalesPersonCode, App.gSOPrefix + "-" + setup.StartNum, App.gCRPrefix + "-" + setup.StartNum, App.gCPPrefix + "-" + setup.StartNum, App.gRSPrefix + "-" + setup.StartNum, App.gULPrefix + "-" + setup.StartNum, setup.StartNum, setup.StartNum, setup.StartNum, setup.StartNum, setup.StartNum);
                            manager.SaveSQLite_NumberSeries(numSeries);

                            manager = new DataManager();
                            ObservableCollection<Item> unloadItems = new ObservableCollection<Item>();
                            unloadItems = manager.GetSQLite_ItemtoUnload();
                            if(unloadItems!=null)
                            {
                                foreach (Item itm in unloadItems)
                                {
                                    manager.ResetSqlite_Invenotry(itm.ItemNo);
                                }
                                
                            }
                        }
                        retmsg = "Success";
                    }

                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    if (retmsg == "Success")
                    {
                        UserDialogs.Instance.ShowSuccess("Reset Transaction Data Successful!", 3000);
                    }
                    else
                    {
                        UserDialogs.Instance.ShowError(retmsg, 3000);
                    }
                }));
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
        }

        private void ResetMasterButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                DataManager manager = new DataManager();
                manager.resetMasterData();
                App.gCustomers = null;
                App.gItems = null;
               // manager.SaveSQLite_NumberSeries();
               // DependencyService.Get<IMessage>().ShortAlert("Reset Successful !");
                UserDialogs.Instance.ShowSuccess("Reset Master Data Successful!", 3000);

            }
            catch (Exception ex)
            {

                DependencyService.Get<IMessage>().ShortAlert(ex.Message.ToString());
            }
        }

        private void ResetInvButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                DataManager manager = new DataManager();
                ObservableCollection<Item> unloadItems = new ObservableCollection<Item>();
                unloadItems = manager.GetSQLite_ItemtoUnload();
               
                if (unloadItems != null)
                {
                    database.DeleteAll<VanItem>();
                    foreach (Item itm in unloadItems)
                    {
                        manager.ResetSqlite_Invenotry(itm.ItemNo);
                    }

                }
                UserDialogs.Instance.ShowSuccess("Reset Inventory Successful!", 3000);

            }
            catch (Exception ex)
            {

                DependencyService.Get<IMessage>().ShortAlert(ex.Message.ToString());
            }
        }
    }
}