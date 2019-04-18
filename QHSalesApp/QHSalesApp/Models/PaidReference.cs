using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class PaidReference : BaseItem
    {
        public string DocumentNo { get; set; }
        public decimal Amount { get; set; }
        public string SourceType { get; set; }
    }
}
