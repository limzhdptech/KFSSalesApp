using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class SalesPrice : BaseItem
    {
        public int EntryNo { get; set; }
        public  string SalesType { get; set; }
        public string ItemNo { get; set; }
        public string SalesCode { get; set; }
        public decimal MinimumQty { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UOM { get; set; }
        public string PromotionType { get; set; }
        public string CustomerNo { get; set; }
    }
}
