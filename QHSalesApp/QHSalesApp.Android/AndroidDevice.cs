using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QHSalesApp.Droid;
using Android.Provider;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidDevice))]
namespace QHSalesApp.Droid
{
    public class AndroidDevice : IDevice
    {
        public string GetIdentifier()
        {
            return Settings.Secure.GetString(Forms.Context.ContentResolver, Settings.Secure.AndroidId);
        }
    }
}