using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public interface IBluetoothPrinter
    {
        string Print(string deviceName, string printText);
    }
}
