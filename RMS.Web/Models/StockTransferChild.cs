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
    
    public partial class StockTransferChild
    {
        public int StockTransferChildID { get; set; }
        public int StockTransferMasterID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public int UnitID { get; set; }
        public double ItemPrice { get; set; }
        public double TotalAmount { get; set; }
        public Nullable<int> BillSundryID { get; set; }
        public Nullable<double> BillSundryAmount { get; set; }
        public int StockTransferTypeID { get; set; }
    }
}
