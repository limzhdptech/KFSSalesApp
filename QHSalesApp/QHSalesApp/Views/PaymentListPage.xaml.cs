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
    public partial class PaymentListPage : ContentPage
    {
        
        readonly Database database;
        //private ObservableCollection<PaymentList> listpayments { get; set; }
        private ObservableCollection<Payment> _payments { get; set; }
        private string recStatus{get;set;}
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

        public PaymentListPage()
        {
            InitializeComponent();
            DataLayout.IsVisible = false;
            Emptylayout.IsVisible = true;
            database = new Database(Constants.DatabaseName);
            this.BackgroundColor = Color.FromHex("#dddddd");
            recStatus = App.gPaymentStatus;
            this.Title = "Payment";
            if (recStatus=="Open")
                this.ToolbarItems.Add(new ToolbarItem { Text = "Released", Command = new Command(this.ChangeDocumentStatus) });
            else
                this.ToolbarItems.Add(new ToolbarItem { Text = "Open", Command = new Command(this.ChangeDocumentStatus) });
            sbSearch.Placeholder = "Search by Document No or Customer No or Name";
            sbSearch.TextChanged += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => SearchItemsFilter(sbSearch.Text);
            listview.ItemTapped += Listview_ItemTapped;
            IsLoading = false;
            BindingContext = this;
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindData();
            //if (listview.ItemsSource == null)
            //{
            //    DataLayout.IsVisible = false;
            //    EmptyLayout.IsVisible = true;
            //}
            //else
            //{
            //    DataLayout.IsVisible = true;
            //    EmptyLayout.IsVisible = false;
            //}
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(0));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private async Task AddButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            App.gPageTitle = "Add New Payment";
            App.gPaymentEntryNo = 0;
            Navigation.PushAsync(new PaymentPage());
        }

        private void SearchItemsFilter(string filter)
        {
            List<Payment> filterItems = new List<Payment>();
            if (_payments != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    listview.ItemsSource = _payments;

                }
                else
                {
                    filterItems = _payments.Where(x => x.CustomerNo.ToLower().Contains(filter.ToLower()) || x.DocumentNo.ToLower().Contains(filter.ToLower()) || x.CustomerName.ToLower().Contains(filter.ToLower())).ToList();
                    listview.ItemsSource = filterItems;
                }
            }

        }

        private void Listview_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
            var item = (Payment)e.Item;
            if (recStatus == "Open")
            {
                App.gPageTitle = "Edit Payment " + item.DocumentNo;
                App.gPaymentEntryNo = item.ID;
                Navigation.PushAsync(new PaymentPage());
            }
            else
            {
                App.gPageTitle = "Released Payment " + item.DocumentNo;
                Navigation.PushAsync(new ReleasedPaymentPage(item.ID));
            }
            
        }

        private void BindData()
        {
            DataManager manager = new DataManager();

            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {

                    _payments = new ObservableCollection<Payment>();

                    _payments = await manager.GetPaymentbyStatus(recStatus);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        // recItems.OrderByDescending(x => x.ID);
                        if(_payments!=null)
                        {
                            if (_payments.Count > 0)
                            {
                                listview.ItemsSource = _payments.OrderBy(x => x.ID);
                                DataLayout.IsVisible = true;
                                Emptylayout.IsVisible = false;
                            }
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            DataLayout.IsVisible = false;
                            Emptylayout.IsVisible = true;
                        }
                        
                       // listview.ItemsSource = _payments != null ? _payments.OrderBy(x=>x.ID) : null;
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
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
            });
            //IsLoading = true;
            //Task.Run(async () =>
            //{
            //    try
            //    {
            //        await BindRecords(string.Empty, DocStatus);
            //        Device.BeginInvokeOnMainThread(() =>
            //        {

            //            if (recItems != null)
            //            {

            //                if (DocStatus == "Open")
            //                    listview.ItemsSource = recItems.Where(s => s.Status == "Open").OrderBy(x => x.ID);
            //                else
            //                    listview.ItemsSource = recItems.Where(s => s.Status != "Open").OrderBy(x => x.ID);
            //            }
            //            else
            //                listview.ItemsSource = null;

            //            listview.Unfocus();
            //            IsLoading = false;
            //        });

            //    }
            //    catch (OperationCanceledException ex)
            //    {
            //        IsLoading = false;
            //        DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        IsLoading = false;
            //        DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
            //    }
            //});
        }

        private void ChangeDocumentStatus()
        {
            this.ToolbarItems.Clear();
            if (recStatus == "Open")
            {
                recStatus = "Released";
                this.ToolbarItems.Add(new ToolbarItem { Text = "Open", Command = new Command(this.ChangeDocumentStatus) });
                AddButton.IsVisible = false;
            }
            else
            {
                recStatus = "Open";
                this.ToolbarItems.Add(new ToolbarItem { Text = "Released", Command = new Command(this.ChangeDocumentStatus) });
                AddButton.IsVisible = true;
            }
            BindData();
        }
    }
}