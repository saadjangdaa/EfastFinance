using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class trialbalanceviewmodel
    {
        // childId, parentId, childName, [level], [orderSequence],DebitTotal,CreditTotal,ClosingBalance
        public List<int> childId { get; set; }
        public int parentId { get; set; }
        public string childName { get; set; }
        public int level { get; set; }
        public string orderSequence { get; set; }
        public float DebitTotal { get; set; }
        public float CreditTotal { get; set; }
        public float ClosingBalance { get; set; }
        public DateTime enddate2 { get; set; }
        public string enddate3 { get; set; }
        public string startdate2 { get; set; }
        public Account Account { get; set; }



        public List<AccGroup> accgrouplist { get; set; } 
        public List<Account> accountlist { get; set; } 
        public List<double> accountidsdebit { get; set; }
        public List<double> accountidscredit { get; set; }
        public List<double> accountbalance { get; set; }
        public List<double> debitbalance { get; set; }
        public List<double> creditbalance { get; set; }





        //------------Prefrefences----------------//
        public int reporttypeid { get; set; }


    }
}