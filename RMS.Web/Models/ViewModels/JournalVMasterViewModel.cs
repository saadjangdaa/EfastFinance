using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RMS.Web.Models.ViewModels
{
    public class JournalVMasterViewModel
    {
        //----------------Voucher Master------------------//    
        public int JvID_VoucherNo { get; set; }
        public System.DateTime VoucherDate { get; set; }
        public string LongNarration { get; set; }
        public List<int?> CurrencyID { get; set; }
        public double? DebitTotal { get; set; }
        public double? CreditTotal { get; set; }
        public string VoucherOtherDetails { get; set; }

        //----------------VoucherChild------------------//
        public List<int> jvchildid2 { get; set; }
        public List<int> jvchildid3 { get; set; }
        public int JVchildID { get; set; }
        public int JVMasterID { get; set; }
        public List<int> DrCrID { get; set; }
        public List<float> FcRate { get; set; }
        public List<int> AccountID { get; set; }
        public List<string> Accountnameee { get; set; }
        public List<double> CreditAmount { get; set; }
        public List<double> defaultvalue { get; set; }
        public List<double> DebitAmount { get; set; }
        public List<string> ShortNarration { get; set; }
        public string AccountName { get; set; }

        public List<JournalVoucherChild> Journalvoucherchildlist { get; set; }
        public List<JournalVoucherMaster> Journalvouchermasterlist { get; set; }

        //--------Prefrences------------------//
        public int vouchertype { get; set; }
        public List<int> deletedchild { get; set; }
        public List<int> newchild2debit { get; set; }
        public List<int> newchild2credit { get; set; }
        public List<int> newchild2 { get; set; }
        public int modify { get; set; }
        public string message { get; set; }

        //------------accountid for paymentvoucher----------------//
        public int paymentaccountid { get; set; }
    }
    public class JournalVMasterReportViewModel 
    {
        //----------------Voucher Master------------------//    
        public int JvID_VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string LongNarration { get; set; }
        public int CurrencyID { get; set; }
        public double DebitTotal { get; set; }
        public double CreditTotal { get; set; }
        public string VoucherOtherDetails { get; set; }

        //----------------VoucherChild------------------//
        public int jvchildid2 { get; set; }
        public int jvchildid3 { get; set; }
        public int JVchildID { get; set; }
        public int JVMasterID { get; set; }
        public int DrCrID { get; set; }
        public double FcRate { get; set; }
        public int AccountID { get; set; }
        public string Accountnameee { get; set; }
        public double CreditAmount { get; set; }
        public double defaultvalue { get; set; }
        public double DebitAmount { get; set; }
        public string ShortNarration { get; set; }
        public string AccountName { get; set; }

        //--------Prefrences------------------//
        public int vouchertype { get; set; }
        public int deletedchild { get; set; }
        public int newchild2debit { get; set; }
        public int newchild2credit { get; set; }
        public int newchild2 { get; set; }
        public int modify { get; set; }
        public string message { get; set; }

        //------------accountid for paymentvoucher----------------//
        public int paymentaccountid { get; set; }
        public string DrCrName { get; set; }
        public string CurrencyName { get; set; }
        public string VoucherTypeName { get; set; }
        public string VoucherTypeShortName { get; set; }

    }
    public class StockVoucherViewModel
    {
        public int VoucherNum_BillNum { get; set; }
        public DateTime VoucherCreateDate { get; set; }
        public string AccountName { get; set; }
        public string Narration { get; set; }
        public int vouchermasterid { get; set; }
        public int Quantity { get; set; }
        public double CarrierRates { get; set; }
        public double totalamount { get; set; }
        public string UnitName { get; set; }
        public string itemName { get; set; }
        public double VoucherFinalTotal { get; set; }
        public int VoucherTypeID { get; set; }
        public string VoucherTypeName { get; set; }
        public string VoucherTypeShortName { get; set; }
        public string From_Location { get; set; }
        public string To_Location { get; set; }
    }

}