using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QHSalesApp
{
    public partial class UnloadLinePage : ContentPage
    {
        private ObservableCollection<UnloadLine> recItems { get; set; }
        private string HeaderNo { get; set; }
        public UnloadLinePage(string headerno)
        {
            InitializeComponent();

            this.Title = "Unloaded Items";
            this.BackgroundColor = Color.FromHex("#dddddd");
            Datalayout.IsVisible = false;
            Emptylayout.IsVisible = true;

            HeaderNo = headerno;
            BindingContext = this;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await LoadData();
            //if(pagefrom=="Released")
            //{
            //    AddButton.IsVisible = false;
            //}
        }
        async Task LoadData()
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;
            Task.Run(async () =>
            {
                try
                {

                    recItems = new ObservableCollection<UnloadLine>();
                    DataManager manager = new DataManager();
                    recItems = await manager.GetUnloadLinesbyDocNo(HeaderNo);

                    Device.BeginInvokeOnMainThread(() =>
                    {

                        if (recItems != null)
                        {
                            if (recItems.Count > 0)
                            {
                                listview.ItemsSource = recItems.OrderBy(x => x.ItemDesc);
                                Datalayout.IsVisible = true;
                                Emptylayout.IsVisible = false;
                            }
                            else
                            {
                                listview.ItemsSource = null;
                                Datalayout.IsVisible = false;
                                Emptylayout.IsVisible = true;
                            }

                        }
                        else
                        {
                            listview.ItemsSource = null;
                            Datalayout.IsVisible = false;
                            Emptylayout.IsVisible = true;
                        }

                        listview.Unfocus();
                        UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    });

                }
                catch (OperationCanceledException ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.HideLoading(); //IsLoading = false;
                    UserDialogs.Instance.ShowError(ex.Message.ToString(), 3000);
                }
            });
        }

        private void FilterKeyword(string filter)
        {
            if (recItems == null) return;
            listview.BeginRefresh();
            if (string.IsNullOrWhiteSpace(filter))
            {
                listview.ItemsSource = recItems;
            }
            else
            {
                listview.ItemsSource = recItems.Where(x => x.ItemNo.ToLower().Contains(filter.ToLower()) || x.ItemDesc.ToLower().Contains(filter.ToLower()));
            }
            listview.EndRefresh();
        }
    }
}
