using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class ContainerInfo : BaseItem
    {
        public string EntryNo { get; set; }
        public string PalletNo { get; set; }
        public string CartonNo { get; set; }
        public string BoxNo { get; set; }
        public int LineNo { get; set; }
        public string ItemNo { get; set; }
        public string VariantCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal LoadQty { get; set; }
        public decimal UnloadQty { get; set; }
        public decimal SoldQty { get; set; }
        public string LocationCode { get; set; }
        public string BinCode { get; set; }
        public string RefDocNo { get; set; }
        public string RefDocType { get; set; }
        public int RefDocLineNo { get; set; }
        public string MobileEntryNo { get; set; }

    }
}
