using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace QHSalesApp
{
    public partial class App : Application
    {
        public static ServiceManager svcManager { get; set; }
        public static string gPageTitle { get; set; }
        public static string gUserID { get; set; }
        public static string gCustCode { get; set; }
        public static string gCustPriceGroup { get; set; }
        public static int gCompanyID { get; set; }

        public static string gCompanyName { get; set; }
        public static string gSalesPersonCode { get; set; }

        public static string gSalesPersonName { get; set; }
        public static bool IsConnected { get; set; }
        public static string gSOStatus { get; set; }
        public static string gPaymentStatus { get; set; }
        public static List<Item> glstItem { get; set; }
        public static string gItemFilter { get; set; }
        public static Customer gCustomer { get; set; }
        public static Item gItem { get; set; }
        public static string gfromMenu { get; set; }
        public static string gImageString {get;set;}
        public static string gDocType { get; set; }
        public static string gMEntryNo { get; set; }
        public static int gUserEntryNo { get; set; }
        public static string gRefDocNo { get; set; }
        public static string gSOPrefix { get; set; }
        public static string gCRPrefix { get; set; }
        public static string gCPPrefix { get; set; }
        public static string gRSPrefix { get; set; }
        public static string gULPrefix { get; set; }
        public static int gPaymentEntryNo { get; set; }
        public static string gPercentGST { get; set; }
        public static string gDeviceId { get; set; }
        public static string gRegGSTNo { get; set; }
        public static string gAdminPsw { get; set; }
        public static string gCurStatus { get; set; }
        public static List<Item> gItems { get; set; }
        public static List<Customer> gCustomers { get; set; }
        public static string gOnDate { get; set; }

        public static bool gIsExchange { get; set; }

        public static bool gIsEnableReasonCode { get; set; }
        
        public App()
        {
            InitializeComponent();
            IsConnected = true;
            MainPage = new NavigationPage(new LoginPage());
            //MainPage = new NavigationPage(new CustomerInfoPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
