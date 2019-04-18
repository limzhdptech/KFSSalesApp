using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class DeviceInfo : BaseItem
    {
        public string DeviceName { get; set; }
        public string Address { get; set; }
        public string DeviceUniqueId { get; set; }
        public string Port { get; set; }
    }
}
