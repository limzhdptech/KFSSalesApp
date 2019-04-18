using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class UnloadLine: BaseItem
    {
        public string EntryNo { get; set; }
        public string HeaderEntryNo { get; set; }
        public string UserID { get; set; }
        public string ItemNo { get; set; }
        public string ItemDesc { get; set; }
        public decimal Quantity { get; set; }
        public decimal GoodQty { get; set; }
        public decimal BadQty { get; set; }
        public string ItemUom { get; set; }
        public string IsSync { get; set; }
        public string SyncDateTime { get; set; }
    }
}
