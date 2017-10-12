using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using System.Net.Http;

using System.Net.Http.Headers;
using ModernHttpClient;
using Windows.UI.Popups;

namespace BNWallet_Windows
{
    class BNWalletAPI
    {

        private HttpClient client;
        private string BaseAddr { get; set; }
        public bool IsSuccessfull { get; set; }
        public string ErrMesg { get; set; }
        public string ErrCode { get; set; }
        FormUrlEncodedContent fp = null;

        public BNWalletAPI()
        {
            client = new HttpClient(new NativeMessageHandler());
            BaseAddr = "https://wallet1.burstnation.com:8125";
            client.BaseAddress = new System.Uri(BaseAddr);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ErrMesg = "";
            ErrCode = "";
            
        }

        public GetAccountIDResult getAccountID(string secretPhrase, string publicKey)
        {
            GetAccountIDResult GAIR = new GetAccountIDResult();
            BNWalletAPIClasses.ErrorCodes er;
            HttpResponseMessage resp = new HttpResponseMessage();
            string respStr;

            fp = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("requestType","getAccountId"),
                new KeyValuePair<string, string>("secretPhrase",secretPhrase),
                new KeyValuePair<string, string>("publicKey",publicKey)

            });

            try
            {
                 resp = client.PostAsync("burst", fp).Result;
                 respStr = resp.Content.ReadAsStringAsync().Result;
            }
            catch
           
            { 
                respStr = null;
            }
            
