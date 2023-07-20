using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class AccountGroupTreeViewModel
    {
        //--------------AccountGroup------------//
        public int AccgroupID { get; set; }
        public string Accgroupname { get; set; }
        public string Accgroupalias { get; set; }
        public string AccUnderGroup { get; set; }
        public string AccunderprimarygroupName { get; set; }
        public Nullable<double> Accgroupbalance { get; set; }
        public Nullable<int> Accunderprimarygroupid { get; set; }
        public Nullable<double> DebitAmount { get; set; }
        public Nullable<double> CreditAmount { get; set; }

        public List<AccGroup> Accgrouplist { get; set; }

        public List<AccGroup> Accparentgrouplist { get; set; }
        public List<AccGroup> Acclistconsolidated { get; set; }
        //--------------Accounts------------//




        public string parentgroup { get; set; }
        public List<AccGroup> parentgroup2 { get; set; }
        public List<AccGroup> parentgroup3 { get; set; }
        public List<AccGroup> parentgroup33 { get; set; }
        public string parentgroup4 { get; set; }
        public string parentgroup5 { get; set; }
        public string parentgroup6 { get; set; }
        public string parentgroup7 { get; set; }
        public string parentgroup8 { get; set; }






        //public int AccountID { get; set; }
        //public string AccountName { get; set; }
        //public string Alias { get; set; }
        //public string Print_Name { get; set; }
        //public int LedgerTypeid { get; set; }
        //public Nullable<int> SubledgeraccountID { get; set; }
        //public int Accgroupid { get; set; }
        //public int CurrencyID { get; set; }
        //public long Opening_Balance { get; set; }
        //public string Address { get; set; }
        //public int CountryID { get; set; }
        //public Nullable<long> CNIC { get; set; }
        //public Nullable<int> Userid { get; set; }
        //public Nullable<long> mobileNum { get; set; }
        //public string email { get; set; }
        //public string BeneficiaryName { get; set; }
        //public Nullable<int> SubledgerGroupID { get; set; }
        //public string SubledgerGroupName { get; set; }
        //public Nullable<double> ClosingBalance { get; set; }
        //public Nullable<double> CreditTotal { get; set; }
        //public Nullable<double> DebitTotal { get; set; }


        public Nullable<int> childId { get; set; }
        public Nullable<int> parentId { get; set; }
        public string childName { get; set; }
        public Nullable<int> level { get; set; }
        public string orderSequence { get; set; }


    }
}