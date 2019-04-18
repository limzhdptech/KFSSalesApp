using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.FileUploader;
using Plugin.FileUploader.Abstractions;
using Plugin.Media;
using System.IO;
using Acr.UserDialogs;
using System.Windows.Input;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentPage : ContentPage
    {
        readonly Database database;
       
       string _SourceType { get; set; }
        List<PaymentMethod> paymethods { get; set; }
        List<PaymentReference> refList { get; set; }
        string prevRefDoc { get; set; }
        string DocumentNo { get; set; }
        string strImage { get; set; }
        string prevCustomerNo { get; set; }
        
        Queue<string> paths = new Queue<string>();
        string filePath = string.Empty;
        
        SelectMultipleBasePage<PaymentReference> multiPage;

        public PaymentPage()
        {
            InitializeComponent();
            
            this.BackgroundColor = Color.FromHex("#dddddd");
            database = new Database(Constants.DatabaseName);
            database.CreateTable<Payment>();
            database.CreateTable<PaidReference>();
            this.Title = App.gPageTitle;
            

            LoadPaymentMethods();

            DataManager manager = new DataManager();
            if (App.gPaymentEntryNo == 0)
            {
                DocumentNo = manager.GetLastNoSeries(App.gCPPrefix);
                DocumentNoLabel.Text = DocumentNo;
                RefNoEntry.Text = string.Empty;
                _SourceType = string.Empty;
                PaymentDatePicker.Date = DateTime.Today;

                if (PaymentMethodPicker.SelectedItem == null)
                {
                    PaymentMethodPicker.Items.Add("CASH");
                }
                PaymentMethodPicker.SelectedIndex = 0;
                ShowImageButtons(PaymentMethodPicker.SelectedItem.ToString());

                //Customer customer = new Customer();
                //customer = App.gCustomer;
                //CustomerNoLabel.Text = customer.CustomerNo;
                //CustomerNameLabel.Text = customer.Name;
                AmountEntry.Text = string.Format("{0:0.00}", 0);
                NoteEntry.Text = string.Empty;
                // isCustomerExisted = false;
                prevRefDoc = string.Empty;
                prevCustomerNo = string.Empty;
            }

            else
            {
                Payment payment = new Payment();
                payment = manager.GetPaymentbyID(App.gPaymentEntryNo);
                DocumentNo = payment.DocumentNo;
                DocumentNoLabel.Text = payment.DocumentNo;
                RefNoEntry.Text = payment.RefDocumentNo;
                prevRefDoc = payment.RefDocumentNo;
                _SourceType = payment.SourceType;
                PaymentDatePicker.Date = Convert.ToDateTime(payment.OnDate);
                ShowImageButtons(payment.PaymentMethod);
                CustomerNoEntry.Text = prevCustomerNo = payment.CustomerNo;
                CustomerNameLabel.Text = payment.CustomerName;

                // isCustomerExisted = true;
                App.gCustomer = new Customer();
                App.gCustomer = manager.GetSQLite_CustomerbyCustNo(payment.CustomerNo);
                PaymentMethodPicker.SelectedItem = payment.PaymentMethod;
                strImage = payment.Imagestr;
                if (!string.IsNullOrEmpty(strImage))
                {
                    ImageLinkLabel.Text = "Image attachment";
                    ImageLinkLabel.IsVisible = true;
                }
                else
                    ImageLinkLabel.IsVisible = false;
                AmountEntry.Text = string.Format("{0:0.00}", payment.Amount);
                NoteEntry.Text = payment.Note;
            }

            CustomerNoEntry.Completed += CustomerNoEntry_Completed;
            CustomerNoEntry.Unfocused += CustomerNoEntry_Unfocused;
            CustomerlookUpButton.ButtonIcon = AwasomeIcon.FASearch;
            CustomerlookUpButton.ButtonFontSize = 16;
            CustomerlookUpButton.ButtonColor = Color.FromHex("#EC2029");
            CustomerlookUpButton.OnTouchesEnded += CustomerlookUpButton_OnTouchesEnded;

            RefNolookUpButton.ButtonIcon = AwasomeIcon.FASearch;
            RefNolookUpButton.ButtonColor = Color.FromHex("#EC2029");
            RefNolookUpButton.OnTouchesEnded += RefNolookUpButton_OnTouchesEnded;

            AmountEntry.Completed += AmountEntry_Completed;
            AmountEntry.Unfocused += AmountEntry_Unfocused;

        }

        public static List<PaymentReference> SelectedData { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            

            //SalesPersonLabel.Text = App.gSalesPersonCode;
            string references = string.Empty;
            string refNos = string.Empty;

            if(multiPage!=null)
            {
                var result = multiPage.GetSelection();
                if(result.Count >0)
                {
                    int count = 1;
                    decimal amount = 0;
                    RefNoEntry.Text = string.Empty;
                    foreach (var a in result)
                    {
                        if (result.Count == 1) RefNoEntry.Text = a.DocumentNo;
                        else
                        {
                            if (count < result.Count)
                                RefNoEntry.Text += a.DocumentNo + ",";
                            else
                                RefNoEntry.Text += a.DocumentNo;
                        }
                        amount +=decimal.Parse(a.Amount);
                        count++;
                    }

                    AmountEntry.Text = string.Format("{0:0.00}", amount);
                }
                else
                {
                    RefNoEntry.Text = string.Empty;
                    AmountEntry.Text = string.Format("{0:0.00}", 0);
                }
            }
        }

        private  void RefNolookUpButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            try
            {
                if (!RefNolookUpButton.IsEnabled) return;
                RefNolookUpButton.IsEnabled = false;
               
                //var obj = (ActionButton)sender;
                //if(!string.IsNullOrEmpty(CustomerNoEntry.Text))
                //{
                //    var page = new RefNoLookupPage(CustomerNoEntry.Text);
                //   // page.listview.ItemSelected += RefListview_ItemSelected;
                //    Navigation.PushAsync(page);
                //}
                //else
                //    UserDialogs.Instance.ShowError("Choose Customer first!", 3000);0
                if (!string.IsNullOrEmpty(CustomerNoEntry.Text))
                {

                    var items = new List<PaymentReference>();
                    App.gRefDocNo = RefNoEntry.Text;
                    // 22-03-2018 -> add new code block to show loading
                    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));

                    Task.Run(async () =>
                    {
                        items = await GetReferenceData();

                    }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        if (multiPage == null)
                            multiPage = new SelectMultipleBasePage<PaymentReference>(items) { Title = "Document List" };
                        Navigation.PushAsync(multiPage);

                    }));
                    // end block -> to show loading

                    //App.gRefDocNo = RefNoEntry.Text;
                    //UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
                    //    items = await GetReferenceData();


                    //UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    //if (multiPage == null)
                    //    multiPage = new SelectMultipleBasePage<PaymentReference>(items) { Title = "Document List" };
                    //await Navigation.PushAsync(multiPage);
                }
                else
                    UserDialogs.Instance.ShowError("Choose Customer first!", 3000);
                RefNolookUpButton.IsEnabled = true;
               // UserDialogs.Instance.HideLoading(); //IsLoading = false;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                RefNolookUpButton.IsEnabled = true;
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
            }
        }

        private async Task<List<PaymentReference>> GetReferenceData()
        {
            //GetSQLite_CLEUnpaidBill
            DataManager manager = new DataManager();
           
            List<CustLedgerEntry> unpaidBills = new List<CustLedgerEntry>();
            unpaidBills = await manager.GetSQLite_CLEUnpaidBill(CustomerNoEntry.Text);
            //GetSQLite_SalesHeaders
            List<SalesHeader> salesHeaders = new List<SalesHeader>();
            salesHeaders = await manager.GetSQLite_SalesHeadersbyCustNo(CustomerNoEntry.Text);

            List<PaidReference> pfList = new List<PaidReference>();
            pfList = await manager.GetSQLite_PaidReference();

            List<PaymentReference> lstRef = new List<PaymentReference>();
            List<PaymentReference> retRef = new List<PaymentReference>();

            if (unpaidBills == null && salesHeaders == null)
            {
                lstRef = null;
            }
            else
            {
                if (unpaidBills != null)
                {
                    if (unpaidBills.Count > 0)
                    {
                        foreach (CustLedgerEntry c in unpaidBills)
                        {
                            if (pfList.Where(x => x.DocumentNo == c.DocNo).ToList().Count == 0)
                            {
                                lstRef.Add(new PaymentReference
                                {
                                    DocumentNo = c.DocNo,
                                    DocumentDate=c.TransDate,
                                    Amount = c.UnpaidAmount,
                                    SourceType = "CLE"
                                });
                            }
                               
                        }
                    }
                }

                if (salesHeaders != null)
                {
                    if (salesHeaders.Count > 0)
                    {
                        foreach (SalesHeader s in salesHeaders)
                        {
                            if (pfList.Where(x => x.DocumentNo == s.DocumentNo).ToList().Count == 0)
                            {
                                lstRef.Add(new PaymentReference
                                {
                                    DocumentNo = s.DocumentNo,
                                    DocumentDate=s.DocumentDate,
                                    Amount = string.Format("{0:0.00}", s.NetAmount),
                                    SourceType = "SO",
                                });
                            }                                 
                        }
                    }
                }
               
            }
            retRef = lstRef.OrderBy(x => x.DocumentDate).ThenByDescending(n=> n.DocumentNo).ToList();
            return retRef;
        }

        private void RefListview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            var selectedItem = e.SelectedItem as PaymentReference;

            RefNoEntry.Text = selectedItem.DocumentNo;
            _SourceType = selectedItem.SourceType;

            Navigation.PopAsync();
        }

        private void AmountEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (!string.IsNullOrEmpty(AmountEntry.Text))
            {
                //if(isNumeric(AmountEntry.Text,System.Globalization.NumberStyles.AllowDecimalPoint))
                //{
                AmountEntry.Text = string.Format("{0:0.00}", decimal.Parse(AmountEntry.Text));
                ContinueButton.Focus();
                // }
            }
            else
            {
                //DependencyService.Get<IMessage>().LongAlert("Empty Amount!");
                UserDialogs.Instance.ShowError("Empty Amount!", 3000);
                this.Focus();
            }
        }

        private void AmountEntry_Completed(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(AmountEntry.Text))
            {
                //if(isNumeric(AmountEntry.Text,System.Globalization.NumberStyles.AllowDecimalPoint))
                //{
                    AmountEntry.Text = string.Format("{0:0.00}", decimal.Parse(AmountEntry.Text));
                    ContinueButton.Focus();
               // }
            }
            else
            {
                //DependencyService.Get<IMessage>().LongAlert("Empty Amount!");
                UserDialogs.Instance.ShowError("Empty Amount!", 3000);
                this.Focus();
            }
        }

        private bool isNumeric(string val,System.Globalization.NumberStyles numStyle)
        {
            decimal result;
            return decimal.TryParse(val, numStyle, System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        private void ShowImageButtons(string method)
        {
            if (method != "CASH")
            {
                CameraButton.IsVisible = true;
                GallerydButton.IsVisible = true;
            }
            else
            {
                CameraButton.IsVisible = false;
                GallerydButton.IsVisible = false;
            }
        }

        private void CustomerlookUpButton_OnTouchesEnded(object sender, IEnumerable<NGraphics.Point> e)
        {
            var page = new CustomerPage();
            var obj = (ActionButton)sender;
            
            page.listview.ItemSelected += Listview_ItemSelected;
            Navigation.PushAsync(page);
        }

        private void CustomerNoEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (!string.IsNullOrEmpty(CustomerNoEntry.Text))
            {
                DataManager manager = new DataManager();
                Customer customer = new Customer();
                customer = manager.GetSQLite_CustomerbyCustNo(CustomerNoEntry.Text);
                if (customer != null)
                {
                    App.gCustomer = new Customer();
                    App.gCustomer = customer;
                    CustomerNoEntry.Text = customer.CustomerNo;
                    CustomerNameLabel.Text = customer.Name;
                    multiPage = null;
                    if (customer.CustomerNo != prevCustomerNo)
                    {
                        prevRefDoc = RefNoEntry.Text;
                        RefNoEntry.Text = string.Empty;
                        AmountEntry.Text = string.Format("{0:0.00}", 0);
                    }
                }
                else
                {
                    UserDialogs.Instance.ShowError("Wrong customer no or customer does not existed!", 3000);

                    //CustomerNoEntry.Text = string.Empty;
                    //CustomerNameLabel.Text = string.Empty;

                    CustomerNoEntry.Focus();
                }
            }
        }

        private void CustomerNoEntry_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CustomerNoEntry.Text))
            {
                DataManager manager = new DataManager();
                Customer customer = new Customer();
                customer = manager.GetSQLite_CustomerbyCustNo(CustomerNoEntry.Text);
                if (customer != null)
                {
                    App.gCustomer = new Customer();
                    App.gCustomer = customer;
                    CustomerNoEntry.Text = customer.CustomerNo;
                    CustomerNameLabel.Text = customer.Name;

                    multiPage = null;
                    if (customer.CustomerNo != prevCustomerNo)
                    {
                        prevRefDoc = RefNoEntry.Text;
                        RefNoEntry.Text = string.Empty;
                        AmountEntry.Text = string.Format("{0:0.00}", 0);
                    }
                }
                else
                {
                    UserDialogs.Instance.ShowError("Wrong customer no or customer does not existed!", 3000);

                    CustomerNoEntry.Text = string.Empty;
                    CustomerNameLabel.Text = string.Empty;

                    CustomerNoEntry.Focus();
                }
            }
        }

        private void Listview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            var selectedItem = e.SelectedItem as Customer;
            App.gCustomer = new Customer();
            App.gCustomer = selectedItem;

            CustomerNoEntry.Text = selectedItem.CustomerNo;
            CustomerNameLabel.Text = selectedItem.Name;
            multiPage = null;
            if (selectedItem.CustomerNo != prevCustomerNo)
            {
                prevRefDoc = RefNoEntry.Text;
                RefNoEntry.Text = string.Empty;
                AmountEntry.Text = string.Format("{0:0.00}", 0);
            }
            Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
           base.OnBackButtonPressed();
           Application.Current.MainPage = new NavigationPage(new MainPage(3));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private void LoadPaymentMethods()
        {
            DataManager manager = new DataManager();
            paymethods = new List<PaymentMethod>();
            paymethods = manager.GetSQLite_PaymentMethods();
            if (paymethods != null)
            {
                foreach (PaymentMethod pm in paymethods)
                {
                    PaymentMethodPicker.Items.Add(pm.Code);
                }
                PaymentMethodPicker.SelectedIndex = 1;
            }
        }

        private string DataValidate()
        {
            string retval = "Success";
            //if(string.IsNullOrEmpty(RefNoEntry.Text)) retval = "Required Reference No";
            if (string.IsNullOrEmpty(CustomerNoEntry.Text)) retval = "Required Customer No";
            
            else if (string.IsNullOrEmpty(AmountEntry.Text)) retval = "Amount can not be emptry";
            else if (decimal.Parse(AmountEntry.Text) < 1) retval = "Amount can not zero";
            return retval;
        }

        private async void ContinueButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string retmsg = DataValidate();
                if (retmsg == "Success")
                {
                    Payment payment = new Payment()
                    {
                        ID = App.gPaymentEntryNo,
                        DocumentNo = DocumentNo,
                        OnDate = PaymentDatePicker.Date.ToString("yyyy-MM-dd"),
                        CustomerNo = CustomerNoEntry.Text,
                        CustomerName = CustomerNameLabel.Text,
                        PaymentMethod = PaymentMethodPicker.SelectedItem.ToString(),
                        Amount = decimal.Parse(AmountEntry.Text),
                        CustomerSignature = string.Empty,
                        SalesPersonCode = App.gSalesPersonCode,
                        Note = NoteEntry.Text,
                        RecStatus = "Open",
                        Imagestr=strImage,
                        RefDocumentNo=RefNoEntry.Text,
                        SourceType= _SourceType
                    };
                   
                    DataManager manager = new DataManager();
                    Dictionary<int, string> dicResult = new Dictionary<int, string>();
                    dicResult= await manager.SaveSQLite_Payment(payment);
                    string retval = string.Empty;
                    int entryno = 0;
                    if (dicResult.Count>0)
                    {
                        foreach (KeyValuePair<int, string> pair in dicResult)
                        {
                            entryno = pair.Key;
                            retval = pair.Value;
                        }
                    }
                    
                    if (retval== "Success")
                    {
                        // if RefNoEntry is not blank Save to PaidReference table.
                       // if (!string.IsNullOrEmpty(RefNoEntry.Text.ToString()))
                       // {
                            //if(!string.IsNullOrEmpty(prevRef))
                            //{
                               // string[] prevNos = prevRef.Split(',');
                                //foreach (string n in prevNos)
                                //{
                                    //string retpaid = manager.DeletePaidReferencebyDocNo(n);
                                //}
                            //}

                            //refList = new List<PaymentReference>();
                            //refList = await GetReferenceData();

                            //string[] refnos = RefNoEntry.Text.Split(',');
                            //foreach(string r in refnos)
                            //{
                            //    PaymentReference pf = new PaymentReference();
                            //    pf = refList.Where(x => x.DocumentNo == r).FirstOrDefault();
                            //    if(pf!=null)
                            //    {
                            //        string retpaid = await manager.SaveSQLite_PaidReference(new PaidReference { DocumentNo = pf.DocumentNo, Amount = pf.Amount, SourceType = pf.SourceType });
                            //    }   
                            //}
                        //}

                        if (App.gPaymentEntryNo==0)
                        {
                            manager.IncreaseNoSeries(App.gCPPrefix, DocumentNo, "CP");
                        }
                        App.gPaymentEntryNo = entryno;   
                        Navigation.PushAsync(new PaymentConfirmPage(payment));
                    }
                    //DependencyService.Get<IMessage>().LongAlert(retval);
                    UserDialogs.Instance.ShowSuccess(retval, 3000);
                }
                else
                    UserDialogs.Instance.ShowError(retmsg, 3000);
                //DependencyService.Get<IMessage>().LongAlert("Fail");
            }
            catch (Exception ex)
            {

                //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            
        }

        private void PaymentMethodPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowImageButtons(PaymentMethodPicker.SelectedItem.ToString());
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", "No camera available.", "OK");
                    return;
                }
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    Directory = "Sample",
                    Name = "test.jpg"
                });

                if (file == null)
                    return;
                filePath = file.Path;
                paths.Enqueue(filePath);

                //image.Source = ImageSource.FromStream(() =>
                //{
                //    var stream = file.GetStream();
                //    file.Dispose();
                //    return stream;
                //});

                using (var memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    file.Dispose();
                    byte[] bytes = memoryStream.ToArray();
                    strImage = Convert.ToBase64String(bytes);
                    if (!string.IsNullOrEmpty(strImage))
                    {
                        ImageLinkLabel.Text = "Image attachment";
                        ImageLinkLabel.IsVisible = true;
                    }
                    else
                        ImageLinkLabel.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Get Image Error", ":(" + ex.Message.ToString(), "OK");
                
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            App.gPageTitle = "View Image";
            Navigation.PushAsync(new ImageViewerPage(strImage, DocumentNoLabel.Text));
        }

        private async void GallerydButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    return;
                }
                var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });


                if (file == null)
                    return;

                filePath = file.Path;
                paths.Enqueue(filePath);

                //image.Source = ImageSource.FromStream(() =>
                //{
                //    var stream = file.GetStream();
                //    //  file.Dispose();
                //    return stream;
                //});

                using (var memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    file.Dispose();
                    byte[] bytes = memoryStream.ToArray();

                    strImage = Convert.ToBase64String(bytes);
                    if (!string.IsNullOrEmpty(strImage))
                    {
                        ImageLinkLabel.Text = "Image attachment";
                        ImageLinkLabel.IsVisible = true;
                    }
                    else
                        ImageLinkLabel.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Get Image Error", ":(" + ex.Message.ToString(), "OK");
            }
        }
    }
}