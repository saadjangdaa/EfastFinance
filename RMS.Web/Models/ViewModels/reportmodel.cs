using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class reportmodel
    {

        //public string Url { get; set; }
        //public string Title { get; set; }
        //public short Height { get; set; }
        //public int DialogWidth { get { return Width + 60; } }
        public short Width { get; set; }
        /////----------------Report View Model----------------------////
         
            public int AccountID { get; set; }
            public DateTime startdate { get; set; }
            public DateTime enddate { get; set; }

        public int LocationID { get; set; }
        public int ItemID { get; set; }
        public int vouchertypeid { get; set; }




    }
}