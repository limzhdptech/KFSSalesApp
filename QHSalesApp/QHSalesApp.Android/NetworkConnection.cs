
using Android.Content;
using QHSalesApp.Droid;
using Android.Net;
using System.Net;
using Xamarin.Forms;

[assembly: Dependency(typeof(NetworkConnection))]
namespace QHSalesApp.Droid
{
    public class NetworkConnection : INetworkConnection
    {
        public bool IsConnected { get; set; }
        public void CheckNetworkConnection()
        {
            var connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);
            var activeNetworkInfo = connectivityManager.ActiveNetworkInfo;
            if (activeNetworkInfo != null && activeNetworkInfo.IsConnectedOrConnecting)
                IsConnected = true;
            else
                IsConnected = false;
        }
        public string IsServiceOnline(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
            request.Timeout = 15000;
            request.Method = "GET"; // HEAD - As per Lasse's comment
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        return "true";
                    else
                        return "false : " + response.StatusCode;
                }
            }
            catch (WebException wex)
            {
                return wex.Message;
            }
        }
    }
}