using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class PaymentReference : BaseItem
    {
        public string DocumentNo { get; set; }
        public string DocumentDate { get; set; }
        public string Amount { get; set; }
        public string SourceType { get; set; }
    }

}
