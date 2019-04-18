using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class SalesLine : BaseItem
    {
        public string DocumentNo { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string LocationCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal BadQuantity { get; set; }
        public string TotalQty { get { return (BadQuantity + Quantity).ToString()+" "+ UnitofMeasurementCode; } }
        public string UnitofMeasurementCode { get; set; }
        public decimal FOCQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineDiscountPercent { get; set; }
        public decimal LineDiscountAmount { get; set; }
        public decimal LineAmount { get { return UnitPrice * (BadQuantity+ Quantity); } }
        public string BagNo { get; set; }
        public string GoodReasonCode { get; set; }
        public string BadReasonCode { get; set; }
        public string ItemType { get; set; } // SALE,FOC,EXC
        //public string IsSync { get; set; }
        //public string SyncDateTime { get; set; }
    }
}
