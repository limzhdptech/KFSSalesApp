using System;

namespace QHSalesApp
{
    public class MasterPageItem
    {
        public string Title { get; set; }

        public string IconSource { get; set; }

        public Type TargetType { get; set; }

        public int PageEntryNo { get; set; }
    }
}
