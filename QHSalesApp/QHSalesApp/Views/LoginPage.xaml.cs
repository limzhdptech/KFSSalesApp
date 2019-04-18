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
    public partial class LoginPage : ContentPage
    {
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

        public LoginPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            //PasswordEntry.Text = "123";
            LoginButton.Clicked += LoginButton_Clicked;
            UserEntry.Completed += UserEntry_Completed;
            PasswordEntry.Completed += PasswordEntry_Completed;
            IsLoading = false;
            BindingContext = this;
        }

        private void PasswordEntry_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PasswordEntry.Text))
            {
                LoginButton.Focus();
            }
            else
            {
                //DependencyService.Get<IMessage>().LongAlert("Password is blank!");
                UserDialogs.Instance.ShowError("Password is blank!", 3000);
                PasswordEntry.Focus();
            }
        }

        private void UserEntry_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(UserEntry.Text))
            {
                Helpers.Settings.UserEmail = UserEntry.Text;
                PasswordEntry.Focus();
            }
            else
            {
                //DependencyService.Get<IMessage>().LongAlert("User Email is blank!");
                UserDialogs.Instance.ShowError("User Email is blank!", 3000);
                UserEntry.Focus();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            VersionLabel.Text = Constants.ApkVersion;
            if (Helpers.Settings.UserEmail != null)
                UserEntry.Text = Helpers.Settings.UserEmail;
            IDevice device = DependencyService.Get<IDevice>();
           string  deviceIdentifier = device.GetIdentifier();
            ForgetLabel.Text = deviceIdentifier;
        }

        private void SetupButton_Clicked(object sender, EventArgs e)
        {
            //  Application.Current.MainPage =  new NavigationPage(new SetupPage());
            Navigation.PushAsync(new SetupPage());
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {

            string email = string.Empty;
            string url = string.Empty;
            string accesskey = string.Empty;
            IDevice device = DependencyService.Get<IDevice>();
            
            string deviceId = device.GetIdentifier();
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            try
            {
                
                //using (UserDialogs.Instance.Loading("Loading...", null, null, true, MaskType.Black))
                //{
                //    await Task.Delay(500);
                //}
                    url = Helpers.Settings.GeneralSettings;
                accesskey = Helpers.Settings.DeviceAccessKey;
                email = Helpers.Settings.UserEmail;
            }
            catch (System.NullReferenceException)
            {
                //IsLoading = false;
                UserDialogs.Instance.HideLoading();
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            { //IsLoading = false;  
                UserDialogs.Instance.HideLoading();
            }

                if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(accesskey))
            {
                //IsLoading = false;
                UserDialogs.Instance.HideLoading();
                // DependencyService.Get<IMessage>().LongAlert("Please setup the web service connection and mobile access key first");
                UserDialogs.Instance.ShowError("Please setup the web service connection and mobile access key first", 3000);
            }
            else
            {
                if(string.IsNullOrEmpty(UserEntry.Text))
                {
                    //IsLoading = false;
                    UserDialogs.Instance.HideLoading();
                    //DependencyService.Get<IMessage>().LongAlert("Email is blank!");
                    UserDialogs.Instance.ShowError("Email is blank!", 3000);
                    return;
                }

                if (string.IsNullOrEmpty(PasswordEntry.Text))
                {
                    //IsLoading = false;
                    UserDialogs.Instance.HideLoading();
                    //DependencyService.Get<IMessage>().LongAlert("Password is blank!");
                    UserDialogs.Instance.ShowError("Password is blank!", 3000);
                    return;
                }
                //string retval = App.svcManager.CheckUserLogin(accesskey,deviceId,UserEntry.Text, PasswordEntry.Text);
                User user = new User();
                DataManager manager = new DataManager();
                user = manager.LoadSQLite_User(UserEntry.Text, PasswordEntry.Text);
                if (user != null)
                {
                    if(user.DeviceID!= deviceId)
                    {
                        UserDialogs.Instance.HideLoading();
                        UserDialogs.Instance.ShowError("No permission to use this device!", 3000);
                        return;
                    }

                    if (string.IsNullOrEmpty(email))
                    {
                        Helpers.Settings.UserEmail = UserEntry.Text;
                    }
                    else
                    {
                        Helpers.Settings.UserEmail = email;
                    }

                    Task.Run(async () =>
                    {
                        try
                        {
                            //DataTable dt = new DataTable();
                            //dt = await App.svcManager.RetLoginData(UserEntry.Text, PasswordEntry.Text);
                            //if (dt.Rows.Count > 0)
                            //{
                            App.gUserEntryNo = user.EntryNo;
                            App.gUserID = user.UserID;
                            App.gCompanyID = user.Default_CustEntryNo;
                            App.gPageTitle = "Home";
                            App.gSalesPersonCode = user.SalesPersonCode;
                            App.gSalesPersonName = user.Name;
                            App.gCompanyName = Constants.CompanyName;
                            App.gItems = new List<Item>();
                            App.gCustomers = new List<Customer>();
                            App.gItems = await manager.GetSQLite_Items(); ;
                            App.gCustomers = await manager.GetSQLite_Customers(); ;
                            Setup setup = new Setup();
                            setup = manager.GetSQLite_Setup();
                            if (setup != null)
                            {
                                App.gPercentGST = setup.GSTPercent;
                                App.gRegGSTNo = setup.GSTRegNo;
                                App.gDeviceId = setup.DeviceId;
                                App.gAdminPsw = setup.AdminPsw;
                            }  
                            else
                            {
                                App.gPercentGST = "0";
                                App.gRegGSTNo = "";
                            }
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PushAsync(new MainPage(0));
                                UserDialogs.Instance.HideLoading();
                                //IsLoading = false;
                            });

                            //}
                            //else
                            //{
                            //    IsLoading = false;
                            //    DependencyService.Get<IMessage>().LongAlert("Required CompanyID!");

                            //}

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
                else
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    //DependencyService.Get<IMessage>().LongAlert("Wrong email or password!");
                    UserDialogs.Instance.ShowError("Wrong email or password!", 3000);
                }
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //DependencyService.Get<IMessage>().LongAlert("Forget password!");
            UserDialogs.Instance.ShowError("Forget password!", 3000);
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SetupPage());
        }
    }
}