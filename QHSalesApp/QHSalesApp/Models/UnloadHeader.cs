﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class UnloadHeader : BaseItem
    {
        public string EntryNo { get; set; }
        public string SalesPersonCode { get; set; }
        public string UnloadDocNo { get; set; }
        public string UnloadDate { get; set; }
        public string IsSync { get; set; }
        public string SyncDateTime { get; set; }
        public string CurStatus { get; set; }
    }
}
