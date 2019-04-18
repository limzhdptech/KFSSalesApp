using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public  class ServiceManager
    {
        ISoapService soapService;

        public ServiceManager(ISoapService svc)
        {
            soapService = svc;
        }

        public string DeviceRegister(string accesskey, string deviceId,string strEmail)
        {
            return soapService.DeviceRegistration(accesskey, deviceId,strEmail);
        }

        public string CheckDeviceAccessKey(string accesskey)
        {
            return soapService.CheckMobileAccessKey(accesskey);
        }

        public string CheckUserLogin(string mobileAccessKey,string deviceId, string email, string password)
        {
            return soapService.UserLogin(mobileAccessKey,deviceId ,email, password);
        }

        public string RetGSTPercent()
        {
            return soapService.GetGSTPercent();
        }

        public string RetSalesPersonCode(string email)
        {
            return soapService.GetSalesPersonCode(email);
        }
        public DataTable RetSetup()
        {
            return soapService.GetSetupData();
        }

        public DataTable RetNumSeries(string deviceId,string salesperson)
        {
            return soapService.GetNumSerices(deviceId, salesperson);
        }

        //public async Task<DataTable> RetLoginData(string usrEmail, string usrPassword)
        //{
        //    DataTable dt = new DataTable();
        //    dt = soapService.get
        //    return dt;
        //}

        public DataTable RetLoginUsers(string mobileAccessKey)
        {
            return soapService.GetLoginUsers(mobileAccessKey);
        }
        public string ImportDataToNAV(string navtype, string nextstatus)
        {
            return soapService.ImportDataToNAV(navtype, nextstatus);
        }
        public DataTable RetItems()
        {
            return soapService.GetItems();
        }

        public DataTable RetItemUOMs()
        {
            return soapService.GetItemUOMs();
        }

        public DataTable RetItemSalesPrices(string salesPersonCode)
        {
            return soapService.GetItemSalesPrices(salesPersonCode);
        }

        public DataTable RetCustomers(string salesPersonCode)
        {
            return soapService.GetCustomers(salesPersonCode);
        }

        
        public DataTable RetVendors()
        {
            return soapService.GetVendors();
        }

        public DataTable RetItemBarcode()
        {
            return soapService.GetItemBarCodes();
        }

        public async Task <DataTable> RetCustomerPriceHistory(string custno,string itemno)
        {
            return soapService.GetCustomerPriceHistory(custno,itemno);
        }

        public DataTable RetCustomerPriceHistorywithCustNos(string strCustNos)
        {
            return soapService.GetCustomerPriceHistorywithCustNos(strCustNos);
        }

        public DataTable RetCustomerLedgerEntry(string custno)
        {
            return soapService.GetCustLedgerEntry(custno);
        }

        public DataTable RetRequestHeaderbyStatus(string status)
        {
            return soapService.GetRequestedHeader(status);
        }

        public DataTable RetRequestedLines(string hdkey)
        {
            return soapService.GetRequestedLines(hdkey);
        }

        public DataTable RetRequestedPickedLines(string reqno)
        {
            return soapService.GetRequestedPickedLines(reqno);
        }

        public DataTable RetContainerInfobyDocNo(string docno)
        {
            return soapService.GetcontainerInfobyDocNo(docno);
        }

        public DataTable RetContainerInfobyBoxNo(string boxno)
        {
            return soapService.GetContainerInfobyBoxNo(boxno);
        }
        public DataTable RetContainerInfobyDocLineNo(string docno, int lineno)
        {
            return soapService.GetcontainerInfobyDocLineNo(docno,lineno);
        }

        public DataTable RetPaymentMethods()
        {
            return soapService.GetPaymentMethods();
        }

        public DataTable RetReasonCodes()
        {
            return soapService.GetResonCodes();
        }

        public string ExportSalesHeader(string docNo, string selltocustomer, string selltoName, string billtoCustomer, string billtoName, string docDate, string status, string paymentMethod, decimal totalAmt,string doctype,string note, string strSingature,string salesPersonCode,string deviceId,string comment,string isvoid,string extdocno)
        {
            return soapService.ExportSalesHeader(docNo,selltocustomer,selltoName,billtoCustomer,billtoName,docDate,status,paymentMethod,totalAmt,doctype,note,strSingature,salesPersonCode,deviceId,comment,isvoid,extdocno);
        }

        public string ExportSalesLine(string mEntryNo,string docNo, string itemNo, string locCode, decimal qty, decimal focQty,decimal badQty, string uom, decimal unitPrice, decimal lineDisPercent, decimal lineDisAmt, decimal lineAmt,string reasonCode,string badReasonCode,string itemType)
        {
            return soapService.ExportSalesLine(mEntryNo,docNo, itemNo, locCode, qty, focQty,badQty, uom, unitPrice, lineDisPercent, lineDisAmt, lineAmt,reasonCode, badReasonCode,itemType);
        }

        public string ExportPayment(string docNo, string onDate, string customerNo, string paymentMethod, decimal amount, string strSignature, string strImage, string salesPersonCode, string note, string recStatus,string refdocno,string sourcetype)
        {
            return soapService.ExportPayment(docNo, onDate, customerNo, paymentMethod, amount, strSignature, strImage, salesPersonCode, note, recStatus,refdocno,sourcetype);
        }

        public string ExportRequestStock(string entryNo, string salesPersonCode, string requestNo, string requestDate,string curstatus)
        {
            return soapService.ExportRequestStock(entryNo, salesPersonCode, requestNo, requestDate,curstatus);
        }
        public string ExportRequestLine(string entryNo, string hdEntryNo, string userId, string itemNo, decimal qtyPerBag, decimal noofBags, decimal qty,decimal pickqty, decimal loadqty,decimal unloadqty,
          string uom, string vendorNo, bool inhouse, string requestId)
        {
            return soapService.ExportRequestLine(entryNo, hdEntryNo, userId, itemNo, qtyPerBag, noofBags, qty,pickqty,loadqty,unloadqty,uom, vendorNo, inhouse, requestId);
        }
        public string ExportNumSeries(string deviceId, string spcode, string solastno, string crlastno, string cplastno,string rslastno,string ullastno,string soseries,string crseries,string cpseries,string rsseries,string ulseries)
        {
            return soapService.ExportNumSeries(deviceId, spcode, solastno, crlastno, cplastno,rslastno,ullastno,soseries,crseries,cpseries,rsseries,rsseries);
        }

        public string ExportNAVSalesHeader()
        {
            return soapService.ExportNAVSalesHeader();
        }
        public string ExportNAVPayment()
        {
            return soapService.ExportNAVPayment();
        }
        public DataTable ConvertDataTable<T>(IEnumerable<T> list)
        {
            return soapService.CreateDataTable<T>(list);
        }

        public string UpdateContainerInfo(string bagno,decimal LoadQty,decimal SoldQty,decimal UnloadQty)
        {
            return soapService.UpdateContainerInfo(bagno, LoadQty, SoldQty, UnloadQty);
        }

        public string ImportLoadandUploadbyRequestDocument(string navtype,string docno,string nextstatus)
        {
            return soapService.ImportLoadAndUnloadToNAvbyRequestDocument(navtype, docno, nextstatus);
        }

        public string ExportUnloadHistory(string mDeviceID, string mItemNo, decimal mUnloadQty, decimal mSOQty, decimal mCRQty, decimal mBalQty, string mSalesPersonCode)
        {
            return soapService.ExportUnloadHistory(mDeviceID, mItemNo, mUnloadQty, mSOQty, mCRQty, mBalQty, mSalesPersonCode);
        }

        public string UnloadHistoryReclasstoNAV(string mSalesPersonCode, string mDeviceID)
        {
            return soapService.UnloadHistoryReclasstoNAV(mSalesPersonCode, mDeviceID);
        }

        public string UnloadQSReclasstoNAV(string mSalesPersonCode)
        {
            return soapService.UnloadedQSReclasstoNAV(mSalesPersonCode);
        }
        public string ExportUnloadReturn(string mSalesPersonCode, string mItemNo, decimal mQSRetQty, string mToBin, string mSyncStatus)
        {
            return soapService.ExportUnloadReturn(mSalesPersonCode, mItemNo, mQSRetQty, mToBin, mSyncStatus);
        }
        public DataTable GetUnloadReturnbySalesPerson(string SalesPersonCode)
        {
            return soapService.GetUnloadReturnList(SalesPersonCode);
        }

        public DataTable GetUserList()
        {
            return soapService.GetUserList();
        }

        public string ExportUnloadHeader(string entryNo, string salesPersonCode, string unloadDocNo, string unloadDatetime, string curstatus)
        {
            return soapService.ExportUnloadHeader(entryNo, salesPersonCode, unloadDocNo, unloadDatetime, curstatus);
        }
        public string ExportUnloadLine(string entryNo, string hdEntryNo, string userId, string itemNo, string itemDesc,string itemUom, decimal qty, decimal badqty)
        {
            return soapService.ExportUnloadLine(entryNo, hdEntryNo, userId, itemNo,itemDesc,itemUom,qty,badqty);
        }
    }
}
