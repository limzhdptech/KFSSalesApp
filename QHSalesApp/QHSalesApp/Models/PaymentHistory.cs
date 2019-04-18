using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class PaymentHistory : BaseItem
    {
        public string DocumentDate { get; set; }
        public string DocumentNo { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
    }
}
