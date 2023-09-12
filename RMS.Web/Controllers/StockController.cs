using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Shared;
using RMS.Web.Models;
using RMS.Web.Models.ViewModels;
using RMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace RMS.Web.Controllers
{
    public class StockController : Controller
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        // GET: Stock
        MasterServices ms = new MasterServices();


        #region StockReport
        public ActionResult Index()
        {
            try
            {


                var stocklist = db.Stock.ToList();
                ViewBag.ItemLists = new SelectList(db.Items, "ItemID", "ItemName");
                ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName");


                return View();
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }
        }

        [HttpPost]
        public ActionResult Index(int txtItems, int LocationID, DateTime startdate, DateTime enddate)
        {
            try
            {
                VoucherMasterViewModel vmodel = new VoucherMasterViewModel();
                vmodel = ms.StockLedger(txtItems,LocationID,startdate,enddate);
                vmodel.LocationID = LocationID;



                return PartialView("stockledger", vmodel);

            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }
        }
        [HttpGet]
        public ActionResult Gettransbystock(int id)
        {
            //var trans = db.Transactions.Where(x=>x.ItemID == id).ToList(); 
            var trans = db.VoucherChild.Where(x => x.ItemID == id).ToList();
            VoucherMasterViewModel vmodel = new VoucherMasterViewModel();
            var itemid = id;

            List<VoucherMaster> accounts = new List<VoucherMaster>();
            for (int i = 0; i < trans.Count; i++)
            {
                var transid = trans[i].VoucherMasterID;
                var master = db.VoucherMaster.Where(m => m.vouchermasterid == transid).ToList();
                accounts.AddRange(master);
            }

            vmodel.VoucherChildlist = trans;
            vmodel.Vouchermasterlist = accounts;

            if (itemid == 9 || itemid == 10 || itemid == 11)
            {
                ViewBag.name = "testing";
                ViewBag.balance = 0;

            }
            else
            {
                var itemname = db.Items.Where(m => m.ItemID == itemid).FirstOrDefault().ItemName;
                var itemunit = db.Items.Where(m => m.ItemID == itemid).FirstOrDefault().ItemUnitID;
                var itemunitname = db.Units.Where(m => m.UnitID == itemunit).FirstOrDefault().UnitName;
                var stockbalance = db.Stock.Where(m => m.ItemID == itemid).FirstOrDefault().Quantity;

                ViewBag.name = itemname;
                ViewBag.balance = stockbalance;
                ViewBag.itemunit = itemunitname;

            }

            if (trans.Count > 0)
            {
                return View(vmodel);
            }
            else
            {
                ViewBag.error = "No transactions occured on this selected item";
                return View();
            }



        }
        [HttpGet]
        public ActionResult StockReport()
        {



            return View();
        }
        [HttpPost]
        public ActionResult StockReport(DateTime todate)
        {
            var vouchers = db.VoucherMaster.Where(m => m.VoucherCreateDate <= todate).ToList();
            List<VoucherChild> vouceheschild = new List<VoucherChild>();
            for (int i = 0; i < vouchers.Count; i++)
            {
                var transid = vouchers[i].vouchermasterid;
                var master = db.VoucherChild.Where(m => m.VoucherMasterID == transid).ToList();
                vouceheschild.AddRange(master);
            }




            var items = db.Items.ToList();
            for (int i = 0; i < items.Count; i++)
            {
                var itemidd = items[i].ItemID;
                var trans = db.VoucherChild.Where(x => x.ItemID == itemidd).ToList();

            }


            //var itemid = id;

            //List<VoucherMaster> accounts = new List<VoucherMaster>();
            //for (int i = 0; i < trans.Count; i++)
            //{
            //    var transid = trans[i].VoucherMasterID;
            //    var master = db.VoucherMaster.Where(m => m.vouchermasterid == transid).ToList();
            //    accounts.AddRange(master);
            //}

            //vmodel.VoucherChildlist = trans;
            //vmodel.Vouchermasterlist = accounts;



            return PartialView("report");
        }

        #endregion StockReport
        //-------------------------StockTransfer Voucher--------------------------//
        #region StockTransferVoucher

        public ActionResult StockTransfer()
        {
            ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName");

            ViewBag.Accountname = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
            ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
            ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName");
            ViewBag.ordernumber = db.VoucherMaster.Count() + 1;
            VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();

            vmmodel.vouchertypeid = 12;

            return View(vmmodel);
        }
        public ActionResult StockTransferHistory()
        {
            var vouchercount = db.VoucherMaster.Where(x => x.VoucherTypeID == 12).Count();
            VoucherMasterViewModel stocktransfhist = new VoucherMasterViewModel();
            stocktransfhist.vouchertypeid = 12;
            var carrierexpenseaccount = db.Preferences.Where(m => m.PreferenceID == 1).FirstOrDefault().Preferencevalue;

            if (vouchercount > 0)
            {
                stocktransfhist.Vouchermasterlist = db.VoucherMaster.Where(x => x.VoucherTypeID == 12 && x.Partyid_Accountid != carrierexpenseaccount).ToList();
                //var totalsale = db.VoucherMaster.Where(m => m.VoucherTypeID == 12).Sum(m => m.VoucherFinalTotal);

            }
            else
            {
                stocktransfhist.vouchertypeid =12;
            }


            return View(stocktransfhist);
        }

        public ActionResult EditStockVoucher(int id)
        {
            if (id != 0)
            {
                var masterid = db.VoucherMaster.Where(m => m.vouchermasterid == id).FirstOrDefault().VoucherNum_BillNum;



                var vouchermaster = db.VoucherMaster.Find(masterid);
                var voucherhild = db.VoucherChild.Where(m => m.VoucherMasterID == id).ToList();
                List<BillSundryChild> billsundrychild = new List<BillSundryChild>();
                if (vouchermaster.vouchermasterid != 0)
                {
                    billsundrychild = db.BillSundryChild.Where(m => m.Vouchermasterid == vouchermaster.vouchermasterid).ToList();

                }


                ViewBag.Accountname = new SelectList(db.Account, "AccountID", "AccountName", vouchermaster.Partyid_Accountid);
                ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
                ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID);
                ViewBag.MaterialCentres2 = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID2);

                ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
                ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName", vouchermaster.BillSundryId);

                ViewBag.vouchernumber = vouchermaster.VoucherNum_BillNum;
                ViewBag.VoucherCreateDate = vouchermaster.VoucherCreateDate;
                ViewBag.Narration = vouchermaster.Narration;

                double billsamount = 0.00;
                //double billsamount2 = 0.00;
                if (vouchermaster.BillSundryAmount > 0)
                {
                    billsamount = vouchermaster.BillSundryAmount;
                }
                else if (vouchermaster.BillSundryAmount <= 0)
                {
                    billsamount = 0.00;
                }
                VoucherMasterViewModel masterview = new VoucherMasterViewModel
                {
                    VoucherNumber = (int)vouchermaster.vouchermasterid,
                    VoucherCreateDate = vouchermaster.VoucherCreateDate,
                    Partyid = vouchermaster.Partyid_Accountid,
                    Narration = vouchermaster.Narration,
                    BillSundryAmount2 = (int)billsamount,
                    FinalTotal = vouchermaster.VoucherFinalTotal,
                    VoucherType_IDddddd = vouchermaster.VoucherTypeID,
                    DrCrType = vouchermaster.DrCrType,
                    VoucherChildlist = voucherhild,
                    modify = 1,
                    vouchertypeid = vouchermaster.VoucherTypeID,
                    BillSundrychildlist = billsundrychild,
                    LocationID = vouchermaster.LocationID,
                    LocationID2 = vouchermaster.LocationID2


                };









                return View("~/Views/Stock/StockTransfer.cshtml", masterview);
            }
            else
            {
                return RedirectToAction("StockTransfer");
            }
        }


        [HttpPost]

        public ActionResult SaveStockTransfer(VoucherMasterViewModel vouchermaster)
        {

            if (vouchermaster.modify == 0)
            {
                try
                {
                    var vouchermasterid2 = vouchermaster.VoucherNumber;
                    //var itemid = vouchermaster.ItemID; 
                    //var orderMasterId = 0;
                    int billsid = 0;
                    int billsundrynumber = db.BillSundryChild.Count() + 1;
                    double billsundrytotalamount = 0;
                    double billsamount = 0;

                    //if (vouchermaster.BillSundryId == 0) 
                    //{
                    //    billsid = 3;
                    //}
                    //else if (vouchermaster.BillSundryId != 0)
                    //{
                    //    billsid = vouchermaster.BillSundryId;
                    //}

                    //---------------------add billsundry----------------//
                    if (vouchermaster.BillSundryID != null && vouchermaster.BillSundryID.Count > 0)
                    {
                        for (int i = 0; i < vouchermaster.BillSundryAmount.Count; i++)
                        {
                            billsundrytotalamount += vouchermaster.BillSundryAmount[i];
                        }

                        for (int i = 0; i < vouchermaster.BillSundryID.Count; i++)
                        {
                            BillSundryChild billSundryChild = new BillSundryChild
                            {
                                BillSundryID = vouchermaster.BillSundryID[i],
                                BillSundryAmount = vouchermaster.BillSundryAmount[i],
                                Vouchermasterid = vouchermasterid2,
                                BillSundryNumber = billsundrynumber
                            };
                            db.BillSundryChild.Add(billSundryChild);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        billsundrynumber = 0;
                        billsundrytotalamount = 0;
                    }

                    //---------------------Add VoucherChild---------------//
                    for (int i = 0; i < vouchermaster.ItemID.Count; i++)
                    {
                        if (vouchermaster.vouchertypeid == 12)
                        {
                            VoucherChild voucherchild = new VoucherChild
                            {
                                //VoucherchildID = 0,
                                ItemID = vouchermaster.ItemID[i],
                                Quantity = vouchermaster.Quantity[i],
                                Unitid = vouchermaster.Unitid[i],
                                ItemPrice = vouchermaster.ItemPrice[i],
                                TotalAmount = vouchermaster.TotalAmount[i],
                                BillsundryID = billsundrynumber,
                                VoucherMasterID = vouchermasterid2,
                                VoucherTypeID = 12,



                            };
                            db.VoucherChild.Add(voucherchild);
                            db.SaveChanges();

                        }
                        else if (vouchermaster.vouchertypeid == 13)
                        {
                            VoucherChild voucherchild = new VoucherChild
                            {
                                //VoucherchildID = 0,
                                ItemID = vouchermaster.ItemID[i],
                                Quantity = vouchermaster.Quantity[i],
                                Unitid = vouchermaster.Unitid[i],
                                ItemPrice = 0,
                                TotalAmount = 0,
                                BillsundryID = billsundrynumber,
                                VoucherMasterID = vouchermasterid2,
                                VoucherTypeID = 13


                            };
                            db.VoucherChild.Add(voucherchild);
                            db.SaveChanges();
                        }


                        //Transactions trans = new Transactions
                        //{
                        //    //TransactionID = 0,
                        //    ItemID = vouchermaster.ItemID[i],
                        //    Quantity = (-1) * vouchermaster.Quantity[i],
                        //    TransDate = DateTime.Now,
                        //    PayTypeID = 1,
                        //    ItemPrice = vouchermaster.ItemPrice[i],
                        //    TransType = 1
                        //};
                        //db.Transactions.Add(trans);
                        //db.SaveChanges();
                        OrderServices orderServices = new OrderServices();

                        try
                        {
                            //---update Stonks---//
                            if (vouchermaster.vouchertypeid == 12)
                            {
                                var StockValues = orderServices.GetStockById(vouchermaster.ItemID[i]);
                                StockValues.Quantity = StockValues.Quantity - vouchermaster.Quantity[i];
                                orderServices.UpdateStock(StockValues);
                            }
                            else if (vouchermaster.vouchertypeid == 9)
                            {
                                var StockValues = orderServices.GetStockById(vouchermaster.ItemID[i]);
                                StockValues.Quantity = StockValues.Quantity + vouchermaster.Quantity[i];
                                orderServices.UpdateStock(StockValues);
                            }
                            //---update Stonks---//

                        }
                        catch (Exception)
                        {

                            var StockValues = orderServices.GetStockById(vouchermaster.ItemID[i]);
                            StockValues.Quantity = 0;
                            orderServices.UpdateStock(StockValues);
                        }

                    }

                    //---------------------Add VoucherMaster---------------//

                    if (vouchermaster.vouchertypeid == 12)
                    {
                        var carrierexpenseacount = db.Preferences.Where(m => m.PreferenceID == 1).FirstOrDefault().Preferencevalue;

                        //-----------Stock Transfer To Transit-----------------//
                        VoucherMaster stocktransferfrom = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = vouchermaster.Partyid,
                            Narration = vouchermaster.Narration,
                            BillSundryId = billsundrynumber,
                            BillSundryAmount = billsundrytotalamount,
                            VoucherFinalTotal = vouchermaster.FinalTotal,
                            VoucherTypeID = 12,
                            DrCrType = 1,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID,
                            LocationID2 = vouchermaster.LocationID2



                        };
                        db.VoucherMaster.Add(stocktransferfrom);
                        db.SaveChanges();
                        ////-----------Stock Receive In Transit-----------------//

                        //VoucherMaster STrecieveintransit = new VoucherMaster
                        //{
                        //    //VoucherNum_BillNum = 0,
                        //    VoucherCreateDate = vouchermaster.VoucherCreateDate,
                        //    Partyid_Accountid = vouchermaster.Partyid,
                        //    Narration = vouchermaster.Narration,
                        //    BillSundryId = billsundrynumber,
                        //    BillSundryAmount = billsundrytotalamount,
                        //    VoucherFinalTotal = vouchermaster.FinalTotal,
                        //    VoucherTypeID = 12,
                        //    DrCrType = 2,
                        //    vouchermasterid = vouchermasterid2,
                        //    LocationID = vouchermaster.LocationID,
                        //    LocationID2 = vouchermaster.LocationID2



                        //};
                        //db.VoucherMaster.Add(STrecieveintransit);
                        //db.SaveChanges();


                        //-----------Carreier Expenses-----------------//
                        VoucherMaster carreirmaster = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = carrierexpenseacount,
                            Narration = vouchermaster.Narration,
                            BillSundryId = billsundrynumber,
                            BillSundryAmount = billsundrytotalamount,
                            VoucherFinalTotal = vouchermaster.FinalTotal,
                            VoucherTypeID = 12,
                            DrCrType = 2,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID,
                            LocationID2 = vouchermaster.LocationID2



                        };
                        db.VoucherMaster.Add(carreirmaster);
                        db.SaveChanges();


                    }
                    else if (vouchermaster.vouchertypeid == 13)
                    {
                        VoucherMaster stocktransferTo = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = vouchermaster.Partyid,
                            Narration = vouchermaster.Narration,
                            BillSundryId = billsundrynumber,
                            BillSundryAmount = billsundrytotalamount,
                            VoucherFinalTotal = 0,
                            VoucherTypeID = 13,
                            DrCrType = 2,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID2,
                            LocationID2 = vouchermaster.LocationID

                        };
                        db.VoucherMaster.Add(stocktransferTo);
                        db.SaveChanges();
                    }

                    return RedirectToAction("StockTransfer");
                }
                catch (Exception e)
                {
                    ViewBag.message = e.ToString();
                    return View("~/Views/Shared/Error.cshtml");

                }
                //var oldbalanceaccprgroupid = db.AccGroup.Where(m=>m.)

            }
            else if (vouchermaster.modify == 1)
            {
                int billsid = 0;

                if (vouchermaster.vouchertypeid == 12)
                {
                    var oldstockmaster1 = db.VoucherMaster.Where(m => m.vouchermasterid == vouchermaster.VoucherNumber && m.DrCrType == 1).FirstOrDefault();
                    var oldstockmaster2 = db.VoucherMaster.Where(m => m.vouchermasterid == vouchermaster.VoucherNumber && m.DrCrType == 2).FirstOrDefault();

                    var oldchildfind = db.VoucherChild.Where(m => m.VoucherMasterID == oldstockmaster1.vouchermasterid).ToList();
                    int billsundrynumber = db.BillSundryChild.Count() + 1;


                    double billsamount = 0;
                    double billsundrytotalamount = 0;
                    //-------update billsundrychild----------//
                    if (vouchermaster.deletebillsundrychild != null)
                    {
                        for (int i = 0; i < vouchermaster.deletebillsundrychild.Count; i++)
                        {
                            //List<int> ttas = JVmastermodel.deletedchild;

                            //int findid = db.JournalVoucherChild.Where(m =>k m.JVchildID == ttas[i]).FirstOrDefault().JVchildID;

                            //var oldchildfind2 = db.JournalVoucherChild.Find(db.JournalVoucherChild.Where(m => m.JVMasterID == JVmastermodel.deletedchild[i]).FirstOrDefault().JVchildID);


                            var olddbillsundrychild = db.BillSundryChild.Find(vouchermaster.deletebillsundrychild[i]);
                            olddbillsundrychild.BillSundryNumber = 0;
                            olddbillsundrychild.Vouchermasterid = 0;
                            db.Entry(olddbillsundrychild).State = System.Data.Entity.EntityState.Modified;
                            //db.BillSundryChild.Remove(olddbillsundrychild);

                            db.SaveChanges();

                            billsundrytotalamount += olddbillsundrychild.BillSundryAmount;



                        }

                        oldstockmaster1.BillSundryAmount = oldstockmaster1.BillSundryAmount - billsundrytotalamount;
                        oldstockmaster1.BillSundryId = billsundrynumber;

                    }
                    if (vouchermaster.newbillsundrychild2 != null && vouchermaster.newbillsundrychild2.Count > 0)
                    {
                        for (int i = 0; i < vouchermaster.newbillsundrychild2.Count; i++)
                        {
                            var vouchermasterid2 = vouchermaster.VoucherNumber;


                            billsundrytotalamount += vouchermaster.BillSundryAmount[i];

                            BillSundryChild billSundryChild = new BillSundryChild
                            {
                                BillSundryID = vouchermaster.BillSundryID[i],
                                BillSundryAmount = vouchermaster.BillSundryAmount[i],
                                Vouchermasterid = vouchermasterid2,
                                BillSundryNumber = billsundrynumber
                            };
                            db.BillSundryChild.Add(billSundryChild);
                            db.SaveChanges();

                            //---Stockss---//
                            //var oldquantity = (int)db.Stock.Find(vouchermaster.ItemID[i]).Quantity;
                            // int? oldquantity = db.Stock.Where(m=>m.ItemID== vouchermaster.ItemID[i]).FirstOrDefault().Quantity;
                            //OrderServices orderServices = new OrderServices();
                            //var StockValues = orderServices.GetStockById(vouchermaster.ItemID[i]);
                            //StockValues.Quantity = StockValues.Quantity + vouchermaster.Quantity[i];
                            //orderServices.UpdateStock(StockValues);
                            // orderServices.UpdateStock(StockValues);
                            //---Stockss---//

                        }

                        oldstockmaster1.BillSundryAmount = oldstockmaster1.BillSundryAmount + billsundrytotalamount;

                        oldstockmaster1.BillSundryId = billsundrynumber;

                    }


                    //-------update vouchermaster----------//
                    oldstockmaster1.Narration = vouchermaster.Narration;
                    oldstockmaster1.VoucherFinalTotal = vouchermaster.FinalTotal;
                    oldstockmaster1.Partyid_Accountid = vouchermaster.Partyid;

                    oldstockmaster1.LocationID = vouchermaster.LocationID;
                    oldstockmaster1.LocationID2 = vouchermaster.LocationID2;

                    db.Entry(oldstockmaster1).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    oldstockmaster2.VoucherFinalTotal = vouchermaster.FinalTotal;
                    oldstockmaster2.LocationID = vouchermaster.LocationID2;
                    oldstockmaster2.LocationID2 = vouchermaster.LocationID;
                    db.Entry(oldstockmaster2).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var oldstockmaster1 = db.VoucherMaster.Where(m => m.vouchermasterid == vouchermaster.VoucherNumber && m.DrCrType == 2).FirstOrDefault();
                    //var oldstockmaster2 = db.VoucherMaster.Where(m => m.vouchermasterid == vouchermaster.VoucherNumber && m.DrCrType == 2).FirstOrDefault();

                    var oldchildfind = db.VoucherChild.Where(m => m.VoucherMasterID == oldstockmaster1.vouchermasterid).ToList();
                    int billsundrynumber = db.BillSundryChild.Count() + 1;


                    double billsamount = 0;
                    double billsundrytotalamount = 0;
                    //-------update billsundrychild----------//
                    if (vouchermaster.deletebillsundrychild != null)
                    {
                        for (int i = 0; i < vouchermaster.deletebillsundrychild.Count; i++)
                        {
                            //List<int> ttas = JVmastermodel.deletedchild;

                            //int findid = db.JournalVoucherChild.Where(m =>k m.JVchildID == ttas[i]).FirstOrDefault().JVchildID;

                            //var oldchildfind2 = db.JournalVoucherChild.Find(db.JournalVoucherChild.Where(m => m.JVMasterID == JVmastermodel.deletedchild[i]).FirstOrDefault().JVchildID);


                            var olddbillsundrychild = db.BillSundryChild.Find(vouchermaster.deletebillsundrychild[i]);
                            olddbillsundrychild.BillSundryNumber = 0;
                            olddbillsundrychild.Vouchermasterid = 0;
                            db.Entry(olddbillsundrychild).State = System.Data.Entity.EntityState.Modified;
                            //db.BillSundryChild.Remove(olddbillsundrychild);

                            db.SaveChanges();

                            billsundrytotalamount += olddbillsundrychild.BillSundryAmount;



                        }

                        oldstockmaster1.BillSundryAmount = oldstockmaster1.BillSundryAmount - billsundrytotalamount;
                        oldstockmaster1.BillSundryId = billsundrynumber;

                    }
                    if (vouchermaster.newbillsundrychild2 != null && vouchermaster.newbillsundrychild2.Count > 0)
                    {
                        for (int i = 0; i < vouchermaster.newbillsundrychild2.Count; i++)
                        {
                            var vouchermasterid2 = vouchermaster.VoucherNumber;


                            billsundrytotalamount += vouchermaster.BillSundryAmount[i];

                            BillSundryChild billSundryChild = new BillSundryChild
                            {
                                BillSundryID = vouchermaster.BillSundryID[i],
                                BillSundryAmount = vouchermaster.BillSundryAmount[i],
                                Vouchermasterid = vouchermasterid2,
                                BillSundryNumber = billsundrynumber
                            };
                            db.BillSundryChild.Add(billSundryChild);
                            db.SaveChanges();

                            //---Stockss---//
                            //var oldquantity = (int)db.Stock.Find(vouchermaster.ItemID[i]).Quantity;
                            // int? oldquantity = db.Stock.Where(m=>m.ItemID== vouchermaster.ItemID[i]).FirstOrDefault().Quantity;
                            //OrderServices orderServices = new OrderServices();
                            //var StockValues = orderServices.GetStockById(vouchermaster.ItemID[i]);
                            //StockValues.Quantity = StockValues.Quantity + vouchermaster.Quantity[i];
                            //orderServices.UpdateStock(StockValues);
                            // orderServices.UpdateStock(StockValues);
                            //---Stockss---//

                        }

                        oldstockmaster1.BillSundryAmount = oldstockmaster1.BillSundryAmount + billsundrytotalamount;

                        oldstockmaster1.BillSundryId = billsundrynumber;

                    }


                    //-------update vouchermaster----------//
                    oldstockmaster1.Narration = vouchermaster.Narration;
                    oldstockmaster1.VoucherFinalTotal = vouchermaster.FinalTotal;
                    oldstockmaster1.Partyid_Accountid = vouchermaster.Partyid;

                    oldstockmaster1.LocationID = vouchermaster.LocationID;
                    oldstockmaster1.LocationID2 = vouchermaster.LocationID2;

                    db.Entry(oldstockmaster1).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //oldstockmaster2.VoucherFinalTotal = vouchermaster.FinalTotal;
                    //oldstockmaster2.LocationID = vouchermaster.LocationID2;
                    //oldstockmaster2.LocationID2 = vouchermaster.LocationID;
                    //db.Entry(oldstockmaster2).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                }



                //-------update voucherchild----------//

                if (vouchermaster.deletedchild != null)
                {

                    for (int i = 0; i < vouchermaster.deletedchild.Count; i++)
                    {
                        //List<int> ttas = JVmastermodel.deletedchild;

                        //int findid = db.JournalVoucherChild.Where(m =>k m.JVchildID == ttas[i]).FirstOrDefault().JVchildID;

                        //var oldchildfind2 = db.JournalVoucherChild.Find(db.JournalVoucherChild.Where(m => m.JVMasterID == JVmastermodel.deletedchild[i]).FirstOrDefault().JVchildID);
                        var olddchild = db.VoucherChild.Find(vouchermaster.deletedchild[i]);
                        db.VoucherChild.Remove(olddchild);
                        db.SaveChanges();

                        //---Stockss---//
                        //var oldquantity = (int)db.Stock.Find(vouchermaster.ItemID[i]).Quantity;
                        // int? oldquantity = db.Stock.Where(m=>m.ItemID== vouchermaster.ItemID[i]).FirstOrDefault().Quantity;
                        OrderServices orderServices = new OrderServices();

                        var deleteditemid = olddchild.ItemID;
                        var deletedquantity = olddchild.Quantity;
                        var StockValues = orderServices.GetStockById(deleteditemid);
                        StockValues.Quantity = StockValues.Quantity + deletedquantity;
                        orderServices.UpdateStock(StockValues);
                        //---Stockss---//

                    }
                }
                if (vouchermaster.newchild2 != null && vouchermaster.newchild2.Count > 0)
                {
                    for (int i = 0; i < vouchermaster.newchild2.Count; i++)
                    {
                        var vouchermasterid = vouchermaster.VoucherNumber;


                        /* if (JVmastermodel.CreditAmount[i] == 0.00)
                         {

                         }

                        else if (JVmastermodel.DrCrID[i] == 2)
                        {
                            creditamount = JVmastermodel.CreditAmount[i];
                            debitamount = 0.00;
                        }*/
                        VoucherChild voucherchild = new VoucherChild
                        {
                            //VoucherchildID = 0,
                            ItemID = vouchermaster.ItemID[i],
                            Quantity = vouchermaster.Quantity[i],
                            Unitid = vouchermaster.Unitid[i],
                            ItemPrice = 0,
                            TotalAmount = 0,
                            BillsundryID = billsid,
                            VoucherMasterID = vouchermasterid


                        };
                        db.VoucherChild.Add(voucherchild);
                        db.SaveChanges();


                        //---Stockss---//
                        //var oldquantity = (int)db.Stock.Find(vouchermaster.ItemID[i]).Quantity;
                        // int? oldquantity = db.Stock.Where(m=>m.ItemID== vouchermaster.ItemID[i]).FirstOrDefault().Quantity;
                        OrderServices orderServices = new OrderServices();
                        var StockValues = orderServices.GetStockById(vouchermaster.ItemID[i]);
                        StockValues.Quantity = StockValues.Quantity - vouchermaster.Quantity[i];
                        orderServices.UpdateStock(StockValues);
                        // orderServices.UpdateStock(StockValues);
                        //---Stockss---//

                    }



                }
                return RedirectToAction("StockTransfer");

            }
            else
            {
                return RedirectToAction("StockTransfer");

            }







        }
        #endregion StockTransferVoucher

        #region RecieveFromVoucher
        public ActionResult RecieveFromVoucher()
        {
            ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName");


            ViewBag.Accountname = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
            ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
            ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName");

            ViewBag.ordernumber = db.VoucherMaster.Count() + 1;
            VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();

            vmmodel.vouchertypeid = 13;

            return View("~/Views/Stock/StockTransfer.cshtml", vmmodel);
        }
        public ActionResult RecieveFromVoucherHistory()
        {
            var vouchercount = db.VoucherMaster.Where(x => x.VoucherTypeID == 13).Count();
            VoucherMasterViewModel stocktransfhist = new VoucherMasterViewModel();
            stocktransfhist.vouchertypeid = 13;
            if (vouchercount > 0)
            {
                stocktransfhist.Vouchermasterlist = db.VoucherMaster.Where(x => x.VoucherTypeID == 13 && x.Partyid_Accountid != 1059).ToList();
                var totalsale = db.VoucherMaster.Where(m => m.VoucherTypeID == 13).Sum(m => m.VoucherFinalTotal);
                ViewBag.sales = totalsale;

            }
            else
            {
                stocktransfhist.vouchertypeid = 13;
                ViewBag.sales = null;
            }


            return View("~/Views/Stock/StockTransferHistory.cshtml", stocktransfhist);
        }



        #endregion RecieveFromVoucher

        public ActionResult ReportStockVoucher(int? MasterID, int? TypeID)
        {
            ReportDocument report = new ReportDocument();

            report.Load(Server.MapPath("~/Reports/StockVoucherRpt.rpt"));

            report.SetParameterValue("@vouchermasterid", MasterID);
            report.SetParameterValue("@typeid", TypeID);

            var ReportData = db.Database.SqlQuery<StockVoucherViewModel>("Sp_Stockvoucher @vouchermasterid, @typeid",
            new SqlParameter("vouchermasterid", MasterID),
            new SqlParameter("typeid", TypeID)).ToList();

            report.SetDataSource(ReportData);

            var stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult ReportStockReceiveVoucher(int? MasterID, int? TypeID)
        {
            ReportDocument report = new ReportDocument();

            report.Load(Server.MapPath("~/Reports/StockVoucherReceiveRpt.rpt"));

            report.SetParameterValue("@vouchermasterid", MasterID);
            report.SetParameterValue("@typeid", TypeID);

            var ReportData = db.Database.SqlQuery<StockVoucherViewModel>("Sp_Stockvoucher @vouchermasterid, @typeid",
            new SqlParameter("vouchermasterid", MasterID),
            new SqlParameter("typeid", TypeID)).ToList();

            report.SetDataSource(ReportData);

            var stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

    }
}