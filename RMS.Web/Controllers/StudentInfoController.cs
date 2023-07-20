using RMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMS.Web.Models;

namespace RMS.Web.Controllers
{
    public class StudentInfoController : Controller
    {
        StudentInfoServices services = new StudentInfoServices();
        // GET: StudentInfo
        public ActionResult Index()
        {
            var studentlist = services.GetallStudents();
            return View(studentlist);
        }
        [HttpPost]
        public ActionResult SaveStudents(StudentInfo studentInfo , int Studentid=0)
        {
            if (Studentid > 0)
            {
                var dbstudent = services.Getstudentbyid(Studentid);
                studentInfo.StudentID = Studentid;

                dbstudent.StudentName = studentInfo.StudentName;
                dbstudent.StudentFatherName = studentInfo.StudentFatherName;
                dbstudent.StudentAge = studentInfo.StudentAge;
                dbstudent.StudentEmail = studentInfo.StudentEmail;
                dbstudent.Fathercnic = studentInfo.Fathercnic;
                dbstudent.FatherOcp = studentInfo.FatherOcp;
                dbstudent.FatherEmail = studentInfo.FatherEmail;
                dbstudent.FatherNumber = studentInfo.FatherNumber;
                dbstudent.Course = studentInfo.Course;
                dbstudent.Branch = studentInfo.Branch;
                dbstudent.Timings = studentInfo.Timings;



                services.UpdateStudent(dbstudent);
                    return RedirectToAction("Index");
            }

            else {
                services.AddStudent(studentInfo);
                return RedirectToAction("Index");

            }


        }

        public ActionResult Getstudent( StudentInfo student, int studentid = 0)
        {
            
               var stdid = services.Getstudentbyid(studentid);
                return Json(stdid, JsonRequestBehavior.AllowGet);
            
        }
        public ActionResult DeleteStudent(int Studentid=0)
        {
            if(Studentid>0)
            {
                var student = services.Getstudentbyid(Studentid);
                services.deletestudent(student);
                return RedirectToAction("Index");

            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        

    }
}