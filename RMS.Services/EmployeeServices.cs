using RMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Services
{
    public class EmployeeServices
    {

        CRUDDBEntities db = new CRUDDBEntities();

        public List<tbl_employee1> GetAllmployee()
        {
           return db.tbl_employee1.ToList();


        }


        public void AddEnp(tbl_employee1 employee)
        {
            db.tbl_employee1.Add(employee);
            db.SaveChanges();



        }
        public tbl_employee1 Getempbyid(int empid)
        {
           var emp = db.tbl_employee1.Find(empid);
            return emp;
        }
         public void updateemp(tbl_employee1 emp)
        {
            db.Entry(emp).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }



    }
}