using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public interface ISoapService
    {
       // string DeviceRegistration(string strMobileAccessKey, string deviceId);
        string DeviceRegistration(string strMobileAccessKey, string deviceId, string strEmail);
        string UserLogin(string mobileAccessKey, string deviceId, string userEmail, string password);
        DataTable GetLoginUsers(string mobileAccessKey);
        string CheckMobileAccessKey(string strMobileAccessKey);
        string ImportDataToNAV(string navtype, string nextstatus);
        DataTable GetItems();
        DataTable GetItemUOMs();
        DataTable GetVendors();
        DataTable GetItemSalesPrices(string salesPersonCode);
        DataTable GetCustomers(string salesPersonCode);
        DataTable GetCustomerPriceHistory(string custno, string itemno);
        DataTable GetCustomerPriceHistorywithCustNos(string strCustNos);
        DataTable GetCustLedgerEntry(string custno);
        DataTable GetPaymentMethods();
        DataTable GetSetupData();
        DataTable GetNumSerices(string deviceid, string spcode);
        DataTable GetRequestedHeader(string status);
        DataTable GetRequestedLines(string hdkey);
        DataTable GetRequestedPickedLines(string reqno);
        DataTable GetcontainerInfobyDocNo(string docno);
        DataTable GetcontainerInfobyDocLineNo(string docno, int lineno);
        DataTable GetItemBarCodes();
        DataTable GetContainerInfobyBoxNo(string boxno);
        string GetSalesPersonCode(string email);
        string GetGSTPercent();

        string ExportNumSeries(string deviceId, string spcode, string solastno, string crlastno, string cplastno, string rslastno, string ullastno, string soseries, string crseries, string cpseries, string rsseries, string ulseries);
        string ExportSalesHeader(string docNo, string selltocustomer, string selltoName, string billtoCustomer, string billtoName, string docDate, string status, string paymentMethod, decimal totalAmt, string doctype, string note, string strSingature, string salesPersonCode, string deviceId, string comment, string isvoid, string extdocno);
        string ExportSalesLine(string mEntryNo, string docNo, string itemNo, string locCode, decimal qty, decimal focQty, decimal badQty, string uom, decimal unitPrice, decimal lineDisPercent, decimal lineDisAmt, decimal lineAmt, string reasonCode, string badReasonCode, string itemType);
        string ExportPayment(string docNo, string onDate, string customerNo, string paymentMethod, decimal amount, string strSignature, string strImage, string salesPersonCode, string note, string recStatus, string refdocno, string sourcetype);
        string ExportRequestStock(string entryNo, string salesPersonCode, string requestNo, string requestDate, string curstatus);
        string ExportUnloadHistory(string mDeviceID, string mItemNo, decimal mUnloadQty, decimal mSOQty, decimal mCRQty, decimal mBalQty, string mSalesPersonCode);
        string UnloadHistoryReclasstoNAV(string mSalesPersonCode, string mDeviceID);
        string ExportRequestLine(string entryNo, string hdEntryNo, string userId, string itemNo, decimal qtyPerBag, decimal noofBags, decimal qty, decimal pickqty, decimal loadqty, decimal unloadqty, string uom, string vendorNo, bool inhouse, string requestId);
        string ExportNAVSalesHeader();
        string ExportNAVPayment();
        DataTable CreateDataTable<T>(IEnumerable<T> list);
        string UpdateContainerInfo(string boxno, decimal loadQty, decimal soldQty, decimal unloadQty);
        string ImportLoadAndUnloadToNAvbyRequestDocument(string navtype, string docno, string nextstatus);
        string ExportUnloadReturn(string mSalesPersonCode, string mItemNo, decimal mQSRetQty, string mToBin, string mSyncStatus);
        DataTable GetUnloadReturnList(string salesPersonCode);
        DataTable GetUserList();
        string UnloadedQSReclasstoNAV(string mSalesPersonCode);
        DataTable GetResonCodes();
        string ExportUnloadHeader(string entryNo, string salesPersonCode, string unloadDocNo, string unloadDatetime, string curstatus);
        string ExportUnloadLine(string entryNo, string hdEntryNo, string userId, string itemNo, string itemDesc, string itemUom, decimal qty, decimal badqty);

    }
}
