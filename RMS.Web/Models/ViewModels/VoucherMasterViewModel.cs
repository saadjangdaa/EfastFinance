using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class VoucherMasterViewModel
    {
        //----------------Voucher Master------------------//    
        public int VoucherNumber { get; set; }
        public System.DateTime VoucherCreateDate { get; set; }
        public int Partyid { get; set; }
        public int? LocationID { get; set; }
        public int? LocationID2 { get; set; }
        public string Narration { get; set; }
        //public  int BillSundryId { get; set; }
        //public double BillSundryAmount { get; set; }
        public int BillSundryAmount2 { get; set; }
        public double FinalTotal { get; set; }
        public int VoucherType_IDddddd { get; set; }
        public int DrCrType { get; set; }

        //----------------VoucherChild------------------//

        public Nullable<int> VoucherchildID { get; set; }
        public List<int> ItemID { get; set; }
        public List<int> Quantity { get; set; }
        public List<int> Unitid { get; set; }
        public List<float> ItemPrice { get; set; }
        public List<float> TotalAmount { get; set; }
        //public List<int> VoucherchildTypeID { get; set; }
        //public int VoucherMasterID { get; set; }

        
        //---------------- Bill Sundry------------------//
        public List<double> BillSundryAmount { get; set; }
        public List<int> BillSundryID { get; set; }



        //---------------- For Editing Voucher------------------//

        public List<VoucherChild> VoucherChildlist { get; set; }
        public List<VoucherMaster> Vouchermasterlist { get; set; }
        public List<BillSundryChild> BillSundrychildlist { get; set; }
        public virtual Account Accountname { get; set; }
        public virtual BillSundryChild BillSuundrychild4 { get; set; }


        //--------Prefrences------------------//
        public int modify { get; set; }
        public int vouchertypeid { get; set; }
        public List<int> deletedchild { get; set; }
        public List<int> deletebillsundrychild { get; set; }
        public List<int> newchild2 { get; set; }
        public List<int> newbillsundrychild2 { get; set; }
        public List<int?> masterids { get; set; }

        //------------MaterialCenterids for stocktransfer----------------//
        public int FromMaterialCentreID { get; set; }
        public int ToMaterialCentreID { get; set; }
        public int stockquantity { get; set; }
    }
}