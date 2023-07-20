using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class OrderMasterViewModel
    {
        //---OrderMaster---//
        public int OrderID { get; set; }
        public int PaymentID { get; set; }
        public int CustomerID { get; set; }
        public int SupplierID { get; set; }
        public int OrderNumber { get; set; }
        public System.DateTime OrderDate { get; set; }
        public decimal FinalTotal { get; set; }

        //---OrderChild--//

        public List<OrderChild> orderChildViewModels { get; set; }
        public List<int> ItemID { get; set; }
        public List<decimal> UnitPrice { get; set; }
        public List<decimal> Discount { get; set; }
        public List<decimal> Total { get; set; }
        public List<int> Quantity { get; set; }

        ///////////////////////////////////////////////////
        public int ItemIDlist { get; set; }
        public decimal UnitPricelist { get; set; }
        public decimal Discountlist { get; set; }
        public decimal Totallist { get; set; }
        public decimal Quantitylist { get; set; }



    }


}