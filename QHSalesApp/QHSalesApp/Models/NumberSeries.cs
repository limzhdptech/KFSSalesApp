using System;

namespace QHSalesApp
{
    public class NumberSeries : BaseItem
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int Increment { get; set; }
        public string LastNoCode { get; set; }
        public int LastNoSeries { get; set; }
    }
}