            if (!string.IsNullOrEmpty(respStr))
            {
                if (respStr.Contains("\"errorCode\":"))
                {
                    try
                    {
                        er = JsonConvert.DeserializeObject<BNWalletAPIClasses.ErrorCodes>(respStr);
                        GAIR.success = false;
                        GAIR.errorMsg = er.errorDescription;
                    }
                    catch (Exception e)
                    {
                        GAIR.success = false;
                        GAIR.errorMsg = "Exception Error Deserializing Error Code: " + e.Message;
                    }
                    //GAIR.errorMsg = fp;
                }
                else
                {
                    try
                    {
                        BNWalletAPIClasses.GetAccountIDResponse gair = JsonConvert.DeserializeObject<BNWalletAPIClasses.GetAccountIDResponse>(respStr);
                        GAIR.success = true;
                        GAIR.accountRS = gair.accountRS;
                        GAIR.account = gair.account;
                    }
                    catch (Exception e)
                    {
                        GAIR.success = false;
                        GAIR.errorMsg = "Exception Error Deserializing GetAccountIDResponse: " + e.Message;
                    }
                }
            }
            else
            {
                GAIR.success = false;
                GAIR.errorMsg = "Cannot get Wallet info as Online wallet is possibly offline";
            }
            return GAIR;

        }

        public GetAccountResult getAccount(string WalletID)
        {
            GetAccountResult GAR = new GetAccountResult();
            BNWalletAPIClasses.ErrorCodes er;
            HttpResponseMessage resp = new HttpResponseMessage();
            string respStr;

            fp = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("requestType","getAccount"),
                new KeyValuePair<string, string>("account",WalletID)

            });

            try
            {
                resp = client.PostAsync("burst", fp).Result;
                respStr = resp.Content.ReadAsStringAsync().Result;
            }
            catch

            {
                respStr = null;
            }
            if (!string.IsNullOrEmpty(respStr))
            {
                if (respStr.Contains("\"errorCode\":"))
                {
                    er = JsonConvert.DeserializeObject<BNWalletAPIClasses.ErrorCodes>(respStr);
                    GAR.success = false;
                    GAR.errorMsg = er.errorDescription;

                }
                else
                {
                    BNWalletAPIClasses.GetAccountResponse gar = JsonConvert.DeserializeObject<BNWalletAPIClasses.GetAccountResponse>(respStr);
                    GAR.success = true;
                    GAR.accountRS = gar.accountRs;
                    GAR.name = gar.name;
                    GAR.balanceNQT = gar.balanceNQT;

                }
            }
            else
            {
                GAR.success = false;
                GAR.errorMsg = "Receive blank response from API call";
            }
            return GAR;

        }

        public SetAccountInfo setAccountInfo(string name,string secretphrase)
        {
            SetAccountInfo SAI = new SetAccountInfo();
            BNWalletAPIClasses.ErrorCodes er;
            HttpResponseMessage resp = new HttpResponseMessage();
            string respStr;

            fp = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("requestType","setAccountInfo"),
                new KeyValuePair<string, string>("account",name),
                new KeyValuePair<string, string>("secretPhrase",secretphrase),
                new KeyValuePair<string, string>("feeNQT","100000000"),
                new KeyValuePair<string, string>("deadline","60"),

            });

            try
            {
                resp = client.PostAsync("burst", fp).Result;
                respStr = resp.Content.ReadAsStringAsync().Result;
            }
            catch

            {
                respStr = null;
            }
            if (!string.IsNullOrEmpty(respStr))
            {
                if (respStr.Contains("\"errorCode\":"))
                {
                    er = JsonConvert.DeserializeObject<BNWalletAPIClasses.ErrorCodes>(respStr);
                    SAI.success = false;
                    SAI.errorMsg = er.errorDescription;

                }
                else
                {
                    BNWalletAPIClasses.SetAccountInfo sai = JsonConvert.DeserializeObject<BNWalletAPIClasses.SetAccountInfo>(respStr);
                    SAI.success = true;
                    

                }
            }
            else
            {
                SAI.success = false;
                SAI.errorMsg = "Receive blank response from API call";
            }
            return SAI;

        }

        public GetsendMoneyResult sendMoney(string BurstAddress, string amountNQT, string feeNQT, string secretPhrase, string message, bool encrypmf)
        {
            GetsendMoneyResult SMR = new GetsendMoneyResult();
            BNWalletAPIClasses.ErrorCodes er;
            HttpResponseMessage resp = new HttpResponseMessage();
            string respStr;

            Dictionary<string, string> fpDict = new Dictionary<string, string>()
            {
                { "requestType","sendMoney" },
                { "secretPhrase",secretPhrase },
                { "recipient",BurstAddress },
                { "amountNQT",amountNQT },
                { "feeNQT","100000000" },
                { "deadline","60" }

            };
            if (encrypmf)
                fpDict.Add("messageToEncrypt", message);
            else
                fpDict.Add("message", message);
            fp = new FormUrlEncodedContent(fpDict);

            try
            {
                resp = client.PostAsync("burst", fp).Result;
                respStr = resp.Content.ReadAsStringAsync().Result;
            }
            catch

            {
                respStr = null;
            }
            if (!string.IsNullOrEmpty(respStr))
            {
                if (respStr.Contains("\"errorCode\":"))
                {
                    er = JsonConvert.DeserializeObject<BNWalletAPIClasses.ErrorCodes>(respStr);
                    SMR.success = false;
                    SMR.errorMsg = er.errorDescription;
                    //GAIR.errorMsg = fp;
                }
                else
                {
                    BNWalletAPIClasses.sendMoneyResponse smr = JsonConvert.DeserializeObject<BNWalletAPIClasses.sendMoneyResponse>(respStr);
                    SMR.success = true;
                    SMR.signatureHash = smr.signatureHash;
                    SMR.transaction = smr.transaction;
                }
            }
            else
            {
                SMR.success = false;
                SMR.errorMsg = "Receive blank response from API call";
            }
            return SMR;

        }

        public GetTransactionResult getTransaction(string transaction)
        {
            GetTransactionResult GTR = new GetTransactionResult();
            BNWalletAPIClasses.ErrorCodes er;
            HttpResponseMessage resp = new HttpResponseMessage();
            string respStr;

            fp = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("requestType","getTransaction"),
                new KeyValuePair<string, string>("transaction",transaction)
            });

            try
            {
                resp = client.PostAsync("burst", fp).Result;
                respStr = resp.Content.ReadAsStringAsync().Result;
            }
            catch

            {
                respStr = null;
            }
            if (!string.IsNullOrEmpty(respStr))
            {
                if (respStr.Contains("\"errorCode\":"))
                {
                    er = JsonConvert.DeserializeObject<BNWalletAPIClasses.ErrorCodes>(respStr);
                    GTR.success = false;
                    GTR.errormsg = er.errorDescription;
                    //GAIR.errorMsg = fp;
                }
                else
                {
                    BNWalletAPIClasses.getTransactionResponse gtr = JsonConvert.DeserializeObject<BNWalletAPIClasses.getTransactionResponse>(respStr);
                    GTR.success = true;
                    GTR.feeNQT = gtr.feeNQT;
                    GTR.senderRS = gtr.senderRS;
                    GTR.amountNQT = gtr.amountNQT;
                    GTR.recipientRS = gtr.recipientRS;
                    GTR.ecBlockHeight = gtr.ecBlockHeight;
                }
            }
            else
            {
                GTR.success = false;
                GTR.errormsg = "Receive blank response from API call";
            }
            return GTR;

        }

        public GetMiningInfo getMiningInfo()
        {
            GetMiningInfo GMI = new GetMiningInfo();
            BNWalletAPIClasses.ErrorCodes er;
            HttpResponseMessage resp = new HttpResponseMessage();
            string respStr;

            fp = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("requestType","getMiningInfo"),


            });

            try
            {
                resp = client.PostAsync("burst", fp).Result;
                respStr = resp.Content.ReadAsStringAsync().Result;
            }
            catch

            {
                respStr = null;
            }
            if (!string.IsNullOrEmpty(respStr))
            {
                if (respStr.Contains("\"errorCode\":"))
                {
                    er = JsonConvert.DeserializeObject<BNWalletAPIClasses.ErrorCodes>(respStr);
                    GMI.success = false;
                    GMI.errorMsg = er.errorDescription;

                }
                else
                {
                    BNWalletAPIClasses.GetMiningInfoResult gmir = JsonConvert.DeserializeObject<BNWalletAPIClasses.GetMiningInfoResult>(respStr);
                    GMI.success = true;
                    GMI.height = gmir.height;


                }
            }
            else
            {
                GMI.success = false;
                GMI.errorMsg = "Receive blank response from API call";
            }
            return GMI;

        }

        public GetTransactionListResult getTransactionList(string BurstAddress,string lastIndex)
        {
            GetTransactionListResult GTL = new GetTransactionListResult();
            BNWalletAPIClasses.ErrorCodes er;
            HttpResponseMessage resp = new HttpResponseMessage();
            string respStr;
            double LastIndex = Convert.ToDouble(lastIndex);
            LastIndex = LastIndex - 1;
            lastIndex = LastIndex.ToString();

            fp = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("requestType","getAccountTransactions"),
                new KeyValuePair<string,string>("account",BurstAddress),
                new KeyValuePair<string,string>("lastIndex",lastIndex),
            });

            try
            {
                resp = client.PostAsync("burst", fp).Result;
                respStr = resp.Content.ReadAsStringAsync().Result;
            }
            catch

            {
                respStr = null;
            }
            if (!string.IsNullOrEmpty(respStr))
            {
                if (respStr.Contains("\"errorCode\":"))
                {
                    er = JsonConvert.DeserializeObject<BNWalletAPIClasses.ErrorCodes>(respStr);
                    GTL.success = false;
                    GTL.errormsg = er.errorDescription;
                }
                else
                {
                    BNWalletAPIClasses.GetTransactions gtlr = JsonConvert.DeserializeObject<BNWalletAPIClasses.GetTransactions>(respStr);
                    GTL.success = true;
                    GTL.transactions = gtlr.transactions;
                }
            }
            else
            {
                GTL.success = false;
                GTL.errormsg = "Receive blank response from API call";
            }
            return GTL;

        }

    }



    public class GetAccountIDResult
    {
        public bool success { get; set; }
        public string accountRS { get; set; }
        public string account { get; set; }
        public string errorMsg { get; set; }

        public GetAccountIDResult()
        {
            success = false;
            accountRS = "";
            account = "";
            errorMsg = "";

        }
    }

    public class GetAccountResult
    {
        public bool success { get; set; }
        public string accountRS { get; set; }
        public string name { get; set; }
        public string balanceNQT { get; set; }
        public string errorMsg { get; set; }

        public GetAccountResult()
        {
            success = false;
            accountRS = "";
            name = "";
            balanceNQT = "";
            errorMsg = "";

        }
    }

    public class SetAccountInfo
    {
        public bool success { get; set; }
        public string errorMsg { get; set; }

        public SetAccountInfo()
        {
            success = false;
            errorMsg = "";

        }
    }

    public class GetsendMoneyResult
    {
        public bool success { get; set; }
        public string signatureHash { get; set; }
        public string transaction { get; set; }
        public string errorMsg { get; set; }

        public GetsendMoneyResult()
        {
            success = false;
            signatureHash = "";
            transaction = "";
            errorMsg = "";
        }
    }

    public class GetMiningInfo
    {
        public bool success { get; set; }
        public string height { get; set; }
        public string errorMsg { get; set; }

        public GetMiningInfo()
        {
            success = false;
            height = "";
            errorMsg = "";

        }
    }

    public class GetTransactionResult
    {
        public bool success { get; set; }
        public string feeNQT { get; set; }
        public string senderRS { get; set; }
        public string amountNQT { get; set; }
        public string recipientRS { get; set; }
        public string ecBlockHeight { get; set; }
        public string errormsg { get; set; }

        public GetTransactionResult()
        {
            success = false;
            feeNQT = "";
            senderRS = "";
            amountNQT = "";
            recipientRS = "";
            ecBlockHeight = "";
            errormsg = "";
        }



    }

    public class GetTransactionListResult
    {
        public bool success { get; set; }
        public Transactions[] transactions { get; set; }
        public string errormsg { get; set; }

        public GetTransactionListResult()
        {
            success = false;
            transactions = null;
            errormsg = "";
        }
    }
}