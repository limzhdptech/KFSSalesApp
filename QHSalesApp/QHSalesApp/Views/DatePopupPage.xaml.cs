using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Acr.UserDialogs;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace QHSalesApp
{
    public partial class DatePopupPage : PopupPage
    {
        private string ReportType { get; set; }
        public DatePopupPage(string type)
        {
            InitializeComponent();
            ReportType = type;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            OnDateDatePicker.Date = DateTime.Today;
            //App.gOnDate = DateTime.Today.ToString("yyyy-MM-dd");

            //if (ReportType=="void")
            //{
            //    VoidTypeLabel.IsVisible = false;
            //    VoidTypeLayout.IsVisible = false;
            //}
            //else
            //{
            //    VoidTypePicker.Items.Clear();
            //    VoidTypePicker.Items.Add("SO");
            //    VoidTypePicker.Items.Add("CN");
            //    VoidTypePicker.SelectedIndex = 0;
            //}
           
        }

        private async void OnClose(object sender, EventArgs e)
        {
            App.gOnDate = string.Empty;
           await PopupNavigation.PopAsync();
        }

        private async void UpdateButtonOnClicked(object sender, EventArgs e)
        {
            try
            {
                UpdateButton.IsEnabled = false;
                string retval = string.Empty;
               // App.gInvoiceType = VoidTypePicker.SelectedItem.ToString();
                App.gOnDate = OnDateDatePicker.Date.ToString("yyyy-MM-dd");
                switch (ReportType)
                {
                    case "Activity":
                        Print_ActivityReport();
                        break;
                    case "SalesSummary":
                        Print_DailySalesSummary();
                        break;
                    case "Void":
                        Print_VoidReport();
                        break;
                    case "SalesReturn":
                        Print_DailySalesReturn();
                        break;
                }
                UpdateButton.IsEnabled = true;
                // await PopupNavigation.PopAsync();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
            }

        }

        private void Print_ActivityReport()
        {
            try
            {
                string retmsg = string.Empty;
                DataManager manager = new DataManager();
                ObservableCollection<SalesHeader> head = new ObservableCollection<SalesHeader>();
                DeviceInfo info = new DeviceInfo();

                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {

                    info = await manager.GetDeviceInfo();
                    if (info != null)
                    {
                        if (!string.IsNullOrEmpty(info.DeviceName))
                        {
                            manager = new DataManager();
                            head = await manager.GetSQLite_SalesHeaderByDocDate(App.gOnDate);
                            if (head != null)
                            {
                                if (head.Count > 0)
                                {
                                    retmsg = "Success";
                                }
                                else
                                    retmsg = "No Data";
                            }
                            else
                                retmsg = "No Data";

                        }
                        else
                            retmsg = "Required to setup bluetooth printer!";
                        //UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);
                    }
                    else
                        retmsg = "Required to setup bluetooth printer!";
                    //UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);

                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {

                    UserDialogs.Instance.HideLoading();
                    if (retmsg == "Success")
                    {
                        var a = Utils.Print_Activity(info.DeviceName, head, App.gCompanyName, App.gSalesPersonCode, App.gSalesPersonName);
                        UserDialogs.Instance.Alert(a);

                    }
                    else
                    {
                        // UserDialogs.Instance.ShowError(retmsg, 3000);
                        UserDialogs.Instance.Alert(retmsg, "Alert", "OK");
                    }
                }));
            }
            catch (OperationCanceledException ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                //UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert", "OK");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                //UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert", "OK");
            }
        }

        private void Print_DailySalesSummary()
        {
            try
            {
                string retmsg = string.Empty;
                DataManager manager = new DataManager();
                ObservableCollection<SalesHeader> head = new ObservableCollection<SalesHeader>();
                DeviceInfo info = new DeviceInfo();

                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {

                    info = await manager.GetDeviceInfo();
                    if (info != null)
                    {
                        if (!string.IsNullOrEmpty(info.DeviceName))
                        {
                            manager = new DataManager();
                            head = await manager.GetSQLite_SalesHeaderByDocDate(App.gOnDate);
                            if (head != null)
                            {
                                if (head.Count > 0)
                                {
                                    retmsg = "Success";
                                }
                                else
                                    retmsg = "No Data";
                            }
                            else
                                retmsg = "No Data";

                        }
                        else
                            retmsg = "Required to setup bluetooth printer!";
                        //UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);
                    }
                    else
                        retmsg = "Required to setup bluetooth printer!";
                    //UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);

                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {

                    UserDialogs.Instance.HideLoading();
                    if (retmsg == "Success")
                    {
                        var b = Utils.Print_DailySalesInvSummarySO(info.DeviceName, head, App.gCompanyName, App.gSalesPersonCode, App.gSalesPersonName);
                        UserDialogs.Instance.Alert(b);

                    }
                    else
                    {
                        // UserDialogs.Instance.ShowError(retmsg, 3000);
                        UserDialogs.Instance.Alert(retmsg, "Alert", "OK");
                    }
                }));
            }
            catch (OperationCanceledException ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                //UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert", "OK");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                //UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert", "OK");
            }
        }

        private void Print_DailySalesReturn()
        {
            try
            {
                string retmsg = string.Empty;
                DataManager manager = new DataManager();
                ObservableCollection<SalesHeader> head = new ObservableCollection<SalesHeader>();
                DeviceInfo info = new DeviceInfo();

                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {

                    info = await manager.GetDeviceInfo();
                    if (info != null)
                    {
                        if (!string.IsNullOrEmpty(info.DeviceName))
                        {
                            manager = new DataManager();
                            head = await manager.GetSQLite_SalesHeaderByDocDate(App.gOnDate);
                            if (head != null)
                            {
                                if (head.Count > 0)
                                {
                                    retmsg = "Success";
                                }
                                else
                                    retmsg = "No Data";
                            }
                            else
                                retmsg = "No Data";

                        }
                        else
                            retmsg = "Required to setup bluetooth printer!";
                        //UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);
                    }
                    else
                        retmsg = "Required to setup bluetooth printer!";
                    //UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);

                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {

                    UserDialogs.Instance.HideLoading();
                    if (retmsg == "Success")
                    {
                        var a = Utils.Print_DailySalesInvSummaryCN(info.DeviceName, head, App.gCompanyName, App.gSalesPersonCode, App.gSalesPersonName);
                        UserDialogs.Instance.Alert(a);
                    }
                    else
                    {
                        // UserDialogs.Instance.ShowError(retmsg, 3000);
                        UserDialogs.Instance.Alert(retmsg, "Alert", "OK");
                    }
                }));
            }
            catch (OperationCanceledException ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                //UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert", "OK");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                //UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert", "OK");
            }
        }

        private void Print_VoidReport()
        {
            try
            {
                string retmsg = string.Empty;
                DataManager manager = new DataManager();
                ObservableCollection<SalesHeader> head = new ObservableCollection<SalesHeader>();
                DeviceInfo info = new DeviceInfo();

                Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading", MaskType.Black));
                Task.Run(async () =>
                {

                    info = await manager.GetDeviceInfo();
                    if (info != null)
                    {
                        if (!string.IsNullOrEmpty(info.DeviceName))
                        {
                            manager = new DataManager();
                            head = await manager.GetSQLite_VoidSalesHeaderByDocDate(App.gOnDate);
                            if (head != null)
                            {
                                if (head.Count > 0)
                                {
                                    retmsg = "Success";
                                }
                                else
                                    retmsg = "No Data";
                            }
                            else
                                retmsg = "No Data";

                        }
                        else
                            retmsg = "Required to setup bluetooth printer!";
                        //UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);
                    }
                    else
                        retmsg = "Required to setup bluetooth printer!";
                    //UserDialogs.Instance.ShowError("Required to setup bluetooth printer!", 3000);

                }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
                {

                    UserDialogs.Instance.HideLoading();
                    if (retmsg == "Success")
                    {
                        var a = Utils.Print_VoidSalesSummary(info.DeviceName, head, App.gCompanyName, App.gSalesPersonCode, App.gSalesPersonName);
                        UserDialogs.Instance.Alert(a);
                    }
                    else
                    {
                        // UserDialogs.Instance.ShowError(retmsg, 3000);
                        UserDialogs.Instance.Alert(retmsg, "Alert", "OK");
                    }
                }));

            }
            catch (OperationCanceledException ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading(); //IsLoading = false;
                //UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert", "OK");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading(); //IsLoading = false;
                //UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                UserDialogs.Instance.Alert(ex.Message.ToString(), "Alert", "OK");
            }
        }
    }
}
