using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class VoucherChildViewModel
    {
        public VoucherChildViewModel()
        {
            VoucherChildList = new List<VoucherChildViewModel>();
        }
        public int VoucherchildID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public int Unitid { get; set; }
        public double ItemPrice { get; set; }
        public double TotalAmount { get; set; }
        public int BillsundryID { get; set; }
        public int VoucherTypeID { get; set; }
        public int? VoucherMasterID { get; set; }
        public string UnitName { get; set; }
        public string ItemName { get; set; }
        public VoucherMasterViewModel VoucherMasterList { get; set; }
        public List<VoucherChildViewModel> VoucherChildList { get; set; }
    }
}