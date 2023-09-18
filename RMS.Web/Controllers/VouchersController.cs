using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Shared;
using RMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RMS.Web.Models.ViewModels;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using RMS.Web.Services;

namespace RMS.Web.Controllers
{
    [Authorize]
    public class VouchersController : Controller
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        MasterServices ms = new MasterServices();


        // GET: Vouchers 
        public JsonResult getcurrency(int cid)
        {
            var currencyshortname = db.Currency.Where(m => m.CurrencyID == cid).FirstOrDefault().CurrencyShortName;
            return Json(currencyshortname, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSVoucher(int id)
        {
            try
            {
                if (id != 0)
                {
                    ms.DeleteSPVoucher(id);


                    return Json("Reverse Successfully", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("failed", JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception e)
            {
                var err = e.ToString();
                return Json(err, JsonRequestBehavior.AllowGet);
                throw;
            }

        }

        public JsonResult DeleteJVoucher(int id)
        {
            try
            {

                if (id != 0)
                {
                    ms.DeleteJVoucher(id);


                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("fail", JsonRequestBehavior.AllowGet);

                }


            }
            catch (Exception e)
            {
                var err = e.ToString();
                return Json(err, JsonRequestBehavior.AllowGet);
                throw;
            }
        }
        //-------------------------Journal Voucher--------------------------//
        #region JournalVoucher
        public ActionResult JournalVoucher()
        {
            try
            {


                ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
                ViewBag.currency = new SelectList(db.Currency, "CurrencyID", "CurrencyShortName");
                ViewBag.vouchernumber = db.JournalVoucherMaster.Count() + 1;
                ViewBag.DrCrID = new SelectList(db.DrCr, "DrCrID", "DrCrShortName");

                JournalVMasterViewModel viewmodel = new JournalVMasterViewModel();
                viewmodel.vouchertype = 5;
                viewmodel.modify = 0;



                return View(viewmodel);
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("/Views/Shared/Error.cshtml");
                throw;
            }
        }
        public ActionResult JournalVoucherHistory()
        {

            try
            {


                var vouchercount = db.JournalVoucherMaster.Where(x => x.VoucherTypeID == 5).Count();
                List<JournalVoucherMaster> jvlist;
                JournalVMasterViewModel jvmodel = new JournalVMasterViewModel();

                if (vouchercount > 0)
                {

                    jvlist = db.JournalVoucherMaster.Where(m => m.VoucherTypeID == 5).ToList();
                    //var jvchildlist = db.JournalVoucherChild.ToList();


                    jvmodel.Journalvouchermasterlist = jvlist;
                    //Journalvoucherchildlist = jvchildlist, 
                    jvmodel.vouchertype = 5;


                }
                else
                {
                    jvmodel.vouchertype = 5;

                }

                //int accountid = 2043;
                //float creditamount = 1588;

                //            //---------------------Update Account  credit----------------------------//

                //            var findaccountCr = ms.Getaccountbyid(accountid);
                //            //---------------------Update Account  group credit----------------------------//

                //            var findaccgroupCrr = ms.Getaccgroupbyid(findaccountCr.Accgroupid);
                //            var oldaccgrupcredit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().CreditAmount;
                //            var oldbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().Accgroupbalance;



                return View(jvmodel);
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("/Views/Shared/Error.cshtml");
                throw;
            }
        }
        public ActionResult ModifyJV(int id)
        {
            var jvamsterfind = db.JournalVoucherMaster.Where(m => m.jvmasterid == id).FirstOrDefault();
            var jvchildfind = db.JournalVoucherChild.Where(c => c.JVMasterID == id).ToList();

            if (jvamsterfind.VoucherTypeID == 5)
            {
                ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
                ViewBag.currency = new SelectList(db.Currency, "CurrencyID", "CurrencyShortName");

            }
            else if (jvamsterfind.VoucherTypeID == 11)
            {
                var cashinhand = db.AccGroup.Where(m => m.AccgroupID == 1072).FirstOrDefault();
                var bankacc = db.AccGroup.Where(m => m.AccgroupID == 1073).FirstOrDefault();
                ViewBag.currency = new SelectList(db.Currency, "CurrencyID", "CurrencyShortName");


                List<Account> cashacclist = db.Account.Where(a => a.Accgroupid == cashinhand.AccgroupID).ToList();
                List<Account> bankacclist = db.Account.Where(a => a.Accgroupid == bankacc.AccgroupID).ToList();

                cashacclist.AddRange(bankacclist);
                ViewBag.paymentmode = new SelectList(cashacclist, "AccountID", "AccountName");
            }
            //ViewBag.currency = new SelectList(db.Currency, "CurrencyID", "CurrencyName");
            ViewBag.DrCrID = new SelectList(db.DrCr, "DrCrID", "DrCrShortName");


            JournalVMasterViewModel jvviewmodel = new JournalVMasterViewModel
            {
                JvID_VoucherNo = (int)jvamsterfind.jvmasterid,
                VoucherDate = jvamsterfind.VoucherDate,
                LongNarration = jvamsterfind.LongNarration,
                //CurrencyID = jvamsterfind.CurrencyID,
                DebitTotal = jvamsterfind.DebitTotal,
                CreditTotal = jvamsterfind.CreditTotal,
                VoucherOtherDetails = jvamsterfind.VoucherOtherDetails,
                Journalvoucherchildlist = jvchildfind,
                vouchertype = (int)jvamsterfind.VoucherTypeID,
                modify = 1
            };

            return View("JournalVoucher", jvviewmodel);
        }
        [HttpPost]
        public ActionResult SavejvVoucher(JournalVMasterViewModel JVmastermodel)
        {

            if (JVmastermodel.modify == 0)
            {

                var jvouchermasterid = db.JournalVoucherMaster.Count() + 1;

                for (int i = 0; i < JVmastermodel.AccountID.Count; i++)
                {
                    double creditamount = 0.00;
                    double debitamount = 0.00;

                    if (JVmastermodel.DrCrID[i] == 1)
                    {
                        debitamount = JVmastermodel.DebitAmount[i];
                        creditamount = 0.00;
                    }
                    else if (JVmastermodel.DrCrID[i] == 2)
                    {
                        creditamount = JVmastermodel.CreditAmount[i];
                        debitamount = 0.00;
                    }

                    JournalVoucherChild jvchild = new JournalVoucherChild
                    {

                        JVMasterID = jvouchermasterid,
                        DrCrID = JVmastermodel.DrCrID[i],
                        AccountID = JVmastermodel.AccountID[i],
                        CreditAmount = creditamount,
                        DebitAmount = debitamount,
                        FcRate = JVmastermodel.FcRate[i],
                        CurrencyID = JVmastermodel.CurrencyID[i],
                        ShortNarration = JVmastermodel.ShortNarration[i]


                    };
                    db.JournalVoucherChild.Add(jvchild);
                    db.SaveChanges();
                }
                if (JVmastermodel.vouchertype == 5)
                {
                    JournalVoucherMaster JVouchermaster = new JournalVoucherMaster
                    {

                        VoucherDate = JVmastermodel.VoucherDate,
                        LongNarration = JVmastermodel.LongNarration,
                        CurrencyID = 1,
                        DebitTotal = (double)JVmastermodel.DebitTotal,
                        CreditTotal = (double)JVmastermodel.CreditTotal,
                        VoucherOtherDetails = JVmastermodel.VoucherOtherDetails,
                        VoucherTypeID = 5,
                        jvmasterid = jvouchermasterid
                    };
                    db.JournalVoucherMaster.Add(JVouchermaster);
                    db.SaveChanges();
                }
                else if (JVmastermodel.vouchertype == 11)
                {
                    JournalVoucherMaster JVouchermaster = new JournalVoucherMaster
                    {

                        VoucherDate = JVmastermodel.VoucherDate,
                        LongNarration = JVmastermodel.LongNarration,
                        CurrencyID = 1,
                        DebitTotal = (double)JVmastermodel.DebitTotal,
                        CreditTotal = (double)JVmastermodel.CreditTotal,
                        VoucherOtherDetails = JVmastermodel.VoucherOtherDetails,
                        VoucherTypeID = 11,
                        jvmasterid = jvouchermasterid
                    };
                    db.JournalVoucherMaster.Add(JVouchermaster);
                    db.SaveChanges();
                }



                for (int i = 0; i < JVmastermodel.AccountID.Count; i++)
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
                        ms.Updateaccount(findaccountDr);


                        //---------------------Update Account group 1 debit----------------------------//
                        var findaccgroup = ms.Getaccgroupbyid(findaccountDr.Accgroupid);
                        var oldaccgrupdebit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().DebitAmount;
                        var oldaccgrupbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().Accgroupbalance;

                        if (findaccgroup.Accunderprimarygroupid != 0 && findaccgroup.Accunderprimarygroupid != null)
                        {
                            findaccgroup.Accgroupbalance = oldaccgrupbalance + (long)JVmastermodel.DebitAmount[i];
                            findaccgroup.DebitAmount = oldaccgrupdebit1 + (long)JVmastermodel.DebitAmount[i];
                            ms.UpdateAccgroup(findaccgroup);

                            //---------------------Update Account primary group 2 credit----------------------------//

                            var findaccprimgroupDr = ms.Getaccgroupbyid(findaccgroup.Accunderprimarygroupid);
                            var oldaccprimgrupdebit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().DebitAmount;
                            var oldaccprimbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().Accgroupbalance;

                            if (findaccprimgroupDr.Accunderprimarygroupid != 0 && findaccprimgroupDr.Accunderprimarygroupid != null)
                            {
                                findaccprimgroupDr.Accgroupbalance = oldaccprimbalance2 + (long)JVmastermodel.DebitAmount[i];
                                findaccprimgroupDr.DebitAmount = oldaccprimgrupdebit2 + (long)JVmastermodel.DebitAmount[i];
                                ms.UpdateAccgroup(findaccprimgroupDr);


                                //---------------------Update Account group 3 debit----------------------------//

                                var findaccprimgroupDr3 = ms.Getaccgroupbyid(findaccprimgroupDr.Accunderprimarygroupid);
                                var oldaccprimgrupdebit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr3.AccgroupID).FirstOrDefault().DebitAmount;
                                var oldaccprimbalance3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr3.AccgroupID).FirstOrDefault().Accgroupbalance;

                                if (findaccprimgroupDr3.Accunderprimarygroupid != 0 && findaccprimgroupDr3.Accunderprimarygroupid != null)
                                {
                                    findaccprimgroupDr3.Accgroupbalance = oldaccprimbalance3 + (long)JVmastermodel.DebitAmount[i];
                                    findaccprimgroupDr3.DebitAmount = oldaccprimgrupdebit3 + (long)JVmastermodel.DebitAmount[i];
                                    ms.UpdateAccgroup(findaccprimgroupDr3);


                                    //---------------------Update Account group 4 debit----------------------------//
                                    var findaccprimgroupDr4 = ms.Getaccgroupbyid(findaccprimgroupDr3.Accunderprimarygroupid);
                                    var oldaccprimgrupdebit4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr4.AccgroupID).FirstOrDefault().DebitAmount;
                                    var oldaccprimbalance4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr4.AccgroupID).FirstOrDefault().Accgroupbalance;

                                    if (findaccprimgroupDr4.Accunderprimarygroupid != 0 && findaccprimgroupDr4.Accunderprimarygroupid != null)
                                    {
                                        findaccprimgroupDr4.Accgroupbalance = oldaccprimbalance4 + (long)JVmastermodel.DebitAmount[i];
                                        findaccprimgroupDr4.DebitAmount = oldaccprimgrupdebit4 + (long)JVmastermodel.DebitAmount[i];
                                        ms.UpdateAccgroup(findaccprimgroupDr4);

                                        //---------------------Update Account group 5 debit----------------------------//
                                        var findaccprimgroupDr5 = ms.Getaccgroupbyid(findaccprimgroupDr4.Accunderprimarygroupid);
                                        var oldaccprimgrupdebit5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr5.AccgroupID).FirstOrDefault().DebitAmount;
                                        var oldaccprimbalance5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr5.AccgroupID).FirstOrDefault().Accgroupbalance;
                                        if (findaccprimgroupDr5.Accunderprimarygroupid != 0 && findaccprimgroupDr5.Accunderprimarygroupid != null)
                                        {
                                            findaccprimgroupDr5.Accgroupbalance = oldaccprimbalance5 + (long)JVmastermodel.DebitAmount[i];
                                            findaccprimgroupDr5.DebitAmount = oldaccprimgrupdebit5 + (long)JVmastermodel.DebitAmount[i];
                                            ms.UpdateAccgroup(findaccprimgroupDr5);


                                            //---------------------Update Account group 6 debit----------------------------//
                                            var findaccprimgroupDr6 = ms.Getaccgroupbyid(findaccprimgroupDr5.Accunderprimarygroupid);
                                            var oldaccprimgrupdebit6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr6.AccgroupID).FirstOrDefault().DebitAmount;
                                            var oldaccprimbalance6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr6.AccgroupID).FirstOrDefault().Accgroupbalance;
                                            if (findaccprimgroupDr6.Accunderprimarygroupid != 0 && findaccprimgroupDr6.Accunderprimarygroupid != null)
                                            {
                                                findaccprimgroupDr6.Accgroupbalance = oldaccprimbalance6 + (long)JVmastermodel.DebitAmount[i];
                                                findaccprimgroupDr6.DebitAmount = oldaccprimgrupdebit6 + (long)JVmastermodel.DebitAmount[i];
                                                ms.UpdateAccgroup(findaccprimgroupDr6);

                                                //---------------------Update Account group 7 debit----------------------------//
                                                var findaccprimgroupDr7 = ms.Getaccgroupbyid(findaccprimgroupDr6.Accunderprimarygroupid);
                                                var oldaccprimgrupdebit7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr7.AccgroupID).FirstOrDefault().DebitAmount;
                                                var oldaccprimbalance7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr7.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                if (findaccprimgroupDr7.Accunderprimarygroupid != 0 && findaccprimgroupDr7.Accunderprimarygroupid != null)
                                                {
                                                    findaccprimgroupDr7.Accgroupbalance = oldaccprimbalance7 + (long)JVmastermodel.DebitAmount[i];
                                                    findaccprimgroupDr7.DebitAmount = oldaccprimgrupdebit7 + (long)JVmastermodel.DebitAmount[i];
                                                    ms.UpdateAccgroup(findaccprimgroupDr7);


                                                    //---------------------Update Account group 8 debit----------------------------//
                                                    var findaccprimgroupDr8 = ms.Getaccgroupbyid(findaccprimgroupDr7.Accunderprimarygroupid);
                                                    var oldaccprimgrupdebit8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr8.AccgroupID).FirstOrDefault().DebitAmount;
                                                    var oldaccprimbalance8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr8.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                    if (findaccprimgroupDr8.Accunderprimarygroupid != 0 && findaccprimgroupDr8.Accunderprimarygroupid != null)
                                                    {
                                                        findaccprimgroupDr8.Accgroupbalance = oldaccprimbalance8 + (long)JVmastermodel.DebitAmount[i];
                                                        findaccprimgroupDr8.DebitAmount = oldaccprimgrupdebit8 + (long)JVmastermodel.DebitAmount[i];
                                                        ms.UpdateAccgroup(findaccprimgroupDr8);


                                                        //---------------------Update Account group 9 debit----------------------------//
                                                        var findaccprimgroupDr9 = ms.Getaccgroupbyid(findaccprimgroupDr8.Accunderprimarygroupid);
                                                        var oldaccprimgrupdebit9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr9.AccgroupID).FirstOrDefault().DebitAmount;
                                                        var oldaccprimbalance9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr9.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                        if (findaccprimgroupDr9.Accunderprimarygroupid != 0 && findaccprimgroupDr9.Accunderprimarygroupid != null)
                                                        {
                                                            findaccprimgroupDr9.Accgroupbalance = oldaccprimbalance9 + (long)JVmastermodel.DebitAmount[i];
                                                            findaccprimgroupDr9.DebitAmount = oldaccprimgrupdebit9 + (long)JVmastermodel.DebitAmount[i];
                                                            ms.UpdateAccgroup(findaccprimgroupDr9);

                                                            //---------------------Update Account group 10 debit----------------------------//
                                                            var findaccprimgroupDr10 = ms.Getaccgroupbyid(findaccprimgroupDr9.Accunderprimarygroupid);
                                                            var oldaccprimgrupdebit10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr10.AccgroupID).FirstOrDefault().DebitAmount;
                                                            var oldaccprimbalance10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr10.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                            if (findaccprimgroupDr10.Accunderprimarygroupid != 0 && findaccprimgroupDr10.Accunderprimarygroupid != null)
                                                            {
                                                                findaccprimgroupDr10.Accgroupbalance = oldaccprimbalance10 + (long)JVmastermodel.DebitAmount[i];
                                                                findaccprimgroupDr10.DebitAmount = oldaccprimgrupdebit10 + (long)JVmastermodel.DebitAmount[i];
                                                                ms.UpdateAccgroup(findaccprimgroupDr10);




                                                            }
                                                            else
                                                            {
                                                                findaccprimgroupDr10.Accgroupbalance = oldaccprimbalance10 + (long)JVmastermodel.DebitAmount[i];
                                                                findaccprimgroupDr10.DebitAmount = oldaccprimgrupdebit10 + (long)JVmastermodel.DebitAmount[i];
                                                                ms.UpdateAccgroup(findaccprimgroupDr10);

                                                            }


                                                        }
                                                        else
                                                        {
                                                            findaccprimgroupDr9.Accgroupbalance = oldaccprimbalance9 + (long)JVmastermodel.DebitAmount[i];
                                                            findaccprimgroupDr9.DebitAmount = oldaccprimgrupdebit9 + (long)JVmastermodel.DebitAmount[i];
                                                            ms.UpdateAccgroup(findaccprimgroupDr9);

                                                        }


                                                    }
                                                    else
                                                    {
                                                        findaccprimgroupDr8.Accgroupbalance = oldaccprimbalance8 + (long)JVmastermodel.DebitAmount[i];
                                                        findaccprimgroupDr8.DebitAmount = oldaccprimgrupdebit8 + (long)JVmastermodel.DebitAmount[i];
                                                        ms.UpdateAccgroup(findaccprimgroupDr8);

                                                    }


                                                }
                                                else
                                                {
                                                    findaccprimgroupDr7.Accgroupbalance = oldaccprimbalance7 + (long)JVmastermodel.DebitAmount[i];
                                                    findaccprimgroupDr7.DebitAmount = oldaccprimgrupdebit7 + (long)JVmastermodel.DebitAmount[i];
                                                    ms.UpdateAccgroup(findaccprimgroupDr7);

                                                }



                                            }
                                            else
                                            {
                                                findaccprimgroupDr6.Accgroupbalance = oldaccprimbalance6 + (long)JVmastermodel.DebitAmount[i];
                                                findaccprimgroupDr6.DebitAmount = oldaccprimgrupdebit6 + (long)JVmastermodel.DebitAmount[i];
                                                ms.UpdateAccgroup(findaccprimgroupDr6);

                                            }

                                        }
                                        else
                                        {
                                            findaccprimgroupDr5.Accgroupbalance = oldaccprimbalance5 + (long)JVmastermodel.DebitAmount[i];
                                            findaccprimgroupDr5.DebitAmount = oldaccprimgrupdebit5 + (long)JVmastermodel.DebitAmount[i];
                                            ms.UpdateAccgroup(findaccprimgroupDr5);

                                        }

                                    }
                                    else
                                    {
                                        findaccprimgroupDr4.Accgroupbalance = oldaccprimbalance4 + (long)JVmastermodel.DebitAmount[i];
                                        findaccprimgroupDr4.DebitAmount = oldaccprimgrupdebit4 + (long)JVmastermodel.DebitAmount[i];
                                        ms.UpdateAccgroup(findaccprimgroupDr4);

                                    }

                                }
                                else
                                {
                                    findaccprimgroupDr3.Accgroupbalance = oldaccprimbalance3 + (long)JVmastermodel.DebitAmount[i];
                                    findaccprimgroupDr3.DebitAmount = oldaccprimgrupdebit3 + (long)JVmastermodel.DebitAmount[i];
                                    ms.UpdateAccgroup(findaccprimgroupDr3);

                                }

                            }
                            else
                            {
                                findaccprimgroupDr.Accgroupbalance = oldaccprimbalance2 + (long)JVmastermodel.DebitAmount[i];
                                findaccprimgroupDr.DebitAmount = oldaccprimgrupdebit2 + (long)JVmastermodel.DebitAmount[i];
                                ms.UpdateAccgroup(findaccprimgroupDr);

                            }


                        }
                        else
                        {
                            findaccgroup.Accgroupbalance = oldaccgrupbalance + (long)JVmastermodel.DebitAmount[i];
                            findaccgroup.DebitAmount = oldaccgrupdebit1 + (long)JVmastermodel.DebitAmount[i];
                            ms.UpdateAccgroup(findaccgroup);
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
                        ms.Updateaccount(findaccountCr);



                        //---------------------Update Account  group 1 credit----------------------------//

                        var findaccgroupCrr = ms.Getaccgroupbyid(findaccountCr.Accgroupid);
                        var oldaccgrupcredit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().CreditAmount;
                        var oldbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().Accgroupbalance;
                        if (findaccgroupCrr.Accunderprimarygroupid != 0 && findaccgroupCrr.Accunderprimarygroupid != null)
                        {
                            findaccgroupCrr.Accgroupbalance = oldbalance - (long)JVmastermodel.CreditAmount[i];
                            findaccgroupCrr.CreditAmount = oldaccgrupcredit1 + (long)JVmastermodel.CreditAmount[i];
                            ms.UpdateAccgroup(findaccgroupCrr);

                            //---------------------Update Account group 2 credit----------------------------//
                            var findaccprimgroupCr2 = ms.Getaccgroupbyid(findaccgroupCrr.Accunderprimarygroupid);
                            var oldaccprimgrupcredit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr2.AccgroupID).FirstOrDefault().CreditAmount;
                            var oldaccprimgrupbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr2.AccgroupID).FirstOrDefault().Accgroupbalance;
                            if (findaccprimgroupCr2.Accunderprimarygroupid != 0 && findaccprimgroupCr2.Accunderprimarygroupid != null)
                            {
                                findaccprimgroupCr2.Accgroupbalance = oldaccprimgrupbalance2 - (long)JVmastermodel.CreditAmount[i];
                                findaccprimgroupCr2.CreditAmount = oldaccprimgrupcredit2 + (long)JVmastermodel.CreditAmount[i];
                                ms.UpdateAccgroup(findaccprimgroupCr2);


                                //---------------------Update Account primary group 3 credit----------------------------//
                                var findaccprimgroupCr3 = ms.Getaccgroupbyid(findaccprimgroupCr2.Accunderprimarygroupid);
                                var oldaccprimgrupcredit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr3.AccgroupID).FirstOrDefault().CreditAmount;
                                var oldaccprimgrupbalance3 = findaccprimgroupCr3.Accgroupbalance;
                                if (findaccprimgroupCr3.Accunderprimarygroupid != 0 && findaccprimgroupCr3.Accunderprimarygroupid != null)
                                {
                                    findaccprimgroupCr3.Accgroupbalance = oldaccprimgrupbalance3 - (long)JVmastermodel.CreditAmount[i];
                                    findaccprimgroupCr3.CreditAmount = oldaccprimgrupcredit3 + (long)JVmastermodel.CreditAmount[i];
                                    ms.UpdateAccgroup(findaccprimgroupCr3);


                                    //---------------------Update Account primary group 4 credit----------------------------//
                                    var findaccprimgroupCr4 = ms.Getaccgroupbyid(findaccprimgroupCr3.Accunderprimarygroupid);
                                    var oldaccprimgrupcredit4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr4.AccgroupID).FirstOrDefault().CreditAmount;
                                    var oldaccprimgrupbalance4 = findaccprimgroupCr4.Accgroupbalance;

                                    if (findaccprimgroupCr4.Accunderprimarygroupid != 0 && findaccprimgroupCr4.Accunderprimarygroupid != null)
                                    {
                                        findaccprimgroupCr4.Accgroupbalance = oldaccprimgrupbalance4 - (long)JVmastermodel.CreditAmount[i];
                                        findaccprimgroupCr4.CreditAmount = oldaccprimgrupcredit4 + (long)JVmastermodel.CreditAmount[i];
                                        ms.UpdateAccgroup(findaccprimgroupCr4);


                                        //---------------------Update Account primary group 5 credit----------------------------//
                                        var findaccprimgroupCr5 = ms.Getaccgroupbyid(findaccprimgroupCr4.Accunderprimarygroupid);
                                        var oldaccprimgrupcredit5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr5.AccgroupID).FirstOrDefault().CreditAmount;
                                        var oldaccprimgrupbalance5 = findaccprimgroupCr5.Accgroupbalance;

                                        if (findaccprimgroupCr5.Accunderprimarygroupid != 0 && findaccprimgroupCr5.Accunderprimarygroupid != null)
                                        {
                                            findaccprimgroupCr5.Accgroupbalance = oldaccprimgrupbalance5 - (long)JVmastermodel.CreditAmount[i];
                                            findaccprimgroupCr5.CreditAmount = oldaccprimgrupcredit5 + (long)JVmastermodel.CreditAmount[i];
                                            ms.UpdateAccgroup(findaccprimgroupCr5);


                                            //---------------------Update Account primary group 6 credit----------------------------//
                                            var findaccprimgroupCr6 = ms.Getaccgroupbyid(findaccprimgroupCr5.Accunderprimarygroupid);
                                            var oldaccprimgrupcredit6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr6.AccgroupID).FirstOrDefault().CreditAmount;
                                            var oldaccprimgrupbalance6 = findaccprimgroupCr6.Accgroupbalance;
                                            if (findaccprimgroupCr6.Accunderprimarygroupid != 0 && findaccprimgroupCr6.Accunderprimarygroupid != null)
                                            {
                                                findaccprimgroupCr6.Accgroupbalance = oldaccprimgrupbalance6 - (long)JVmastermodel.CreditAmount[i];
                                                findaccprimgroupCr6.CreditAmount = oldaccprimgrupcredit6 + (long)JVmastermodel.CreditAmount[i];
                                                ms.UpdateAccgroup(findaccprimgroupCr6);



                                                //---------------------Update Account primary group 7 credit----------------------------//
                                                var findaccprimgroupCr7 = ms.Getaccgroupbyid(findaccprimgroupCr6.Accunderprimarygroupid);
                                                var oldaccprimgrupcredit7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr7.AccgroupID).FirstOrDefault().CreditAmount;
                                                var oldaccprimgrupbalance7 = findaccprimgroupCr7.Accgroupbalance;
                                                if (findaccprimgroupCr7.Accunderprimarygroupid != 0 && findaccprimgroupCr7.Accunderprimarygroupid != null)
                                                {
                                                    findaccprimgroupCr7.Accgroupbalance = oldaccprimgrupbalance7 - (long)JVmastermodel.CreditAmount[i];
                                                    findaccprimgroupCr7.CreditAmount = oldaccprimgrupcredit7 + (long)JVmastermodel.CreditAmount[i];
                                                    ms.UpdateAccgroup(findaccprimgroupCr7);

                                                    //---------------------Update Account primary group 8 credit----------------------------//
                                                    var findaccprimgroupCr8 = ms.Getaccgroupbyid(findaccprimgroupCr7.Accunderprimarygroupid);
                                                    var oldaccprimgrupcredit8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr8.AccgroupID).FirstOrDefault().CreditAmount;
                                                    var oldaccprimgrupbalance8 = findaccprimgroupCr8.Accgroupbalance;
                                                    if (findaccprimgroupCr8.Accunderprimarygroupid != 0 && findaccprimgroupCr8.Accunderprimarygroupid != null)
                                                    {
                                                        findaccprimgroupCr8.Accgroupbalance = oldaccprimgrupbalance8 - (long)JVmastermodel.CreditAmount[i];
                                                        findaccprimgroupCr8.CreditAmount = oldaccprimgrupcredit8 + (long)JVmastermodel.CreditAmount[i];
                                                        ms.UpdateAccgroup(findaccprimgroupCr8);


                                                        //---------------------Update Account primary group 9 credit----------------------------//
                                                        var findaccprimgroupCr9 = ms.Getaccgroupbyid(findaccprimgroupCr8.Accunderprimarygroupid);
                                                        var oldaccprimgrupcredit9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr9.AccgroupID).FirstOrDefault().CreditAmount;
                                                        var oldaccprimgrupbalance9 = findaccprimgroupCr9.Accgroupbalance;
                                                        if (findaccprimgroupCr9.Accunderprimarygroupid != 0 && findaccprimgroupCr9.Accunderprimarygroupid != null)
                                                        {
                                                            findaccprimgroupCr9.Accgroupbalance = oldaccprimgrupbalance9 - (long)JVmastermodel.CreditAmount[i];
                                                            findaccprimgroupCr9.CreditAmount = oldaccprimgrupcredit9 + (long)JVmastermodel.CreditAmount[i];
                                                            ms.UpdateAccgroup(findaccprimgroupCr9);

                                                            //---------------------Update Account primary group 9 credit----------------------------//
                                                            var findaccprimgroupCr10 = ms.Getaccgroupbyid(findaccprimgroupCr9.Accunderprimarygroupid);
                                                            var oldaccprimgrupcredit10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr10.AccgroupID).FirstOrDefault().CreditAmount;
                                                            var oldaccprimgrupbalance10 = findaccprimgroupCr10.Accgroupbalance;
                                                            if (findaccprimgroupCr10.Accunderprimarygroupid != 0 && findaccprimgroupCr10.Accunderprimarygroupid != null)
                                                            {
                                                                findaccprimgroupCr10.Accgroupbalance = oldaccprimgrupbalance10 - (long)JVmastermodel.CreditAmount[i];
                                                                findaccprimgroupCr10.CreditAmount = oldaccprimgrupcredit10 + (long)JVmastermodel.CreditAmount[i];
                                                                ms.UpdateAccgroup(findaccprimgroupCr10);


                                                            }
                                                            else
                                                            {
                                                                findaccprimgroupCr10.Accgroupbalance = oldaccprimgrupbalance10 - (long)JVmastermodel.CreditAmount[i];
                                                                findaccprimgroupCr10.CreditAmount = oldaccprimgrupcredit10 + (long)JVmastermodel.CreditAmount[i];
                                                                ms.UpdateAccgroup(findaccprimgroupCr10);
                                                            }





                                                        }
                                                        else
                                                        {
                                                            findaccprimgroupCr9.Accgroupbalance = oldaccprimgrupbalance9 - (long)JVmastermodel.CreditAmount[i];
                                                            findaccprimgroupCr9.CreditAmount = oldaccprimgrupcredit9 + (long)JVmastermodel.CreditAmount[i];
                                                            ms.UpdateAccgroup(findaccprimgroupCr9);
                                                        }




                                                    }
                                                    else
                                                    {
                                                        findaccprimgroupCr8.Accgroupbalance = oldaccprimgrupbalance8 - (long)JVmastermodel.CreditAmount[i];
                                                        findaccprimgroupCr8.CreditAmount = oldaccprimgrupcredit8 + (long)JVmastermodel.CreditAmount[i];
                                                        ms.UpdateAccgroup(findaccprimgroupCr8);
                                                    }





                                                }
                                                else
                                                {
                                                    findaccprimgroupCr7.Accgroupbalance = oldaccprimgrupbalance7 - (long)JVmastermodel.CreditAmount[i];
                                                    findaccprimgroupCr7.CreditAmount = oldaccprimgrupcredit7 + (long)JVmastermodel.CreditAmount[i];
                                                    ms.UpdateAccgroup(findaccprimgroupCr7);
                                                }



                                            }
                                            else
                                            {
                                                findaccprimgroupCr6.Accgroupbalance = oldaccprimgrupbalance6 - (long)JVmastermodel.CreditAmount[i];
                                                findaccprimgroupCr6.CreditAmount = oldaccprimgrupcredit6 + (long)JVmastermodel.CreditAmount[i];
                                                ms.UpdateAccgroup(findaccprimgroupCr6);
                                            }




                                        }
                                        else
                                        {
                                            findaccprimgroupCr5.Accgroupbalance = oldaccprimgrupbalance5 - (long)JVmastermodel.CreditAmount[i];
                                            findaccprimgroupCr5.CreditAmount = oldaccprimgrupcredit5 + (long)JVmastermodel.CreditAmount[i];
                                            ms.UpdateAccgroup(findaccprimgroupCr5);
                                        }



                                    }
                                    else
                                    {
                                        findaccprimgroupCr4.Accgroupbalance = oldaccprimgrupbalance4 - (long)JVmastermodel.CreditAmount[i];
                                        findaccprimgroupCr4.CreditAmount = oldaccprimgrupcredit4 + (long)JVmastermodel.CreditAmount[i];
                                        ms.UpdateAccgroup(findaccprimgroupCr4);
                                    }


                                }
                                else
                                {
                                    findaccprimgroupCr3.Accgroupbalance = oldaccprimgrupbalance3 - (long)JVmastermodel.CreditAmount[i];
                                    findaccprimgroupCr3.CreditAmount = oldaccprimgrupcredit3 + (long)JVmastermodel.CreditAmount[i];
                                    ms.UpdateAccgroup(findaccprimgroupCr3);
                                }


                            }
                            else
                            {
                                findaccprimgroupCr2.Accgroupbalance = oldaccprimgrupbalance2 - (long)JVmastermodel.CreditAmount[i];
                                findaccprimgroupCr2.CreditAmount = oldaccprimgrupcredit2 + (long)JVmastermodel.CreditAmount[i];
                                ms.UpdateAccgroup(findaccprimgroupCr2);

                            }

                        }

                        else
                        {

                            findaccgroupCrr.Accgroupbalance = oldbalance - (long)JVmastermodel.CreditAmount[i];
                            findaccgroupCrr.CreditAmount = oldaccgrupcredit1 + (long)JVmastermodel.CreditAmount[i];
                            ms.UpdateAccgroup(findaccgroupCrr);

                        }













                    }
                }




                return RedirectToAction("JournalVoucher");
            }
            else if (JVmastermodel.modify == 1)
            {
                var oldmasterfind = db.JournalVoucherMaster.Where(m => m.jvmasterid == JVmastermodel.JVMasterID).FirstOrDefault();
                var oldchildfind = db.JournalVoucherChild.Where(m => m.JVMasterID == oldmasterfind.jvmasterid).ToList();


                //-------update voucher----------// 
                oldmasterfind.VoucherDate = JVmastermodel.VoucherDate;
                oldmasterfind.LongNarration = JVmastermodel.LongNarration;
                oldmasterfind.DebitTotal = (double)JVmastermodel.DebitTotal;
                oldmasterfind.CreditTotal = (double)JVmastermodel.CreditTotal;
                oldmasterfind.VoucherOtherDetails = JVmastermodel.VoucherOtherDetails;
                db.Entry(oldmasterfind).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //-------------------delete old child---------------//
                if (JVmastermodel.deletedchild != null)
                {

                    for (int i = 0; i < JVmastermodel.deletedchild.Count; i++)
                    {
                        //List<int> ttas = JVmastermodel.deletedchild;

                        //int findid = db.JournalVoucherChild.Where(m =>k m.JVchildID == ttas[i]).FirstOrDefault().JVchildID;

                        //var oldchildfind2 = db.JournalVoucherChild.Find(db.JournalVoucherChild.Where(m => m.JVMasterID == JVmastermodel.deletedchild[i]).FirstOrDefault().JVchildID);
                        var olddfd = db.JournalVoucherChild.Find(JVmastermodel.deletedchild[i]);
                        db.JournalVoucherChild.Remove(olddfd);
                        db.SaveChanges();

                    }
                }

                //-------------------Add new child---------------// 
                if (JVmastermodel.newchild2 != null && JVmastermodel.newchild2.Count > 0)
                {
                    //var newrow = 0;
                    //if (JVmastermodel.newchild2credit != null)
                    //{
                    //    newrow = JVmastermodel.newchild2credit.Count;
                    //}
                    //else if(JVmastermodel.newchild2debit != null)
                    //{
                    //    newrow = JVmastermodel.newchild2debit.Count;
                    //}
                    //for (int i = 0; i < newrow; i++)
                    //{
                    //    var jvouchermasterid = JVmastermodel.JVMasterID;

                    //    double creditamount;
                    //    double debitamount = 0.00;
                    //    debitamount = JVmastermodel.DebitAmount[i];
                    //    creditamount = JVmastermodel.CreditAmount[i];

                    //    /* if (JVmastermodel.CreditAmount[i] == 0.00)
                    //     {
                    //     }
                    //    else if (JVmastermodel.DrCrID[i] == 2)
                    //    {
                    //        creditamount = JVmastermodel.CreditAmount[i];
                    //        debitamount = 0.00;
                    //    }*/
                    //    JournalVoucherChild jvchild = new JournalVoucherChild
                    //    {

                    //        JVMasterID = jvouchermasterid,
                    //        DrCrID = JVmastermodel.DrCrID[i],
                    //        AccountID = JVmastermodel.AccountID[i],
                    //        CreditAmount = JVmastermodel.CreditAmount[i],
                    //        DebitAmount = JVmastermodel.DebitAmount[i],
                    //        ShortNarration = JVmastermodel.ShortNarration[i],
                    //        JvDate = JVmastermodel.VoucherDate

                    //    };
                    //    db.JournalVoucherChild.Add(jvchild);
                    //    db.SaveChanges();

                    //}

                    for (int i = 0; i < JVmastermodel.AccountID.Count; i++)
                    {
                        double creditamount = 0.00;
                        double debitamount = 0.00;
                        var jvouchermasterid = JVmastermodel.JVMasterID;


                        if (JVmastermodel.DrCrID[i] == 1)
                        {
                            debitamount = JVmastermodel.DebitAmount[i];
                            creditamount = 0.00;
                        }
                        else if (JVmastermodel.DrCrID[i] == 2)
                        {
                            creditamount = JVmastermodel.CreditAmount[i];
                            debitamount = 0.00;
                        }

                        JournalVoucherChild jvchild = new JournalVoucherChild
                        {

                            JVMasterID = jvouchermasterid,
                            DrCrID = JVmastermodel.DrCrID[i],
                            AccountID = JVmastermodel.AccountID[i],
                            CreditAmount = creditamount,
                            FcRate = JVmastermodel.FcRate[i],
                            CurrencyID = JVmastermodel.CurrencyID[i],
                            DebitAmount = debitamount,
                            ShortNarration = JVmastermodel.ShortNarration[i]


                        };
                        db.JournalVoucherChild.Add(jvchild);
                        db.SaveChanges();
                    }


                }

                if (JVmastermodel.newchild2debit != null && JVmastermodel.newchild2debit.Count > 0)
                {
                    var newrow = 0;
                    if (JVmastermodel.newchild2credit != null)
                    {
                        newrow = JVmastermodel.newchild2credit.Count;
                    }
                    else if (JVmastermodel.newchild2debit != null)
                    {
                        newrow = JVmastermodel.newchild2debit.Count;
                    }
                    for (int i = 0; i < newrow; i++)
                    {
                        var jvouchermasterid = JVmastermodel.JVMasterID;

                        double creditamount;
                        double debitamount = 0.00;
                        debitamount = JVmastermodel.DebitAmount[i];
                        creditamount = JVmastermodel.CreditAmount[i];

                        /* if (JVmastermodel.CreditAmount[i] == 0.00)
                         {
                         }
                        else if (JVmastermodel.DrCrID[i] == 2)
                        {
                            creditamount = JVmastermodel.CreditAmount[i];
                            debitamount = 0.00;
                        }*/
                        JournalVoucherChild jvchild = new JournalVoucherChild
                        {

                            JVMasterID = jvouchermasterid,
                            DrCrID = JVmastermodel.DrCrID[i],
                            AccountID = JVmastermodel.AccountID[i],
                            CreditAmount = JVmastermodel.CreditAmount[i],
                            DebitAmount = JVmastermodel.DebitAmount[i],
                            FcRate = JVmastermodel.FcRate[i],
                            CurrencyID = JVmastermodel.CurrencyID[i],
                            ShortNarration = JVmastermodel.ShortNarration[i],
                            JvDate = JVmastermodel.VoucherDate

                        };
                        db.JournalVoucherChild.Add(jvchild);
                        db.SaveChanges();

                    }



                }


                return RedirectToAction("JournalVoucher");
            }
            return RedirectToAction("JournalVoucher");


            //catch(Exception e)
            //{

            //    ViewBag.message = e.Message;
            //    return View("~/Views/Shared/Error.cshtml");
            //}

        }
        #endregion JournalVoucher


        //-------------------------Contra Voucher--------------------------//
        #region ContraVoucher

        public ActionResult ContraVoucher()
        {
            JournalVMasterViewModel viewmodel = new JournalVMasterViewModel();

            try
            {


                int currid = 1;
                ViewBag.currency = new SelectList(db.Currency, "CurrencyID", "CurrencyName", currid);
                ViewBag.vouchernumber = db.JournalVoucherMaster.Count() + 1;
                ViewBag.DrCrID = new SelectList(db.DrCr, "DrCrID", "DrCrShortName");



                var cashinhand = db.AccGroup.Where(m => m.AccgroupID == 1072).FirstOrDefault();
                var bankacc = db.AccGroup.Where(m => m.AccgroupID == 1073).FirstOrDefault();

                List<Account> cashacclist = db.Account.Where(a => a.Accgroupid == cashinhand.AccgroupID).ToList();
                List<Account> bankacclist = db.Account.Where(a => a.Accgroupid == bankacc.AccgroupID).ToList();

                cashacclist.AddRange(bankacclist);

                ViewBag.paymentmode = new SelectList(cashacclist, "AccountID", "AccountName");

                viewmodel.vouchertype = 11;
                viewmodel.modify = 0;
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("/Views/Shared/Error.cshtml");
                throw;
            }

            return View("~/Views/Vouchers/JournalVoucher.cshtml", viewmodel);
        }

        public ActionResult ContraVoucherHistory()
        {


            var vouchercount = db.JournalVoucherMaster.Where(x => x.VoucherTypeID == 11).Count();
            List<JournalVoucherMaster> jvlist;
            JournalVMasterViewModel jvmodel = new JournalVMasterViewModel();

            if (vouchercount > 0)
            {
                jvlist = db.JournalVoucherMaster.Where(m => m.VoucherTypeID == 11).ToList();
                //var jvchildlist = db.JournalVoucherChild.ToList();

                jvmodel.Journalvouchermasterlist = jvlist;
                //jvmodel.Journalvoucherchildlist = jvchildlist;
                jvmodel.vouchertype = 11;
            }
            else
            {
                jvmodel.vouchertype = 11;

            }

            //int accountid = 2043;
            //float creditamount = 1588;

            //            //---------------------Update Account  credit----------------------------//

            //            var findaccountCr = ms.Getaccountbyid(accountid);
            //            //---------------------Update Account  group credit----------------------------//

            //            var findaccgroupCrr = ms.Getaccgroupbyid(findaccountCr.Accgroupid);
            //            var oldaccgrupcredit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().CreditAmount;
            //            var oldbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().Accgroupbalance;



            return View("/Views/Vouchers/JournalVoucherHistory.cshtml", jvmodel);

        }
        #endregion ContraVoucher


        //-------------------------Payment Voucher--------------------------//
        #region Payment Voucher

        public ActionResult PaymentVoucher()
        {
            int currid = 1;
            ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.currency = new SelectList(db.Currency, "CurrencyID", "CurrencyName", currid);
            ViewBag.vouchernumber = db.JournalVoucherMaster.Count() + 1;
            ViewBag.DrCrID = new SelectList(db.DrCr, "DrCrID", "DrCrShortName");
            ViewBag.currency = new SelectList(db.Currency, "CurrencyID", "CurrencyShortName");


            var cashinhand = db.AccGroup.Where(m => m.AccgroupID == 1072).FirstOrDefault();
            var bankacc = db.AccGroup.Where(m => m.AccgroupID == 1073).FirstOrDefault();

            List<Account> cashacclist = db.Account.Where(a => a.Accgroupid == cashinhand.AccgroupID).ToList();
            List<Account> bankacclist = db.Account.Where(a => a.Accgroupid == bankacc.AccgroupID).ToList();

            cashacclist.AddRange(bankacclist);

            ViewBag.paymentmode = new SelectList(cashacclist, "AccountID", "AccountName");
            JournalVMasterViewModel viewmodel = new JournalVMasterViewModel();
            viewmodel.vouchertype = 6;
            viewmodel.modify = 0;



            return View("/Views/Vouchers/SingleEntryVoucher.cshtml", viewmodel);
        }
        public ActionResult ModifyPaymentVoucher(int id)
        {
            var jvamsterfind = db.JournalVoucherMaster.Where(m => m.jvmasterid == id && m.VoucherTypeID == 6).FirstOrDefault();
            var jvchildfind = db.JournalVoucherChild.Where(c => c.JVMasterID == id && c.DrCrID == 1).ToList();
            var paymentmode = db.JournalVoucherChild.Where(c => c.JVMasterID == id && c.DrCrID == 2).FirstOrDefault().Account;

            ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.currency = new SelectList(db.Currency, "CurrencyID", "CurrencyName", jvamsterfind.CurrencyID);
            ViewBag.DrCrID = new SelectList(db.DrCr, "DrCrID", "DrCrShortName");

            var cashinhand = db.AccGroup.Where(m => m.Accgroupname == "cash-in-hand").FirstOrDefault();
            var bankacc = db.AccGroup.Where(m => m.Accgroupname == "Bank Accounts").FirstOrDefault();

            List<Account> cashacclist = db.Account.Where(a => a.Accgroupid == cashinhand.AccgroupID).ToList();
            List<Account> bankacclist = db.Account.Where(a => a.Accgroupid == bankacc.AccgroupID).ToList();

            cashacclist.AddRange(bankacclist);
            ViewBag.paymentmode = new SelectList(cashacclist, "AccountID", "AccountName", paymentmode);



            JournalVMasterViewModel jvviewmodel = new JournalVMasterViewModel
            {
                JvID_VoucherNo = (int)jvamsterfind.jvmasterid,
                VoucherDate = Convert.ToDateTime(jvamsterfind.VoucherDate),
                LongNarration = jvamsterfind.LongNarration,
                //CurrencyID = jvamsterfind.CurrencyID,
                DebitTotal = jvamsterfind.DebitTotal,
                CreditTotal = jvamsterfind.CreditTotal,
                VoucherOtherDetails = jvamsterfind.VoucherOtherDetails,
                paymentaccountid = paymentmode.AccountID,
                Journalvoucherchildlist = jvchildfind,
                vouchertype = 6,
                modify = 1

            };


            //return RedirectToAction("PaymentVoucher",jvviewmodel);
            return View("/Views/Vouchers/SingleEntryVoucher.cshtml", jvviewmodel);
        }
        public ActionResult Getaccounts(int accountid)
        {
            trialbalanceviewmodel tbmodel = new trialbalanceviewmodel();
            //IEnumerable<ProductDTO> accounts = new List<ProductDTO>();
            //accounts = from Accountlist in db.Account
            //           select new ProductDTO
            //           {
            //               AccountID = Accountlist.AccountID,
            //               AccountName = Accountlist.AccountName
            //           }; 
            //var accounts = db.Account.Where(m => m.AccountName == accountname).ToList();

            var accounts = db.Account.Where(m => m.AccountID == accountid).FirstOrDefault();
            tbmodel.childName = accounts.AccountName;
            tbmodel.CreditTotal = accounts.CurrencyID;
            //var currid = accounts.FirstOrDefault();

            return Json(tbmodel, JsonRequestBehavior.AllowGet);
        }
        public class ProductDTO
        {
            public int AccountID { get; set; }
            public string AccountName { get; set; }
            // Other field you may need from the Product entity
        }
        public ActionResult PaymentVoucherHistory()
        {
            var vouchercount = db.JournalVoucherMaster.Where(x => x.VoucherTypeID == 6).Count();
            List<JournalVoucherMaster> pvlist;
            JournalVMasterViewModel pvmodel = new JournalVMasterViewModel();

            if (vouchercount > 0)
            {

                pvlist = db.JournalVoucherMaster.Where(m => m.VoucherTypeID == 6).ToList();
                var pvchildlist = db.JournalVoucherChild.ToList();
                //for (int i = 0; i < pvlist.Count;)
                //{var pvchildlist = db.JournalVoucherChild.Where(m=>m.JVMasterID == pvlist[i].JvID_VoucherNo).ToList(); 

                pvmodel.Journalvouchermasterlist = pvlist;
                pvmodel.Journalvoucherchildlist = pvchildlist;
                pvmodel.vouchertype = 6;

            }

            return View(pvmodel);

        }
        [HttpPost]
        public ActionResult SavePaymentVoucher(JournalVMasterViewModel JVmastermodel)
        {
            JournalVoucherMaster jvmaster = new JournalVoucherMaster();
            if (JVmastermodel.DrCrID.Count != 0 && JVmastermodel.jvchildid3.FirstOrDefault() == 0)
            {
                var jvouchermasterid = db.JournalVoucherMaster.Count() + 1;

                for (int i = 0; i < JVmastermodel.Accountnameee.Count; i++)
                {

                    var accname = JVmastermodel.Accountnameee[i];
                    var accountid = db.Account.Where(m => m.AccountName == accname).FirstOrDefault().AccountID;


                    double creditamount;
                    double debitamount = 0.00;
                    debitamount = JVmastermodel.DebitAmount[i];
                    creditamount = JVmastermodel.CreditAmount[i];

                    /* if (JVmastermodel.CreditAmount[i] == 0.00) 
                     {
                         
                     }

                    else if (JVmastermodel.DrCrID[i] == 2)
                    {
                        creditamount = JVmastermodel.CreditAmount[i];
                        debitamount = 0.00;
                    }*/
                    if (JVmastermodel.paymentaccountid > 0 && i == 0)
                    {
                        JournalVoucherChild jvchildforcash = new JournalVoucherChild
                        {

                            JVMasterID = jvouchermasterid,
                            DrCrID = 2,
                            AccountID = JVmastermodel.paymentaccountid,
                            CreditAmount = (double)JVmastermodel.DebitTotal,

                            CurrencyID = 1,
                            DebitAmount = 0.00,
                            ShortNarration = "payment account credit",
                            JvDate = DateTime.Now
                        };
                        db.JournalVoucherChild.Add(jvchildforcash);
                        db.SaveChanges();
                    }

                    JournalVoucherChild jvchild = new JournalVoucherChild
                    {

                        JVMasterID = jvouchermasterid,
                        DrCrID = JVmastermodel.DrCrID[i],
                        AccountID = accountid,
                        CreditAmount = JVmastermodel.CreditAmount[i],
                        DebitAmount = JVmastermodel.DebitAmount[i],
                        FcRate = JVmastermodel.FcRate[i],
                        CurrencyID = JVmastermodel.CurrencyID[i],
                        ShortNarration = JVmastermodel.ShortNarration[i],
                        JvDate = DateTime.Now


                    };
                    db.JournalVoucherChild.Add(jvchild);
                    db.SaveChanges();
                }

                JournalVoucherMaster JVouchermaster = new JournalVoucherMaster
                {

                    VoucherDate = JVmastermodel.VoucherDate,
                    LongNarration = JVmastermodel.LongNarration,
                    CurrencyID = 1,
                    DebitTotal = (double)JVmastermodel.DebitTotal,
                    CreditTotal = (double)JVmastermodel.CreditTotal,
                    VoucherOtherDetails = JVmastermodel.VoucherOtherDetails,
                    VoucherTypeID = 6,
                    jvmasterid = jvouchermasterid

                };
                db.JournalVoucherMaster.Add(JVouchermaster);
                db.SaveChanges();


                for (int i = 0; i < JVmastermodel.Accountnameee.Count; i++)
                {

                    var accname2 = JVmastermodel.Accountnameee[i];
                    var accountid2 = db.Account.Where(m => m.AccountName == accname2).FirstOrDefault().AccountID;





                    if (accountid2 > 0)
                    {
                        var findaccountDr = ms.Getaccountbyid(accountid2);
                        var oldbalanceDr = db.Account.Where(x => x.AccountID == accountid2).FirstOrDefault().ClosingBalance;
                        var olddebit = db.Account.Where(x => x.AccountID == accountid2).FirstOrDefault().DebitTotal;

                        //var findaccgroupDr = ms.Getaccgroupbyid(findaccountDr.Accgroupid);
                        //var oldgroupbalanceDr = db.AccGroup.Where(m => m.AccgroupID == findaccgroupDr.AccgroupID).FirstOrDefault();


                        findaccountDr.ClosingBalance = oldbalanceDr + JVmastermodel.DebitAmount[i];
                        findaccountDr.DebitTotal = olddebit + JVmastermodel.DebitAmount[i];
                        ms.Updateaccount(findaccountDr);



                        //---------------------Update Account group 1 debit----------------------------//
                        var findaccgroup = ms.Getaccgroupbyid(findaccountDr.Accgroupid);
                        var oldaccgrupdebit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().DebitAmount;
                        var oldaccgrupbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().Accgroupbalance;

                        if (findaccgroup.Accunderprimarygroupid != 0 && findaccgroup.Accunderprimarygroupid != null)
                        {
                            findaccgroup.Accgroupbalance = oldaccgrupbalance + JVmastermodel.DebitAmount[i];
                            findaccgroup.DebitAmount = oldaccgrupdebit1 + JVmastermodel.DebitAmount[i];
                            ms.UpdateAccgroup(findaccgroup);

                            //---------------------Update Account primary group 2 credit----------------------------//

                            var findaccprimgroupDr = ms.Getaccgroupbyid(findaccgroup.Accunderprimarygroupid);
                            var oldaccprimgrupdebit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().DebitAmount;
                            var oldaccprimbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().Accgroupbalance;

                            if (findaccprimgroupDr.Accunderprimarygroupid != 0 && findaccprimgroupDr.Accunderprimarygroupid != null)
                            {
                                findaccprimgroupDr.Accgroupbalance = oldaccprimbalance2 + JVmastermodel.DebitAmount[i];
                                findaccprimgroupDr.DebitAmount = oldaccprimgrupdebit2 + JVmastermodel.DebitAmount[i];
                                ms.UpdateAccgroup(findaccprimgroupDr);


                                //---------------------Update Account group 3 debit----------------------------//

                                var findaccprimgroupDr3 = ms.Getaccgroupbyid(findaccprimgroupDr.Accunderprimarygroupid);
                                var oldaccprimgrupdebit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr3.AccgroupID).FirstOrDefault().DebitAmount;
                                var oldaccprimbalance3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr3.AccgroupID).FirstOrDefault().Accgroupbalance;

                                if (findaccprimgroupDr3.Accunderprimarygroupid != 0 && findaccprimgroupDr3.Accunderprimarygroupid != null)
                                {
                                    findaccprimgroupDr3.Accgroupbalance = oldaccprimbalance3 + JVmastermodel.DebitAmount[i];
                                    findaccprimgroupDr3.DebitAmount = oldaccprimgrupdebit3 + JVmastermodel.DebitAmount[i];
                                    ms.UpdateAccgroup(findaccprimgroupDr3);


                                    //---------------------Update Account group 4 debit----------------------------//
                                    var findaccprimgroupDr4 = ms.Getaccgroupbyid(findaccprimgroupDr3.Accunderprimarygroupid);
                                    var oldaccprimgrupdebit4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr4.AccgroupID).FirstOrDefault().DebitAmount;
                                    var oldaccprimbalance4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr4.AccgroupID).FirstOrDefault().Accgroupbalance;

                                    if (findaccprimgroupDr4.Accunderprimarygroupid != 0 && findaccprimgroupDr4.Accunderprimarygroupid != null)
                                    {
                                        findaccprimgroupDr4.Accgroupbalance = oldaccprimbalance4 + JVmastermodel.DebitAmount[i];
                                        findaccprimgroupDr4.DebitAmount = oldaccprimgrupdebit4 + JVmastermodel.DebitAmount[i];
                                        ms.UpdateAccgroup(findaccprimgroupDr4);

                                        //---------------------Update Account group 5 debit----------------------------//
                                        var findaccprimgroupDr5 = ms.Getaccgroupbyid(findaccprimgroupDr4.Accunderprimarygroupid);
                                        var oldaccprimgrupdebit5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr5.AccgroupID).FirstOrDefault().DebitAmount;
                                        var oldaccprimbalance5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr5.AccgroupID).FirstOrDefault().Accgroupbalance;
                                        if (findaccprimgroupDr5.Accunderprimarygroupid != 0 && findaccprimgroupDr5.Accunderprimarygroupid != null)
                                        {
                                            findaccprimgroupDr5.Accgroupbalance = oldaccprimbalance5 + JVmastermodel.DebitAmount[i];
                                            findaccprimgroupDr5.DebitAmount = oldaccprimgrupdebit5 + JVmastermodel.DebitAmount[i];
                                            ms.UpdateAccgroup(findaccprimgroupDr5);


                                            //---------------------Update Account group 6 debit----------------------------//
                                            var findaccprimgroupDr6 = ms.Getaccgroupbyid(findaccprimgroupDr5.Accunderprimarygroupid);
                                            var oldaccprimgrupdebit6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr6.AccgroupID).FirstOrDefault().DebitAmount;
                                            var oldaccprimbalance6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr6.AccgroupID).FirstOrDefault().Accgroupbalance;
                                            if (findaccprimgroupDr6.Accunderprimarygroupid != 0 && findaccprimgroupDr6.Accunderprimarygroupid != null)
                                            {
                                                findaccprimgroupDr6.Accgroupbalance = oldaccprimbalance6 + JVmastermodel.DebitAmount[i];
                                                findaccprimgroupDr6.DebitAmount = oldaccprimgrupdebit6 + JVmastermodel.DebitAmount[i];
                                                ms.UpdateAccgroup(findaccprimgroupDr6);

                                                //---------------------Update Account group 7 debit----------------------------//
                                                var findaccprimgroupDr7 = ms.Getaccgroupbyid(findaccprimgroupDr6.Accunderprimarygroupid);
                                                var oldaccprimgrupdebit7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr7.AccgroupID).FirstOrDefault().DebitAmount;
                                                var oldaccprimbalance7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr7.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                if (findaccprimgroupDr7.Accunderprimarygroupid != 0 && findaccprimgroupDr7.Accunderprimarygroupid != null)
                                                {
                                                    findaccprimgroupDr7.Accgroupbalance = oldaccprimbalance7 + JVmastermodel.DebitAmount[i];
                                                    findaccprimgroupDr7.DebitAmount = oldaccprimgrupdebit7 + JVmastermodel.DebitAmount[i];
                                                    ms.UpdateAccgroup(findaccprimgroupDr7);


                                                    //---------------------Update Account group 8 debit----------------------------//
                                                    var findaccprimgroupDr8 = ms.Getaccgroupbyid(findaccprimgroupDr7.Accunderprimarygroupid);
                                                    var oldaccprimgrupdebit8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr8.AccgroupID).FirstOrDefault().DebitAmount;
                                                    var oldaccprimbalance8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr8.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                    if (findaccprimgroupDr8.Accunderprimarygroupid != 0 && findaccprimgroupDr8.Accunderprimarygroupid != null)
                                                    {
                                                        findaccprimgroupDr8.Accgroupbalance = oldaccprimbalance8 + JVmastermodel.DebitAmount[i];
                                                        findaccprimgroupDr8.DebitAmount = oldaccprimgrupdebit8 + JVmastermodel.DebitAmount[i];
                                                        ms.UpdateAccgroup(findaccprimgroupDr8);


                                                        //---------------------Update Account group 9 debit----------------------------//
                                                        var findaccprimgroupDr9 = ms.Getaccgroupbyid(findaccprimgroupDr8.Accunderprimarygroupid);
                                                        var oldaccprimgrupdebit9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr9.AccgroupID).FirstOrDefault().DebitAmount;
                                                        var oldaccprimbalance9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr9.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                        if (findaccprimgroupDr9.Accunderprimarygroupid != 0 && findaccprimgroupDr9.Accunderprimarygroupid != null)
                                                        {
                                                            findaccprimgroupDr9.Accgroupbalance = oldaccprimbalance9 + JVmastermodel.DebitAmount[i];
                                                            findaccprimgroupDr9.DebitAmount = oldaccprimgrupdebit9 + JVmastermodel.DebitAmount[i];
                                                            ms.UpdateAccgroup(findaccprimgroupDr9);

                                                            //---------------------Update Account group 10 debit----------------------------//
                                                            var findaccprimgroupDr10 = ms.Getaccgroupbyid(findaccprimgroupDr9.Accunderprimarygroupid);
                                                            var oldaccprimgrupdebit10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr10.AccgroupID).FirstOrDefault().DebitAmount;
                                                            var oldaccprimbalance10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr10.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                            if (findaccprimgroupDr10.Accunderprimarygroupid != 0 && findaccprimgroupDr10.Accunderprimarygroupid != null)
                                                            {
                                                                findaccprimgroupDr10.Accgroupbalance = oldaccprimbalance10 + JVmastermodel.DebitAmount[i];
                                                                findaccprimgroupDr10.DebitAmount = oldaccprimgrupdebit10 + JVmastermodel.DebitAmount[i];
                                                                ms.UpdateAccgroup(findaccprimgroupDr10);




                                                            }
                                                            else
                                                            {
                                                                findaccprimgroupDr10.Accgroupbalance = oldaccprimbalance10 + JVmastermodel.DebitAmount[i];
                                                                findaccprimgroupDr10.DebitAmount = oldaccprimgrupdebit10 + JVmastermodel.DebitAmount[i];
                                                                ms.UpdateAccgroup(findaccprimgroupDr10);

                                                            }


                                                        }
                                                        else
                                                        {
                                                            findaccprimgroupDr9.Accgroupbalance = oldaccprimbalance9 + JVmastermodel.DebitAmount[i];
                                                            findaccprimgroupDr9.DebitAmount = oldaccprimgrupdebit9 + JVmastermodel.DebitAmount[i];
                                                            ms.UpdateAccgroup(findaccprimgroupDr9);

                                                        }


                                                    }
                                                    else
                                                    {
                                                        findaccprimgroupDr8.Accgroupbalance = oldaccprimbalance8 + JVmastermodel.DebitAmount[i];
                                                        findaccprimgroupDr8.DebitAmount = oldaccprimgrupdebit8 + JVmastermodel.DebitAmount[i];
                                                        ms.UpdateAccgroup(findaccprimgroupDr8);

                                                    }


                                                }
                                                else
                                                {
                                                    findaccprimgroupDr7.Accgroupbalance = oldaccprimbalance7 + JVmastermodel.DebitAmount[i];
                                                    findaccprimgroupDr7.DebitAmount = oldaccprimgrupdebit7 + JVmastermodel.DebitAmount[i];
                                                    ms.UpdateAccgroup(findaccprimgroupDr7);

                                                }



                                            }
                                            else
                                            {
                                                findaccprimgroupDr6.Accgroupbalance = oldaccprimbalance6 + JVmastermodel.DebitAmount[i];
                                                findaccprimgroupDr6.DebitAmount = oldaccprimgrupdebit6 + JVmastermodel.DebitAmount[i];
                                                ms.UpdateAccgroup(findaccprimgroupDr6);

                                            }

                                        }
                                        else
                                        {
                                            findaccprimgroupDr5.Accgroupbalance = oldaccprimbalance5 + JVmastermodel.DebitAmount[i];
                                            findaccprimgroupDr5.DebitAmount = oldaccprimgrupdebit5 + JVmastermodel.DebitAmount[i];
                                            ms.UpdateAccgroup(findaccprimgroupDr5);

                                        }

                                    }
                                    else
                                    {
                                        findaccprimgroupDr4.Accgroupbalance = oldaccprimbalance4 + JVmastermodel.DebitAmount[i];
                                        findaccprimgroupDr4.DebitAmount = oldaccprimgrupdebit4 + JVmastermodel.DebitAmount[i];
                                        ms.UpdateAccgroup(findaccprimgroupDr4);

                                    }

                                }
                                else
                                {
                                    findaccprimgroupDr3.Accgroupbalance = oldaccprimbalance3 + JVmastermodel.DebitAmount[i];
                                    findaccprimgroupDr3.DebitAmount = oldaccprimgrupdebit3 + JVmastermodel.DebitAmount[i];
                                    ms.UpdateAccgroup(findaccprimgroupDr3);

                                }

                            }
                            else
                            {
                                findaccprimgroupDr.Accgroupbalance = oldaccprimbalance2 + JVmastermodel.DebitAmount[i];
                                findaccprimgroupDr.DebitAmount = oldaccprimgrupdebit2 + JVmastermodel.DebitAmount[i];
                                ms.UpdateAccgroup(findaccprimgroupDr);

                            }


                        }
                        else
                        {
                            findaccgroup.Accgroupbalance = oldaccgrupbalance + JVmastermodel.DebitAmount[i];
                            findaccgroup.DebitAmount = oldaccgrupdebit1 + JVmastermodel.DebitAmount[i];
                            ms.UpdateAccgroup(findaccgroup);
                        }





                    }

                    if (JVmastermodel.paymentaccountid > 0 && i == 0)
                    {

                        //---------------------Update Account  credit----------------------------//

                        var findaccountCr = ms.Getaccountbyid(JVmastermodel.paymentaccountid);
                        var oldbalanceCr = db.Account.Where(x => x.AccountID == JVmastermodel.paymentaccountid).FirstOrDefault().ClosingBalance;
                        var oldcredit = db.Account.Where(x => x.AccountID == JVmastermodel.paymentaccountid).FirstOrDefault().CreditTotal;


                        findaccountCr.ClosingBalance = oldbalanceCr - JVmastermodel.DebitTotal;
                        findaccountCr.CreditTotal = oldcredit + JVmastermodel.DebitTotal;
                        ms.Updateaccount(findaccountCr);



                        //---------------------Update Account  group 1 credit----------------------------//

                        var findaccgroupCrr = ms.Getaccgroupbyid(findaccountCr.Accgroupid);
                        var oldaccgrupcredit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().CreditAmount;
                        var oldbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().Accgroupbalance;
                        if (findaccgroupCrr.Accunderprimarygroupid != 0 && findaccgroupCrr.Accunderprimarygroupid != null)
                        {
                            findaccgroupCrr.Accgroupbalance = oldbalance - JVmastermodel.DebitAmount[i];
                            findaccgroupCrr.CreditAmount = oldaccgrupcredit1 + JVmastermodel.DebitAmount[i];
                            ms.UpdateAccgroup(findaccgroupCrr);

                            //---------------------Update Account group 2 credit----------------------------//
                            var findaccprimgroupCr2 = ms.Getaccgroupbyid(findaccgroupCrr.Accunderprimarygroupid);
                            var oldaccprimgrupcredit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr2.AccgroupID).FirstOrDefault().CreditAmount;
                            var oldaccprimgrupbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr2.AccgroupID).FirstOrDefault().Accgroupbalance;
                            if (findaccprimgroupCr2.Accunderprimarygroupid != 0 && findaccprimgroupCr2.Accunderprimarygroupid != null)
                            {
                                findaccprimgroupCr2.Accgroupbalance = oldaccprimgrupbalance2 - JVmastermodel.DebitAmount[i];
                                findaccprimgroupCr2.CreditAmount = oldaccprimgrupcredit2 + JVmastermodel.DebitAmount[i];
                                ms.UpdateAccgroup(findaccprimgroupCr2);


                                //---------------------Update Account primary group 3 credit----------------------------//
                                var findaccprimgroupCr3 = ms.Getaccgroupbyid(findaccprimgroupCr2.Accunderprimarygroupid);
                                var oldaccprimgrupcredit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr3.AccgroupID).FirstOrDefault().CreditAmount;
                                var oldaccprimgrupbalance3 = findaccprimgroupCr3.Accgroupbalance;
                                if (findaccprimgroupCr3.Accunderprimarygroupid != 0 && findaccprimgroupCr3.Accunderprimarygroupid != null)
                                {
                                    findaccprimgroupCr3.Accgroupbalance = oldaccprimgrupbalance3 - JVmastermodel.DebitAmount[i];
                                    findaccprimgroupCr3.CreditAmount = oldaccprimgrupcredit3 + JVmastermodel.DebitAmount[i];
                                    ms.UpdateAccgroup(findaccprimgroupCr3);


                                    //---------------------Update Account primary group 4 credit----------------------------//
                                    var findaccprimgroupCr4 = ms.Getaccgroupbyid(findaccprimgroupCr3.Accunderprimarygroupid);
                                    var oldaccprimgrupcredit4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr4.AccgroupID).FirstOrDefault().CreditAmount;
                                    var oldaccprimgrupbalance4 = findaccprimgroupCr4.Accgroupbalance;

                                    if (findaccprimgroupCr4.Accunderprimarygroupid != 0 && findaccprimgroupCr4.Accunderprimarygroupid != null)
                                    {
                                        findaccprimgroupCr4.Accgroupbalance = oldaccprimgrupbalance4 - JVmastermodel.DebitAmount[i];
                                        findaccprimgroupCr4.CreditAmount = oldaccprimgrupcredit4 + JVmastermodel.DebitAmount[i];
                                        ms.UpdateAccgroup(findaccprimgroupCr4);


                                        //---------------------Update Account primary group 5 credit----------------------------//
                                        var findaccprimgroupCr5 = ms.Getaccgroupbyid(findaccprimgroupCr4.Accunderprimarygroupid);
                                        var oldaccprimgrupcredit5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr5.AccgroupID).FirstOrDefault().CreditAmount;
                                        var oldaccprimgrupbalance5 = findaccprimgroupCr5.Accgroupbalance;

                                        if (findaccprimgroupCr5.Accunderprimarygroupid != 0 && findaccprimgroupCr5.Accunderprimarygroupid != null)
                                        {
                                            findaccprimgroupCr5.Accgroupbalance = oldaccprimgrupbalance5 - JVmastermodel.DebitAmount[i];
                                            findaccprimgroupCr5.CreditAmount = oldaccprimgrupcredit5 + JVmastermodel.DebitAmount[i];
                                            ms.UpdateAccgroup(findaccprimgroupCr5);


                                            //---------------------Update Account primary group 6 credit----------------------------//
                                            var findaccprimgroupCr6 = ms.Getaccgroupbyid(findaccprimgroupCr5.Accunderprimarygroupid);
                                            var oldaccprimgrupcredit6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr6.AccgroupID).FirstOrDefault().CreditAmount;
                                            var oldaccprimgrupbalance6 = findaccprimgroupCr6.Accgroupbalance;
                                            if (findaccprimgroupCr6.Accunderprimarygroupid != 0 && findaccprimgroupCr6.Accunderprimarygroupid != null)
                                            {
                                                findaccprimgroupCr6.Accgroupbalance = oldaccprimgrupbalance6 - JVmastermodel.DebitAmount[i];
                                                findaccprimgroupCr6.CreditAmount = oldaccprimgrupcredit6 + JVmastermodel.DebitAmount[i];
                                                ms.UpdateAccgroup(findaccprimgroupCr6);



                                                //---------------------Update Account primary group 7 credit----------------------------//
                                                var findaccprimgroupCr7 = ms.Getaccgroupbyid(findaccprimgroupCr6.Accunderprimarygroupid);
                                                var oldaccprimgrupcredit7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr7.AccgroupID).FirstOrDefault().CreditAmount;
                                                var oldaccprimgrupbalance7 = findaccprimgroupCr7.Accgroupbalance;
                                                if (findaccprimgroupCr7.Accunderprimarygroupid != 0 && findaccprimgroupCr7.Accunderprimarygroupid != null)
                                                {
                                                    findaccprimgroupCr7.Accgroupbalance = oldaccprimgrupbalance7 - JVmastermodel.DebitAmount[i];
                                                    findaccprimgroupCr7.CreditAmount = oldaccprimgrupcredit7 + JVmastermodel.DebitAmount[i];
                                                    ms.UpdateAccgroup(findaccprimgroupCr7);

                                                    //---------------------Update Account primary group 8 credit----------------------------//
                                                    var findaccprimgroupCr8 = ms.Getaccgroupbyid(findaccprimgroupCr7.Accunderprimarygroupid);
                                                    var oldaccprimgrupcredit8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr8.AccgroupID).FirstOrDefault().CreditAmount;
                                                    var oldaccprimgrupbalance8 = findaccprimgroupCr8.Accgroupbalance;
                                                    if (findaccprimgroupCr8.Accunderprimarygroupid != 0 && findaccprimgroupCr8.Accunderprimarygroupid != null)
                                                    {
                                                        findaccprimgroupCr8.Accgroupbalance = oldaccprimgrupbalance8 - JVmastermodel.DebitAmount[i];
                                                        findaccprimgroupCr8.CreditAmount = oldaccprimgrupcredit8 + JVmastermodel.DebitAmount[i];
                                                        ms.UpdateAccgroup(findaccprimgroupCr8);


                                                        //---------------------Update Account primary group 9 credit----------------------------//
                                                        var findaccprimgroupCr9 = ms.Getaccgroupbyid(findaccprimgroupCr8.Accunderprimarygroupid);
                                                        var oldaccprimgrupcredit9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr9.AccgroupID).FirstOrDefault().CreditAmount;
                                                        var oldaccprimgrupbalance9 = findaccprimgroupCr9.Accgroupbalance;
                                                        if (findaccprimgroupCr9.Accunderprimarygroupid != 0 && findaccprimgroupCr9.Accunderprimarygroupid != null)
                                                        {
                                                            findaccprimgroupCr9.Accgroupbalance = oldaccprimgrupbalance9 - JVmastermodel.DebitAmount[i];
                                                            findaccprimgroupCr9.CreditAmount = oldaccprimgrupcredit9 + JVmastermodel.DebitAmount[i];
                                                            ms.UpdateAccgroup(findaccprimgroupCr9);

                                                            //---------------------Update Account primary group 9 credit----------------------------//
                                                            var findaccprimgroupCr10 = ms.Getaccgroupbyid(findaccprimgroupCr9.Accunderprimarygroupid);
                                                            var oldaccprimgrupcredit10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr10.AccgroupID).FirstOrDefault().CreditAmount;
                                                            var oldaccprimgrupbalance10 = findaccprimgroupCr10.Accgroupbalance;
                                                            if (findaccprimgroupCr10.Accunderprimarygroupid != 0 && findaccprimgroupCr10.Accunderprimarygroupid != null)
                                                            {
                                                                findaccprimgroupCr10.Accgroupbalance = oldaccprimgrupbalance10 - JVmastermodel.DebitAmount[i];
                                                                findaccprimgroupCr10.CreditAmount = oldaccprimgrupcredit10 + JVmastermodel.DebitAmount[i];
                                                                ms.UpdateAccgroup(findaccprimgroupCr10);


                                                            }
                                                            else
                                                            {
                                                                findaccprimgroupCr10.Accgroupbalance = oldaccprimgrupbalance10 - JVmastermodel.DebitAmount[i];
                                                                findaccprimgroupCr10.CreditAmount = oldaccprimgrupcredit10 + JVmastermodel.DebitAmount[i];
                                                                ms.UpdateAccgroup(findaccprimgroupCr10);
                                                            }





                                                        }
                                                        else
                                                        {
                                                            findaccprimgroupCr9.Accgroupbalance = oldaccprimgrupbalance9 - JVmastermodel.DebitAmount[i];
                                                            findaccprimgroupCr9.CreditAmount = oldaccprimgrupcredit9 + JVmastermodel.DebitAmount[i];
                                                            ms.UpdateAccgroup(findaccprimgroupCr9);
                                                        }




                                                    }
                                                    else
                                                    {
                                                        findaccprimgroupCr8.Accgroupbalance = oldaccprimgrupbalance8 - JVmastermodel.DebitAmount[i];
                                                        findaccprimgroupCr8.CreditAmount = oldaccprimgrupcredit8 + JVmastermodel.DebitAmount[i];
                                                        ms.UpdateAccgroup(findaccprimgroupCr8);
                                                    }





                                                }
                                                else
                                                {
                                                    findaccprimgroupCr7.Accgroupbalance = oldaccprimgrupbalance7 - JVmastermodel.DebitAmount[i];
                                                    findaccprimgroupCr7.CreditAmount = oldaccprimgrupcredit7 + JVmastermodel.DebitAmount[i];
                                                    ms.UpdateAccgroup(findaccprimgroupCr7);
                                                }



                                            }
                                            else
                                            {
                                                findaccprimgroupCr6.Accgroupbalance = oldaccprimgrupbalance6 - JVmastermodel.DebitAmount[i];
                                                findaccprimgroupCr6.CreditAmount = oldaccprimgrupcredit6 + JVmastermodel.DebitAmount[i];
                                                ms.UpdateAccgroup(findaccprimgroupCr6);
                                            }




                                        }
                                        else
                                        {
                                            findaccprimgroupCr5.Accgroupbalance = oldaccprimgrupbalance5 - JVmastermodel.DebitAmount[i];
                                            findaccprimgroupCr5.CreditAmount = oldaccprimgrupcredit5 + JVmastermodel.DebitAmount[i];
                                            ms.UpdateAccgroup(findaccprimgroupCr5);
                                        }



                                    }
                                    else
                                    {
                                        findaccprimgroupCr4.Accgroupbalance = oldaccprimgrupbalance4 - JVmastermodel.DebitAmount[i];
                                        findaccprimgroupCr4.CreditAmount = oldaccprimgrupcredit4 + JVmastermodel.DebitAmount[i];
                                        ms.UpdateAccgroup(findaccprimgroupCr4);
                                    }


                                }
                                else
                                {
                                    findaccprimgroupCr3.Accgroupbalance = oldaccprimgrupbalance3 - JVmastermodel.DebitAmount[i];
                                    findaccprimgroupCr3.CreditAmount = oldaccprimgrupcredit3 + JVmastermodel.DebitAmount[i];
                                    ms.UpdateAccgroup(findaccprimgroupCr3);
                                }


                            }
                            else
                            {
                                findaccprimgroupCr2.Accgroupbalance = oldaccprimgrupbalance2 - JVmastermodel.DebitAmount[i];
                                findaccprimgroupCr2.CreditAmount = oldaccprimgrupcredit2 + JVmastermodel.DebitAmount[i];
                                ms.UpdateAccgroup(findaccprimgroupCr2);

                            }

                        }


                        else
                        {

                            findaccgroupCrr.Accgroupbalance = oldbalance - JVmastermodel.DebitAmount[i];
                            findaccgroupCrr.CreditAmount = oldaccgrupcredit1 + JVmastermodel.DebitAmount[i];
                            ms.UpdateAccgroup(findaccgroupCrr);

                        }
                    }

                }





            }
            else if (JVmastermodel.jvchildid3.FirstOrDefault() == 1)
            {
                var oldmasterfind = db.JournalVoucherMaster.Where(m => m.jvmasterid == JVmastermodel.JVMasterID).FirstOrDefault();
                var oldchildfind = db.JournalVoucherChild.Where(m => m.JVMasterID == oldmasterfind.jvmasterid).ToList();
                var oldpayemntmode = db.JournalVoucherChild.Where(m => m.JVMasterID == oldmasterfind.jvmasterid && m.CreditAmount != 0 && m.DrCrID == 2).FirstOrDefault();


                //-------update voucher----------//

                oldmasterfind.VoucherDate = JVmastermodel.VoucherDate;
                oldmasterfind.LongNarration = JVmastermodel.LongNarration;
                oldmasterfind.CurrencyID = 1;
                oldmasterfind.DebitTotal = (double)JVmastermodel.DebitTotal;
                oldmasterfind.CreditTotal = (double)JVmastermodel.CreditTotal;
                oldmasterfind.VoucherOtherDetails = JVmastermodel.VoucherOtherDetails;
                db.Entry(oldmasterfind).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                //-------update paymentmode----------//
                oldpayemntmode.DebitAmount = 0.00;
                oldpayemntmode.CreditAmount = (double)JVmastermodel.CreditTotal;
                db.Entry(oldpayemntmode).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                //          int countdeleted = JVmastermodel.deletedchild.Count;

                if (JVmastermodel.deletedchild != null)
                {

                    for (int i = 0; i < JVmastermodel.deletedchild.Count; i++)
                    {
                        //List<int> ttas = JVmastermodel.deletedchild;

                        //int findid = db.JournalVoucherChild.Where(m =>k m.JVchildID == ttas[i]).FirstOrDefault().JVchildID;

                        //var oldchildfind2 = db.JournalVoucherChild.Find(db.JournalVoucherChild.Where(m => m.JVMasterID == JVmastermodel.deletedchild[i]).FirstOrDefault().JVchildID);
                        var olddfd = db.JournalVoucherChild.Find(JVmastermodel.deletedchild[i]);
                        db.JournalVoucherChild.Remove(olddfd);
                        db.SaveChanges();

                    }
                }
                if (JVmastermodel.newchild2 != null && JVmastermodel.newchild2.Count > 0)
                {
                    for (int i = 0; i < JVmastermodel.newchild2.Count; i++)
                    {
                        var jvouchermasterid = JVmastermodel.JVMasterID;

                        double creditamount;
                        double debitamount = 0.00;
                        debitamount = JVmastermodel.DebitAmount[i];
                        creditamount = JVmastermodel.CreditAmount[i];

                        /* if (JVmastermodel.CreditAmount[i] == 0.00)
                         {

                         }

                        else if (JVmastermodel.DrCrID[i] == 2)
                        {
                            creditamount = JVmastermodel.CreditAmount[i];
                            debitamount = 0.00;
                        }*/
                        JournalVoucherChild jvchild = new JournalVoucherChild
                        {

                            JVMasterID = jvouchermasterid,
                            DrCrID = JVmastermodel.DrCrID[i],
                            AccountID = JVmastermodel.AccountID[i],
                            CreditAmount = JVmastermodel.CreditAmount[i],
                            FcRate = JVmastermodel.FcRate[i],
                            CurrencyID = JVmastermodel.CurrencyID[i],
                            DebitAmount = JVmastermodel.DebitAmount[i],
                            ShortNarration = JVmastermodel.ShortNarration[i],
                            JvDate = DateTime.Now
                        };
                        db.JournalVoucherChild.Add(jvchild);
                        db.SaveChanges();

                    }



                }
                return RedirectToAction("PaymentVoucher");

            }




            return RedirectToAction("PaymentVoucher");



        }

        #endregion Payment Voucher


        //-------------------------Receipt Voucher--------------------------//
        #region Receipt Voucher

        public ActionResult RecieptVoucher()
        {
            int currid = 1;
            ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.currency = new SelectList(db.Currency, "CurrencyID", "CurrencyName", currid);
            ViewBag.vouchernumber = db.JournalVoucherMaster.Count() + 1;
            ViewBag.DrCrID = new SelectList(db.DrCr, "DrCrID", "DrCrShortName");


            var cashinhand = db.AccGroup.Where(m => m.AccgroupID == 1072).FirstOrDefault();
            var bankacc = db.AccGroup.Where(m => m.AccgroupID == 1073).FirstOrDefault();

            List<Account> cashacclist = db.Account.Where(a => a.Accgroupid == cashinhand.AccgroupID).ToList();
            List<Account> bankacclist = db.Account.Where(a => a.Accgroupid == bankacc.AccgroupID).ToList();

            cashacclist.AddRange(bankacclist);

            ViewBag.paymentmode = new SelectList(cashacclist, "AccountID", "AccountName");
            JournalVMasterViewModel viewmodel = new JournalVMasterViewModel();
            viewmodel.vouchertype = 8;
            viewmodel.modify = 0;


            return View("/Views/Vouchers/SingleEntryVoucher.cshtml", viewmodel);
        }
        public ActionResult RecieptVoucherHistory()
        {

            var vouchercount = db.JournalVoucherMaster.Where(x => x.VoucherTypeID == 8).Count();
            List<JournalVoucherMaster> pvlist;
            JournalVMasterViewModel pvmodel = new JournalVMasterViewModel();

            if (vouchercount > 0)
            {
                pvlist = db.JournalVoucherMaster.Where(m => m.VoucherTypeID == 8).ToList();
                var pvchildlist = db.JournalVoucherChild.ToList();
                //for (int i = 0; i < pvlist.Count;)
                //{var pvchildlist = db.JournalVoucherChild.Where(m=>m.JVMasterID == pvlist[i].JvID_VoucherNo).ToList();

                pvmodel.Journalvouchermasterlist = pvlist;
                pvmodel.Journalvoucherchildlist = pvchildlist;
                pvmodel.vouchertype = 8;

            }

            return View(pvmodel);

        }
        public ActionResult ModifyRecieptVoucher(int id)
        {
            var jvamsterfind = db.JournalVoucherMaster.Where(m => m.jvmasterid == id && m.VoucherTypeID == 8).FirstOrDefault();
            var jvchildfind = db.JournalVoucherChild.Where(c => c.JVMasterID == id && c.DrCrID == 2).ToList();
            var paymentmode = db.JournalVoucherChild.Where(c => c.JVMasterID == id && c.DrCrID == 1).FirstOrDefault().Account;

            ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.currency = new SelectList(db.Currency, "CurrencyID", "CurrencyName", jvamsterfind.CurrencyID);
            ViewBag.DrCrID = new SelectList(db.DrCr, "DrCrID", "DrCrShortName");

            var cashinhand = db.AccGroup.Where(m => m.Accgroupname == "cash-in-hand").FirstOrDefault();
            var bankacc = db.AccGroup.Where(m => m.Accgroupname == "Bank Accounts").FirstOrDefault();

            List<Account> cashacclist = db.Account.Where(a => a.Accgroupid == cashinhand.AccgroupID).ToList();
            List<Account> bankacclist = db.Account.Where(a => a.Accgroupid == bankacc.AccgroupID).ToList();

            cashacclist.AddRange(bankacclist);
            ViewBag.paymentmode = new SelectList(cashacclist, "AccountID", "AccountName", paymentmode);



            JournalVMasterViewModel jvviewmodel = new JournalVMasterViewModel
            {
                JvID_VoucherNo = (int)jvamsterfind.jvmasterid,
                VoucherDate = jvamsterfind.VoucherDate,
                LongNarration = jvamsterfind.LongNarration,
                //CurrencyID = jvamsterfind.CurrencyID,
                DebitTotal = jvamsterfind.DebitTotal,
                CreditTotal = jvamsterfind.CreditTotal,
                VoucherOtherDetails = jvamsterfind.VoucherOtherDetails,
                paymentaccountid = paymentmode.AccountID,
                Journalvoucherchildlist = jvchildfind,
                vouchertype = 8,
                modify = 1
            };


            return View("/Views/Vouchers/SingleEntryVoucher.cshtml", jvviewmodel);
        }
        [HttpPost]
        public ActionResult SaveRecieptVoucher(JournalVMasterViewModel JVmastermodel)
        {
            JournalVoucherMaster jvmaster = new JournalVoucherMaster();
            if (JVmastermodel.DrCrID.Count != 0 && JVmastermodel.jvchildid3.FirstOrDefault() == 0)
            {

                var jvouchermasterid = db.JournalVoucherMaster.Count() + 1;

                for (int i = 0; i < JVmastermodel.Accountnameee.Count; i++)
                {


                    var accname = JVmastermodel.Accountnameee[i];
                    var accountid = db.Account.Where(m => m.AccountName == accname).FirstOrDefault().AccountID;


                    double creditamount;
                    double debitamount = 0.00;
                    debitamount = JVmastermodel.DebitAmount[i];
                    creditamount = JVmastermodel.CreditAmount[i];

                    /* if (JVmastermodel.CreditAmount[i] == 0.00)
                     {
                         
                     }

                    else if (JVmastermodel.DrCrID[i] == 2)
                    {
                        creditamount = JVmastermodel.CreditAmount[i];
                        debitamount = 0.00;
                    }*/
                    if (JVmastermodel.paymentaccountid > 0 && i == 0)
                    {
                        JournalVoucherChild jvchildforpaymentmode = new JournalVoucherChild
                        {

                            JVMasterID = jvouchermasterid,
                            DrCrID = 1,
                            AccountID = JVmastermodel.paymentaccountid,
                            CreditAmount = 0.00,
                            CurrencyID = 1,
                            DebitAmount = (double)JVmastermodel.CreditTotal,
                            ShortNarration = "payment account debit",
                            JvDate = DateTime.Now

                        };
                        db.JournalVoucherChild.Add(jvchildforpaymentmode);
                        db.SaveChanges();
                    }

                    JournalVoucherChild jvchild = new JournalVoucherChild
                    {

                        JVMasterID = jvouchermasterid,
                        DrCrID = JVmastermodel.DrCrID[i],
                        AccountID = accountid,
                        FcRate = JVmastermodel.FcRate[i],
                        CurrencyID = JVmastermodel.CurrencyID[i],
                        CreditAmount = JVmastermodel.CreditAmount[i],
                        DebitAmount = JVmastermodel.DebitAmount[i],
                        ShortNarration = JVmastermodel.ShortNarration[i],
                        JvDate = DateTime.Now

                    };
                    db.JournalVoucherChild.Add(jvchild);
                    db.SaveChanges();
                }

                JournalVoucherMaster JVouchermaster = new JournalVoucherMaster
                {

                    VoucherDate = JVmastermodel.VoucherDate,
                    LongNarration = JVmastermodel.LongNarration,
                    CurrencyID = 1,
                    DebitTotal = (double)JVmastermodel.DebitTotal,
                    CreditTotal = (double)JVmastermodel.CreditTotal,
                    VoucherOtherDetails = JVmastermodel.VoucherOtherDetails,
                    VoucherTypeID = 8,
                    jvmasterid = jvouchermasterid

                };
                db.JournalVoucherMaster.Add(JVouchermaster);
                db.SaveChanges();


                for (int i = 0; i < JVmastermodel.Accountnameee.Count; i++)
                {

                    var accname2 = JVmastermodel.Accountnameee[i];
                    var accountid2 = db.Account.Where(m => m.AccountName == accname2).FirstOrDefault().AccountID;



                    if (accountid2 > 0)
                    {





                        //---------------------Update Account  credit----------------------------//

                        var findaccountCr = ms.Getaccountbyid(accountid2);
                        var oldbalanceCr = db.Account.Where(x => x.AccountID == accountid2).FirstOrDefault().ClosingBalance;
                        var oldcredit = db.Account.Where(x => x.AccountID == accountid2).FirstOrDefault().CreditTotal;


                        findaccountCr.ClosingBalance = (double)oldbalanceCr - JVmastermodel.CreditAmount[i];
                        findaccountCr.CreditTotal = (double)oldcredit + JVmastermodel.CreditAmount[i];
                        ms.Updateaccount(findaccountCr);



                        //---------------------Update Account  group 1 credit----------------------------//

                        var findaccgroupCrr = ms.Getaccgroupbyid(findaccountCr.Accgroupid);
                        var oldaccgrupcredit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().CreditAmount;
                        var oldbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().Accgroupbalance;
                        if (findaccgroupCrr.Accunderprimarygroupid != 0 && findaccgroupCrr.Accunderprimarygroupid != null)
                        {
                            findaccgroupCrr.Accgroupbalance = (double)oldbalance - JVmastermodel.CreditAmount[i];
                            findaccgroupCrr.CreditAmount = (double)oldaccgrupcredit1 + JVmastermodel.CreditAmount[i];
                            ms.UpdateAccgroup(findaccgroupCrr);

                            //---------------------Update Account group 2 credit----------------------------//
                            var findaccprimgroupCr2 = ms.Getaccgroupbyid(findaccgroupCrr.Accunderprimarygroupid);
                            var oldaccprimgrupcredit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr2.AccgroupID).FirstOrDefault().CreditAmount;
                            var oldaccprimgrupbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr2.AccgroupID).FirstOrDefault().Accgroupbalance;
                            if (findaccprimgroupCr2.Accunderprimarygroupid != 0 && findaccprimgroupCr2.Accunderprimarygroupid != null)
                            {
                                findaccprimgroupCr2.Accgroupbalance = (double)oldaccprimgrupbalance2 - JVmastermodel.CreditAmount[i];
                                findaccprimgroupCr2.CreditAmount = (double)oldaccprimgrupcredit2 + JVmastermodel.CreditAmount[i];
                                ms.UpdateAccgroup(findaccprimgroupCr2);


                                //---------------------Update Account primary group 3 credit----------------------------//
                                var findaccprimgroupCr3 = ms.Getaccgroupbyid(findaccprimgroupCr2.Accunderprimarygroupid);
                                var oldaccprimgrupcredit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr3.AccgroupID).FirstOrDefault().CreditAmount;
                                var oldaccprimgrupbalance3 = findaccprimgroupCr3.Accgroupbalance;
                                if (findaccprimgroupCr3.Accunderprimarygroupid != 0 && findaccprimgroupCr3.Accunderprimarygroupid != null)
                                {
                                    findaccprimgroupCr3.Accgroupbalance = (double)oldaccprimgrupbalance3 - JVmastermodel.CreditAmount[i];
                                    findaccprimgroupCr3.CreditAmount = (double)oldaccprimgrupcredit3 + JVmastermodel.CreditAmount[i];
                                    ms.UpdateAccgroup(findaccprimgroupCr3);


                                    //---------------------Update Account primary group 4 credit----------------------------//
                                    var findaccprimgroupCr4 = ms.Getaccgroupbyid(findaccprimgroupCr3.Accunderprimarygroupid);
                                    var oldaccprimgrupcredit4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr4.AccgroupID).FirstOrDefault().CreditAmount;
                                    var oldaccprimgrupbalance4 = findaccprimgroupCr4.Accgroupbalance;

                                    if (findaccprimgroupCr4.Accunderprimarygroupid != 0 && findaccprimgroupCr4.Accunderprimarygroupid != null)
                                    {
                                        findaccprimgroupCr4.Accgroupbalance = (double)oldaccprimgrupbalance4 - JVmastermodel.CreditAmount[i];
                                        findaccprimgroupCr4.CreditAmount = (double)oldaccprimgrupcredit4 + JVmastermodel.CreditAmount[i];
                                        ms.UpdateAccgroup(findaccprimgroupCr4);


                                        //---------------------Update Account primary group 5 credit----------------------------//
                                        var findaccprimgroupCr5 = ms.Getaccgroupbyid(findaccprimgroupCr4.Accunderprimarygroupid);
                                        var oldaccprimgrupcredit5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr5.AccgroupID).FirstOrDefault().CreditAmount;
                                        var oldaccprimgrupbalance5 = findaccprimgroupCr5.Accgroupbalance;

                                        if (findaccprimgroupCr5.Accunderprimarygroupid != 0 && findaccprimgroupCr5.Accunderprimarygroupid != null)
                                        {
                                            findaccprimgroupCr5.Accgroupbalance = (double)oldaccprimgrupbalance5 - JVmastermodel.CreditAmount[i];
                                            findaccprimgroupCr5.CreditAmount = (double)oldaccprimgrupcredit5 + JVmastermodel.CreditAmount[i];
                                            ms.UpdateAccgroup(findaccprimgroupCr5);


                                            //---------------------Update Account primary group 6 credit----------------------------//
                                            var findaccprimgroupCr6 = ms.Getaccgroupbyid(findaccprimgroupCr5.Accunderprimarygroupid);
                                            var oldaccprimgrupcredit6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr6.AccgroupID).FirstOrDefault().CreditAmount;
                                            var oldaccprimgrupbalance6 = findaccprimgroupCr6.Accgroupbalance;
                                            if (findaccprimgroupCr6.Accunderprimarygroupid != 0 && findaccprimgroupCr6.Accunderprimarygroupid != null)
                                            {
                                                findaccprimgroupCr6.Accgroupbalance = (double)oldaccprimgrupbalance6 - JVmastermodel.CreditAmount[i];
                                                findaccprimgroupCr6.CreditAmount = (double)oldaccprimgrupcredit6 + JVmastermodel.CreditAmount[i];
                                                ms.UpdateAccgroup(findaccprimgroupCr6);



                                                //---------------------Update Account primary group 7 credit----------------------------//
                                                var findaccprimgroupCr7 = ms.Getaccgroupbyid(findaccprimgroupCr6.Accunderprimarygroupid);
                                                var oldaccprimgrupcredit7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr7.AccgroupID).FirstOrDefault().CreditAmount;
                                                var oldaccprimgrupbalance7 = findaccprimgroupCr7.Accgroupbalance;
                                                if (findaccprimgroupCr7.Accunderprimarygroupid != 0 && findaccprimgroupCr7.Accunderprimarygroupid != null)
                                                {
                                                    findaccprimgroupCr7.Accgroupbalance = (double)oldaccprimgrupbalance7 - JVmastermodel.CreditAmount[i];
                                                    findaccprimgroupCr7.CreditAmount = (double)oldaccprimgrupcredit7 + JVmastermodel.CreditAmount[i];
                                                    ms.UpdateAccgroup(findaccprimgroupCr7);

                                                    //---------------------Update Account primary group 8 credit----------------------------//
                                                    var findaccprimgroupCr8 = ms.Getaccgroupbyid(findaccprimgroupCr7.Accunderprimarygroupid);
                                                    var oldaccprimgrupcredit8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr8.AccgroupID).FirstOrDefault().CreditAmount;
                                                    var oldaccprimgrupbalance8 = findaccprimgroupCr8.Accgroupbalance;
                                                    if (findaccprimgroupCr8.Accunderprimarygroupid != 0 && findaccprimgroupCr8.Accunderprimarygroupid != null)
                                                    {
                                                        findaccprimgroupCr8.Accgroupbalance = (double)oldaccprimgrupbalance8 - JVmastermodel.CreditAmount[i];
                                                        findaccprimgroupCr8.CreditAmount = (double)oldaccprimgrupcredit8 + JVmastermodel.CreditAmount[i];
                                                        ms.UpdateAccgroup(findaccprimgroupCr8);


                                                        //---------------------Update Account primary group 9 credit----------------------------//
                                                        var findaccprimgroupCr9 = ms.Getaccgroupbyid(findaccprimgroupCr8.Accunderprimarygroupid);
                                                        var oldaccprimgrupcredit9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr9.AccgroupID).FirstOrDefault().CreditAmount;
                                                        var oldaccprimgrupbalance9 = findaccprimgroupCr9.Accgroupbalance;
                                                        if (findaccprimgroupCr9.Accunderprimarygroupid != 0 && findaccprimgroupCr9.Accunderprimarygroupid != null)
                                                        {
                                                            findaccprimgroupCr9.Accgroupbalance = (double)oldaccprimgrupbalance9 - JVmastermodel.CreditAmount[i];
                                                            findaccprimgroupCr9.CreditAmount = (double)oldaccprimgrupcredit9 + JVmastermodel.CreditAmount[i];
                                                            ms.UpdateAccgroup(findaccprimgroupCr9);

                                                            //---------------------Update Account primary group 9 credit----------------------------//
                                                            var findaccprimgroupCr10 = ms.Getaccgroupbyid(findaccprimgroupCr9.Accunderprimarygroupid);
                                                            var oldaccprimgrupcredit10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr10.AccgroupID).FirstOrDefault().CreditAmount;
                                                            var oldaccprimgrupbalance10 = findaccprimgroupCr10.Accgroupbalance;
                                                            if (findaccprimgroupCr10.Accunderprimarygroupid != 0 && findaccprimgroupCr10.Accunderprimarygroupid != null)
                                                            {
                                                                findaccprimgroupCr10.Accgroupbalance = (double)oldaccprimgrupbalance10 - JVmastermodel.CreditAmount[i];
                                                                findaccprimgroupCr10.CreditAmount = (double)oldaccprimgrupcredit10 + JVmastermodel.CreditAmount[i];
                                                                ms.UpdateAccgroup(findaccprimgroupCr10);


                                                            }
                                                            else
                                                            {
                                                                findaccprimgroupCr10.Accgroupbalance = (double)oldaccprimgrupbalance10 - JVmastermodel.CreditAmount[i];
                                                                findaccprimgroupCr10.CreditAmount = (double)oldaccprimgrupcredit10 + JVmastermodel.CreditAmount[i];
                                                                ms.UpdateAccgroup(findaccprimgroupCr10);
                                                            }





                                                        }
                                                        else
                                                        {
                                                            findaccprimgroupCr9.Accgroupbalance = (double)oldaccprimgrupbalance9 - JVmastermodel.CreditAmount[i];
                                                            findaccprimgroupCr9.CreditAmount = (double)oldaccprimgrupcredit9 + JVmastermodel.CreditAmount[i];
                                                            ms.UpdateAccgroup(findaccprimgroupCr9);
                                                        }




                                                    }
                                                    else
                                                    {
                                                        findaccprimgroupCr8.Accgroupbalance = (double)oldaccprimgrupbalance8 - JVmastermodel.CreditAmount[i];
                                                        findaccprimgroupCr8.CreditAmount = (double)oldaccprimgrupcredit8 + JVmastermodel.CreditAmount[i];
                                                        ms.UpdateAccgroup(findaccprimgroupCr8);
                                                    }





                                                }
                                                else
                                                {
                                                    findaccprimgroupCr7.Accgroupbalance = (double)oldaccprimgrupbalance7 - JVmastermodel.CreditAmount[i];
                                                    findaccprimgroupCr7.CreditAmount = (double)oldaccprimgrupcredit7 + JVmastermodel.CreditAmount[i];
                                                    ms.UpdateAccgroup(findaccprimgroupCr7);
                                                }



                                            }
                                            else
                                            {
                                                findaccprimgroupCr6.Accgroupbalance = (double)oldaccprimgrupbalance6 - JVmastermodel.CreditAmount[i];
                                                findaccprimgroupCr6.CreditAmount = (double)oldaccprimgrupcredit6 + JVmastermodel.CreditAmount[i];
                                                ms.UpdateAccgroup(findaccprimgroupCr6);
                                            }




                                        }
                                        else
                                        {
                                            findaccprimgroupCr5.Accgroupbalance = (double)oldaccprimgrupbalance5 - JVmastermodel.CreditAmount[i];
                                            findaccprimgroupCr5.CreditAmount = (double)oldaccprimgrupcredit5 + JVmastermodel.CreditAmount[i];
                                            ms.UpdateAccgroup(findaccprimgroupCr5);
                                        }



                                    }
                                    else
                                    {
                                        findaccprimgroupCr4.Accgroupbalance = (double)oldaccprimgrupbalance4 - JVmastermodel.CreditAmount[i];
                                        findaccprimgroupCr4.CreditAmount = (double)oldaccprimgrupcredit4 + JVmastermodel.CreditAmount[i];
                                        ms.UpdateAccgroup(findaccprimgroupCr4);
                                    }


                                }
                                else
                                {
                                    findaccprimgroupCr3.Accgroupbalance = (double)oldaccprimgrupbalance3 - JVmastermodel.CreditAmount[i];
                                    findaccprimgroupCr3.CreditAmount = (double)oldaccprimgrupcredit3 + JVmastermodel.CreditAmount[i];
                                    ms.UpdateAccgroup(findaccprimgroupCr3);
                                }


                            }
                            else
                            {
                                findaccprimgroupCr2.Accgroupbalance = (double)oldaccprimgrupbalance2 - JVmastermodel.CreditAmount[i];
                                findaccprimgroupCr2.CreditAmount = (double)oldaccprimgrupcredit2 + JVmastermodel.CreditAmount[i];
                                ms.UpdateAccgroup(findaccprimgroupCr2);

                            }

                        }

                        else
                        {

                            findaccgroupCrr.Accgroupbalance = (double)oldbalance - JVmastermodel.CreditAmount[i];
                            findaccgroupCrr.CreditAmount = (double)oldaccgrupcredit1 + JVmastermodel.CreditAmount[i];
                            ms.UpdateAccgroup(findaccgroupCrr);

                        }
                    }

                    if (JVmastermodel.paymentaccountid > 0 && i == 0)
                    {
                        var findaccountDr = ms.Getaccountbyid(JVmastermodel.paymentaccountid);
                        var oldbalanceDr = db.Account.Where(x => x.AccountID == JVmastermodel.paymentaccountid).FirstOrDefault().ClosingBalance;
                        var olddebit = db.Account.Where(x => x.AccountID == JVmastermodel.paymentaccountid).FirstOrDefault().DebitTotal;

                        //var findaccgroupDr = ms.Getaccgroupbyid(findaccountDr.Accgroupid);
                        //var oldgroupbalanceDr = db.AccGroup.Where(m => m.AccgroupID == findaccgroupDr.AccgroupID).FirstOrDefault();


                        findaccountDr.ClosingBalance = oldbalanceDr + JVmastermodel.DebitTotal;
                        findaccountDr.DebitTotal = olddebit + JVmastermodel.DebitTotal;
                        ms.Updateaccount(findaccountDr);



                        //---------------------Update Account group 1 debit----------------------------//
                        var findaccgroup = ms.Getaccgroupbyid(findaccountDr.Accgroupid);
                        var oldaccgrupdebit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().DebitAmount;
                        var oldaccgrupbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().Accgroupbalance;

                        if (findaccgroup.Accunderprimarygroupid != 0 && findaccgroup.Accunderprimarygroupid != null)
                        {
                            findaccgroup.Accgroupbalance = (double)oldaccgrupbalance + JVmastermodel.DebitTotal;
                            findaccgroup.DebitAmount = (double)oldaccgrupdebit1 + JVmastermodel.DebitTotal;
                            ms.UpdateAccgroup(findaccgroup);

                            //---------------------Update Account primary group 2 credit----------------------------//

                            var findaccprimgroupDr = ms.Getaccgroupbyid(findaccgroup.Accunderprimarygroupid);
                            var oldaccprimgrupdebit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().DebitAmount;
                            var oldaccprimbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().Accgroupbalance;

                            if (findaccprimgroupDr.Accunderprimarygroupid != 0 && findaccprimgroupDr.Accunderprimarygroupid != null)
                            {
                                findaccprimgroupDr.Accgroupbalance = (double)oldaccprimbalance2 + JVmastermodel.DebitTotal;
                                findaccprimgroupDr.DebitAmount = (double)oldaccprimgrupdebit2 + JVmastermodel.DebitTotal;
                                ms.UpdateAccgroup(findaccprimgroupDr);


                                //---------------------Update Account group 3 debit----------------------------//

                                var findaccprimgroupDr3 = ms.Getaccgroupbyid(findaccprimgroupDr.Accunderprimarygroupid);
                                var oldaccprimgrupdebit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr3.AccgroupID).FirstOrDefault().DebitAmount;
                                var oldaccprimbalance3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr3.AccgroupID).FirstOrDefault().Accgroupbalance;

                                if (findaccprimgroupDr3.Accunderprimarygroupid != 0 && findaccprimgroupDr3.Accunderprimarygroupid != null)
                                {
                                    findaccprimgroupDr3.Accgroupbalance = (double)oldaccprimbalance3 + JVmastermodel.DebitTotal;
                                    findaccprimgroupDr3.DebitAmount = (double)oldaccprimgrupdebit3 + JVmastermodel.DebitTotal;
                                    ms.UpdateAccgroup(findaccprimgroupDr3);


                                    //---------------------Update Account group 4 debit----------------------------//
                                    var findaccprimgroupDr4 = ms.Getaccgroupbyid(findaccprimgroupDr3.Accunderprimarygroupid);
                                    var oldaccprimgrupdebit4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr4.AccgroupID).FirstOrDefault().DebitAmount;
                                    var oldaccprimbalance4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr4.AccgroupID).FirstOrDefault().Accgroupbalance;

                                    if (findaccprimgroupDr4.Accunderprimarygroupid != 0 && findaccprimgroupDr4.Accunderprimarygroupid != null)
                                    {
                                        findaccprimgroupDr4.Accgroupbalance = (double)oldaccprimbalance4 + JVmastermodel.DebitTotal;
                                        findaccprimgroupDr4.DebitAmount = (double)oldaccprimgrupdebit4 + JVmastermodel.DebitTotal;
                                        ms.UpdateAccgroup(findaccprimgroupDr4);

                                        //---------------------Update Account group 5 debit----------------------------//
                                        var findaccprimgroupDr5 = ms.Getaccgroupbyid(findaccprimgroupDr4.Accunderprimarygroupid);
                                        var oldaccprimgrupdebit5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr5.AccgroupID).FirstOrDefault().DebitAmount;
                                        var oldaccprimbalance5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr5.AccgroupID).FirstOrDefault().Accgroupbalance;
                                        if (findaccprimgroupDr5.Accunderprimarygroupid != 0 && findaccprimgroupDr5.Accunderprimarygroupid != null)
                                        {
                                            findaccprimgroupDr5.Accgroupbalance = (double)oldaccprimbalance5 + JVmastermodel.DebitTotal;
                                            findaccprimgroupDr5.DebitAmount = (double)oldaccprimgrupdebit5 + JVmastermodel.DebitTotal;
                                            ms.UpdateAccgroup(findaccprimgroupDr5);


                                            //---------------------Update Account group 6 debit----------------------------//
                                            var findaccprimgroupDr6 = ms.Getaccgroupbyid(findaccprimgroupDr5.Accunderprimarygroupid);
                                            var oldaccprimgrupdebit6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr6.AccgroupID).FirstOrDefault().DebitAmount;
                                            var oldaccprimbalance6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr6.AccgroupID).FirstOrDefault().Accgroupbalance;
                                            if (findaccprimgroupDr6.Accunderprimarygroupid != 0 && findaccprimgroupDr6.Accunderprimarygroupid != null)
                                            {
                                                findaccprimgroupDr6.Accgroupbalance = (double)oldaccprimbalance6 + JVmastermodel.DebitTotal;
                                                findaccprimgroupDr6.DebitAmount = (double)oldaccprimgrupdebit6 + JVmastermodel.DebitTotal;
                                                ms.UpdateAccgroup(findaccprimgroupDr6);

                                                //---------------------Update Account group 7 debit----------------------------//
                                                var findaccprimgroupDr7 = ms.Getaccgroupbyid(findaccprimgroupDr6.Accunderprimarygroupid);
                                                var oldaccprimgrupdebit7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr7.AccgroupID).FirstOrDefault().DebitAmount;
                                                var oldaccprimbalance7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr7.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                if (findaccprimgroupDr7.Accunderprimarygroupid != 0 && findaccprimgroupDr7.Accunderprimarygroupid != null)
                                                {
                                                    findaccprimgroupDr7.Accgroupbalance = (double)oldaccprimbalance7 + JVmastermodel.DebitTotal;
                                                    findaccprimgroupDr7.DebitAmount = (double)oldaccprimgrupdebit7 + JVmastermodel.DebitTotal;
                                                    ms.UpdateAccgroup(findaccprimgroupDr7);


                                                    //---------------------Update Account group 8 debit----------------------------//
                                                    var findaccprimgroupDr8 = ms.Getaccgroupbyid(findaccprimgroupDr7.Accunderprimarygroupid);
                                                    var oldaccprimgrupdebit8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr8.AccgroupID).FirstOrDefault().DebitAmount;
                                                    var oldaccprimbalance8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr8.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                    if (findaccprimgroupDr8.Accunderprimarygroupid != 0 && findaccprimgroupDr8.Accunderprimarygroupid != null)
                                                    {
                                                        findaccprimgroupDr8.Accgroupbalance = (double)oldaccprimbalance8 + JVmastermodel.DebitTotal;
                                                        findaccprimgroupDr8.DebitAmount = (double)oldaccprimgrupdebit8 + JVmastermodel.DebitTotal;
                                                        ms.UpdateAccgroup(findaccprimgroupDr8);


                                                        //---------------------Update Account group 9 debit----------------------------//
                                                        var findaccprimgroupDr9 = ms.Getaccgroupbyid(findaccprimgroupDr8.Accunderprimarygroupid);
                                                        var oldaccprimgrupdebit9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr9.AccgroupID).FirstOrDefault().DebitAmount;
                                                        var oldaccprimbalance9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr9.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                        if (findaccprimgroupDr9.Accunderprimarygroupid != 0 && findaccprimgroupDr9.Accunderprimarygroupid != null)
                                                        {
                                                            findaccprimgroupDr9.Accgroupbalance = (double)oldaccprimbalance9 + JVmastermodel.DebitTotal;
                                                            findaccprimgroupDr9.DebitAmount = (double)oldaccprimgrupdebit9 + JVmastermodel.DebitTotal;
                                                            ms.UpdateAccgroup(findaccprimgroupDr9);

                                                            //---------------------Update Account group 10 debit----------------------------//
                                                            var findaccprimgroupDr10 = ms.Getaccgroupbyid(findaccprimgroupDr9.Accunderprimarygroupid);
                                                            var oldaccprimgrupdebit10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr10.AccgroupID).FirstOrDefault().DebitAmount;
                                                            var oldaccprimbalance10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr10.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                            if (findaccprimgroupDr10.Accunderprimarygroupid != 0 && findaccprimgroupDr10.Accunderprimarygroupid != null)
                                                            {
                                                                findaccprimgroupDr10.Accgroupbalance = (double)oldaccprimbalance10 + JVmastermodel.DebitTotal;
                                                                findaccprimgroupDr10.DebitAmount = (double)oldaccprimgrupdebit10 + JVmastermodel.DebitTotal;
                                                                ms.UpdateAccgroup(findaccprimgroupDr10);




                                                            }
                                                            else
                                                            {
                                                                findaccprimgroupDr10.Accgroupbalance = (double)oldaccprimbalance10 + JVmastermodel.DebitTotal;
                                                                findaccprimgroupDr10.DebitAmount = (double)oldaccprimgrupdebit10 + JVmastermodel.DebitTotal;
                                                                ms.UpdateAccgroup(findaccprimgroupDr10);

                                                            }


                                                        }
                                                        else
                                                        {
                                                            findaccprimgroupDr9.Accgroupbalance = (double)oldaccprimbalance9 + JVmastermodel.DebitTotal;
                                                            findaccprimgroupDr9.DebitAmount = (double)oldaccprimgrupdebit9 + JVmastermodel.DebitTotal;
                                                            ms.UpdateAccgroup(findaccprimgroupDr9);

                                                        }


                                                    }
                                                    else
                                                    {
                                                        findaccprimgroupDr8.Accgroupbalance = (double)oldaccprimbalance8 + JVmastermodel.DebitTotal;
                                                        findaccprimgroupDr8.DebitAmount = (double)oldaccprimgrupdebit8 + JVmastermodel.DebitTotal;
                                                        ms.UpdateAccgroup(findaccprimgroupDr8);

                                                    }


                                                }
                                                else
                                                {
                                                    findaccprimgroupDr7.Accgroupbalance = (double)oldaccprimbalance7 + JVmastermodel.DebitTotal;
                                                    findaccprimgroupDr7.DebitAmount = (double)oldaccprimgrupdebit7 + JVmastermodel.DebitTotal;
                                                    ms.UpdateAccgroup(findaccprimgroupDr7);

                                                }



                                            }
                                            else
                                            {
                                                findaccprimgroupDr6.Accgroupbalance = (double)oldaccprimbalance6 + JVmastermodel.DebitTotal;
                                                findaccprimgroupDr6.DebitAmount = (double)oldaccprimgrupdebit6 + JVmastermodel.DebitTotal;
                                                ms.UpdateAccgroup(findaccprimgroupDr6);

                                            }

                                        }
                                        else
                                        {
                                            findaccprimgroupDr5.Accgroupbalance = (double)oldaccprimbalance5 + JVmastermodel.DebitTotal;
                                            findaccprimgroupDr5.DebitAmount = (double)oldaccprimgrupdebit5 + JVmastermodel.DebitTotal;
                                            ms.UpdateAccgroup(findaccprimgroupDr5);

                                        }

                                    }
                                    else
                                    {
                                        findaccprimgroupDr4.Accgroupbalance = (double)oldaccprimbalance4 + JVmastermodel.DebitTotal;
                                        findaccprimgroupDr4.DebitAmount = (double)oldaccprimgrupdebit4 + JVmastermodel.DebitTotal;
                                        ms.UpdateAccgroup(findaccprimgroupDr4);

                                    }

                                }
                                else
                                {
                                    findaccprimgroupDr3.Accgroupbalance = (double)oldaccprimbalance3 + JVmastermodel.DebitTotal;
                                    findaccprimgroupDr3.DebitAmount = (double)oldaccprimgrupdebit3 + JVmastermodel.DebitTotal;
                                    ms.UpdateAccgroup(findaccprimgroupDr3);

                                }

                            }
                            else
                            {
                                findaccprimgroupDr.Accgroupbalance = (double)oldaccprimbalance2 + JVmastermodel.DebitTotal;
                                findaccprimgroupDr.DebitAmount = (double)oldaccprimgrupdebit2 + JVmastermodel.DebitTotal;
                                ms.UpdateAccgroup(findaccprimgroupDr);

                            }


                        }
                        else
                        {
                            findaccgroup.Accgroupbalance = (double)oldaccgrupbalance + JVmastermodel.DebitTotal;
                            findaccgroup.DebitAmount = (double)oldaccgrupdebit1 + JVmastermodel.DebitTotal;
                            ms.UpdateAccgroup(findaccgroup);
                        }

                    }
                }





            }
            else if (JVmastermodel.jvchildid3.FirstOrDefault() == 1)
            {
                var oldmasterfind = db.JournalVoucherMaster.Where(m => m.jvmasterid == JVmastermodel.JVMasterID).FirstOrDefault();
                var oldchildfind = db.JournalVoucherChild.Where(m => m.JVMasterID == oldmasterfind.jvmasterid).ToList();
                var oldpayemntmode = db.JournalVoucherChild.Where(m => m.JVMasterID == oldmasterfind.jvmasterid && m.DebitAmount != 0 && m.DrCrID == 1).FirstOrDefault();


                //-------update vouchermaster----------//
                oldmasterfind.VoucherDate = JVmastermodel.VoucherDate;
                oldmasterfind.LongNarration = JVmastermodel.LongNarration;
                oldmasterfind.CurrencyID = 1;
                oldmasterfind.DebitTotal = (double)JVmastermodel.DebitTotal;
                oldmasterfind.CreditTotal = (double)JVmastermodel.CreditTotal;
                oldmasterfind.VoucherOtherDetails = JVmastermodel.VoucherOtherDetails;

                db.Entry(oldmasterfind).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                //-------update paymentmode----------//
                oldpayemntmode.DebitAmount = (double)JVmastermodel.CreditTotal;
                oldpayemntmode.CreditAmount = 0.00;
                db.Entry(oldpayemntmode).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();




                if (JVmastermodel.deletedchild != null)
                {

                    for (int i = 0; i < JVmastermodel.deletedchild.Count; i++)
                    {
                        //List<int> ttas = JVmastermodel.deletedchild;

                        //int findid = db.JournalVoucherChild.Where(m =>k m.JVchildID == ttas[i]).FirstOrDefault().JVchildID;

                        //var oldchildfind2 = db.JournalVoucherChild.Find(db.JournalVoucherChild.Where(m => m.JVMasterID == JVmastermodel.deletedchild[i]).FirstOrDefault().JVchildID);
                        var olddfd = db.JournalVoucherChild.Find(JVmastermodel.deletedchild[i]);
                        db.JournalVoucherChild.Remove(olddfd);
                        db.SaveChanges();

                    }
                }
                if (JVmastermodel.newchild2 != null && JVmastermodel.newchild2.Count > 0)
                {
                    for (int i = 0; i < JVmastermodel.newchild2.Count; i++)
                    {
                        var jvouchermasterid = JVmastermodel.JVMasterID;

                        double creditamount;
                        double debitamount = 0.00;
                        debitamount = JVmastermodel.DebitAmount[i];
                        creditamount = JVmastermodel.CreditAmount[i];

                        /* if (JVmastermodel.CreditAmount[i] == 0.00)
                         {

                         }

                        else if (JVmastermodel.DrCrID[i] == 2)
                        {
                            creditamount = JVmastermodel.CreditAmount[i];
                            debitamount = 0.00;
                        }*/
                        JournalVoucherChild jvchild = new JournalVoucherChild
                        {

                            JVMasterID = jvouchermasterid,
                            DrCrID = JVmastermodel.DrCrID[i],
                            AccountID = JVmastermodel.AccountID[i],
                            CreditAmount = JVmastermodel.CreditAmount[i],
                            FcRate = JVmastermodel.FcRate[i],
                            CurrencyID = JVmastermodel.CurrencyID[i],
                            DebitAmount = JVmastermodel.DebitAmount[i],
                            ShortNarration = JVmastermodel.ShortNarration[i],
                            JvDate = JVmastermodel.VoucherDate

                        };
                        db.JournalVoucherChild.Add(jvchild);
                        db.SaveChanges();

                    }



                }




                return RedirectToAction("RecieptVoucher");

            }




            return RedirectToAction("RecieptVoucher");

        }
        #endregion Receipt Voucher

        public ActionResult ReportJVVoucher(int? MasterID, int? TypeID)
        {
            ReportDocument report = new ReportDocument();

            report.Load(Server.MapPath("~/Reports/VoucherRpt.rpt"));

            report.SetParameterValue("@vouchermasterid", MasterID);
            report.SetParameterValue("@typeid", TypeID);

            var ReportData = db.Database.SqlQuery<JournalVMasterReportViewModel>("Sp_VoucherRpt @vouchermasterid, @typeid",
            new SqlParameter("vouchermasterid", MasterID),
            new SqlParameter("typeid", TypeID)).ToList();

            report.SetDataSource(ReportData);

            var stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }


    }
}