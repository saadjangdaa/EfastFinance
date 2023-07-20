using RMS.Web.Models;
using RMS.Web.Models.ViewModels;
using RMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMS.Web.Controllers
{
    public class ClosingController : Controller
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        // GET: Closing
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult DayClosing(JournalVMasterViewModel JVmastermodel)
        {

            var date = "10/01/2020";
            var closingaccountcounts = db.JournalVoucherChild.Where(x => x.JvDate == Convert.ToDateTime(date) && x.AccountID != 0).ToList();

            //List<int> accountslists = db.Account.Where(m=>m.AccountID == closingaccountcoun);




            for (int i = 0; i < closingaccountcounts.Count; i++)
            {
                   
                MasterServices ms = new MasterServices();

                int accountid = JVmastermodel.AccountID[i];
                if (JVmastermodel.DrCrID[i] == 1)
                {
                    var findaccountDr = ms.Getaccountbyid(accountid);
                    var oldbalanceDr = db.Account.Where(x => x.AccountID == accountid).FirstOrDefault().ClosingBalance;
                    var olddebit = db.Account.Where(x => x.AccountID == accountid).FirstOrDefault().DebitTotal;

                    //var findaccgroupDr = ms.Getaccgroupbyid(findaccountDr.Accgroupid);
                    //var oldgroupbalanceDr = db.AccGroup.Where(m => m.AccgroupID == findaccgroupDr.AccgroupID).FirstOrDefault();


                    findaccountDr.ClosingBalance = oldbalanceDr + (long)JVmastermodel.DebitAmount[i];
                    findaccountDr.DebitTotal = olddebit + (long)JVmastermodel.DebitAmount[i];
                    //   ms.Updateaccount(findaccountDr);


                    //---------------------Update Account group debit----------------------------//
                    var findaccgroup = ms.Getaccgroupbyid(findaccountDr.Accgroupid);

                    var oldaccgrupdebit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().DebitAmount;
                    var oldaccgrupbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().Accgroupbalance;

                    if (findaccgroup.Accunderprimarygroupid != 0 && findaccgroup.Accunderprimarygroupid != null)
                    {
                        findaccgroup.Accgroupbalance = oldaccgrupbalance + (long)JVmastermodel.DebitAmount[i];
                        findaccgroup.DebitAmount = oldaccgrupdebit1 + (long)JVmastermodel.DebitAmount[i];

                    }
                    //     ms.UpdateAccgroup(findaccgroup);

                    //---------------------Update Account primary group credit----------------------------//

                    var findaccprimgroupDr = ms.Getaccgroupbyid(findaccgroup.Accunderprimarygroupid);
                    var oldaccprimgrupdebit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().DebitAmount;
                    var oldaccprimbalance = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().Accgroupbalance;

                    if (findaccprimgroupDr.Accunderprimarygroupid != 0 && findaccprimgroupDr.Accunderprimarygroupid != null)
                    {
                        findaccprimgroupDr.Accgroupbalance = oldaccprimbalance + (long)JVmastermodel.DebitAmount[i];
                        findaccprimgroupDr.DebitAmount = oldaccprimgrupdebit2 + (long)JVmastermodel.DebitAmount[i];
                        //        ms.UpdateAccgroup(findaccprimgroupDr);

                    }
                    else
                    {
                        findaccprimgroupDr.Accgroupbalance = oldaccprimbalance + (long)JVmastermodel.DebitAmount[i];
                        findaccprimgroupDr.DebitAmount = oldaccprimgrupdebit2 + (long)JVmastermodel.DebitAmount[i];
                        //     ms.UpdateAccgroup(findaccprimgroupDr);

                    }





                }
                else if (JVmastermodel.DrCrID[i] == 2)
                {

                    //---------------------Update Account  credit----------------------------//

                    var findaccountCr = ms.Getaccountbyid(accountid);
                    var oldbalanceCr = db.Account.Where(x => x.AccountID == accountid).FirstOrDefault().ClosingBalance;
                    var oldcredit = db.Account.Where(x => x.AccountID == accountid).FirstOrDefault().CreditTotal;


                    findaccountCr.ClosingBalance = oldbalanceCr - (long)JVmastermodel.CreditAmount[i];
                    findaccountCr.CreditTotal = oldcredit + (long)JVmastermodel.CreditAmount[i];
                    //    ms.Updateaccount(findaccountCr);



                    //---------------------Update Account  group credit----------------------------//

                    var findaccgroupCrr = ms.Getaccgroupbyid(findaccountCr.Accgroupid);
                    var oldaccgrupcredit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().CreditAmount;
                    var oldbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().Accgroupbalance;
                    if (findaccgroupCrr.Accunderprimarygroupid != 0 && findaccgroupCrr.Accunderprimarygroupid != null)
                    {
                        findaccgroupCrr.Accgroupbalance = oldbalance - (long)JVmastermodel.CreditAmount[i];
                        findaccgroupCrr.CreditAmount = oldaccgrupcredit1 + (long)JVmastermodel.CreditAmount[i];
                        //       ms.UpdateAccgroup(findaccgroupCrr);

                    }



                    //---------------------Update Account group 2 credit----------------------------//

                    var findaccprimgroupCr = ms.Getaccgroupbyid(findaccgroupCrr.Accunderprimarygroupid);
                    var oldaccprimgrupcredit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr.AccgroupID).FirstOrDefault().CreditAmount;
                    var oldaccprimgrupbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr.AccgroupID).FirstOrDefault().Accgroupbalance;
                    if (findaccprimgroupCr.Accunderprimarygroupid != 0 && findaccprimgroupCr.Accunderprimarygroupid != null)
                    {
                        findaccprimgroupCr.Accgroupbalance = oldaccprimgrupbalance2 - (long)JVmastermodel.CreditAmount[i];
                        findaccprimgroupCr.CreditAmount = oldaccprimgrupcredit2 + (long)JVmastermodel.CreditAmount[i];
                        //        ms.UpdateAccgroup(findaccprimgroupCr);

                    }
                    else
                    {
                        findaccprimgroupCr.Accgroupbalance = oldaccprimgrupbalance2 - (long)JVmastermodel.CreditAmount[i];
                        findaccprimgroupCr.CreditAmount = oldaccprimgrupcredit2 + (long)JVmastermodel.CreditAmount[i];
                        //        ms.UpdateAccgroup(findaccprimgroupCr);

                    }




                    //---------------------Update Account primary group  credit----------------------------//

                    if (findaccprimgroupCr.Accunderprimarygroupid != 0 && findaccprimgroupCr.Accunderprimarygroupid != null)
                    {

                        var findaccprimgroupCr3 = ms.Getaccgroupbyid(findaccprimgroupCr.Accunderprimarygroupid);
                        var oldaccprimgrupcredit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr3.AccgroupID).FirstOrDefault().CreditAmount;
                        var oldaccprimgrupbalance3 = findaccprimgroupCr3.Accgroupbalance;
                        if (findaccprimgroupCr3.Accunderprimarygroupid != 0 && findaccprimgroupCr3.Accunderprimarygroupid != null)
                        {
                            findaccprimgroupCr3.Accgroupbalance = oldaccprimgrupbalance3 - (long)JVmastermodel.CreditAmount[i];
                            findaccprimgroupCr3.CreditAmount = oldaccprimgrupcredit3 + (long)JVmastermodel.CreditAmount[i];
                            //     ms.UpdateAccgroup(findaccprimgroupCr3);
                        }
                        else
                        {
                            findaccprimgroupCr3.Accgroupbalance = oldaccprimgrupbalance3 - (long)JVmastermodel.CreditAmount[i];
                            findaccprimgroupCr3.CreditAmount = oldaccprimgrupcredit3 + (long)JVmastermodel.CreditAmount[i];
                            // ms.UpdateAccgroup(findaccprimgroupCr3);
                        }

                    }
                }
            }





            return View();

        }

    }
}