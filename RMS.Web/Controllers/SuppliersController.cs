using RMS.Web.Models;
using RMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMS.Web.Controllers
{
    public class SuppliersController : Controller
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        SupplierServices Service = new SupplierServices();
        // GET: Suppliers
        public ActionResult Index()
        {
            var list = Service.Getallsupp();
            return View(list);
        }
        [HttpPost]
        public ActionResult SaveSupp(Suppliers Supp)
        {
            if(Supp.SupplierID>0)
            {
                var olddata = Service.Getsuppbyid(Supp.SupplierID);
                olddata.SupplierName = Supp.SupplierName;
                olddata.SuplierNumber = Supp.SuplierNumber;
                olddata.SuplierCNIC = Supp.SuplierCNIC;
                olddata.SupplierAddress = Supp.SupplierAddress;
                Service.Updatesupp(olddata);
                return RedirectToAction("Index");


            }
            else
            {
                if(Supp.SupplierName!=null)
                {
                    Service.Addsupp(Supp);
                    return RedirectToAction("Index");

                }
                return RedirectToAction("Index");

            }

        }
        public ActionResult Getsupp(int suppid)
        {
            var getsupp = Service.Getsuppbyid(suppid);

            Suppliers supp = new Suppliers();
            supp.SupplierID = getsupp.SupplierID;
            supp.SupplierName = getsupp.SupplierName;
            supp.SuplierNumber = getsupp.SuplierNumber;
            supp.SuplierCNIC = getsupp.SuplierCNIC;
            supp.SupplierAddress = getsupp.SupplierAddress;


            return Json(supp, JsonRequestBehavior.AllowGet);

        }
        public ActionResult DeleteSupp(int SupplierID)
        {
            var oldsupp = Service.Getsuppbyid(SupplierID);
            Service.Deletesupplier(oldsupp);

            return RedirectToAction("Index");
        }

    }
}