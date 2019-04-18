
using QHSalesApp;
using System.Linq;
using Android.Bluetooth;
using Java.IO;
using Java.Util;
using Java.Lang;
[assembly: Xamarin.Forms.Dependency(typeof(BluetoothPrinter))]
namespace QHSalesApp
{
    public class BluetoothPrinter : IBluetoothPrinter
    {
        public string Print(string deviceName,string printText)
        {
            try
            {
                BluetoothSocket socket = null;
                BufferedReader inReader = null;
                BufferedWriter outReader = null;
                BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                if (adapter.BondedDevices.Count == 0) // update by Zhi Hong Lim
                {
                    throw new System.Exception("Bluetooth is not turned on.");
                }

                string bt_printer = (from d in adapter.BondedDevices
                                     where d.Name == deviceName
                                     select d.Address).FirstOrDefault().ToString();
                BluetoothDevice device = adapter.GetRemoteDevice(bt_printer);
                UUID applicationUUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

                socket = device.CreateRfcommSocketToServiceRecord(applicationUUID);
                socket.Connect();

                inReader = new BufferedReader(new InputStreamReader(socket.InputStream));
                outReader = new BufferedWriter(new OutputStreamWriter(socket.OutputStream));

                outReader.Write(printText);

                outReader.Flush();
                Thread.Sleep(1000);

                var s = inReader.Ready();
                inReader.Skip(0);

                //Close All
                inReader.Close();
                outReader.Close();
                socket.Close();

                return "Success";
            }
            catch (Java.Lang.Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}