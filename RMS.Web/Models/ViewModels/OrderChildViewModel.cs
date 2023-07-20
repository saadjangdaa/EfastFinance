using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class OrderChildViewModel
    {
        public List<int> ItemID { get; set; }
        public List<decimal> UnitPrice { get; set; }
        public List<decimal> Discount { get; set; }
        public List<decimal> Total { get; set; }
        public List<int> Quantity { get; set; }


    }
}