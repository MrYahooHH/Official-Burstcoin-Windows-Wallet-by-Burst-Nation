using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using ZXing;

namespace BNWallet_Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InfoScreen : Page
    {

        BNWalletAPI BNWAPI;
        UserAccounts UA;
        UserAccountsDB UADB;
        UserAccountRuntime UAR;
        UserAccountRuntimeDB UARDB;
        List<string> items;
        Dictionary<string, int> userAccountIndex;



        public InfoScreen()
        {
            this.InitializeComponent();

            MediaElement MediaElement = new MediaElement();
            MediaElement.Source = new Uri("http://198.27.68.226:8000/stream");
            MediaElement.AreTransportControlsEnabled = true;
            MediaElement.TransportControls.IsZoomButtonVisible = false;
            MediaElement.TransportControls.IsZoomEnabled = false;
            MediaElement.TransportControls.IsSeekBarVisible = false;
            MediaElement.TransportControls.IsCompact = true;
            MediaElement.TransportControls.IsFullWindowButtonVisible = false;
            MediaElement.TransportControls.IsFullWindowEnabled = false;


            MediaElement.AutoPlay = false;
            RadioGrid.Children.Add(MediaElement);

            UserAccountsDB userAccountDB = new UserAccountsDB();
            UserAccounts[] userAccount = userAccountDB.GetAccountList();
            if(WalletName.Text == "")
            {

            }
            else
            {
                WalletName.Text = userAccount[0].AccountName;
                BurstAddress.Text = userAccount[0].BurstAddress;
                
            }
            PopulateWalletList();




        }

        public void PopulateWalletList()
        {
            UserAccountsDB userAccountDB = new UserAccountsDB();

            UserAccounts[] userAccount = userAccountDB.GetAccountList();

            //items = userAccount.ToList<UserAccounts>();
            userAccountIndex = new Dictionary<string, int>();
            items = new List<string>();
            for (int i = 0; i < userAccount.Length; i++)
            {
                items.Add(userAccount[i].AccountName);
                userAccountIndex.Add(userAccount[i].AccountName, i);
            }

            WalletList.ItemsSource = items;
            Passphrase.Text = "";
        }
     
        private async void BtnAddWallet_Click(System.Object sender, RoutedEventArgs e)
        {
            BNWAPI = new BNWalletAPI();
            try
            {                
                GetAccountIDResult gair = BNWAPI.getAccountID(Passphrase.Text, "");
           
                if (gair.success)
                {
                    GetAccountResult gar = BNWAPI.getAccount(gair.accountRS);
                    if (gar.success)
                    {
                        UADB = new UserAccountsDB();
                        UA = UADB.Get(gar.name);
                        if (UA != null)
                        {
                            MessageDialog incorrectAlert = new MessageDialog("Wallet Already Exists" + UA.AccountName);
                            incorrectAlert.Title = "Error";
                            incorrectAlert.Commands.Add(new UICommand("Ok") { Id = 0 });
                            incorrectAlert.DefaultCommandIndex = 0;
                            var result = await incorrectAlert.ShowAsync();

                        }
                        else
                        {
                            UARDB = new UserAccountRuntimeDB();
                            UAR = UARDB.Get();
                            string password = UAR.Password;
                            UA = new UserAccounts();
                            string plaintext = Passphrase.Text;
                            string encryptedstring = StringCipher.Encrypt(plaintext);

                            if (gar.name == "")
                                UA.AccountName = "No Name Set";
                            else
                                UA.AccountName = gar.name;
                            UA.BurstAddress = gar.accountRS;
                            UA.PassPhrase = encryptedstring;
                            UADB.Save(UA);

                            

                            MessageDialog Success = new MessageDialog("Wallet Added Successfully :" + UA.BurstAddress);
                            Success.Title = "Success";
                            Success.Commands.Add(new UICommand("Ok") { Id = 0 });
                            Success.DefaultCommandIndex = 0;
                            var SuccessResult = await Success.ShowAsync();
                            PopulateWalletList();

                        }
                    }
                    else
                    {

                        UADB = new UserAccountsDB();
                        UARDB = new UserAccountRuntimeDB();
                        UAR = UARDB.Get();
                        string password = UAR.Password;
                        UA = new UserAccounts();
                        string plaintext = Passphrase.Text;
                        string encryptedstring = StringCipher.Encrypt(plaintext);
                        UA.AccountName = "Unknown Account";
                        UA.BurstAddress = gair.accountRS;
                        UA.PassPhrase = encryptedstring;
                        UADB.Save(UA);

                        MessageDialog Success = new MessageDialog("Wallet Added Successfully :" + UA.BurstAddress);
                        Success.Title = "Success";
                        Success.Commands.Add(new UICommand("Ok") { Id = 0 });
                        Success.DefaultCommandIndex = 0;
                        var SuccessResult = await Success.ShowAsync();
                        PopulateWalletList();

                    }
                }
                else
                {
                    MessageDialog ErrorAlert = new MessageDialog("Received Error: Please enter a valid passphrase for an existing Burstcoin wallet or click the generate new passphrase button." );
                    ErrorAlert.Title = "Error";
                    ErrorAlert.Commands.Add(new UICommand("Ok") { Id = 0 });
                    ErrorAlert.DefaultCommandIndex = 0;
                    var result = await ErrorAlert.ShowAsync();

                }
            }
            catch
            {
                MessageDialog ConfirmationDetailsDialog = new MessageDialog("Received Error: Please enter a valid passphrase for an existing Burstcoin wallet or click the generate new passphrase button.");
                ConfirmationDetailsDialog.Title = "Error";
                ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                var ConfResult = await ConfirmationDetailsDialog.ShowAsync();

            }

        }

        private async void BtnCreateWallet_Click(object sender, RoutedEventArgs e)
        {
            Passphrase.Text= CreatePassword(88);
            
            MessageDialog alertDialog = new MessageDialog("Please copy your new passphrase and keep it somewhere safe. Losing this will result in you losing your wallet. Once you have saved your generated address, please click add wallet to add it to the list.");
            alertDialog.Title = "Confirmation";
            alertDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
            alertDialog.DefaultCommandIndex = 0;
            var result = await alertDialog.ShowAsync();

           



        }

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        private async void WalletList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserAccountsDB userAccountDB = new UserAccountsDB();
            UserAccounts[] userAccount = userAccountDB.GetAccountList();
            if (WalletList.SelectedItems.Count > 0)
            {
                string Wallet = WalletList.SelectedItems[0].ToString();
                Balance.Text = "";
                
                BurstAddress.Text = userAccount[userAccountIndex[Wallet]].BurstAddress;
                string SecretPhrase = StringCipher.Decrypt(userAccount[userAccountIndex[Wallet]].PassPhrase);
                BNWAPI = new BNWalletAPI();
                GetAccountIDResult gair = BNWAPI.getAccountID(SecretPhrase, "");
                try
                {
                    if (gair.success)
                    {

                        GetAccountResult gar = BNWAPI.getAccount(gair.accountRS);
                        if (gar.success)
                        {   
                            if(gar.name == null) { WalletName.Text = "Unknown Account"; } else { WalletName.Text = gar.name; }
                            
                            BurstAddress.Text = gar.accountRS;
                            WalletName.Text = Wallet;
                            string BB;
                            BB = gar.balanceNQT;
                            double burstdbl = Convert.ToDouble(BB);
                            burstdbl = burstdbl / 100000000;
                            Balance.Text = "Balance: " + burstdbl.ToString("#,0.00000000") + " BURST";
                            TransactionNumber.Text = "100";

                            GetTransactionListResult gtlr = BNWAPI.getTransactionList(gair.accountRS,TransactionNumber.Text);
                            if (gtlr.success)
                            {
                                
                                TransactionList_ListView.Items.Clear();
                                //TransactionList_ListView.Items.Add("Date & Time" + "                    " + "Amount + Fee" + "     "+"Account" + "                                            " + "Confirmations");
                                for (int i = 0; i < gtlr.transactions.Length; i++)
                                {

                                    DateTime date = UnixTimeStampToDateTime(gtlr.transactions[i].timestamp);
                                    string transactionlist = gtlr.transactions[i].transaction;
                                    string amount = gtlr.transactions[i].amountNQT;
                                    double amnt = Convert.ToDouble(amount);
                                    amnt = amnt / 100000000;
                                    string newamnt = amnt.ToString("#,0.00000000");
                                    string Fee = gtlr.transactions[i].feeNQT;
                                    double feeamnt = Convert.ToDouble(Fee);
                                    feeamnt = feeamnt / 100000000;
                                    string newFeeAmnt = feeamnt.ToString("#,0");
                                    string SenderAccount = gtlr.transactions[i].senderRS;
                                    string Confirmations = gtlr.transactions[i].confirmations;
                                    double Conf = Convert.ToDouble(Confirmations);
                                    string Recipient = gtlr.transactions[i].recipientRS;
                                    if (Recipient == BurstAddress.Text)
                                    {
                                        newamnt = "+" + newamnt;
                                    }
                                    else
                                    {
                                        newamnt = "-" + newamnt;
                                        SenderAccount = Recipient;
                                        if (SenderAccount == "")
                                        {
                                            SenderAccount = "/                                                      ";
                                        }
                                    }

                                    if (Conf > 9)
                                    {
                                        Conf = 10;
                                        string newConf = Conf.ToString() + "+";
                                        TransactionList_ListView.Items.Add(date + "    " + newamnt + "+" + newFeeAmnt + "    " + SenderAccount + "     " + newConf);
                                    }
                                    else
                                    {
                                        string newConf = Conf.ToString();
                                        TransactionList_ListView.Items.Add(date + "    " + newamnt + "+" + newFeeAmnt + "    " + SenderAccount + "     " + newConf);
                                    }
                                    
                                }
                            }                                                      
                        }
                        else
                        {
                            MessageDialog ConfirmationDetailsDialog = new MessageDialog("Received Error: " + gar.errorMsg);
                            ConfirmationDetailsDialog.Title = "API Error";
                            ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                            var ConfResult = await ConfirmationDetailsDialog.ShowAsync();
                            TransactionList_ListView.Items.Clear();
                            Balance.Text = "Balance: 0.00000000 BURST";
                        }

                        CreateQRCode();
                    }
                    else
                    {
                        MessageDialog ConfirmationDetailsDialog = new MessageDialog("Received Error: " + gair.errorMsg);
                        ConfirmationDetailsDialog.Title = "API Error";
                        ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                        var ConfResult = await ConfirmationDetailsDialog.ShowAsync();
                    }
                }
                catch
                {
                    MessageDialog ConfirmationDetailsDialog = new MessageDialog("Received Error: " + gair.errorMsg);
                    ConfirmationDetailsDialog.Title = "API Error";
                    ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                    var ConfResult = await ConfirmationDetailsDialog.ShowAsync();                   
                }
            }
        }

        private void CreateQRCode()
        {
            IBarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 600,
                    Width = 600
                }
            };
            var result = writer.Write(BurstAddress.Text);
            var wb = result as WriteableBitmap;


            QRCode.Source = wb;
        }

        private async void BtnRemoveWallet_Click(object sender, RoutedEventArgs e)
        {
            UserAccountsDB UADB = new UserAccountsDB();
            UserAccounts UA = new UserAccounts();
            try
            {
                UA = UADB.Get(WalletName.Text);
                UADB.RemoveWallet(UA);
                PopulateWalletList();
            }
            catch
            {
                MessageDialog ConfirmationDetailsDialog = new MessageDialog("Received Error: You need to have a wallet present in order to remove one.");
                ConfirmationDetailsDialog.Title = "Error";
                ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                var ConfResult = await ConfirmationDetailsDialog.ShowAsync();
            }
                
            

        }

        private async void Send_Burst_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UARDB = new UserAccountRuntimeDB();
                UAR = UARDB.Get();
                UserAccountsDB userAccountDB = new UserAccountsDB();
                UserAccounts[] userAccount = userAccountDB.GetAccountList();
                string SecretPhrase = StringCipher.Decrypt(userAccount[userAccountIndex[WalletName.Text]].PassPhrase);

                MessageDialog alertDialog = new MessageDialog("Are you sure all the details are correct?");
                alertDialog.Title = "Confirmation";
                alertDialog.Commands.Add(new UICommand("Yes") { Id = 0 });
                alertDialog.Commands.Add(new UICommand("No") { Id = 1 });
                alertDialog.DefaultCommandIndex = 0;
                alertDialog.CancelCommandIndex = 1;
                var result = await alertDialog.ShowAsync();
                if ((int)result.Id == 0)
                {


                    double amntdbl = Convert.ToDouble(Amount.Text);
                    amntdbl = amntdbl * 100000000;
                    string amount = amntdbl.ToString();

                    double amntdblconf = Convert.ToDouble(amount);
                    amntdblconf = amntdblconf / 100000000;
                    Amount.Text = amntdblconf.ToString("#,0.00000000");

                    if (Fee.Text == "")
                    {
                        Fee.Text = "0";
                    }
                    double feeamnt = Convert.ToDouble(Fee.Text);
                    feeamnt = feeamnt * 100000000;
                    string fee = feeamnt.ToString();

                    double feeamntconf = Convert.ToDouble(fee);
                    feeamntconf = feeamntconf / 100000000;
                    Fee.Text = feeamntconf.ToString("#,0.00000000");


                    BNWAPI = new BNWalletAPI();
                    GetsendMoneyResult gsmr = BNWAPI.sendMoney(Recepient_Address.Text, amount, fee, SecretPhrase, Message.Text, cbEncrypt.IsChecked.HasValue);
                    if (gsmr.success)
                    {
                        GetTransactionResult gtr = BNWAPI.getTransaction(gsmr.transaction);
                        if (gtr.success)
                        {
                            GetMiningInfo gmi = BNWAPI.getMiningInfo();
                            if (gmi.success)
                            {

                                MessageDialog ConfirmationDetailsDialog = new MessageDialog("Sender Address: " + gtr.senderRS + "\n" + "Amount of Burst sent: " + Amount.Text + "\n" + "Fee: " + Fee.Text + "\n" + "Recipient Address: " + gtr.recipientRS + "\n" + "Signature Hash: " + gsmr.signatureHash
                                + "\n" + "Transaction ID: " + gsmr.transaction + "\n" + "Block Height: " + gmi.height);
                                ConfirmationDetailsDialog.Title = "Confirmation Details";
                                ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                                var ConfResult = await ConfirmationDetailsDialog.ShowAsync();

                                if ((int)result.Id == 0)
                                {
                                    Recepient_Address.Text = "";
                                    Amount.Text = "";
                                    Fee.Text = "";
                                    Message.Text = "";
                                    PopulateWalletList();
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageDialog ConfirmationDetailsDialog = new MessageDialog("Received Error: " + gsmr.errorMsg);
                        ConfirmationDetailsDialog.Title = "API Error";
                        ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                        var ConfResult = await ConfirmationDetailsDialog.ShowAsync();

                    }
                }
                else
                {

                }
            }
            catch
            {
                MessageDialog ConfirmationDetailsDialog = new MessageDialog("Received Error: A valid Burst Wallet needs to be selected before any funds can be sent");
                ConfirmationDetailsDialog.Title = "Error";
                ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                var ConfResult = await ConfirmationDetailsDialog.ShowAsync();

            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(2014, 08, 11, 2, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private void BurstAddress_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Recepient_Address.Text = BurstAddress.Text;
        }

        private async void TransactionList_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetTransactionListResult gtlr = BNWAPI.getTransactionList(BurstAddress.Text,TransactionNumber.Text);
            if (gtlr.success)
            {
                for (int i = 0; i < gtlr.transactions.Length; i++)
                {
                    if (i == TransactionList_ListView.SelectedIndex)
                    {
                        GetTransactionResult gtr = BNWAPI.getTransaction(gtlr.transactions[i].transaction);
                        if (gtr.success)
                        {
                                

                            double amntdblconf = Convert.ToDouble(gtr.amountNQT);
                            amntdblconf = amntdblconf / 100000000;
                            string Amount = amntdblconf.ToString("#,0.00000000");

                                

                            double feeamntconf = Convert.ToDouble(gtr.feeNQT);
                            feeamntconf = feeamntconf / 100000000;
                            string Fee = feeamntconf.ToString("#,0.00000000");

                            MessageDialog ConfirmationDetailsDialog = new MessageDialog("Sender Address: " + gtr.senderRS + "\n" + "Amount of Burst sent: " + Amount + "\n" + "Fee: " + Fee + "\n" + "Recipient Address: " + gtr.recipientRS
                            + "\n" + "Block Height: " + gtr.ecBlockHeight + "\n" + "Confirmations: " + gtlr.transactions[i].confirmations);
                            ConfirmationDetailsDialog.Title = "Transaction " + gtlr.transactions[i].transaction + " Info";
                            ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                            var ConfResult = await ConfirmationDetailsDialog.ShowAsync();
                        }
                        
                    }
                }
            }
        }

        private async void Logo_Tapped(object sender, TappedRoutedEventArgs e)
        {           
            var uri = new Uri(@"https://www.burstnation.com");            
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);            
        }

        private async void AndroidWalletBanner_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(@"https://play.google.com/store/apps/details?id=com.official.bnwallet");
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void BurstAddress_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(BurstAddress.Text);
            Clipboard.SetContent(dataPackage);
        }

        private async void BtnChangeWalletName_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog alertDialog = new MessageDialog("Changing your Wallet Name costs 1 Burst as a fee, continue?");
            alertDialog.Title = "Confirmation";
            alertDialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            alertDialog.Commands.Add(new UICommand("No") { Id = 1 });
            alertDialog.DefaultCommandIndex = 0;
            alertDialog.CancelCommandIndex = 1;
            var result = await alertDialog.ShowAsync();
            if ((int)result.Id == 0)
            {
                UserAccountsDB userAccountDB = new UserAccountsDB();
                try
                {

                    UserAccounts[] userAccount = userAccountDB.GetAccountList();
                    UADB = new UserAccountsDB();
                    UA = UADB.Get(WalletName.Text);

                    string SecretPhrase = StringCipher.Decrypt(UA.PassPhrase);
                    SetAccountInfo sai = BNWAPI.setAccountInfo(New_WalletName.Text, SecretPhrase);

                    if (sai.success)
                    {
                        UserAccounts NU = new UserAccounts();
                        NU.AccountName = New_WalletName.Text;
                        NU.BurstAddress = BurstAddress.Text;
                        NU.PassPhrase = StringCipher.Encrypt(SecretPhrase);
                        UADB.Save(NU);
                        UADB.RemoveWallet(UA);
                        WalletName.Text = NU.AccountName;
                        MessageDialog ConfirmationDetailsDialog = new MessageDialog("Wallets Name has been changed");
                        ConfirmationDetailsDialog.Title = "Confirmation";
                        ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                        var ConfResult = await ConfirmationDetailsDialog.ShowAsync();
                        PopulateWalletList();
                    }
                    else
                    {
                        MessageDialog ConfirmationDetailsDialog = new MessageDialog("Received Error:" + sai.errorMsg);
                        ConfirmationDetailsDialog.Title = "Error";
                        ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                        var ConfResult = await ConfirmationDetailsDialog.ShowAsync();
                    }
                }
                catch
                {
                    MessageDialog ConfirmationDetailsDialog = new MessageDialog("Received Error: A valid Burstcoin wallet needs to be present before attempting to change the name. Please create a new Burstcoin wallet first.");
                    ConfirmationDetailsDialog.Title = "Error";
                    ConfirmationDetailsDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                    var ConfResult = await ConfirmationDetailsDialog.ShowAsync();

                }
            }
            else
            {

            }
        }

        private void RefreshImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            WalletName.Text = "";
            BurstAddress.Text = "";
            Balance.Text = "";
            TransactionList_ListView.Items.Clear();

            PopulateWalletList();

        }

        private async void LoadMore_Tapped(object sender, TappedRoutedEventArgs e)
        {

            MessageDialog alertDialog = new MessageDialog("Loading more transactions may take a longer time to load, continue?");
            alertDialog.Title = "Confirmation";
            alertDialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            alertDialog.Commands.Add(new UICommand("No") { Id = 1 });
            alertDialog.DefaultCommandIndex = 0;
            alertDialog.CancelCommandIndex = 1;
            var result = await alertDialog.ShowAsync();
            if ((int)result.Id == 0)
            {
                GetTransactionListResult gtlr = BNWAPI.getTransactionList(BurstAddress.Text, TransactionNumber.Text);
                if (gtlr.success)
                {

                    TransactionList_ListView.Items.Clear();
                    //TransactionList_ListView.Items.Add("Date & Time" + "                    " + "Amount + Fee" + "     "+"Account" + "                                            " + "Confirmations");
                    for (int i = 0; i < gtlr.transactions.Length; i++)
                    {

                        DateTime date = UnixTimeStampToDateTime(gtlr.transactions[i].timestamp);
                        string transactionlist = gtlr.transactions[i].transaction;
                        string amount = gtlr.transactions[i].amountNQT;
                        double amnt = Convert.ToDouble(amount);
                        amnt = amnt / 100000000;
                        string newamnt = amnt.ToString("#,0.00000000");
                        string Fee = gtlr.transactions[i].feeNQT;
                        double feeamnt = Convert.ToDouble(Fee);
                        feeamnt = feeamnt / 100000000;
                        string newFeeAmnt = feeamnt.ToString("#,0");
                        string SenderAccount = gtlr.transactions[i].senderRS;
                        string Confirmations = gtlr.transactions[i].confirmations;
                        double Conf = Convert.ToDouble(Confirmations);
                        string Recipient = gtlr.transactions[i].recipientRS;
                        if (Recipient == BurstAddress.Text)
                        {
                            newamnt = "+" + newamnt;
                        }
                        else
                        {
                            newamnt = "-" + newamnt;
                            SenderAccount = Recipient;
                            if (SenderAccount == "")
                            {
                                SenderAccount = "/                                                      ";
                            }
                        }

                        if (Conf > 9)
                        {
                            Conf = 10;
                            string newConf = Conf.ToString() + "+";
                            TransactionList_ListView.Items.Add(date + "    " + newamnt + "+" + newFeeAmnt + "    " + SenderAccount + "     " + newConf);
                        }
                        else
                        {
                            string newConf = Conf.ToString();
                            TransactionList_ListView.Items.Add(date + "    " + newamnt + "+" + newFeeAmnt + "    " + SenderAccount + "     " + newConf);
                        }
                    }
                }
            }
            else
            {

            }
        }

        private async void Privacy_Policy_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(@"https://www.burstnation.com/index.php?windows-wallet-privacy-policy/");
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
