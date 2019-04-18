using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class User : BaseItem
    {
        public int EntryNo { get; set; }
        public int Default_CustEntryNo { get; set; }        
        public string Email { get; set; }
        public string UserID { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string pwd { get; set; }
        public bool Status { get; set; }
        public string Outlet_Loc { get; set; }
        public string WH_Loc { get; set; }
        public string SalesPersonCode { get; set;}
        public string DeviceID { get; set; }
    }
}
