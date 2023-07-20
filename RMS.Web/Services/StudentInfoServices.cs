using RMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RMS.Web.Services
{
    public class StudentInfoServices
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        


        public List<StudentInfo> GetallStudents()
        {
            return  db.StudentInfo.ToList();
                                            
        }
        public StudentInfo Getstudentbyid(int studentid)
        {
            return db.StudentInfo.Find(studentid);
        }

        public void AddStudent(StudentInfo studentInfo)
        {
            db.StudentInfo.Add(studentInfo);
            db.SaveChanges();


        }
        public void UpdateStudent(StudentInfo studentInfo)
        {
            db.Entry(studentInfo).State = EntityState.Modified;

            db.SaveChanges();

        }
        public void deletestudent(StudentInfo studentInfo)
        {
            db.StudentInfo.Remove(studentInfo);
            db.SaveChanges();
        }


    }
}