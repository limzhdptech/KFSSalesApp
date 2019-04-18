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
    public partial class HomePage : ContentPage
    {
        readonly Database database;
        public HomePage()
        {
            InitializeComponent();
            this.Title = "KFS Mobile Sales";
            //this.ToolbarItems.Add(new ToolbarItem { Text = "Logout",Icon="exit.png", Command = new Command(this.OnLogout) });
            //Code + EntryNo + Sales Person code + Series
            App.gIsExchange = false;
            Setup setup = new Setup();
            DataManager manager = new DataManager();
            setup = manager.GetSQLite_Setup();

            string codePart = App.gSalesPersonCode; //App.gUserEntryNo.ToString();

            App.gSOPrefix = setup.SOPrefix+codePart;
            App.gCRPrefix = setup.CRPrefix+codePart ;
            App.gCPPrefix = setup.CPPrefix+codePart;
            App.gRSPrefix = setup.RSPrefix + codePart;
            App.gULPrefix = setup.ULPrefix + codePart;
            database = new Database(Constants.DatabaseName);

            RequestStockButton.Clicked += RequestStockButton_Clicked;
            LoadStockButton.Clicked += LoadStockButton_Clicked;
            SalesOrderButton.Clicked += SalesOrderButton_Clicked;
            CreditMemoButton.Clicked += CreditMemoButton_Clicked;
            UnloadStockButton.Clicked += UnloadStockButton_Clicked;
            SyncDataButton.Clicked += SyncDataButton_Clicked;
            CustomerButton.Clicked += CustomerButton_Clicked;
            ItemsButton.Clicked += ItemsButton_Clicked;
            BluetoothSettingButton.Clicked += BluetoothSettingButton_Clicked;
            LogoutButton.Clicked += LogoutButton_Clicked;
        }

        private async void LogoutButton_Clicked(object sender, EventArgs e)
        {
            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = "logout",
                Message = "Are you sure to logout?",
                CancelText = "No",
                OkText = "Yes"
            });
            //  var answer = await DisplayAlert("Logout", "Are you sure to logout?", "Yes", "No");
            if (result)
            {
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        private void BluetoothSettingButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(12));
        }

        private void ItemsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(6));
        }

        private void CustomerButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(5));
        }

        private void SyncDataButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(11));
        }

        private void UnloadStockButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(10));
        }

        private void CreditMemoButton_Clicked(object sender, EventArgs e)
        {
            App.gSOStatus = "Open";
            App.gPaymentStatus = "Open";
            App.gDocType = "CN";
            Navigation.PushAsync(new MainPage(4));
        }

        private void SalesOrderButton_Clicked(object sender, EventArgs e)
        {
            App.gSOStatus = "Open";
            App.gPaymentStatus = "Open";
            App.gDocType = "SO";
            Navigation.PushAsync(new MainPage(1));
        }

        private void LoadStockButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(9));
        }

        private void RequestStockButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(8));
        }

        private void OnLogout()
        {
            Task.Run(async () =>
            {
                var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Title = "logout",
                    Message = "Are you sure to logout?",
                    CancelText = "No",
                    OkText = "Yes"
                });
                //  var answer = await DisplayAlert("Logout", "Are you sure to logout?", "Yes", "No");
                if (result)
                {
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                }
            });  
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            DataManager manager = new DataManager();
            manager.CreateTables();

            //// Number Series
            //database.CreateTable<NumberSeries>();
            //List<NumberSeries> numList = new List<NumberSeries>();
            //numList = await manager.GetSQLite_NumberSeries();
            //if (numList!=null)
            //{
            //    if(numList.Count==0)
            //    {
            //        ObservableCollection<NumberSeries> numSeries = new ObservableCollection<NumberSeries>();
            //        numSeries.Add(new NumberSeries() { Code = App.gSOPrefix, Description = "Sales Order", Increment = 1, LastNoCode = App.gSOPrefix + "-10000", LastNoSeries = 10000 });
            //        numSeries.Add(new NumberSeries() { Code = App.gCRPrefix, Description = "Credit Memo", Increment = 1, LastNoCode = App.gCRPrefix + "-10000", LastNoSeries = 10000 });
            //        numSeries.Add(new NumberSeries() { Code = App.gCPPrefix, Description = "Customer Payment", Increment = 1, LastNoCode = App.gCPPrefix + "-10000", LastNoSeries = 10000 });
            //        manager.SaveSQLite_NumberSeries(numSeries);
            //    }               
            //} 
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Exit page?", "Are you sure you want to exit this page? You will not be able to continue it.", "Yes", "No"))
                {
                    base.OnBackButtonPressed();

                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                }
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }
    }
}