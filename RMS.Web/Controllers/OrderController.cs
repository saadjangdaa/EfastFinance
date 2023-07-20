using RMS.Web.Models;
using RMS.Web.Models.ViewModels;
using RMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMS.Web.Controllers
{
    public class OrderController : Controller
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        // GET: Order
        public ActionResult Index(ordermaster om)
        {
            ViewData["CustomerID"] = new SelectList(db.Customers, "CustomerID", "CustomerName",om.CustomerID);
            //ViewBag.GetItems = new SelectList(db.Items, "ItemID", "ItemName");
            ViewBag.GetPayTypes = new SelectList(db.PaymentTypes, "PaymentTypeID", "PaymentTypeName");
            ViewBag.OrderNumbr = db.ordermaster.Count() +1 ;

            return View();
        }

        public JsonResult Getitemprice(int ItemID)
        {
            //db.Configuration.ProxyCreationEnabled = false;

            var itemprice = db.Items.Where(m => m.ItemID == ItemID).FirstOrDefault().ItemPrice;
           

            return Json(itemprice, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getitemunit(int ItemID)
        {
            //db.Configuration.ProxyCreationEnabled = false;

            var itemunit = db.Items.Where(m => m.ItemID == ItemID).FirstOrDefault().ItemUnitID;


            return Json(itemunit, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SaveOrder(OrderMasterViewModel OrderMasterViewModel  )
        {

            if (OrderMasterViewModel.ItemID != null)
            {
                var orderid = OrderMasterViewModel.OrderNumber;
                var itemid = OrderMasterViewModel.ItemID;
                //var orderMasterId = 0;

                for (int i = 0; i < OrderMasterViewModel.ItemID.Count; i++)
                {
                    OrderChild child = new OrderChild
                    {
                        OrderChildID = 0,
                        OrderMasterID = orderid,
                        ItemID = OrderMasterViewModel.ItemID[i],
                        UnitPrice = OrderMasterViewModel.UnitPrice[i],
                        Discount = OrderMasterViewModel.Discount[i],
                        Quantity = OrderMasterViewModel.Quantity[i],
                        Total = OrderMasterViewModel.Total[i],
                    };
                    db.OrderChild.Add(child);
                    db.SaveChanges();

                    Transactions trans = new Transactions
                    {
                        TransactionID = 0,
                        ItemID = OrderMasterViewModel.ItemID[i],
                        Quantity = (-1)*OrderMasterViewModel.Quantity[i],
                        TransDate = DateTime.Now,
                        PayTypeID = OrderMasterViewModel.PaymentID
                    };
                    db.Transactions.Add(trans);
                    db.SaveChanges();

                    //---Stockss---//
                    var oldquantity = db.Stock.Find(itemid[i]).Quantity;
                    OrderServices orderServices = new OrderServices();
                    var StockValues = orderServices.GetStockById(itemid[i]);
                    StockValues.Quantity = oldquantity - OrderMasterViewModel.Quantity[i];
                    orderServices.UpdateStock(StockValues);
                    //---Stockss---//
                }
                ordermaster ordermaster = new ordermaster
                {
                    //OrderID = 0,
                    PaymentID = OrderMasterViewModel.PaymentID,
                    CustomerID = (-1)*OrderMasterViewModel.CustomerID,
                    OrderNumber = OrderMasterViewModel.OrderNumber,
                    OrderDate = OrderMasterViewModel.OrderDate,
                    FinalTotal = OrderMasterViewModel.FinalTotal,
                    OrderType = "Sale"

                };
                db.ordermaster.Add(ordermaster);
                db.SaveChanges();
                

            }
             return RedirectToAction("Index");
        }
        public ActionResult EditSvoucher(int id)
        {
            //ordermaster om = new ordermaster();
            //ViewData["CustomerID"] = new SelectList(db.Customers, "CustomerID", "CustomerName", om.CustomerID);
            //ViewBag.GetItems = new SelectList(db.Items, "ItemID", "ItemName");
            //ViewBag.GetPayTypes = new SelectList(db.PaymentTypes, "PaymentTypeID", "PaymentTypeName");
            //ViewBag.OrderNumbr = db.ordermaster.Count() + 1;

            if (id != 0)
            {
                var masterfind = db.ordermaster.Find(id);
                var childfind = db.OrderChild.Where(o => o.OrderMasterID == id).ToList();

                ViewBag.Customers = new SelectList(db.Customers, "CustomerID", "CustomerName", masterfind.CustomerID);
                ViewBag.GetItems = new SelectList(db.Items, "ItemID", "ItemName");
                ViewBag.GetPayTypes = new SelectList(db.PaymentTypes, "PaymentTypeID", "PaymentTypeName", masterfind.PaymentTypes);
                ViewBag.OrderNumbr = masterfind.OrderID;
                ViewBag.OrderDate = masterfind.OrderDate;
                ViewBag.Finaltotal = masterfind.FinalTotal;

                //OrderChild oc = new OrderChild();
                OrderMasterViewModel masterr = new OrderMasterViewModel
                {
                    OrderID = masterfind.OrderID,
                    PaymentID = masterfind.PaymentID,
                    CustomerID = masterfind.CustomerID,
                    OrderNumber = masterfind.OrderNumber,
                    OrderDate = masterfind.OrderDate,
                    FinalTotal = masterfind.FinalTotal,
                    orderChildViewModels = childfind

                    //Discountlist = oc.Discount,
                    //ItemIDlist = oc.ItemID,
                    //UnitPricelist = oc.UnitPrice,
                    //Totallist = oc.Total,
                    //Quantitylist = oc.Quantity

                };






                return View(masterr);

            }
            else
            {


                return View();
            }





        }

        public ActionResult Orders()
        {
            //ordermaster om = new ordermaster();
            //var customername = db.Customers.Select(o => o.);
            //var orderlist = db.ordermaster.Include(o => o.Customers);
            var orderlist = db.ordermaster.ToList();

            var voucherlist = db.VoucherMaster.ToList();

            //var orderbysp = db.Sp_GetOrders(); 

            return View(voucherlist);

        }
        //public ActionResult OrderChild()
        //{

        //    var list =  db.OrderChild.Include(b => b.Items);
        //    return View(list.ToList());
        //}
        [HttpGet]
        public ActionResult GetOrderChild(int id)
        {

            //var orderChildren = db.OrderChild.Where(o => o.OrderMasterID == id).ToList();
            var orderchilddetails = db.OrderChild.Where(o => o.OrderMasterID == id).ToList();

            return View(orderchilddetails);
        }

        public ActionResult GetTrans()
        {
            //var trans = db.Transactions.Include(o => o.PaymentTypes.PaymentTypeName);
            //var trans = db.Transactions;

            return View(db.Transactions.ToList());
        }

        public ActionResult getdsalechild(int id)
        {
            var saleitemlist = db.OrderChild.Where(x => x.OrderMasterID == id).ToList();
            return View(saleitemlist);
        }
        


        //public ActionResult Addsalevoucher()
        //{
        //    ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
        //    ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
        //    ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
        //    ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName");

        //    ViewBag.ordernumber = db.VoucherMaster.Count() + 1;


        //    return View();
        //}        //public ActionResult Addsalevoucher()
        //{
        //    ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
        //    ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
        //    ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
        //    ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName");

        //    ViewBag.ordernumber = db.VoucherMaster.Count() + 1;


        //    return View();
        //}        //public ActionResult Addsalevoucher()
        //{
        //    ViewBag.Account = new SelectList(db.Account, "AccountID", "AccountName");
        //    ViewBag.Units = new SelectList(db.Units, "UnitID", "UnitName");
        //    ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
        //    ViewBag.BillSundry = new SelectList(db.BillSundry, "BiilSundryID", "BillSundryName");

        //    ViewBag.ordernumber = db.VoucherMaster.Count() + 1;


        //    return View();
        //}






    }



}