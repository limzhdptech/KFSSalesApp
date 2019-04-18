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
    public partial class ItemDetailPage : ContentPage
    {
        Item item { get; set; }
        public ItemDetailPage()
        {
            InitializeComponent();
           // NavigationPage.SetHasBackButton(this, false);
           // NavigationPage.SetHasNavigationBar(this, false);
           // this.ToolbarItems.Add(new ToolbarItem { Text = "Back", Command = new Command(this.BackPage) });
            this.Title = "Item Detail";
           // TitleLabel.Text = "Item Detail";
            this.BackgroundColor = Color.FromHex("#dddddd");
            item = new Item();
            item = App.gItem;
            
        }

        void BackPage()
        {
            Navigation.PushAsync(new MainPage(6));
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(6));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (item != null)
            {
                ItemNoLabel.Text = item.ItemNo;
                DescLabel.Text = item.Description;
                Desc2Label.Text = item.Description2;
                DataManager manager = new DataManager();
                List<SalesPrice> lstPrice = new List<SalesPrice>();
                lstPrice=manager.GetItemPricebyItemPriceGroup(item.ItemNo, App.gCustPriceGroup);
                if(lstPrice!=null)
                {
                    if(lstPrice.Count>0)
                    {
                        UnitPriceLabel.Text = lstPrice.FirstOrDefault().UnitPrice.ToString();
                    }
                    else
                        UnitPriceLabel.Text = item.UnitPrice;
                }
                else
                    UnitPriceLabel.Text = item.UnitPrice;
                BalQtyLabel.Text = item.InvQty.ToString() + " " + item.BaseUOM;
            }
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ItemDetailPage());
        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            var page = new CustomerPage();
            page.listview.ItemSelected += ItemListview_ItemSelected;
            Navigation.PushAsync(page);

            //Navigation.PushAsync(new ItemPurHistoryPage());
            
        }


        private void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ItemLocationPage());
        }

        private void TapGestureRecognizer_Tapped_4(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(6));
        }
        private void ItemListview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
           
            var selectedItem = e.SelectedItem as Customer;
            App.gCustomer = selectedItem;
            //UnitPricePicker.IsEnabled = true;
            Navigation.PushAsync(new CustPriceHisPage());
        }
    }
}