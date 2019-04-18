using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadItemPage : ContentPage
    {
        private ObservableCollection<RequestLine> recItems { get; set; }
     
        private string HDRequestNo { get; set; }

        public LoadItemPage(string requstno)
        {
            InitializeComponent();
            MessagingCenter.Subscribe<App>((App)Application.Current, "OnLoadData", (sender) => {
                LoadData();
            });
            HDRequestNo = requstno;
            this.Title = App.gPageTitle;
            this.BackgroundColor = Color.FromHex("#dddddd");
            //listview.ItemTapped += Listview_ItemTapped;
            if (App.gCurStatus != "loaded")
            {
               // LoadButton.IsVisible = true;
                this.ToolbarItems.Add(new ToolbarItem { Text = "Load Confirm", Command = new Command(this.ConfirmLoad) });
            }
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        void ConfirmLoad()
        {
            try
            {
                string retmsg = string.Empty;
                DataManager manager = new DataManager();
                RequestHeader recHeader = new RequestHeader();
                List<RequestLine> reclines = new List<RequestLine>();

                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {
                    recHeader = await manager.GetSQLite_RequestHeadebyRequestNo(HDRequestNo);
                    if (recHeader != null)
                    {
                        reclines.Clear();
                        reclines = recItems.Where(x => x.HeaderEntryNo == recHeader.EntryNo).ToList();

                        if (reclines != null)
                        {
                            if (reclines.Count > 0)
                            {

                                string retStatus = retmsg;
                                foreach (RequestLine ln in reclines)
                                {
                                    // Step 3 -> Update loaded items quantity Inventory
                                    //Get current load Qty for loop item
                                    decimal ivnLoadQty = await manager.GetSQLite_ItemLoadedQty(ln.ItemNo);
                                    ivnLoadQty = ivnLoadQty + ln.PickQty;
                                    retmsg = await manager.UpdateSQLite_LoadInventory(ln.ItemNo, ivnLoadQty);

                                    RequestLine line = new RequestLine()
                                    {
                                        ID = ln.ID,
                                        EntryNo = ln.EntryNo,
                                        HeaderEntryNo = ln.HeaderEntryNo,
                                        ItemNo = ln.ItemNo,
                                        ItemDesc = ln.ItemDesc,
                                        QtyperBag = ln.QtyperBag,
                                        NoofBags = ln.NoofBags,
                                        Quantity = ln.Quantity,
                                        PickQty = ln.PickQty,
                                        LoadQty = ln.PickQty,
                                        SoldQty = ln.SoldQty,
                                        UnloadQty = ln.UnloadQty,
                                        UomCode = ln.UomCode,
                                        VendorNo = ln.VendorNo,
                                        RequestNo = ln.RequestNo,
                                        UserID = ln.UserID,
                                        InHouse = ln.InHouse,
                                        IsSync = "loaded",
                                        SyncDateTime = string.Empty
                                    };
                                    // Step 2 -> Update Request Line to mobile database
                                    string retval = await manager.SaveSQLite_RequestLine(line);

                                    

                                    VanItem ckitm = new VanItem();
                                    ckitm = await manager.GetSQLite_VanItembyItemNo(ln.ItemNo);
                                    if (ckitm != null)
                                    {
                                        decimal vanqty = ckitm.LoadQty + ln.PickQty;
                                      retmsg=  await manager.UpdateSQLite_VanItem(ln.ItemNo, vanqty);
                                    }
                                    else
                                    {
                                        Item itm = new Item();
                                        itm = manager.GetSQLite_ItembyItemNo(ln.ItemNo);

                                        VanItem vitm = new VanItem()
                                        {
                                            ID = 0,
                                            ItemNo = itm.ItemNo,
                                            Description = itm.Description,
                                            BarCode = itm.BarCode,
                                            BaseUOM = ln.UomCode,
                                            UnitPrice = itm.UnitPrice,
                                            Str64Img = itm.Str64Img,
                                            LoadQty = ln.PickQty,
                                            SoldQty = 0,
                                            ReturnQty = 0,
                                            BadQty = 0,
                                            UnloadQty = 0,
                                            Balance = 0
                                        };
                                     retmsg= await manager.SaveSQLite_VanItem(vitm);
                                    }

                                }

                                manager = new DataManager();
                                string rethead = await manager.SaveSQLite_RequestHeader(new RequestHeader
                                {
                                    ID = recHeader.ID,
                                    EntryNo = recHeader.EntryNo,
                                    SalesPersonCode = recHeader.SalesPersonCode,
                                    RequestNo = recHeader.RequestNo,
                                    RequestDate = recHeader.RequestDate,
                                    IsSync = recHeader.IsSync,
                                    SyncDateTime = recHeader.SyncDateTime,
                                    CurStatus = "loaded"
                                });
                                retmsg = "Success";
                            }
                        }
                        else
                        {
                            retmsg = "No requested lines to load!";
                        }
                    }
                    else
                    {
                        retmsg = "No data to load!";
                    }
                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    if(!string.IsNullOrEmpty(retmsg))
                    {
                        if (retmsg == "Success")
                        {
                            UserDialogs.Instance.ShowSuccess(retmsg, 3000);
                            Navigation.PushAsync(new MainPage(9));
                        }
                        else
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
        async Task LoadData()
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    recItems = new ObservableCollection<RequestLine>();
                    DataManager manager = new DataManager();
                    recItems = await manager.GetRequestLinesbyRequestNo(HDRequestNo);
                    Device.BeginInvokeOnMainThread(() =>
                    {

                        if (recItems != null)
                        {
                            listview.BeginRefresh();
                            listview.ItemsSource = recItems.OrderBy(x=>x.ItemDesc);
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

        private async void LoadButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            await LoadData();
        }

        private async void ChangeButton_Clicked(object sender, EventArgs e)
        {
            var item = (Button)sender;
            int id =int.Parse(item.CommandParameter.ToString());
            RequestLine obj = new RequestLine();
            obj = recItems.Where(x => x.ID == id ).FirstOrDefault();
           // if(obj.PickQty>0)
                Navigation.PushPopupAsync(new EditQtyPage(obj,"Load"));
        }
    }
}