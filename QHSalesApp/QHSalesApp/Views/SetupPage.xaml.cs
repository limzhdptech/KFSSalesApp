using Acr.UserDialogs;
using Java.IO;
using Java.Nio.Channels;
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
    public partial class SetupPage : ContentPage
    {
        private string deviceIdentifier { get; set; }
        readonly Database database;
        SvcConnection svc;

        public SetupPage()
        {
            InitializeComponent();
            var navipage = Application.Current.MainPage as NavigationPage;
            navipage.BarBackgroundColor = Color.Black;
            database = new Database(Constants.DatabaseName);
            database.CreateTable<User>();
            database.CreateTable<Setup>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSetting();
            
            NavigationPage.SetHasBackButton(this, true);
            IDevice device = DependencyService.Get<IDevice>();
            deviceIdentifier = device.GetIdentifier();
            Title = "Device Registration";
            this.BackgroundColor = Color.FromHex("#dddddd");
            RegisterButton.Clicked += RegisterButton_Clicked;
            BackupButton.Clicked += BackupButton_Clicked;
         
        }

        private void BackupButton_Clicked(object sender, EventArgs e)
        {
            // Automatically uses the implementation you defined in your platform project.
            var fileAccessHelper = DependencyService.Get<IFileHelper>();
            var dbFileName =Constants.DatabaseName+".db";

            //// In my code, the repository class creates the SQLite datbase and schema in the ctor using SQLiteAsyncConnection.
            //Repository yourSQliteRepository = new Repository(fileAccessHelper.GetLocalFilePath(dbFileName));

            // Now that the file is created, we can do a backup copy.
            var dbBackupFileName = $"DatabaseBackup{DateTime.Today.ToString("DD-MMM-YYYY")}.db";
            fileAccessHelper.CopyFile(dbFileName, dbBackupFileName, true);

            var fileExists = fileAccessHelper.IsDbFileExist(dbBackupFileName);
        }

        async void RegisterButton_Clicked(object sender, EventArgs e)
        {
            string retval = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(WebServiceUrlEntry.Text))
                {
                    if (!IsValidURL(WebServiceUrlEntry.Text.Trim()))
                    {
                        //DependencyService.Get<IMessage>().ShortAlert("Error : Web service URL is invalid!");
                        UserDialogs.Instance.ShowError("Error : Web service URL is invalid!", 3000);
                        return;
                    }

                    if (string.IsNullOrEmpty(MobileAccessKeyEntry.Text))
                    {
                        //DependencyService.Get<IMessage>().ShortAlert("Required Mobile Access Key !");
                        UserDialogs.Instance.ShowError("Required Mobile Access Key !", 3000);
                        return;
                    }

                    if (string.IsNullOrEmpty(EmailEntry.Text))
                    {
                        //DependencyService.Get<IMessage>().LongAlert("Email is blank!");
                        UserDialogs.Instance.ShowError("Email is blank!", 3000);
                        return;
                    }

                    var answer = DisplayAlert("Confirm", "Are you sure to register device?", "Yes", "No");
                    if (await answer)
                    {
                        DependencyService.Get<INetworkConnection>().CheckNetworkConnection();
                        if (DependencyService.Get<INetworkConnection>().IsConnected)
                        {
                            string result = DependencyService.Get<INetworkConnection>().IsServiceOnline(WebServiceUrlEntry.Text.Trim());
                            if (result != "true")
                            {
                                //DependencyService.Get<IMessage>().LongAlert("Error : Service is offline. [" + result + "]");
                                UserDialogs.Instance.ShowError("Error : Service is offline. [" + result + "]", 3000);
                                return;
                            }


                            string retmsg = App.svcManager.CheckDeviceAccessKey(MobileAccessKeyEntry.Text.Trim());
                            if (retmsg != "Success")
                            {
                                UserDialogs.Instance.ShowError("Mobile Access Key is wrong! Please contact to the administrator", 3000);
                                return;
                            }

                            // *** Save WebService Url and Mobile Access Key to Local ***
                            if (svc == null) svc = new SvcConnection() { svcUrl = WebServiceUrlEntry.Text.Trim(), deviceAccessKey = MobileAccessKeyEntry.Text.Trim() };
                            else
                            {
                                svc.svcUrl = WebServiceUrlEntry.Text.Trim();
                                svc.deviceAccessKey = MobileAccessKeyEntry.Text.Trim();
                            }

                            Helpers.Settings.GeneralSettings = svc.svcUrl;
                            Helpers.Settings.DeviceAccessKey = svc.deviceAccessKey;
                            string salesperson = App.svcManager.RetSalesPersonCode(EmailEntry.Text.Trim());
                            if (salesperson == "Failed")
                            {
                                UserDialogs.Instance.ShowError("Email does not exist! Please contact to the administrator", 3000);
                                return;
                            }

                            DataManager datamgr = new DataManager();
                            datamgr.CreateTables();
                            //ObservableCollection<RequestHeader> rqHead = new ObservableCollection<RequestHeader>();
                            if (datamgr.CheckRequestHeaderNotSync())
                            {
                                UserDialogs.Instance.ShowError("Please sync remaining request documents before device register.", 3000);
                                return;
                            }

                            if (datamgr.CheckSalesHeaderNotSync())
                            {
                                UserDialogs.Instance.ShowError("Please sync remaining order documents before device register.", 3000);
                                return;
                            }

                            if (datamgr.CheckUnloadHeaderNotSync())
                            {
                                UserDialogs.Instance.ShowError("Please sync remaining unload documents before device register.", 3000);
                                return;
                            }
                            retval = App.svcManager.DeviceRegister(MobileAccessKeyEntry.Text.Trim(), deviceIdentifier, EmailEntry.Text.Trim());
                            if (retval == "Success")
                            {
                                DataManager manager = new DataManager();
                                manager.SaveSQLite_Users(MobileAccessKeyEntry.Text);

                                User user = new User();
                                DataManager dm = new DataManager();
                                user = dm.LoadSQLite_UserbyEmail(EmailEntry.Text.Trim());
                                if (user == null)
                                {
                                    UserDialogs.Instance.ShowError("User Email not found!", 3000);
                                    return;
                                }

                                manager.SaveSQLite_Setup(deviceIdentifier);
                                // Number Series
                                Setup setup = new Setup();
                                setup = manager.GetSQLite_Setup();
                                if (setup != null)
                                {
                                    
                                    DataTable dt = new DataTable();
                                    dt = App.svcManager.RetNumSeries(deviceIdentifier, salesperson);
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

                                        numSeries.Add(new NumberSeries() { Code = App.gSOPrefix, Description = "SO", Increment = int.Parse(setup.Increment), LastNoCode = App.gSOPrefix + setup.StartNum, LastNoSeries = int.Parse(setup.StartNum) });
                                        numSeries.Add(new NumberSeries() { Code = App.gCRPrefix, Description = "CR", Increment = int.Parse(setup.Increment), LastNoCode = App.gCRPrefix + setup.StartNum, LastNoSeries = int.Parse(setup.StartNum) });
                                        numSeries.Add(new NumberSeries() { Code = App.gCPPrefix, Description = "CP", Increment = int.Parse(setup.Increment), LastNoCode = App.gCPPrefix + setup.StartNum, LastNoSeries = int.Parse(setup.StartNum) });
                                        numSeries.Add(new NumberSeries() { Code = App.gRSPrefix, Description = "RS", Increment = int.Parse(setup.Increment), LastNoCode = App.gRSPrefix + setup.StartNum, LastNoSeries = int.Parse(setup.StartNum) });
                                        numSeries.Add(new NumberSeries() { Code = App.gULPrefix, Description = "UL", Increment = int.Parse(setup.Increment), LastNoCode = App.gULPrefix + setup.StartNum, LastNoSeries = int.Parse(setup.StartNum) });

                                        App.svcManager.ExportNumSeries(deviceIdentifier, salesperson, App.gSOPrefix + setup.StartNum, App.gCRPrefix + setup.StartNum, App.gCPPrefix + setup.StartNum, App.gRSPrefix + setup.StartNum, App.gULPrefix + setup.StartNum, setup.StartNum, setup.StartNum, setup.StartNum, setup.StartNum, setup.StartNum);

                                        manager.SaveSQLite_NumberSeries(numSeries);
                                    }

                                }
                                Helpers.Settings.UserEmail = EmailEntry.Text;
                                UserDialogs.Instance.ShowSuccess("Device registered success!", 3000);
                            }
                            else
                                UserDialogs.Instance.ShowError(retval, 3000);
                            //DependencyService.Get<IMessage>().ShortAlert("Error : " + retval);


                        }
                        else
                            UserDialogs.Instance.ShowError("Error : No internet connection", 3000);
                        //DependencyService.Get<IMessage>().ShortAlert("Error : No internet connection");
                    }
                    { }
                }
                else
                    UserDialogs.Instance.ShowError("Required service Url!", 3000);
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            
            
        }

        private void LoadSetting()
        {
            string setting_url = string.Empty;
            string device_access_key = string.Empty;
            string email = string.Empty;
            try
            {
                setting_url = Helpers.Settings.GeneralSettings;
                device_access_key = Helpers.Settings.DeviceAccessKey;
                email = Helpers.Settings.UserEmail;
            }
            catch (System.NullReferenceException) { }
            catch (System.Collections.Generic.KeyNotFoundException) { }

            if (string.IsNullOrEmpty(setting_url))
            {
                //DependencyService.Get<IMessage>().ShortAlert("NAV URL not setup yet");
               // UserDialogs.Instance.ShowError("NAV URL not setup yet", 3000);
                WebServiceUrlEntry.Text = Constants.SoapUrl;
                MobileAccessKeyEntry.Text = string.Empty;
                EmailEntry.Text = string.Empty;
            }
            else
            {
                svc = new SvcConnection() { svcUrl = setting_url, deviceAccessKey = device_access_key };
                if (svc != null)
                {
                    WebServiceUrlEntry.Text = svc.svcUrl;
                    MobileAccessKeyEntry.Text = svc.deviceAccessKey;
                    EmailEntry.Text = email;
                }
            }
        }

        private bool IsValidURL(string url)
        {
            Uri uri = null;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri) || null == uri)
                return false;
            return true;
        }
    }
}