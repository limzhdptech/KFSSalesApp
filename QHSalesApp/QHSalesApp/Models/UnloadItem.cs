﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class UnloadItem : BaseItem
    {
        public string EntryNo { get; set; }
        public string HeaderEntryNo { get; set; }
        public string UserID { get; set; }
        public string ItemNo { get; set; }
        public string ItemDesc { get; set; }
        public decimal QtyperBag { get; set; }
        public decimal NoofBags { get; set; }
        public decimal Quantity { get; set; }
        public string UomCode { get; set; }
        public string VendorNo { get; set; }
        public bool InHouse { get; set; }
        public string RequestNo { get; set; }
        public string IsSync { get; set; }
        public string SyncDateTime { get; set; }
    }
}
