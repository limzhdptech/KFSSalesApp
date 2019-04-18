using Acr.UserDialogs;
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
    public partial class RequestHDEntryPage : ContentPage
    {
        private int RequestID { get; set; }
        public string guid { get; set; }
        private RequestHeader data { get; set; }
        private bool IsBack { get; set; }
        private bool _isEnableSaveBtn { get; set; }
        public RequestHDEntryPage(int id)
        {
            InitializeComponent();
            this.Title = "Request Stock Entry";
            RequestID = id;
            this.BackgroundColor = Color.FromHex("#dddddd");
            saveButton.Clicked += SaveButton_Clicked;
            //RequestDatePicker.IsEnabled = false;
            IsBack = false;
            BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            _isEnableSaveBtn = true;
            data = new RequestHeader();
            if (!IsBack)
            {
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
                Task.Run(async () =>
                {
                    try
                    {
                        if (RequestID != 0)
                        {
                            DataManager manager = new DataManager();
                            data = await manager.GetRequestHeaderbyID(RequestID);
                        }
                        else
                            data = null;

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayData(data);
                            UserDialogs.Instance.HideLoading(); //IsLoading = false;
                        });

                    }
                    catch (OperationCanceledException ex)
                    {
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                        //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                        UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    }
                    catch (Exception ex)
                    {
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                        //DependencyService.Get<IMessage>().LongAlert(ex.Message.ToString());
                        UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                    }
                });
            }
        }

        void DisplayData(RequestHeader record)
        {
            if (record != null)
            {
                RequestID = record.ID;
                RequestNoLabel.Text = record.RequestNo;
                //RequestDatePicker.Date = Convert.ToDateTime(record.RequestDate);
                RequestDateTimeLabel.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"); ;
                guid = record.EntryNo;
            }
            else
            {
                RequestID = 0;
                guid = Guid.NewGuid().ToString();
                DataManager manager = new DataManager();
                RequestNoLabel.Text = manager.GetLastNoSeries(App.gRSPrefix);
                //RequestDatePicker.Date = DateTime.Today;
                RequestDateTimeLabel.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt");
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (_isEnableSaveBtn)
                {
                    _isEnableSaveBtn = false;
                    DataManager manager = new DataManager();
                    string retval = await manager.SaveSQLite_RequestHeader(new RequestHeader
                    {
                        ID = RequestID,
                        RequestNo = RequestNoLabel.Text,
                        RequestDate = RequestDateTimeLabel.Text,//RequestDatePicker.Date.ToString("yyyy-MM-dd"),
                        EntryNo = guid,
                        SalesPersonCode = App.gSalesPersonCode,
                        IsSync = "false",
                        SyncDateTime = string.Empty,
                        CurStatus = "request"
                    });

                    if (retval == "Success")
                    {
                        if (RequestID == 0) //DD #284
                        {
                            manager.IncreaseNoSeries(App.gRSPrefix, RequestNoLabel.Text, "RS");
                        }
                       
                        //DependencyService.Get<IMessage>().LongAlert(retval);
                        UserDialogs.Instance.ShowSuccess(retval, 3000);
                        Navigation.PopAsync();
                    }
                    else
                    {
                        //DependencyService.Get<IMessage>().LongAlert(retval);
                        UserDialogs.Instance.ShowError(retval, 3000);
                        _isEnableSaveBtn = true;
                    }
                }
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                _isEnableSaveBtn = true;
            }
            
            
        }
    }
}