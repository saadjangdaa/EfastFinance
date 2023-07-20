using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class ordersview
    {
        public int OrderID { get; set; }
        public string PaymentTypeName { get; set; }
        public string CustomerName { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public float FinalTotal { get; set; }


    }
}