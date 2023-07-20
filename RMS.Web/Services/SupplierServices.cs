using RMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Services
{
    public class SupplierServices
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();

        public List<Suppliers> Getallsupp()
        {
            return db.Suppliers.ToList();
        }
        public Suppliers Getsuppbyid(int id)
        {
           return db.Suppliers.Find(id);

        }
        public void Updatesupp(Suppliers supp)
        {
            db.Entry(supp).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }



        public void Addsupp(Suppliers supp)
        {
            db.Suppliers.Add(supp);
            db.SaveChanges();

        }

        public void Deletesupplier(Suppliers supp)
        {
            db.Suppliers.Remove(supp);
            db.SaveChanges();
        }


    }
}