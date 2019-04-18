using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class Item : BaseItem
    {
        public int EntryNo { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string BaseUOM { get; set; }
        public string UnitPrice { get; set; }
        public string CategoryCode { get; set; }
        public string Str64Img { get; set; }
        public decimal InvQty { get; set; }
        public decimal LoadQty { get; set; }
        public decimal SoldQty { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal BadQty { get; set; }
        public decimal Balance { get; set; }
        public decimal UnloadQty { get; set; }
        public decimal BalQty => LoadQty + ReturnQty+ BadQty - SoldQty;

        public decimal AvailableQty => LoadQty + ReturnQty - SoldQty;
        public string BarCode { get; set; }
        public string IsActive { get; set; }
        //public string FullCustomerName => string.Format("{0} - {1}", CustomerNo, CustomerName);
    }

    public class ChangedItem: BaseItem
    {
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
    }
}
