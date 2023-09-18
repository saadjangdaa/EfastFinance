using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Shared;
using RMS.Web.Models;
using RMS.Web.Models.ViewModels;
using RMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMS.Web.Controllers
{
    [Authorize]

    public class SalesController : Controller
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        MasterServices ms = new MasterServices();

        #region SalesVoucher
        // GET: Sales 
        public ActionResult salehistory()
        {

            try
            {
                var vouchercount = db.VoucherMaster.Where(x => x.VoucherTypeID == 1 && x.Partyid_Accountid != 1060).Count();
                if (vouchercount > 0)
                {
                    ViewBag.sales = (int)db.VoucherMaster.Where(m => m.VoucherTypeID == 1 && m.Partyid_Accountid != 1060).Sum(m => m.VoucherFinalTotal);
                    VoucherMasterViewModel sales = new VoucherMasterViewModel();
                    sales.Vouchermasterlist = db.VoucherMaster.Where(m => m.VoucherTypeID == 1 && m.Partyid_Accountid != 1060).ToList();

                    sales.vouchertypeid = 1;
                    //var sales = from p in db.VoucherMaster
                    //            where p.VoucherTypeID == 1 
                    //            select new
                    //            {
                    //                p.VoucherCreateDate,
                    //                p.Partyid_Accountid,
                    //                p.vouchermasterid,
                    //                p.Narration,
                    //                p.VoucherFinalTotal 
                    //            }; 
                    return View(sales);

                }
                else
                {
                    VoucherMasterViewModel sales = new VoucherMasterViewModel();

                    sales.vouchertypeid = 1;

                    ViewBag.sales = null;
                    return View(sales);

                }
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("/Views/Shared/Error.cshtml");
                throw;
            }



        }
        public ActionResult Addsalevoucher()
        {
            ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
            ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
            ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName");
            ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName");

            ViewBag.ordernumber = db.VoucherMaster.Count() + 1;

            VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();
            vmmodel.vouchertypeid = 1;

            return View("~/Views/Sales/SPVoucher.cshtml", vmmodel);
        }
        public JsonResult Getstockquantity(int ItemId)
        {
            try
            {
                var quantity = ms.FinalStock(ItemId);
                return Json(quantity, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);


            }

        }


        public JsonResult GetstockquantitybyLoc(int ItemId, int LocationID)
        {
            try
            {
                var vmasters = ms.StockByLocation(ItemId, LocationID);
                var quantity = vmasters.stockquantity;


                return Json(quantity, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);


            }

        }
        public ActionResult ModifySvoucher()
        {
            VoucherMasterViewModel sales = new VoucherMasterViewModel();

            sales.Vouchermasterlist = db.VoucherMaster.Where(m => m.VoucherTypeID == 1 && m.Partyid_Accountid != 1060).ToList();
            sales.vouchertypeid = 1;
            return View(sales);
        }
        public ActionResult detailSVoucher(int id)
        {
            if (id != 0)
            {
                var vtypeid = db.VoucherMaster.Where(m => m.vouchermasterid == id).FirstOrDefault().VoucherTypeID;

                if (vtypeid == 1)
                {
                    var masterid = db.VoucherMaster.Where(m => m.vouchermasterid == id && m.DrCrType == 1).FirstOrDefault().VoucherNum_BillNum;
                    var vouchermaster = db.VoucherMaster.Find(masterid);
                    var voucherhild = db.VoucherChild.Where(m => m.VoucherMasterID == id).ToList();
                    List<BillSundryChild> billsundrychild = new List<BillSundryChild>();
                    if (vouchermaster.BillSundryId != 0)
                    {
                        billsundrychild = db.BillSundryChild.Where(m => m.Vouchermasterid == vouchermaster.vouchermasterid).ToList();

                    }

                    ViewBag.Accountname = new SelectList(db.Account, "AccountID", "AccountName", vouchermaster.Partyid_Accountid);
                    ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
                    ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
                    ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName");
                    ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID);

                    ViewBag.vouchernumber = vouchermaster.VoucherNum_BillNum;
                    ViewBag.VoucherCreateDate = vouchermaster.VoucherCreateDate;
                    ViewBag.Narration = vouchermaster.Narration;

                    double billsamount = 0.00;
                    double billsamount2 = 0.00;
                    if (vouchermaster.BillSundryId == 1)
                    {
                        billsamount = vouchermaster.BillSundryAmount;
                    }
                    else if (vouchermaster.BillSundryId == 2)
                    {
                        billsamount2 = vouchermaster.BillSundryAmount;
                    }
                    VoucherMasterViewModel masterview = new VoucherMasterViewModel
                    {
                        VoucherNumber = (int)vouchermaster.vouchermasterid,
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
                        LocationID = vouchermaster.LocationID

                    };
                    return View(masterview);
                }
                else
                {
                    var masterid = db.VoucherMaster.Where(m => m.vouchermasterid == id && m.DrCrType == 2).FirstOrDefault().VoucherNum_BillNum;
                    var vouchermaster = db.VoucherMaster.Find(masterid);
                    var voucherhild = db.VoucherChild.Where(m => m.VoucherMasterID == id).ToList();
                    List<BillSundryChild> billsundrychild = new List<BillSundryChild>();
                    if (vouchermaster.BillSundryId != 0)
                    {
                        billsundrychild = db.BillSundryChild.Where(m => m.Vouchermasterid == vouchermaster.vouchermasterid).ToList();

                    }

                    ViewBag.Accountname = new SelectList(db.Account, "AccountID", "AccountName", vouchermaster.Partyid_Accountid);
                    ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
                    ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
                    ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName");
                    ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", vouchermaster.LocationID);

                    ViewBag.vouchernumber = vouchermaster.VoucherNum_BillNum;
                    ViewBag.VoucherCreateDate = vouchermaster.VoucherCreateDate;
                    ViewBag.Narration = vouchermaster.Narration;

                    double billsamount = 0.00;
                    double billsamount2 = 0.00;
                    if (vouchermaster.BillSundryId == 1)
                    {
                        billsamount = vouchermaster.BillSundryAmount;
                    }
                    else if (vouchermaster.BillSundryId == 2)
                    {
                        billsamount2 = vouchermaster.BillSundryAmount;
                    }
                    VoucherMasterViewModel masterview = new VoucherMasterViewModel
                    {
                        VoucherNumber = (int)vouchermaster.vouchermasterid,
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
                        LocationID = vouchermaster.LocationID

                    };
                    return View(masterview);
                }

            }
            else
            {
                return RedirectToAction("Addsalevoucher");
            }

        }
        public ActionResult EditSVoucher(int id)
        {
            if (id != 0)
            {
                var masterid = db.VoucherMaster.Where(m => m.vouchermasterid == id && m.Partyid_Accountid != 1060).FirstOrDefault().VoucherNum_BillNum;



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
                    VoucherMasterID = vouchermaster.vouchermasterid
                };

                return View("~/Views/Sales/SPVoucher.cshtml", masterview);
            }
            else
            {
                return RedirectToAction("Addsalevoucher");
            }
        }
        public ActionResult PrintSaveVoucher(int? MasterID,int TypeID)
        {
            var Units = db.Units.ToList();
            var items = db.Items.ToList();
            VoucherChildViewModel model = new VoucherChildViewModel();
            model.VoucherChildList = db.VoucherChild.Where(x => x.VoucherMasterID == MasterID && x.VoucherTypeID == TypeID).Select(w => new VoucherChildViewModel
            {
                ItemID = w.ItemID,
                Quantity = w.Quantity,
                Unitid = w.Unitid,
                ItemPrice = w.ItemPrice,
                UnitName = db.Units.Where(u => u.UnitID == w.Unitid).Select(q => q.UnitName).FirstOrDefault(),
                ItemName = db.Items.Where(i => i.ItemID == w.ItemID).Select(q => q.ItemName).FirstOrDefault(),
                TotalAmount = w.TotalAmount
            }).ToList();
            model.VoucherMasterID = MasterID;
            model.VoucherTypeID = TypeID;
            model.VoucherMasterList = db.VoucherMaster.Where(w => w.vouchermasterid == MasterID).Select(s => new VoucherMasterViewModel
            {
                Partyid = s.Partyid_Accountid,
                Narration = s.Narration,
                VoucherNumber = s.VoucherNum_BillNum,
                VoucherCreateDate = s.VoucherCreateDate,
                PartyName = db.Account.Where(i => i.AccountID == s.Partyid_Accountid).Select(q => q.AccountName).FirstOrDefault(),
            }).FirstOrDefault();
            return View(model);
        }
        public ActionResult ReportSaleVoucher(int? MasterID,int? TypeID)
        {
            ReportDocument report = new ReportDocument();
          
            report.Load(Server.MapPath("~/Reports/SaleVoucher.rpt"));

            report.SetParameterValue("@vouchermasterid", MasterID);
            report.SetParameterValue("@typeid", TypeID);

            var ReportData = db.Database.SqlQuery<VoucherMasterReport>("Sp_Salevoucher @vouchermasterid, @typeid",
            new SqlParameter("vouchermasterid", MasterID),
            new SqlParameter("typeid", TypeID)).ToList();

            report.SetDataSource(ReportData);

            var stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }



        //public ActionResult ReportSaleVoucher(int? MasterID)
        //{
        //    ReportDocument rd = new ReportDocument();
        //    rd.Load(Server.MapPath("~/Reports/SaleVoucher.rpt"));
        //    rd.SetParameterValue("@vouchermasterid", MasterID);

        //    var parameter = new SqlParameter("@vouchermasterid", SqlDbType.Int);
        //    parameter.Value = MasterID == null ? 0 : MasterID; //?? (object)DBNull.Value;
        //    var data = db.Database.SqlQuery<VoucherMaster>("EXEC Sp_Salevoucher @vouchermasterid", parameter).ToList();

        //    rd.SetDataSource(data);
        //    var stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    return File(stream, "application/pdf");
        //}
        //public ActionResult ReportSaleVoucher(int? MasterID)
        //{
        //    ReportDocument rd = new ReportDocument();
        //    rd.Load(Server.MapPath("~/Reports/SaleVoucher.rpt"));
        //    rd.SetParameterValue("@vouchermasterid", MasterID);

        //    var parameter = new SqlParameter("@vouchermasterid", SqlDbType.Int);
        //    parameter.Value = MasterID == null ? 0 : MasterID;

        //    var data = db.Database.SqlQuery<VoucherMaster>("EXEC Sp_Salevoucher @vouchermasterid", parameter)
        //    .Select(item => new VoucherMaster
        //    {
        //        vouchermasterid = item.vouchermasterid,
        //        LocationID2= item.LocationID2,
        //    })
        //    .ToList();

        //    var dataSet = new DataSet();
        //    var dataTable = new DataTable();
        //    dataTable.TableName = "VoucherMaster";
        //    dataTable.Columns.Add("vouchermasterid", typeof(int));

        //    foreach (var item in data)
        //    {
        //        dataTable.Rows.Add(item.vouchermasterid);
        //    }

        //    dataSet.Tables.Add(dataTable);
        //    rd.SetDataSource(dataSet);

        //    var stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    return File(stream, "application/pdf");
        //}
        //public ActionResult _PrintSaveVoucher2(int? MasterID)
        //{
        //    var Units = db.Units.ToList();
        //    var items = db.Items.ToList();
        //    VoucherChildViewModel model = new VoucherChildViewModel();
        //    model.VoucherChildList = db.VoucherChild.Where(x => x.VoucherMasterID == MasterID && x.VoucherTypeID == 1).Select(w => new VoucherChildViewModel
        //    {
        //        ItemID = w.ItemID,
        //        Quantity = w.Quantity,
        //        Unitid = w.Unitid,
        //        ItemPrice = w.ItemPrice,
        //        ItemName = db.Items.Where(i => i.ItemID == w.ItemID).Select(q => q.ItemName).FirstOrDefault(),
        //        UnitName = db.Units.Where(u => u.UnitID == w.Unitid).Select(q => q.UnitName).FirstOrDefault()
        //    }).ToList();
        //    model.VoucherMasterID = MasterID;
        //    model.VoucherMasterList = db.VoucherMaster.Where(w => w.vouchermasterid == MasterID).Select(s => new VoucherMasterViewModel
        //    {
        //        Partyid = s.Partyid_Accountid,
        //        Narration = s.Narration,
        //        VoucherNumber = s.VoucherNum_BillNum,
        //        VoucherCreateDate = s.VoucherCreateDate,
        //        PartyName = db.Account.Where(i => i.Accgroupid == s.Partyid_Accountid).Select(q => q.AccountName).FirstOrDefault(),
        //    }).FirstOrDefault();

        //    return PartialView("_PrintSaveVoucher2", model);
        //}

        [HttpPost]
        public ActionResult SaveSVoucher(VoucherMasterViewModel vouchermaster)
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
                        if (vouchermaster.vouchertypeid == 1)
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
                                VoucherTypeID = 1,



                            };
                            db.VoucherChild.Add(voucherchild);
                            db.SaveChanges();

                        }
                        else if (vouchermaster.vouchertypeid == 9)
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
                                VoucherTypeID = 9


                            };
                            db.VoucherChild.Add(voucherchild);
                            db.SaveChanges();
                        }


                        Transactions trans = new Transactions
                        {
                            //TransactionID = 0,
                            ItemID = vouchermaster.ItemID[i],
                            Quantity = (-1) * vouchermaster.Quantity[i],
                            TransDate = DateTime.Now,
                            PayTypeID = 1,
                            ItemPrice = vouchermaster.ItemPrice[i],
                            TransType = 1
                        };
                        db.Transactions.Add(trans);
                        db.SaveChanges();
                        OrderServices orderServices = new OrderServices();

                        try
                        {
                            //---update Stonks---//
                            if (vouchermaster.vouchertypeid == 1)
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

                    if (vouchermaster.vouchertypeid == 1)
                    {

                        VoucherMaster salemaster = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = 1060,
                            Narration = "sale account credit",
                            VoucherFinalTotal = vouchermaster.FinalTotal,
                            VoucherTypeID = 1,
                            DrCrType = 2,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID



                        };
                        db.VoucherMaster.Add(salemaster);
                        db.SaveChanges();
                        ////////////////////////

                        VoucherMaster vouchermsterr = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = vouchermaster.Partyid,
                            Narration = vouchermaster.Narration,
                            BillSundryId = billsundrynumber,
                            BillSundryAmount = billsundrytotalamount,
                            VoucherFinalTotal = vouchermaster.FinalTotal,
                            VoucherTypeID = 1,
                            DrCrType = 1,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID


                        };
                        db.VoucherMaster.Add(vouchermsterr);
                        db.SaveChanges();
                    }
                    else if (vouchermaster.vouchertypeid == 9)
                    {
                        VoucherMaster salemaster = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = 1060,
                            Narration = "sale account debit",
                            BillSundryAmount = billsundrytotalamount,
                            VoucherFinalTotal = vouchermaster.FinalTotal,
                            VoucherTypeID = 9,
                            DrCrType = 1,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID

                        };
                        db.VoucherMaster.Add(salemaster);
                        db.SaveChanges();
                        ////////////////////////
                        VoucherMaster vouchermsterr = new VoucherMaster
                        {
                            //VoucherNum_BillNum = 0,
                            VoucherCreateDate = vouchermaster.VoucherCreateDate,
                            Partyid_Accountid = vouchermaster.Partyid,
                            Narration = vouchermaster.Narration,
                            BillSundryId = billsundrynumber,
                            BillSundryAmount = billsundrytotalamount,
                            VoucherFinalTotal = vouchermaster.FinalTotal,
                            VoucherTypeID = 9,
                            DrCrType = 2,
                            vouchermasterid = vouchermasterid2,
                            LocationID = vouchermaster.LocationID

                        };
                        db.VoucherMaster.Add(vouchermsterr);
                        db.SaveChanges();
                    }




                    if (vouchermaster.vouchertypeid == 1)
                    {
                        //var masterid = db.VoucherMaster.Where(m => m.vouchermasterid == 76).FirstOrDefault().VoucherNum_BillNum;
                        //var updatesalesvoucher = db.VoucherMaster.Find(masterid);
                        //updatesalesvoucher.VoucherFinalTotal = updatesalesvoucher.VoucherFinalTotal + vouchermaster.FinalTotal;
                        //db.Entry(updatesalesvoucher).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();


                        //-------------------Update Revenue Group----------------------//

                        var findaccidsales = db.AccGroup.Where(m => m.AccgroupID == 24).FirstOrDefault().AccgroupID;
                        var findaccgroupsales = ms.Getaccgroupbyid(findaccidsales);
                        var oldcreditsales = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales.AccgroupID).FirstOrDefault().CreditAmount;
                        var oldbalancesales = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales.AccgroupID).FirstOrDefault().Accgroupbalance;

                        findaccgroupsales.CreditAmount = oldcreditsales + vouchermaster.FinalTotal;
                        findaccgroupsales.Accgroupbalance = oldbalancesales - vouchermaster.FinalTotal;
                        ms.UpdateAccgroup(findaccgroupsales);


                        //-------------------Update Sales Group----------------------//

                        var findaccidsales2 = db.AccGroup.Where(m => m.AccgroupID == 1063).FirstOrDefault().AccgroupID;
                        var findaccgroupsales2 = ms.Getaccgroupbyid(findaccidsales2);
                        var oldcreditsales2 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales2.AccgroupID).FirstOrDefault().CreditAmount;
                        var oldbalancesales2 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales2.AccgroupID).FirstOrDefault().Accgroupbalance;

                        findaccgroupsales2.CreditAmount = oldcreditsales2 + vouchermaster.FinalTotal;
                        findaccgroupsales2.Accgroupbalance = oldbalancesales2 - vouchermaster.FinalTotal;
                        ms.UpdateAccgroup(findaccgroupsales2);


                        ////-------------------Update Sales accouint----------------------//

                        var findaccidsales3 = db.Account.Where(m => m.Accgroupid == findaccidsales2).FirstOrDefault().AccountID;
                        var findaccgroupsales3 = ms.Getaccountbyid(findaccidsales3);
                        var oldcreditsales3 = db.Account.Where(m => m.AccountID == findaccgroupsales3.AccountID).FirstOrDefault().CreditTotal;
                        var oldbalancesales3 = db.Account.Where(m => m.AccountID == findaccgroupsales3.AccountID).FirstOrDefault().ClosingBalance;

                        findaccgroupsales3.CreditTotal = oldcreditsales3 + vouchermaster.FinalTotal;
                        findaccgroupsales3.ClosingBalance = oldbalancesales3 - vouchermaster.FinalTotal;
                        ms.Updateaccount(findaccgroupsales3);


                        //----------Update account Balance----------//
                        var findaccount = ms.Getaccountbyid(vouchermaster.Partyid);
                        var oldbalance = db.Account.Where(x => x.AccountID == vouchermaster.Partyid).FirstOrDefault().ClosingBalance;
                        var olddebit = db.Account.Where(x => x.AccountID == vouchermaster.Partyid).FirstOrDefault().DebitTotal;

                        findaccount.ClosingBalance = oldbalance + vouchermaster.FinalTotal;
                        findaccount.DebitTotal = olddebit + vouchermaster.FinalTotal;
                        ms.Updateaccount(findaccount);




                        //----------Update account Group 1 Balance----------//
                        var findaccgroup = ms.Getaccgroupbyid(findaccount.Accgroupid);
                        var oldaccgrupdebit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().DebitAmount;
                        var oldaccgrupbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().Accgroupbalance;

                        if (findaccgroup.Accunderprimarygroupid != 0 && findaccgroup.Accunderprimarygroupid != null)
                        {
                            findaccgroup.Accgroupbalance = oldaccgrupbalance + vouchermaster.FinalTotal;
                            findaccgroup.DebitAmount = oldaccgrupdebit1 + vouchermaster.FinalTotal;
                            ms.UpdateAccgroup(findaccgroup);

                            //---------------------Update Account primary group 2 credit----------------------------//

                            var findaccprimgroupDr = ms.Getaccgroupbyid(findaccgroup.Accunderprimarygroupid);
                            var oldaccprimgrupdebit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().DebitAmount;
                            var oldaccprimbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().Accgroupbalance;

                            if (findaccprimgroupDr.Accunderprimarygroupid != 0 && findaccprimgroupDr.Accunderprimarygroupid != null)
                            {
                                findaccprimgroupDr.Accgroupbalance = oldaccprimbalance2 + vouchermaster.FinalTotal;
                                findaccprimgroupDr.DebitAmount = oldaccprimgrupdebit2 + vouchermaster.FinalTotal;
                                ms.UpdateAccgroup(findaccprimgroupDr);


                                //---------------------Update Account group 3 debit----------------------------//

                                var findaccprimgroupDr3 = ms.Getaccgroupbyid(findaccprimgroupDr.Accunderprimarygroupid);
                                var oldaccprimgrupdebit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr3.AccgroupID).FirstOrDefault().DebitAmount;
                                var oldaccprimbalance3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr3.AccgroupID).FirstOrDefault().Accgroupbalance;

                                if (findaccprimgroupDr3.Accunderprimarygroupid != 0 && findaccprimgroupDr3.Accunderprimarygroupid != null)
                                {
                                    findaccprimgroupDr3.Accgroupbalance = oldaccprimbalance3 + vouchermaster.FinalTotal;
                                    findaccprimgroupDr3.DebitAmount = oldaccprimgrupdebit3 + vouchermaster.FinalTotal;
                                    ms.UpdateAccgroup(findaccprimgroupDr3);


                                    //---------------------Update Account group 4 debit----------------------------//
                                    var findaccprimgroupDr4 = ms.Getaccgroupbyid(findaccprimgroupDr3.Accunderprimarygroupid);
                                    var oldaccprimgrupdebit4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr4.AccgroupID).FirstOrDefault().DebitAmount;
                                    var oldaccprimbalance4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr4.AccgroupID).FirstOrDefault().Accgroupbalance;

                                    if (findaccprimgroupDr4.Accunderprimarygroupid != 0 && findaccprimgroupDr4.Accunderprimarygroupid != null)
                                    {
                                        findaccprimgroupDr4.Accgroupbalance = oldaccprimbalance4 + vouchermaster.FinalTotal;
                                        findaccprimgroupDr4.DebitAmount = oldaccprimgrupdebit4 + vouchermaster.FinalTotal;
                                        ms.UpdateAccgroup(findaccprimgroupDr4);

                                        //---------------------Update Account group 5 debit----------------------------//
                                        var findaccprimgroupDr5 = ms.Getaccgroupbyid(findaccprimgroupDr4.Accunderprimarygroupid);
                                        var oldaccprimgrupdebit5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr5.AccgroupID).FirstOrDefault().DebitAmount;
                                        var oldaccprimbalance5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr5.AccgroupID).FirstOrDefault().Accgroupbalance;
                                        if (findaccprimgroupDr5.Accunderprimarygroupid != 0 && findaccprimgroupDr5.Accunderprimarygroupid != null)
                                        {
                                            findaccprimgroupDr5.Accgroupbalance = oldaccprimbalance5 + vouchermaster.FinalTotal;
                                            findaccprimgroupDr5.DebitAmount = oldaccprimgrupdebit5 + vouchermaster.FinalTotal;
                                            ms.UpdateAccgroup(findaccprimgroupDr5);


                                            //---------------------Update Account group 6 debit----------------------------//
                                            var findaccprimgroupDr6 = ms.Getaccgroupbyid(findaccprimgroupDr5.Accunderprimarygroupid);
                                            var oldaccprimgrupdebit6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr6.AccgroupID).FirstOrDefault().DebitAmount;
                                            var oldaccprimbalance6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr6.AccgroupID).FirstOrDefault().Accgroupbalance;
                                            if (findaccprimgroupDr6.Accunderprimarygroupid != 0 && findaccprimgroupDr6.Accunderprimarygroupid != null)
                                            {
                                                findaccprimgroupDr6.Accgroupbalance = oldaccprimbalance6 + vouchermaster.FinalTotal;
                                                findaccprimgroupDr6.DebitAmount = oldaccprimgrupdebit6 + vouchermaster.FinalTotal;
                                                ms.UpdateAccgroup(findaccprimgroupDr6);

                                                //---------------------Update Account group 7 debit----------------------------//
                                                var findaccprimgroupDr7 = ms.Getaccgroupbyid(findaccprimgroupDr6.Accunderprimarygroupid);
                                                var oldaccprimgrupdebit7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr7.AccgroupID).FirstOrDefault().DebitAmount;
                                                var oldaccprimbalance7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr7.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                if (findaccprimgroupDr7.Accunderprimarygroupid != 0 && findaccprimgroupDr7.Accunderprimarygroupid != null)
                                                {
                                                    findaccprimgroupDr7.Accgroupbalance = oldaccprimbalance7 + vouchermaster.FinalTotal;
                                                    findaccprimgroupDr7.DebitAmount = oldaccprimgrupdebit7 + vouchermaster.FinalTotal;
                                                    ms.UpdateAccgroup(findaccprimgroupDr7);


                                                    //---------------------Update Account group 8 debit----------------------------//
                                                    var findaccprimgroupDr8 = ms.Getaccgroupbyid(findaccprimgroupDr7.Accunderprimarygroupid);
                                                    var oldaccprimgrupdebit8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr8.AccgroupID).FirstOrDefault().DebitAmount;
                                                    var oldaccprimbalance8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr8.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                    if (findaccprimgroupDr8.Accunderprimarygroupid != 0 && findaccprimgroupDr8.Accunderprimarygroupid != null)
                                                    {
                                                        findaccprimgroupDr8.Accgroupbalance = oldaccprimbalance8 + vouchermaster.FinalTotal;
                                                        findaccprimgroupDr8.DebitAmount = oldaccprimgrupdebit8 + vouchermaster.FinalTotal;
                                                        ms.UpdateAccgroup(findaccprimgroupDr8);


                                                        //---------------------Update Account group 9 debit----------------------------//
                                                        var findaccprimgroupDr9 = ms.Getaccgroupbyid(findaccprimgroupDr8.Accunderprimarygroupid);
                                                        var oldaccprimgrupdebit9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr9.AccgroupID).FirstOrDefault().DebitAmount;
                                                        var oldaccprimbalance9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr9.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                        if (findaccprimgroupDr9.Accunderprimarygroupid != 0 && findaccprimgroupDr9.Accunderprimarygroupid != null)
                                                        {
                                                            findaccprimgroupDr9.Accgroupbalance = oldaccprimbalance9 + vouchermaster.FinalTotal;
                                                            findaccprimgroupDr9.DebitAmount = oldaccprimgrupdebit9 + vouchermaster.FinalTotal;
                                                            ms.UpdateAccgroup(findaccprimgroupDr9);

                                                            //---------------------Update Account group 10 debit----------------------------//
                                                            var findaccprimgroupDr10 = ms.Getaccgroupbyid(findaccprimgroupDr9.Accunderprimarygroupid);
                                                            var oldaccprimgrupdebit10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr10.AccgroupID).FirstOrDefault().DebitAmount;
                                                            var oldaccprimbalance10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr10.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                            if (findaccprimgroupDr10.Accunderprimarygroupid != 0 && findaccprimgroupDr10.Accunderprimarygroupid != null)
                                                            {
                                                                findaccprimgroupDr10.Accgroupbalance = oldaccprimbalance10 + vouchermaster.FinalTotal;
                                                                findaccprimgroupDr10.DebitAmount = oldaccprimgrupdebit10 + vouchermaster.FinalTotal;
                                                                ms.UpdateAccgroup(findaccprimgroupDr10);




                                                            }
                                                            else
                                                            {
                                                                findaccprimgroupDr10.Accgroupbalance = oldaccprimbalance10 + vouchermaster.FinalTotal;
                                                                findaccprimgroupDr10.DebitAmount = oldaccprimgrupdebit10 + vouchermaster.FinalTotal;
                                                                ms.UpdateAccgroup(findaccprimgroupDr10);

                                                            }


                                                        }
                                                        else
                                                        {
                                                            findaccprimgroupDr9.Accgroupbalance = oldaccprimbalance9 + vouchermaster.FinalTotal;
                                                            findaccprimgroupDr9.DebitAmount = oldaccprimgrupdebit9 + vouchermaster.FinalTotal;
                                                            ms.UpdateAccgroup(findaccprimgroupDr9);

                                                        }


                                                    }
                                                    else
                                                    {
                                                        findaccprimgroupDr8.Accgroupbalance = oldaccprimbalance8 + vouchermaster.FinalTotal;
                                                        findaccprimgroupDr8.DebitAmount = oldaccprimgrupdebit8 + vouchermaster.FinalTotal;
                                                        ms.UpdateAccgroup(findaccprimgroupDr8);

                                                    }


                                                }
                                                else
                                                {
                                                    findaccprimgroupDr7.Accgroupbalance = oldaccprimbalance7 + vouchermaster.FinalTotal;
                                                    findaccprimgroupDr7.DebitAmount = oldaccprimgrupdebit7 + vouchermaster.FinalTotal;
                                                    ms.UpdateAccgroup(findaccprimgroupDr7);

                                                }



                                            }
                                            else
                                            {
                                                findaccprimgroupDr6.Accgroupbalance = oldaccprimbalance6 + vouchermaster.FinalTotal;
                                                findaccprimgroupDr6.DebitAmount = oldaccprimgrupdebit6 + vouchermaster.FinalTotal;
                                                ms.UpdateAccgroup(findaccprimgroupDr6);

                                            }

                                        }
                                        else
                                        {
                                            findaccprimgroupDr5.Accgroupbalance = oldaccprimbalance5 + vouchermaster.FinalTotal;
                                            findaccprimgroupDr5.DebitAmount = oldaccprimgrupdebit5 + vouchermaster.FinalTotal;
                                            ms.UpdateAccgroup(findaccprimgroupDr5);

                                        }

                                    }
                                    else
                                    {
                                        findaccprimgroupDr4.Accgroupbalance = oldaccprimbalance4 + vouchermaster.FinalTotal;
                                        findaccprimgroupDr4.DebitAmount = oldaccprimgrupdebit4 + vouchermaster.FinalTotal;
                                        ms.UpdateAccgroup(findaccprimgroupDr4);

                                    }

                                }
                                else
                                {
                                    findaccprimgroupDr3.Accgroupbalance = oldaccprimbalance3 + vouchermaster.FinalTotal;
                                    findaccprimgroupDr3.DebitAmount = oldaccprimgrupdebit3 + vouchermaster.FinalTotal;
                                    ms.UpdateAccgroup(findaccprimgroupDr3);

                                }

                            }
                            else
                            {
                                findaccprimgroupDr.Accgroupbalance = oldaccprimbalance2 + vouchermaster.FinalTotal;
                                findaccprimgroupDr.DebitAmount = oldaccprimgrupdebit2 + vouchermaster.FinalTotal;
                                ms.UpdateAccgroup(findaccprimgroupDr);

                            }


                        }
                        else
                        {
                            findaccgroup.Accgroupbalance = oldaccgrupbalance + vouchermaster.FinalTotal;
                            findaccgroup.DebitAmount = oldaccgrupdebit1 + vouchermaster.FinalTotal;
                            ms.UpdateAccgroup(findaccgroup);
                        }



                    }
                    else if (vouchermaster.vouchertypeid == 9)
                    {
                        //var masterid = db.VoucherMaster.Where(m => m.vouchermasterid == 76).FirstOrDefault().VoucherNum_BillNum;
                        //var updatesalesvoucher = db.VoucherMaster.Find(masterid);
                        //updatesalesvoucher.VoucherFinalTotal = updatesalesvoucher.VoucherFinalTotal - vouchermaster.FinalTotal;
                        //db.Entry(updatesalesvoucher).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();



                        //-------------------Update Revenue Group----------------------//

                        var findaccidsales = db.AccGroup.Where(m => m.AccgroupID == 24).FirstOrDefault().AccgroupID;
                        var findaccgroupsales = ms.Getaccgroupbyid(findaccidsales);
                        var olddebitsales = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales.AccgroupID).FirstOrDefault().DebitAmount;
                        var oldbalancesales = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales.AccgroupID).FirstOrDefault().Accgroupbalance;

                        findaccgroupsales.DebitAmount = olddebitsales + vouchermaster.FinalTotal;
                        findaccgroupsales.Accgroupbalance = oldbalancesales + vouchermaster.FinalTotal;
                        ms.UpdateAccgroup(findaccgroupsales);


                        //-------------------Update Sales Group----------------------//

                        var findaccidsales2 = db.AccGroup.Where(m => m.AccgroupID == 1063).FirstOrDefault().AccgroupID;
                        var findaccgroupsales2 = ms.Getaccgroupbyid(findaccidsales2);
                        var olddebitsales2 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales2.AccgroupID).FirstOrDefault().DebitAmount;
                        var oldbalancesales2 = db.AccGroup.Where(m => m.AccgroupID == findaccgroupsales2.AccgroupID).FirstOrDefault().Accgroupbalance;

                        findaccgroupsales2.DebitAmount = olddebitsales2 + vouchermaster.FinalTotal;
                        findaccgroupsales2.Accgroupbalance = oldbalancesales2 + vouchermaster.FinalTotal;
                        ms.UpdateAccgroup(findaccgroupsales2);


                        ////-------------------Update Sales accouint----------------------//

                        var findaccidsales3 = db.Account.Where(m => m.Accgroupid == findaccidsales2).FirstOrDefault().AccountID;
                        var findaccgroupsales3 = ms.Getaccountbyid(findaccidsales3);
                        var olddebitsales3 = db.Account.Where(m => m.AccountID == findaccgroupsales3.AccountID).FirstOrDefault().DebitTotal;
                        var oldbalancesales3 = db.Account.Where(m => m.AccountID == findaccgroupsales3.AccountID).FirstOrDefault().ClosingBalance;

                        findaccgroupsales3.DebitTotal = olddebitsales3 + vouchermaster.FinalTotal;
                        findaccgroupsales3.ClosingBalance = oldbalancesales3 + vouchermaster.FinalTotal;
                        ms.Updateaccount(findaccgroupsales3);


                        //----------Update account Balance----------//
                        var findaccount = ms.Getaccountbyid(vouchermaster.Partyid);
                        var oldbalance = db.Account.Where(x => x.AccountID == vouchermaster.Partyid).FirstOrDefault().ClosingBalance;
                        var olddebit = db.Account.Where(x => x.AccountID == vouchermaster.Partyid).FirstOrDefault().DebitTotal;

                        findaccount.ClosingBalance = oldbalance + vouchermaster.FinalTotal;
                        findaccount.DebitTotal = olddebit + vouchermaster.FinalTotal;
                        ms.Updateaccount(findaccount);










                        //----------Update account Group 1 Balance----------//
                        var findaccgroup = ms.Getaccgroupbyid(findaccount.Accgroupid);
                        var oldaccgrupdebit1 = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().CreditAmount;
                        var oldaccgrupbalance = db.AccGroup.Where(m => m.AccgroupID == findaccgroup.AccgroupID).FirstOrDefault().Accgroupbalance;

                        if (findaccgroup.Accunderprimarygroupid != 0 && findaccgroup.Accunderprimarygroupid != null)
                        {
                            findaccgroup.Accgroupbalance = oldaccgrupbalance - vouchermaster.FinalTotal;
                            findaccgroup.CreditAmount = oldaccgrupdebit1 + vouchermaster.FinalTotal;
                            ms.UpdateAccgroup(findaccgroup);

                            //---------------------Update Account primary group 2 credit----------------------------//

                            var findaccprimgroupDr = ms.Getaccgroupbyid(findaccgroup.Accunderprimarygroupid);
                            var oldaccprimgrupdebit2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().CreditAmount;
                            var oldaccprimbalance2 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr.AccgroupID).FirstOrDefault().Accgroupbalance;

                            if (findaccprimgroupDr.Accunderprimarygroupid != 0 && findaccprimgroupDr.Accunderprimarygroupid != null)
                            {
                                findaccprimgroupDr.Accgroupbalance = oldaccprimbalance2 - vouchermaster.FinalTotal;
                                findaccprimgroupDr.CreditAmount = oldaccprimgrupdebit2 + vouchermaster.FinalTotal;
                                ms.UpdateAccgroup(findaccprimgroupDr);


                                //---------------------Update Account group 3 debit----------------------------//

                                var findaccprimgroupDr3 = ms.Getaccgroupbyid(findaccprimgroupDr.Accunderprimarygroupid);
                                var oldaccprimgrupdebit3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr3.AccgroupID).FirstOrDefault().CreditAmount;
                                var oldaccprimbalance3 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr3.AccgroupID).FirstOrDefault().Accgroupbalance;

                                if (findaccprimgroupDr3.Accunderprimarygroupid != 0 && findaccprimgroupDr3.Accunderprimarygroupid != null)
                                {
                                    findaccprimgroupDr3.Accgroupbalance = oldaccprimbalance3 - vouchermaster.FinalTotal;
                                    findaccprimgroupDr3.CreditAmount = oldaccprimgrupdebit3 + vouchermaster.FinalTotal;
                                    ms.UpdateAccgroup(findaccprimgroupDr3);


                                    //---------------------Update Account group 4 debit----------------------------//
                                    var findaccprimgroupDr4 = ms.Getaccgroupbyid(findaccprimgroupDr3.Accunderprimarygroupid);
                                    var oldaccprimgrupdebit4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr4.AccgroupID).FirstOrDefault().CreditAmount;
                                    var oldaccprimbalance4 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr4.AccgroupID).FirstOrDefault().Accgroupbalance;

                                    if (findaccprimgroupDr4.Accunderprimarygroupid != 0 && findaccprimgroupDr4.Accunderprimarygroupid != null)
                                    {
                                        findaccprimgroupDr4.Accgroupbalance = oldaccprimbalance4 - vouchermaster.FinalTotal;
                                        findaccprimgroupDr4.CreditAmount = oldaccprimgrupdebit4 + vouchermaster.FinalTotal;
                                        ms.UpdateAccgroup(findaccprimgroupDr4);

                                        //---------------------Update Account group 5 debit----------------------------//
                                        var findaccprimgroupDr5 = ms.Getaccgroupbyid(findaccprimgroupDr4.Accunderprimarygroupid);
                                        var oldaccprimgrupdebit5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr5.AccgroupID).FirstOrDefault().CreditAmount;
                                        var oldaccprimbalance5 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr5.AccgroupID).FirstOrDefault().Accgroupbalance;
                                        if (findaccprimgroupDr5.Accunderprimarygroupid != 0 && findaccprimgroupDr5.Accunderprimarygroupid != null)
                                        {
                                            findaccprimgroupDr5.Accgroupbalance = oldaccprimbalance5 - vouchermaster.FinalTotal;
                                            findaccprimgroupDr5.CreditAmount = oldaccprimgrupdebit5 + vouchermaster.FinalTotal;
                                            ms.UpdateAccgroup(findaccprimgroupDr5);


                                            //---------------------Update Account group 6 debit----------------------------//
                                            var findaccprimgroupDr6 = ms.Getaccgroupbyid(findaccprimgroupDr5.Accunderprimarygroupid);
                                            var oldaccprimgrupdebit6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr6.AccgroupID).FirstOrDefault().CreditAmount;
                                            var oldaccprimbalance6 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr6.AccgroupID).FirstOrDefault().Accgroupbalance;
                                            if (findaccprimgroupDr6.Accunderprimarygroupid != 0 && findaccprimgroupDr6.Accunderprimarygroupid != null)
                                            {
                                                findaccprimgroupDr6.Accgroupbalance = oldaccprimbalance6 - vouchermaster.FinalTotal;
                                                findaccprimgroupDr6.CreditAmount = oldaccprimgrupdebit6 + vouchermaster.FinalTotal;
                                                ms.UpdateAccgroup(findaccprimgroupDr6);

                                                //---------------------Update Account group 7 debit----------------------------//
                                                var findaccprimgroupDr7 = ms.Getaccgroupbyid(findaccprimgroupDr6.Accunderprimarygroupid);
                                                var oldaccprimgrupdebit7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr7.AccgroupID).FirstOrDefault().CreditAmount;
                                                var oldaccprimbalance7 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr7.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                if (findaccprimgroupDr7.Accunderprimarygroupid != 0 && findaccprimgroupDr7.Accunderprimarygroupid != null)
                                                {
                                                    findaccprimgroupDr7.Accgroupbalance = oldaccprimbalance7 - vouchermaster.FinalTotal;
                                                    findaccprimgroupDr7.CreditAmount = oldaccprimgrupdebit7 + vouchermaster.FinalTotal;
                                                    ms.UpdateAccgroup(findaccprimgroupDr7);


                                                    //---------------------Update Account group 8 debit----------------------------//
                                                    var findaccprimgroupDr8 = ms.Getaccgroupbyid(findaccprimgroupDr7.Accunderprimarygroupid);
                                                    var oldaccprimgrupdebit8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr8.AccgroupID).FirstOrDefault().CreditAmount;
                                                    var oldaccprimbalance8 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr8.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                    if (findaccprimgroupDr8.Accunderprimarygroupid != 0 && findaccprimgroupDr8.Accunderprimarygroupid != null)
                                                    {
                                                        findaccprimgroupDr8.Accgroupbalance = oldaccprimbalance8 - vouchermaster.FinalTotal;
                                                        findaccprimgroupDr8.CreditAmount = oldaccprimgrupdebit8 + vouchermaster.FinalTotal;
                                                        ms.UpdateAccgroup(findaccprimgroupDr8);


                                                        //---------------------Update Account group 9 debit----------------------------//
                                                        var findaccprimgroupDr9 = ms.Getaccgroupbyid(findaccprimgroupDr8.Accunderprimarygroupid);
                                                        var oldaccprimgrupdebit9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr9.AccgroupID).FirstOrDefault().CreditAmount;
                                                        var oldaccprimbalance9 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr9.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                        if (findaccprimgroupDr9.Accunderprimarygroupid != 0 && findaccprimgroupDr9.Accunderprimarygroupid != null)
                                                        {
                                                            findaccprimgroupDr9.Accgroupbalance = oldaccprimbalance9 - vouchermaster.FinalTotal;
                                                            findaccprimgroupDr9.CreditAmount = oldaccprimgrupdebit9 + vouchermaster.FinalTotal;
                                                            ms.UpdateAccgroup(findaccprimgroupDr9);

                                                            //---------------------Update Account group 10 debit----------------------------//
                                                            var findaccprimgroupDr10 = ms.Getaccgroupbyid(findaccprimgroupDr9.Accunderprimarygroupid);
                                                            var oldaccprimgrupdebit10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr10.AccgroupID).FirstOrDefault().CreditAmount;
                                                            var oldaccprimbalance10 = db.AccGroup.Where(m => m.AccgroupID == findaccprimgroupDr10.AccgroupID).FirstOrDefault().Accgroupbalance;
                                                            if (findaccprimgroupDr10.Accunderprimarygroupid != 0 && findaccprimgroupDr10.Accunderprimarygroupid != null)
                                                            {
                                                                findaccprimgroupDr10.Accgroupbalance = oldaccprimbalance10 - vouchermaster.FinalTotal;
                                                                findaccprimgroupDr10.CreditAmount = oldaccprimgrupdebit10 + vouchermaster.FinalTotal;
                                                                ms.UpdateAccgroup(findaccprimgroupDr10);




                                                            }
                                                            else
                                                            {
                                                                findaccprimgroupDr10.Accgroupbalance = oldaccprimbalance10 - vouchermaster.FinalTotal;
                                                                findaccprimgroupDr10.CreditAmount = oldaccprimgrupdebit10 + vouchermaster.FinalTotal;
                                                                ms.UpdateAccgroup(findaccprimgroupDr10);

                                                            }


                                                        }
                                                        else
                                                        {
                                                            findaccprimgroupDr9.Accgroupbalance = oldaccprimbalance9 - vouchermaster.FinalTotal;
                                                            findaccprimgroupDr9.CreditAmount = oldaccprimgrupdebit9 + vouchermaster.FinalTotal;
                                                            ms.UpdateAccgroup(findaccprimgroupDr9);

                                                        }


                                                    }
                                                    else
                                                    {
                                                        findaccprimgroupDr8.Accgroupbalance = oldaccprimbalance8 - vouchermaster.FinalTotal;
                                                        findaccprimgroupDr8.CreditAmount = oldaccprimgrupdebit8 + vouchermaster.FinalTotal;
                                                        ms.UpdateAccgroup(findaccprimgroupDr8);

                                                    }


                                                }
                                                else
                                                {
                                                    findaccprimgroupDr7.Accgroupbalance = oldaccprimbalance7 - vouchermaster.FinalTotal;
                                                    findaccprimgroupDr7.CreditAmount = oldaccprimgrupdebit7 + vouchermaster.FinalTotal;
                                                    ms.UpdateAccgroup(findaccprimgroupDr7);

                                                }



                                            }
                                            else
                                            {
                                                findaccprimgroupDr6.Accgroupbalance = oldaccprimbalance6 - vouchermaster.FinalTotal;
                                                findaccprimgroupDr6.CreditAmount = oldaccprimgrupdebit6 + vouchermaster.FinalTotal;
                                                ms.UpdateAccgroup(findaccprimgroupDr6);

                                            }

                                        }
                                        else
                                        {
                                            findaccprimgroupDr5.Accgroupbalance = oldaccprimbalance5 - vouchermaster.FinalTotal;
                                            findaccprimgroupDr5.CreditAmount = oldaccprimgrupdebit5 + vouchermaster.FinalTotal;
                                            ms.UpdateAccgroup(findaccprimgroupDr5);

                                        }

                                    }
                                    else
                                    {
                                        findaccprimgroupDr4.Accgroupbalance = oldaccprimbalance4 - vouchermaster.FinalTotal;
                                        findaccprimgroupDr4.CreditAmount = oldaccprimgrupdebit4 + vouchermaster.FinalTotal;
                                        ms.UpdateAccgroup(findaccprimgroupDr4);

                                    }

                                }
                                else
                                {
                                    findaccprimgroupDr3.Accgroupbalance = oldaccprimbalance3 - vouchermaster.FinalTotal;
                                    findaccprimgroupDr3.CreditAmount = oldaccprimgrupdebit3 + vouchermaster.FinalTotal;
                                    ms.UpdateAccgroup(findaccprimgroupDr3);

                                }

                            }
                            else
                            {
                                findaccprimgroupDr.Accgroupbalance = oldaccprimbalance2 - vouchermaster.FinalTotal;
                                findaccprimgroupDr.CreditAmount = oldaccprimgrupdebit2 + vouchermaster.FinalTotal;
                                ms.UpdateAccgroup(findaccprimgroupDr);

                            }


                        }
                        else
                        {
                            findaccgroup.Accgroupbalance = oldaccgrupbalance - vouchermaster.FinalTotal;
                            findaccgroup.CreditAmount = oldaccgrupdebit1 + vouchermaster.FinalTotal;
                            ms.UpdateAccgroup(findaccgroup);
                        }
                    }







                    //var oldbalanceaccprgroupid = db.AccGroup.Where(m=>m.)

                }
                else if (vouchermaster.modify == 1)
                {
                    var oldmasterfind = db.VoucherMaster.Where(m => m.vouchermasterid == vouchermaster.VoucherNumber && m.Partyid_Accountid != 1060).FirstOrDefault();
                    var oldmastersales = db.VoucherMaster.Where(m => m.vouchermasterid == vouchermaster.VoucherNumber && m.Partyid_Accountid == 1060).FirstOrDefault();

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
                        oldmasterfind.BillSundryId = billsundrynumber;

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
                    oldmasterfind.VoucherFinalTotal = vouchermaster.FinalTotal;
                    oldmasterfind.Partyid_Accountid = vouchermaster.Partyid;
                    oldmasterfind.LocationID = vouchermaster.LocationID;

                    db.Entry(oldmasterfind).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    oldmastersales.VoucherFinalTotal = vouchermaster.FinalTotal;
                    db.Entry(oldmastersales).State = System.Data.Entity.EntityState.Modified;
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
                            StockValues.Quantity = StockValues.Quantity - vouchermaster.Quantity[i];
                            orderServices.UpdateStock(StockValues);
                            // orderServices.UpdateStock(StockValues);
                            //---Stockss---//

                        }



                    }

                }

                return RedirectToAction("salehistory");
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }
        }

        #endregion SalesVoucher

        //---------------------------Sale Return Voucher--------------////
        #region SalesReturn


        public ActionResult AddSaleReturn()
        {
            ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
            ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
            ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName");
            ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName");

            ViewBag.ordernumber = db.VoucherMaster.Count() + 1;

            VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();
            vmmodel.vouchertypeid = 9;

            return View("~/Views/Sales/SPVoucher.cshtml", vmmodel);

        }

        public ActionResult SaleReturnHistoy()
        {
            var vouchercount = db.VoucherMaster.Where(x => x.VoucherTypeID == 9 && x.Partyid_Accountid != 1060).Count();
            //List<VoucherMaster> sales;
            VoucherMasterViewModel sales = new VoucherMasterViewModel();

            if (vouchercount > 0)
            {
                sales.Vouchermasterlist = db.VoucherMaster.Where(m => m.VoucherTypeID == 9 && m.Partyid_Accountid != 1060).ToList();
                sales.vouchertypeid = 9;
                ViewBag.sales = db.VoucherMaster.Where(m => m.VoucherTypeID == 9 && m.Partyid_Accountid != 1060).Sum(m => m.VoucherFinalTotal);
            }
            else
            {
                sales.vouchertypeid = 9;
                ViewBag.sales = 0;
            }

            return View("~/Views/Sales/salehistory.cshtml", sales);
        }

        public ActionResult ModifySReturnvoucher()
        {

            VoucherMasterViewModel sales = new VoucherMasterViewModel();
            sales.Vouchermasterlist = db.VoucherMaster.Where(m => m.VoucherTypeID == 9 && m.Partyid_Accountid != 1060).ToList();
            sales.vouchertypeid = 9;
            return View("/Views/Sales/ModifySvoucher.cshtml", sales);
        }
        #endregion SalesReturn



    }
}