using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class LoadItem : INotifyPropertyChanged
    {
        int id;
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }
        string entryNo;
        public string EntryNo
        {
            get { return entryNo; }
            set { entryNo = value;
                NotifyPropertyChanged("EntryNo");
            }
        }

        string headerEntryNo;
        public string HeaderEntryNo
        {
            get { return headerEntryNo; }
            set
            {
                headerEntryNo = value;
                NotifyPropertyChanged("HeaderEntryNo");
            }
        }

        string userId;
        public string UserID
        {
            get { return userId; }
            set
            {
                userId = value;
                NotifyPropertyChanged("UserID");
            }
        }

        string itemNo;
        public string ItemNo
        {
            get { return itemNo; }
            set
            {
                itemNo = value;
                NotifyPropertyChanged("ItemNo");
            }
        }

        string itemDesc;
        public string ItemDesc
        {
            get { return itemDesc; }
            set
            {
                itemDesc = value;
                NotifyPropertyChanged("ItemDesc");
            }
        }

        decimal qtyPerBag;
        public decimal QtyPerBag
        {
            get { return qtyPerBag; }
            set
            {
                qtyPerBag = value;
                NotifyPropertyChanged("QtyPerBag");
            }
        }

        decimal noofBages;
        public decimal NoofBags
        {
            get { return noofBages; }
            set
            {
                noofBages = value;
                NotifyPropertyChanged("NoofBags");
            }
        }

        decimal quantity;
        public decimal Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                NotifyPropertyChanged("Quantity");
            }
        }

        decimal pickQty;
        public decimal PickQty
        {
            get { return pickQty; }
            set
            {
                pickQty = value;
                NotifyPropertyChanged("PickQty");
            }
        }

        decimal loadQty;
        public decimal LoadQty
        {
            get { return loadQty; }
            set
            {
                loadQty = value;
                NotifyPropertyChanged("LoadQty");
            }
        }

        string uomCode;
        public string UomCode
        {
            get { return uomCode; }
            set
            {
                uomCode = value;
                NotifyPropertyChanged("UomCode");
            }
        }

        string vendorNo;
        public string VendorNo
        {
            get { return vendorNo; }
            set
            {
                vendorNo = value;
                NotifyPropertyChanged("VendorNo");
            }
        }

        string requestNo;
        public string RequestNo
        {
            get { return requestNo; }
            set
            {
                requestNo = value;
                NotifyPropertyChanged("RequestNo");
            }
        }

        bool inhouse;
        public bool InHouse
        {
            get { return inhouse; }
            set
            {
                inhouse = value;
                NotifyPropertyChanged("InHouse");
            }
        }

        string isSync;
        public string IsSync
        {
            get { return isSync; }
            set
            {
                isSync = value;
                NotifyPropertyChanged("IsSync");
            }
        }

        string syncDateTime;
        public string SyncDateTime
        {
            get { return syncDateTime; }
            set
            {
                syncDateTime = value;
                NotifyPropertyChanged("SyncDateTime");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
       
    }
}
