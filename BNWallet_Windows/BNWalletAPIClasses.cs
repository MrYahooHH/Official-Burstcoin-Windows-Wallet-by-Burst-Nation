using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BNWallet_Windows
{
    class BNWalletAPIClasses
    {

        public class GetAccountIDResponse
        {
            public string accountRS { get; set; }
            public string publicKey { get; set; }
            public int requestProcessingTime { get; set; }
            public string account { get; set; }

        }

        public class GetAccountResponse
        {
            public string accountRs { get; set; }
            public string name { get; set; }
            public string balanceNQT { get; set; }

        }

        public class sendMoneyResponse
        {
            public string signatureHash { get; set; }
            public string transaction { get; set; }
        }

        public class getTransactionResponse
        {
            public string feeNQT { get; set; }
            public string senderRS { get; set; }
            public string amountNQT { get; set; }
            public string recipientRS { get; set; }
            public string ecBlockHeight { get; set; }
        }

        public class ErrorCodes
        {
            public string errorDescription { get; set; }
            public int errorCode { get; set; }
        }
        public class GetMiningInfoResult
        {
            public string height { get; set; }
        }

        public class GetTransactions
        {
            public Transactions[] transactions { get; set; }
        }

        public class SetAccountInfo
        {
            
        }
    }
}