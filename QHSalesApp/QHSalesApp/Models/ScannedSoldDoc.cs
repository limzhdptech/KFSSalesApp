using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class ScannedSoldDoc : BaseItem
    {
        public string BagNo { get; set; }
        public string RequestDocNo { get; set; }
        public string RequestLineNo { get; set; }
        public decimal SoldQty { get; set; }
    }
}
