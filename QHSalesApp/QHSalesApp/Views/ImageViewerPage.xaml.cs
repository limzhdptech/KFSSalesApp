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
    public partial class ImageViewerPage : ContentPage
    {
        public ImageViewerPage(string StrImage,string docno)
        {
            InitializeComponent();
            this.BackgroundColor = Color.FromHex("#dddddd");
            this.Title = docno + " - Image Viewer";
            imgPayment.Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(StrImage)));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}