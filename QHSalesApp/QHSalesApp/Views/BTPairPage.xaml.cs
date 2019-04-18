using Acr.UserDialogs;
using PCLBluetooth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QHSalesApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BTPairPage : ContentPage
    {
        private IBluetoothClient _bluetoothClient;
        private readonly ObservableCollection<IBluetoothDevice> _pairedDevices = new ObservableCollection<IBluetoothDevice>();
        public ObservableCollection<IBluetoothDevice> PairedDevices
        {
            get { return _pairedDevices; }

        }

        private readonly ObservableCollection<IPairableBluetoothDevice> _discoveredDevices = new ObservableCollection<IPairableBluetoothDevice>();
        public ObservableCollection<IPairableBluetoothDevice> DiscoveredDevices
        {
            get { return _discoveredDevices; }

        }

        public BTPairPage()
        {
            InitializeComponent();
            this.Title = "Bluetooth Setting";
            ButtonGetPaired.Clicked += ButtonGetPaired_OnClicked;
            _bluetoothClient = DependencyService.Get<IBluetoothClient>();
            listViewPaired.ItemsSource = PairedDevices;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MainPage(0));

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as IBluetoothDevice;

            var view = new BTConnectPage();
            view.Init(item);
            /*
            view.DeviceConnected += async (o, t) =>
            {
                var connectedView = new ConnectedView(t.Device);
                await Navigation.PopModalAsync();
                await Navigation.PushModalAsync(connectedView);
            };
            */
            await Navigation.PushAsync(view);

            (sender as ListView).SelectedItem = null;
        }

        //private async void ButtonDiscover_OnClicked(object sender, EventArgs e)
        //{

        //    DiscoveredDevices.Clear();

        //    using (UserDialogs.Instance.Loading("Searching for devices"))
        //    {
        //        await SearchDevices();
        //    }


        //}

        private async void ButtonGetPaired_OnClicked(object sender, EventArgs e)
        {
            try
            {

                PairedDevices.Clear();

                using (UserDialogs.Instance.Loading("Getting paired devices"))
                {
                    //_bluetoothClient.EndDiscovery();
                    var devices = await _bluetoothClient.GetPairedDevices();
                    if(devices!=null)
                    {
                        if(devices.Count>0)
                        {
                            foreach (var device in devices)
                            {
                                PairedDevices.Add(device);
                            }
                        }
                        else
                            UserDialogs.Instance.AlertAsync("Not found bluetooth device!", "Alert");

                    }
                    else
                    {
                        UserDialogs.Instance.AlertAsync("No paired bluetooth device!", "Alert");
                    }
                    
                }

                if(PairedDevices!=null)
                {
                    listViewPaired.ItemsSource = PairedDevices;
                }
                else
                {
                    UserDialogs.Instance.AlertAsync("No paired bluetooth device!", "Alert");
                }
                

            }
            catch (Exception ex)
            {
                UserDialogs.Instance.AlertAsync(ex.Message, "Error");
            }
        }

        private void OnPairMenuItem_Clicked(object sender, EventArgs e)
        {
            /*
            var device = listViewDiscovered.SelectedItem as IPairableBluetoothDevice;

            var query = PairedDevices.FirstOrDefault(x => x.Name == device.Name && x.Address == device.Address);

            if (query == null)
            {
                try
                {
                    device.Pair();
                }
                catch (Exception ex)
                {
                    
                }
                
            }
            else
            {
                UserDialogs.Instance.AlertAsync("Device already paired");
            }
            */
        }
    }
}