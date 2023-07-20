using RMS.Web.Models;
using RMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMS.Web.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer

        CustomerServices services = new CustomerServices();
        public ActionResult Index()
        {
            var customers = services.GetAllCustomers();
            return View(customers);
        }
        [HttpPost]
        public ActionResult SaveCustomer(string CustomerName = "", int CustomerId=0)
        {
            if (CustomerId > 0)
            {
                var customer = services.GetCustomerById(CustomerId);
                if (customer != null && !string.IsNullOrEmpty(CustomerName))
                {
                    customer.CustomerName = CustomerName;
                    services.UpdateCustomer(customer);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(CustomerName))
                {
                    Customers customer = new Customers
                    {
                        CustomerName = CustomerName,
                    };

                    services.AddCustomer(customer);
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult GetCustomer(int CustomerId = 0)
        {
            if (CustomerId > 0)
            {
                var customerbyid = services.GetCustomerById(CustomerId);

                Customers customer = new Customers();
                customer.CustomerID = customerbyid.CustomerID;
                customer.CustomerName = customerbyid.CustomerName;



                return Json(customer, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("");
            }
        }

        public ActionResult DeleteCustomer(int CustomerId = 0)
        {
            if (CustomerId > 0)
            {
                var customer = services.GetCustomerById(CustomerId);
                services.DeleteCustomer(customer);
            }
            return RedirectToAction("Index");
        }
    }
}