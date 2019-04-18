using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class ScannedUnloadDoc : BaseItem
    {
        public string BagNo { get; set; }
        public string RequestDocNo { get; set; }
        public string RequestLineNo { get; set; }
        public decimal UnloadQty { get; set; }
    }
}
