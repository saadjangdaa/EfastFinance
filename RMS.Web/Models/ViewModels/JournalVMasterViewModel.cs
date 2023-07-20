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
}