using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data;
using System.Reflection;
using QHSalesApp;

namespace QHSalesApp.Droid
{
    public class SoapService : ISoapService
    {
        public AppSvc.WebService service { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SoapService()
        {
            GetNavUrl();
        }

        private void GetNavUrl()
        {
            //if(service!=null) service.Dispose();
            service = new AppSvc.WebService();
            string setting_url = "";
            try
            {
                setting_url = Helpers.Settings.GeneralSettings;
            }
            catch (System.NullReferenceException) { }
            catch (System.Collections.Generic.KeyNotFoundException) { }

            if (setting_url == "")
            {
                service.Url = Constants.SoapUrl; //dummy add for avoid nullrefexception 
            }
            else
            {
                service.Url = setting_url;
            }
            // service.Credentials = new NetworkCredential("admin", "bingo28*", "dptech");
        }

        public string DeviceRegistration(string strMobileAccessKey, string deviceId,string strEmail)
        {
            try
            {
                service.Dispose();
                GetNavUrl();
                return service.RegisterDevicebySalePerson(strMobileAccessKey, deviceId,strEmail);
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }

        }
        public string CheckMobileAccessKey(string strMobileAccessKey)
        {
            try
            {
                service.Dispose();
                GetNavUrl();
                return service.CheckMobileAccessKey(strMobileAccessKey);
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public string UserLogin(string mobileAccessKey, string deviceId,string userEmail,string password)
        {
            try
            {
                service.Dispose();
                GetNavUrl();
                return service.UserLogin(mobileAccessKey, deviceId, userEmail, password);
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        public DataTable GetLoginUsers(string mobileAccessKey)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetMobileUsers(mobileAccessKey);
        }

        public string ImportDataToNAV(string navtype, string nextstatus)
        {
            try
            {
                service.Dispose();
                GetNavUrl();
                return service.ImportDataToNAV(navtype, nextstatus);
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public DataTable GetItems()
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSItems();
        }

        public DataTable GetVendors()
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSVendors();
        }
        public DataTable GetItemUOMs()
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSItemUOMs();
        }

        public DataTable GetItemSalesPrices(string salesPersonCode)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSItemPrices(salesPersonCode);
        }

        public DataTable GetCustomers(string salesPersonCode)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSCustomers(salesPersonCode);
        }

        public DataTable GetCustomerPriceHistory(string custno,string itemno)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSCustomerPriceHistory(custno,itemno);
        }

        public DataTable GetCustomerPriceHistorywithCustNos(string strCustNos)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSCustomerPriceHistorywithCustNo(strCustNos);
        }

