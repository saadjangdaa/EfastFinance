//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RMS.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VoucherMaster
    {
        public int VoucherNum_BillNum { get; set; }
        public System.DateTime VoucherCreateDate { get; set; }
        public int Partyid_Accountid { get; set; }
        public string Narration { get; set; }
        public int BillSundryId { get; set; }
        public double BillSundryAmount { get; set; }
        public double VoucherFinalTotal { get; set; }
        public int VoucherTypeID { get; set; }
        public int DrCrType { get; set; }
        public Nullable<int> vouchermasterid { get; set; }
        public Nullable<int> LocationID { get; set; }
        public Nullable<int> LocationID2 { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual DrCr DrCr { get; set; }
        public virtual VoucherType VoucherType { get; set; }
        public virtual MaterialCentre MaterialCentre { get; set; }
        public virtual MaterialCentre MaterialCentre1 { get; set; }
    }
}
