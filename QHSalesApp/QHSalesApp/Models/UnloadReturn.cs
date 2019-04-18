using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class UnloadReturn : BaseItem
    {
        public int EntryNo { get; set; }
        public string ItemNo { get; set; }
        public string ItemDesc { get; set; }
        public decimal Quantity { get; set; }
        public decimal QSReturnQty { get; set; }
        public string FromBin { get; set; }
        public string ToBin { get; set; }
        
    }
}
