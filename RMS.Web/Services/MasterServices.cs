using RMS.Web.Models;
using RMS.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RMS.Web.Services
{
    public class MasterServices
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        VoucherMasterViewModel vmodel = new VoucherMasterViewModel();
        ADOServices treeservice = new ADOServices();
        trialbalanceviewmodel tbmodel = new trialbalanceviewmodel();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MVCDBADO"].ConnectionString);




        //------------------Account Group------------------//
        #region AccountGroup
        public void AddAccgrup(AccGroup accGroup)
        {
            db.AccGroup.Add(accGroup);
            db.SaveChanges();


        }

        public AccGroup Getaccgroupbyid(int? AccgroupID)
        {
            var oldd =  db.AccGroup.Find(AccgroupID);
            return oldd;
        }
        public void UpdateAccgroup(AccGroup accGroup)
        {
            db.Entry(accGroup).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

        }

        public void DEleteaccgroup(AccGroup accGroup)
        {
            db.AccGroup.Remove(accGroup);
            db.SaveChanges();


        }
        #endregion AccountGroup
        //------------------Accounts------------------//
        #region Account
        public void AddAccount(Account account)
        {
            db.Account.Add(account);
            db.SaveChanges();


        }

        public Account Getaccountbyid(int idd)
        {
           return db.Account.Find(idd);
        }



        public void Updateaccount(Account account)
        {
            db.Entry(account).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

        }

        public void deleteaccount(Account account)
        {
            db.Account.Remove(account);
            db.SaveChanges();


        }


        #endregion Account

        //------------------Material Centre Group------------------//
        #region MaterialCentreGroup
        public void AddMaterialCentregroup(MaterialCentreGroup materialcentreGroup)
        {
            db.MaterialCentreGroup.Add(materialcentreGroup);
            db.SaveChanges();


        }

        public MaterialCentreGroup GetmaterialcentreGroupbyid(int? materialgroupID)
        {
            var oldd = db.MaterialCentreGroup.Find(materialgroupID);
            return oldd;
        }
        public void UpdatematerialcentreGroup(MaterialCentreGroup materialcentreGroup)
        {
            db.Entry(materialcentreGroup).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

        }

        public void DEleteMaterialCentreGroup(MaterialCentreGroup materialcentreGroup)
        {
            db.MaterialCentreGroup.Remove(materialcentreGroup);
            db.SaveChanges();


        }

        #endregion MaterialCentreGroup
        //------------------Material Centre ------------------//
        #region MaterialCentre
        public void AddMaterialCentre(MaterialCentre materialcentre)
        {
            db.MaterialCentre.Add(materialcentre);
            db.SaveChanges();


        }

        public MaterialCentre Getmaterialcentrebyid(int? materialID)
        {
            var oldd = db.MaterialCentre.Find(materialID);
            return oldd;
        }
        public void Updatematerialcentre(MaterialCentre materialcentre)
        {
            db.Entry(materialcentre).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

        }

        public void DEleteMaterialCentre(MaterialCentre materialcentre)
        {
            db.MaterialCentre.Remove(materialcentre);
            db.SaveChanges();


        }
        #endregion MaterialCentre
        //------------------Currency Form------------------//
        #region CurrencyForm
        public void AddCurrency(Currency Currencydetails)
        {
            db.Currency.Add(Currencydetails);
            db.SaveChanges();


        }

        public Currency GetCurrencyid(int? currencyID)
        {
            var oldd = db.Currency.Find(currencyID);
            return oldd;
        }
        public void UpdateCurrency(Currency Currencydetails)
        {
            db.Entry(Currencydetails).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

        }

        public void DEleteCurrency(Currency Currencydetails)
        {
            db.Currency.Remove(Currencydetails);
            db.SaveChanges();


        }

        #endregion CurrencyForm

        //------------------Item Stock------------------//
        #region ItemStock
        public int FinalStock(int ItemId)
        {

            //List<VoucherChild> voucherchildlist = new List<VoucherChild>();
            vmodel.VoucherChildlist = new List<VoucherChild>();
            var totalpurchase = 0;

            var vmasterpuchase = db.VoucherMaster.Where(m => m.Partyid_Accountid != 1060  && m.DrCrType == 2 && m.Partyid_Accountid != 1059).ToList();
            for (int i = 0; i < vmasterpuchase.Count; i++)
            {
                var vmmasterid = vmasterpuchase[i].vouchermasterid;
                var childlist = db.VoucherChild.Where(m => m.VoucherMasterID == vmmasterid && m.ItemID == ItemId).ToList();

                vmodel.VoucherChildlist.AddRange(childlist);
            }
            var pquantity = 0;
            for (int i = 0; i < vmodel.VoucherChildlist.Count; i++)
            {
                pquantity = vmodel.VoucherChildlist[i].Quantity;
                totalpurchase = totalpurchase + pquantity;
            }
            ////////////////////////////
            vmodel.VoucherChildlist = new List<VoucherChild>();
            var totalsold = 0;

            var vmastersales = db.VoucherMaster.Where(m => m.Partyid_Accountid != 1060 && m.DrCrType == 1 && m.Partyid_Accountid != 1059).ToList();
            for (int i = 0; i < vmastersales.Count; i++)
            {
                var vmmasterid = vmastersales[i].vouchermasterid;
                var childlist = db.VoucherChild.Where(m => m.VoucherMasterID == vmmasterid && m.ItemID == ItemId).ToList();

                vmodel.VoucherChildlist.AddRange(childlist);
            }
            var squantity = 0;
            for (int i = 0; i < vmodel.VoucherChildlist.Count; i++)
            {
                squantity = vmodel.VoucherChildlist[i].Quantity;
                totalsold = totalsold + squantity;
            }
            var quantity = totalpurchase - totalsold;
            return quantity;
        }


        public VoucherMasterViewModel StockByLocation(int ItemId, int LocationID)
        {

            vmodel.VoucherChildlist = new List<VoucherChild>();
            var childlistin = new List<VoucherChild>();
            var childlistout = new List<VoucherChild>();

            vmodel.masterids = new List<int?>();

            var carrierexpenseaccount = db.Preferences.Where(m => m.PreferenceID == 1).FirstOrDefault().Preferencevalue;

            var stockin = db.VoucherMaster.Where(m => m.LocationID == LocationID && m.DrCrType==1 && m.Partyid_Accountid != carrierexpenseaccount && m.Partyid_Accountid != 1059 && m.Partyid_Accountid != 1060).ToList();
            var stockout = db.VoucherMaster.Where(m => m.LocationID == LocationID && m.DrCrType == 2 && m.Partyid_Accountid != carrierexpenseaccount && m.Partyid_Accountid != 1059 && m.Partyid_Accountid != 1060).ToList();

            if (stockin != null)
            {
                for (int i = 0; i < stockin.Count; i++)
                {
                    var vmmasterid = stockin[i].vouchermasterid;
                    var vchild = db.VoucherChild.Where(m => m.VoucherMasterID == vmmasterid && m.ItemID == ItemId).FirstOrDefault();

                    if (vchild != null)
                    {
                        vmodel.masterids.Add(vchild.VoucherMasterID);
                        childlistin.Add(vchild);

                    }
                    else
                    {
                        vmodel.masterids.Add(0);


                    }
                }
            }
            if (stockout != null)
            {
                for (int i = 0; i < stockout.Count; i++)
                {
                    var vmmasterid = stockout[i].vouchermasterid;
                    var vchild = db.VoucherChild.Where(m => m.VoucherMasterID == vmmasterid && m.ItemID == ItemId).FirstOrDefault();

                    if (vchild != null)
                    {
                        vmodel.masterids.Add(vchild.VoucherMasterID);
                        childlistout.Add(vchild);

                    }
                    else
                    {
                        vmodel.masterids.Add(0);


                    }
                }
            }
           
            
            var Quantityin = childlistin.Sum(m => m.Quantity);
            var Quantityout = childlistout.Sum(m => m.Quantity);
             
            vmodel.stockquantity = Quantityout - Quantityin ;



            return vmodel;
        }
        #endregion ItemStock

        //----------------------Login Services--------------------------//
        #region LoginServices

        public List<MenuItems> GetAllMenus()
        {
            return db.MenuItems.ToList();
        }

        public List<int> GetUserExistingMenus(int userId)
        {
            var menuIds = db.UsersMenu.Where(x => x.UserID == userId).Select(x => x.MenuItemID).ToList();
            return db.MenuItems.Where(x => menuIds.Contains(x.ID)).Select(x => x.ID).ToList();
        }

        public bool isExistUserPrivilege(int userId)
        {
            return db.UsersMenu.Any(x => x.UserID == userId);
        }

        public void DeleteExisitingUserPrivileges(int userId)
        {
            var privs = db.UsersMenu.Where(x => x.UserID == userId).ToList();
            if (privs.Count <= 0) return;

            foreach (var p in privs)
            {
                db.UsersMenu.Remove(p);
            }
            db.SaveChanges();
        }

        public void AddParentMenusInPrivileges(int userId, List<int?> menuIds)
        {
            var parentMenus = db.MenuItems.Where(x => x.ParentID==0).ToList();
            List<int?> menuids = new List<int?>();
            List<MenuItems> childmenus = new List<MenuItems>();

            var childMenus = db.MenuItems.Where(x => menuIds.Contains(x.ID)).ToList();



            for (int i = 0; i < childMenus.Count; i++)
            {
                var childid = childMenus[i].ParentID;
     

                if (menuids.Contains(childid) != true)
                {
                    menuids.Add(childid);

                }
            }

            childMenus = null;
            //var parentids = db.MenuItems.Where(x => childids.Contains(x.ID)).ToList();

            //var parentids = db.MenuItems.Where(m=>m.ParentID== menuIds.Contains(x.ID))
            //var finalMenuIds = (from parent in parentMenus where parentMenus.Count > 0 from c in childMenus where menuIds.Contains(c.ID) select parent.ID).ToList();

            foreach (var pmId in menuids)
            {
                db.UsersMenu.Add(new UsersMenu { MenuItemID = (int)pmId, UserID = userId });
            }
            db.SaveChanges();
        }

        public void AddUserPrivilege(UsersMenu userPrivilege)
        {
            db.UsersMenu.Add(userPrivilege);
            db.SaveChanges();
        }

        public void SaveUser(UserPrivilegeViewModel model)
        {
            AspNetUsers usermodel = new AspNetUsers();
            usermodel.UserName = model.registerViewModel.UserName;
            usermodel.Email = model.registerViewModel.Email;
            usermodel.PhoneNumber = model.registerViewModel.PhoneNumber;
            usermodel.LocationID = model.registerViewModel.locationid;

            db.Entry(usermodel).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        #endregion LoginServices

        //----------------------Report Services--------------------------//.
        #region ReportServices




        public trialbalanceviewmodel payablereport(List<Account> accounts,DateTime enddate)
        {
            trialbalanceviewmodel tbmodel = new trialbalanceviewmodel();
            var carrierexpenseaccount = db.Preferences.Where(m => m.PreferencetypeID == 1).FirstOrDefault().Preferencevalue;

            var totalcredit = db.VoucherMaster.Where(m => m.DrCrType == 2 &&m.VoucherCreateDate <= enddate && m.Partyid_Accountid != carrierexpenseaccount && m.Partyid_Accountid != 1059 && m.Partyid_Accountid != 1060).ToList();
            var totalcredit2 = db.JournalVoucherChild.Where(m=>m.DrCrID==2 && m.JvDate <= enddate).ToList();
            
            tbmodel.accountlist = new List<Account>();
            tbmodel.accountidsdebit = new List<double>();
            tbmodel.accountidscredit = new List<double>();
            tbmodel.accountbalance = new List<double>();
            tbmodel.debitbalance = new List<double>();
            tbmodel.creditbalance = new List<double>();



            //for (int i = 0; i < totaldebit.Count; i++)
            //{
            //    var acid = totaldebit[i].Partyid_Accountid;
            //    var accbalance = totaldebit.Where(m => m.Partyid_Accountid == acid).Sum(m => m.VoucherFinalTotal);
            //    if (tbmodel.accountidsdebit.Contains(acid) != true)
            //    {
            //        tbmodel.accountidsdebit.Add(acid);
            //        tbmodel.debitbalance.Add(accbalance);


            //    }
            //}


            for (int i = 0; i < accounts.Count; i++)
            {
                var acid = accounts[i].AccountID;
                var account1 = db.Account.Where(m => m.AccountID == acid).FirstOrDefault();
                account1.CreditTotal = totalcredit.Where(m => m.Partyid_Accountid == acid).Sum(m => m.VoucherFinalTotal);
                account1.CreditTotal = account1.CreditTotal + totalcredit2.Where(n => n.AccountID == acid).Sum(m => m.CreditAmount);


                if (tbmodel.accountidscredit.Contains(acid) != true)
                {
                    tbmodel.accountidscredit.Add(acid);


                }
                if (tbmodel.accountlist.Contains(account1) != true)
                {
                    tbmodel.accountlist.Add(account1);

                }


            }




            return tbmodel;
        }

        public trialbalanceviewmodel recieveablereport(List<Account> accounts, DateTime enddate)
        {
            trialbalanceviewmodel tbmodel = new trialbalanceviewmodel();

            var carrierexpenseaccount = db.Preferences.Where(m => m.PreferencetypeID == 1).FirstOrDefault().Preferencevalue;

            var totaldebit = db.VoucherMaster.Where(m => m.DrCrType == 1 && m.VoucherCreateDate <= enddate && m.Partyid_Accountid != carrierexpenseaccount && m.Partyid_Accountid != 1059 && m.Partyid_Accountid != 1060).ToList();
            var totaldebit2 = db.JournalVoucherChild.Where(m => m.DrCrID == 1 && m.JvDate <= enddate).ToList();

            tbmodel.accountlist = new List<Account>();
            tbmodel.accountidsdebit = new List<double>();
            tbmodel.accountidscredit = new List<double>();
            tbmodel.accountbalance = new List<double>();
            tbmodel.debitbalance = new List<double>();
            tbmodel.creditbalance = new List<double>();



            //for (int i = 0; i < totaldebit.Count; i++)
            //{
            //    var acid = totaldebit[i].Partyid_Accountid;
            //    var accbalance = totaldebit.Where(m => m.Partyid_Accountid == acid).Sum(m => m.VoucherFinalTotal);
            //    if (tbmodel.accountidsdebit.Contains(acid) != true)
            //    {
            //        tbmodel.accountidsdebit.Add(acid);
            //        tbmodel.debitbalance.Add(accbalance);


            //    }
            //}


            for (int i = 0; i < accounts.Count; i++)
            {
                var acid = accounts[i].AccountID;
                var account1 = db.Account.Where(m => m.AccountID == acid).FirstOrDefault();
                account1.DebitTotal = totaldebit.Where(m => m.Partyid_Accountid == acid).Sum(m => m.VoucherFinalTotal);
                account1.DebitTotal = account1.DebitTotal + totaldebit2.Where(n => n.AccountID == acid).Sum(m => m.DebitAmount);

                if (tbmodel.accountidsdebit.Contains(acid) != true)
                {
                    tbmodel.accountidsdebit.Add(acid);


                }
                if (tbmodel.accountlist.Contains(account1) != true)
                {
                    tbmodel.accountlist.Add(account1);

                }


            }




            return tbmodel;
        }

        public trialbalanceviewmodel Trialbalance(DateTime enddate, int type = 0)
        {
            if (type == 1)
            {
                tbmodel.accgrouplist = db.AccGroup.ToList();
            }
            var totaldebit = db.VoucherMaster.Where(m => m.DrCrType == 2).ToList();
            var totalcredit = db.VoucherMaster.Where(m => m.DrCrType == 1).ToList();
            tbmodel.accountlist = new List<Account>();
            tbmodel.accountidsdebit = new List<double>();
            tbmodel.accountidscredit = new List<double>();
            tbmodel.accountbalance = new List<double>();
            tbmodel.debitbalance = new List<double>();
            tbmodel.creditbalance = new List<double>();
            tbmodel.enddate2 = enddate;

            for (int i = 0; i < totaldebit.Count; i++)
            {
                var acid = totaldebit[i].Partyid_Accountid;
                var accbalance = totaldebit.Where(m => m.Partyid_Accountid == acid).Sum(m => m.VoucherFinalTotal);
                if (tbmodel.accountidsdebit.Contains(acid) != true)
                {
                    tbmodel.accountidsdebit.Add(acid);
                    tbmodel.debitbalance.Add(accbalance);


                }
            }

            for (int i = 0; i < totalcredit.Count; i++)
            {
                var acid = totalcredit[i].Partyid_Accountid;
                var accbalance = totalcredit.Where(m => m.Partyid_Accountid == acid).Sum(m => m.VoucherFinalTotal);

                if (tbmodel.accountidscredit.Contains(acid) != true)
                {
                    tbmodel.accountidscredit.Add(acid);
                    tbmodel.creditbalance.Add(accbalance);


                }
            }

            for (int i = 0; i < tbmodel.accountidscredit.Count; i++)
            {
                var accid = tbmodel.accountidscredit[i];
                if (tbmodel.accountidsdebit.Contains(accid) != true)
                {
                    tbmodel.accountidsdebit.Add(accid);

                }
                //tbmodel.accountids.Add(totaldebit[i].Partyid_Accountid);
                //var testing = db.Account.Where(m => m.AccountID == accid).FirstOrDefault();
                //if (tbmodel.accountlist.Contains(testing) != true)
                //{
                //    tbmodel.accountlist.Add(testing);

                //}


                // var debitamount = totaldebit.Where(m => m.Partyid_Accountid == accid).Sum(m => m.VoucherFinalTotal);
                // var creditamount = totalcredit.Where(m => m.Partyid_Accountid == accid).Sum(m => m.VoucherFinalTotal);
                //var totalamount = debitamount - creditamount;
                // if (totalamount > 0)
                // {
                //     tbmodel.accountbalance.Add(totalamount);
                // }
            }

            for (int i = 0; i < tbmodel.accountidsdebit.Count; i++)
            {
                var accid = tbmodel.accountidsdebit[i];
                var account1 = db.Account.Where(m => m.AccountID == accid).FirstOrDefault();
                account1.DebitTotal = totalcredit.Where(m => m.Partyid_Accountid == accid).Sum(m => m.VoucherFinalTotal);
                account1.CreditTotal = totaldebit.Where(m => m.Partyid_Accountid == accid).Sum(m => m.VoucherFinalTotal);

                if (tbmodel.accountlist.Contains(account1) != true)
                {
                    tbmodel.accountlist.Add(account1);

                }
            }


            //---PROFITREPORT-----//
            //if (type == 13)
            //{
            //    int purchaseacc = 1062;//db.Preferences.Where(m => m.PreferenceID == 2).ToList();
            //    int salesacc = 1063;//db.Preferences.Where(m => m.PreferenceID == 2).ToList();
            //    int expensedirect = 1058;//db.Preferences.Where(m => m.PreferenceID == 2).ToList();
            //    int expenseindirect = 1059;//db.Preferences.Where(m => m.PreferenceID == 2).ToList();
            //    int incomeindirect = 1061;//db.Preferences.Where(m => m.PreferenceID == 2).ToList();
            //    int incomedirect = 1060;//db.Preferences.Where(m => m.PreferenceID == 2).ToList();


            //    double? purchaseaccsum = 0;
            //    double? salesaccsum        =0;
            //    double? expensedirectsum   =0;
            //    double? expenseindirectsum =0;
            //    double? incomeindirectsum  =0;
            //    double? incomedirectsum = 0 ;


            //    var purchaseacc2 = tbmodel.accountlist.Where(m => m.Accgroupid == purchaseacc).ToList();
            //    for (int i = 0; i < purchaseacc2.Count; i++)
            //    {
            //        purchaseaccsum = purchaseacc2[i].DebitTotal - purchaseacc2[i].CreditTotal;
            //    }


            //    var salesacc2 = tbmodel.accountlist.Where(m => m.Accgroupid == salesacc).ToList();
            //    for (int i = 0; i < salesacc2.Count; i++)
            //    {
            //        salesaccsum = salesacc2[i].DebitTotal - salesacc2[i].CreditTotal;
            //    }

            //    var expensedirectsum2 = tbmodel.accountlist.Where(m => m.Accgroupid == expensedirect).ToList();
            //    for (int i = 0; i < expensedirectsum2.Count; i++)
            //    {
            //        expensedirectsum = expensedirectsum2[i].DebitTotal - expensedirectsum2[i].CreditTotal;
            //    }
            //   var expenseindirect2 = tbmodel.accountlist.Where(m => m.Accgroupid == expenseindirect).ToList();
            //    for (int i = 0; i < expenseindirect2.Count; i++)
            //    {
            //        expenseindirectsum = expenseindirect2[i].DebitTotal - expenseindirect2[i].CreditTotal;
            //    }


            //    var incomeindirectsum2 = tbmodel.accountlist.Where(m => m.Accgroupid == incomeindirect).ToList();
            //    for (int i = 0; i < incomeindirectsum2.Count; i++)
            //    {
            //        incomeindirectsum = incomeindirectsum2[i].DebitTotal - incomeindirectsum2[i].CreditTotal;
            //    }

            //    var incomedirectsum2 = tbmodel.accountlist.Where(m => m.Accgroupid == incomedirect).ToList();
            //    for (int i = 0; i < incomedirectsum2.Count; i++)
            //    {
            //        incomedirectsum = incomedirectsum2[i].DebitTotal - incomedirectsum2[i].CreditTotal;
            //    }

            //}

            //////--------store proc repport-----------------------------////////////

            //con.Open();

            //SqlCommand cmd = new SqlCommand("Sp_TrialBalance", con);
            //SqlDataAdapter sda = new SqlDataAdapter(cmd);
            //DataTable ds = new DataTable();
            //sda.Fill(ds);
            //con.Close();

            //tbmodel.accountlist = (from DataRow row in ds.Rows
            //                       select new Account
            //                       {
            //                           AccountID = row.Field<int>("childId"),
            //                           AccountName = row.Field<string>("childName"),
            //                           DebitTotal = row.Field<double>("DebitTotal"),
            //                           CreditTotal = row.Field<double>("CreditTotal")

            //                       }).ToList();
            return tbmodel;
        }

        public VoucherMasterViewModel StockLedger(int txtItems, int LocationID, DateTime startdate, DateTime enddate)
        {
            //List<VoucherChild> voucherchildlist = new List<VoucherChild>();
            vmodel.VoucherChildlist = new List<VoucherChild>();

            vmodel.masterids = new List<int?>();

            var carrierexpenseaccount = db.Preferences.Where(m => m.PreferencetypeID == 1).FirstOrDefault().Preferencevalue;
            var salesaccount = db.Preferences.Where(m => m.PreferencetypeID == 2).FirstOrDefault().Preferencevalue;
            var purchaseaccount = db.Preferences.Where(m => m.PreferencetypeID == 4).FirstOrDefault().Preferencevalue;

            vmodel.Vouchermasterlist = db.VoucherMaster.Where(m => m.LocationID == LocationID  && m.Partyid_Accountid != carrierexpenseaccount && m.Partyid_Accountid != purchaseaccount && m.Partyid_Accountid != salesaccount && m.VoucherTypeID != 16 && m.VoucherTypeID != 18 && m.VoucherTypeID != 19 && m.VoucherTypeID != 20 && m.VoucherTypeID != 26 && m.VoucherTypeID != 28).ToList();
            vmodel.Vouchermasterlist.AddRange(db.VoucherMaster.Where(m => m.LocationID2 == LocationID  && m.Partyid_Accountid != carrierexpenseaccount && m.Partyid_Accountid != purchaseaccount && m.Partyid_Accountid != salesaccount && m.VoucherTypeID != 16 && m.VoucherTypeID != 18 && m.VoucherTypeID != 19 && m.VoucherTypeID != 20 && m.VoucherTypeID != 26 && m.VoucherTypeID != 28).ToList()); 
            for (int i = 0; i < vmodel.Vouchermasterlist.Count; i++)
            {
                var vmmasterid = vmodel.Vouchermasterlist[i].vouchermasterid;
                var childlist = db.VoucherChild.Where(m => m.VoucherMasterID == vmmasterid && m.ItemID == txtItems).FirstOrDefault();

                if (childlist != null)
                {
                    vmodel.masterids.Add(childlist.VoucherMasterID);
                    vmodel.VoucherChildlist.Add(childlist);

                }
                else
                {
                    vmodel.masterids.Add(0);
                }
            }

            return vmodel;

        }

        #endregion ReportServices


        //----------------------Sale/Purchase Services--------------------------//.
        #region SaleServices

        public void DeleteSPVoucher(int  vmasterid)
        {
            var vouchers = db.VoucherMaster.Where(m => m.vouchermasterid == vmasterid).ToList();
            int vtype;
            int sreversetype = db.Preferences.Where(x=>x.PreferencetypeID==12).FirstOrDefault().Preferencevalue;
            int pvreversetype = db.Preferences.Where(x=>x.PreferencetypeID==14).FirstOrDefault().Preferencevalue;
            int srvreversetype = db.Preferences.Where(x=>x.PreferencetypeID==15).FirstOrDefault().Preferencevalue;
            int prvreversetype = db.Preferences.Where(x=>x.PreferencetypeID==16).FirstOrDefault().Preferencevalue;
            int stocktransfreversetype = db.Preferences.Where(x => x.PreferencetypeID == 25).FirstOrDefault().Preferencevalue;
            int recievefromreversetype = db.Preferences.Where(x => x.PreferencetypeID == 26).FirstOrDefault().Preferencevalue;

            for (int i = 0; i < vouchers.Count; i++)
            {
                   vtype =  vouchers[i].VoucherTypeID;

                switch (vtype)
                {
                    case 1:
                        vouchers[i].VoucherTypeID = sreversetype;
                        break;
                    case 2:
                        vouchers[i].VoucherTypeID = pvreversetype;
                        break;
                    case 9:
                        vouchers[i].VoucherTypeID = srvreversetype;
                        break;
                    case 10:
                        vouchers[i].VoucherTypeID = prvreversetype;
                        break;
                    case 12:
                        vouchers[i].VoucherTypeID = stocktransfreversetype;
                        break;
                    case 13:
                        vouchers[i].VoucherTypeID = recievefromreversetype;
                        break;
                    default:
                        break;
                }
                db.Entry(vouchers[i]).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            
        }


        #endregion SaleServices

        //----------------------Voucher Services--------------------------//.
        #region VoucherServices

        public void DeleteJVoucher(int vmasterid)
        {
            var vouchers = db.JournalVoucherMaster.Where(m => m.jvmasterid == vmasterid).ToList();
            int? vtype;
            int jvreversetype = db.Preferences.Where(x => x.PreferencetypeID == 20).FirstOrDefault().Preferencevalue;
            int pymvreversetype = db.Preferences.Where(x => x.PreferencetypeID == 23).FirstOrDefault().Preferencevalue;
            int receiptrvreversetype = db.Preferences.Where(x => x.PreferencetypeID == 24).FirstOrDefault().Preferencevalue;
            
            int contrareversetype = db.Preferences.Where(x => x.PreferencetypeID == 27).FirstOrDefault().Preferencevalue;


            for (int i = 0; i < vouchers.Count; i++)
            {
                vtype = vouchers[i].VoucherTypeID;

                switch (vtype)
                {
                    case 5:
                        vouchers[i].VoucherTypeID = jvreversetype;
                        break;
                    case 6:
                        vouchers[i].VoucherTypeID = pymvreversetype;
                        break;
                    case 8:
                        vouchers[i].VoucherTypeID = receiptrvreversetype;
                        break;
                    case 11:
                        vouchers[i].VoucherTypeID = contrareversetype;
                        break;
                    default:
                        break;
                }
                db.Entry(vouchers[i]).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }


        }


        #endregion VoucherServices


    }
}