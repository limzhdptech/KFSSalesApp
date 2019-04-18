using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class Payment : BaseItem
    {
        public string DocumentNo { get; set; }
        public string OnDate { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string CustomerSignature { get; set; }
        public string SalesPersonCode { get; set; }
        public string Note { get; set; }
        public string RecStatus { get; set; }
        public string Imagestr { get; set; }
        public string RefDocumentNo { get; set; }
        public string SourceType { get; set; }
        public string FullCustomerName => string.Format("{0} - {1}", CustomerNo, CustomerName);
        //public string IsSync { get; set; }
        //public string SyncDateTime { get; set; }
    }

    //public class PaymentList: List<Payment>
    //{
    //    public string HeaderDate { get; set; }
    //    public List<Payment> payments => this;
    //}
}