        public DataTable GetCustLedgerEntry(string custno)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSCustLedgerEntry(custno);
        }

        public DataTable GetPaymentMethods()
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSPaymentMethods();
        }

        public DataTable GetSetupData()
        {
            //GetQHSetupData
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSSetupData();
        }

        public DataTable GetNumSerices(string deviceid,string spcode)
        {
            //GetQHSetupData
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSNumSeries(deviceid,spcode);
        }

        public DataTable GetRequestedHeader(string status)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSRequestedHeaderbyStatus(status);
        }

        public DataTable GetRequestedLines(string hdkey)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSRequestedLines(hdkey);
        }

        public DataTable GetcontainerInfobyDocNo(string docno)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSContainerInfobyDoc(docno);
        }

        public DataTable GetContainerInfobyBoxNo(string boxno)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSContainerInfobyBoxNo(boxno);
        }
        public DataTable GetRequestedPickedLines(string reqno)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSRequestedPickedLines(reqno);
        }
        public DataTable GetcontainerInfobyDocLineNo(string docno, int lineno)
        {
            DataTable dt = new DataTable();
            service.Dispose();
            GetNavUrl();
            return service.GetKFSContainerInfobyDocLine(docno,lineno);
        }

        public string ExportSalesHeader(string docNo, string selltocustomer, string selltoName, string billtoCustomer, string billtoName, string docDate, string status, string paymentMethod, decimal totalAmt,string doctype,string note,string strSingature,string salesPersonCode,string deviceId,string comment,string isvoid,string extdocno)
        {
            service.Dispose();
            GetNavUrl();
            return service.ExportKFSSalesOrder(docNo,selltocustomer,selltoName,billtoCustomer,billtoName,docDate,status,paymentMethod,totalAmt,doctype,note,strSingature,salesPersonCode,deviceId,comment,isvoid,extdocno);
        }

        public string ExportSalesLine(string mEntryNo,string docNo, string itemNo, string locCode, decimal qty, decimal focQty,decimal badQty, string uom, decimal unitPrice, decimal lineDisPercent, decimal lineDisAmt, decimal lineAmt,string reasonCode,string badReasonCode,string itemType)
        {
                service.Dispose();
                GetNavUrl();
                return service.ExportKFSSalesLine(mEntryNo,docNo, itemNo, locCode, qty, focQty,badQty, uom, unitPrice, lineDisPercent, lineDisAmt, lineAmt,reasonCode,badReasonCode,itemType);
        }

        public string ExportPayment(string docNo, string onDate, string customerNo, string paymentMethod, decimal amount, string strSignature, string strImage, string salesPersonCode, string note, string recStatus,string refdocno,string sourcetype)
        {
            service.Dispose();
            GetNavUrl();
            return service.ExportKFSPayment(docNo, onDate, customerNo, paymentMethod, amount, strSignature, strImage, salesPersonCode, note, recStatus,refdocno,sourcetype);
        }
        public string ExportRequestStock(string entryNo, string salesPersonCode, string requestNo, string requestDate,string curstatus)
        {
            service.Dispose();
            GetNavUrl();
            return service.ExportKFSRequestStock(entryNo, salesPersonCode, requestNo, requestDate,curstatus);
        }

        public string ExportRequestLine(string entryNo, string hdEntryNo, string userId, string itemNo, decimal qtyPerBag, decimal noofBags, decimal qty,decimal pickqty,decimal loadqty,decimal unloadqty,
          string uom, string vendorNo, bool inhouse, string requestId)
        {
            service.Dispose();
            GetNavUrl();
            return service.ExportKFSRequestLine(entryNo, hdEntryNo, userId, itemNo, qtyPerBag, noofBags, qty,pickqty,loadqty,unloadqty,uom, vendorNo, inhouse, requestId);
        }

        public string ExportUnloadHeader(string entryNo, string salesPersonCode, string unloadDocNo, string unloadDatetime, string curstatus)
        {
            service.Dispose();
            GetNavUrl();
            return service.ExportKFSUnloadHeader(entryNo, salesPersonCode, unloadDocNo, unloadDatetime, curstatus);
        }

        public string ExportUnloadLine(string entryNo, string hdEntryNo, string userId, string itemNo, string itemDesc,string itemUom, decimal qty, decimal badqty)
        {
            service.Dispose();
            GetNavUrl();
            return service.ExportKFSUnloadLine(entryNo, hdEntryNo, userId, itemNo,itemDesc,itemUom,qty,badqty);
        }
        public string ExportNumSeries(string deviceId, string spcode, string solastno, string crlastno, string cplastno, string rslastno, string ullastno, string soseries, string crseries, string cpseries, string rsseries, string ulseries)
        {
            service.Dispose();
            GetNavUrl();
            return service.ExportKFSNumSeries(deviceId, spcode, solastno, crlastno, cplastno, rslastno, ullastno, soseries, crseries, cpseries, rsseries, ulseries);
        }
        public string UpdateContainerInfo(string boxno, decimal loadQty, decimal soldQty, decimal unloadQty)
        {
            service.Dispose();
            GetNavUrl();
            return service.UpdateContainerInfo(boxno, loadQty,soldQty,unloadQty);
        }

        public string GetGSTPercent()
        {
            service.Dispose();
            GetNavUrl();
            return service.GetKFSGSTPercent();
        }

        public string GetSalesPersonCode(string email)
        {
            service.Dispose();
            GetNavUrl();
            return service.GetSalesPersonCodebyUser(email);
        }

        public string ImportLoadAndUnloadToNAvbyRequestDocument(string navtype,string docno,string nextstatus)
        {
            service.Dispose();
            GetNavUrl();
            return service.ImportLoadAndUnloadToNAVByReqDocument(navtype, docno, nextstatus);
        }

        public string ExportUnloadHistory(string mDeviceID, string mItemNo, decimal mUnloadQty,decimal mSOQty,decimal mCRQty,decimal mBalQty, string mSalesPersonCode)
        {
            service.Dispose();
            GetNavUrl();
            return service.Export_KFSUnloadHistory(mDeviceID,mItemNo,mUnloadQty,mSOQty,mCRQty,mBalQty,mSalesPersonCode);
        }

        public string UnloadHistoryReclasstoNAV(string mSalesPersonCode,string mDeviceID)
        {
            service.Dispose();
            GetNavUrl();
            return service.KFSUnloadHistoryReclasstoNAV( mSalesPersonCode, mDeviceID);
        }
        public string ExportUnloadReturn(string mSalesPersonCode, string mItemNo, decimal mQSRetQty, string mToBin, string mSyncStatus)
        {
            service.Dispose();
            GetNavUrl();
            return service.Update_KFSUnloadedReturn(mSalesPersonCode,mItemNo,mQSRetQty,mToBin,mSyncStatus);
        }

        public string ExportNAVSalesHeader()
        {
            service.Dispose();
            GetNavUrl();
            return service.ExportNAVSalesHeader();
        }

        public string ExportNAVPayment()
        {
            service.Dispose();
            GetNavUrl();
            return service.ExportNAVPayment();
        }
        public DataTable GetUnloadReturnList(string salesPersonCode)
        {
            service.Dispose();
            GetNavUrl();
            return service.GetKFSUnloadReturnList(salesPersonCode);
        }

        public DataTable GetUserList()
        {
            service.Dispose();
            GetNavUrl();
            return service.GetKFSUserList();
        }
        public DataTable GetItemBarCodes()
        {
            service.Dispose();
            GetNavUrl();
            return service.GetKFSItemsBarcodes();
        }

        public DataTable GetResonCodes()
        {
            service.Dispose();
            GetNavUrl();
            return service.GetKFSReasonCode();
        }

        public string UnloadedQSReclasstoNAV(string mSalesPersonCode)
        {
            service.Dispose();
            GetNavUrl();
            return service.UnloadedQSReclasstoNAV(mSalesPersonCode);
        }
        public DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

    }
}