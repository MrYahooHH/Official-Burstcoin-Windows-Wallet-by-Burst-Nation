using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNWallet_Windows
{
    public class Transactions
    {
        public double timestamp { get; set; }
        public string transaction { get; set; }
        public string amountNQT { get; set; }
        public string feeNQT { get; set; }
        public string senderRS { get; set; }
        public string confirmations { get; set; }
        public string recipientRS { get; set; }

        public Transactions()
        {
            timestamp = 0;
            transaction = "";
            amountNQT = "";
            feeNQT = "";
            senderRS = "";
            confirmations = "";
            recipientRS = "";
        }
    }
}
