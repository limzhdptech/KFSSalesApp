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
    public partial class AdminLoginPage : ContentPage
    {
        public AdminLoginPage()
        {
            InitializeComponent();
            this.Title = "Admin Login";
            LoginButton.Clicked += LoginButton_Clicked;
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
           // UserDialogs.Instance.ShowLoading("Loading", MaskType.Black); //IsLoading = true;

            if (string.IsNullOrEmpty(PasswordEntry.Text))
            {
                //IsLoading = false;
                UserDialogs.Instance.HideLoading();
                //DependencyService.Get<IMessage>().LongAlert("Password is blank!");
                UserDialogs.Instance.ShowError("Password is blank!", 3000);
                return;
            }

            if(PasswordEntry.Text==App.gAdminPsw)
            {
                Navigation.PushAsync(new MainPage(7));
                
            }
            else
            {
                UserDialogs.Instance.ShowError("Wrong Password!", 3000);
                PasswordEntry.Focus();
            }
               

           // UserDialogs.Instance.HideLoading();
        }
    }
}