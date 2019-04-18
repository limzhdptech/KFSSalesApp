using Acr.UserDialogs;
using PCLBluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BTConnectPage : ContentPage
    {
        public IBluetoothDevice Device;
        readonly Database database;

        public BTConnectPage()
        {
            InitializeComponent();
            database = new Database(Constants.DatabaseName);
            database.CreateTable<DeviceInfo>();
            this.Title = "Save Bluetooth Printer";
            SaveButton.Clicked += SaveButton_Clicked;
           
            this.ToolbarItems.Add(new ToolbarItem { Text = "Back", Command = new Command(this.BackPage) });
        }

        void BackPage()
        {
            Navigation.PushAsync(new MainPage(0));
        }
       
        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            DataManager manager = new DataManager();
            DeviceInfo line = new DeviceInfo()
            {
                DeviceName = DeviceNameLabel.Text,
                Address = AddressLabel.Text,
                DeviceUniqueId = UniqueIdenLabel.Text,
                Port = "0"
            };
            string retval = await manager.SaveSQLite_DeviceInfo(line);
            if (retval == "Success")
            {
                UserDialogs.Instance.ShowSuccess(retval, 3000);
            }
            else
            {
                UserDialogs.Instance.ShowError(retval, 3000);
            }
        }

        public void Init(IBluetoothDevice device)
        {
            Device = device;
            DeviceNameLabel.Text = Device.Name;
            AddressLabel.Text = Device.Address;
            var ui = Device.UniqueIdentifiers.FirstOrDefault();
            if (ui != null)
                UniqueIdenLabel.Text = ui.ToString();

        }
    }
}