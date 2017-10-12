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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BNWallet_Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        UserAccountRuntime UAR;
        UserAccountRuntimeDB UARDB;
        UserAccounts UA;
        UserAccountsDB UADB;
       

        public MainPage()
        {
            this.InitializeComponent();
            UARDB = new UserAccountRuntimeDB();
            UAR = UARDB.Get();
            UADB = new UserAccountsDB();
            
        }

        
      

        private async void Login_Button_Click(System.Object sender, RoutedEventArgs e)
        {
            if (UAR != null)
            {
                if (UAR.Username == LoginUsername.Text)
                {
                    if (StringCipher.Decrypt(UAR.Password) == (LoginPassword.Password))
                    {
                        Frame.Navigate(typeof(InfoScreen));
                    }
                    else
                    {
                        MessageDialog incorrectAlert = new MessageDialog("Password is incorrect");
                        incorrectAlert.Title = "Incorrect";
                        incorrectAlert.Commands.Add(new UICommand("Ok") { Id = 0 });
                        incorrectAlert.DefaultCommandIndex = 0;
                        var result = await incorrectAlert.ShowAsync();
                        LoginPassword.Password = "";
                    }
                }
                else
                {
                    MessageDialog incorrectAlert = new MessageDialog("Username doesn't exist, please register");
                    incorrectAlert.Title = "Incorrect";
                    incorrectAlert.Commands.Add(new UICommand("Ok") { Id = 0 });
                    incorrectAlert.DefaultCommandIndex = 0;
                    var result = await incorrectAlert.ShowAsync();

                }
            }
            else
            {
                MessageDialog incorrectAlert = new MessageDialog("User doesn't exist, please register");
                incorrectAlert.Title = "Incorrect";
                incorrectAlert.Commands.Add(new UICommand("Ok") { Id = 0 });
                incorrectAlert.DefaultCommandIndex = 0;
                var result = await incorrectAlert.ShowAsync();

            }
        }

        private async void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog alertDialog = new MessageDialog("Registering a new user clears the database. Are you sure?");
            alertDialog.Title = "Confirmation";
            alertDialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            alertDialog.Commands.Add(new UICommand("No") { Id = 1 });
            alertDialog.DefaultCommandIndex = 0;
            alertDialog.CancelCommandIndex = 1;
            var result = await alertDialog.ShowAsync();
            if ((int)result.Id == 0)
            {
                UADB.ClearDB();

                Frame.Navigate(typeof(RegisterPage));

            }
            else
            {
                
            };
            //alertDialog.Show();
            
        }
    }
}
