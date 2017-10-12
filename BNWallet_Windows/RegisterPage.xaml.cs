using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BNWallet_Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        UserAccountRuntime UAR;
        UserAccountRuntimeDB UARDB;

        public RegisterPage()
        {
            this.InitializeComponent();
        }

        private async void RegisterSaveBtn_Click(object sender, RoutedEventArgs e)
        {

            if (RegisterConfirmPassword.Password != RegisterPassword.Password)
            {

                MessageDialog incorrectAlert = new MessageDialog("Passwords do not match");
                incorrectAlert.Title = "Incorrect";
                incorrectAlert.Commands.Add(new UICommand("Ok") { Id = 0 });
                incorrectAlert.DefaultCommandIndex = 0;
                var result = await incorrectAlert.ShowAsync(); 
                RegisterConfirmPassword.Password = "";             
            }
            else
            {
                MessageDialog msgDialog = new MessageDialog("Are you Sure?");
                msgDialog.Title = "Confirmation";
                msgDialog.Commands.Add(new UICommand("Yes") { Id = 0 });
                msgDialog.Commands.Add(new UICommand("No") { Id = 1 });
                msgDialog.DefaultCommandIndex = 0;
                msgDialog.CancelCommandIndex = 1;
                var result = await msgDialog.ShowAsync();
                if ((int)result.Id == 0)
                {
                    UARDB = new UserAccountRuntimeDB();
                    UAR = new UserAccountRuntime();
                    UAR.Username = RegisterUsername.Text;
                    UAR.Password = StringCipher.Encrypt(RegisterPassword.Password);
                    UARDB.Save(UAR);
                    Frame.Navigate(typeof(MainPage));
                }
                else
                {

                }
            }   

        }
    }
}
