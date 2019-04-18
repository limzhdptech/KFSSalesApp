using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public List<MasterPageItem> masterPageItems { get; set; }
        public MainPage(int page)
        {
            InitializeComponent();
            this.BackgroundColor = Color.FromHex("#dddddd");
            masterPageItems = new List<MasterPageItem>();
                
                    masterPageItems.Add(new MasterPageItem
                    {
                        Title = "Home",
                        IconSource="home.png",
                        TargetType = typeof(HomePage)
                        
                    });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Request Stock",
                IconSource = "request.png",
                TargetType = typeof(RequestHDPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Load Stock",
                IconSource = "load.png",
                TargetType = typeof(LoadHDPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Sales Order",
                IconSource = "cart.png",
                TargetType = typeof(SalesHeaderPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Credit Memo",
                IconSource = "favourite.png",
                TargetType = typeof(SalesHeaderPage)

            });

            //masterPageItems.Add(new MasterPageItem
            //{
            //    Title = "Payment",
            //    IconSource = "money.png",
            //    TargetType = typeof(PaymentListPage)

            //});

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Unload Stock",
                IconSource = "unload.png",
                TargetType = typeof(UnloadItemPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Report",
                IconSource = "item.png",
                TargetType = typeof(ReportPage)

            });
            //masterPageItems.Add(new MasterPageItem
            //{
            //    Title = "Unload Return",
            //    IconSource = "box.png",
            //    TargetType = typeof(UserListPage)

            //});

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Customers",
                IconSource = "customer.png",
                TargetType = typeof(CustomerInfoPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Items",
                IconSource = "barcode.png",
                TargetType = typeof(ItemInfoPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Check Inventory",
                IconSource = "invbox.png",
                TargetType = typeof(VanInventoryPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Sync Data",
                IconSource = "sync.png",
                TargetType = typeof(SyncPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Bluetooth Setting",
                IconSource = "bluetooth.png",
                TargetType = typeof(BTPairPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Reset Data",
                IconSource = "reload.png",
                TargetType = typeof(AdminLoginPage)

            });

            masterPageItems.Add(new MasterPageItem
                {
                    Title = "Logout",
                    IconSource = "exit.png",
                    TargetType = typeof(LoginPage)
                });

                // Setting our list to be ItemSource for ListView in MainPage.xaml
                navigationDrawerList.ItemsSource = masterPageItems;


            // Initial navigation, this can be used for our home page
            //Page displayPage = (Page)Activator.CreateInstance(typeof(PickPage));
            //Detail.Navigation.PushAsync(displayPage);
            if(page==0)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(HomePage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Home" };
            }    
            else if(page ==1)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(SalesHeaderPage))){ BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Sales Order" };
            else if(page==2)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(ReleaseOrderPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Released Order" };
            else if (page == 3)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(PaymentListPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Payment" };
            else if (page == 4)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(SalesHeaderPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Credit Memo" };
            else if (page == 5)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(CustomerInfoPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Customer List" };
            else if (page == 6)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(ItemInfoPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Item List" };
            else if (page == 7)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(ResetDataPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Reset Data" };
            else if (page == 8)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(RequestHDPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Request Stock" };
            else if (page == 9)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(LoadHDPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Load Stock" };
            else if (page == 10)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(UnloadItemPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Unload Stock" };
            else if (page == 11)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(SyncPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Unload Stock" };
            else if (page == 12)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(BTPairPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Unload Stock" };
            else if (page == 13)
                Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(ReportPage))) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = "Report" };

            //Navigation.PushAsync(new SalesHeaderPage());
            //if (Device.OS == TargetPlatform.Windows)
            //{
            //    Master.Icon = "swap.png";
            //}

            //ListViewMenu.ItemSelected += async (sender, e) =>
            //{
            //    if (ListViewMenu.SelectedItem == null) return;
            //    if (((HomeMenuItem)e.SelectedItem).MenuType == MenuType.Logout)
            //    {
            //        await OnLogout();
            //    }
            //    else
            //        await this.root.NavigateAsync(((HomeMenuItem)e.SelectedItem).MenuType);
            //};
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        //protected  override  bool OnBackButtonPressed()
        //{
        //    Device.BeginInvokeOnMainThread(async() =>
        //    {
        //        if (await DisplayAlert("Exit page?", "Are you sure you want to exit this page? You will not be able to continue it.", "Yes", "No"))
        //        {
        //            base.OnBackButtonPressed();

        //            Application.Current.MainPage = new NavigationPage(new LoginPage());
        //        }
        //    });

        //    // Always return true because this method is not asynchronous.
        //    // We must handle the action ourselves: see above.
        //    return true;
        //}

        private async void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (MasterPageItem)e.SelectedItem;
            Type page = item.TargetType;
            App.gSOStatus = "Open";
            App.gPaymentStatus = "Open";
            App.gfromMenu = item.Title;
            if (item.Title == "Logout")
            {
                await OnLogout();
            }
            else
            {
                if (item.Title == "Credit Memo") App.gDocType = "CN";
                if (item.Title == "Sales Order") App.gDocType = "SO";
                Detail = new NavigationPage((Page)Activator.CreateInstance(page)) { BarBackgroundColor = Color.Black, BarTextColor = Color.White, Title = item.Title };
                IsPresented = false;
            }
        }

        private async Task OnLogout()
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
    }
}