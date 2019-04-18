using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using Xamarin.Forms;
using System.Data;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace QHSalesApp
{
    public  class DataManager
    {
        readonly Database database;
        public DataManager()
        {
            database = new Database(Constants.DatabaseName);           
        }
        public ObservableCollection<T> ConvertObservable<T>(IEnumerable<T> original)
        {
            return new ObservableCollection<T>(original);
        }

        #region Number Series
        public string SaveSQLite_NumberSeries(ObservableCollection<NumberSeries> numSeries)
        {
            try
            {
                database.DeleteAll<NumberSeries>();

                foreach (NumberSeries rec in numSeries)
                {
                    database.SaveData<NumberSeries>(rec);
                }
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public string GetLastNoSeries(string code)
        {
            try
            {
                string strLastNumCode = string.Empty;
                string strQuery = "SELECT * FROM NumberSeries WHERE Code=?";
                string[] parameters = new string[] { code };
                NumberSeries series = new NumberSeries();
                series = database.Query<NumberSeries>(strQuery, parameters).FirstOrDefault();
                if (series != null)
                {
                    //int lastNumSeries = series.LastNoSeries + 1;
                     //series.Code + "-" + lastNumSeries.ToString();
                    //if(series.LastNoSeries>0)
                    //    strLastNumCode = series.LastNoCode;
                    //else
                        strLastNumCode = Utils.TryToIncrement(series.LastNoCode);
                    

                }
                return strLastNumCode;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public string IncreaseNoSeries(string code, string strLastNumCode, string desc)
        {
            try
            {
                int lastNumSeries;
                string strQuery = "SELECT * FROM NumberSeries WHERE Code=?";
                string[] parameters = new string[] { code };
                NumberSeries series = new NumberSeries();
                series = database.Query<NumberSeries>(strQuery, parameters).FirstOrDefault();
                if (series != null)
                {
                    lastNumSeries = series.LastNoSeries + 1;
                    //if (series.LastNoCode == strLastNumCode)
                    //{
                    //    lastNumSeries = series.LastNoSeries;
                    //   // strLastNumCode = series.Code +lastNumSeries.ToString();
                    //}
                    //else
                    //{
                    //    lastNumSeries = series.LastNoSeries + 1;
                    //    strLastNumCode = Utils.TryToIncrement(strLastNumCode);
                    //}

                    NumberSeries newSeries = new NumberSeries() { ID = series.ID, Description = desc, Code = series.Code, LastNoCode = strLastNumCode, LastNoSeries = lastNumSeries, Increment = 1 };
                    database.SaveData<NumberSeries>(newSeries);

                }
                return strLastNumCode;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        #endregion

        #region Reset Methods
        public void CreateTables()
        {
            database.CreateTable<RequestHeader>();
            database.CreateTable<RequestLine>();
            database.CreateTable<UnloadHeader>();
            database.CreateTable<UnloadLine>();
            database.CreateTable<ContainerInfo>();
            database.CreateTable<ScannedLoadDoc>();
            database.CreateTable<ScannedSoldDoc>();
            database.CreateTable<ScannedUnloadDoc>();
            database.CreateTable<ScannedUnloadReturnDoc>();
            database.CreateTable<SalesHeader>();
            database.CreateTable<SalesLine>();
            database.CreateTable<Payment>();
            database.CreateTable<Item>();
            database.CreateTable<ChangedItem>();
            database.CreateTable<ReasonCode>();
            database.CreateTable<VanItem>();
            database.CreateTable<SalesPrice>();
            database.CreateTable<Customer>();
            database.CreateTable<Vendor>();
            database.CreateTable<CustomerPriceHistory>();
            database.CreateTable<PaymentMethod>();
            database.CreateTable<CustLedgerEntry>();
            database.CreateTable<PaidReference>();
            database.CreateTable<Setup>();
            database.CreateTable<UnloadReturn>();
            database.CreateTable<DeviceInfo>();
        }
        public void ResetTransData()
        {
            database.DeleteAll<NumberSeries>();
            database.DeleteAll<RequestHeader>();
            database.DeleteAll<RequestLine>();
            database.DeleteAll<UnloadHeader>();
            database.DeleteAll<UnloadLine>();
            database.DeleteAll<ContainerInfo>();
            database.DeleteAll<UnloadReturn>();
            database.DeleteAll<ScannedLoadDoc>();
            database.DeleteAll<ScannedSoldDoc>();
            database.DeleteAll<ScannedUnloadDoc>();
            database.DeleteAll<ScannedUnloadReturnDoc>();
            database.DeleteAll<SalesHeader>();
            database.DeleteAll<SalesLine>();
            database.DeleteAll<Payment>();
            database.DeleteAll<PaidReference>();
        }
        public void resetMasterData()
        {
            database.DeleteAll<Item>();
            database.DeleteAll<ChangedItem>();
            database.DeleteAll<VanItem>();
            database.DeleteAll<ReasonCode>();
            database.DeleteAll<SalesPrice>();
            database.DeleteAll<Customer>();
            database.DeleteAll<CustomerPriceHistory>();
            database.DeleteAll<PaymentMethod>();
            database.DeleteAll<CustLedgerEntry>();
            
           
            //  database.DeleteAll<PaymentHistory>();
            //database.DeleteAll<CustomerPriceHistory>();
        }
        public string ResetSqlite_Invenotry(string itemno)
        {
            try
            {

                Item item = new Item();
                item = GetSQLite_ItembyItemNo(itemno);
                var record = new Item
                {
                    ID = item.ID,
                    EntryNo = item.EntryNo,
                    ItemNo = item.ItemNo,
                    Description = item.Description,
                    Description2 = item.Description2,
                    Str64Img = item.Str64Img,
                    BarCode = item.BarCode,
                    BaseUOM = item.BaseUOM,
                    UnitPrice = item.UnitPrice,
                    CategoryCode = item.CategoryCode,
                    InvQty = item.InvQty,
                    LoadQty = 0,
                    SoldQty = 0,
                    ReturnQty = 0,
                    BadQty = 0,
                    UnloadQty = 0,
                    IsActive=item.IsActive
                };
                database.SaveData<Item>(record);
                //database.SaveDataAll(items);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        #endregion

        #region Save Methods
        public string SaveSQLite_Users(string mobileAccessKey)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetLoginUsers(mobileAccessKey);

                if (dt.Rows.Count > 0)
                {
                    database.DeleteAll<User>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var record = new User
                        {
                            EntryNo = int.Parse(dr["EntryNo"].ToString()),
                            Default_CustEntryNo = int.Parse(dr["Default_CustEntryNo"].ToString()),
                            Email = dr["Email"].ToString().ToLower(),
                            UserID = dr["ID"].ToString(),
                            Role = dr["Role"].ToString(),
                            Name = dr["Name"].ToString(),
                            pwd = dr["pwd"].ToString(),
                            Status = dr["Status"].ToString().ToLower() == "true" ? true : false,
                            Outlet_Loc = dr["Default_Loc"].ToString(),
                            WH_Loc = dr["Main_Loc"].ToString(),
                            SalesPersonCode = dr["SalesPersonCode"].ToString(),
                            DeviceID=dr["DeviceID"].ToString()
                        };

                        database.SaveData(record);
                    }
                    return "Success";
                }
                else
                    return "No Users!";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public string SaveSQLite_Setup(string deviceid)
        {
            try
            {
                string valGST = App.svcManager.RetGSTPercent();
                DataTable dt = App.svcManager.RetSetup();
                if (dt.Rows.Count > 0)
                {

                    database.DeleteAll<Setup>();
                    var record = new Setup
                    {
                        GSTPercent = dt.Rows[0]["GSTPercent"].ToString(),
                        GSTRegNo = dt.Rows[0]["GSTRegNo"].ToString(),
                        SOPrefix = dt.Rows[0]["SOPrefix"].ToString(),
                        CRPrefix = dt.Rows[0]["CRPrefix"].ToString(),
                        CPPrefix = dt.Rows[0]["CPPrefix"].ToString(),
                        RSPrefix = dt.Rows[0]["RSPrefix"].ToString(),
                        ULPrefix = dt.Rows[0]["ULPrefix"].ToString(),
                        StartNum = dt.Rows[0]["StartNum"].ToString(),
                        Increment = dt.Rows[0]["Increment"].ToString(),
                        AdminPsw = dt.Rows[0]["AdminPsw"].ToString(),
                        DeviceId = deviceid
                    };

                    database.SaveData(record);
                    return "Success";
                }
                else
                    return "No Setup value!";

            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_Items()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetItems();

                if (dt.Rows.Count > 0)
                {
                    //database.DeleteAll<Item>();
                    //IEnumerable items = dt.Select().AsEnumerable();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Item itm = new Item();
                        itm=GetSQLite_ItembyItemNo(dr["No"].ToString());
                        string strimg = string.Empty;
                        if (string.IsNullOrEmpty(dr["strImage"].ToString()))
                            strimg = Constants.BlankImgStr;
                        else
                            strimg = dr["strImage"].ToString();

                        if (itm!=null)
                        {
                            var record = new Item
                            {
                                ID=itm.ID,
                                EntryNo = itm.EntryNo,
                                ItemNo = dr["No"].ToString(),
                                Description = dr["Description"].ToString(),
                                Description2 = dr["Description2"].ToString(),
                                BaseUOM = dr["BaseUOM"].ToString(),
                                UnitPrice = dr["Unit_Price"].ToString(),
                                CategoryCode = dr["CategoryCode"].ToString(),
                                Str64Img = strimg,
                                InvQty = decimal.Parse(dr["InvQty"].ToString()),
                                LoadQty = itm.LoadQty,
                                SoldQty = itm.SoldQty,
                                ReturnQty = itm.ReturnQty,
                                BadQty = itm.BadQty,
                                UnloadQty = itm.UnloadQty,
                                BarCode = dr["Barcode"].ToString(),
                                IsActive= dr["IsActive"].ToString()
                            };
                            database.SaveData<Item>(record);
                        }
                        else
                        {
                            var record = new Item
                            {
                                EntryNo = int.Parse(dr["EntryNo"].ToString()),
                                ItemNo = dr["No"].ToString(),
                                Description = dr["Description"].ToString(),
                                Description2 = dr["Description2"].ToString(),
                                BaseUOM = dr["BaseUOM"].ToString(),
                                UnitPrice = dr["Unit_Price"].ToString(),
                                CategoryCode = dr["CategoryCode"].ToString(),
                                Str64Img = strimg,
                                InvQty = decimal.Parse(dr["InvQty"].ToString()),
                                LoadQty = 0,
                                SoldQty = 0,    
                                ReturnQty = 0,
                                BadQty = 0,
                                UnloadQty = 0,
                                BarCode = dr["Barcode"].ToString(),
                                IsActive = dr["IsActive"].ToString()
                            };
                            database.SaveData<Item>(record);
                        }
                    }
                    //database.SaveDataAll(items);
                    App.gItems= await GetSQLite_Items();
                    return "Success";
                }
                else
                {
                    App.gItems = null;
                    return "No Items!";
                }
                    
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_BarCodeInfo()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetItemBarcode();

                if (dt.Rows.Count > 0)
                {
                    database.DeleteAll<BarCodeInfo>();
                    //IEnumerable items = dt.Select().AsEnumerable();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var record = new BarCodeInfo
                        {
                            BarCode=dr["Barcode"].ToString(),
                            ItemNo = dr["ItemNo"].ToString(),
                            Description = dr["Description"].ToString(),
                        };
                        database.SaveData<BarCodeInfo>(record);
                    }
                    //database.SaveDataAll(items);
                    return "Success";
                }
                else
                    return "No Items!";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_ReasonCodes()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetReasonCodes();

                if (dt.Rows.Count > 0)
                {
                    database.DeleteAll<ReasonCode>();
                    //IEnumerable items = dt.Select().AsEnumerable();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var record = new ReasonCode
                        {
                            Code = dr["Code"].ToString(),
                            Description = dr["Description"].ToString(),
                        };
                        database.SaveData<ReasonCode>(record);
                    }
                    //database.SaveDataAll(items);
                    return "Success";
                }
                else
                    return "No Items!";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_ItemUOms()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetItemUOMs();

                if (dt.Rows.Count > 0)
                {
                    database.DeleteAll<ItemUOM>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var record = new ItemUOM
                        {
                            EntryNo = int.Parse(dr["EntryNo"].ToString()),
                            ItemNo = dr["ItemNo"].ToString(),
                            UOMCode = dr["UOMCode"].ToString(),
                        };
                        database.SaveData(record);
                    }
                    return "Success Save rows : " + dt.Rows.Count;
                }
                else
                    return "No Item UOMs!";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_SalesPrices(string salesPersonCode)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetItemSalesPrices(salesPersonCode);

                if (dt.Rows.Count > 0)
                {
                    database.DeleteAll<SalesPrice>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var record = new SalesPrice
                        {
                            EntryNo = int.Parse(dr["EntryNo"].ToString()),
                            SalesType = dr["SalesType"].ToString(),
                            ItemNo = dr["ItemNo"].ToString(),
                            SalesCode = dr["SalesCode"].ToString(),
                            MinimumQty = decimal.Parse(dr["MinimumQty"].ToString()),
                            UnitPrice = decimal.Parse(dr["UnitPrice"].ToString()),
                            StartDate = Convert.ToDateTime(dr["StartDate"].ToString()),
                            EndDate = Convert.ToDateTime(dr["EndDate"].ToString()),
                            UOM = dr["UOM"].ToString(),
                            CustomerNo = dr["CustomerNo"].ToString(),
                            PromotionType = dr["PromotionType"].ToString()
                        };
                        database.SaveData< SalesPrice>(record);
                    }
                    return "Success";
                }
                else
                    return "No Sales Prices!";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_Customers(string salesPersonCode)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetCustomers(salesPersonCode);

                if (dt.Rows.Count > 0)
                {
                    database.DeleteAll<Customer>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var record = new Customer
                        {
                            EntryNo = int.Parse(dr["EntryNo"].ToString()),
                            CustomerNo= dr["CustomerNo"].ToString(),
                            Country= dr["Country"].ToString(),
                            SalesPersonCode= dr["SalesPersonCode"].ToString(),
                            Name= dr["Name"].ToString(),
                            Name2= dr["Name2"].ToString(),
                            SearchName= dr["SearchName"].ToString(),
                            Contact= dr["Contact"].ToString(),
                            Address= dr["Address"].ToString(),
                            Address2= dr["Address2"].ToString(),
                            City= dr["City"].ToString(),
                            Postcode= dr["Postcode"].ToString(),
                            CountryCode= dr["CountryCode"].ToString(),
                            PhoneNo= dr["PhoneNo"].ToString(),
                            MobileNo= dr["MobileNo"].ToString(),
                            TelexNo= dr["TelexNo"].ToString(),
                            FaxNo= dr["FaxNo"].ToString(),
                            Email= dr["Email"].ToString(),
                            Website= dr["Website"].ToString(),
                            CreditLimit= dr["CreditLimit"].ToString(),
                            InvoiceLimit= dr["InvoiceLimit"].ToString(),
                            Outstanding= dr["Outstanding"].ToString(),
                            CurrencyCode= dr["CurrencyCode"].ToString(),
                            PaymentTerms= dr["PaymentTerms"].ToString(),
                            CustomerPriceGroup= dr["CustomerPriceGroup"].ToString(),
                            CustomerDiscGroup= dr["CustomerDiscGroup"].ToString(),
                            PaymentTermsDesc=dr["PaymentTermsDesc"].ToString(),
                            billtoCustNo=dr["billtoCustNo"].ToString()
                        };
                        database.SaveData<Customer>(record);
                    }
                    App.gCustomers=await GetSQLite_Customers();
                    return "Success";
                }
                else
                {
                    App.gCustomers = null;
                    return "No customers!";
                }
                    
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_Vendors()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetVendors();

                if (dt.Rows.Count > 0)
                {
                    database.DeleteAll<Vendor>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var record = new Vendor
                        {
                            EntryNo =int.Parse(dr["EntryNo"].ToString()),
                            VendorNo = dr["VendorNo"].ToString(),
                            VendorName=dr["Name"].ToString()
                        };
                        database.SaveData<Vendor>(record);
                    }
                    return "Success";
                }
                else
                    return "No customers!";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_CustLederEntry(string custno)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetCustomerLedgerEntry(custno);

                if (dt.Rows.Count > 0)
                {
                    database.DeleteAll<CustLedgerEntry>();
                    { }
                    foreach (DataRow dr in dt.Rows)
                    {
                        var record = new CustLedgerEntry
                        {
                            //  EntryNo = int.Parse(dr["EntryNo"].ToString()),
                            TransType = dr["TransType"].ToString(),
                            DocNo = dr["DocNo"].ToString(),
                            CustNo=dr["CustNo"].ToString(),
                            ExtDocNo=dr["ExtDocNo"].ToString(),
                            IsOpenItem= dr["IsOpenItem"].ToString(),
                            TransDate= dr["TransDate"].ToString(),
                            PaymentTerm= dr["PaymentTerm"].ToString(),
                            InvoiceAmount=dr["InvoiceAmount"].ToString(),
                            PaidAmount= dr["PaidAmount"].ToString(),
                            UnpaidAmount= dr["UnpaidAmount"].ToString()
                        };
                        database.SaveData<CustLedgerEntry>(record);
                    }
                    return "Success";
                }
                else
                    return "No customers Ledger Entry!";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_CustomerPriceHistory(string strCustNo)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetCustomerPriceHistorywithCustNos(strCustNo);

                if (dt.Rows.Count > 0)
                {
                    database.DeleteAll<CustomerPriceHistory>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var record = new CustomerPriceHistory
                        {
                            CustNo = dr["CustNo"].ToString(),
                            ItemNo = dr["ItemNo"].ToString(),
                            UOM = dr["UOM"].ToString(),
                            Currency = dr["Currency"].ToString(),
                            UnitPrice = dr["UnitPrice"].ToString(),
                            Qty = dr["Qty"].ToString(),
                            TransDate = dr["TransDate"].ToString(),
                            UnitPrice2 = dr["unitPrice2"].ToString(),
                            Qty2 = dr["Qty2"].ToString(),
                            TransDate2 = dr["TransDate2"].ToString(),
                            unitPrice3 = dr["unitPrice3"].ToString(),
                            Qty3 = dr["Qty3"].ToString(),
                            TransDate3 = dr["TransDate3"].ToString()
                        };
                        database.SaveData<CustomerPriceHistory>(record);
                    }
                    return "Success";
                }
                else
                    return "No Customer Price History!";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_PaymentMethods()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetPaymentMethods();

                if (dt.Rows.Count > 0)
                {
                    database.DeleteAll<PaymentMethod>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var record = new PaymentMethod
                        {
                           
                            Code = dr["Code"].ToString(),
                            Description = dr["Description"].ToString(),
                        };
                        database.SaveData<PaymentMethod>(record);
                    }
                    return "Success";
                }
                else
                    return "No Payment Methods!";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_UnloadReturn(string salesPersonCode)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.GetUnloadReturnbySalesPerson(salesPersonCode);
                ObservableCollection<UnloadReturn> records = new ObservableCollection<UnloadReturn>();
                if (dt.Rows.Count > 0)
                {
                    database.CreateTable<UnloadReturn>();
                    database.DeleteAll<UnloadReturn>();
                    //database.DeleteAll<ScannedUnloadReturnDoc>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        UnloadReturn checkrec = new UnloadReturn();
                        int id = 0;
                        decimal qsRetQty = await GetSQLite_SumQSReturnQty(dr["ItemNo"].ToString());
                        ScannedUnloadReturnDoc doc = new ScannedUnloadReturnDoc();
                        doc = await GetSQLite_ScannedUnloadReturnDoc(dr["ItemNo"].ToString());
                        string tobin = dr["ToBin"].ToString();
                        if (doc != null)
                            tobin = doc.ToBin; //dr["ToBin"].ToString();

                        checkrec = await GetSQLite_UnloadReturnbyItemNo(dr["ItemNo"].ToString());
                        if (checkrec != null)
                        {
                            id = checkrec.ID;
                            qsRetQty = checkrec.QSReturnQty;
                            tobin = checkrec.ToBin;
                        }

                        var record = new UnloadReturn
                        {
                            ID = id,
                            EntryNo = int.Parse(dr["EntryNo"].ToString()),
                            ItemNo = dr["ItemNo"].ToString(),
                            ItemDesc = dr["ItemDesc"].ToString(),
                            Quantity = decimal.Parse(dr["Quantity"].ToString()),
                            QSReturnQty = qsRetQty,
                            FromBin = dr["FromBin"].ToString(),
                            ToBin = tobin
                        };
                        database.SaveData<UnloadReturn>(record);

                    }
                    return "Success";
                }
                else
                    return "No records!";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_PopulateContainerInfo(string requestNo)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetContainerInfobyDocNo(requestNo);
                //dt = App.svcManager.RetRequestedLines(hdkey);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        int recId = 0;
                        decimal recLoadQty = 0;
                        ContainerInfo info = new ContainerInfo();
                        info = await GetContainerInfowithRefDocBagID(dr["BoxNo"].ToString(), dr["RefDocNo"].ToString());
                        //info = await GetSQLite_ContainerInfobyEntryNo(dr["EntryNo"].ToString());
                        if (info != null)
                        {
                            recId = info.ID;
                            recLoadQty = info.LoadQty;
                        }
                        else
                        {
                            recLoadQty = decimal.Parse(dr["LoadQty"].ToString());
                        }

                        var record = new ContainerInfo
                        {
                            ID = recId,
                            EntryNo = dr["EntryNo"].ToString(),
                            PalletNo = dr["PalletNo"].ToString(),
                            CartonNo = dr["CartonNo"].ToString(),
                            BoxNo = dr["BoxNo"].ToString(),
                            LineNo = int.Parse(dr["LineNo"].ToString()),
                            ItemNo = dr["ItemNo"].ToString(),
                            VariantCode = dr["VariantCode"].ToString(),
                            Quantity = decimal.Parse(dr["Quantity"].ToString()),
                            LoadQty = recLoadQty,
                            SoldQty = decimal.Parse(dr["SoldQty"].ToString()),
                            UnloadQty = decimal.Parse(dr["UnloadQty"].ToString()),
                            LocationCode = dr["LocationCode"].ToString(),
                            BinCode = dr["BinCode"].ToString(),
                            RefDocNo = dr["RefDocNo"].ToString(),
                            RefDocLineNo = int.Parse(dr["RefDocLineNo"].ToString()),
                            RefDocType = dr["RefDocType"].ToString(),
                            MobileEntryNo = dr["MobileEntryNo"].ToString(),
                        };
                        database.SaveData<ContainerInfo>(record);
                    }
                    return "Success";
                }
                else
                    return "No Data!";
            }
            catch (Exception ex)
            {

                return "PopulateContainerInfo Error :" + ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_PopulatedPick(string status)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetRequestHeaderbyStatus(status);
                //dt = App.svcManager.RetRequestedLines(hdkey);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        RequestHeader rhd = new RequestHeader();
                        rhd = await GetSQLite_RequestHeadebyRequestNo(dr["RequestNo"].ToString());
                        if (rhd != null)
                        {
                            if (rhd.CurStatus == "topick" || rhd.CurStatus == "picking")
                            {
                                var record = new RequestHeader
                                {
                                    ID = rhd.ID,
                                    EntryNo = rhd.EntryNo,
                                    SalesPersonCode = rhd.SalesPersonCode,
                                    RequestNo = rhd.RequestNo,
                                    RequestDate = rhd.RequestDate,
                                    IsSync = rhd.IsSync,
                                    SyncDateTime = rhd.SyncDateTime,
                                    CurStatus = status
                                };
                                database.SaveData<RequestHeader>(record);

                                SaveSQLite_PopulatedPickedLine(rhd.RequestNo);

                                // SaveSQLite_PopulateContainerInfo(rhd.RequestNo);
                            }
                        }
                    }
                    return "Success";
                }
                else
                    return "No requested headers!";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }      
        private async Task<string> SaveSQLite_PopulatedPickedLine(string reqno)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = App.svcManager.RetRequestedPickedLines(reqno);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        RequestLine rln = new RequestLine();
                        rln = await GetSQLite_RequestLinebyEntryNo(dr["EntryNo"].ToString());
                        var record = new RequestLine
                        {
                            ID = rln.ID,
                            RLineNo = dr["LineNo"].ToString(),
                            EntryNo = rln.EntryNo,
                            HeaderEntryNo = rln.HeaderEntryNo,
                            ItemNo = rln.ItemNo,
                            ItemDesc = rln.ItemDesc,
                            QtyperBag = rln.QtyperBag,
                            NoofBags = rln.NoofBags,
                            Quantity = rln.Quantity,
                            PickQty = decimal.Parse(dr["PickQty"].ToString()),
                            LoadQty = rln.LoadQty,
                            SoldQty = rln.SoldQty,
                            UnloadQty = rln.UnloadQty,
                            UomCode = rln.UomCode,
                            VendorNo = rln.VendorNo,
                            UserID = rln.UserID,
                            InHouse = rln.InHouse,
                            RequestNo = rln.RequestNo,
                            IsSync = "picking",
                            SyncDateTime = rln.SyncDateTime
                        };
                        database.SaveData<RequestLine>(record);
                    }
                    return "Success";
                }
                else
                    return "No requested lines!";
            }
            catch (Exception ex)
            {

                return "PopulateRequestLine Error :" + ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_ScannedLoadDoc(string bagno, string requestno, string reqlineno, decimal qty, string itemno)
        {
            try
            {

                var record = new ScannedLoadDoc
                {
                    BagNo = bagno,
                    RequestDocNo = requestno,
                    RequestLineNo = reqlineno,
                    LoadQty = qty,
                    ItemNo = itemno
                };
                database.SaveData<ScannedLoadDoc>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_ScannedSoldDoc(int id, string bagno, string requestno, string reqlineno, decimal qty)
        {
            try
            {

                var record = new ScannedSoldDoc
                {
                    ID = id,
                    BagNo = bagno,
                    RequestDocNo = requestno,
                    RequestLineNo = reqlineno,
                    SoldQty = qty
                };
                database.SaveData<ScannedSoldDoc>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_ScannedUnloadDoc(string bagno, string requestno, string reqlineno, decimal qty)
        {
            try
            {

                var record = new ScannedUnloadDoc
                {
                    BagNo = bagno,
                    RequestDocNo = requestno,
                    RequestLineNo = reqlineno,
                    UnloadQty = qty
                };
                database.SaveData<ScannedUnloadDoc>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_ScannedUnloadReturnDoc(string bagno, string requestno, string reqlineno, decimal qty, string itemno, string tobin)
        {
            try
            {
                var record = new ScannedUnloadReturnDoc
                {
                    BagNo = bagno,
                    RequestDocNo = requestno,
                    RequestLineNo = reqlineno,
                    QSReturnQty = qty,
                    ItemNo = itemno,
                    ToBin = tobin
                };
                database.SaveData<ScannedUnloadReturnDoc>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_SalesHeader(SalesHeader record)
        {
            try
            {
                database.SaveData<SalesHeader>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_RequestHeader(RequestHeader record)
        {
            try
            {
                database.SaveData<RequestHeader>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_UnloadHeader(UnloadHeader record)
        {
            try
            {
                database.SaveData<UnloadHeader>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_UnloadLine(UnloadLine record)
        {
            try
            {
                database.SaveData<UnloadLine>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<Dictionary<int, string>> SaveSQLite_Payment(Payment record)
        {
            Dictionary<int, string> dicResult = new Dictionary<int, string>();
            try
            {

                int retval = database.SaveData<Payment>(record);
                dicResult.Add(retval, "Success");
                return dicResult;
            }
            catch (Exception ex)
            {
                dicResult.Add(-1, ex.Message.ToString());
                return dicResult;
            }
        }
        public async Task<string> SaveSQLite_PaidReference(PaidReference record)
        {
            try
            {
                database.SaveData<PaidReference>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public string SaveSQLite_SalesLine(SalesLine record)
        {
            try
            {
                database.SaveData<SalesLine>(record);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_RequestLine(RequestLine record)
        {
            try
            {
                database.SaveData<RequestLine>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_DeviceInfo(DeviceInfo record)
        {
            try
            {
                database.DeleteAll<DeviceInfo>();
                database.SaveData<DeviceInfo>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_ItemInfo(Item record)
        {
            try
            {
                database.SaveData<Item>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_ChangedItem(ChangedItem record)
        {
            try
            {
                database.SaveData<ChangedItem>(record);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public async Task<string> SaveSQLite_VanItem(VanItem record)
        {
            try
            {
                database.SaveData<VanItem>(record);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        #endregion

        #region Update Methods
        public async Task<string> UpdateSQLite_ContainerInfo(ContainerInfo record)
        {
            try
            {               
                database.SaveData<ContainerInfo>(record);
                
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public async Task<string> UpdateSQLite_UnloadReturn(string itemno, decimal qsretQty, string tobin)
        {
            try
            {
                string[] parameters = new string[] { qsretQty.ToString(), tobin, itemno };
                database.Query<UnloadReturn>("UPDATE UnloadReturn SET QSReturnQty=?,ToBin=? WHERE ItemNo=?", parameters);
                //var record = new UnloadReturn
                //            {
                //                EntryNo = entryNo,
                //                ItemNo = itemno,
                //                ItemDesc = itemdesc,
                //                Quantity =qty,
                //                QSReturnQty = qsretQty,
                //                FromBin =frombin,
                //                ToBin = tobin
                //            };
                // database.SaveData<UnloadReturn>(record);
                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public async Task<string> UpdateSQLite_VanItem(string itemno, decimal qty)
        {
            string[] parameters = new string[] { qty.ToString(), itemno };
            database.Query<VanItem>("UPDATE VanItem SET  LoadQty=? WHERE ItemNo=?", parameters);
            return "Success";
        }

        public async Task<string> DeleteSQLite_VanItem(string itemno)
        {
            string[] parameters = new string[] {itemno };
            database.Query<VanItem>("DELETE FROM VanItem WHERE ItemNo=?", parameters);
            return "Success";
        }
        public string UpdateSalesHeaderTotalAmount(decimal totalAmt, string docno, ObservableCollection<SalesLine> lines)
        {
            decimal gstamt = 0;
            if (lines != null)
            {
                if (lines.Count > 0)
                {
                    foreach (SalesLine s in lines)
                    {
                        gstamt += (s.LineAmount * decimal.Parse(App.gPercentGST)) / 100;
                    }
                }
            }
            decimal netamt = totalAmt + gstamt;
            string[] parameters = new string[] { totalAmt.ToString(), string.Format("{0:0.00}", gstamt), string.Format("{0:0.00}", netamt), docno };
            database.Query<SalesHeader>("UPDATE SalesHeader SET TotalAmount=?,GSTAmount=?,NetAmount=? WHERE DocumentNo=? ", parameters);
            return "Success";
        }
        public string UpdateSQLite_SOInventory(string itemno, decimal qty)
        {
            string[] parameters = new string[] { qty.ToString(), itemno };
            database.Query<Item>("UPDATE Item SET  SoldQty=? WHERE ItemNo=?", parameters);
            return "Success";
        }
        public async Task<string> UpdateSQLite_LoadInventory(string itemno, decimal qty)
        {
            string[] parameters = new string[] { qty.ToString(), itemno };
            database.Query<Item>("UPDATE Item SET  LoadQty=? WHERE ItemNo=?", parameters);
            return "Success";
        }
        public string UpdateSQLite_ReturnInventory(string itemno, decimal qty, decimal badqty)
        {
            string[] parameters = new string[] { qty.ToString(), badqty.ToString(), itemno };
            database.Query<Item>("UPDATE Item SET  ReturnQty=?,BadQty=? WHERE ItemNo=?", parameters);
            return "Success";
        }
        public string UpdateSQLite_ExchangeInventory(string itemno,decimal badqty)
        {
            string[] parameters = new string[] { badqty.ToString(), itemno };
            database.Query<Item>("UPDATE Item SET  BadQty=? WHERE ItemNo=?", parameters);
            return "Success";
        }
        public async Task<string> UpdateSQLite_UnloadInventory(string itemno, decimal qty)
        {
            string[] parameters = new string[] { qty.ToString(), itemno };
            database.Query<Item>("UPDATE Item SET  UnloadQty=? WHERE ItemNo=?", parameters);
            return "Success";
        }

        public string UpdateSQLite_Inventory(string itemno, decimal loadQty, decimal soldQty, decimal returnQty, decimal badQty, decimal unloadQty)
        {
            try
            {

                Item item = new Item();
                item = GetSQLite_ItembyItemNo(itemno);

                decimal newLoadQty = loadQty; // - prevloadQty
                decimal newSoldQty = item.SoldQty + soldQty; //- prevsoldQty
                decimal newReturnQty = item.ReturnQty + returnQty; //- prevReturnQty
                decimal newUnloadQty = item.UnloadQty + unloadQty;
                decimal newBadQty = item.BadQty + badQty;
                var record = new Item
                {
                    ID = item.ID,
                    EntryNo = item.EntryNo,
                    ItemNo = item.ItemNo,
                    Str64Img = item.Str64Img,
                    Description = item.Description,
                    Description2 = item.Description2,
                    BaseUOM = item.BaseUOM,
                    UnitPrice = item.UnitPrice,
                    CategoryCode = item.CategoryCode,
                    BarCode = item.BarCode,
                    InvQty = item.InvQty,
                    LoadQty = newLoadQty,
                    SoldQty = newSoldQty,
                    ReturnQty = newReturnQty,
                    BadQty = newBadQty,
                    UnloadQty = newUnloadQty
                };
                database.SaveData<Item>(record);
                //database.SaveDataAll(items);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        #endregion

        #region Delete Methods
        public void DeleteItem()
        {
            database.DeleteAll<Item>();
        }
        public void DeleteSalesOrder()
        {
            database.DeleteAll<SalesHeader>();
        }
        public void DeletePayment()
        {
            database.DeleteAll<Payment>();
        }
        public void DeleteSalesOrderbyID(int id)
        {
            database.DeleteData<SalesHeader>(id);
        }
        public void DeleteRequestHeaderbyID(int id)
        {
            database.DeleteData<RequestHeader>(id);
        }
        public async Task<string> DeleteSQLite_UnloadReturnAndScannedDoc()
        {
            try
            {
                database.DeleteAll<UnloadReturn>();
                database.DeleteAll<ScannedUnloadReturnDoc>();
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public async Task<string> ClearSQLite_ScannedUnloadReturn(string itemno)
        {
            try
            {
                //decimal scannedqty = await GetSQLite_SumQSReturnQty(itemno);
                string[] parameters = new string[] { itemno };
                database.Query<ScannedUnloadReturnDoc>("DELETE FROM ScannedUnloadReturnDoc WHERE ItemNo=?", parameters);
                //database.Query<UnloadReturn>("UPDATE UnloadReturn SET QSReturnQty=0 WHERE ItemNo=?", parameters);

                return "Success";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public string DeleteScannedSoldDocbyBagNo(string bagno)
        {
            string[] parameters = new string[] { bagno };
            database.Query<ScannedSoldDoc>("DELETE FROM ScannedSoldDoc WHERE BagNo=?", parameters);
            return "Success";
        }
        public string DeletePaidReferencebyDocNo(string docno)
        {
            string[] parameters = new string[] { docno };
            database.Query<PaidReference>("DELETE FROM PaidReference WHERE DocumentNo=?", parameters);
            return "Success";
        }
        public string DeleteUnload()
        {
            database.DeleteAll<ScannedUnloadDoc>();
            return "Success";
        }
        public string DeleteAllScanDocTables()
        {
            database.DeleteAll<ContainerInfo>();
            database.DeleteAll<ScannedLoadDoc>();
            database.DeleteAll<ScannedSoldDoc>();
            database.DeleteAll<ScannedUnloadDoc>();
            return "Success";
        }
        public async Task<string> DeleteSQLite_VanItem()
        {
            try
            {
                database.DeleteAll<VanItem>();
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string DeleteSalesLinebyID(int id)
        {
            string[] parameters = new string[] { id.ToString() };
            database.Query<SalesLine>("DELETE FROM SalesLine WHERE ID=?", parameters);
            return "Success";
        }
        public string DeleteSalesLinebyDocNo(string docno)
        {
            string[] parameters = new string[] { docno };
            database.Query<SalesLine>("DELETE FROM SalesLine WHERE DocumentNo=?", parameters);
            return "Success";
        }
        public string DeleteSingleSalesLine(string docno, string itemno)
        {
            string[] parameters = new string[] { docno, itemno };
            database.Query<SalesLine>("DELETE FROM SalesLine WHERE DocumentNo=? AND ItemNo=?", parameters);
            return "Success";
        }
        public string DeleteSingleRequestLine(string entryNo)
        {
            string[] parameters = new string[] { entryNo };
            database.Query<RequestLine>("DELETE FROM RequestLine WHERE EntryNo=?", parameters);
            return "Success";
        }
        public void DeleteContainerInfo()
        {
            database.DeleteAll<ContainerInfo>();
        }
        public string DeleteChangedItem()
        {
            database.DeleteAll<ChangedItem>();
            return "Success";
        }
        #endregion

        #region oldcode
        //public async Task<string> SaveSQLite_UnloadReturn(string salesPersonCode)
        //{
        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        dt = App.svcManager.GetUnloadReturnbySalesPerson(salesPersonCode);
        //        ObservableCollection<UnloadReturn> records = new ObservableCollection<UnloadReturn>();
        //        if (dt.Rows.Count > 0)
        //        {
        //            database.CreateTable<UnloadReturn>();
        //            //database.DeleteAll<UnloadReturn>();
        //            //database.DeleteAll<ScannedUnloadReturnDoc>();
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                UnloadReturn checkrec = new UnloadReturn();
        //                int id = 0;
        //                decimal qsRetQty = decimal.Parse(dr["QSReturnQty"].ToString());
        //                string tobin = dr["ToBin"].ToString();
        //                checkrec = await GetSQLite_UnloadReturnbyItemNo(dr["ItemNo"].ToString());
        //                if (checkrec != null)
        //                {
        //                    id = checkrec.ID;
        //                    qsRetQty = checkrec.QSReturnQty;
        //                    tobin = checkrec.ToBin;
        //                }

        //                var record = new UnloadReturn
        //                {
        //                    ID = id,
        //                    EntryNo = int.Parse(dr["EntryNo"].ToString()),
        //                    ItemNo = dr["ItemNo"].ToString(),
        //                    ItemDesc = dr["ItemDesc"].ToString(),
        //                    Quantity = decimal.Parse(dr["Quantity"].ToString()),
        //                    QSReturnQty = qsRetQty,
        //                    FromBin = dr["FromBin"].ToString(),
        //                    ToBin = tobin
        //                };
        //                database.SaveData<UnloadReturn>(record);

        //            }
        //            return "Success";
        //        }
        //        else
        //            return "No records!";
        //    }
        //    catch (Exception ex)
        //    {

        //        return ex.Message.ToString();
        //    }
        //}
        #endregion

        #region Get Data
        public async Task<decimal> GetSQLite_SumQSReturnQty(string itemno)
        {
            var obj = database.GetData<ScannedUnloadReturnDoc>();
            return obj.Where(i => i.ItemNo == itemno).ToList().Sum(x => x.QSReturnQty);
        }
        public async Task<ScannedUnloadReturnDoc> GetSQLite_ScannedUnloadReturnDoc(string itemno)
        {
            var obj = database.GetData<ScannedUnloadReturnDoc>();
            return obj.Where(i => i.ItemNo == itemno).FirstOrDefault();
        }          
        public async Task<ObservableCollection<UnloadReturn>> GetSQLite_UnloadReturn()
        {
            ObservableCollection<UnloadReturn> items = new ObservableCollection<UnloadReturn>();
            var obj = database.GetData<UnloadReturn>();
            if (obj != null)
            {
                foreach (UnloadReturn r in obj)
                {
                    items.Add(r);
                }
            }    
            else
                items = null;
            return items;
        }
        public async Task<ScannedLoadDoc> GetSQLite_ScannedLoadDoc(string bagno)
        {
            ScannedLoadDoc item = new ScannedLoadDoc();
            var obj = database.GetData<ScannedLoadDoc>();
            item = obj.Where(i => i.BagNo == bagno).FirstOrDefault();
            return item;
        }
        public async Task<decimal> GetSQLite_SumLoadedItems(string itemno)
        {
            var obj = database.GetData<ScannedLoadDoc>();
            return obj.Where(i => i.ItemNo == itemno).ToList().Sum(x=> x.LoadQty);
        }
        public async Task<decimal> GetSQLite_ItemLoadedQty(string itemno)
        {
            decimal value=0;
            //var obj = database.GetData<Item>();

            string[] parameters = new string[] { itemno };
            var obj = database.Query<Item>("SELECT * FROM Item WHERE ItemNo=?", parameters);

            //Item itm = new Item();
            //itm = obj.Where(i => i.ItemNo == itemno).FirstOrDefault();
            Item itm = new Item();
            itm = obj.FirstOrDefault();
            if (itm != null)
                value = itm.LoadQty;
            else
                value = 0;
            return value;
        }
        public async Task<ScannedSoldDoc> GetSQLite_ScannedSoldDoc(string bagno)
        {
            ScannedSoldDoc item = new ScannedSoldDoc();
            var obj = database.GetData<ScannedSoldDoc>();
            item = obj.Where(i => i.BagNo == bagno).FirstOrDefault();
            return item;
        }       
        public async Task<ScannedUnloadDoc> GetSQLite_ScannedUnloadDocByBagNo(string bagno)
        {
            ScannedUnloadDoc item = new ScannedUnloadDoc();
            var obj = database.GetData<ScannedUnloadDoc>();
            item = obj.Where(i => i.BagNo == bagno).FirstOrDefault();
            return item;
        }
        public async Task<ObservableCollection<ScannedUnloadDoc>> GetSQLite_ScannedUnloadDocs()
        {
            ObservableCollection<ScannedUnloadDoc> items = new ObservableCollection<ScannedUnloadDoc>();
            var obj = database.GetData<ScannedUnloadDoc>();
            if (obj != null) items = ConvertObservable<ScannedUnloadDoc>(obj.Where(i => i.UnloadQty > 0));
            return items;
        }
        public async Task<ScannedUnloadReturnDoc> GetSQLite_ScannedReturnUnloadDocByBagNo(string bagno)
        {
            ScannedUnloadReturnDoc item = new ScannedUnloadReturnDoc();
            var obj = database.GetData<ScannedUnloadReturnDoc>();
            item = obj.Where(i => i.BagNo == bagno).FirstOrDefault();
            return item;
        }
        public async Task<RequestLine> GetSQLite_RequestLinebyEntryNo(string entryNo)
        {
            RequestLine item = new RequestLine();
            var obj = database.GetData<RequestLine>();
            item = obj.Where(i => i.EntryNo == entryNo).FirstOrDefault();
            return item;
        }
        public async Task<UnloadReturn> GetSQLite_UnloadReturnbyEntryNo(int entryNo)
        {
            UnloadReturn item = new UnloadReturn();
            var obj = database.GetData<UnloadReturn>();
            item = obj.Where(i => i.EntryNo == entryNo).FirstOrDefault();
            return item;
        }
        public async Task<UnloadReturn> GetSQLite_UnloadReturnbyItemNo(string itemNo)
        {
            UnloadReturn item = new UnloadReturn();
            var obj = database.GetData<UnloadReturn>();
            item = obj.Where(i => i.ItemNo == itemNo).FirstOrDefault();
            return item;
        }
        public  User LoadSQLite_User(string email, string pswd)
        {
            string[] parameters = new string[] { email, pswd };
            var user = database.Query<User>("Select * from User WHERE Email=? AND pwd=?", parameters);
            return user.FirstOrDefault();
        }
        public User LoadSQLite_UserbyEmail(string email)
        {
            string[] parameters = new string[] { email};
            var user = database.Query<User>("Select * from User WHERE Email=?", parameters);
            return user.FirstOrDefault();
        }
        public User GetSQLlite_User()
        {
            User user = new User();
            var obj = database.GetData<User>();
            user = obj.FirstOrDefault(); //Where(i => i.CustomerNo.Trim() == custno).
            return user;
        }
        public async Task<List<Item>> GetSQLite_Items()
        {
            List<Item> items = new List<Item>();
            string[] parameters = new string[] { "True" };
            var obj = database.Query<Item>("SELECT * FROM Item  WHERE IsActive=? ORDER BY ItemNo ASC", parameters);
            items = obj.ToList();
            return items;
        }
        public async Task<List<VanItem>> GetSQLite_VanItems(string filter)
        {
            List<VanItem> items = new List<VanItem>();

            //var obj = database.GetData<VanItem>();
            string[] parameters = new string[] { null };
            var obj = database.Query<VanItem>("SELECT * FROM VanItem WHERE LoadQty>0 ORDER BY ItemNo ASC", parameters);
            //if (items != null)
            //{
            if (filter == "ALL")
                items = obj.ToList();
            else
            {
                //var gObj = database.GetData<SalesPrice>().Where(s => s.SalesCode == filter)
                //    .GroupBy(i => i.ItemNo)
                //    .Select(g => g.First()).ToList();
                string[] paras = new string[] { filter };
                var gObj = database.Query<SalesPrice>("SELECT * FROM SalesPrice WHERE SalesCode=? GROUP BY ItemNo", paras);
                items = (from itm in obj
                         join gitm in gObj on itm.ItemNo equals gitm.ItemNo
                         select itm).ToList();
            }

            // }
            return items;
        }
        public async Task<List<Item>> GetSQLite_CustomerItems(string filter)
        {
            List<Item> items = new List<Item>();
            List<SalesPrice> SalesItem = new List<SalesPrice>();
            var sObj = database.GetData<SalesPrice>();
            var obj = database.GetData<Item>().Where(x=> x.IsActive=="True");

            
            if (items != null)
            {
                if (filter == "ALL")
                    items = obj.ToList();
                else if(filter=="LOAD_ITEMS")
                    items = obj.Where(i => i.LoadQty > 0).ToList();
                else
                {
                    items = (from itm in obj
                             join sitm in sObj on itm.ItemNo equals sitm.ItemNo
                             where(itm.LoadQty>0) && (sitm.SalesCode==filter)
                             select itm).ToList();
                }
                    
            }
            return items;
        }
        public Item GetSQLite_ItembyItemNo(string itemno)
        {
            string[] parameters = new string[] { itemno.ToLower()};
            var obj = database.Query<Item>("SELECT * FROM Item WHERE LOWER(ItemNo)=?", parameters);

            Item item = new Item();
            //var obj = database.GetData<Item>();
            //item = obj.Where(i => i.ItemNo.Trim().ToUpper() == itemno.ToUpper()).FirstOrDefault();
            item = obj.FirstOrDefault();
            return item;
        }
        public ChangedItem GetSQLite_ChangedItembyItemNo(string itemno)
        {
            string[] parameters = new string[] { itemno.ToLower(),"True" };
            var obj = database.Query<ChangedItem>("SELECT * FROM ChangedItem WHERE LOWER(ItemNo)=? AND IsActive=?", parameters);
            ChangedItem item = new ChangedItem();
            item = obj.FirstOrDefault();
            return item;
        }
        public async Task<Item> GetSQLite_ItembyBarCode(string barcode)
        {
            //Item itm = new Item();
            //var obj = database.GetData<Item>();
            //itm = obj.Where(i => i.BarCode.ToUpper()== barcode.ToUpper()).FirstOrDefault();
            //return itm;

            string[] parameters = new string[] { barcode.ToLower(),"True" };
            var Items = database.Query<Item>("SELECT * FROM Item WHERE LOWER(BarCode)=? AND IsActive=?", parameters);
            return Items.FirstOrDefault();
        }
        public async Task<VanItem> GetSQLite_VanItembyBarCode(string barcode)
        {
            //VanItem itm = new VanItem();
            //var obj = database.GetData<VanItem>();
            //itm = obj.Where(i => i.BarCode.ToUpper() == barcode.ToUpper()).FirstOrDefault();
            //return itm;

            string[] parameters = new string[] { barcode.ToLower() };
            var Items = database.Query<VanItem>("SELECT * FROM VanItem WHERE LOWER(BarCode)=?", parameters);
            return Items.FirstOrDefault();
        }
        public async Task<VanItem> GetSQLite_VanItembyItemNo(string itemno)
        {

            //VanItem item = new VanItem();
            //var obj = database.GetData<VanItem>();
            //item = obj.Where(i => i.ItemNo.Trim().ToUpper() == itemno.ToUpper()).FirstOrDefault();
            //return item;

            string[] parameters = new string[] { itemno.ToLower() };
            var Items = database.Query<VanItem>("SELECT * FROM VanItem WHERE LOWER(ItemNo)=?", parameters);
            return Items.FirstOrDefault();
        }
        public ObservableCollection<Item> GetSQLite_ItemtoUnload()
        {
            ObservableCollection<Item> items = new ObservableCollection<Item>();
            string[] parameters = new string[] { null};
            var obj = database.Query<Item>("SELECT * FROM Item WHERE LoadQty>0 OR ReturnQty>0 OR BadQty>0 OR SoldQty>0 ORDER BY ItemNo ASC", parameters);
            //var obj = database.GetData<Item>();
            //if (obj != null) items = ConvertObservable<Item>(obj.Where(i => i.LoadQty > 0 || i.ReturnQty>0 || i.BadQty>0));
            if (obj != null) items = ConvertObservable<Item>(obj);
            return items;
        }
        public async Task<ObservableCollection<Item>> GetSQLite_ItemUnloadtoSync()
        {
            ObservableCollection<Item> items = new ObservableCollection<Item>();
            string[] parameters = new string[] { null };
            var obj = database.Query<Item>("SELECT * FROM Item WHERE UnloadQty>0 ORDER BY ItemNo ASC", parameters);
            if (obj != null) items = ConvertObservable<Item>(obj);
            return items;
        }
        public async Task<ObservableCollection<RequestLine>> GetSQLite_RequestLinesbyReqestNo(string headno)
        {
            ObservableCollection<RequestLine> items = new ObservableCollection<RequestLine>();
            var obj = database.GetData<RequestLine>();
            if (obj != null) items = ConvertObservable<RequestLine>(obj.Where(i => i.HeaderEntryNo == headno));
            return items;
        }
        public async Task<ObservableCollection<RequestLine>> GetSQLite_UnloadRequestLines(string reqheader)
        {
            ObservableCollection<RequestLine> items = new ObservableCollection<RequestLine>();
            var obj = database.GetData<RequestLine>();
            if (obj != null) items = ConvertObservable<RequestLine>(obj.Where(i => i.UnloadQty > 0 && i.RequestNo==reqheader));
            return items;
        }
        public async Task<List<ItemUOM>> GetSQLite_ItemUOMs()
        {
            List<ItemUOM> items = new List<ItemUOM>();
            var obj = database.GetData<ItemUOM>();
            if (items != null) items = obj.ToList();
            return items;
        }
        public async Task<List<ReasonCode>> GetSQLite_ReasonCodes(string strcontain)
        {
            List<ReasonCode> items = new List<ReasonCode>();
            string[] parameters = new string[] { null };
            var obj = database.Query<ReasonCode>("SELECT * FROM ReasonCode WHERE Code LIKE '%"+ strcontain+ "%'", parameters);
            //var obj = database.GetData<ReasonCode>();
            if (items != null) items = obj.ToList();
            return items;
        }

        public string GetSQLite_ReasonDesc(string rcode)
        {
            string retval = string.Empty;
            //List<ReasonCode> items = new List<ReasonCode>();
            string[] parameters = new string[] { rcode };
            var obj = database.Query<ReasonCode>("SELECT * FROM ReasonCode WHERE Code=?", parameters);
            if (obj != null)
            {
                if(obj.Count>0)
                retval = obj.FirstOrDefault().Description;
            }
            return retval ;
        }
        public async Task<List<SalesPrice>> GetSQLite_SalesPrices()
        {
            List<SalesPrice> items = new List<SalesPrice>();
            var obj = database.GetData<SalesPrice>();
            if (items != null) items = obj.ToList();
            return items;
        }
        public List<PaymentMethod> GetSQLite_PaymentMethods()
        {
            List<PaymentMethod> items = new List<PaymentMethod>();
            var obj = database.GetData<PaymentMethod>();
            if (items != null) items = obj.ToList();
            return items;
        }
        public async Task<List<Customer>> GetSQLite_Customers()
        {
            List<Customer> items = new List<Customer>();
            //var obj = database.GetData<Customer>();
            string[] parameters = new string[] { null };
            var obj = database.Query<Customer>("SELECT * FROM Customer ORDER BY CustomerNo ASC", parameters);
            if (obj != null) items = obj.Where(i => i.Name.Trim() != string.Empty).ToList();
            //items = obj.Where(i => i.ItemNo == itemno).ToList();
            return items;
        }
        public async Task<List<Vendor>> GetSQLite_Vendors()
        {
            List<Vendor> items = new List<Vendor>();
            var obj = database.GetData<Vendor>();
            if (obj != null) items = obj.Where(i => i.VendorName.Trim() != string.Empty).ToList();
            //items = obj.Where(i => i.ItemNo == itemno).ToList();
            return items;
        }
        public async Task<string> GetSQLite_CustomersbySalesPerson(string spcode)
        {
            string retval = string.Empty;
            List<Customer> items = new List<Customer>();
            var obj = database.GetData<Customer>();
            if (obj != null)
            {
                items = obj.Where(i => i.Name.Trim() != string.Empty && i.SalesPersonCode == spcode).ToList();
                if(items!=null)
                {
                    int count = 1;
                    foreach (Customer c in items)
                    {
                        if (items.Count == 1) retval = "'" + c.CustomerNo + "'";
                        else
                        {
                            if (count < items.Count) retval += "'" + c.CustomerNo + "',";
                            else
                                retval += "'" + c.CustomerNo + "'";
                        }
                        count++;
                    }
                    
                }
            }   
           
            return retval;
        }
        public  Customer GetSQLite_CustomerbyCustNo(string custno)
        {
            Customer customer = new Customer();
            var obj = database.GetData<Customer>();
            customer = obj.Where(i => i.CustomerNo.Trim().ToUpper() == custno.ToUpper()).FirstOrDefault();
            return customer;
        }
        public Setup GetSQLite_Setup()
        {
            Setup setup = new Setup();
            //var obj = database.GetData<Setup>();
            string[] parameters = new string[] { null };
            var obj = database.Query<Setup>("SELECT * FROM Setup", parameters);
            setup = obj.FirstOrDefault();
            return setup;
        }
        public async Task<List<CustLedgerEntry>> GetSQLite_CustomerLedgerEntry(string custno)
        {
            List<CustLedgerEntry> items = new List<CustLedgerEntry>();
            var obj = database.GetData<CustLedgerEntry>();
            if (items != null) items = obj.Where(i => i.CustNo.Trim().ToUpper() == custno.ToUpper()).ToList();
            //items = obj.Where(i => i.ItemNo == itemno).ToList();
            return items;
        }
        public async Task<List<CustLedgerEntry>> GetSQLite_CLEUnpaidBill(string custno)
        {
            // Comment on 22-03-2018 - query change to be improved search data speed
            //List<CustLedgerEntry> items = new List<CustLedgerEntry>();
            //var obj = database.GetData<CustLedgerEntry>();
            //if (items != null) items = obj.Where(i => i.TransType.Trim() == "Invoice" && decimal.Parse(i.UnpaidAmount)>0 && i.CustNo==custno).ToList();
            // return items;
            List<CustLedgerEntry> items = new List<CustLedgerEntry>();
            try
            {
                string[] parameters = new string[] { "Invoice", custno };
                items = database.Query<CustLedgerEntry>("SELECT * FROM CustLedgerEntry WHERE TransType=? AND UnpaidAmount>0 AND CustNo=?", parameters);
            }
            catch (Exception)
            {

                items = null;
            }
            
           
            return items;
        }
        public async Task<List<SalesHeader>> GetSQLite_SalesHeadersbyCustNo(string custno)
        {
            ////Comment on 22 - 03 - 2018 - query change to be improved search data speed
            //List<SalesHeader> items = new List<SalesHeader>();
            //var obj = database.GetData<SalesHeader>();
            //if (obj != null) items = ConvertObservable<SalesHeader>(obj.Where(i => i.Status == "Released" && i.DocumentType == "SO" && i.SellToCustomer == custno)).ToList().OrderBy(x=> x.DocumentDate).ToList();
            //return items;
            List<SalesHeader> items = new List<SalesHeader>();
            try
            {
               
                string[] parameters = new string[] { "Released", "SO", custno };
                items = database.Query<SalesHeader>("SELECT * FROM SalesHeader WHERE [Status]=? AND DocumentType=? AND SellToCustomer=?", parameters);
            }
            catch (Exception ex)
            {
                items = null;
            }
            
            return items;
        }
        public async Task<int> GetSQLite_SalesLineCount(string docno)
        {
            int value = 0;
            string[] parameters = new string[] { docno };
            var obj = database.Query<int>("SELECT * FROM SalesLine WHERE DocumentNo=?", parameters);
            if (obj != null) value = obj.ToList().Count;
                return value;
        }
        public async Task<ObservableCollection<SalesHeader>> GetSQLite_SalesHeader()
        {
            ObservableCollection<SalesHeader> items = new ObservableCollection<SalesHeader>();
            var obj = database.GetData<SalesHeader>();
            if (obj != null) items = ConvertObservable<SalesHeader>(obj);
            return items;
        }
        public SalesHeader GetSalesHeaderbyID(int id)
        {
            var Items = database.GetData<SalesHeader>();
            return Items.Where(i => i.ID == id).ToList().FirstOrDefault();
        }
        public async Task<RequestHeader> GetRequestHeaderbyID(int id)
        {
            var Items = database.GetData<RequestHeader>();
            return Items.Where(i => i.ID == id).ToList().FirstOrDefault();
        }
        public async Task<UnloadHeader> GetUnloadHeaderbyID(int id)
        {
            var Items = database.GetData<UnloadHeader>();
            return Items.Where(i => i.ID == id).ToList().FirstOrDefault();
        }
        public SalesHeader GetSalesHeaderbyDocNo(string docNo)
        {
            var Items = database.GetData<SalesHeader>();
            return Items.Where(i => i.DocumentNo == docNo).ToList().FirstOrDefault();
        }
        public Payment GetPaymentbyID(int id)
        {
            var Items = database.GetData<Payment>();
            return Items.Where(i => i.ID == id).ToList().FirstOrDefault();
        } 
        public async Task<ObservableCollection<SalesHeader>> GetSQLite_SalesHeaderbyStatus(string status,string docType)
        {
            ObservableCollection<SalesHeader> items = new ObservableCollection<SalesHeader>();
           // var obj = database.GetData<SalesHeader>();
            string[] parameters = new string[] { status,docType };
            var obj = database.Query<SalesHeader>("SELECT * FROM SalesHeader WHERE Status=? AND DocumentType=?", parameters);
            //if (obj != null) items = ConvertObservable<SalesHeader>(obj.Where(i => i.Status == status && i.DocumentType==docType));
            if (obj != null)
                items = ConvertObservable<SalesHeader>(obj);
            else
                items = null;
            return items;
        }
        public async Task<ObservableCollection<SalesHeader>> GetSQLite_UnsyncSalesHeaderbyStatus(string status, string docType)
        {
            ObservableCollection<SalesHeader> items = new ObservableCollection<SalesHeader>();
            string[] parameters = new string[] { status, docType,"true"};
            var obj = database.Query<SalesHeader>("SELECT * FROM SalesHeader WHERE Status=? AND DocumentType=? AND IsSync!=?", parameters);
            if (obj != null)
                items = ConvertObservable<SalesHeader>(obj);
            else
                items = null;
            return items;
        }
        public async Task<ObservableCollection<SalesHeader>> GetSQLite_SalesOrderbyStatus(string status)
        {
            ObservableCollection<SalesHeader> items = new ObservableCollection<SalesHeader>();
            //var obj = database.GetData<SalesHeader>();
            string[] parameters = new string[] { status};
            var obj = database.Query<SalesHeader>("SELECT * FROM SalesHeader WHERE Status=?", parameters);
            //if (obj != null) items = ConvertObservable<SalesHeader>(obj.Where(i => i.Status == status));
            if (obj != null) items = ConvertObservable<SalesHeader>(obj);
            return items;
        }


        public async Task<ObservableCollection<Payment>> GetSQLite_PaymentbyStatus(string status)
        {
            ObservableCollection<Payment> items = new ObservableCollection<Payment>();
            var obj = database.GetData<Payment>();
            if (obj != null) items = ConvertObservable<Payment>(obj.Where(i => i.RecStatus == status));
            return items;
        }
        public async Task<List<CustomerPriceHistory>> GetSQLite_CustomerPriceHistory(string custno,string itemNo)
        {
            List<CustomerPriceHistory> items = new List<CustomerPriceHistory>();
            var obj = database.GetData<CustomerPriceHistory>();
            if (items != null) items = obj.Where(i => i.CustNo.Trim() == custno && i.ItemNo.Trim()==itemNo).ToList();
            //items = obj.Where(i => i.ItemNo == itemno).ToList();
            return items;
        }        
        public async Task<List<NumberSeries>> GetSQLite_NumberSeries()
        {
            List<NumberSeries> items = new List<NumberSeries>();
            var obj = database.GetData<NumberSeries>();
            if (items != null) items = obj.ToList();
            return items;
        }
        public async Task<List<PaidReference>> GetSQLite_PaidReference()
        {
            List<PaidReference> items = new List<PaidReference>();
            var obj = database.GetData<PaidReference>();
            if (items != null) items = obj.ToList();
            return items;
        }
        public async Task<List<VanItem>> GetSQLite_VanItem()
        {
            //List<VanItem> items = new List<VanItem>();
            //var obj = database.GetData<VanItem>();
            //if (items != null) items = obj.ToList();

            List<VanItem> items = new List<VanItem>();
            string[] parameters = new string[] { null };
            var obj = database.Query<VanItem>("SELECT * FROM VanItem ORDER BY ItemNo ASC", parameters);
            if (items != null) items = obj.ToList();
            return items;
        }
        public async Task<ObservableCollection<SalesLine>> GetSQLite_SalesLine()
        {
            ObservableCollection<SalesLine> items = new ObservableCollection<SalesLine>();
            var obj = database.GetData<SalesLine>();
            if (obj != null) items = ConvertObservable<SalesLine>(obj);
            return items;
        }
        public async Task<SalesLine> GetSalesLinebyID(int id)
        {
            var Items = database.GetData<SalesLine>();
            return Items.Where(i => i.ID == id).ToList().FirstOrDefault();
        }
        public async Task<RequestLine> GetRequestItemLinebyID(int id)
        {
            var Items = database.GetData<RequestLine>();
            return Items.Where(i => i.ID == id).ToList().FirstOrDefault();
        }
        public async Task<DeviceInfo> GetDeviceInfo()
        {
            var Items = database.GetData<DeviceInfo>();
            return Items.ToList().FirstOrDefault();
        }
        public async Task<Customer> GetCustomerbyCode(string code)
        {
            //var Customers = database.GetData<Customer>();
            //return Customers.Where(i => i.CustomerNo == code).ToList().FirstOrDefault();
            string[] parameters = new string[] {code };
            var customer = database.Query<Customer>("SELECT * FROM Customer WHERE CustomerNo=?", parameters);
            return customer.FirstOrDefault();
        }        
        public ObservableCollection<SalesLine> GetSalesLinesbyDocNo(string docNo)
        {
            ObservableCollection<SalesLine>  recSalesLine = new ObservableCollection<SalesLine>();
            recSalesLine.Clear();
            string[] parameters = new string[] { docNo };
            //var Items = database.Query<SalesLine>("SELECT * FROM SalesLine WHERE DocumentNo=? AND IsSync='false'", parameters);
            var Items = database.Query<SalesLine>("SELECT * FROM SalesLine WHERE DocumentNo=?", parameters);
            foreach (var item in Items)
            {
                recSalesLine.Add(item);
            }
            return recSalesLine;
        }

        public async Task<decimal> GetSumSalesLinesbyItemNoAndItemType(string itemno,string itemtype)
        {
            decimal retval = 0;
            ObservableCollection<SalesLine> recSalesLine = new ObservableCollection<SalesLine>();
            recSalesLine.Clear();
            string[] parameters = new string[] { itemno,itemtype};
            //var Items = database.Query<SalesLine>("SELECT * FROM SalesLine WHERE DocumentNo=? AND IsSync='false'", parameters);
            var Items = database.Query<SalesLine>("SELECT * FROM SalesLine WHERE ItemNo=? AND ItemType=? ORDER BY ItemNo ASC", parameters);
            if(Items!=null)
            {
                foreach (var item in Items)
                {
                    recSalesLine.Add(item);
                }
                retval = recSalesLine.Sum(x => x.Quantity);
            }
            
            return retval;
        }
        public async Task<ObservableCollection<UnloadLine>> GetUnloadLinesbyDocNo(string docNo)
        {
            ObservableCollection<UnloadLine> recUnload = new ObservableCollection<UnloadLine>();
            recUnload.Clear();
            string[] parameters = new string[] { docNo,"0"};
            //var Items = database.Query<SalesLine>("SELECT * FROM SalesLine WHERE DocumentNo=? AND IsSync='false'", parameters);
            var Items = database.Query<UnloadLine>("SELECT * FROM UnloadLine WHERE HeaderEntryNo=?  ORDER BY ItemNo ASC", parameters);
            foreach (var item in Items)
            {
                if(item.BadQty!=0 || item.GoodQty!=0)
                recUnload.Add(item);
            }
            return recUnload;
        }
        public async Task<ObservableCollection<RequestLine>> GetRequestLinesbyRequestNo(string reqno)
        {
            ObservableCollection<RequestLine> recs = new ObservableCollection<RequestLine>();
            recs.Clear();
            string[] parameters = new string[] { reqno };
            //var Items = database.Query<SalesLine>("SELECT * FROM SalesLine WHERE DocumentNo=? AND IsSync='false'", parameters);
            var Items = database.Query<RequestLine>("SELECT * FROM RequestLine WHERE RequestNo=? ORDER BY ItemNo ASC", parameters);

            foreach (var item in Items)
            {
                recs.Add(item);
            }
            return recs;
        }
        public async Task<ObservableCollection<Customer>> GetCustomerbySalesPersonCode(string code)
        {
            ObservableCollection<Customer> records = new ObservableCollection<Customer>();
            records.Clear();
            string[] parameters = new string[] { code };
            var Items = database.Query<Customer>("SELECT * FROM Customer WHERE SalesPersonCode=?", parameters);
            foreach (var item in Items)
            {
                records.Add(item);
            }
            return records;
        }
        public ObservableCollection<SalesLine> GetReleaseLinesbyDocNo(string docNo)
        {
            // var Items = database.GetData<SalesLine>();
            //if(App.Id!=0)
            // {
            ObservableCollection<SalesLine> recSalesLine = new ObservableCollection<SalesLine>();
            recSalesLine.Clear();
            string[] parameters = new string[] { docNo };
            var Items = database.Query<SalesLine>("SELECT * FROM SalesLine WHERE DocumentNo=?", parameters);
            foreach (var item in Items)
            {
                recSalesLine.Add(item);
            }
            // }
            return recSalesLine;
        }
        public async Task<ObservableCollection<ItemUOM>> GetItemUOMbyItemNo(string itemNo)
        {
            // var Items = database.GetData<SalesLine>();
            //if(App.Id!=0)
            // {
            ObservableCollection<ItemUOM> rec = new ObservableCollection<ItemUOM>();
            rec.Clear();
            string[] parameters = new string[] { itemNo };
            var Items = database.Query<ItemUOM>("SELECT * FROM ItemUOM WHERE ItemNo=?", parameters);
            foreach (var item in Items)
            {
                rec.Add(item);
            }
            // }
            return rec;
        }
        public List<SalesPrice> GetItemPricebyItemNo(string itemNo,string saleCode)
        {
            // var Items = database.GetData<SalesLine>();
            //if(App.Id!=0)
            // {
            List<SalesPrice> rec = new List<SalesPrice>();
            string[] parameters = new string[] { itemNo.Trim(),saleCode.Trim()};
            var Items = database.Query<SalesPrice>("SELECT * FROM SalesPrice WHERE ItemNo=? and CustomerNo=?", parameters);
            foreach (var item in Items)
            {
                rec.Add(item);
            }
            // }
            return rec;
        }
        public List<SalesPrice> GetItemPricebyItemPriceGroup(string itemNo, string pricegroup)
        {
            // var Items = database.GetData<SalesLine>();
            //if(App.Id!=0)
            // {
            List<SalesPrice> rec = new List<SalesPrice>();
            string[] parameters = new string[] { "Customer Price Group", itemNo, pricegroup };
            //string[] parameters2 = new string[] { "Customer Price Group", itemNo.Trim()};
            //var tmpItems= database.Query<SalesPrice>("SELECT * FROM SalesPrice WHERE SalesType=? and ItemNo=?", parameters2);
            var Items = database.Query<SalesPrice>("SELECT * FROM SalesPrice WHERE SalesType=? and ItemNo=? and SalesCode=?", parameters);
            foreach (var item in Items)
            {
                rec.Add(item);
            }
            // }
            return rec;
        }
        public async Task<ObservableCollection<Payment>> GetPaymentbyStatus(string status)
        {
            // var Items = database.GetData<SalesLine>();
            //if(App.Id!=0)
            // {
            ObservableCollection<Payment> records = new ObservableCollection<Payment>();
            records.Clear();
            string[] parameters = new string[] { status };
            var Items = database.Query<Payment>("SELECT * FROM Payment WHERE RecStatus=?", parameters);
            foreach (var item in Items)
            {
                records.Add(item);
            }
            // }
            return records;
        }
        public ObservableCollection<Payment> GetPaymentOnDate(string ondate)
        {
            // var Items = database.GetData<SalesLine>();
            //if(App.Id!=0)
            // {
            ObservableCollection<Payment> records = new ObservableCollection<Payment>();
            records.Clear();
            string[] parameters = new string[] { ondate };
            var Items = database.Query<Payment>("SELECT * FROM Payment WHERE OnDate=?", parameters);
            foreach (var item in Items)
            {
                records.Add(item);
            }
            // }
            return records;
        }        
        public async Task<ObservableCollection<RequestHeader>> GetSQLite_RequestHeader()
        {
            ObservableCollection<RequestHeader> items = new ObservableCollection<RequestHeader>();
            var obj = database.GetData<RequestHeader>();
            if (obj != null) items = ConvertObservable<RequestHeader>(obj);
            return items;
        }
        public async Task<ObservableCollection<RequestHeader>> GetSQLite_RequestHeaderNotSync()
        {
            ObservableCollection<RequestHeader> records = new ObservableCollection<RequestHeader>();
            records.Clear();
            string[] parameters = new string[] { "true", "loaded" };
            var obj = database.Query<RequestHeader>("SELECT * FROM RequestHeader WHERE IsSync!=? AND CurStatus=? ", parameters);
            //var obj = database.GetData<RequestHeader>();
            if (obj != null)
                records = ConvertObservable<RequestHeader>(obj);
            else
                records = null;
            return records;
        }
        public async Task<ObservableCollection<UnloadHeader>> GetSQLite_UnloadHeader()
        {
            ObservableCollection<UnloadHeader> items = new ObservableCollection<UnloadHeader>();
            var obj = database.GetData<UnloadHeader>();
            if (obj != null) items = ConvertObservable<UnloadHeader>(obj);
            return items;
        }

        public async Task<ObservableCollection<UnloadHeader>> GetSQLite_UnloadHeaderNotSync()
        {
            ObservableCollection<UnloadHeader> records = new ObservableCollection<UnloadHeader>();
            records.Clear();
            string[] parameters = new string[] { "true" };
            var obj = database.Query<UnloadHeader>("SELECT * FROM UnloadHeader WHERE IsSync!=?", parameters);
            //var obj = database.GetData<RequestHeader>();
            if (obj != null)
                records = ConvertObservable<UnloadHeader>(obj);
            else
                records = null;
            return records;
        }
        public async Task<RequestHeader> GetSQLite_RequestHeadebyKey(string  hdkey)
        {
            var obj = database.GetData<RequestHeader>();
            return obj.Where(i => i.EntryNo == hdkey).ToList().FirstOrDefault();
        }
        public async Task<RequestHeader> GetSQLite_RequestHeadebyRequestNo(string reqno)
        {
           // var obj = database.GetData<RequestHeader>();
            string[] parameters = new string[] { reqno };
            var obj = database.Query<RequestHeader>("SELECT * FROM RequestHeader WHERE RequestNo=? ", parameters);
            return obj.FirstOrDefault();
           // return obj.Where(i => i.RequestNo == reqno).ToList().FirstOrDefault();
        }
        public async Task<ObservableCollection<RequestHeader>> GetRequestHeaderbyQuery(string query,string status)
        {
            ObservableCollection<RequestHeader> header = new ObservableCollection<RequestHeader>();
            header.Clear();
            string[] parameters = new string[] { status };
            var Items = database.Query<RequestHeader>("SELECT * FROM RequestHeader "+query,parameters);
            foreach (var item in Items)
            {
                header.Add(item);
            }
            return header;
        }
        public async Task<ObservableCollection<RequestHeader>> GetRequestHeaderbyStatus(string status)
        {
            ObservableCollection<RequestHeader> header = new ObservableCollection<RequestHeader>();
            header.Clear();
            string[] parameters = new string[] { status};
            var Items = database.Query<RequestHeader>("SELECT * FROM RequestHeader WHERE CurStatus=?", parameters);
            foreach (var item in Items)
            {
                header.Add(item);
            }
            return header;
        }

        public bool CheckSalesHeaderNotSync()
        {
            bool retVal = false;
            string[] parameters = new string[] { "true" };
            var Items = database.Query<SalesHeader>("SELECT * FROM SalesHeader WHERE IsSync!=? ", parameters);
            if (Items != null)
            {
                if (Items.Count > 0)
                {
                    retVal = true;
                }
                else
                    retVal = false;
            }
            else
            {
                retVal = false;
            }
            return retVal;
        }

        public bool CheckUnloadHeaderNotSync()
        {
            bool retVal = false;
            string[] parameters = new string[] { "true" };
            var Items = database.Query<UnloadHeader>("SELECT * FROM UnloadHeader WHERE IsSync!=? ", parameters);
            if (Items != null)
            {
                if (Items.Count > 0)
                {
                    retVal = true;
                }
                else
                    retVal = false;
            }
            else
            {
                retVal = false;
            }
            return retVal;
        }

        public bool CheckRequestHeaderNotSync()
        {
            bool retVal = false;
            string[] parameters = new string[] { "true" };
            var Items = database.Query<RequestHeader>("SELECT * FROM RequestHeader WHERE IsSync!=?", parameters);
            if (Items != null)
            {
                if (Items.Count > 0)
                {
                    retVal = true;
                }
                else
                    retVal = false;
            }
            else
            {
                retVal = false;
            }
            return retVal;
        }
        public async Task<ObservableCollection<RequestLine>> GetRequestLinesbyDocNo(string hdEntryNo)
        {
            ObservableCollection<RequestLine> lines = new ObservableCollection<RequestLine>();
            lines.Clear();
            string[] parameters = new string[] { hdEntryNo };
            var Items = database.Query<RequestLine>("SELECT * FROM RequestLine WHERE HeaderEntryNo=?", parameters);
            foreach (var item in Items)
            {
                lines.Add(item);
            }
            return lines;
        }
        public async Task<RequestLine> GetRequestLinebyEntryNo(string entryNo)
        {
            //var Items = database.GetData<RequestLine>();
            //return Items.Where(i => i.EntryNo == entryNo).ToList().FirstOrDefault();

            string[] parameters = new string[] { entryNo };
            var Items = database.Query<RequestLine>("SELECT * FROM RequestLine WHERE EntryNo=?", parameters);
            return Items.ToList().FirstOrDefault();
        }
        public async Task<ContainerInfo> GetContainerInfobyBagLabel(string  bagLabel)
        {
            var Items = database.GetData<ContainerInfo>();
            return Items.Where(i => i.BoxNo == bagLabel).ToList().FirstOrDefault();
        }
        public async Task<ContainerInfo> GetContainerInfobyBagLabelDirect(string bagLabel)
        {
            DataTable dt = new DataTable();
            dt = App.svcManager.RetContainerInfobyBoxNo(bagLabel);
            ContainerInfo info = new ContainerInfo();
            if(dt.Rows.Count>0)
            {

                foreach(DataRow dr in dt.Rows)
                {
                    info = new ContainerInfo
                    {
                        EntryNo = dr["EntryNo"].ToString(),
                        PalletNo = dr["PalletNo"].ToString(),
                        CartonNo = dr["CartonNo"].ToString(),
                        BoxNo = dr["BoxNo"].ToString(),
                        LineNo = int.Parse(dr["LineNo"].ToString()),
                        ItemNo = dr["ItemNo"].ToString(),
                        VariantCode = dr["VariantCode"].ToString(),
                        Quantity = decimal.Parse(dr["Quantity"].ToString()),
                        LoadQty = decimal.Parse(dr["LoadQty"].ToString()),
                        SoldQty = decimal.Parse(dr["SoldQty"].ToString()),
                        UnloadQty = decimal.Parse(dr["UnloadQty"].ToString()),
                        LocationCode = dr["LocationCode"].ToString(),
                        BinCode = dr["BinCode"].ToString(),
                        RefDocNo = dr["RefDocNo"].ToString(),
                        RefDocLineNo = int.Parse(dr["RefDocLineNo"].ToString()),
                        RefDocType = dr["RefDocType"].ToString(),
                    };
                }
            }
            else
            {
                info = null;
            }
            return info;
        }
        public async Task<ContainerInfo> GetSQLite_ContainerInfobyEntryNo(string entno)
        {
            var obj = database.GetData<ContainerInfo>();
            return obj.Where(i => i.EntryNo == entno).ToList().FirstOrDefault();
        }
        public async Task<ContainerInfo> GetContainerInfowithRefDocBagID(string bagLabel,string refdocno)
        {
            var Items = database.GetData<ContainerInfo>();
            return Items.Where(i => i.BoxNo == bagLabel && i.RefDocNo== refdocno).ToList().FirstOrDefault();
        }
        public async Task<ObservableCollection<ContainerInfo>> GetSQLite_ContainerInfobyRequestDocNo(string reqDocNo)
        {
            ObservableCollection<ContainerInfo> lines = new ObservableCollection<ContainerInfo>();
            lines.Clear();
            string[] parameters = new string[] { reqDocNo };
            var Items = database.Query<ContainerInfo>("SELECT * FROM ContainerInfo WHERE RefDocNo=?", parameters);
            foreach (var item in Items)
            {
                lines.Add(item);
            }
            return lines;
        }
        public async Task<ObservableCollection<ContainerInfo>> GetSQLite_ContainerInfobyRequestLineNo(string reqLineNo)
        {
            ObservableCollection<ContainerInfo> lines = new ObservableCollection<ContainerInfo>();
            lines.Clear();
            string[] parameters = new string[] { reqLineNo };
            var Items = database.Query<ContainerInfo>("SELECT * FROM ContainerInfo WHERE MobileEntryNo=?", parameters);
            foreach (var item in Items)
            {
                lines.Add(item);
            }
            return lines;
        }
        public async Task<List<SalesPerson>> GetSalesPersonList()
        {
            DataTable dt = new DataTable();
            dt = App.svcManager.GetUserList();
            List<SalesPerson> persons = new List<SalesPerson>();
            SalesPerson person ;
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    person = new SalesPerson
                    {
                        SalesPersoncode = dr["SalesPersoncode"].ToString()
                    };
                    persons.Add(person);
                }
            }
            return persons;
        }
        public async Task<ObservableCollection<SalesHeader>> GetSQLite_SalesHeaderByDocDate(string strDate) //"yyyy-MM-dd"
        {
            ObservableCollection<SalesHeader> items = new ObservableCollection<SalesHeader>();
            var obj = database.GetData<SalesHeader>();
            if (obj != null) items = ConvertObservable<SalesHeader>(obj.Where(i => i.DocumentDate.Contains(strDate)));
            return items;
        }

        public async Task<ObservableCollection<SalesHeader>> GetSQLite_VoidSalesHeaderByDocDate(string strDate) //"yyyy-MM-dd"
        {
            ObservableCollection<SalesHeader> items = new ObservableCollection<SalesHeader>();
            var obj = database.GetData<SalesHeader>();
            if (obj != null) items = ConvertObservable<SalesHeader>(obj.Where(i => i.IsVoid=="true" && i.DocumentDate.Contains(strDate) && i.DocumentType=="SO"));
            return items;
        }

        #endregion
    }

    public class Database
    {
        static object locker = new object();

        ISQLite SQLite
        {
            get { return DependencyService.Get<ISQLite>(); }
        }

        readonly SQLiteConnection connection;
        readonly string DatabaseName;

        public Database(string databaseName)
        {
            DatabaseName = databaseName;
            connection = SQLite.GetConnection(DatabaseName);

            //  database = new SQLiteAsyncConnection(dbPath);
            //bool flag = Xamarin.Forms.DependencyService.Get<IFileHelper>().IsDbFileExist("DPDeliverySQLite.db3");
            // if (!flag) DatabaseInitialize();
        }

        public void DatabaseInitialize<T>()
        {
            lock (locker)
            {
                connection.DropTable<T>();
                connection.CreateTable<T>();
            }
        }
        public void CreateTable<T>()
        {
            lock (locker)
            {
                connection.CreateTable<T>();
            }
        }

        public long GetSize()
        {
            return SQLite.GetSize(DatabaseName);
        }

        public int SaveData<T>(T obj)
        {
            lock (locker)
            {
                var id = ((BaseItem)(Object)obj).ID;

                if (id != 0)
                {
                    connection.Update(obj);
                    return id;
                }
                else
                {
                    connection.Execute("PRAGMA synchronous = OFF");//THU test
                    connection.Insert(obj);
                    return connection.ExecuteScalar<int>("SELECT last_insert_rowid();");
                }
            }
        }

        public int SaveDataAll<T>(IEnumerable<T> objs)
        {
            lock (locker)
            {
                  connection.Execute("PRAGMA synchronous = OFF");//THU test
                  return  connection.InsertAll(objs);
            }
        }

        public int UpdateDataAll<T>(IEnumerable<T> objs)
        {
            lock (locker)
            {
                connection.Execute("PRAGMA synchronous = OFF");//THU test
                return  connection.UpdateAll(objs);
            }
        }

        public void ExecuteQuery(string query, object[] args)
        {
            lock (locker)
            {
                connection.Execute(query, args);
            }
        }


        public List<T> Query<T>(string query, object[] args) where T : new()
        {
            lock (locker)
            {
                return connection.Query<T>(query, args);
            }
        }

        public IEnumerable<T> GetData<T>() where T : new()
        {
            lock (locker)
            {
                return (from i in connection.Table<T>() select i).ToList();
            }
        }

        public int DeleteData<T>(int id)
        {
            lock (locker)
            {
                return connection.Delete<T>(id);
            }
        }

        public int DeleteAll<T>()
        {
            lock (locker)
            {
                return connection.DeleteAll<T>();
            }
        }
    }
}
