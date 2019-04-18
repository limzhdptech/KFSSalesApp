using Acr.UserDialogs;
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
    public partial class ReleasedPaymentPage : ContentPage
    {
        int EntryNo { get; set; }
        string strImage { get; set; }
        private Payment payment { get; set; }
        public ReleasedPaymentPage(int id)
        {
            InitializeComponent();
            EntryNo = id;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            DataManager manager = new DataManager();
            payment = new Payment(); 
            payment = manager.GetPaymentbyID(EntryNo);

            this.Title = "Payment Detail";
            this.BackgroundColor = Color.FromHex("#dddddd");
            DocumentNoLabel.Text = payment.DocumentNo;
            PaymentDateLabel.Text = payment.OnDate;
            RefDocumentNoLabel.Text = payment.RefDocumentNo;
            CustomerNoLabel.Text = payment.CustomerNo;
            CustomerNameLabel.Text = payment.CustomerName;
            NoteLabel.Text = payment.Note;
            PaymentMethodLabel.Text = payment.PaymentMethod;
            if (payment.PaymentMethod != "CASH") ImageLinkLabel.IsVisible = true; else ImageLinkLabel.IsVisible = false;
            AmountLabel.Text = string.Format("{0:0.00}", payment.Amount);
            imgSignature.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(payment.CustomerSignature)));
            if(!string.IsNullOrEmpty(payment.Imagestr))
            {
                ImageLinkLabel.IsVisible = true;
                ImageLinkLabel.Text = payment.DocumentNo + " - reference image link";
                strImage = payment.Imagestr;
            }
            else
            {
                ImageLinkLabel.IsVisible = false;
                strImage = string.Empty;
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
                      PrintText(info.DeviceName);
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

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            App.gPageTitle = "View Image";
            Navigation.PushAsync(new ImageViewerPage(strImage,DocumentNoLabel.Text));
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
                //var a = Utils.Print_Payment(SelectedBthDevice, payment);
                //UserDialogs.Instance.Alert(a);
                Navigation.PushAsync(new MainPage(3));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }

        }
    }
}