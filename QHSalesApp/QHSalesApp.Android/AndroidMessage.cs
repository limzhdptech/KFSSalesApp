
using Android.App;
using Android.Widget;
using QHSalesApp.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidMessage))]
namespace QHSalesApp.Droid
{
    public class AndroidMessage : IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }
        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}