using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public class Customer : BaseItem
    {
        public int EntryNo { get; set; }
        public string  CustomerNo { get; set; }
        public string  Country { get; set; }
        public string  SalesPersonCode { get; set; }
        public string  Name { get; set; }
        public string  Name2 { get; set; }
        public string  SearchName { get; set; }
        public string  Contact { get; set; }
        public string  Address { get; set; }
        public string  Address2 { get; set; }
        public string  City { get; set; }
        public string  Postcode { get; set; }
        public string  CountryCode { get; set; }
        public string  PhoneNo { get; set; }
        public string  MobileNo { get; set; }
        public string  TelexNo { get; set; }
        public string  FaxNo { get; set; }
        public string  Email { get; set; }
        public string  Website { get; set; }
        public string  CreditLimit { get; set; }
        public string  InvoiceLimit { get; set; }
        public string  Outstanding { get; set; }
        public string  CurrencyCode { get; set; }
        public string  PaymentTerms { get; set; }
        public string  CustomerPriceGroup { get; set; }
        public string  CustomerDiscGroup { get; set; }
        public string PaymentTermsDesc { get; set; }
        public string country_name { get; set; }
        public string billtoCustNo { get; set; }
    }

}
