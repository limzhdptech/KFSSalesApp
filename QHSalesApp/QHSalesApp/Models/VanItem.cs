using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class VanItem : BaseItem
    {
        public string ItemNo { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }
        public string BaseUOM { get; set; }
        public string UnitPrice { get; set; }
        public string Str64Img { get; set; }
        public decimal LoadQty { get; set; }
        public decimal SoldQty { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal BadQty { get; set; }
        public decimal UnloadQty { get; set; }
        public decimal Balance { get; set; }
        public decimal BalQty => LoadQty + ReturnQty + BadQty - SoldQty;

        
    }
}
