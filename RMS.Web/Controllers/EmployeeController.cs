using RMS.Web.Models;
using RMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMS.Web.Controllers
{
    public class EmployeeController : Controller
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        EmployeeServices services = new EmployeeServices();

        //// GET: Employee
        //public ActionResult Index()
        //{
        //    var employeelist = services.GetAllmployee();
        //    ViewBag.Error = "Data not found,contact Admin";
        //    return View(employeelist);
        //}

        //public ActionResult SaveEmp(tbl_employee1 employee, int Emp_id = 0)
        //{
        //    if (Emp_id > 0)
        //    {


        //        var emp = services.Getempbyid(Emp_id);

        //        emp.Name = employee.Name;
        //        emp.Address = employee.Address;
        //        emp.City = employee.City;
        //        emp.Class_Section = employee.Class_Section;
        //        emp.Designation = employee.Designation;
        //        emp.Pin_code = employee.Pin_code;



        //        services.updateemp(emp);

        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {

        //        services.AddEnp(employee);
        //        return RedirectToAction("Index");
        //    }


        //}

        //public JsonResult GetEmp(int empid)
        //{
        //   var getempbyid = services.Getempbyid(empid);
        //    tbl_employee1 employee = new tbl_employee1();

        //    employee.Emp_id = getempbyid.Emp_id;
        //    employee.Name = getempbyid.Name;
        //    employee.Address = getempbyid.Address;
        //    employee.Pin_code = getempbyid.Pin_code;
        //    employee.Designation = getempbyid.Designation;
        //    employee.Class_Section = getempbyid.Class_Section;
        //    employee.City = getempbyid.City;

        //    return Json(employee, JsonRequestBehavior.AllowGet);


        //}
        //public ActionResult samplpe()
        //{


        //    return View();
        //}

        ////public ActionResult Practice()
        //{
           
        //    ordermaster om = new ordermaster();
        //    OrderChild oc = new OrderChild();
        //    var ordercounnt = db.ordermaster.Count();
        //    ViewBag.ordernum = ordercounnt +1 ;

        //    ViewBag.customers = new SelectList(db.Customers, "CustomerID", "CustomerName",om.CustomerID);
        //    ViewBag.getitems = new SelectList(db.Items, "ItemID", "ItemName", oc.OrderChildID);
        //    return View();
        //}
        //public JsonResult Getpricebyitem(int itemid)
        //{
        //    var price = db.Items.Where(i => i.ItemID == itemid).FirstOrDefault().ItemPrice;

        //    return Json(price, JsonRequestBehavior.AllowGet);

        //}



    }
}