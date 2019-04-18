using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class SalesHeader : BaseItem
    {
        public string DocumentNo { get; set; }
        public string SellToCustomer { get; set; }
        public string SellToName { get; set; }
        public string BillToCustomer { get; set; }
        public string BillToName { get; set; }
        public string DocumentDate { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string StrSignature { get; set; }
        public string DocumentType { get; set; }
        public string Note { get; set; }
        public string NAVDocNo { get; set; }
        public string SourceType { get; set; }
        public string Comment { get; set; }
        public string FullCustomerName => string.Format("{0} - {1}", SellToCustomer, SellToName);
        public string IsVoid { get; set; }
        public string IsSync { get; set; } 
        public string SyncDateTime { get; set; }
        public string ExternalDocNo { get; set; }
        
    }
}
