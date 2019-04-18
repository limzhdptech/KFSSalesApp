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

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileUploadPage : ContentPage
    {
        Queue<string> paths = new Queue<string>();
        string filePath = string.Empty;
        bool isBusy = false;
        public FileUploadPage()
        {
            InitializeComponent();
            this.BackgroundColor = Color.FromHex("#dddddd");
            this.Title = "Take Image";
            CrossFileUploader.Current.FileUploadCompleted += Current_FileUploadCompleted;
            CrossFileUploader.Current.FileUploadError += Current_FileUploadError;
            CrossFileUploader.Current.FileUploadProgress += Current_FileUploadProgress;
        }

        private void Current_FileUploadProgress(object sender, FileUploadProgress e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                progress.Progress = e.Percentage / 100.0f;
            });
        }

        private void Current_FileUploadError(object sender, FileUploadResponse e)
        {
            isBusy = false;
            System.Diagnostics.Debug.WriteLine($"{e.StatusCode} - {e.Message}");
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("File Upload", "Upload Failed", "Ok");
                progress.IsVisible = false;
                progress.Progress = 0.0f;
            });
        }

        private void Current_FileUploadCompleted(object sender, FileUploadResponse e)
        {
            isBusy = false;
            System.Diagnostics.Debug.WriteLine($"{e.StatusCode} - {e.Message}");
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("File Upload", "Upload Completed", "Ok");
                progress.IsVisible = false;
                progress.Progress = 0.0f;
            });
        }

        void OnUpload(object sender, EventArgs args)
        {
            if (isBusy)
                return;
            isBusy = true;
            progress.IsVisible = true;


            //Uploading multiple images at once

            /*List<FilePathItem> pathItems = new List<FilePathItem>();
            while (paths.Count > 0)
            {
                pathItems.Add(new FilePathItem("image",paths.Dequeue()));
            }*/

            CrossFileUploader.Current.UploadFileAsync("<URL HERE>", new FilePathItem("<FIELD NAME HERE>", filePath), new Dictionary<string, string>()
            {
                /*<HEADERS HERE>*/
            });
        }

        private async void TakePhotoButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":(No camera available.", "OK");
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
                image.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();

                    file.Dispose();
                    return stream;
                });
            }
            catch (Exception ex)
            {

                await DisplayAlert("Get Image Error", ":(" + ex.Message.ToString(), "OK");
            }
           
        }

        private async void PickPhotoButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {

                await DisplayAlert("Get Image Error", ":(" + ex.Message.ToString(), "OK");
            }
            

           
        }
    }
}