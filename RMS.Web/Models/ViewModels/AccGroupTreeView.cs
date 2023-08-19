using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class AccGroupTreeView
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public int AccgroupID { get; set; }
        public float CreditAmount { get; set; }
        public float DebitAmount { get; set; }
        public int? Accunderprimarygroupid { get; set; }
        public float Accgroupbalance { get; set; }
        public string AccunderprimarygroupName { get; set; }
        public string Accgroupalias { get; set; }
        public string Accgroupname { get; set; }
        public int? MainGroupID { get; set; }
        public string MainGroupName { get; set; }
        public List<AccGroupTreeView> parent { get; set; }
        public List<AccGroupTreeView> Child { get; set; }
        public List<AccGroupTreeView> ChildChild { get; set; }

    }
}