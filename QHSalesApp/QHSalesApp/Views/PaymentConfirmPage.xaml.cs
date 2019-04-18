using Acr.UserDialogs;
using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentConfirmPage : ContentPage
    {
        private Payment record { get; set; }

        decimal GSTPercent { get; set; }
        decimal TotalAmount { get; set; }
        decimal GSTAmount { get; set; }
        decimal NetAmount { get; set; }

        public PaymentConfirmPage(Payment payment)
        {
            InitializeComponent();
            this.BackgroundColor = Color.FromHex("#dddddd");
            this.Title = "Payment Confirmation";
            record = new Payment();
            record = payment;
            Customer customer = new Customer();
            customer = App.gCustomer;
            if (customer != null)
            {
                CustomerNameLabel.Text = customer.Name;
                Line1Label.Text = customer.Address;
                string seperator = string.Empty;
                if ((!string.IsNullOrEmpty(customer.MobileNo)) && (!string.IsNullOrEmpty(customer.MobileNo)))
                {
                    seperator = " ,";
                }
                Line2Label.Text = customer.MobileNo + seperator + customer.PhoneNo;
            }
            
        }

        public decimal CalculateExclusiveGSTAmount(decimal amount)
        {

            //decimal SubTotal = decimal.Parse(lines.Sum(x => x.LineAmount).ToString());
            //decimal GSTAmount = (SubTotal * 7) / 100;
            decimal retamt = 0;
            decimal gstinclusiveprice = 100 + decimal.Parse(App.gPercentGST);
            retamt = amount * 100;
            retamt = retamt / gstinclusiveprice;

            return retamt;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            NetAmount = record.Amount;
            TotalAmount = CalculateExclusiveGSTAmount(NetAmount);
            GSTAmount = NetAmount - TotalAmount;
            
           // NetAmount = TotalAmount + GSTAmount;

            //NetAmountLabel.Text = string.Format("Included GST Amount : {0:0.00}", NetAmount);
            
            //GSTAmountLabel.Text = string.Format("GST" + App.gPercentGST + "% : {0:0.00}", GSTAmount);
            TotalAmountLabel.Text = string.Format("Total : {0:0.00}", NetAmount);
        }
        private async Task<List<PaymentReference>> GetReferenceData(string custno)
        {
            //GetSQLite_CLEUnpaidBill
            DataManager manager = new DataManager();

            List<CustLedgerEntry> unpaidBills = new List<CustLedgerEntry>();
            unpaidBills = await manager.GetSQLite_CLEUnpaidBill(custno);
            //GetSQLite_SalesHeaders
            List<SalesHeader> salesHeaders = new List<SalesHeader>();
            salesHeaders = await manager.GetSQLite_SalesHeadersbyCustNo(custno);

            List<PaymentReference> lstRef = new List<PaymentReference>();

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

                if (salesHeaders != null)
                {
                    if (salesHeaders.Count > 0)
                    {
                        foreach (SalesHeader s in salesHeaders)
                        {
                            lstRef.Add(new PaymentReference
                            {
                                DocumentNo = s.DocumentNo,
                                DocumentDate=s.DocumentDate,
                                Amount = s.NetAmount.ToString(),
                                SourceType = "SO",
                            });
                        }
                    }
                }
            }

            return lstRef;
        }

        private async void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Confirm Payment", "Are you sure to confirm payment?", "Yes", "No");
            if (answer)
            {
                string _signature64str=string.Empty;
                try
                {
                    var signedImageStream = await CustomerSignaturePad.GetImageStreamAsync(SignatureImageFormat.Png);
                    if(signedImageStream is MemoryStream)
                    {
                        var signatureMemoryStream = signedImageStream as MemoryStream;
                        byte[] bytes = signatureMemoryStream.ToArray();
                        _signature64str = Convert.ToBase64String(bytes);
                    }
                }
                catch (ArgumentException)
                {
                    _signature64str = string.Empty;
                }
               


                DataManager manager = new DataManager();
                Dictionary<int, string> dicResult = new Dictionary<int, string>();
                dicResult = await manager.SaveSQLite_Payment(new Payment
                {
                    ID = record.ID,
                    DocumentNo = record.DocumentNo,
                    OnDate = record.OnDate,
                    CustomerNo = record.CustomerNo,
                    CustomerName = record.CustomerName,
                    PaymentMethod = record.PaymentMethod,
                    Amount = record.Amount,
                    CustomerSignature = _signature64str,
                    SalesPersonCode = record.SalesPersonCode,
                    Note = record.Note,
                    Imagestr = record.Imagestr,
                    RecStatus = "Released",
                    RefDocumentNo = record.RefDocumentNo,
                    SourceType = record.SourceType,
                    //IsSync = "false",
                    //SyncDateTime = string.Empty
                });
                string retval = string.Empty;
                int entryno = 0;
                if (dicResult.Count > 0)
                {
                    foreach (KeyValuePair<int, string> pair in dicResult)
                    {
                        entryno = pair.Key;
                        retval = pair.Value;
                    }
                }
                if (retval == "Success")
                {
                    if(!string.IsNullOrEmpty(record.RefDocumentNo))
                    {
                        List<PaymentReference>  refList = new List<PaymentReference>();
                        refList = await GetReferenceData(record.CustomerNo);
                        string[] refnos = record.RefDocumentNo.Split(',');
                        foreach (string r in refnos)
                        {
                            PaymentReference pf = new PaymentReference();
                            pf = refList.Where(x => x.DocumentNo == r).FirstOrDefault();
                            if (pf != null)
                            {
                                string retpaid = await manager.SaveSQLite_PaidReference(new PaidReference { DocumentNo = pf.DocumentNo, Amount =decimal.Parse(pf.Amount), SourceType = pf.SourceType });
                            }
                        }
                    }
                    
                    //DependencyService.Get<IMessage>().LongAlert(retval);
                    UserDialogs.Instance.ShowSuccess(retval, 3000);
                    App.gPaymentStatus = "Released";
                    //var navipages = Navigation.NavigationStack.ToList();
                    //if(navipages !=null)
                    //{
                    //    foreach (var pg in navipages)
                    //    {
                    //        Navigation.RemovePage(pg);
                    //    }
                    //}
                    App.gPaymentEntryNo = 0;
                    Navigation.PushAsync(new MainPage(3));
                }
                else
                {
                    //DependencyService.Get<IMessage>().LongAlert(retval);
                    UserDialogs.Instance.ShowError(retval, 3000);
                }
            }
        }

        private async void PrintButton_Clicked(object sender, EventArgs e)
        {

            var answer = await DisplayAlert("Print Payment", "Are you sure to print payment?", "Yes", "No");
            if (answer)
            {
                DataManager manager = new DataManager();
                DeviceInfo info = new DeviceInfo();
                info = await manager.GetDeviceInfo();
                if (info != null)
                {
                    if (!string.IsNullOrEmpty(info.DeviceName))
                    {

                        var signedImageStream = await CustomerSignaturePad.GetImageStreamAsync(SignatureImageFormat.Png);
                        var signatureMemoryStream = signedImageStream as MemoryStream;
                        byte[] bytes = signatureMemoryStream.ToArray();
                        string _signature64str = Convert.ToBase64String(bytes);

                        manager = new DataManager();
                        Dictionary<int, string> dicResult = new Dictionary<int, string>();
                        dicResult = await manager.SaveSQLite_Payment(new Payment
                        {
                            ID = record.ID,
                            DocumentNo = record.DocumentNo,
                            OnDate = record.OnDate,
                            CustomerNo = record.CustomerNo,
                            CustomerName = record.CustomerName,
                            PaymentMethod = record.PaymentMethod,
                            Amount = record.Amount,
                            CustomerSignature = _signature64str,
                            SalesPersonCode = record.SalesPersonCode,
                            Imagestr = record.Imagestr,
                            RecStatus = "Released",
                            RefDocumentNo = record.RefDocumentNo,
                            SourceType = record.SourceType
                        });

                        string retval = string.Empty;
                        int entryno = 0;
                        if (dicResult.Count > 0)
                        {
                            foreach (KeyValuePair<int, string> pair in dicResult)
                            {
                                entryno = pair.Key;
                                retval = pair.Value;
                            }
                        }

                        if (retval == "Success")
                        {
                            if (!string.IsNullOrEmpty(record.RefDocumentNo))
                            {
                                List<PaymentReference> refList = new List<PaymentReference>();
                                refList = await GetReferenceData(record.CustomerNo);
                                string[] refnos = record.RefDocumentNo.Split(',');
                                foreach (string r in refnos)
                                {
                                    PaymentReference pf = new PaymentReference();
                                    pf = refList.Where(x => x.DocumentNo == r).FirstOrDefault();
                                    if (pf != null)
                                    {
                                        string retpaid = await manager.SaveSQLite_PaidReference(new PaidReference { DocumentNo = pf.DocumentNo, Amount =decimal.Parse(pf.Amount), SourceType = pf.SourceType });
                                    }
                                }
                            }
                            //DependencyService.Get<IMessage>().LongAlert(retval);
                            UserDialogs.Instance.ShowSuccess(retval, 3000);
                            App.gPaymentStatus = "Released";
                            //var navipages = Navigation.NavigationStack.ToList();
                            //if(navipages !=null)
                            //{
                            //    foreach (var pg in navipages)
                            //    {
                            //        Navigation.RemovePage(pg);
                            //    }
                            //}
                            PrintText(info.DeviceName);

                            
                        }
                        else
                        {
                            //DependencyService.Get<IMessage>().LongAlert(retval);
                            UserDialogs.Instance.ShowError(retval, 3000);
                        }
                    }
                       
                    else
                        UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);
                }
                else
                {
                    UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);
                }

                
            }
        }

        private void PrintText(string SelectedBthDevice)
        {

            //{PRINT, Global Options:@row,column:Name,Field Options|data|}
            //PRINT,->	Use a comma after the PRINT command only if there are global options.
            //Global Options: ->	Global Options include BACK, DEMAND, QSTOP, QUANTITY, ROTATION, and STOP. If you use more than one global option, separate each option with a comma.
            //Name ->	Specifies the name of the bar code, font, graphic, or line to print. The Name must be five characters.
            //Field Options	-> Field options increase the size of fonts, graphics, or lines. Each field option is separated from the next by a comma.
            //var s = "{PRINT:@10,30:PE203,HMULT2,VMULT2 | Total:$13.15 |@60,30:PE203,HMULT2,VMULT2 | 01 - 01 - 05 |}";

            //var s = "\x1B\x21\x30HelloWord\n test1\n test2\n";
            //string esc = "\x1B";


            // string ESC;// = "\x1B\x45"; //ESC byte in hex notation
            //LF byte in hex notation
            //\x21\x08 is Bold.
            //string cmds = "\x1B \x45 \x5A Testing \x0D \x0A \x1B \x12 test2";

            //x1b\x61\x01 = Text Center Alignment (x00 - Left,x02 right)
            //x1b\x45\x01 - Bold Letter mode
            //x1b\x2d\x02 - Underline mode
            //x1b\x21\x10 - Enabled double - height mode
            //x1b\x21\x20 = Enabled double - width mode
            ////ESC = "\x1B\x46\x1B\x38";
            // "\x00"= Character font A selected (ESC ! 0)
            //// "\x18"; //Emphasized + Double-height mode selected (ESC ! (16 + 8)) 24 dec => 18 hex

            try
            {
                //var a = Utils.Print_Payment(SelectedBthDevice, record);
               // UserDialogs.Instance.Alert(a);
                Navigation.PushAsync(new MainPage(3));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }
            
        }
    }
}