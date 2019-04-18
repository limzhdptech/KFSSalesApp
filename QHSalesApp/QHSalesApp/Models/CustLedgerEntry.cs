using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class CustLedgerEntry : BaseItem
    {
        public string TransType { get; set; }
        public string DocNo { get; set; }
        public string CustNo { get; set; }
        public string ExtDocNo { get; set; }
        public string IsOpenItem { get; set; }
        public string TransDate { get; set; }
        public string PaymentTerm { get; set; }
        public string InvoiceAmount { get; set; }
        public string PaidAmount { get; set; }
        public string UnpaidAmount { get; set; }
    }
}
