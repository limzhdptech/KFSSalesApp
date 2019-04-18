using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QHSalesApp
{
    public static class Utils
    {
        //public static string Print_Payment(string SelectedBthDevice,Payment record)
        // {
        //     string retval = string.Empty;
        //     try
        //     {
        //         string NewLine = "\x0A";
        //         string cmds = "\x1b\x61\x01\x1b\x45\x01\x1b\x21\x10\x1b\x21\x20      Payment Receipt \x1b\x45\x00";
        //         // string cmds ="\x1b\x45\x01" + "Payment Receipt";// = "\x1B\x45\x01";
        //         cmds += NewLine;
        //         cmds += "\x1b\x61\x01\x1b\x45\x01\x1b\x21\x10\x1b\x21\x20    Qian Hu Fish Farm Trading \x1b\x45\x00"; //text to print
        //         cmds += NewLine;
        //         cmds += "\x1b\x61\x01     No.71 Jalan Lekar Singapore 698950";
        //         cmds += NewLine;


        //         cmds += "\x1b\x21\x00  Date: " + record.OnDate + "                    SalesPerson : " + record.SalesPersonCode;
        //         cmds += NewLine;
        //         cmds += "\x1b\x21\x00  Cust No: " + record.CustomerNo;
        //         cmds += NewLine;
        //         cmds += "\x1b\x21\x00  Cust Name: " + record.CustomerName;
        //         cmds += NewLine;
        //         cmds += "\x1b\x21\x00  Receipt No. : " + record.DocumentNo;
        //         cmds += NewLine;
        //         cmds += "\x1b\x21\x00  Ref Document No : " + record.RefDocumentNo;
        //         cmds += NewLine;
        //         cmds += "\x1b\x21\x00  -------------------------------------------------------";
        //         cmds += NewLine;
        //         cmds += "\x1b\x45\x01\x1b\x21\x10\x1b\x21\x20    Amount(SGD):   $" + record.Amount + " \x1b\x45\x00";
        //         cmds += NewLine;
        //         cmds += NewLine;
        //         //ESC = leftAlign + "\x1B\x46\x00";
        //         cmds += "\x1b\x21\x00  Remarks : " + record.Note;
        //         cmds += NewLine;
        //         cmds += NewLine;
        //         cmds += "\x1b\x21\x00  -------------------------------------------------------";
        //         cmds += NewLine;
        //         cmds += "\x1b\x21\x00  -------------------------------------------------------";
        //         //cmds += "\x1b\x61\x02\x1b\x21\x00                           Customer Signature"; // Signature should be shown as right side.
        //         cmds += NewLine;

        //         retval = DependencyService.Get<IBluetoothPrinter>().Print(SelectedBthDevice, cmds);
        //         return retval;
        //     }
        //     catch (Exception ex)
        //     {

        //         return ex.Message.ToString();
        //     }
        // }
        public static string Print_Payment(string SelectedBthDevice, Payment record)
        {
            string retval = string.Empty;
            try
            {
                string spacing = string.Empty;
                int strlen, halflen;
                string strjoin;
                string title, company, address;
                string NewLine = "\x0A";
                string space = "\x20";
                string fontFormat = "\x1b\x45\x01\x1b\x21\x10";
                string cmds;
                string line = string.Empty;
                for (int i = 0; i < 57; i++)
                {
                    line += "-";

                }
                // Calculate Sales Invoice Text position
                //title = "      Payment Receipt";

                //string cmds = "\x1b\x61\x01\x1b\x45\x01\x1b\x21\x10\x1b\x21\x20  " + title + "\x1b\x45\x00";
                //cmds += NewLine;

                // Calculate Company Name position
                company = "Qian Hu Fish Farm Trading";
                //company = "DP Technology Pte Ltd";
                spacing = string.Empty;
                strlen = 57 - (company.Length); //for Double - width mode
                halflen = strlen / 2;
                for (int i = 1; i < halflen; i++)
                {
                    spacing += space;
                }
                company = spacing + company + spacing;
                //cmds += "\x1b\x61\x01\x1b\x45\x01\x1b\x21\x10\x1b\x21\x20  " + company + "\x1b\x45\x00"; //text to print
                cmds = fontFormat + company; //text to print
                cmds += NewLine;

                // Calculate Company Name position
                address = "No.71 Jalan Lekar Singapore 698950";
                // address = "2304 Bedok Reservoir Road, #04–03";
                spacing = string.Empty;
                strlen = 57 - address.Length;
                halflen = strlen / 2;
                for (int i = 1; i < halflen; i++)
                {
                    spacing += space;
                }
                address = spacing + address + spacing;

                cmds += fontFormat + address;
                cmds += NewLine;
                //address = "Bedok Industrial Park C, Singapore 479223";
                //spacing = string.Empty;
                //strlen = 55 - address.Length;
                //halflen = strlen / 2;
                //for (int i = 1; i < halflen; i++)
                //{
                //    spacing += " ";
                //}
                //address = spacing + address + spacing;

                //cmds += "\x1b\x61\x02  " + address;
                //cmds += NewLine;
                string GSTRegNo = "GST Registration No. : " + App.gRegGSTNo;
                spacing = string.Empty;
                strlen = 57 - GSTRegNo.Length;
                halflen = strlen / 2;
                for (int i = 1; i < halflen; i++)
                {
                    spacing += space;
                }
                GSTRegNo = spacing + GSTRegNo + spacing;

                cmds += fontFormat + GSTRegNo;
                cmds += NewLine;
                cmds += NewLine;
                // Calculate sales person position
                spacing = string.Empty;
                string docDate = "Date: " + record.OnDate;
                strlen = 55 - (docDate).Length; ;
                string salesperson = "Sales Person: " + record.SalesPersonCode;
                strlen = strlen - (salesperson).Length;
                for (int i = 1; i < strlen; i++)
                {
                    spacing += space;
                }
                strjoin = docDate + spacing + salesperson;
                cmds += fontFormat + strjoin;
                spacing = string.Empty;
                string custno = "Cust No: " + record.CustomerNo;
                string custname = "Cust Name: " + record.CustomerName;
                cmds += NewLine;
                cmds += fontFormat + custno;
                cmds += NewLine;
                cmds += fontFormat + custname;
                cmds += NewLine;
                cmds += fontFormat + "Receipt No. : " + record.DocumentNo;
                cmds += NewLine;
                cmds += fontFormat + "Ref Document No : " + record.RefDocumentNo;
                cmds += NewLine;
                cmds += fontFormat + line;
                cmds += NewLine;
                // cmds += NewLine;
                //cmds += "\x1b\x45\x01\x1b\x21\x10\x1b\x21\x20      Amount(SGD): $" + record.Amount + " \x1b\x45\x00";
                spacing = string.Empty;
                string amount = "Amount(SGD): $" + record.Amount;
                strlen = 57 - amount.Length;
                halflen = strlen / 2;
                for (int i = 1; i < halflen; i++)
                {
                    spacing += space;
                }
                amount = spacing + amount + spacing;
                cmds += fontFormat + amount;
                cmds += NewLine;
                // cmds += NewLine;
                //ESC = leftAlign + "\x1B\x46\x00";
                cmds += fontFormat + "Remarks : " + record.Note;
                cmds += NewLine;
                cmds += NewLine;
                cmds += fontFormat + line;
                cmds += NewLine;
                cmds += fontFormat + line;
                //cmds += "\x1b\x61\x02\x1b\x21\x00                           Customer Signature"; // Signature should be shown as right side.
                cmds += NewLine;

                retval = DependencyService.Get<IBluetoothPrinter>().Print(SelectedBthDevice, cmds);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public static string Print_SalesOrder(string SelectedBthDevice, SalesHeader head, ObservableCollection<SalesLine> lines, Customer customer)
        {
            string retval = string.Empty;
            try
            {

                string spacing = string.Empty;
                string space = "\x20";
                int strlen, halflen;
                string no, code, desc, desc2, qty, price, amount, subtotal, gstamt, netamt, title, company, address;
                string NewLine = "\x0A";
                string normalFont = "\x1b\x45\x01\x1b\x21\x10";
                string cmds;
                // Calculate Sales Invoice Text position
                title = "       Sales Invoice";
                string line = string.Empty;
                for (int i = 0; i < 57; i++)
                {
                    line += "-";
                }
                //spacing = string.Empty;
                //strlen = 55- (title.Length*2);// for Double-width mode
                //halflen = strlen / 2;
                //for (int i = 1; i < halflen; i++)
                //{
                //    spacing += " ";
                //}
                //title = spacing + title + spacing;

                //string cmds = "\x1b\x61\x01\x1b\x45\x01\x1b\x21\x10\x1b\x21\x20  "+ title +"\x1b\x45\x00";
                //cmds += NewLine;
                // Calculate Company Name position
                company = "Qian Hu Fish Farm Trading";
                //company = "DP Technology Pte Ltd";
                spacing = string.Empty;
                strlen = 57 - (company.Length); //for Double - width mode
                halflen = strlen / 2;
                for (int i = 0; i < halflen; i++)
                {
                    spacing += space;
                }
                company = spacing + company + spacing;
                //cmds += "\x1b\x45\x01\x1b\x21\x10\x1b\x21\x20"+company+"\x1b\x45\x00"; //text to print
                cmds = normalFont + company; //text to print
                cmds += NewLine;

                // Calculate Company Name position
                address = "No.71 Jalan Lekar Singapore 698950";
                //address = "2304 Bedok Reservoir Road, #04–03";
                spacing = string.Empty;
                strlen = 57 - address.Length;
                halflen = strlen / 2;
                for (int i = 0; i < halflen; i++)
                {
                    spacing += space;
                }
                address = spacing + address + spacing;

                // cmds += "\x1b\x61\x02  " + address;
                cmds += normalFont + address;
                cmds += NewLine;
                //address = "Bedok Industrial Park C, Singapore 479223";
                //spacing = string.Empty;
                //strlen = 55 - address.Length;
                //halflen = strlen / 2;
                //for (int i = 1; i < halflen; i++)
                //{
                //    spacing += " ";
                //}
                //address = spacing + address + spacing;

                //cmds += "\x1b\x61\x02  " + address;
                //cmds += NewLine;
                string GSTRegNo = "GST Registration No. : " + App.gRegGSTNo;
                spacing = string.Empty;
                strlen = 57 - GSTRegNo.Length;
                halflen = strlen / 2;
                for (int i = 0; i < halflen; i++)
                {
                    spacing += space;
                }
                GSTRegNo = spacing + GSTRegNo + spacing;

                cmds += normalFont + GSTRegNo;
                cmds += NewLine;
                cmds += NewLine;
                // --------------- Header --------------------------------
                // Calculate customer position
                string custno = "Cust No: " + customer.CustomerNo;
                string custname = "Cust Name: " + customer.Name;

                cmds += normalFont + custno;
                cmds += NewLine;
                cmds += normalFont + custname;
                cmds += NewLine;
                cmds += normalFont + "Sales No: " + head.DocumentNo;
                // Calculate Posting Date position
                spacing = string.Empty;
                strlen = 57 - ("Sales No: " + head.DocumentNo).Length; ;
                strlen = strlen - ("Posting Date: " + head.DocumentDate).Length;
                for (int i = 0; i < strlen; i++)
                {
                    spacing += space;
                }
                string docDate = spacing + "Posting Date: " + head.DocumentDate;
                cmds += normalFont + docDate;
                cmds += NewLine;
                cmds += normalFont + "Terms: " + customer.PaymentTerms;

                // Calculate Sales Person position
                spacing = string.Empty;
                strlen = 57 - ("Terms: " + customer.PaymentTerms).Length; ;
                strlen = strlen - ("Sales Person: " + App.gSalesPersonCode).Length;
                for (int i = 0; i < strlen; i++)
                {
                    spacing += space;
                }
                string salesperson = spacing + "Sales Person: " + App.gSalesPersonCode;
                cmds += normalFont + salesperson;
                // 123456789012345678901234567890123456789012345678901234567
                //NO CODE      DESC QTY     PRICE AMOUNT
                //NO..CODE......DESC................QTY.....PRICE....AMOUNT |
                //1234 | 123456789 | 123456789012345678 | 1234 | 12345678 | 123456789 |
                //...4 | 10.......| 19................| 5...| 9.......| 10.......| => 57

                cmds += NewLine;
                cmds += normalFont + line;
                cmds += NewLine;
                //cmds += normalFont+"NO  CODE      DESC                QTY   PRICE      AMOUNT";
                cmds += normalFont + "NO  CODE     DESC                QTY     PRICE     AMOUNT";
                cmds += NewLine;
                cmds += normalFont + line;
                cmds += NewLine;
                int count = 1;
                desc2 = string.Empty;
                foreach (SalesLine l in lines)
                {
                    // Calculate No Position

                    if (!string.IsNullOrEmpty(desc2.Trim()))
                    {
                        spacing = string.Empty;
                        for (int i = 0; i < 14; i++)
                        {
                            spacing += space;
                        }
                        cmds += normalFont + spacing + desc2;
                        cmds += NewLine;
                        desc2 = string.Empty;
                    }

                    spacing = string.Empty;
                    strlen = 4 - count.ToString().Length;

                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    no = count + spacing;

                    // Calculate Item code position
                    spacing = string.Empty;
                    strlen = 10 - l.ItemNo.Length;
                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    code = l.ItemNo + spacing;

                    // Calculate Item description position
                    if (l.Description.Length > 20)
                    {
                        desc = l.Description.Substring(0, 20);
                        if (l.Description.Length > 21)
                            desc2 = l.Description.Substring(20, l.Description.Length - 20);
                    }
                    else
                        desc = l.Description;
                    spacing = string.Empty;
                    strlen = 20 - desc.Length;
                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    desc = desc + spacing;
                    // Calculate quantity position
                    spacing = string.Empty;
                    strlen = 5 - l.Quantity.ToString().Length;
                    halflen = strlen / 2;
                    for (int i = 1; i < halflen; i++)
                    {
                        spacing += space;
                    }
                    qty = spacing + l.Quantity.ToString() + spacing;

                    // Calculate unit price position
                    spacing = string.Empty;
                    price = string.Format("{0:0.00}", l.UnitPrice);
                    strlen = 9 - price.Length;
                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    //string.Format("Total : {0:0.00}",TotalAmount);
                    price = spacing + price;

                    // Calculate line amount position
                    spacing = string.Empty;
                    amount = string.Format("{0:0.00}", l.LineAmount);
                    strlen = 10 - amount.Length;
                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    amount = spacing + amount;

                    cmds += normalFont + no + code + desc + qty + price + amount;
                    cmds += NewLine;
                    //cmds += "\x1b\x24\x10\x100 " + l.LineAmount;
                    //cmds += NewLine;
                    count++;
                }
                if (!string.IsNullOrEmpty(desc2.Trim()))
                {
                    spacing = string.Empty;
                    for (int i = 0; i < 14; i++)
                    {
                        spacing += space;
                    }
                    cmds += normalFont + spacing + desc2;
                    cmds += NewLine;
                    desc2 = string.Empty;
                }

                cmds += normalFont + line;
                cmds += NewLine;
                //ESC = leftAlign + "\x1B\x46\x00";
                decimal SubTotal = decimal.Parse(lines.Sum(x => x.LineAmount).ToString());
                decimal GSTAmount = (SubTotal * 7) / 100;
                //43,12
                // Calculate Sub Total position
                spacing = string.Empty;
                string strlabel = "SUBTOTAL:  ";
                strlen = 43 - strlabel.Length;
                for (int i = 0; i < strlen; i++)
                {
                    spacing += space;
                }
                strlabel = spacing + strlabel;

                spacing = string.Empty;
                subtotal = string.Format("{0:0.00}", SubTotal);
                strlen = 12 - subtotal.Length;
                for (int i = 0; i < strlen; i++)
                {
                    spacing += space;
                }
                subtotal = strlabel + spacing + subtotal;

                cmds += normalFont + subtotal;
                cmds += NewLine;
                cmds += normalFont + line;
                cmds += NewLine;

                //43,12
                // Calculate GST TOTAL position
                spacing = string.Empty;
                strlabel = "GST TOTAL (" + App.gPercentGST + "%):  ";
                strlen = 43 - strlabel.Length;
                for (int i = 0; i < strlen; i++)
                {
                    spacing += space;
                }
                strlabel = spacing + strlabel;

                spacing = string.Empty;
                gstamt = string.Format("{0:0.00}", GSTAmount);
                strlen = 12 - gstamt.Length;
                for (int i = 0; i < strlen; i++)
                {
                    spacing += space;
                }
                gstamt = strlabel + spacing + gstamt;

                //ESC = leftAlign + "\x1B\x46\x00";
                cmds += normalFont + gstamt; // Need GST formular
                cmds += NewLine;
                cmds += normalFont + line;
                cmds += NewLine;
                decimal NetAmount = GSTAmount + SubTotal;
                //43,12
                // Calculate TOTAL AMOUNT position
                spacing = string.Empty;
                strlabel = "TOTAL AMOUNT:  ";
                strlen = 43 - strlabel.Length;
                for (int i = 0; i < strlen; i++)
                {
                    spacing += space;
                }
                strlabel = spacing + strlabel;

                spacing = string.Empty;
                netamt = string.Format("{0:0.00}", NetAmount);
                strlen = 12 - netamt.Length;
                for (int i = 0; i < strlen; i++)
                {
                    spacing += space;
                }
                netamt = strlabel + spacing + netamt;
                //ESC = leftAlign + "\x1B\x46\x00";
                cmds += normalFont + netamt;
                cmds += NewLine;
                cmds += normalFont + line;
                //cmds += "\x1b\x61\x02\x1b\x21\x00                           Customer Signature"; // Signature should be shown as right side.
                cmds += NewLine;
                cmds += NewLine;
                cmds += normalFont + line;
                cmds += NewLine;
                cmds += normalFont + line;
                //cmds += "\x1b\x61\x02\x1b\x21\x00                           Customer Signature"; // Signature should be shown as right side.
                cmds += NewLine;
                retval = DependencyService.Get<IBluetoothPrinter>().Print(SelectedBthDevice, cmds);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        public static string Print_TaxInvoice(string SelectedBthDevice, SalesHeader head, ObservableCollection<SalesLine> lines, Customer billToCust, Customer sellToCust, string areaCode)
        {
            string retval = string.Empty;
            try
            {
                // Declaration
                bool is_exchange;
                string spacing = string.Empty;
                string space = "\x20";
                int total_len = 70;
                int half_len = 60;
                int strlen, halflen, pageNo;
                string no, code, desc, desc2, salesqty, exchangeqty, price, amount, subtotal, gstamt, netamt;
                string NewLine = "\x0A";
                string pageBreak = "\x0C";
                string normalFont = "\x1b\x45\x01\x1b\x21\x10";
                string cmds = "";

                //Print Header
                pageNo = 1;
                Print_TaxInvoiceHeader(head, billToCust, sellToCust, areaCode, pageNo.ToString(), ref cmds);

                #region --------------- Print Lines ---------------
                // ---------------------------------------- LINE ----------------------------------------
                //No     Item No      Description               SalesQty     ExchangeQty     UnitPrice      Amount
                //cmds += NewLine;
                int count = 1, linesCount = 1;
                desc2 = string.Empty;
                foreach (SalesLine l in lines)
                {
                    //is_exchange = false;
                    //Exchange:

                    // Calculate No Position
                    if (!string.IsNullOrEmpty(desc2.Trim()))
                    {
                        spacing = string.Empty;
                        for (int i = 0; i < 14; i++)
                        {
                            spacing += space;
                        }
                        cmds += normalFont + spacing + desc2;
                        cmds += NewLine;
                        linesCount++;
                        desc2 = string.Empty;
                    }

                    spacing = string.Empty;
                    strlen = 5 - count.ToString().Length;

                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    no = count + spacing;

                    // Calculate Item no position
                    spacing = string.Empty;
                    strlen = 10 - l.ItemNo.Length;
                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    code = l.ItemNo + spacing;

                    // Calculate Item description position
                    string description = l.Description;
                    //if (is_exchange) description = "EX:" + description;
                    if (l.ItemType == "EXC") description = "EX:" + description;
                    if (l.ItemType == "FOC") description = "FOC:" + description;

                    string codeandDesc = code + description;
                    List<string> warpCodeanddesc = new List<string>();
                    warpCodeanddesc = WordWrap(codeandDesc, 37);
                    string codeandDesctoPrint = "";
                    if (warpCodeanddesc.Count >= 1)
                    {
                        codeandDesctoPrint = warpCodeanddesc[0].ToString().Trim();
                    }
                    if (warpCodeanddesc.Count >= 2)
                    {
                        desc2 = warpCodeanddesc[1].ToString().Trim();
                    }
                    //if (description.Length > 27)
                    //{
                    //    desc = description.Substring(0, 27);
                    //    if (description.Length > 28)
                    //        desc2 = description.Substring(27, description.Length - 27);
                    //}
                    //else
                    //    desc = description;


                    spacing = string.Empty;
                    strlen = 37 - codeandDesctoPrint.Length;
                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    codeandDesctoPrint = codeandDesctoPrint + spacing;

                    // Prepare for normal line and exchange line
                    string _qty, _exqty;
                    decimal _price, _amt;
                    //if (is_exchange)
                    if (l.ItemType == "EXC")
                    {
                        _qty = ""; _exqty = l.Quantity.ToString();
                        _price = _amt = 0;
                    }
                    else
                    {
                        // _qty = l.Quantity.ToString(); _exqty = "";
                        _qty = (l.Quantity + l.BadQuantity).ToString(); _exqty = "";
                        _price = l.UnitPrice; _amt = l.LineAmount;
                    }

                    // Calculate Sales quantity position
                    spacing = string.Empty;
                    _qty = AddSpaceAtString(_qty, true, 6);
                    strlen = 8 - _qty.Length;
                    halflen = strlen / 2;
                    for (int i = 1; i < halflen; i++)
                    {
                        spacing += space;
                    }
                    salesqty = spacing + _qty + spacing;

                    // Calculate Exchange quantity position
                    spacing = string.Empty;
                    _exqty = AddSpaceAtString(_exqty, true, 6);
                    strlen = 10 - _exqty.Length;
                    halflen = strlen / 2;
                    for (int i = 1; i < halflen; i++)
                    {
                        spacing += space;
                    }
                    exchangeqty = spacing + _exqty + spacing;

                    // Calculate unit price position
                    spacing = string.Empty;
                    //price = string.Format("{0:0.00}", _price);
                    price = _price.ToString("N");
                    price = AddSpaceAtString(price, true, 10);
                    strlen = 10 - price.Length;
                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    //string.Format("Total : {0:0.00}",TotalAmount);
                    price = spacing + price;

                    // Calculate line amount position
                    spacing = string.Empty;
                    //amount = string.Format("{0:0.00}", _amt);
                    amount = _amt.ToString("N");
                    amount = AddSpaceAtString(amount, true, 10);
                    strlen = 13 - amount.Length;
                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    amount = spacing + amount;

                    //cmds += normalFont + no + code + desc + salesqty + exchangeqty + price + amount;
                    cmds += normalFont + no + codeandDesctoPrint + salesqty + exchangeqty + price + amount;
                    cmds += NewLine;

                    count++;
                    linesCount++;

                    //desc2 session
                    int lenNoAndCode = 0;
                    lenNoAndCode = no.Length + code.Length;
                    if (!string.IsNullOrEmpty(desc2.Trim()))
                    {
                        spacing = string.Empty;
                        for (int i = 0; i < lenNoAndCode; i++)
                        {
                            spacing += space;
                        }
                        cmds += normalFont + spacing + desc2;
                        cmds += NewLine;
                        linesCount++;
                        desc2 = string.Empty;
                    }
                    //desc2 session


                    //if (l.ItemType == "EXC" && !is_exchange) { is_exchange = true; goto Exchange; }

                    // Next page
                    if (linesCount >= 25)
                    {
                        cmds += pageBreak;
                        pageNo++;
                        linesCount = 1;
                        Print_TaxInvoiceHeader(head, billToCust, sellToCust, areaCode, pageNo.ToString(), ref cmds);
                    }

                }
                #endregion

                #region --------------- Print Footer ---------------
                //int lenNoAndCode = 0;
                //lenNoAndCode = nolen + code.Length;
                //if (!string.IsNullOrEmpty(desc2.Trim()))
                //{
                //    spacing = string.Empty;
                //    for (int i = 0; i < 14; i++)
                //    {
                //        spacing += space;
                //    }
                //    cmds += normalFont + spacing + desc2;
                //    cmds += NewLine;
                //    linesCount++;
                //    desc2 = string.Empty;
                //}

                // Last page condition
                int copylinesCount = 0;
                copylinesCount = linesCount;
                if (linesCount >= 21)
                {
                    cmds += pageBreak;
                    pageNo++;
                    linesCount = 1;
                    Print_TaxInvoiceHeader(head, billToCust, sellToCust, areaCode, pageNo.ToString(), ref cmds);
                }

                decimal SubTotal = decimal.Parse(lines.Sum(x => x.LineAmount).ToString());
                decimal GSTAmount = (SubTotal * 7) / 100;

                // Calcuate spacing 1 and 2 for total section
                // first No->Desc = 16, second No->Amt = 67
                string spacing1 = "", spacing2 = "";
                //for (int i = 0; i < 67; i++)
                //{
                //    if (i < 17) spacing1 += space;
                //    spacing2 += space;
                //}

                //put footer to below
                //for (int i = 0; i < (24 -(copylinesCount + 4)); i++)
                for (int i = 0; i < (24 - (copylinesCount + 7)); i++)
                {
                    cmds += NewLine;
                }

                string beforeLineSpace = "";
                for (int i = 0; i < 14; i++)
                {
                    beforeLineSpace += space;
                    //spacing2 += space;
                }
                // cmds += "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string myline = beforeLineSpace + "__________________________________________________________________"; //Line68
                cmds += myline;

                cmds += NewLine;
                cmds += NewLine;
                cmds += NewLine;
                cmds += NewLine;
                for (int i = 0; i < 17; i++)
                {
                    spacing1 += space;
                    //spacing2 += space;
                }
                for (int i = 0; i < 33; i++)
                {
                    spacing2 += space;
                    //spacing2 += space;
                }


                // Sub Total position
                //string strlabel = "Total Excluding GST  ";

                string strlabel = "Total Excluding GST";
                strlabel = spacing1 + strlabel;
                //                subtotal = string.Format("{0:0.00}", SubTotal);
                subtotal = SubTotal.ToString("N");
                subtotal = AddSpaceAtString(subtotal, true, 10);
                subtotal = strlabel + spacing2 + subtotal;

                cmds += normalFont + subtotal;
                cmds += NewLine;

                spacing1 = "";
                spacing2 = "";
                for (int i = 0; i < 17; i++)
                {
                    spacing1 += space;
                    //spacing2 += space;
                }
                for (int i = 0; i < 43; i++)
                {
                    spacing2 += space;
                    //spacing2 += space;
                }

                // GST position
                strlabel = "GST @ " + App.gPercentGST + "% ";
                //strlabel = "GST Amount";
                strlabel = spacing1 + strlabel;

                //gstamt = string.Format("{0:0.00}", GSTAmount);
                gstamt = GSTAmount.ToString("N");
                gstamt = AddSpaceAtString(gstamt, true, 10);
                gstamt = strlabel + spacing2 + gstamt;
                cmds += normalFont + gstamt; // Need GST formular
                cmds += NewLine;

                spacing1 = "";
                spacing2 = "";
                for (int i = 0; i < 17; i++)
                {
                    spacing1 += space;
                    //spacing2 += space;
                }
                for (int i = 0; i < 39; i++)
                {
                    spacing2 += space;
                    //spacing2 += space;
                }
                // TOTAL AMOUNT position
                decimal NetAmount = GSTAmount + SubTotal;
                strlabel = "Total Payable";
                strlabel = spacing1 + strlabel;
                //netamt = string.Format("{0:0.00}", NetAmount);
                netamt = NetAmount.ToString("N");
                netamt = AddSpaceAtString(netamt, true, 10);
                netamt = strlabel + spacing2 + netamt;
                cmds += normalFont + netamt;
                cmds += NewLine;
                cmds += pageBreak;
                #endregion

                retval = DependencyService.Get<IBluetoothPrinter>().Print(SelectedBthDevice, cmds);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        private static void Print_TaxInvoiceHeader(SalesHeader head, Customer billToCust, Customer sellToCust, string areaCode, string pageNo, ref string cmds)
        {
            string spacing = string.Empty;
            string space = "\x20";
            int half_len = 61;
            int strlen, halflen;
            string NewLine = "\x0A";
            string normalFont = "\x1b\x45\x01\x1b\x21\x10";

            //Skip lines for paper head
            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;

            //string left_spacing = "";
            //for (int i = 0; i < 5; i++)
            //{
            //    left_spacing += space;
            //}

            // Calculate invoice date
            spacing = string.Empty;
            for (int i = 0; i < half_len; i++)
            {
                spacing += space;
            }
            string invDate = spacing + DateTime.Parse(head.DocumentDate).ToString("dd/MM/yy");
            cmds += normalFont + invDate;
            cmds += NewLine;

            // Calculate invoice to > bill to
            string bill_to = head.BillToName;
            cmds += normalFont + bill_to;
            cmds += NewLine;

            // Calculate invoice to > add1 ------------------------------- invoice no
            string add1 = billToCust.Address;
            cmds += normalFont + add1;

            spacing = string.Empty;
            strlen = half_len - add1.Length;
            //strlen = strlen - head.DocumentNo.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string invNo = spacing + head.DocumentNo;
            cmds += normalFont + invNo;
            cmds += NewLine;

            // Calculate invoice to > add2
            string add2 = billToCust.Address2;
            cmds += normalFont + add2;
            cmds += NewLine;

            // Calculate invoice to > postal ------------------------------- account no
            //string postal = billToCust.Country + space + billToCust.Postcode;
            string postal = billToCust.City + space + billToCust.Postcode;
            cmds += normalFont + postal;

            spacing = string.Empty;
            strlen = half_len - postal.Length;
            //strlen = strlen - head.SellToCustomer.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string accNo = spacing + head.SellToCustomer;
            cmds += normalFont + accNo;
            cmds += NewLine;

            // Skip P/O No line 
            cmds += NewLine;
            spacing = string.Empty;
            for (int i = 0; i < half_len; i++)
            {
                spacing += space;
            }
            string StrPONo = "";
            if (!string.IsNullOrEmpty(head.ExternalDocNo))
            {
                StrPONo = spacing + head.ExternalDocNo;
            }
            cmds += normalFont + StrPONo;

            cmds += NewLine;
            cmds += NewLine;

            // Calculate deliver to > ship to ------------------------------- payment terms
            string ship_to = head.SellToName;
            cmds += normalFont + ship_to;

            spacing = string.Empty;
            strlen = half_len - ship_to.Length;
            string paymentTerms = "";
            if (!String.IsNullOrEmpty(billToCust.PaymentTermsDesc))
            {
                paymentTerms = billToCust.PaymentTermsDesc;
            }
            //strlen = strlen - paymentTerms.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            paymentTerms = spacing + paymentTerms;
            cmds += normalFont + paymentTerms;
            cmds += NewLine;

            // Calculate deliver to > add1
            add1 = sellToCust.Address;
            cmds += normalFont + add1;
            cmds += NewLine;

            // Calculate deliver to > add2  ------------------------------- area
            add2 = sellToCust.Address2;
            cmds += normalFont + add2;

            spacing = string.Empty;
            strlen = half_len - add2.Length;
            //strlen = strlen - areaCode.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string area = spacing + TruncateString(areaCode, 15);
            cmds += normalFont + area;
            cmds += NewLine;


            // Calculate deliver to > postal ------------------------------- page no
            //postal = sellToCust.Country + space + sellToCust.Postcode;
            postal = sellToCust.City + space + sellToCust.Postcode;
            cmds += normalFont + postal;

            spacing = string.Empty;
            strlen = half_len - postal.Length;
            //strlen = strlen - ("Page No: 1").Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string pageno = spacing + ("Page No: " + pageNo);
            cmds += normalFont + pageno;
            cmds += NewLine;
            // ---------------------------------------- END HEADER ----------------------------------------

            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;
        }
        public static string Print_CheckIn(string SelectedBthDevice, RequestHeader head, ObservableCollection<RequestLine> lines, string companyName, string areaCode, bool isBeforeConfirmed)
        {
            string retval = string.Empty;
            try
            {
                string spacing = string.Empty;
                string space = "\x20";
                int total_len = 80;
                int half_len = 37;
                int strlen, halflen;
                string desc, desc2, uom, qty;
                string NewLine = "\x0A";
                string pageBreak = "\x0C";
                string normalFont = "\x1b\x45\x01\x1b\x21\x10";
                string cmds = "";

                //Print Header
                if (isBeforeConfirmed)
                {
                    Print_CheckHeader("LOADSHEET", companyName, head.RequestNo, head.RequestDate, areaCode, ref cmds);
                }
                else
                {
                    Print_CheckHeader("CHECK-IN", companyName, head.RequestNo, head.RequestDate, areaCode, ref cmds);
                }

                #region --------------- Print Lines ---------------
                // ---------------------------------------- LINE ----------------------------------------
                // 27 - 10 - 41 = 78
                cmds += "________________________________________________________________________________";
                cmds += "Item   Description                                   UOM             Quantity   ";
                cmds += "________________________________________________________________________________";
                cmds += NewLine;

                int count = 1, linesCount = 1;
                desc2 = string.Empty;
                List<RequestLine> filter = new List<RequestLine>();
                if (isBeforeConfirmed == true)
                {
                    filter = lines.Where(x => x.Quantity != 0).ToList();
                }
                else
                {
                    filter = lines.Where(x => x.LoadQty != 0).ToList();
                }

                List<RequestLine> sorted = filter.OrderBy(x => x.ItemDesc).ToList();
                foreach (RequestLine l in sorted)
                {

                    // Calculate Item description position
                    if (!string.IsNullOrEmpty(desc2.Trim()))
                    {
                        cmds += normalFont + desc2;
                        cmds += NewLine;
                        linesCount++;
                        desc2 = string.Empty;
                    }
                    string description = l.ItemNo + " - " + l.ItemDesc;
                    if (description.Length > 50)
                    {
                        desc = description.Substring(0, 50);
                        if (description.Length > 51)
                            desc2 = description.Substring(50, description.Length - 50);
                    }
                    else
                        desc = description;
                    spacing = string.Empty;
                    strlen = 50 - desc.Length;
                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    desc = desc + spacing;


                    // Calculate uom position
                    spacing = string.Empty;
                    uom = l.UomCode;
                    strlen = 20 - uom.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    uom = uom + spacing;

                    // Calculate quantity position
                    spacing = string.Empty;
                    if (isBeforeConfirmed == true)
                    {
                        qty = l.Quantity.ToString();
                    }
                    else
                    {
                        qty = l.LoadQty.ToString();
                    }
                    strlen = 5 - qty.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    //if (isBeforeConfirmed)
                    //{
                    //    qty = "";
                    //}
                    //else
                    //{
                    //    qty = AddSpaceAtString(qty, true, 6);

                    //}
                    qty = AddSpaceAtString(qty, true, 6);
                    qty = qty + spacing;

                    cmds += normalFont + desc + uom + qty;
                    cmds += NewLine;

                    count++;
                    linesCount++;

                    // Next page
                    if (linesCount >= 45)
                    {
                        cmds += pageBreak;
                        //pageNo++;
                        linesCount = 1;
                        //Print_CheckHeader("CHECK-IN", companyName, head.RequestNo, head.RequestDate, areaCode, ref cmds);
                        if (isBeforeConfirmed)
                        {
                            Print_CheckHeader("LOADSHEET", companyName, head.RequestNo, head.RequestDate, areaCode, ref cmds);
                        }
                        else
                        {
                            Print_CheckHeader("CHECK-IN", companyName, head.RequestNo, head.RequestDate, areaCode, ref cmds);
                        }
                        //THU
                        cmds += "________________________________________________________________________________";
                        cmds += "Item   Description                                   UOM             Quantity   ";
                        cmds += "________________________________________________________________________________";
                        cmds += NewLine;
                        //THU
                    }

                }
                #endregion

                if (!string.IsNullOrEmpty(desc2.Trim()))
                {
                    cmds += normalFont + desc2;
                    cmds += NewLine;
                    desc2 = string.Empty;
                }

                //Last page condition
                if (linesCount > 35)
                {
                    cmds += pageBreak;
                    //pageNo++;
                    linesCount = 0;
                    //Print_CheckHeader("CHECK-IN", companyName, head.RequestNo, head.RequestDate, areaCode, ref cmds);
                    if (isBeforeConfirmed)
                    {
                        Print_CheckHeader("LOADSHEET", companyName, head.RequestNo, head.RequestDate, areaCode, ref cmds);
                    }
                    else
                    {
                        Print_CheckHeader("CHECK-IN", companyName, head.RequestNo, head.RequestDate, areaCode, ref cmds);
                    }

                }
                for (int i = 1; i < (35 - linesCount); i++)
                {
                    cmds += NewLine;
                }

                string Qtytotal = "0.00";
                //string Qtytotal = lines.Sum(x => x.Quantity).ToString() + space + space;
                //string Qtytotal = lines.Sum(x => x.LoadQty).ToString();

                spacing = string.Empty;
                if (isBeforeConfirmed == true)
                {
                    Qtytotal = lines.Sum(x => x.Quantity).ToString();
                }
                else
                {
                    Qtytotal = lines.Sum(x => x.LoadQty).ToString();
                }


                Qtytotal = AddSpaceAtString(Qtytotal, true, 9);

                cmds += "________________________________________________________________________________";
                cmds += "                                            Total Quantity  :      " + Qtytotal + "    "; //74 + 6
                cmds += "________________________________________________________________________________";
                cmds += NewLine;
                cmds += NewLine;
                cmds += NewLine;
                cmds += NewLine;
                cmds += "______________________________                    ______________________________";
                cmds += "        Supervisor                                              Driver          ";
                cmds += NewLine;

                spacing = string.Empty;
                for (int i = 0; i < 27; i++)
                {
                    spacing += space;
                }
                string printDate = "Printed Date/Time : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm tt");
                cmds += normalFont + spacing + printDate;
                cmds += NewLine;
                cmds += pageBreak;

                retval = DependencyService.Get<IBluetoothPrinter>().Print(SelectedBthDevice, cmds);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public static string TruncateString(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        public static string AddSpaceAtString(this string value, bool isFront, int maxLength)
        {
            string strResult = "";
            string space = "\x20";
            if (string.IsNullOrEmpty(value))
            {
                for (int i = 0; i < maxLength; i++)
                {
                    strResult += space;
                }
                return strResult;
            }

            if (value.Length > maxLength)
            {
                value = value.Substring(0, maxLength);
            }

            string strSpace = "";
            for (int i = 0; i < maxLength - value.Length; i++)
            {
                strSpace += space;
            }
            if (isFront)
            {
                return strSpace + value;
            }
            else
            {
                return value + strSpace;
            }

            return strResult;

            //return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        private static void Print_CheckHeader(string title, string companyName, string transNo, string docDate, string areaCode, ref string cmds)
        {
            string spacing = string.Empty;
            string space = "\x20";
            int total_len = 78;
            int strlen, half_len = 37;
            string NewLine = "\x0A";
            string normalFont = "\x1b\x45\x01\x1b\x21\x10";

            //Skip lines for paper head
            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;

            // Calculate company name
            cmds += normalFont + companyName;
            cmds += NewLine;

            // Calculate title
            spacing = string.Empty;
            strlen = total_len - title.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            cmds += normalFont + spacing + title;
            cmds += NewLine;

            // Calculate trans no  ------------------------------- docu date
            string docNo = "Transaction No: " + transNo;
            cmds += normalFont + docNo;

            spacing = string.Empty;
            strlen = half_len - docNo.Length;
            //strlen = strlen - head.DocumentNo.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string docuDate = "Doc Date/Time : " + DateTime.Parse(docDate).ToString("dd/MM/yyyy - HH:mm tt");
            cmds += normalFont + spacing + docuDate;
            cmds += NewLine;

            // Calculate salesman ------------------------------- print date
            string salesMan = "SALESMAN NAME : " + areaCode;
            cmds += normalFont + salesMan;

            spacing = string.Empty;
            strlen = half_len - salesMan.Length;
            //strlen = strlen - head.SellToCustomer.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string printDate = "Printed Date/Time : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm tt");
            cmds += normalFont + spacing + printDate;
            cmds += NewLine;

        }
        public static string Print_CheckOut(string SelectedBthDevice, UnloadHeader head, ObservableCollection<UnloadLine> lines, string companyName, string areaCode)
        {
            string retval = string.Empty;
            try
            {
                string spacing = string.Empty;
                string space = "\x20";
                int total_len = 80;
                int half_len = 37;
                int strlen, halflen;
                string desc, desc2, uom, goodqty, badqty;
                string NewLine = "\x0A";
                string pageBreak = "\x0C";
                string normalFont = "\x1b\x45\x01\x1b\x21\x10";
                string cmds = "";

                //Print Header
                Print_CheckHeader("CHECK-OUT", companyName, head.UnloadDocNo, head.UnloadDate, areaCode, ref cmds);

                #region --------------- Print Lines ---------------
                // ---------------------------------------- LINE ----------------------------------------
                // 27 - 10 - 41 = 78
                cmds += "________________________________________________________________________________";
                cmds += "Item Description                          UOM            Good Qty      Bad Qty  ";
                cmds += "________________________________________________________________________________";
                cmds += NewLine;

                int count = 1, linesCount = 1;
                desc2 = string.Empty;
                List<UnloadLine> sorted = lines.OrderBy(x => x.ItemDesc).ToList();
                foreach (UnloadLine l in sorted)
                {

                    // Calculate Item description position
                    if (!string.IsNullOrEmpty(desc2.Trim()))
                    {
                        cmds += normalFont + desc2;
                        cmds += NewLine;
                        linesCount++;
                        desc2 = string.Empty;
                    }
                    string description = l.ItemNo + " - " + l.ItemDesc;
                    if (description.Length > 42)
                    {
                        desc = description.Substring(0, 42);
                        if (description.Length > 43)
                            desc2 = description.Substring(42, description.Length - 42);
                    }
                    else
                        desc = description;
                    spacing = string.Empty;
                    strlen = 42 - desc.Length;
                    for (int i = 0; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    desc = desc + spacing;


                    // Calculate uom position
                    spacing = string.Empty;
                    uom = l.ItemUom;
                    strlen = 17 - uom.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    uom = uom + spacing;

                    // Calculate good qty position
                    spacing = string.Empty;
                    goodqty = l.GoodQty.ToString();
                    strlen = 6 - goodqty.Length;
                    for (int i = 1; i <= strlen; i++)
                    {
                        //spacing += space;
                        spacing += '-';
                    }
                    goodqty = AddSpaceAtString(goodqty, true, 6);
                    // goodqty = goodqty + spacing;

                    // Calculate bad qty position
                    spacing = string.Empty;
                    badqty = l.BadQty.ToString();
                    strlen = 2 - badqty.Length;
                    for (int i = 1; i <= strlen; i++)
                    {
                        spacing += space;

                    }
                    badqty = AddSpaceAtString(badqty, true, 12);
                    badqty = badqty + spacing;

                    cmds += normalFont + desc + uom + goodqty + badqty;
                    cmds += NewLine;

                    count++;
                    linesCount++;

                    // Next page
                    if (linesCount >= 45)
                    {
                        cmds += pageBreak;
                        //pageNo++;
                        linesCount = 1;
                        Print_CheckHeader("CHECK-OUT", companyName, head.UnloadDocNo, head.UnloadDate, areaCode, ref cmds);
                        //THU
                        cmds += "________________________________________________________________________________";
                        cmds += "Item Description                          UOM            Good Qty      Bad Qty  ";
                        cmds += "________________________________________________________________________________";
                        cmds += NewLine;
                        //THU
                    }

                }
                #endregion

                if (!string.IsNullOrEmpty(desc2.Trim()))
                {
                    cmds += normalFont + desc2;
                    cmds += NewLine;
                    desc2 = string.Empty;
                }

                //Last page condition
                if (linesCount > 35)
                {
                    cmds += pageBreak;
                    //pageNo++;
                    linesCount = 0;
                    Print_CheckHeader("CHECK-OUT", companyName, head.UnloadDocNo, head.UnloadDate, areaCode, ref cmds);
                }
                for (int i = 1; i < (35 - linesCount); i++)
                {
                    cmds += NewLine;
                }

                string GoodQtyTotal = lines.Sum(x => x.GoodQty).ToString() + space + space;
                string BadQtyTotal = lines.Sum(x => x.BadQty).ToString() + space + space;

                GoodQtyTotal = AddSpaceAtString(GoodQtyTotal, true, 6);
                BadQtyTotal = AddSpaceAtString(BadQtyTotal, true, 6);


                //cmds += "________________________________________________________________________________";
                //cmds += "                                   Total Quantity  :  " + GoodQtyTotal + BadQtyTotal;
                //cmds += "________________________________________________________________________________";
                //cmds += NewLine;
                //cmds += NewLine;
                //cmds += "______________________________                    ______________________________";
                //cmds += "        Supervisor                                              Driver          ";
                //cmds += NewLine;

                cmds += "________________________________________________________________________________";
                cmds += "                                       Total Quantity  :    " + GoodQtyTotal + "       " + BadQtyTotal + " "; //74 + 6
                cmds += "________________________________________________________________________________";
                cmds += NewLine;
                cmds += NewLine;
                cmds += NewLine;
                cmds += NewLine;
                cmds += "______________________________                    ______________________________";
                cmds += "        Supervisor                                              Driver          ";
                cmds += NewLine;

                spacing = string.Empty;
                for (int i = 0; i < 27; i++)
                {
                    spacing += space;
                }
                string printDate = "Printed Date/Time : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm tt");
                cmds += normalFont + spacing + printDate;
                cmds += NewLine;
                cmds += pageBreak;
                retval = DependencyService.Get<IBluetoothPrinter>().Print(SelectedBthDevice, cmds);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public static string Print_Activity(string SelectedBthDevice, ObservableCollection<SalesHeader> head, string companyName, string saleManNo, string saleManName)
        {
            string retval = string.Empty;
            try
            {
                string spacing = string.Empty;
                string space = "\x20";
                int total_len = 80;
                int half_len = 37;
                int strlen, halflen, pageNo;
                string docno, custno, custname, custname2, date, terms, status, amt;
                string NewLine = "\x0A";
                string pageBreak = "\x0C";
                string normalFont = "\x1b\x45\x01\x1b\x21\x10";
                string cmds = "";

                //Print Header
                pageNo = 1;
                Print_ActivityHeader(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);

                #region --------------- Print Lines ---------------

                //cmds += NewLine;
                custname2 = string.Empty;
                int count = 0, linesCount = 1;
                foreach (SalesHeader l in head)
                {
                    if (!string.IsNullOrEmpty(custname2.Trim()))
                    {
                        spacing = string.Empty;
                        strlen = 17 - spacing.Length;
                        for (int i = 1; i < strlen; i++)
                        {
                            spacing += space;
                        }

                        cmds += normalFont + spacing + custname2;
                        cmds += NewLine;
                        linesCount++;
                        custname2 = string.Empty;
                    }

                    // Doc No
                    docno = l.DocumentNo + space;

                    // Cust No
                    spacing = string.Empty;
                    strlen = 9 - l.SellToCustomer.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    custno = l.SellToCustomer + spacing;

                    // Cust Name
                    spacing = string.Empty;
                    //strlen = 22 - l.SellToName.Length;
                    strlen = 0;
                    custname = l.SellToName;

                    if (custname.Length > 21)
                    {
                        custname = l.SellToName.Substring(0, 21);
                        if (l.SellToName.Length > 22)
                            custname2 = l.SellToName.Substring(21, l.SellToName.Length - 21);
                    }
                    else
                    {
                        strlen = 22 - l.SellToName.Length;
                        custname = l.SellToName;
                    }


                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }

                    custname = custname + spacing;
                    //custname = l.SellToName + spacing;

                    // Date
                    date = " " + DateTime.Parse(l.DocumentDate).ToString("dd/MM/yy HH:mm");

                    // Terms
                    spacing = string.Empty;
                    if (l.PaymentMethod != null)
                    {
                        strlen = 5 - l.PaymentMethod.Length;
                    }
                    else
                    {
                        strlen = 5;
                    }
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    terms = l.PaymentMethod + spacing;

                    // Status
                    spacing = string.Empty;
                    for (int i = 1; i < 3; i++)
                    {
                        spacing += space;
                    }

                    status = l.IsVoid == "true" ? "V " + spacing : "SO" + spacing;
                    if (l.DocumentType == "CN")
                    {
                        status = "CN" + spacing;
                        if (l.IsVoid == "true")
                        {
                            status = "V " + spacing;
                        }
                    }
                    // Amt
                    //amt = string.Format("{0:0.00}", l.NetAmount);
                    //amt =  l.NetAmount.ToString("N"); ;
                    amt = l.IsVoid == "true" ? "0.00" : l.NetAmount.ToString("N");
                    //amt = AddSpaceAtString(amt, true, 9);
                    if (l.DocumentType == "CN")
                    {
                        amt = "-" + amt;
                        if (l.IsVoid == "true")
                        {
                            amt = "0.00";
                        }
                    }
                    amt = AddSpaceAtString(amt, true, 10);
                    cmds += normalFont + docno + custno + custname + date + terms + status + amt;
                    cmds += NewLine;

                    count++;
                    linesCount++;

                    // Next page
                    if (linesCount >= 45)
                    {
                        cmds += pageBreak;
                        pageNo++;
                        linesCount = 1;
                        Print_ActivityHeader(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);
                    }

                }
                #endregion

                if (!string.IsNullOrEmpty(custname2.Trim()))
                {
                    spacing = string.Empty;
                    strlen = 17 - spacing.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }

                    cmds += normalFont + spacing + custname2;
                    cmds += NewLine;
                    custname2 = string.Empty;
                }


                //Last page condition
                if (linesCount > 42)
                {
                    cmds += pageBreak;
                    pageNo++;
                    linesCount = 0;
                    Print_ActivityHeader(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);
                }
                for (int i = 1; i < (42 - linesCount); i++)
                {
                    cmds += NewLine;
                }

                // string AmtTotal = head.Sum(x => x.NetAmount).ToString();
                decimal AmountSO = 0;
                decimal AmountCN = 0;
                AmountSO = head.Where(c => c.DocumentType == "SO" && c.IsVoid != "true").Sum(c => c.NetAmount);
                AmountCN = head.Where(c => c.DocumentType == "CN").Sum(c => c.NetAmount);
                string AmtTotal = (AmountSO - AmountCN).ToString("N");
                string strCount = count.ToString();
                strCount = AddSpaceAtString(strCount, true, 5);
                AmtTotal = AddSpaceAtString(AmtTotal, true, 9);

                //cmds += "--------------------------------------------------------------------------------";
                //cmds += "Count : " + count.ToString();
                //cmds += "    Status:(SO-Sales, V-Voided, CN-Credit Note)  Total : " + AmtTotal + "     ";
                //cmds += "--------------------------------------------------------------------------------";

                cmds += "--------------------------------------------------------------------------------";
                cmds += "Count : " + strCount + "    Status:(SO-Sales, V-Voided, CN-Credit Note)  Total : " + AmtTotal + " ";//8 + 5 + 57 + 9 +1
                cmds += "--------------------------------------------------------------------------------";

                retval = DependencyService.Get<IBluetoothPrinter>().Print(SelectedBthDevice, cmds);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        private static void Print_ActivityHeader(string companyName, string salesManNo, string salesManName, string pageNo, ref string cmds)
        {
            string spacing = string.Empty;
            string space = "\x20";
            int total_len = 80;
            int strlen, half_len = 37;
            string NewLine = "\x0A";
            string normalFont = "\x1b\x45\x01\x1b\x21\x10";

            //Skip lines for paper head
            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;

            // Calculate company name
            //cmds += normalFont + companyName;
            //cmds += NewLine;

            // Calculate title
            spacing = string.Empty;
            string title = "ACTIVITY REPORT";
            int companyNameLen = 30;
            strlen = (total_len - companyNameLen - 2) - title.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            cmds += normalFont + companyName + spacing + title;
            cmds += NewLine;

            // Calculate SALESMAN NO  ------------------------------- PAGE
            string idNo = "SALESMAN ID/NO : " + salesManNo;
            cmds += normalFont + idNo;

            spacing = string.Empty;
            strlen = half_len - idNo.Length;
            //strlen = strlen - head.DocumentNo.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string pageNum = "PAGE : " + pageNo;
            cmds += normalFont + spacing + pageNum;
            cmds += NewLine;

            // Calculate SALESMAN name ------------------------------- print date
            string salesMan = "SALESMAN NAME : " + salesManName;
            cmds += normalFont + salesMan;

            spacing = string.Empty;
            strlen = half_len - salesMan.Length;
            //strlen = strlen - head.SellToCustomer.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string printDate = "Date : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm tt");
            cmds += normalFont + spacing + printDate;
            cmds += NewLine;
            cmds += NewLine;

        }
        public static string Print_DailySalesInvSummarySO(string SelectedBthDevice, ObservableCollection<SalesHeader> head, string companyName, string saleManNo, string saleManName)
        {
            string retval = string.Empty;
            try
            {
                string spacing = string.Empty;
                string space = "\x20";
                int total_len = 80;
                int half_len = 37;
                int strlen, halflen, pageNo;
                string docno, custno, custname, custname2, date, terms, status, amt;
                string NewLine = "\x0A";
                string pageBreak = "\x0C";
                string normalFont = "\x1b\x45\x01\x1b\x21\x10";
                string cmds = "";

                //Print Header
                pageNo = 1;
                Print_DailySalesInvSummaryHeaderSO(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);

                #region --------------- Print Lines ---------------

                var lines = head.Where(x => x.DocumentType == "SO").ToList();

                //cmds += NewLine;
                custname2 = string.Empty;
                int count = 0, linesCount = 1;
                foreach (SalesHeader l in lines)
                {
                    if (!string.IsNullOrEmpty(custname2.Trim()))
                    {
                        spacing = string.Empty;
                        strlen = 17 - spacing.Length;
                        for (int i = 1; i < strlen; i++)
                        {
                            spacing += space;
                        }

                        cmds += normalFont + spacing + custname2;
                        cmds += NewLine;
                        linesCount++;
                        custname2 = string.Empty;
                    }

                    // Doc No
                    docno = l.DocumentNo + space;

                    // Cust No
                    spacing = string.Empty;
                    strlen = 9 - l.SellToCustomer.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    custno = l.SellToCustomer + spacing;

                    // Cust Name
                    spacing = string.Empty;
                    //strlen = 22 - l.SellToName.Length;
                    strlen = 0;
                    custname = l.SellToName;

                    if (custname.Length > 21)
                    {
                        custname = l.SellToName.Substring(0, 21);
                        if (l.SellToName.Length > 22)
                            custname2 = l.SellToName.Substring(21, l.SellToName.Length - 21);
                    }
                    else
                    {
                        strlen = 22 - l.SellToName.Length;
                        custname = l.SellToName;
                    }


                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }

                    custname = custname + spacing;
                    //custname = l.SellToName + spacing;

                    // Date
                    date = " " + DateTime.Parse(l.DocumentDate).ToString("dd/MM/yy HH:mm");

                    // Terms
                    spacing = string.Empty;
                    if (l.PaymentMethod != null)
                    {
                        strlen = 5 - l.PaymentMethod.Length;
                    }
                    else
                    {
                        strlen = 5;
                    }
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    terms = l.PaymentMethod + spacing;

                    // Status
                    spacing = string.Empty;
                    for (int i = 1; i < 3; i++)
                    {
                        spacing += space;
                    }

                    status = l.IsVoid == "true" ? "V " + spacing : "SO" + spacing;
                    if (l.DocumentType == "CN")
                    {
                        status = "CN" + spacing;
                        if (l.IsVoid == "true")
                        {
                            status = "V " + spacing;
                        }
                    }
                    // Amt
                    //amt = string.Format("{0:0.00}", l.NetAmount);
                    //amt =  l.NetAmount.ToString("N"); ;
                    amt = l.IsVoid == "true" ? "0.00" : l.NetAmount.ToString("N");
                    //amt = AddSpaceAtString(amt, true, 9);
                    if (l.DocumentType == "CN")
                    {
                        amt = "-" + amt;
                        if (l.IsVoid == "true")
                        {
                            amt = "0.00";
                        }
                    }
                    amt = AddSpaceAtString(amt, true, 10);
                    cmds += normalFont + docno + custno + custname + date + terms + status + amt;
                    cmds += NewLine;

                    count++;
                    linesCount++;

                    // Next page
                    if (linesCount >= 45)
                    {
                        cmds += pageBreak;
                        pageNo++;
                        linesCount = 1;
                        Print_DailySalesInvSummaryHeaderSO(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);
                    }

                }
                #endregion

                if (!string.IsNullOrEmpty(custname2.Trim()))
                {
                    spacing = string.Empty;
                    strlen = 17 - spacing.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }

                    cmds += normalFont + spacing + custname2;
                    cmds += NewLine;
                    custname2 = string.Empty;
                }


                //Last page condition
                if (linesCount > 42)
                {
                    cmds += pageBreak;
                    pageNo++;
                    linesCount = 0;
                    Print_DailySalesInvSummaryHeaderSO(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);
                }
                for (int i = 1; i < (42 - linesCount); i++)
                {
                    cmds += NewLine;
                }

                // string AmtTotal = head.Sum(x => x.NetAmount).ToString();
                decimal AmountSO = 0;
                //decimal AmountCN = 0;
                AmountSO = lines.Where(c => c.DocumentType == "SO" && c.IsVoid != "true").Sum(c => c.NetAmount);
                //AmountCN = lines.Where(c => c.DocumentType == "CN").Sum(c => c.NetAmount);
                string AmtTotal = AmountSO.ToString("N");
                string strCount = count.ToString();
                strCount = AddSpaceAtString(strCount, true, 5);
                AmtTotal = AddSpaceAtString(AmtTotal, true, 9);

                cmds += "--------------------------------------------------------------------------------";
                cmds += "Count : " + strCount; //8+5
                cmds += "    Status:(SO-Sales Order)                     Total : " + AmtTotal + "  "; //56 + 9 + 2
                cmds += "--------------------------------------------------------------------------------";

                retval = DependencyService.Get<IBluetoothPrinter>().Print(SelectedBthDevice, cmds);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        private static void Print_DailySalesInvSummaryHeaderSO(string companyName, string salesManNo, string salesManName, string pageNo, ref string cmds)
        {
            string spacing = string.Empty;
            string space = "\x20";
            int total_len = 80;
            int strlen, half_len = 37;
            string NewLine = "\x0A";
            string normalFont = "\x1b\x45\x01\x1b\x21\x10";

            //Skip lines for paper head
            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;

            // Calculate company name
            //cmds += normalFont + companyName;
            //cmds += NewLine;

            // Calculate title
            spacing = string.Empty;
            string title = "Daily Sales Invoice Summary Report";
            int companyNameLen = 30;
            strlen = (total_len - companyNameLen - 2) - title.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            cmds += normalFont + companyName + spacing + title;
            cmds += NewLine;

            // Calculate SALESMAN NO  ------------------------------- PAGE
            string idNo = "SALESMAN ID/NO : " + salesManNo;
            cmds += normalFont + idNo;

            spacing = string.Empty;
            strlen = half_len - idNo.Length;
            //strlen = strlen - head.DocumentNo.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string pageNum = "PAGE : " + pageNo;
            cmds += normalFont + spacing + pageNum;
            cmds += NewLine;

            // Calculate SALESMAN name ------------------------------- print date
            string salesMan = "SALESMAN NAME : " + salesManName;
            cmds += normalFont + salesMan;

            spacing = string.Empty;
            strlen = half_len - salesMan.Length;
            //strlen = strlen - head.SellToCustomer.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string printDate = "Date : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm tt");
            cmds += normalFont + spacing + printDate;
            cmds += NewLine;
            cmds += NewLine;

        }
        public static string Print_DailySalesInvSummaryCN(string SelectedBthDevice, ObservableCollection<SalesHeader> head, string companyName, string saleManNo, string saleManName)
        {
            string retval = string.Empty;
            try
            {
                string spacing = string.Empty;
                string space = "\x20";
                int total_len = 80;
                int half_len = 37;
                int strlen, halflen, pageNo;
                string docno, custno, custname, custname2, date, terms, status, amt;
                string NewLine = "\x0A";
                string pageBreak = "\x0C";
                string normalFont = "\x1b\x45\x01\x1b\x21\x10";
                string cmds = "";

                //Print Header
                pageNo = 1;
                Print_DailySalesInvSummaryHeaderCN(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);

                #region --------------- Print Lines ---------------

                var lines = head.Where(x => x.DocumentType == "CN").ToList();

                //cmds += NewLine;
                custname2 = string.Empty;
                int count = 0, linesCount = 1;
                foreach (SalesHeader l in lines)
                {
                    if (!string.IsNullOrEmpty(custname2.Trim()))
                    {
                        spacing = string.Empty;
                        strlen = 17 - spacing.Length;
                        for (int i = 1; i < strlen; i++)
                        {
                            spacing += space;
                        }

                        cmds += normalFont + spacing + custname2;
                        cmds += NewLine;
                        linesCount++;
                        custname2 = string.Empty;
                    }

                    // Doc No
                    docno = l.DocumentNo + space;

                    // Cust No
                    spacing = string.Empty;
                    strlen = 9 - l.SellToCustomer.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    custno = l.SellToCustomer + spacing;

                    // Cust Name
                    spacing = string.Empty;
                    //strlen = 22 - l.SellToName.Length;
                    strlen = 0;
                    custname = l.SellToName;

                    if (custname.Length > 21)
                    {
                        custname = l.SellToName.Substring(0, 21);
                        if (l.SellToName.Length > 22)
                            custname2 = l.SellToName.Substring(21, l.SellToName.Length - 21);
                    }
                    else
                    {
                        strlen = 22 - l.SellToName.Length;
                        custname = l.SellToName;
                    }


                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }

                    custname = custname + spacing;
                    //custname = l.SellToName + spacing;

                    // Date
                    date = " " + DateTime.Parse(l.DocumentDate).ToString("dd/MM/yy HH:mm");

                    // Terms
                    spacing = string.Empty;
                    if (l.PaymentMethod != null)
                    {
                        strlen = 5 - l.PaymentMethod.Length;
                    }
                    else
                    {
                        strlen = 5;
                    }
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    terms = l.PaymentMethod + spacing;

                    // Status
                    spacing = string.Empty;
                    for (int i = 1; i < 3; i++)
                    {
                        spacing += space;
                    }

                    status = l.IsVoid == "true" ? "V " + spacing : "SO" + spacing;
                    if (l.DocumentType == "CN")
                    {
                        status = "CN" + spacing;
                        if (l.IsVoid == "true")
                        {
                            status = "V " + spacing;
                        }
                    }
                    // Amt
                    //amt = string.Format("{0:0.00}", l.NetAmount);
                    //amt =  l.NetAmount.ToString("N"); ;
                    amt = l.IsVoid == "true" ? "0.00" : l.NetAmount.ToString("N");
                    //amt = AddSpaceAtString(amt, true, 9);
                    if (l.DocumentType == "CN")
                    {
                        amt = "-" + amt;
                        if (l.IsVoid == "true")
                        {
                            amt = "0.00";
                        }
                    }
                    amt = AddSpaceAtString(amt, true, 10);
                    cmds += normalFont + docno + custno + custname + date + terms + status + amt;
                    cmds += NewLine;

                    count++;
                    linesCount++;

                    // Next page
                    if (linesCount >= 45)
                    {
                        cmds += pageBreak;
                        pageNo++;
                        linesCount = 1;
                        Print_DailySalesInvSummaryHeaderCN(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);
                    }

                }
                #endregion

                if (!string.IsNullOrEmpty(custname2.Trim()))
                {
                    spacing = string.Empty;
                    strlen = 17 - spacing.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }

                    cmds += normalFont + spacing + custname2;
                    cmds += NewLine;
                    custname2 = string.Empty;
                }


                //Last page condition
                if (linesCount > 42)
                {
                    cmds += pageBreak;
                    pageNo++;
                    linesCount = 0;
                    Print_DailySalesInvSummaryHeaderCN(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);
                }
                for (int i = 1; i < (42 - linesCount); i++)
                {
                    cmds += NewLine;
                }

                // string AmtTotal = head.Sum(x => x.NetAmount).ToString();
                // decimal AmountSO = 0;
                decimal AmountCN = 0;
                // AmountSO = head.Where(c => c.DocumentType == "SO" && c.IsVoid != "true").Sum(c => c.NetAmount);
                AmountCN = lines.Where(c => c.DocumentType == "CN").Sum(c => c.NetAmount);
                string AmtTotal = AmountCN.ToString("N");
                string strCount = count.ToString();
                strCount = AddSpaceAtString(strCount, true, 5);
                AmtTotal = AddSpaceAtString(AmtTotal, true, 9);

                cmds += "--------------------------------------------------------------------------------";
                cmds += "Count : " + strCount; //8+5
                cmds += "    Status:(CN-Credit Note)                     Total : " + AmtTotal + "  "; //56 +9 + 2
                cmds += "--------------------------------------------------------------------------------";

                retval = DependencyService.Get<IBluetoothPrinter>().Print(SelectedBthDevice, cmds);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        private static void Print_DailySalesInvSummaryHeaderCN(string companyName, string salesManNo, string salesManName, string pageNo, ref string cmds)
        {
            string spacing = string.Empty;
            string space = "\x20";
            int total_len = 80;
            int strlen, half_len = 37;
            string NewLine = "\x0A";
            string normalFont = "\x1b\x45\x01\x1b\x21\x10";

            //Skip lines for paper head
            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;

            // Calculate company name
            //cmds += normalFont + companyName;
            //cmds += NewLine;

            // Calculate title
            spacing = string.Empty;
            string title = "DAILY SALES RETURN SUMMARY REPORT";
            int companyNameLen = 30;
            strlen = (total_len - companyNameLen - 2) - title.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            cmds += normalFont + companyName + spacing + title;
            cmds += NewLine;

            // Calculate SALESMAN NO  ------------------------------- PAGE
            string idNo = "SALESMAN ID/NO : " + salesManNo;
            cmds += normalFont + idNo;

            spacing = string.Empty;
            strlen = half_len - idNo.Length;
            //strlen = strlen - head.DocumentNo.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string pageNum = "PAGE : " + pageNo;
            cmds += normalFont + spacing + pageNum;
            cmds += NewLine;

            // Calculate SALESMAN name ------------------------------- print date
            string salesMan = "SALESMAN NAME : " + salesManName;
            cmds += normalFont + salesMan;

            spacing = string.Empty;
            strlen = half_len - salesMan.Length;
            //strlen = strlen - head.SellToCustomer.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string printDate = "Date : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm tt");
            cmds += normalFont + spacing + printDate;
            cmds += NewLine;
            cmds += NewLine;

        }
        public static string Print_VoidSalesSummary(string SelectedBthDevice, ObservableCollection<SalesHeader> head, string companyName, string saleManNo, string saleManName)
        {
            string retval = string.Empty;
            try
            {
                string spacing = string.Empty;
                string space = "\x20";
                int total_len = 80;
                int half_len = 37;
                int strlen, halflen, pageNo;
                string docno, custno, custname, custname2, date, terms, status, amt;
                string NewLine = "\x0A";
                string pageBreak = "\x0C";
                string normalFont = "\x1b\x45\x01\x1b\x21\x10";
                string cmds = "";

                //Print Header
                pageNo = 1;
                Print_VoidSalesHeader(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);

                #region --------------- Print Lines ---------------

                //cmds += NewLine;
                custname2 = string.Empty;
                int count = 0, linesCount = 1;
                foreach (SalesHeader l in head)
                {
                    if (!string.IsNullOrEmpty(custname2.Trim()))
                    {
                        spacing = string.Empty;
                        strlen = 17 - spacing.Length;
                        for (int i = 1; i < strlen; i++)
                        {
                            spacing += space;
                        }

                        cmds += normalFont + spacing + custname2;
                        cmds += NewLine;
                        linesCount++;
                        custname2 = string.Empty;
                    }

                    // Doc No
                    docno = l.DocumentNo + space;

                    // Cust No
                    spacing = string.Empty;
                    strlen = 9 - l.SellToCustomer.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    custno = l.SellToCustomer + spacing;

                    // Cust Name
                    spacing = string.Empty;
                    //strlen = 22 - l.SellToName.Length;
                    strlen = 0;
                    custname = l.SellToName;

                    if (custname.Length > 20)
                    {
                        custname = l.SellToName.Substring(0, 20);
                        if (l.SellToName.Length > 21)
                            custname2 = l.SellToName.Substring(20, l.SellToName.Length - 20);
                    }
                    else
                    {
                        strlen = 21 - l.SellToName.Length;
                        custname = l.SellToName;
                    }


                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }

                    custname = custname + spacing;
                    //custname = l.SellToName + spacing;

                    // Date
                    date = " " + DateTime.Parse(l.DocumentDate).ToString("dd/MM/yy HH:mm");

                    // Terms
                    spacing = string.Empty;
                    if (l.PaymentMethod != null)
                    {
                        strlen = 5 - l.PaymentMethod.Length;
                    }
                    else
                    {
                        strlen = 5;
                    }
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }
                    terms = l.PaymentMethod + spacing;

                    // Status
                    spacing = string.Empty;
                    for (int i = 1; i < 3; i++)
                    {
                        spacing += space;
                    }

                    status = l.IsVoid == "true" ? "V " + spacing : "SO" + spacing;
                    if (l.DocumentType == "CN")
                    {
                        status = "CN" + spacing;
                        if (l.IsVoid == "true")
                        {
                            status = "V " + spacing;
                        }
                    }
                    // Amt
                    ////amt = string.Format("{0:0.00}", l.NetAmount);
                    amt = l.NetAmount.ToString("N"); ;
                    // amt = l.IsVoid == "true" ? "0.00" : l.NetAmount.ToString("N");
                    ////amt = AddSpaceAtString(amt, true, 9);
                    //if (l.DocumentType == "CN")
                    //{
                    //    amt = "-" + amt;
                    //    if (l.IsVoid == "true")
                    //    {
                    //        amt = "0.00";
                    //    }
                    //}
                    amt = AddSpaceAtString(amt, true, 9);
                    cmds += normalFont + docno + custno + custname + date + terms + status + amt;
                    cmds += NewLine;

                    count++;
                    linesCount++;

                    // Next page
                    if (linesCount >= 45)
                    {
                        cmds += pageBreak;
                        pageNo++;
                        linesCount = 1;
                        Print_VoidSalesHeader(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);
                    }

                }
                #endregion

                if (!string.IsNullOrEmpty(custname2.Trim()))
                {
                    spacing = string.Empty;
                    strlen = 17 - spacing.Length;
                    for (int i = 1; i < strlen; i++)
                    {
                        spacing += space;
                    }

                    cmds += normalFont + spacing + custname2;
                    cmds += NewLine;
                    custname2 = string.Empty;
                }


                //Last page condition
                if (linesCount > 42)
                {
                    cmds += pageBreak;
                    pageNo++;
                    linesCount = 0;
                    Print_VoidSalesHeader(companyName, saleManNo, saleManName, pageNo.ToString(), ref cmds);
                }
                for (int i = 1; i < (42 - linesCount); i++)
                {
                    cmds += NewLine;
                }

                // string AmtTotal = head.Sum(x => x.NetAmount).ToString();
                decimal AmountSO = 0;
                //decimal AmountCN = 0;
                AmountSO = head.Where(c => c.DocumentType == "SO" && c.IsVoid == "true").Sum(c => c.NetAmount);
                //AmountCN = head.Where(c => c.DocumentType == "CN" && c.IsVoid != "true").Sum(c => c.NetAmount);
                string AmtTotal = AmountSO.ToString("N");
                string strCount = count.ToString();
                strCount = AddSpaceAtString(strCount, true, 5);
                AmtTotal = AddSpaceAtString(AmtTotal, true, 9);

                cmds += "--------------------------------------------------------------------------------";
                cmds += "Count : " + strCount; //8+5
                cmds += "                                           Void Total : " + AmtTotal + "  ";//56 +9 + 2
                cmds += "--------------------------------------------------------------------------------";

                retval = DependencyService.Get<IBluetoothPrinter>().Print(SelectedBthDevice, cmds);
                return retval;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        private static void Print_VoidSalesHeader(string companyName, string salesManNo, string salesManName, string pageNo, ref string cmds)
        {
            string spacing = string.Empty;
            string space = "\x20";
            int total_len = 80;
            int strlen, half_len = 37;
            string NewLine = "\x0A";
            string normalFont = "\x1b\x45\x01\x1b\x21\x10";

            //Skip lines for paper head
            cmds += NewLine;
            cmds += NewLine;
            cmds += NewLine;

            // Calculate company name
            //cmds += normalFont + companyName;
            //cmds += NewLine;

            // Calculate title
            spacing = string.Empty;
            string title = "VOID SALES ORDER REPORT";
            int companyNameLen = 30;
            strlen = (total_len - companyNameLen - 2) - title.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            cmds += normalFont + companyName + spacing + title;
            cmds += NewLine;

            // Calculate SALESMAN NO  ------------------------------- PAGE
            string idNo = "SALESMAN ID/NO : " + salesManNo;
            cmds += normalFont + idNo;

            spacing = string.Empty;
            strlen = half_len - idNo.Length;
            //strlen = strlen - head.DocumentNo.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string pageNum = "PAGE : " + pageNo;
            cmds += normalFont + spacing + pageNum;
            cmds += NewLine;

            // Calculate SALESMAN name ------------------------------- print date
            string salesMan = "SALESMAN NAME : " + salesManName;
            cmds += normalFont + salesMan;

            spacing = string.Empty;
            strlen = half_len - salesMan.Length;
            //strlen = strlen - head.SellToCustomer.Length;
            for (int i = 0; i < strlen; i++)
            {
                spacing += space;
            }
            string printDate = "Date : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm tt");
            cmds += normalFont + spacing + printDate;
            cmds += NewLine;
            cmds += NewLine;

        }

        public static List<string> WordWrap(string input, int maxCharacters)
        {
            List<string> lines = new List<string>();

            if (!input.Contains(" "))
            {
                int start = 0;
                while (start < input.Length)
                {
                    lines.Add(input.Substring(start, Math.Min(maxCharacters, input.Length - start)));
                    start += maxCharacters;
                }
            }
            else
            {
                string[] words = input.Split(' ');

                string line = "";
                foreach (string word in words)
                {
                    if ((line + word).Length > maxCharacters)
                    {
                        lines.Add(line.Trim());
                        line = "";
                    }

                    line += string.Format("{0} ", word);
                }

                if (line.Length > 0)
                {
                    lines.Add(line.Trim());
                }
            }

            return lines;
        }

        public static string TryToIncrement(string tag)
        {
            string result = null;
            Match m = Regex.Match(tag, @"^(.*?)(\d+)$");
            if (m.Success)
            {
                string head = m.Groups[1].Value;
                string tail = m.Groups[2].Value;
                string format = new string('0', tail.Length);
                int incremented = int.Parse(tail) + 1;
                result = head + incremented.ToString(format);
            }
            return result;
        }

        public static string TryToIncrementTail(string tag)
        {
            string result = null;
            Match m = Regex.Match(tag, @"^(.*?)(\d+)$");
            if (m.Success)
            {
                string head = m.Groups[1].Value;
                string tail = m.Groups[2].Value;
                string format = new string('0', tail.Length);
                int incremented = int.Parse(tail) + 1;
                result = incremented.ToString(format);
            }
            return result;
        }
    }
}