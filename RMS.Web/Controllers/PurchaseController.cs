using RMS.Web.Models;
using RMS.Web.Models.ViewModels;
using RMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMS.Web.Controllerssdsdasd
{
    public class PurchaseController : Controller
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        MasterServices accountsercvices = new MasterServices();
        MasterServices ms = new MasterServices();

        /*---------------------------------Not IN USE--------------------------------*/

        #region notinuse
        //public ActionResult Index(orderm om)
        //{

        //    ViewData["SupplierID"] = new SelectList(db.Suppliers, "SupplierID", "SupplierName", om.CustomerID);
        //    ViewBag.GetItems = new SelectList(db.Items, "ItemID", "ItemName");
        //    ViewBag.GetPayTypes = new SelectList(db.PaymentTypes, "PaymentTypeID", "PaymentTypeName");
        //    ViewBag.OrderNumbr = db.ordermaster.Count() + 1;
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult PurchaseItem(OrderMasterViewModel OrderMasterViewModel)
        //{
        //    if (OrderMasterViewModel.ItemID != null)
        //    {
        //        var orderid = OrderMasterViewModel.OrderNumber;
        //        var itemid = OrderMasterViewModel.ItemID;
        //        //var orderMasterId = 0;

        //        for (int i = 0; i < OrderMasterViewModel.ItemID.Count; i++)
        //        {
        //            OrderChild child = new OrderChild
        //            {
        //                OrderChildID = 0,
        //                OrderMasterID = orderid,
        //                ItemID = OrderMasterViewModel.ItemID[i],
        //                UnitPrice = OrderMasterViewModel.UnitPrice[i],
        //                Discount = OrderMasterViewModel.Discount[i],
        //                Quantity = OrderMasterViewModel.Quantity[i],
        //                Total = OrderMasterViewModel.Total[i],
        //            };
        //            db.OrderChild.Add(child);
        //            db.SaveChanges();

        //            Transactions trans = new Transactions
        //            {
        //                TransactionID = 0,
        //                ItemID = OrderMasterViewModel.ItemID[i],
        //                Quantity = (-1) * OrderMasterViewModel.Quantity[i],
        //                TransDate = DateTime.Now,
        //                PayTypeID = OrderMasterViewModel.PaymentID
        //            };
        //            db.Transactions.Add(trans);
        //            db.SaveChanges();

        //            var oldquantity = db.Stock.Find(itemid[i]).Quantity;

        //            OrderServices orderServices = new OrderServices();
        //            var StockValues = orderServices.GetStockById(itemid[i]);
        //            StockValues.Quantity = oldquantity + OrderMasterViewModel.Quantity[i];
        //            orderServices.UpdateStock(StockValues);

        //        }
        //        ordermaster ordermaster = new ordermaster
        //        {
        //            //OrderID = 0,
        //            PaymentID = OrderMasterViewModel.PaymentID,
        //            CustomerID = OrderMasterViewModel.CustomerID,
        //            OrderNumber = OrderMasterViewModel.OrderNumber,
        //            OrderDate = OrderMasterViewModel.OrderDate,
        //            FinalTotal = OrderMasterViewModel.FinalTotal,
        //            OrderType = "Purchase"
        //        };
        //        db.ordermaster.Add(ordermaster);
        //        db.SaveChanges();


        //    }
        //    return RedirectToAction("Index");
        //}
        //public JsonResult Getquantitybyitem(int ItemID)
        //{
        //    var getitem = db.Stock.Where(i => i.ItemID == ItemID).FirstOrDefault().Quantity;

        //    int quantity = (int)getitem;
        //    return Json(quantity, JsonRequestBehavior.AllowGet);
        //}
        #endregion notinuse


        /*---------------------------------New Purhcase--------------------------------*/
        #region Purhcase

        public ActionResult AddPVoucher()
        {

            ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
            ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
            var nullbills = db.BillSundry.Where(m => m.BiilSundryID == 3).FirstOrDefault().BillSundryName;
            ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName", nullbills);

            ViewBag.ordernumber = (int)db.VoucherMaster.Count() + 1;
            ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName");

            VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();
            vmmodel.vouchertypeid = 2;

            return View("~/Views/Sales/SPVoucher.cshtml", vmmodel);
        }
        public ActionResult ModifyPvoucher()
        {
            VoucherMasterViewModel purchasehist = new VoucherMasterViewModel();
            var vouchercount = db.VoucherMaster.Where(x => x.VoucherTypeID == 2).Count();

            if (vouchercount > 0)
            {
                purchasehist.Vouchermasterlist = db.VoucherMaster.Where(m => m.VoucherTypeID == 2 && m.Partyid_Accountid != 1059).ToList();
                purchasehist.vouchertypeid = 2;
                var totalsale = db.VoucherMaster.Where(m => m.VoucherTypeID == 2).Sum(m => m.VoucherFinalTotal);
                ViewBag.sales = totalsale;

            }
            else
            {
                purchasehist.vouchertypeid = 2;
                ViewBag.sales = null;
                purchasehist.vouchertypeid = 2;

            }
            return View(purchasehist);

        }
        [HttpGet]
        public ActionResult EditPVoucher(int id)
        {
            if (id != 0)
            {
                var masterid = db.VoucherMaster.Where(m => m.vouchermasterid == id && m.Partyid_Accountid != 1059).FirstOrDefault().VoucherNum_BillNum;

                var vouchermaster = db.VoucherMaster.Find(masterid);
                var voucherhild = db.VoucherChild.Where(m => m.VoucherMasterID == id).ToList();
                List<BillSundryChild> billsundrychild = new List<BillSundryChild>();
                if (vouchermaster.vouchermasterid != 0)
                {
                    billsundrychild = db.BillSundryChild.Where(m => m.Vouchermasterid == vouchermaster.vouchermasterid).ToList();

                }


                ViewBag.Accountname = new SelectList(db.Account, "AccountID", "AccountName", vouchermaster.Partyid_Accountid);
                ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
                ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
                ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName");
                ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID);

                ViewBag.vouchernumber = vouchermaster.vouchermasterid;
                ViewBag.VoucherCreateDate = vouchermaster.VoucherCreateDate;
                ViewBag.Narration = vouchermaster.Narration;

                double billsamount2 = 0.00;
                if (vouchermaster.BillSundryAmount > 0)
                {
                    billsamount2 = vouchermaster.BillSundryAmount;
                }
                else if (vouchermaster.BillSundryAmount <= 0)
                {
                    billsamount2 = vouchermaster.BillSundryAmount;
                }
                VoucherMasterViewModel masterview = new VoucherMasterViewModel
                {
                    VoucherNumber = (int)vouchermaster.vouchermasterid,
                    VoucherCreateDate = vouchermaster.VoucherCreateDate,
                    Partyid = vouchermaster.Partyid_Accountid,
                    LocationID = vouchermaster.LocationID,
                    Narration = vouchermaster.Narration,
                    BillSundryAmount2 = (int)billsamount2,
                    FinalTotal = vouchermaster.VoucherFinalTotal,
                    VoucherType_IDddddd = vouchermaster.VoucherTypeID,
                    DrCrType = vouchermaster.DrCrType,
                    VoucherChildlist = voucherhild,
                    modify = 1,
                    vouchertypeid = vouchermaster.VoucherTypeID,
                    BillSundrychildlist = billsundrychild
                };







                return View("~/Views/Sales/SPVoucher.cshtml", masterview);

            }
            else
            {
                return RedirectToAction("ModifySvoucher");
            }
        }

        public ActionResult Newpurchasehistory()
        {
            var vouchercount = db.VoucherMaster.Where(x => x.VoucherTypeID == 2).Count();
            VoucherMasterViewModel purchasehist = new VoucherMasterViewModel();

            if (vouchercount > 0)
            {
                 purchasehist.Vouchermasterlist = db.VoucherMaster.Where(x => x.VoucherTypeID == 2 && x.Partyid_Accountid != 1059).ToList();
                var totalsale = db.VoucherMaster.Where(m => m.VoucherTypeID == 2).Sum(m => m.VoucherFinalTotal);
                ViewBag.sales = totalsale;

            }
            else
            {
                purchasehist.vouchertypeid = 2;
                ViewBag.sales = null;
            }


            return View(purchasehist);
        }

        public ActionResult detailPVoucher(int id)
        {
            if (id != 0)
            {

                var vtypeid = db.VoucherMaster.Where(m => m.vouchermasterid == id ).FirstOrDefault().VoucherTypeID;
                if (vtypeid == 2 )
                {
                var vouchermaster = db.VoucherMaster.Where(m => m.vouchermasterid == id && m.DrCrType == 2).FirstOrDefault();
                    var voucherhild = db.VoucherChild.Where(m => m.VoucherMasterID == id).ToList();
                    List<BillSundryChild> billsundrychild = new List<BillSundryChild>();
                    if (vouchermaster.vouchermasterid != 0)
                    {
                        billsundrychild = db.BillSundryChild.Where(m => m.Vouchermasterid == vouchermaster.vouchermasterid).ToList();

                    }
                    ViewBag.Accountname = new SelectList(db.Account, "AccountID", "AccountName", vouchermaster.Partyid_Accountid);
                    ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
                    ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
                    ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName", vouchermaster.BillSundryId);
                    ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID);
                    ViewBag.MaterialCentres2 = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID2);

                    ViewBag.vouchernumber = vouchermaster.vouchermasterid;
                    ViewBag.VoucherCreateDate = vouchermaster.VoucherCreateDate;
                    ViewBag.Narration = vouchermaster.Narration;

                    //double billsamount = 0.00;
                    double billsamount2 = 0.00;
                    //if (vouchermaster.BillSundryId == 1)
                    //{
                    //    billsamount = vouchermaster.BillSundryAmount;
                    //}
                    //else if (vouchermaster.BillSundryId == 2)
                    //{
                    //    billsamount2 = vouchermaster.BillSundryAmount;
                    //}
                    VoucherMasterViewModel masterview = new VoucherMasterViewModel
                    {
                        VoucherNumber = vouchermaster.VoucherNum_BillNum,
                        VoucherCreateDate = vouchermaster.VoucherCreateDate,
                        Partyid = vouchermaster.Partyid_Accountid,
                        Narration = vouchermaster.Narration,
                        BillSundryAmount2 = (int)billsamount2,
                        FinalTotal = vouchermaster.VoucherFinalTotal,
                        VoucherType_IDddddd = vouchermaster.VoucherTypeID,
                        DrCrType = vouchermaster.DrCrType,
                        VoucherChildlist = voucherhild,
                        modify = 1,
                        vouchertypeid = vouchermaster.VoucherTypeID,
                        BillSundrychildlist = billsundrychild,
                        LocationID = vouchermaster.LocationID,
                        LocationID2 = vouchermaster.LocationID2,
                    };
                    return View("/Views/Sales/detailSVoucher.cshtml", masterview);
                }
                else if (vtypeid == 10)
                {

                    var vouchermaster = db.VoucherMaster.Where(m => m.vouchermasterid == id && m.DrCrType == 1).FirstOrDefault();
                    var voucherhild = db.VoucherChild.Where(m => m.VoucherMasterID == id).ToList();
                    List<BillSundryChild> billsundrychild = new List<BillSundryChild>();
                    if (vouchermaster.vouchermasterid != 0)
                    {
                        billsundrychild = db.BillSundryChild.Where(m => m.Vouchermasterid == vouchermaster.vouchermasterid).ToList();

                    }
                    ViewBag.Accountname = new SelectList(db.Account, "AccountID", "AccountName", vouchermaster.Partyid_Accountid);
                    ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
                    ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
                    ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName", vouchermaster.BillSundryId);
                    ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID);
                    ViewBag.MaterialCentres2 = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID2);

                    ViewBag.vouchernumber = vouchermaster.vouchermasterid;
                    ViewBag.VoucherCreateDate = vouchermaster.VoucherCreateDate;
                    ViewBag.Narration = vouchermaster.Narration;

                    //double billsamount = 0.00;
                    double billsamount2 = 0.00;
                    //if (vouchermaster.BillSundryId == 1)
                    //{
                    //    billsamount = vouchermaster.BillSundryAmount;
                    //}
                    //else if (vouchermaster.BillSundryId == 2)
                    //{
                    //    billsamount2 = vouchermaster.BillSundryAmount;
                    //}
                    VoucherMasterViewModel masterview = new VoucherMasterViewModel
                    {
                        VoucherNumber = vouchermaster.VoucherNum_BillNum,
                        VoucherCreateDate = vouchermaster.VoucherCreateDate,
                        Partyid = vouchermaster.Partyid_Accountid,
                        Narration = vouchermaster.Narration,
                        BillSundryAmount2 = (int)billsamount2,
                        FinalTotal = vouchermaster.VoucherFinalTotal,
                        VoucherType_IDddddd = vouchermaster.VoucherTypeID,
                        DrCrType = vouchermaster.DrCrType,
                        VoucherChildlist = voucherhild,
                        modify = 1,
                        vouchertypeid = vouchermaster.VoucherTypeID,
                        BillSundrychildlist = billsundrychild,
                        LocationID = vouchermaster.LocationID,
                        LocationID2 = vouchermaster.LocationID2,
                    };
                    return View("/Views/Sales/detailSVoucher.cshtml", masterview);
                }
                else
                {
                    var vouchermaster = db.VoucherMaster.Where(m => m.vouchermasterid == id).FirstOrDefault();
                    var voucherhild = db.VoucherChild.Where(m => m.VoucherMasterID == id).ToList();
                    List<BillSundryChild> billsundrychild = new List<BillSundryChild>();
                    if (vouchermaster.vouchermasterid != 0)
                    {
                        billsundrychild = db.BillSundryChild.Where(m => m.Vouchermasterid == vouchermaster.vouchermasterid).ToList();

                    }
                    ViewBag.Accountname = new SelectList(db.Account, "AccountID", "AccountName", vouchermaster.Partyid_Accountid);
                    ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
                    ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
                    ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName", vouchermaster.BillSundryId);
                    ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID);
                    ViewBag.MaterialCentres2 = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID2);

                    ViewBag.vouchernumber = vouchermaster.vouchermasterid;
                    ViewBag.VoucherCreateDate = vouchermaster.VoucherCreateDate;
                    ViewBag.Narration = vouchermaster.Narration;

                    //double billsamount = 0.00;
                    double billsamount2 = 0.00;
                    //if (vouchermaster.BillSundryId == 1)
                    //{
                    //    billsamount = vouchermaster.BillSundryAmount;
                    //}
                    //else if (vouchermaster.BillSundryId == 2)
                    //{
                    //    billsamount2 = vouchermaster.BillSundryAmount;
                    //}
                    VoucherMasterViewModel masterview = new VoucherMasterViewModel
                    {
                        VoucherNumber = vouchermaster.VoucherNum_BillNum,
                        VoucherCreateDate = vouchermaster.VoucherCreateDate,
                        Partyid = vouchermaster.Partyid_Accountid,
                        Narration = vouchermaster.Narration,
                        BillSundryAmount2 = (int)billsamount2,
                        FinalTotal = vouchermaster.VoucherFinalTotal,
                        VoucherType_IDddddd = vouchermaster.VoucherTypeID,
                        DrCrType = vouchermaster.DrCrType,
                        VoucherChildlist = voucherhild,
                        modify = 1,
                        vouchertypeid = vouchermaster.VoucherTypeID,
                        BillSundrychildlist = billsundrychild,
                        LocationID = vouchermaster.LocationID,
                        LocationID2 = vouchermaster.LocationID2,
                    };
                    return View("/Views/Sales/detailSVoucher.cshtml", masterview);
                }
            }
            else
            {
                return RedirectToAction("ModifyPvoucher");
            }

        }

        public void updatestock(int quantity)
        {
            var stock = new Stock() { Quantity = quantity };
            using (var dbb = new MVCDBLiveEntities())
            {
                dbb.Stock.Attach(stock);
                dbb.Entry(stock).Property(x => x.Quantity).IsModified = true;
                dbb.SaveChanges();
            }
        }
        public JsonResult getaccountbalance(int accountid)
        {
            var bal = db.Account.Find(accountid).ClosingBalance;
            return Json(bal,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SavePVoucher(VoucherMasterViewModel vouchermaster)
        {
            try
            {
                if (vouchermaster.modify == 0)
                {
                    var vouchermasterid2 = db.VoucherMaster.Count() + 1;
                    //var itemid = vouchermaster.ItemID;
                    //var orderMasterId = 0;
                    int billsid = 0;
                    int billsundrynumber = db.BillSundryChild.Count() + 1;
                    double billsundrytotalamount = 0;
                    double billsamount = 0;


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
                        if (vouchermaster.vouchertypeid == 2)
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
                                VoucherTypeID = 2


                            };
                            db.VoucherChild.Add(voucherchild);
                            db.SaveChanges();
                        }
                        else if (vouchermaster.vouchertypeid == 10)
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
                                VoucherTypeID = 10


                            };
                            db.VoucherChild.Add(voucherchild);
                            db.SaveChanges();
                        }
                        Transactions trans = new Transactions
                        {
                            //TransactionID = 0,
                            ItemID = vouchermaster.ItemID[i],
                            Quantity = (+1) * vouchermaster.Quantity[i],
                            TransDate = DateTime.Now,
                            PayTypeID = 1,
                            ItemPrice = vouchermaster.ItemPrice[i],
                            TransType = 2
                        };
                        db.Transactions.Add(trans);
                        db.SaveChanges();
                        OrderServices orderServices = new OrderServices();

                        try
                        {
                            //---update Stonks---//
                            if (vouchermaster.vouchertypeid == 2)
                            {
                                var StockValues = orderServices.GetStockById(vouchermaster.ItemID[i]);
                                StockValues.Quantity = StockValues.Quantity + vouchermaster.Quantity[i];
                                orderServices.UpdateStock(StockValues);
                            }
                            else if (vouchermaster.vouchertypeid == 10)
                            {
                                var StockValues = orderServices.GetStockById(vouchermaster.ItemID[i]);
                                StockValues.Quantity = StockValues.Quantity - vouchermaster.Quantity[i];
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


                    //VoucherMaster vouchermsterr = new VoucherMaster
                    //{
                    //    //VoucherNum_BillNum = 0,
                    //    VoucherCreateDate = vouchermaster.VoucherCreateDate,
                    //    Partyid_Accountid = vouchermaster.Partyid,
                    //    Narration = vouchermaster.Narration,
                    //    BillSundryId = billsundrynumber,
                    //    BillSundryAmount = billsundrytotalamount,
                    //    VoucherFinalTotal = vouchermaster.FinalTotal,
                    //    VoucherTypeID = 2,
                    //    DrCrType = 2,
                    //    vouchermasterid = vouchermasterid2

                    //};
                    //db.VoucherMaster.Add(vouchermsterr);
                    //db.SaveChanges();

                    if (vouchermaster.vouchertypeid == 2)
                    {


                        VoucherMaster salemaster = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = 1059,
                            Narration = "purchase account debit",
                            VoucherFinalTotal = vouchermaster.FinalTotal,
                            VoucherTypeID = 2,
                            DrCrType = 1,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID


                        };
                        db.VoucherMaster.Add(salemaster);
                        db.SaveChanges();
                        ////////////////////////
                        ////
                        VoucherMaster vouchermsterr = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = vouchermaster.Partyid,
                            Narration = vouchermaster.Narration,
                            BillSundryId = billsundrynumber,
                            BillSundryAmount = billsundrytotalamount,
                            VoucherFinalTotal = vouchermaster.FinalTotal,
                            VoucherTypeID = 2,
                            DrCrType = 2,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID

                        };
                        db.VoucherMaster.Add(vouchermsterr);
                        db.SaveChanges();
                    }
                    else if (vouchermaster.vouchertypeid == 10)
                    {

                        VoucherMaster salemaster = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = 1059,
                            Narration = "purchase account credit",
                            VoucherFinalTotal = vouchermaster.FinalTotal,
                            VoucherTypeID = 10,
                            DrCrType = 2,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID


                        };
                        db.VoucherMaster.Add(salemaster);
                        db.SaveChanges();
                        //////////////////////////////
                        VoucherMaster vouchermsterr = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = vouchermaster.Partyid,
                            Narration = vouchermaster.Narration,
                            BillSundryId = billsundrynumber,
                            BillSundryAmount = billsundrytotalamount,
                            VoucherFinalTotal = vouchermaster.FinalTotal,
                            VoucherTypeID = 10,
                            DrCrType = 1,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID


                        };
                        db.VoucherMaster.Add(vouchermsterr);
                        db.SaveChanges();
                    }




                    if (vouchermaster.vouchertypeid == 2)
                    {
                        //var masterid = db.VoucherMaster.Where(m => m.vouchermasterid == 77).FirstOrDefault().VoucherNum_BillNum;
                        //var updatesalesvoucher = db.VoucherMaster.Find(masterid);
                        //updatesalesvoucher.VoucherFinalTotal = updatesalesvoucher.VoucherFinalTotal + vouchermaster.FinalTotal;
                        //db.Entry(updatesalesvoucher).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();


                        //-------------------Update Revenue Group----------------------//

                        var findaccidsales = db.AccGroup.Where(m => m.AccgroupID == 24).FirstOrDefault().AccgroupID;
                        var findaccgroupsales = ms.Getaccgroupbyid(findaccidsales);
                        var oldcreditsales = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales.AccgroupID).FirstOrDefault().DebitAmount;
                        var oldbalancesales = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales.AccgroupID).FirstOrDefault().Accgroupbalance;

                        findaccgroupsales.DebitAmount = oldcreditsales + vouchermaster.FinalTotal;
                        findaccgroupsales.Accgroupbalance = oldbalancesales + vouchermaster.FinalTotal;
                        ms.UpdateAccgroup(findaccgroupsales);

                        //-------------------Update Purchase group----------------------//

                        var findaccidsales2 = db.AccGroup.Where(m => m.AccgroupID == 1062).FirstOrDefault().AccgroupID;
                        var findaccgroupsales2 = ms.Getaccgroupbyid(findaccidsales2);
                        var oldcreditsales2 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales2.AccgroupID).FirstOrDefault().DebitAmount;
                        var oldbalancesales2 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales2.AccgroupID).FirstOrDefault().Accgroupbalance;

                        findaccgroupsales2.DebitAmount = oldcreditsales2 + vouchermaster.FinalTotal;
                        findaccgroupsales2.Accgroupbalance = oldbalancesales2 + vouchermaster.FinalTotal;
                        ms.UpdateAccgroup(findaccgroupsales2);


                        ////-------------------Update Purchase accouint----------------------//

                        var findaccidpurchase3 = db.Account.Where(m => m.Accgroupid == findaccidsales2).FirstOrDefault().AccountID;
                        var findaccgrouppurchase3 = ms.Getaccountbyid(findaccidpurchase3);
                        var oldcreditpurchase3 = db.Account.Where(m => m.AccountID == findaccgrouppurchase3.AccountID).FirstOrDefault().DebitTotal;
                        var oldbalancepurchase3 = db.Account.Where(m => m.AccountID == findaccgrouppurchase3.AccountID).FirstOrDefault().ClosingBalance;

                        findaccgrouppurchase3.DebitTotal = oldcreditpurchase3 + vouchermaster.FinalTotal;
                        findaccgrouppurchase3.ClosingBalance = oldbalancepurchase3 + vouchermaster.FinalTotal;
                        ms.Updateaccount(findaccgrouppurchase3);

                        //----------Update account Balance----------//
                        var findaccount = ms.Getaccountbyid(vouchermaster.Partyid);
                        var oldbalance = db.Account.Where(x => x.AccountID == vouchermaster.Partyid).FirstOrDefault().ClosingBalance;
                        var oldcredit = db.Account.Where(x => x.AccountID == vouchermaster.Partyid).FirstOrDefault().CreditTotal;

                        findaccount.ClosingBalance = oldbalance - vouchermaster.FinalTotal;
                        findaccount.CreditTotal = oldcredit + vouchermaster.FinalTotal;
                        ms.Updateaccount(findaccount);


                        //---------------------Update Account  group 1 credit----------------------------//

                        var findaccgroupCrr = ms.Getaccgroupbyid(findaccount.Accgroupid);
                        var oldaccgrupcredit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().CreditAmount;
                        var oldbalancecredit = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().Accgroupbalance;
                        if (findaccgroupCrr.Accunderprimarygroupid != 0 && findaccgroupCrr.Accunderprimarygroupid != null)
                        {
                            findaccgroupCrr.Accgroupbalance = oldbalancecredit - vouchermaster.FinalTotal;
                            findaccgroupCrr.CreditAmount = oldaccgrupcredit1 + vouchermaster.FinalTotal;
                            ms.UpdateAccgroup(findaccgroupCrr);

                            //---------------------Update Account group 2 credit----------------------------//
                            var findaccprimgroupCr2 = ms.Getaccgroupbyid(findaccgroupCrr.Accunderprimarygroupid);
                            var oldaccprimgrupcredit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr2.AccgroupID).FirstOrDefault().CreditAmount;
                            var oldaccprimgrupbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr2.AccgroupID).FirstOrDefault().Accgroupbalance;
                            if (findaccprimgroupCr2.Accunderprimarygroupid != 0 && findaccprimgroupCr2.Accunderprimarygroupid != null)
                            {
                                findaccprimgroupCr2.Accgroupbalance = oldaccprimgrupbalance2 - vouchermaster.FinalTotal;
                                findaccprimgroupCr2.CreditAmount = oldaccprimgrupcredit2 + vouchermaster.FinalTotal;
                                ms.UpdateAccgroup(findaccprimgroupCr2);


                                //---------------------Update Account primary group 3 credit----------------------------//
                                var findaccprimgroupCr3 = ms.Getaccgroupbyid(findaccprimgroupCr2.Accunderprimarygroupid);
                                var oldaccprimgrupcredit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr3.AccgroupID).FirstOrDefault().CreditAmount;
                                var oldaccprimgrupbalance3 = findaccprimgroupCr3.Accgroupbalance;
                                if (findaccprimgroupCr3.Accunderprimarygroupid != 0 && findaccprimgroupCr3.Accunderprimarygroupid != null)
                                {
                                    findaccprimgroupCr3.Accgroupbalance = oldaccprimgrupbalance3 - vouchermaster.FinalTotal;
                                    findaccprimgroupCr3.CreditAmount = oldaccprimgrupcredit3 + vouchermaster.FinalTotal;
                                    ms.UpdateAccgroup(findaccprimgroupCr3);


                                    //---------------------Update Account primary group 4 credit----------------------------//
                                    var findaccprimgroupCr4 = ms.Getaccgroupbyid(findaccprimgroupCr3.Accunderprimarygroupid);
                                    var oldaccprimgrupcredit4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr4.AccgroupID).FirstOrDefault().CreditAmount;
                                    var oldaccprimgrupbalance4 = findaccprimgroupCr4.Accgroupbalance;

                                    if (findaccprimgroupCr4.Accunderprimarygroupid != 0 && findaccprimgroupCr4.Accunderprimarygroupid != null)
                                    {
                                        findaccprimgroupCr4.Accgroupbalance = oldaccprimgrupbalance4 - vouchermaster.FinalTotal;
                                        findaccprimgroupCr4.CreditAmount = oldaccprimgrupcredit4 + vouchermaster.FinalTotal;
                                        ms.UpdateAccgroup(findaccprimgroupCr4);


                                        //---------------------Update Account primary group 5 credit----------------------------//
                                        var findaccprimgroupCr5 = ms.Getaccgroupbyid(findaccprimgroupCr4.Accunderprimarygroupid);
                                        var oldaccprimgrupcredit5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr5.AccgroupID).FirstOrDefault().CreditAmount;
                                        var oldaccprimgrupbalance5 = findaccprimgroupCr5.Accgroupbalance;

                                        if (findaccprimgroupCr5.Accunderprimarygroupid != 0 && findaccprimgroupCr5.Accunderprimarygroupid != null)
                                        {
                                            findaccprimgroupCr5.Accgroupbalance = oldaccprimgrupbalance5 - vouchermaster.FinalTotal;
                                            findaccprimgroupCr5.CreditAmount = oldaccprimgrupcredit5 + vouchermaster.FinalTotal;
                                            ms.UpdateAccgroup(findaccprimgroupCr5);


                                            //---------------------Update Account primary group 6 credit----------------------------//
                                            var findaccprimgroupCr6 = ms.Getaccgroupbyid(findaccprimgroupCr5.Accunderprimarygroupid);
                                            var oldaccprimgrupcredit6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr6.AccgroupID).FirstOrDefault().CreditAmount;
                                            var oldaccprimgrupbalance6 = findaccprimgroupCr6.Accgroupbalance;
                                            if (findaccprimgroupCr6.Accunderprimarygroupid != 0 && findaccprimgroupCr6.Accunderprimarygroupid != null)
                                            {
                                                findaccprimgroupCr6.Accgroupbalance = oldaccprimgrupbalance6 - vouchermaster.FinalTotal;
                                                findaccprimgroupCr6.CreditAmount = oldaccprimgrupcredit6 + vouchermaster.FinalTotal;
                                                ms.UpdateAccgroup(findaccprimgroupCr6);



                                                //---------------------Update Account primary group 7 credit----------------------------//
                                                var findaccprimgroupCr7 = ms.Getaccgroupbyid(findaccprimgroupCr6.Accunderprimarygroupid);
                                                var oldaccprimgrupcredit7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr7.AccgroupID).FirstOrDefault().CreditAmount;
                                                var oldaccprimgrupbalance7 = findaccprimgroupCr7.Accgroupbalance;
                                                if (findaccprimgroupCr7.Accunderprimarygroupid != 0 && findaccprimgroupCr7.Accunderprimarygroupid != null)
                                                {
                                                    findaccprimgroupCr7.Accgroupbalance = oldaccprimgrupbalance7 - vouchermaster.FinalTotal;
                                                    findaccprimgroupCr7.CreditAmount = oldaccprimgrupcredit7 + vouchermaster.FinalTotal;
                                                    ms.UpdateAccgroup(findaccprimgroupCr7);

                                                    //---------------------Update Account primary group 8 credit----------------------------//
                                                    var findaccprimgroupCr8 = ms.Getaccgroupbyid(findaccprimgroupCr7.Accunderprimarygroupid);
                                                    var oldaccprimgrupcredit8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr8.AccgroupID).FirstOrDefault().CreditAmount;
                                                    var oldaccprimgrupbalance8 = findaccprimgroupCr8.Accgroupbalance;
                                                    if (findaccprimgroupCr8.Accunderprimarygroupid != 0 && findaccprimgroupCr8.Accunderprimarygroupid != null)
                                                    {
                                                        findaccprimgroupCr8.Accgroupbalance = oldaccprimgrupbalance8 - vouchermaster.FinalTotal;
                                                        findaccprimgroupCr8.CreditAmount = oldaccprimgrupcredit8 + vouchermaster.FinalTotal;
                                                        ms.UpdateAccgroup(findaccprimgroupCr8);


                                                        //---------------------Update Account primary group 9 credit----------------------------//
                                                        var findaccprimgroupCr9 = ms.Getaccgroupbyid(findaccprimgroupCr8.Accunderprimarygroupid);
                                                        var oldaccprimgrupcredit9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr9.AccgroupID).FirstOrDefault().CreditAmount;
                                                        var oldaccprimgrupbalance9 = findaccprimgroupCr9.Accgroupbalance;
                                                        if (findaccprimgroupCr9.Accunderprimarygroupid != 0 && findaccprimgroupCr9.Accunderprimarygroupid != null)
                                                        {
                                                            findaccprimgroupCr9.Accgroupbalance = oldaccprimgrupbalance9 - vouchermaster.FinalTotal;
                                                            findaccprimgroupCr9.CreditAmount = oldaccprimgrupcredit9 + vouchermaster.FinalTotal;
                                                            ms.UpdateAccgroup(findaccprimgroupCr9);

                                                            //---------------------Update Account primary group 9 credit----------------------------//
                                                            var findaccprimgroupCr10 = ms.Getaccgroupbyid(findaccprimgroupCr9.Accunderprimarygroupid);
                                                            var oldaccprimgrupcredit10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr10.AccgroupID).FirstOrDefault().CreditAmount;
                                                            var oldaccprimgrupbalance10 = findaccprimgroupCr10.Accgroupbalance;
                                                            if (findaccprimgroupCr10.Accunderprimarygroupid != 0 && findaccprimgroupCr10.Accunderprimarygroupid != null)
                                                            {
                                                                findaccprimgroupCr10.Accgroupbalance = oldaccprimgrupbalance10 - vouchermaster.FinalTotal;
                                                                findaccprimgroupCr10.CreditAmount = oldaccprimgrupcredit10 + vouchermaster.FinalTotal;
                                                                ms.UpdateAccgroup(findaccprimgroupCr10);


                                                            }
                                                            else
                                                            {
                                                                findaccprimgroupCr10.Accgroupbalance = oldaccprimgrupbalance10 - vouchermaster.FinalTotal;
                                                                findaccprimgroupCr10.CreditAmount = oldaccprimgrupcredit10 + vouchermaster.FinalTotal;
                                                                ms.UpdateAccgroup(findaccprimgroupCr10);
                                                            }





                                                        }
                                                        else
                                                        {
                                                            findaccprimgroupCr9.Accgroupbalance = oldaccprimgrupbalance9 - vouchermaster.FinalTotal;
                                                            findaccprimgroupCr9.CreditAmount = oldaccprimgrupcredit9 + vouchermaster.FinalTotal;
                                                            ms.UpdateAccgroup(findaccprimgroupCr9);
                                                        }




                                                    }
                                                    else
                                                    {
                                                        findaccprimgroupCr8.Accgroupbalance = oldaccprimgrupbalance8 - vouchermaster.FinalTotal;
                                                        findaccprimgroupCr8.CreditAmount = oldaccprimgrupcredit8 + vouchermaster.FinalTotal;
                                                        ms.UpdateAccgroup(findaccprimgroupCr8);
                                                    }





                                                }
                                                else
                                                {
                                                    findaccprimgroupCr7.Accgroupbalance = oldaccprimgrupbalance7 - vouchermaster.FinalTotal;
                                                    findaccprimgroupCr7.CreditAmount = oldaccprimgrupcredit7 + vouchermaster.FinalTotal;
                                                    ms.UpdateAccgroup(findaccprimgroupCr7);
                                                }



                                            }
                                            else
                                            {
                                                findaccprimgroupCr6.Accgroupbalance = oldaccprimgrupbalance6 - vouchermaster.FinalTotal;
                                                findaccprimgroupCr6.CreditAmount = oldaccprimgrupcredit6 + vouchermaster.FinalTotal;
                                                ms.UpdateAccgroup(findaccprimgroupCr6);
                                            }




                                        }
                                        else
                                        {
                                            findaccprimgroupCr5.Accgroupbalance = oldaccprimgrupbalance5 - vouchermaster.FinalTotal;
                                            findaccprimgroupCr5.CreditAmount = oldaccprimgrupcredit5 + vouchermaster.FinalTotal;
                                            ms.UpdateAccgroup(findaccprimgroupCr5);
                                        }



                                    }
                                    else
                                    {
                                        findaccprimgroupCr4.Accgroupbalance = oldaccprimgrupbalance4 - vouchermaster.FinalTotal;
                                        findaccprimgroupCr4.CreditAmount = oldaccprimgrupcredit4 + vouchermaster.FinalTotal;
                                        ms.UpdateAccgroup(findaccprimgroupCr4);
                                    }


                                }
                                else
                                {
                                    findaccprimgroupCr3.Accgroupbalance = oldaccprimgrupbalance3 - vouchermaster.FinalTotal;
                                    findaccprimgroupCr3.CreditAmount = oldaccprimgrupcredit3 + vouchermaster.FinalTotal;
                                    ms.UpdateAccgroup(findaccprimgroupCr3);
                                }


                            }
                            else
                            {
                                findaccprimgroupCr2.Accgroupbalance = oldaccprimgrupbalance2 - vouchermaster.FinalTotal;
                                findaccprimgroupCr2.CreditAmount = oldaccprimgrupcredit2 + vouchermaster.FinalTotal;
                                ms.UpdateAccgroup(findaccprimgroupCr2);

                            }

                        }
                        else
                        {

                            findaccgroupCrr.Accgroupbalance = oldbalance - vouchermaster.FinalTotal;
                            findaccgroupCrr.CreditAmount = oldaccgrupcredit1 + vouchermaster.FinalTotal;
                            ms.UpdateAccgroup(findaccgroupCrr);

                        }
                    }
                    else if (vouchermaster.vouchertypeid == 10)
                    {
                        //var masterid = db.VoucherMaster.Where(m => m.vouchermasterid == 77).FirstOrDefault().VoucherNum_BillNum;
                        //var updatesalesvoucher = db.VoucherMaster.Find(masterid);
                        //updatesalesvoucher.VoucherFinalTotal = updatesalesvoucher.VoucherFinalTotal - vouchermaster.FinalTotal;
                        //db.Entry(updatesalesvoucher).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();


                        //-------------------Update Revenue Group----------------------//

                        var findaccidsales = db.AccGroup.Where(m => m.AccgroupID == 24).FirstOrDefault().AccgroupID;
                        var findaccgroupsales = ms.Getaccgroupbyid(findaccidsales);
                        var oldcreditsales = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales.AccgroupID).FirstOrDefault().CreditAmount;
                        var oldbalancesales = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales.AccgroupID).FirstOrDefault().Accgroupbalance;

                        findaccgroupsales.CreditAmount = oldcreditsales - vouchermaster.FinalTotal;
                        findaccgroupsales.Accgroupbalance = oldbalancesales + vouchermaster.FinalTotal;
                        ms.UpdateAccgroup(findaccgroupsales);

                        //-------------------Update Purchase group----------------------//

                        var findaccidsales2 = db.AccGroup.Where(m => m.AccgroupID == 1063).FirstOrDefault().AccgroupID;
                        var findaccgroupsales2 = ms.Getaccgroupbyid(findaccidsales2);
                        var oldcreditsales2 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales2.AccgroupID).FirstOrDefault().CreditAmount;
                        var oldbalancesales2 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales2.AccgroupID).FirstOrDefault().Accgroupbalance;

                        findaccgroupsales2.CreditAmount = oldcreditsales2 - vouchermaster.FinalTotal;
                        findaccgroupsales2.Accgroupbalance = oldbalancesales2 + vouchermaster.FinalTotal;
                        ms.UpdateAccgroup(findaccgroupsales2);


                        ////-------------------Update Purchase accouint----------------------//

                        var findaccidpurchase3 = db.Account.Where(m => m.Accgroupid == findaccidsales2).FirstOrDefault().AccountID;
                        var findaccgrouppurchase3 = ms.Getaccountbyid(findaccidpurchase3);
                        var oldcreditpurchase3 = db.Account.Where(m => m.AccountID == findaccgrouppurchase3.AccountID).FirstOrDefault().CreditTotal;
                        var oldbalancepurchase3 = db.Account.Where(m => m.AccountID == findaccgrouppurchase3.AccountID).FirstOrDefault().ClosingBalance;

                        findaccgrouppurchase3.CreditTotal = oldcreditpurchase3 - vouchermaster.FinalTotal;
                        findaccgrouppurchase3.ClosingBalance = oldbalancepurchase3 + vouchermaster.FinalTotal;
                        ms.Updateaccount(findaccgrouppurchase3);

                        //----------Update account Balance----------//
                        var findaccount = ms.Getaccountbyid(vouchermaster.Partyid);
                        var oldbalance = db.Account.Where(x => x.AccountID == vouchermaster.Partyid).FirstOrDefault().ClosingBalance;
                        var oldcredit = db.Account.Where(x => x.AccountID == vouchermaster.Partyid).FirstOrDefault().DebitTotal;

                        findaccount.ClosingBalance = oldbalance + vouchermaster.FinalTotal;
                        findaccount.DebitTotal = oldcredit + vouchermaster.FinalTotal;
                        ms.Updateaccount(findaccount);


                        //---------------------Update Account  group 1 credit----------------------------//

                        var findaccgroupCrr = ms.Getaccgroupbyid(findaccount.Accgroupid);
                        var oldaccgrupcredit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().DebitAmount;
                        var oldbalancecredit = db.AccGroup.Where(m => m.AccgroupID == findaccgroupCrr.AccgroupID).FirstOrDefault().Accgroupbalance;
                        if (findaccgroupCrr.Accunderprimarygroupid != 0 && findaccgroupCrr.Accunderprimarygroupid != null)
                        {
                            findaccgroupCrr.Accgroupbalance = oldbalancecredit + vouchermaster.FinalTotal;
                            findaccgroupCrr.DebitAmount = oldaccgrupcredit1 + vouchermaster.FinalTotal;
                            ms.UpdateAccgroup(findaccgroupCrr);

                            //---------------------Update Account group 2 credit----------------------------//
                            var findaccprimgroupCr2 = ms.Getaccgroupbyid(findaccgroupCrr.Accunderprimarygroupid);
                            var oldaccprimgrupcredit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr2.AccgroupID).FirstOrDefault().DebitAmount;
                            var oldaccprimgrupbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr2.AccgroupID).FirstOrDefault().Accgroupbalance;
                            if (findaccprimgroupCr2.Accunderprimarygroupid != 0 && findaccprimgroupCr2.Accunderprimarygroupid != null)
                            {
                                findaccprimgroupCr2.Accgroupbalance = oldaccprimgrupbalance2 + vouchermaster.FinalTotal;
                                findaccprimgroupCr2.DebitAmount = oldaccprimgrupcredit2 + vouchermaster.FinalTotal;
                                ms.UpdateAccgroup(findaccprimgroupCr2);


                                //---------------------Update Account primary group 3 credit----------------------------//
                                var findaccprimgroupCr3 = ms.Getaccgroupbyid(findaccprimgroupCr2.Accunderprimarygroupid);
                                var oldaccprimgrupcredit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr3.AccgroupID).FirstOrDefault().DebitAmount;
                                var oldaccprimgrupbalance3 = findaccprimgroupCr3.Accgroupbalance;
                                if (findaccprimgroupCr3.Accunderprimarygroupid != 0 && findaccprimgroupCr3.Accunderprimarygroupid != null)
                                {
                                    findaccprimgroupCr3.Accgroupbalance = oldaccprimgrupbalance3 + vouchermaster.FinalTotal;
                                    findaccprimgroupCr3.DebitAmount = oldaccprimgrupcredit3 + vouchermaster.FinalTotal;
                                    ms.UpdateAccgroup(findaccprimgroupCr3);


                                    //---------------------Update Account primary group 4 credit----------------------------//
                                    var findaccprimgroupCr4 = ms.Getaccgroupbyid(findaccprimgroupCr3.Accunderprimarygroupid);
                                    var oldaccprimgrupcredit4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr4.AccgroupID).FirstOrDefault().DebitAmount;
                                    var oldaccprimgrupbalance4 = findaccprimgroupCr4.Accgroupbalance;

                                    if (findaccprimgroupCr4.Accunderprimarygroupid != 0 && findaccprimgroupCr4.Accunderprimarygroupid != null)
                                    {
                                        findaccprimgroupCr4.Accgroupbalance = oldaccprimgrupbalance4 + vouchermaster.FinalTotal;
                                        findaccprimgroupCr4.DebitAmount = oldaccprimgrupcredit4 + vouchermaster.FinalTotal;
                                        ms.UpdateAccgroup(findaccprimgroupCr4);


                                        //---------------------Update Account primary group 5 credit----------------------------//
                                        var findaccprimgroupCr5 = ms.Getaccgroupbyid(findaccprimgroupCr4.Accunderprimarygroupid);
                                        var oldaccprimgrupcredit5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr5.AccgroupID).FirstOrDefault().DebitAmount;
                                        var oldaccprimgrupbalance5 = findaccprimgroupCr5.Accgroupbalance;

                                        if (findaccprimgroupCr5.Accunderprimarygroupid != 0 && findaccprimgroupCr5.Accunderprimarygroupid != null)
                                        {
                                            findaccprimgroupCr5.Accgroupbalance = oldaccprimgrupbalance5 + vouchermaster.FinalTotal;
                                            findaccprimgroupCr5.DebitAmount = oldaccprimgrupcredit5 + vouchermaster.FinalTotal;
                                            ms.UpdateAccgroup(findaccprimgroupCr5);


                                            //---------------------Update Account primary group 6 credit----------------------------//
                                            var findaccprimgroupCr6 = ms.Getaccgroupbyid(findaccprimgroupCr5.Accunderprimarygroupid);
                                            var oldaccprimgrupcredit6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr6.AccgroupID).FirstOrDefault().DebitAmount;
                                            var oldaccprimgrupbalance6 = findaccprimgroupCr6.Accgroupbalance;
                                            if (findaccprimgroupCr6.Accunderprimarygroupid != 0 && findaccprimgroupCr6.Accunderprimarygroupid != null)
                                            {
                                                findaccprimgroupCr6.Accgroupbalance = oldaccprimgrupbalance6 + vouchermaster.FinalTotal;
                                                findaccprimgroupCr6.DebitAmount = oldaccprimgrupcredit6 + vouchermaster.FinalTotal;
                                                ms.UpdateAccgroup(findaccprimgroupCr6);



                                                //---------------------Update Account primary group 7 credit----------------------------//
                                                var findaccprimgroupCr7 = ms.Getaccgroupbyid(findaccprimgroupCr6.Accunderprimarygroupid);
                                                var oldaccprimgrupcredit7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr7.AccgroupID).FirstOrDefault().DebitAmount;
                                                var oldaccprimgrupbalance7 = findaccprimgroupCr7.Accgroupbalance;
                                                if (findaccprimgroupCr7.Accunderprimarygroupid != 0 && findaccprimgroupCr7.Accunderprimarygroupid != null)
                                                {
                                                    findaccprimgroupCr7.Accgroupbalance = oldaccprimgrupbalance7 + vouchermaster.FinalTotal;
                                                    findaccprimgroupCr7.DebitAmount = oldaccprimgrupcredit7 + vouchermaster.FinalTotal;
                                                    ms.UpdateAccgroup(findaccprimgroupCr7);

                                                    //---------------------Update Account primary group 8 credit----------------------------//
                                                    var findaccprimgroupCr8 = ms.Getaccgroupbyid(findaccprimgroupCr7.Accunderprimarygroupid);
                                                    var oldaccprimgrupcredit8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr8.AccgroupID).FirstOrDefault().DebitAmount;
                                                    var oldaccprimgrupbalance8 = findaccprimgroupCr8.Accgroupbalance;
                                                    if (findaccprimgroupCr8.Accunderprimarygroupid != 0 && findaccprimgroupCr8.Accunderprimarygroupid != null)
                                                    {
                                                        findaccprimgroupCr8.Accgroupbalance = oldaccprimgrupbalance8 + vouchermaster.FinalTotal;
                                                        findaccprimgroupCr8.DebitAmount = oldaccprimgrupcredit8 + vouchermaster.FinalTotal;
                                                        ms.UpdateAccgroup(findaccprimgroupCr8);


                                                        //---------------------Update Account primary group 9 credit----------------------------//
                                                        var findaccprimgroupCr9 = ms.Getaccgroupbyid(findaccprimgroupCr8.Accunderprimarygroupid);
                                                        var oldaccprimgrupcredit9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr9.AccgroupID).FirstOrDefault().DebitAmount;
                                                        var oldaccprimgrupbalance9 = findaccprimgroupCr9.Accgroupbalance;
                                                        if (findaccprimgroupCr9.Accunderprimarygroupid != 0 && findaccprimgroupCr9.Accunderprimarygroupid != null)
                                                        {
                                                            findaccprimgroupCr9.Accgroupbalance = oldaccprimgrupbalance9 + vouchermaster.FinalTotal;
                                                            findaccprimgroupCr9.DebitAmount = oldaccprimgrupcredit9 + vouchermaster.FinalTotal;
                                                            ms.UpdateAccgroup(findaccprimgroupCr9);

                                                            //---------------------Update Account primary group 9 credit----------------------------//
                                                            var findaccprimgroupCr10 = ms.Getaccgroupbyid(findaccprimgroupCr9.Accunderprimarygroupid);
                                                            var oldaccprimgrupcredit10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupCr10.AccgroupID).FirstOrDefault().DebitAmount;
                                                            var oldaccprimgrupbalance10 = findaccprimgroupCr10.Accgroupbalance;
                                                            if (findaccprimgroupCr10.Accunderprimarygroupid != 0 && findaccprimgroupCr10.Accunderprimarygroupid != null)
                                                            {
                                                                findaccprimgroupCr10.Accgroupbalance = oldaccprimgrupbalance10 + vouchermaster.FinalTotal;
                                                                findaccprimgroupCr10.DebitAmount = oldaccprimgrupcredit10 + vouchermaster.FinalTotal;
                                                                ms.UpdateAccgroup(findaccprimgroupCr10);


                                                            }
                                                            else
                                                            {
                                                                findaccprimgroupCr10.Accgroupbalance = oldaccprimgrupbalance10 + vouchermaster.FinalTotal;
                                                                findaccprimgroupCr10.DebitAmount = oldaccprimgrupcredit10 + vouchermaster.FinalTotal;
                                                                ms.UpdateAccgroup(findaccprimgroupCr10);
                                                            }





                                                        }
                                                        else
                                                        {
                                                            findaccprimgroupCr9.Accgroupbalance = oldaccprimgrupbalance9 + vouchermaster.FinalTotal;
                                                            findaccprimgroupCr9.DebitAmount = oldaccprimgrupcredit9 + vouchermaster.FinalTotal;
                                                            ms.UpdateAccgroup(findaccprimgroupCr9);
                                                        }




                                                    }
                                                    else
                                                    {
                                                        findaccprimgroupCr8.Accgroupbalance = oldaccprimgrupbalance8 + vouchermaster.FinalTotal;
                                                        findaccprimgroupCr8.DebitAmount = oldaccprimgrupcredit8 + vouchermaster.FinalTotal;
                                                        ms.UpdateAccgroup(findaccprimgroupCr8);
                                                    }





                                                }
                                                else
                                                {
                                                    findaccprimgroupCr7.Accgroupbalance = oldaccprimgrupbalance7 + vouchermaster.FinalTotal;
                                                    findaccprimgroupCr7.DebitAmount = oldaccprimgrupcredit7 + vouchermaster.FinalTotal;
                                                    ms.UpdateAccgroup(findaccprimgroupCr7);
                                                }



                                            }
                                            else
                                            {
                                                findaccprimgroupCr6.Accgroupbalance = oldaccprimgrupbalance6 + vouchermaster.FinalTotal;
                                                findaccprimgroupCr6.DebitAmount = oldaccprimgrupcredit6 + vouchermaster.FinalTotal;
                                                ms.UpdateAccgroup(findaccprimgroupCr6);
                                            }




                                        }
                                        else
                                        {
                                            findaccprimgroupCr5.Accgroupbalance = oldaccprimgrupbalance5 + vouchermaster.FinalTotal;
                                            findaccprimgroupCr5.DebitAmount = oldaccprimgrupcredit5 + vouchermaster.FinalTotal;
                                            ms.UpdateAccgroup(findaccprimgroupCr5);
                                        }



                                    }
                                    else
                                    {
                                        findaccprimgroupCr4.Accgroupbalance = oldaccprimgrupbalance4 + vouchermaster.FinalTotal;
                                        findaccprimgroupCr4.DebitAmount = oldaccprimgrupcredit4 + vouchermaster.FinalTotal;
                                        ms.UpdateAccgroup(findaccprimgroupCr4);
                                    }


                                }
                                else
                                {
                                    findaccprimgroupCr3.Accgroupbalance = oldaccprimgrupbalance3 + vouchermaster.FinalTotal;
                                    findaccprimgroupCr3.DebitAmount = oldaccprimgrupcredit3 + vouchermaster.FinalTotal;
                                    ms.UpdateAccgroup(findaccprimgroupCr3);
                                }


                            }
                            else
                            {
                                findaccprimgroupCr2.Accgroupbalance = oldaccprimgrupbalance2 + vouchermaster.FinalTotal;
                                findaccprimgroupCr2.DebitAmount = oldaccprimgrupcredit2 + vouchermaster.FinalTotal;
                                ms.UpdateAccgroup(findaccprimgroupCr2);

                            }

                        }
                        else
                        {

                            findaccgroupCrr.Accgroupbalance = oldbalance + vouchermaster.FinalTotal;
                            findaccgroupCrr.DebitAmount = oldaccgrupcredit1 + vouchermaster.FinalTotal;
                            ms.UpdateAccgroup(findaccgroupCrr);

                        }
                    }






                    return RedirectToAction("AddPVoucher");
                }
                else if (vouchermaster.modify == 1)
                {
                    var oldmasterfind = db.VoucherMaster.Where(m => m.vouchermasterid == vouchermaster.VoucherNumber && m.Partyid_Accountid != 1059).FirstOrDefault();
                    var oldmasterpurchase = db.VoucherMaster.Where(m => m.vouchermasterid == vouchermaster.VoucherNumber && m.Partyid_Accountid == 1059).FirstOrDefault();
                    var oldchildfind = db.VoucherChild.Where(m => m.VoucherMasterID == oldmasterfind.vouchermasterid).ToList();

                    int billsundrynumber = db.BillSundryChild.Count() + 1;


                    double billsamount = 0;
                    int billsid = 0;
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

                        oldmasterfind.BillSundryAmount = oldmasterfind.BillSundryAmount - billsundrytotalamount;
                        oldmasterfind.BillSundryId = 0;

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

                        oldmasterfind.BillSundryAmount = oldmasterfind.BillSundryAmount + billsundrytotalamount;

                        oldmasterfind.BillSundryId = billsundrynumber;

                    }


                    //-------update vouchermaster----------//
                    oldmasterfind.Narration = vouchermaster.Narration;
                    ;
                    oldmasterfind.VoucherFinalTotal = vouchermaster.FinalTotal;
                    oldmasterpurchase.VoucherFinalTotal = vouchermaster.FinalTotal;
                    oldmasterfind.Partyid_Accountid = vouchermaster.Partyid;
                    oldmasterfind.LocationID = vouchermaster.LocationID;

                    db.Entry(oldmasterfind).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    db.Entry(oldmasterpurchase).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

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
                            StockValues.Quantity = StockValues.Quantity - deletedquantity;
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
                                ItemPrice = vouchermaster.ItemPrice[i],
                                TotalAmount = vouchermaster.TotalAmount[i],
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
                            StockValues.Quantity = StockValues.Quantity + vouchermaster.Quantity[i];
                            orderServices.UpdateStock(StockValues);
                            // orderServices.UpdateStock(StockValues);
                            //---Stockss---//

                        }



                    }




                    return RedirectToAction("AddPVoucher");


                }

                return RedirectToAction("AddPVoucher");
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }









        }
        #endregion Purhcase


        /*--------------------------------- Purhcase Return--------------------------------*/
        #region PurhcaseReturn

        public ActionResult AddPReturnVoucher()
        {

            ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
            ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
            var nullbills = db.BillSundry.Where(m => m.BiilSundryID == 3).FirstOrDefault().BillSundryName;
            ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName", nullbills);
            ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName");

            ViewBag.ordernumber = (int)db.VoucherMaster.Count() + 1;


            VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();
            vmmodel.vouchertypeid = 10;

            return View("~/Views/Sales/SPVoucher.cshtml", vmmodel);
        }

        public ActionResult ModifyPReturnvoucher()
        {
            VoucherMasterViewModel purchasehist = new VoucherMasterViewModel();
            var vouchercount = db.VoucherMaster.Where(x => x.VoucherTypeID == 10 && x.Partyid_Accountid != 1059).Count();
            purchasehist.vouchertypeid = 10;

            if (vouchercount > 0)
            {
                purchasehist.Vouchermasterlist = db.VoucherMaster.Where(m => m.VoucherTypeID == 10 && m.Partyid_Accountid != 1059).ToList();
                //var totalsale = db.VoucherMaster.Where(m => m.VoucherTypeID == 10).Sum(m => m.VoucherFinalTotal);

            }
            return View("~/Views/Purchase/ModifyPvoucher.cshtml", purchasehist);

        }
        public ActionResult NewpurchaseReturnhistory()
        {   
            var vouchercount = db.VoucherMaster.Where(x => x.VoucherTypeID == 10 && x.Partyid_Accountid != 1059).Count();
            VoucherMasterViewModel purchasehist = new VoucherMasterViewModel();

            if (vouchercount > 0)
            {
                purchasehist.Vouchermasterlist = db.VoucherMaster.Where(x => x.VoucherTypeID == 10 && x.Partyid_Accountid != 1059).ToList();
                var totalsale = db.VoucherMaster.Where(m => m.VoucherTypeID == 10 && m.Partyid_Accountid != 1059).Sum(m => m.VoucherFinalTotal);
                ViewBag.sales = totalsale;

            }
            else
            {
                purchasehist.vouchertypeid = 10;
                ViewBag.sales = null;
            }


            return View("/Views/Purchase/Newpurchasehistory.cshtml", purchasehist);
        }
        #endregion PurhcaseReturn

    }
}