using RMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMS.Web.Services
{
    public class OrderServices
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        public Stock GetStockById(int ItemId)
        {
            //return db.Customers.Where(x => x.CustomerName == "SAAD").FirstOrDefault();
            return db.Stock.Find(ItemId);
        }

        public void UpdateStock(Stock stock)
        {
            db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }



    }
}