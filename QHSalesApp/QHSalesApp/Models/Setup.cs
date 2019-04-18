using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class Setup : BaseItem
    {
 
        public string GSTPercent { get; set; }
        public string GSTRegNo { get; set; }
        public string SOPrefix { get; set; }
        public string CRPrefix { get; set; }
        public string CPPrefix { get; set; }
        public string RSPrefix { get; set; }
        public string ULPrefix { get; set; }
        public string StartNum { get; set; }
        public string Increment { get; set; }
        public string AdminPsw { get; set; }
        public string DeviceId { get; set; }
    }
}
