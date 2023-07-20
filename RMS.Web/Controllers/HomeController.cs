using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RMS.Web.Models;
using RMS.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RMS.Web.Models.ViewModels.DynamicMenuviewmodel;

namespace RMS.Web.Controllers
{

    public class HomeController : Controller
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        //DynamicMenuviewmodel menumodel = new DynamicMenuviewmodel();
    [Authorize]

        public ActionResult Index()
        {
            try
            {
                var useremail = Session["username"];
                if (useremail != null)
                {



                    //var transcount = db.Transactions.Count(); 
                    ViewBag.Transactions = 156;

                    //var supcount = db.Suppliers.Count(); 
                    ViewBag.suppliers = 15;

                    var vouchercount = db.VoucherMaster.Count();
                    var vouchercountpurchase = db.VoucherMaster.Where(x => x.VoucherTypeID == 2).Count();
                    if (vouchercount > 0 && vouchercountpurchase > 0)
                    {
                        try
                        {

                            var salecount = db.VoucherMaster.Where(x => x.VoucherTypeID == 1 && x.VoucherFinalTotal > 0).Sum(x => x.VoucherFinalTotal);
                            ViewBag.sales = salecount;

                            var purchasecount = db.VoucherMaster.Where(x => x.VoucherTypeID == 2 && x.VoucherFinalTotal > 0).Sum(b => b.VoucherFinalTotal);
                            ViewBag.PurchaseCount = purchasecount;
                        }
                        catch (Exception)
                        {
                            ViewBag.sales = 0;
                            ViewBag.PurchaseCount = 0;
                        }

                    }
                    else
                    {
                        ViewBag.sales = 0;
                        ViewBag.PurchaseCount = 0;

                    }
                    var vouchercount2 = db.Items.Count();
                    if (vouchercount > 0)
                    {
                        var itemcount = db.Items.Count();
                        ViewBag.items = itemcount;


                        var stockcount = db.Stock.Sum(m => m.Quantity);
                        ViewBag.stocks = stockcount;
                    }
                    else
                    {
                        ViewBag.items = 0;
                        ViewBag.stocks = 0;

                    }

                    var custcount = db.Customers.Count();
                    if (custcount > 0)
                    {
                        ViewBag.customers = custcount;

                    }
                    else
                    {
                        ViewBag.customers = 0;

                    }


                    //var saleordercount = db.ordermaster.Where(o=>o.OrderType=="sale").Count();
                    ViewBag.saleorders = 0;

                    //var purchaseordercount = db.ordermaster.Where(o => o.OrderType == "purchase").Count(); 
                    ViewBag.purchaseorders = 0;

                    var vouchercount3 = db.Account.Count();
                    if (vouchercount > 0)
                    {
                        var accounts1 = db.Account.Count();
                        ViewBag.accounts = accounts1;
                    }
                    else
                    {
                        ViewBag.accounts = 0;

                    }

                    return View();

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }


        }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("/Views/Shared/Error.cshtml");
                throw;
            }


        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetMenulist()
        {
            //var userid = Session["userid"];
            var user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
            var userid = db.AspNetUsers.Where(m => m.Id == user.Id.ToString()).FirstOrDefault().UserID;
            //var userid = 7;
            if (user != null)
            {


                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MVCDBADO"].ConnectionString);

                con.Open();
                SqlCommand cmd = new SqlCommand("Sp_GetUserrights", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable ds = new DataTable();
                sda.Fill(ds);

                DataTable ds2 = ds;
                //Menu_List menumodel = new Menu_List();
                //menumodel.ID = Convert.ToInt32(ds.Rows[0]);
                //menumodel.ParentID = Convert.ToInt32(ds.Rows[4]);
                //menumodel.MenuName = Convert.ToString(ds.Rows[1]);
                //menumodel.Url = Convert.ToString(ds.Rows[2]);
                //menumodel.Icon = Convert.ToString(ds.Rows[3]);
                con.Close();
                var userid2 = userid;
                //Menu_List menumodel = new Menu_List();
                //for (int i = 0; i < ds2.Rows.Count; i++)
                //{
                //    menumodel.ID = Convert.ToInt32(ds.Rows[i][0]);
                //    menumodel.ParentID = Convert.ToInt32(ds.Rows[i][4]);    
                //    menumodel.MenuName = Convert.ToString(ds.Rows[i][1]);
                //    menumodel.Url = Convert.ToString(ds.Rows[i][2]);
                //    menumodel.Icon = Convert.ToString(ds.Rows[i][3]);
                //}

                try
                {
                    var result = (from p in db.MenuItems
                                  join d in db.UsersMenu on new { DID = (int?)p.ID } equals new { DID = (int?)d.MenuItemID } into dis
                                  from d in dis.DefaultIfEmpty()
                                  join m in db.AspNetUsers on new { DID = (int?)d.UserID } equals new { DID = (int?)m.UserID } into diss
                                  from m in diss.DefaultIfEmpty()
                                  where d.UserID == userid2 
                                  select new RMS.Web.Models.ViewModels.DynamicMenuviewmodel.Menu_List
                                  {
                                      ID = p.ID,
                                      ParentID = p.ParentID,
                                      MenuName = p.MenuName,
                                      Url = p.Url,
                                      Icon = p.Icon,
                                  }).ToList();


                    return View("Menu", result);
                }
                catch (Exception e)
                {
                    ViewBag.message = e.ToString();
                    return View("/Views/Shared/Error.cshtml");
                    throw;
                }

            }
            else
            {
                return RedirectToAction("Login","Action");
            }
        }

        public ActionResult AddMenu( )
        {
            ViewBag.Parentlist = new SelectList(db.MenuItems,"ID", "MenuName");

            return View();
        }
        [HttpPost]
        public ActionResult AddMenu(MenuItems menuItems)
        {
            ViewBag.Parentlist = new SelectList(db.MenuItems, "ID", "MenuName");


            db.MenuItems.Add(menuItems);
            db.SaveChanges();

            return View();
        }
        public JsonResult Redirecttourl(string key)
        {
            if (key == "74" )
            {
                var url = Url.Action("JournalVoucher","Vouchers");
                return Json(url, JsonRequestBehavior.AllowGet);
            }
            else if (key == "80")
            {
                var url = Url.Action("PaymentVoucher", "Vouchers");
                return Json(url, JsonRequestBehavior.AllowGet);
            }
            else if (key == "82")
            {
                var url = Url.Action("RecieptVoucher", "Vouchers");
                return Json(url, JsonRequestBehavior.AllowGet);
            }
            else if (key == "83")
            {
                var url = Url.Action("Addsalevoucher", "Sales");
                return Json(url, JsonRequestBehavior.AllowGet);
            }
            else if (key == "76")
            {
                var url = Url.Action("ShowAccLedger", "Masters");
                return Json(url, JsonRequestBehavior.AllowGet);
            }
            else if (key == "84")
            {
                var url = Url.Action("TrialBalance", "Masters");
                return Json(url, JsonRequestBehavior.AllowGet);
            }
            else if (key == "72")
            {
                var url = Url.Action("Index", "Home");
                return Json(url, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var url = "";
                return Json(url, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult Getsales()
        {
            List<double> sales = new List<double>();

            //var datejan1 = ;
            //var datejan2 = ;
            //var datefeb1 = ;
            //var datefeb2 = ;
            //var datemar1 = ;
            //var datemar2 = ;
            //var dateapr1 = ;
            //var dateapr2 = ;
            //var datemay1 = ;
            //var datemay2 = ;
            //var datejun1 = ;
            //var datejun2 = ;
            var datejuly1 = Convert.ToDateTime("01/07/2019");
            var datejuly2 = Convert.ToDateTime("31/07/2019");
            var dateaug1 = Convert.ToDateTime("01/08/2019");
            var dateaug2 = Convert.ToDateTime("31/08/2019");
            var datesep1 = Convert.ToDateTime("01/09/2019");
            var datesep2 = Convert.ToDateTime("30/09/2019");
            var dateoct1 = Convert.ToDateTime("01/10/2019");
            var dateoct2 = Convert.ToDateTime("31/10/2019");
            var datenov1 = Convert.ToDateTime("01/11/2019");
            var datenov2 = Convert.ToDateTime("30/11/2019");
            var datedec1 = Convert.ToDateTime("01/12/2019");
            var datedec2 = Convert.ToDateTime("31/12/2019");

            var julysales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= datejuly1 && m.VoucherCreateDate >= datejuly2).Sum(m => m.VoucherFinalTotal);
            var augsales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= dateaug1 && m.VoucherCreateDate >= dateaug2).Sum(m => m.VoucherFinalTotal);
            var sepsales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= datesep1 && m.VoucherCreateDate >= datesep2).Sum(m => m.VoucherFinalTotal);
            var octsales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= dateoct1 && m.VoucherCreateDate >= dateoct2).Sum(m => m.VoucherFinalTotal);
            var novsales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= datenov1 && m.VoucherCreateDate >= datenov2).Sum(m => m.VoucherFinalTotal);
            var decsales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= datedec1 && m.VoucherCreateDate >= datedec2).Sum(m => m.VoucherFinalTotal);
            //var jansales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= Convert.ToDateTime("01/01/2020") && m.VoucherCreateDate >= Convert.ToDateTime("01/31/2020")).Sum(m => m.VoucherFinalTotal);
            //var febsales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= Convert.ToDateTime("02/01/2020") && m.VoucherCreateDate >= Convert.ToDateTime("02/28/2020")).Sum(m => m.VoucherFinalTotal);
            //var marsales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= Convert.ToDateTime("03/01/2020") && m.VoucherCreateDate >= Convert.ToDateTime("03/31/2020")).Sum(m => m.VoucherFinalTotal);
            //var aprilsales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= Convert.ToDateTime("04/01/2020") && m.VoucherCreateDate >= Convert.ToDateTime("04/30/2020")).Sum(m => m.VoucherFinalTotal);
            //var maysales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= Convert.ToDateTime("05/01/2020") && m.VoucherCreateDate >= Convert.ToDateTime("05/31/2020")).Sum(m => m.VoucherFinalTotal);
            //var junsales = db.VoucherMaster.Where(m => m.VoucherCreateDate >= Convert.ToDateTime("06/01/2020") && m.VoucherCreateDate >= Convert.ToDateTime("06/30/2020")).Sum(m => m.VoucherFinalTotal);

            //sales.Add(jansales);
            //sales.Add(febsales);
            //sales.Add(marsales);
            //sales.Add(aprilsales);
            //sales.Add(maysales);
            sales.Add(julysales);
            sales.Add(julysales);
            sales.Add(augsales);
            sales.Add(sepsales);
            sales.Add(octsales);
            sales.Add(novsales);
            sales.Add(decsales);

            return Json(sales,JsonRequestBehavior.AllowGet);
        }

    }
}