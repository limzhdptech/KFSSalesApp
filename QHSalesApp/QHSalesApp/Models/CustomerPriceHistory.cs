using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class CustomerPriceHistory : BaseItem
    {
        public string CustNo { get; set; }
        public string ItemNo { get; set; }
        public string UOM { get; set; }
        public string Currency { get; set; }
        public string UnitPrice { get; set; }
        public string Qty { get; set; }
        public string TransDate { get; set; }
        public string UnitPrice2 { get; set; }
        public string Qty2 { get; set; }
        public string TransDate2 { get; set; }
        public string unitPrice3 { get; set; }
        public string Qty3 { get; set; }
        public string TransDate3 { get; set; }
    }
}
