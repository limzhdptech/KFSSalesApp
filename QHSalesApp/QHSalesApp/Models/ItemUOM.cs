using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class ItemUOM : BaseItem
    {
        public int EntryNo { get; set; }
        public string ItemNo { get; set; }
        public string UOMCode { get; set; }
    }
}
