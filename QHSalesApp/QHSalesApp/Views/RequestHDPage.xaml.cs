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
    public partial class RequestHDPage : ContentPage
    {
        private ObservableCollection<RequestHeader> recHeaders { get; set; }
        private bool _isEnableDetailBtn{get;set;}
        private bool _isEnableAddedBtn { get; set; }
        private bool _isEnableDeleteBtn { get; set; }
        public RequestHDPage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);
            NavBar.IsVisible = false;
            NavBox.IsVisible = false;
            this.BackgroundColor = Color.FromHex("#dddddd");
            DataLayout.IsVisible = false;
            Emptylayout.IsVisible = true;
            listview.ItemTapped += Listview_ItemTapped;

            sbSearch.Placeholder = "Search by Request No,Request Date";
            sbSearch.TextChanged += (sender2, e2) => FilterKeyword(sbSearch.Text);
            sbSearch.SearchButtonPressed += (sender2, e2) => FilterKeyword(sbSearch.Text);
            this.Title = "Request Stocks";
            App.gCurStatus = "request";
           // this.ToolbarItems.Add(new ToolbarItem { Text = "Requested List", Command = new Command(this.ChangeDocumentStatus) });

            BindingContext = this;
        }

        private void TapGestureRecognizer_Tapped_5(object sender, EventArgs e)
        {
            ChangeDocumentStatus();
        }
        private async void ChangeDocumentStatus()
        {
           // this.ToolbarItems.Clear();
            if (App.gCurStatus == "request")
            {
                NavigationPage.SetHasBackButton(this, false);
                NavigationPage.SetHasNavigationBar(this, false);
                NavBar.IsVisible = true;
                NavBox.IsVisible = true;
                App.gCurStatus = "topick";
                TitleLabel.Text  = "Requested List";
                AddButton.IsVisible = false;
                this.ToolbarItems.Clear();
               // this.ToolbarItems.Add(new ToolbarItem { Text = "Back", Command = new Command(this.ChangeDocumentStatus) });
            }
            else
            {
                App.gCurStatus = "request";
                this.Title = "Request Stocks";

                NavBar.IsVisible = false;
                NavBox.IsVisible = false;
                NavigationPage.SetHasBackButton(this, true);
                NavigationPage.SetHasNavigationBar(this, true);
                AddButton.IsVisible = true;
                this.ToolbarItems.Add(new ToolbarItem { Text = "Requested List", Command = new Command(this.ChangeDocumentStatus) });
            }
            await LoadData();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            _isEnableAddedBtn = true;
            _isEnableDeleteBtn = true;
            _isEnableDetailBtn = true;
            await LoadData();
        }

        private void DetailButton_Clicked(object sender, EventArgs e)
        {
            if(_isEnableDetailBtn)
            {
                _isEnableDetailBtn = false;
                var item = (Button)sender;
                App.gPageTitle = "Request Items";

                RequestHeader head = new RequestHeader();
                head = recHeaders.Where(x => x.ID == int.Parse(item.CommandParameter.ToString())).FirstOrDefault();
                Navigation.PushAsync(new RequestLinePage(head.EntryNo, head.RequestNo));
            } 
        }

        private async Task DeleteButton_Clicked(object sender, EventArgs e)
        {
            if(_isEnableDeleteBtn)
            {
                _isEnableDeleteBtn = false;
                var item = (Button)sender;

                var answer = await DisplayAlert("Confirmation", "Are you sure to delete?", "Yes", "No");
                if (answer)
                {
                    DataManager manager = new DataManager();
                    manager.DeleteRequestHeaderbyID(int.Parse(item.CommandParameter.ToString()));
                    await LoadData();
                   
                }
                _isEnableDeleteBtn = true;
            }
        }

        private void AddButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            if(_isEnableAddedBtn)
            {
                _isEnableAddedBtn = false;
                App.gPageTitle = "Add New Request Stock";
                Navigation.PushAsync(new RequestHDEntryPage(0));
            }
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
            var item = (RequestHeader)e.Item;
            if(App.gCurStatus=="request")
            {
                App.gPageTitle = "Edit Request Item (" + item.RequestNo + ")";
                Navigation.PushAsync(new RequestHDEntryPage(item.ID));
            }
             else
            {
                App.gPageTitle = "Items to Pick";
                Navigation.PushAsync(new RequestLinePage(item.EntryNo, item.RequestNo));
            }   
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
                listview.ItemsSource = recHeaders.Where(x => x.RequestNo.ToLower().Contains(filter.ToLower()) ||
                x.RequestDate.ToString().ToLower().Contains(filter.ToLower()));
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
                    recHeaders = new ObservableCollection<RequestHeader>();
                    DataManager manager = new DataManager();
                    string query;
                    if (App.gCurStatus == "request")
                    {
                         query = "WHERE CurStatus =?";
                    }
                    else
                    {
                         query = "WHERE CurStatus !=?";
                        
                    }
                    recHeaders = await manager.GetRequestHeaderbyQuery(query,"request");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (recHeaders != null)
                        {
                            if(recHeaders.Count>0)
                            {
                                DataLayout.IsVisible = true;
                                Emptylayout.IsVisible = false;
                                listview.ItemsSource = recHeaders.OrderByDescending(x => x.ID);
                            }
                            else
                            {
                                listview.ItemsSource = null;
                                DataLayout.IsVisible = false;
                                Emptylayout.IsVisible = true;
                            }
                            
                        }
                        else
                        {
                            listview.ItemsSource = null;
                            UserDialogs.Instance.ShowError("No Data", 3000);
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
    }
}