using CrystalDecisions.CrystalReports.Engine;
using RMS.Web.Models;
using RMS.Web.Models.ViewModels;
using RMS.Web.Services;
using RMS.Web.Views.Masters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMS.Web.Controllers
{
    [Authorize]
    public class MastersController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MVCDBADO"].ConnectionString);
        MVCDBLiveEntities db = new MVCDBLiveEntities();
        MasterServices services = new MasterServices();
        ADOServices treeservice = new ADOServices();
        trialbalanceviewmodel tbmodel = new trialbalanceviewmodel();
        Ptofitlossviewmodel proftlossmodel = new Ptofitlossviewmodel();

        // GET: Masters
        //------------------Account Groups------------------//
        #region AccountGroup

        public ActionResult AccountGroup()
        {

            var accountgrouplist = db.AccGroup.ToList();

            AccGroup accg = new AccGroup();
            AccountMainHead accm = new AccountMainHead();

            ViewBag.Accundergroup = new SelectList(db.AccGroup, "AccgroupID", "Accgroupname", 0);
            ViewBag.accundergroup = new SelectList(db.AccGroup, "AccgroupID", "Accgroupname", accg.AccunderprimarygroupName);
            ViewBag.Accmain = new SelectList(db.AccountMainHead, "MainGroupID", "MainAccGroupName", accm.MainAccGroupName);


            return View(accountgrouplist);
        }
        [HttpPost]
        public ActionResult SaveAccgroup(int AccgroupID, string Accgroupname, string Accgroupalias, string Accundergroup, string Accunderprimarygroup, int? Accprimarygroupid, int? MainGroupId)
        {

            if (AccgroupID == 0)
            {
                AccGroup acg = new AccGroup();
                acg.Accgroupname = Accgroupname;
                acg.Accgroupalias = Accgroupalias;
                acg.AccunderprimarygroupName = Accunderprimarygroup;
                acg.Accgroupbalance = 0.00;
                int? val = 0;
                if (Accprimarygroupid == null)
                {
                    val = 0;
                }
                else
                {
                    val = Accprimarygroupid;
                }
                acg.Accunderprimarygroupid = val;
                acg.DebitAmount = 0.00;
                acg.CreditAmount = 0.00;
                acg.MainGroupID = MainGroupId;
                var addaccg = acg;

                services.AddAccgrup(addaccg);


                return RedirectToAction("AccountGroup");
            }
            else
            {


                if (AccgroupID > 0)
                {
                    try
                    {

                        var oldaccg = services.Getaccgroupbyid(AccgroupID);


                        //oldaccg.Accgroupbalance = 
                        if (oldaccg.Accunderprimarygroupid != null)
                        {
                            var oldnewaccgrup = services.Getaccgroupbyid(oldaccg.Accunderprimarygroupid);

                        }
                        else
                        {
                            var oldnewaccgrup = services.Getaccgroupbyid(oldaccg.AccgroupID);
                            var oldnewbalance = oldnewaccgrup.Accgroupbalance;
                            var oldnewdebitbalance = oldnewaccgrup.DebitAmount;
                            var oldnewcreditbalance = oldnewaccgrup.CreditAmount;
                            //---------------------Update Account  group 2 credit----------------------------//
                            if (oldnewaccgrup.Accunderprimarygroupid != 0 && oldnewaccgrup.Accunderprimarygroupid != null)
                            {
                                oldnewaccgrup.Accgroupbalance = oldnewbalance - oldaccg.Accgroupbalance;
                                oldnewaccgrup.DebitAmount = oldnewdebitbalance - oldaccg.DebitAmount;
                                oldnewaccgrup.CreditAmount = oldnewcreditbalance - oldaccg.CreditAmount;
                                services.UpdateAccgroup(oldnewaccgrup);

                                var oldnewaccgrup2 = services.Getaccgroupbyid(oldnewaccgrup.Accunderprimarygroupid);
                                var oldnewbalance2 = oldnewaccgrup2.Accgroupbalance;
                                var oldnewdebitbalance2 = oldnewaccgrup2.DebitAmount;
                                var oldnewcreditbalance2 = oldnewaccgrup2.CreditAmount;
                                if (oldnewaccgrup2.Accunderprimarygroupid != 0 && oldnewaccgrup2.Accunderprimarygroupid != null)
                                {
                                    oldnewaccgrup2.Accgroupbalance = oldnewbalance2 - oldaccg.Accgroupbalance;
                                    oldnewaccgrup2.DebitAmount = oldnewdebitbalance2 - oldaccg.DebitAmount;
                                    oldnewaccgrup2.CreditAmount = oldnewcreditbalance2 - oldaccg.CreditAmount;
                                    services.UpdateAccgroup(oldnewaccgrup2);

                                    //---------------------Update Account group 3 credit----------------------------//
                                    var oldnewaccgrup3 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                    var oldnewbalance3 = oldnewaccgrup.Accgroupbalance;
                                    var oldnewdebitbalance3 = oldnewaccgrup.DebitAmount;
                                    var oldnewcreditbalance3 = oldnewaccgrup.CreditAmount;
                                    if (oldnewaccgrup3.Accunderprimarygroupid != 0 && oldnewaccgrup3.Accunderprimarygroupid != null)
                                    {
                                        oldnewaccgrup3.Accgroupbalance = oldnewbalance3 - oldaccg.Accgroupbalance;
                                        oldnewaccgrup3.DebitAmount = oldnewdebitbalance3 - oldaccg.DebitAmount;
                                        oldnewaccgrup3.CreditAmount = oldnewcreditbalance3 - oldaccg.CreditAmount;
                                        services.UpdateAccgroup(oldnewaccgrup3);


                                        //---------------------Update Account primary group 4 credit----------------------------//
                                        var oldnewaccgrup4 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                        var oldnewbalance4 = oldnewaccgrup.Accgroupbalance;
                                        var oldnewdebitbalance4 = oldnewaccgrup.DebitAmount;
                                        var oldnewcreditbalance4 = oldnewaccgrup.CreditAmount;
                                        if (oldnewaccgrup4.Accunderprimarygroupid != 0 && oldnewaccgrup4.Accunderprimarygroupid != null)
                                        {
                                            oldnewaccgrup4.Accgroupbalance = oldnewbalance4 - oldaccg.Accgroupbalance;
                                            oldnewaccgrup4.DebitAmount = oldnewdebitbalance4 - oldaccg.DebitAmount;
                                            oldnewaccgrup4.CreditAmount = oldnewcreditbalance4 - oldaccg.CreditAmount;
                                            services.UpdateAccgroup(oldnewaccgrup4);


                                            //---------------------Update Account primary group 5 credit----------------------------//
                                            var oldnewaccgrup5 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                            var oldnewbalance5 = oldnewaccgrup.Accgroupbalance;
                                            var oldnewdebitbalance5 = oldnewaccgrup.DebitAmount;
                                            var oldnewcreditbalance5 = oldnewaccgrup.CreditAmount;

                                            if (oldnewaccgrup5.Accunderprimarygroupid != 0 && oldnewaccgrup5.Accunderprimarygroupid != null)
                                            {
                                                oldnewaccgrup5.Accgroupbalance = oldnewbalance5 - oldaccg.Accgroupbalance;
                                                oldnewaccgrup5.DebitAmount = oldnewdebitbalance5 - oldaccg.DebitAmount;
                                                oldnewaccgrup5.CreditAmount = oldnewcreditbalance5 - oldaccg.CreditAmount;
                                                services.UpdateAccgroup(oldnewaccgrup5);


                                                //---------------------Update Account primary group 6 credit----------------------------//
                                                var oldnewaccgrup6 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                                var oldnewbalance6 = oldnewaccgrup.Accgroupbalance;
                                                var oldnewdebitbalance6 = oldnewaccgrup.DebitAmount;
                                                var oldnewcreditbalance6 = oldnewaccgrup.CreditAmount;

                                                if (oldnewaccgrup6.Accunderprimarygroupid != 0 && oldnewaccgrup6.Accunderprimarygroupid != null)
                                                {
                                                    oldnewaccgrup6.Accgroupbalance = oldnewbalance6 - oldaccg.Accgroupbalance;
                                                    oldnewaccgrup6.DebitAmount = oldnewdebitbalance6 - oldaccg.DebitAmount;
                                                    oldnewaccgrup6.CreditAmount = oldnewcreditbalance6 - oldaccg.CreditAmount;
                                                    services.UpdateAccgroup(oldnewaccgrup6);


                                                    //---------------------Update Account primary group 7 credit----------------------------//
                                                    var oldnewaccgrup7 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                                    var oldnewbalance7 = oldnewaccgrup.Accgroupbalance;
                                                    var oldnewdebitbalance7 = oldnewaccgrup.DebitAmount;
                                                    var oldnewcreditbalance7 = oldnewaccgrup.CreditAmount;


                                                    if (oldnewaccgrup7.Accunderprimarygroupid != 0 && oldnewaccgrup7.Accunderprimarygroupid != null)
                                                    {
                                                        oldnewaccgrup7.Accgroupbalance = oldnewbalance7 - oldaccg.Accgroupbalance;
                                                        oldnewaccgrup7.DebitAmount = oldnewdebitbalance7 - oldaccg.DebitAmount;
                                                        oldnewaccgrup7.CreditAmount = oldnewcreditbalance7 - oldaccg.CreditAmount;
                                                        services.UpdateAccgroup(oldnewaccgrup7);



                                                        //---------------------Update Account primary group 8 credit----------------------------//
                                                        var oldnewaccgrup8 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                                        var oldnewbalance8 = oldnewaccgrup.Accgroupbalance;
                                                        var oldnewdebitbalance8 = oldnewaccgrup.DebitAmount;
                                                        var oldnewcreditbalance8 = oldnewaccgrup.CreditAmount;

                                                        if (oldnewaccgrup8.Accunderprimarygroupid != 0 && oldnewaccgrup8.Accunderprimarygroupid != null)
                                                        {
                                                            oldnewaccgrup8.Accgroupbalance = oldnewbalance8 - oldaccg.Accgroupbalance;
                                                            oldnewaccgrup8.DebitAmount = oldnewdebitbalance8 - oldaccg.DebitAmount;
                                                            oldnewaccgrup8.CreditAmount = oldnewcreditbalance8 - oldaccg.CreditAmount;
                                                            services.UpdateAccgroup(oldnewaccgrup8);

                                                            //---------------------Update Account primary group 9 credit----------------------------//
                                                            var oldnewaccgrup9 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                                            var oldnewbalance9 = oldnewaccgrup.Accgroupbalance;
                                                            var oldnewdebitbalance9 = oldnewaccgrup.DebitAmount;
                                                            var oldnewcreditbalance9 = oldnewaccgrup.CreditAmount;

                                                            if (oldnewaccgrup9.Accunderprimarygroupid != 0 && oldnewaccgrup9.Accunderprimarygroupid != null)
                                                            {
                                                                oldnewaccgrup9.Accgroupbalance = oldnewbalance9 - oldaccg.Accgroupbalance;
                                                                oldnewaccgrup9.DebitAmount = oldnewdebitbalance9 - oldaccg.DebitAmount;
                                                                oldnewaccgrup9.CreditAmount = oldnewcreditbalance9 - oldaccg.CreditAmount;
                                                                services.UpdateAccgroup(oldnewaccgrup9);


                                                                //---------------------Update Account primary group 10 credit----------------------------//
                                                                var oldnewaccgrup10 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                                                var oldnewbalance10 = oldnewaccgrup.Accgroupbalance;
                                                                var oldnewdebitbalance10 = oldnewaccgrup.DebitAmount;
                                                                var oldnewcreditbalance10 = oldnewaccgrup.CreditAmount;

                                                                if (oldnewaccgrup10.Accunderprimarygroupid != 0 && oldnewaccgrup10.Accunderprimarygroupid != null)
                                                                {
                                                                    oldnewaccgrup10.Accgroupbalance = oldnewbalance10 - oldaccg.Accgroupbalance;
                                                                    oldnewaccgrup10.DebitAmount = oldnewdebitbalance10 - oldaccg.DebitAmount;
                                                                    oldnewaccgrup10.CreditAmount = oldnewcreditbalance10 - oldaccg.CreditAmount;
                                                                    services.UpdateAccgroup(oldnewaccgrup10);

                                                                    //---------------------Update Account primary group 9 credit----------------------------//
                                                                    var oldnewaccgrup11 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                                                    var oldnewbalance11 = oldnewaccgrup.Accgroupbalance;
                                                                    var oldnewdebitbalance11 = oldnewaccgrup.DebitAmount;
                                                                    var oldnewcreditbalance11 = oldnewaccgrup.CreditAmount;

                                                                    if (oldnewaccgrup11.Accunderprimarygroupid != 0 && oldnewaccgrup11.Accunderprimarygroupid != null)
                                                                    {
                                                                        oldnewaccgrup11.Accgroupbalance = oldnewbalance11 - oldaccg.Accgroupbalance;
                                                                        oldnewaccgrup11.DebitAmount = oldnewdebitbalance11 - oldaccg.DebitAmount;
                                                                        oldnewaccgrup11.CreditAmount = oldnewcreditbalance11 - oldaccg.CreditAmount;
                                                                        services.UpdateAccgroup(oldnewaccgrup11);


                                                                    }
                                                                    else
                                                                    {
                                                                        oldnewaccgrup11.Accgroupbalance = oldnewbalance11 - oldaccg.Accgroupbalance;
                                                                        oldnewaccgrup11.DebitAmount = oldnewdebitbalance11 - oldaccg.DebitAmount;
                                                                        oldnewaccgrup11.CreditAmount = oldnewcreditbalance11 - oldaccg.CreditAmount;
                                                                        services.UpdateAccgroup(oldnewaccgrup11);
                                                                    }





                                                                }
                                                                else
                                                                {
                                                                    oldnewaccgrup10.Accgroupbalance = oldnewbalance10 - oldaccg.Accgroupbalance;
                                                                    oldnewaccgrup10.DebitAmount = oldnewdebitbalance10 - oldaccg.DebitAmount;
                                                                    oldnewaccgrup10.CreditAmount = oldnewcreditbalance10 - oldaccg.CreditAmount;
                                                                    services.UpdateAccgroup(oldnewaccgrup10);
                                                                }




                                                            }
                                                            else
                                                            {
                                                                oldnewaccgrup9.Accgroupbalance = oldnewbalance9 - oldaccg.Accgroupbalance;
                                                                oldnewaccgrup9.DebitAmount = oldnewdebitbalance9 - oldaccg.DebitAmount;
                                                                oldnewaccgrup9.CreditAmount = oldnewcreditbalance9 - oldaccg.CreditAmount;
                                                                services.UpdateAccgroup(oldnewaccgrup9);
                                                            }





                                                        }
                                                        else
                                                        {
                                                            oldnewaccgrup8.Accgroupbalance = oldnewbalance8 - oldaccg.Accgroupbalance;
                                                            oldnewaccgrup8.DebitAmount = oldnewdebitbalance8 - oldaccg.DebitAmount;
                                                            oldnewaccgrup8.CreditAmount = oldnewcreditbalance8 - oldaccg.CreditAmount;
                                                            services.UpdateAccgroup(oldnewaccgrup8);
                                                        }



                                                    }
                                                    else
                                                    {
                                                        oldnewaccgrup7.Accgroupbalance = oldnewbalance7 - oldaccg.Accgroupbalance;
                                                        oldnewaccgrup7.DebitAmount = oldnewdebitbalance7 - oldaccg.DebitAmount;
                                                        oldnewaccgrup7.CreditAmount = oldnewcreditbalance7 - oldaccg.CreditAmount;
                                                        services.UpdateAccgroup(oldnewaccgrup7);
                                                    }




                                                }
                                                else
                                                {
                                                    oldnewaccgrup6.Accgroupbalance = oldnewbalance6 - oldaccg.Accgroupbalance;
                                                    oldnewaccgrup6.DebitAmount = oldnewdebitbalance6 - oldaccg.DebitAmount;
                                                    oldnewaccgrup6.CreditAmount = oldnewcreditbalance6 - oldaccg.CreditAmount;
                                                    services.UpdateAccgroup(oldnewaccgrup6);
                                                }



                                            }
                                            else
                                            {
                                                oldnewaccgrup5.Accgroupbalance = oldnewbalance5 - oldaccg.Accgroupbalance;
                                                oldnewaccgrup5.DebitAmount = oldnewdebitbalance5 - oldaccg.DebitAmount;
                                                oldnewaccgrup5.CreditAmount = oldnewcreditbalance5 - oldaccg.CreditAmount;
                                                services.UpdateAccgroup(oldnewaccgrup5);
                                            }


                                        }
                                        else
                                        {
                                            oldnewaccgrup4.Accgroupbalance = oldnewbalance4 - oldaccg.Accgroupbalance;
                                            oldnewaccgrup4.DebitAmount = oldnewdebitbalance4 - oldaccg.DebitAmount;
                                            oldnewaccgrup4.CreditAmount = oldnewcreditbalance4 - oldaccg.CreditAmount;
                                            services.UpdateAccgroup(oldnewaccgrup4);
                                        }


                                    }
                                    else
                                    {
                                        oldnewaccgrup3.Accgroupbalance = oldnewbalance3 - oldaccg.Accgroupbalance;
                                        oldnewaccgrup3.DebitAmount = oldnewdebitbalance3 - oldaccg.DebitAmount;
                                        oldnewaccgrup3.CreditAmount = oldnewcreditbalance3 - oldaccg.CreditAmount;
                                        services.UpdateAccgroup(oldnewaccgrup3);

                                    }

                                }
                                else
                                {
                                    oldnewaccgrup2.Accgroupbalance = oldnewbalance2 - oldaccg.Accgroupbalance;
                                    oldnewaccgrup2.DebitAmount = oldnewdebitbalance2 - oldaccg.DebitAmount;
                                    oldnewaccgrup2.CreditAmount = oldnewcreditbalance2 - oldaccg.CreditAmount;
                                    services.UpdateAccgroup(oldnewaccgrup2);


                                }

                            }
                            else
                            {
                                oldnewaccgrup.Accgroupbalance = oldnewbalance - oldaccg.Accgroupbalance;
                                oldnewaccgrup.DebitAmount = oldnewdebitbalance - oldaccg.DebitAmount;
                                oldnewaccgrup.CreditAmount = oldnewcreditbalance - oldaccg.CreditAmount;
                                services.UpdateAccgroup(oldnewaccgrup);


                            }

                        }



                        ////////////////////////////////////////////////////////////////////////
                        if (Accprimarygroupid != null)
                        {
                            var findnewaccgrup = services.Getaccgroupbyid(Accprimarygroupid);
                            var findnewbalance = findnewaccgrup.Accgroupbalance;
                            var findnewdebitbalance = findnewaccgrup.DebitAmount;
                            var findnewcreditbalance = findnewaccgrup.CreditAmount;


                            if (findnewaccgrup.Accunderprimarygroupid != 0 && findnewaccgrup.Accunderprimarygroupid != null)
                            {

                                findnewaccgrup.Accgroupbalance = findnewbalance + oldaccg.Accgroupbalance;
                                findnewaccgrup.DebitAmount = findnewdebitbalance + oldaccg.DebitAmount;
                                findnewaccgrup.CreditAmount = findnewcreditbalance + oldaccg.CreditAmount;
                                services.UpdateAccgroup(findnewaccgrup);

                                //---------------------Update Account group 2 credit----------------------------//
                                var findnewaccgrup2 = services.Getaccgroupbyid(findnewaccgrup.Accunderprimarygroupid);
                                var findnewbalance2 = findnewaccgrup2.Accgroupbalance;
                                var findnewdebitbalance2 = findnewaccgrup2.DebitAmount;
                                var findnewcreditbalance2 = findnewaccgrup2.CreditAmount;

                                if (findnewaccgrup2.Accunderprimarygroupid != 0 && findnewaccgrup2.Accunderprimarygroupid != null)
                                {

                                    findnewaccgrup2.Accgroupbalance = findnewbalance2 + oldaccg.Accgroupbalance;
                                    findnewaccgrup2.DebitAmount = findnewdebitbalance2 + oldaccg.DebitAmount;
                                    findnewaccgrup2.CreditAmount = findnewcreditbalance2 + oldaccg.CreditAmount;
                                    services.UpdateAccgroup(findnewaccgrup2);

                                    //---------------------Update Account group 3 credit----------------------------//
                                    var findnewaccgrup3 = services.Getaccgroupbyid(findnewaccgrup2.Accunderprimarygroupid);
                                    var findnewbalance3 = findnewaccgrup3.Accgroupbalance;
                                    var findnewdebitbalance3 = findnewaccgrup3.DebitAmount;
                                    var findnewcreditbalance3 = findnewaccgrup3.CreditAmount;
                                    if (findnewaccgrup3.Accunderprimarygroupid != 0 && findnewaccgrup3.Accunderprimarygroupid != null)
                                    {
                                        findnewaccgrup3.Accgroupbalance = findnewbalance3 + oldaccg.Accgroupbalance;
                                        findnewaccgrup3.DebitAmount = findnewdebitbalance3 + oldaccg.DebitAmount;
                                        findnewaccgrup3.CreditAmount = findnewcreditbalance3 + oldaccg.CreditAmount;
                                        services.UpdateAccgroup(findnewaccgrup3);


                                        //---------------------Update Account primary group 4 credit----------------------------//
                                        var findnewaccgrup4 = services.Getaccgroupbyid(findnewaccgrup3.Accunderprimarygroupid);
                                        var findnewbalance4 = findnewaccgrup4.Accgroupbalance;
                                        var findnewdebitbalance4 = findnewaccgrup4.DebitAmount;
                                        var findnewcreditbalance4 = findnewaccgrup4.CreditAmount;

                                        if (findnewaccgrup4.Accunderprimarygroupid != 0 && findnewaccgrup4.Accunderprimarygroupid != null)
                                        {
                                            findnewaccgrup4.Accgroupbalance = findnewbalance4 + oldaccg.Accgroupbalance;
                                            findnewaccgrup4.DebitAmount = findnewdebitbalance4 + oldaccg.DebitAmount;
                                            findnewaccgrup4.CreditAmount = findnewcreditbalance4 + oldaccg.CreditAmount;
                                            services.UpdateAccgroup(findnewaccgrup4);


                                            //---------------------Update Account primary group 5 credit----------------------------//
                                            var findnewaccgrup5 = services.Getaccgroupbyid(findnewaccgrup4.Accunderprimarygroupid);
                                            var findnewbalance5 = findnewaccgrup5.Accgroupbalance;
                                            var findnewdebitbalance5 = findnewaccgrup5.DebitAmount;
                                            var findnewcreditbalance5 = findnewaccgrup5.CreditAmount;

                                            if (findnewaccgrup5.Accunderprimarygroupid != 0 && findnewaccgrup5.Accunderprimarygroupid != null)
                                            {
                                                findnewaccgrup5.Accgroupbalance = findnewbalance5 + oldaccg.Accgroupbalance;
                                                findnewaccgrup5.DebitAmount = findnewdebitbalance5 + oldaccg.DebitAmount;
                                                findnewaccgrup5.CreditAmount = findnewcreditbalance5 + oldaccg.CreditAmount;
                                                services.UpdateAccgroup(findnewaccgrup5);


                                                //---------------------Update Account primary group 6 credit----------------------------//
                                                var findnewaccgrup6 = services.Getaccgroupbyid(findnewaccgrup5.Accunderprimarygroupid);
                                                var findnewbalance6 = findnewaccgrup6.Accgroupbalance;
                                                var findnewdebitbalance6 = findnewaccgrup6.DebitAmount;
                                                var findnewcreditbalance6 = findnewaccgrup6.CreditAmount;

                                                if (findnewaccgrup6.Accunderprimarygroupid != 0 && findnewaccgrup6.Accunderprimarygroupid != null)
                                                {
                                                    findnewaccgrup6.Accgroupbalance = findnewbalance6 + oldaccg.Accgroupbalance;
                                                    findnewaccgrup6.DebitAmount = findnewdebitbalance6 + oldaccg.DebitAmount;
                                                    findnewaccgrup6.CreditAmount = findnewcreditbalance6 + oldaccg.CreditAmount;
                                                    services.UpdateAccgroup(findnewaccgrup6);


                                                    //---------------------Update Account primary group 7 credit----------------------------//
                                                    var findnewaccgrup7 = services.Getaccgroupbyid(findnewaccgrup6.Accunderprimarygroupid);
                                                    var findnewbalance7 = findnewaccgrup7.Accgroupbalance;
                                                    var findnewdebitbalance7 = findnewaccgrup7.DebitAmount;
                                                    var findnewcreditbalance7 = findnewaccgrup7.CreditAmount;


                                                    if (findnewaccgrup7.Accunderprimarygroupid != 0 && findnewaccgrup7.Accunderprimarygroupid != null)
                                                    {
                                                        findnewaccgrup7.Accgroupbalance = findnewbalance7 + oldaccg.Accgroupbalance;
                                                        findnewaccgrup7.DebitAmount = findnewdebitbalance7 + oldaccg.DebitAmount;
                                                        findnewaccgrup7.CreditAmount = findnewcreditbalance7 + oldaccg.CreditAmount;
                                                        services.UpdateAccgroup(findnewaccgrup7);



                                                        //---------------------Update Account primary group 8 credit----------------------------//
                                                        var findnewaccgrup8 = services.Getaccgroupbyid(findnewaccgrup7.Accunderprimarygroupid);
                                                        var findnewbalance8 = findnewaccgrup8.Accgroupbalance;
                                                        var findnewdebitbalance8 = findnewaccgrup8.DebitAmount;
                                                        var findnewcreditbalance8 = findnewaccgrup8.CreditAmount;

                                                        if (findnewaccgrup8.Accunderprimarygroupid != 0 && findnewaccgrup8.Accunderprimarygroupid != null)
                                                        {
                                                            findnewaccgrup8.Accgroupbalance = findnewbalance8 + oldaccg.Accgroupbalance;
                                                            findnewaccgrup8.DebitAmount = findnewdebitbalance8 + oldaccg.DebitAmount;
                                                            findnewaccgrup8.CreditAmount = findnewcreditbalance8 + oldaccg.CreditAmount;
                                                            services.UpdateAccgroup(findnewaccgrup8);

                                                            //---------------------Update Account primary group 9 credit----------------------------//
                                                            var findnewaccgrup9 = services.Getaccgroupbyid(findnewaccgrup8.Accunderprimarygroupid);
                                                            var findnewbalance9 = findnewaccgrup9.Accgroupbalance;
                                                            var findnewdebitbalance9 = findnewaccgrup9.DebitAmount;
                                                            var findnewcreditbalance9 = findnewaccgrup9.CreditAmount;

                                                            if (findnewaccgrup9.Accunderprimarygroupid != 0 && findnewaccgrup9.Accunderprimarygroupid != null)
                                                            {
                                                                findnewaccgrup9.Accgroupbalance = findnewbalance9 + oldaccg.Accgroupbalance;
                                                                findnewaccgrup9.DebitAmount = findnewdebitbalance9 + oldaccg.DebitAmount;
                                                                findnewaccgrup9.CreditAmount = findnewcreditbalance9 + oldaccg.CreditAmount;
                                                                services.UpdateAccgroup(findnewaccgrup9);


                                                                //---------------------Update Account primary group 10 credit----------------------------//
                                                                var findnewaccgrup10 = services.Getaccgroupbyid(findnewaccgrup9.Accunderprimarygroupid);
                                                                var findnewbalance10 = findnewaccgrup10.Accgroupbalance;
                                                                var findnewdebitbalance10 = findnewaccgrup10.DebitAmount;
                                                                var findnewcreditbalance10 = findnewaccgrup10.CreditAmount;

                                                                if (findnewaccgrup10.Accunderprimarygroupid != 0 && findnewaccgrup10.Accunderprimarygroupid != null)
                                                                {
                                                                    findnewaccgrup10.Accgroupbalance = findnewbalance10 + oldaccg.Accgroupbalance;
                                                                    findnewaccgrup10.DebitAmount = findnewdebitbalance10 + oldaccg.DebitAmount;
                                                                    findnewaccgrup10.CreditAmount = findnewcreditbalance10 + oldaccg.CreditAmount;
                                                                    services.UpdateAccgroup(findnewaccgrup10);

                                                                    //---------------------Update Account primary group 9 credit----------------------------//
                                                                    var findnewaccgrup11 = services.Getaccgroupbyid(findnewaccgrup10.Accunderprimarygroupid);
                                                                    var findnewbalance11 = findnewaccgrup11.Accgroupbalance;
                                                                    var findnewdebitbalance11 = findnewaccgrup11.DebitAmount;
                                                                    var findnewcreditbalance11 = findnewaccgrup11.CreditAmount;

                                                                    if (findnewaccgrup11.Accunderprimarygroupid != 0 && findnewaccgrup11.Accunderprimarygroupid != null)
                                                                    {
                                                                        findnewaccgrup11.Accgroupbalance = findnewbalance11 + oldaccg.Accgroupbalance;
                                                                        findnewaccgrup11.DebitAmount = findnewdebitbalance11 + oldaccg.DebitAmount;
                                                                        findnewaccgrup11.CreditAmount = findnewcreditbalance11 + oldaccg.CreditAmount;
                                                                        services.UpdateAccgroup(findnewaccgrup11);


                                                                    }
                                                                    else
                                                                    {
                                                                        findnewaccgrup11.Accgroupbalance = findnewbalance11 + oldaccg.Accgroupbalance;
                                                                        findnewaccgrup11.DebitAmount = findnewdebitbalance11 + oldaccg.DebitAmount;
                                                                        findnewaccgrup11.CreditAmount = findnewcreditbalance11 + oldaccg.CreditAmount;
                                                                        services.UpdateAccgroup(findnewaccgrup11);
                                                                    }





                                                                }
                                                                else
                                                                {
                                                                    findnewaccgrup10.Accgroupbalance = findnewbalance10 + oldaccg.Accgroupbalance;
                                                                    findnewaccgrup10.DebitAmount = findnewdebitbalance10 + oldaccg.DebitAmount;
                                                                    findnewaccgrup10.CreditAmount = findnewcreditbalance10 + oldaccg.CreditAmount;
                                                                    services.UpdateAccgroup(findnewaccgrup10);
                                                                }




                                                            }
                                                            else
                                                            {
                                                                findnewaccgrup9.Accgroupbalance = findnewbalance9 + oldaccg.Accgroupbalance;
                                                                findnewaccgrup9.DebitAmount = findnewdebitbalance9 + oldaccg.DebitAmount;
                                                                findnewaccgrup9.CreditAmount = findnewcreditbalance9 + oldaccg.CreditAmount;
                                                                services.UpdateAccgroup(findnewaccgrup9);
                                                            }





                                                        }
                                                        else
                                                        {
                                                            findnewaccgrup8.Accgroupbalance = findnewbalance8 + oldaccg.Accgroupbalance;
                                                            findnewaccgrup8.DebitAmount = findnewdebitbalance8 + oldaccg.DebitAmount;
                                                            findnewaccgrup8.CreditAmount = findnewcreditbalance8 + oldaccg.CreditAmount;
                                                            services.UpdateAccgroup(findnewaccgrup8);
                                                        }



                                                    }
                                                    else
                                                    {
                                                        findnewaccgrup7.Accgroupbalance = findnewbalance7 + oldaccg.Accgroupbalance;
                                                        findnewaccgrup7.DebitAmount = findnewdebitbalance7 + oldaccg.DebitAmount;
                                                        findnewaccgrup7.CreditAmount = findnewcreditbalance7 + oldaccg.CreditAmount;
                                                        services.UpdateAccgroup(findnewaccgrup7);
                                                    }




                                                }
                                                else
                                                {
                                                    findnewaccgrup6.Accgroupbalance = findnewbalance6 + oldaccg.Accgroupbalance;
                                                    findnewaccgrup6.DebitAmount = findnewdebitbalance6 + oldaccg.DebitAmount;
                                                    findnewaccgrup6.CreditAmount = findnewcreditbalance6 + oldaccg.CreditAmount;
                                                    services.UpdateAccgroup(findnewaccgrup6);
                                                }



                                            }
                                            else
                                            {
                                                findnewaccgrup5.Accgroupbalance = findnewbalance5 + oldaccg.Accgroupbalance;
                                                findnewaccgrup5.DebitAmount = findnewdebitbalance5 + oldaccg.DebitAmount;
                                                findnewaccgrup5.CreditAmount = findnewcreditbalance5 + oldaccg.CreditAmount;
                                                services.UpdateAccgroup(findnewaccgrup5);
                                            }


                                        }
                                        else
                                        {
                                            findnewaccgrup4.Accgroupbalance = findnewbalance4 + oldaccg.Accgroupbalance;
                                            findnewaccgrup4.DebitAmount = findnewdebitbalance4 + oldaccg.DebitAmount;
                                            findnewaccgrup4.CreditAmount = findnewcreditbalance4 + oldaccg.CreditAmount;
                                            services.UpdateAccgroup(findnewaccgrup4);
                                        }


                                    }
                                    else
                                    {
                                        findnewaccgrup3.Accgroupbalance = findnewbalance3 + oldaccg.Accgroupbalance;
                                        findnewaccgrup3.DebitAmount = findnewdebitbalance3 + oldaccg.DebitAmount;
                                        findnewaccgrup3.CreditAmount = findnewcreditbalance3 + oldaccg.CreditAmount;
                                        services.UpdateAccgroup(findnewaccgrup3);

                                    }

                                }
                                else
                                {
                                    findnewaccgrup2.Accgroupbalance = findnewbalance2 + oldaccg.Accgroupbalance;
                                    findnewaccgrup2.DebitAmount = findnewdebitbalance2 + oldaccg.DebitAmount;
                                    findnewaccgrup2.CreditAmount = findnewcreditbalance2 + oldaccg.CreditAmount;
                                    services.UpdateAccgroup(findnewaccgrup2);


                                }

                            }
                            else
                            {
                                findnewaccgrup.Accgroupbalance = findnewbalance + oldaccg.Accgroupbalance;
                                findnewaccgrup.DebitAmount = findnewdebitbalance + oldaccg.DebitAmount;
                                findnewaccgrup.CreditAmount = findnewcreditbalance + oldaccg.CreditAmount;
                                services.UpdateAccgroup(findnewaccgrup);


                            }
                        }


                        oldaccg.Accgroupname = Accgroupname;
                        oldaccg.Accgroupalias = Accgroupalias;
                        oldaccg.AccunderprimarygroupName = Accunderprimarygroup;
                        oldaccg.Accunderprimarygroupid = Accprimarygroupid;
                        oldaccg.MainGroupID = MainGroupId;
                        services.UpdateAccgroup(oldaccg);

                        var accountchildgroups = db.AccGroup.Where(i => i.Accunderprimarygroupid == oldaccg.AccgroupID).ToList();
                        for (int i = 0; i < accountchildgroups.Count; i++)
                        {
                            var accountidgups = accountchildgroups[i].AccgroupID;
                            var childaccounthgroup = db.AccGroup.Where(r => r.AccgroupID == accountidgups).FirstOrDefault();
                            childaccounthgroup.AccunderprimarygroupName = Accgroupname;
                            db.Entry(childaccounthgroup).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                        }
                        return RedirectToAction("AccountGroup");

                    }
                    catch (Exception e)
                    {

                        ViewBag.message = e.ToString();
                        return View("/Views/Shared/Error.cshtml");
                        throw;
                    }

                }
                return RedirectToAction("AccountGroup");



            }


        }
        public JsonResult Getaccgroup(int AccgroupID)
        {

            var olddata = services.Getaccgroupbyid(AccgroupID);
            AccGroup acc = new AccGroup();
            acc.AccgroupID = olddata.AccgroupID;
            acc.Accgroupname = olddata.Accgroupname;
            acc.Accgroupalias = olddata.Accgroupalias;
            acc.Accunderprimarygroupid = olddata.Accunderprimarygroupid;
            acc.AccunderprimarygroupName = olddata.AccunderprimarygroupName;
            acc.MainGroupID = olddata.MainGroupID;


            return Json(acc, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Deleteaccgroup(int id)
        {
            if (id > 0)
            {
                var olddata = services.Getaccgroupbyid(id);
                services.DEleteaccgroup(olddata);


                return RedirectToAction("AccountGroup");

            }
            else
            {
                return RedirectToAction("AccountGroup");

            }

        }

        #endregion AccountGroup
        //------------------Accounts------------------//

        #region Accounts
        public ActionResult Accounts()
        {

            try {
                var list = db.Account.ToList();

                ViewBag.Ledgertype = new SelectList(db.ledgertype, "LegerID", "LedgerName");
                ViewBag.AccGroup = new SelectList(db.AccGroup, "AccgroupID", "Accgroupname");
                ViewBag.Currency = new SelectList(db.Currency, "CurrencyID", "CurrencyName");
                ViewBag.Country = new SelectList(db.Country, "CountryID", "CountryName");
                ViewBag.accounts = new SelectList(db.Account, "AccountID", "AccountName");



                return View(list);


            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }
          
        }
        [HttpPost]
        public ActionResult SaveAccounts(Account account)
        {
            if (account.AccountID == 0)
            {
                var addaccount = account;
                addaccount.ClosingBalance = 0;
                addaccount.DebitTotal = 0;
                addaccount.CreditTotal = 0;
                services.AddAccount(addaccount);


                return RedirectToAction("Accounts");
            }
            else
            {
                if (account.AccountName != null)
                {

                    var oldaccggg = services.Getaccountbyid(account.AccountID);



                    //---------------------Update Account  group  credit----------------------------//

                    var oldaccg = services.Getaccgroupbyid(oldaccggg.Accgroupid);
                    //oldaccg.Accgroupbalance = 
                    var oldnewaccgrup = services.Getaccgroupbyid(oldaccggg.Accgroupid);
                    var oldnewbalance = oldnewaccgrup.Accgroupbalance;
                    var oldnewdebitbalance = oldnewaccgrup.DebitAmount;
                    var oldnewcreditbalance = oldnewaccgrup.CreditAmount;


                    //---------------------Update Account  group 2 credit----------------------------//
                    if (oldnewaccgrup.Accunderprimarygroupid != 0 && oldnewaccgrup.Accunderprimarygroupid != null)
                    {
                        oldnewaccgrup.Accgroupbalance = oldnewbalance - oldaccggg.ClosingBalance;
                        oldnewaccgrup.DebitAmount = oldnewdebitbalance - oldaccggg.DebitTotal;
                        oldnewaccgrup.CreditAmount = oldnewcreditbalance - oldaccggg.CreditTotal;
                        services.UpdateAccgroup(oldnewaccgrup);

                        var oldnewaccgrup2 = services.Getaccgroupbyid(oldnewaccgrup.Accunderprimarygroupid);
                        var oldnewbalance2 = oldnewaccgrup2.Accgroupbalance;
                        var oldnewdebitbalance2 = oldnewaccgrup2.DebitAmount;
                        var oldnewcreditbalance2 = oldnewaccgrup2.CreditAmount;
                        if (oldnewaccgrup2.Accunderprimarygroupid != 0 && oldnewaccgrup2.Accunderprimarygroupid != null)
                        {
                            oldnewaccgrup2.Accgroupbalance = oldnewbalance2 - oldaccggg.ClosingBalance;
                            oldnewaccgrup2.DebitAmount = oldnewdebitbalance2 - oldaccggg.DebitTotal;
                            oldnewaccgrup2.CreditAmount = oldnewcreditbalance2 - oldaccggg.CreditTotal;
                            services.UpdateAccgroup(oldnewaccgrup2);

                            //---------------------Update Account group 3 credit----------------------------//
                            var oldnewaccgrup3 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                            var oldnewbalance3 = oldnewaccgrup.Accgroupbalance;
                            var oldnewdebitbalance3 = oldnewaccgrup.DebitAmount;
                            var oldnewcreditbalance3 = oldnewaccgrup.CreditAmount;
                            if (oldnewaccgrup3.Accunderprimarygroupid != 0 && oldnewaccgrup3.Accunderprimarygroupid != null)
                            {
                                oldnewaccgrup3.Accgroupbalance = oldnewbalance3 - oldaccggg.ClosingBalance;
                                oldnewaccgrup3.DebitAmount = oldnewdebitbalance3 - oldaccggg.DebitTotal;
                                oldnewaccgrup3.CreditAmount = oldnewcreditbalance3 - oldaccggg.CreditTotal;
                                services.UpdateAccgroup(oldnewaccgrup3);


                                //---------------------Update Account primary group 4 credit----------------------------//
                                var oldnewaccgrup4 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                var oldnewbalance4 = oldnewaccgrup.Accgroupbalance;
                                var oldnewdebitbalance4 = oldnewaccgrup.DebitAmount;
                                var oldnewcreditbalance4 = oldnewaccgrup.CreditAmount;
                                if (oldnewaccgrup4.Accunderprimarygroupid != 0 && oldnewaccgrup4.Accunderprimarygroupid != null)
                                {
                                    oldnewaccgrup4.Accgroupbalance = oldnewbalance4 - oldaccggg.ClosingBalance;
                                    oldnewaccgrup4.DebitAmount = oldnewdebitbalance4 - oldaccggg.DebitTotal;
                                    oldnewaccgrup4.CreditAmount = oldnewcreditbalance4 - oldaccggg.CreditTotal;
                                    services.UpdateAccgroup(oldnewaccgrup4);


                                    //---------------------Update Account primary group 5 credit----------------------------//
                                    var oldnewaccgrup5 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                    var oldnewbalance5 = oldnewaccgrup.Accgroupbalance;
                                    var oldnewdebitbalance5 = oldnewaccgrup.DebitAmount;
                                    var oldnewcreditbalance5 = oldnewaccgrup.CreditAmount;

                                    if (oldnewaccgrup5.Accunderprimarygroupid != 0 && oldnewaccgrup5.Accunderprimarygroupid != null)
                                    {
                                        oldnewaccgrup5.Accgroupbalance = oldnewbalance5 - oldaccggg.ClosingBalance;
                                        oldnewaccgrup5.DebitAmount = oldnewdebitbalance5 - oldaccggg.DebitTotal;
                                        oldnewaccgrup5.CreditAmount = oldnewcreditbalance5 - oldaccggg.CreditTotal;
                                        services.UpdateAccgroup(oldnewaccgrup5);


                                        //---------------------Update Account primary group 6 credit----------------------------//
                                        var oldnewaccgrup6 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                        var oldnewbalance6 = oldnewaccgrup.Accgroupbalance;
                                        var oldnewdebitbalance6 = oldnewaccgrup.DebitAmount;
                                        var oldnewcreditbalance6 = oldnewaccgrup.CreditAmount;

                                        if (oldnewaccgrup6.Accunderprimarygroupid != 0 && oldnewaccgrup6.Accunderprimarygroupid != null)
                                        {
                                            oldnewaccgrup6.Accgroupbalance = oldnewbalance6 - oldaccggg.ClosingBalance;
                                            oldnewaccgrup6.DebitAmount = oldnewdebitbalance6 - oldaccggg.DebitTotal;
                                            oldnewaccgrup6.CreditAmount = oldnewcreditbalance6 - oldaccggg.CreditTotal;
                                            services.UpdateAccgroup(oldnewaccgrup6);


                                            //---------------------Update Account primary group 7 credit----------------------------//
                                            var oldnewaccgrup7 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                            var oldnewbalance7 = oldnewaccgrup.Accgroupbalance;
                                            var oldnewdebitbalance7 = oldnewaccgrup.DebitAmount;
                                            var oldnewcreditbalance7 = oldnewaccgrup.CreditAmount;


                                            if (oldnewaccgrup7.Accunderprimarygroupid != 0 && oldnewaccgrup7.Accunderprimarygroupid != null)
                                            {
                                                oldnewaccgrup7.Accgroupbalance = oldnewbalance7 - oldaccggg.ClosingBalance;
                                                oldnewaccgrup7.DebitAmount = oldnewdebitbalance7 - oldaccggg.DebitTotal;
                                                oldnewaccgrup7.CreditAmount = oldnewcreditbalance7 - oldaccggg.CreditTotal;
                                                services.UpdateAccgroup(oldnewaccgrup7);



                                                //---------------------Update Account primary group 8 credit----------------------------//
                                                var oldnewaccgrup8 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                                var oldnewbalance8 = oldnewaccgrup.Accgroupbalance;
                                                var oldnewdebitbalance8 = oldnewaccgrup.DebitAmount;
                                                var oldnewcreditbalance8 = oldnewaccgrup.CreditAmount;

                                                if (oldnewaccgrup8.Accunderprimarygroupid != 0 && oldnewaccgrup8.Accunderprimarygroupid != null)
                                                {
                                                    oldnewaccgrup8.Accgroupbalance = oldnewbalance8 - oldaccggg.ClosingBalance;
                                                    oldnewaccgrup8.DebitAmount = oldnewdebitbalance8 - oldaccggg.DebitTotal;
                                                    oldnewaccgrup8.CreditAmount = oldnewcreditbalance8 - oldaccggg.CreditTotal;
                                                    services.UpdateAccgroup(oldnewaccgrup8);

                                                    //---------------------Update Account primary group 9 credit----------------------------//
                                                    var oldnewaccgrup9 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                                    var oldnewbalance9 = oldnewaccgrup.Accgroupbalance;
                                                    var oldnewdebitbalance9 = oldnewaccgrup.DebitAmount;
                                                    var oldnewcreditbalance9 = oldnewaccgrup.CreditAmount;

                                                    if (oldnewaccgrup9.Accunderprimarygroupid != 0 && oldnewaccgrup9.Accunderprimarygroupid != null)
                                                    {
                                                        oldnewaccgrup9.Accgroupbalance = oldnewbalance9 - oldaccggg.ClosingBalance;
                                                        oldnewaccgrup9.DebitAmount = oldnewdebitbalance9 - oldaccggg.DebitTotal;
                                                        oldnewaccgrup9.CreditAmount = oldnewcreditbalance9 - oldaccggg.CreditTotal;
                                                        services.UpdateAccgroup(oldnewaccgrup9);


                                                        //---------------------Update Account primary group 10 credit----------------------------//
                                                        var oldnewaccgrup10 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                                        var oldnewbalance10 = oldnewaccgrup.Accgroupbalance;
                                                        var oldnewdebitbalance10 = oldnewaccgrup.DebitAmount;
                                                        var oldnewcreditbalance10 = oldnewaccgrup.CreditAmount;

                                                        if (oldnewaccgrup10.Accunderprimarygroupid != 0 && oldnewaccgrup10.Accunderprimarygroupid != null)
                                                        {
                                                            oldnewaccgrup10.Accgroupbalance = oldnewbalance10 - oldaccggg.ClosingBalance;
                                                            oldnewaccgrup10.DebitAmount = oldnewdebitbalance10 - oldaccggg.DebitTotal;
                                                            oldnewaccgrup10.CreditAmount = oldnewcreditbalance10 - oldaccggg.CreditTotal;
                                                            services.UpdateAccgroup(oldnewaccgrup10);

                                                            //---------------------Update Account primary group 9 credit----------------------------//
                                                            var oldnewaccgrup11 = services.Getaccgroupbyid(oldnewaccgrup2.Accunderprimarygroupid);
                                                            var oldnewbalance11 = oldnewaccgrup.Accgroupbalance;
                                                            var oldnewdebitbalance11 = oldnewaccgrup.DebitAmount;
                                                            var oldnewcreditbalance11 = oldnewaccgrup.CreditAmount;

                                                            if (oldnewaccgrup11.Accunderprimarygroupid != 0 && oldnewaccgrup11.Accunderprimarygroupid != null)
                                                            {
                                                                oldnewaccgrup11.Accgroupbalance = oldnewbalance11 - oldaccggg.ClosingBalance;
                                                                oldnewaccgrup11.DebitAmount = oldnewdebitbalance11 - oldaccggg.DebitTotal;
                                                                oldnewaccgrup11.CreditAmount = oldnewcreditbalance11 - oldaccggg.CreditTotal;
                                                                services.UpdateAccgroup(oldnewaccgrup11);


                                                            }
                                                            else
                                                            {
                                                                oldnewaccgrup11.Accgroupbalance = oldnewbalance11 - oldaccggg.ClosingBalance;
                                                                oldnewaccgrup11.DebitAmount = oldnewdebitbalance11 - oldaccggg.DebitTotal;
                                                                oldnewaccgrup11.CreditAmount = oldnewcreditbalance11 - oldaccggg.CreditTotal;
                                                                services.UpdateAccgroup(oldnewaccgrup11);
                                                            }





                                                        }
                                                        else
                                                        {
                                                            oldnewaccgrup10.Accgroupbalance = oldnewbalance10 - oldaccggg.ClosingBalance;
                                                            oldnewaccgrup10.DebitAmount = oldnewdebitbalance10 - oldaccggg.DebitTotal;
                                                            oldnewaccgrup10.CreditAmount = oldnewcreditbalance10 - oldaccggg.CreditTotal;
                                                            services.UpdateAccgroup(oldnewaccgrup10);
                                                        }




                                                    }
                                                    else
                                                    {
                                                        oldnewaccgrup9.Accgroupbalance = oldnewbalance9 - oldaccggg.ClosingBalance;
                                                        oldnewaccgrup9.DebitAmount = oldnewdebitbalance9 - oldaccggg.DebitTotal;
                                                        oldnewaccgrup9.CreditAmount = oldnewcreditbalance9 - oldaccggg.CreditTotal;
                                                        services.UpdateAccgroup(oldnewaccgrup9);
                                                    }





                                                }
                                                else
                                                {
                                                    oldnewaccgrup8.Accgroupbalance = oldnewbalance8 - oldaccggg.ClosingBalance;
                                                    oldnewaccgrup8.DebitAmount = oldnewdebitbalance8 - oldaccggg.DebitTotal;
                                                    oldnewaccgrup8.CreditAmount = oldnewcreditbalance8 - oldaccggg.CreditTotal;
                                                    services.UpdateAccgroup(oldnewaccgrup8);
                                                }



                                            }
                                            else
                                            {
                                                oldnewaccgrup7.Accgroupbalance = oldnewbalance7 - oldaccggg.ClosingBalance;
                                                oldnewaccgrup7.DebitAmount = oldnewdebitbalance7 - oldaccggg.DebitTotal;
                                                oldnewaccgrup7.CreditAmount = oldnewcreditbalance7 - oldaccggg.CreditTotal;
                                                services.UpdateAccgroup(oldnewaccgrup7);
                                            }




                                        }
                                        else
                                        {
                                            oldnewaccgrup6.Accgroupbalance = oldnewbalance6 - oldaccggg.ClosingBalance;
                                            oldnewaccgrup6.DebitAmount = oldnewdebitbalance6 - oldaccggg.DebitTotal;
                                            oldnewaccgrup6.CreditAmount = oldnewcreditbalance6 - oldaccggg.CreditTotal;
                                            services.UpdateAccgroup(oldnewaccgrup6);
                                        }



                                    }
                                    else
                                    {
                                        oldnewaccgrup5.Accgroupbalance = oldnewbalance5 - oldaccggg.ClosingBalance;
                                        oldnewaccgrup5.DebitAmount = oldnewdebitbalance5 - oldaccggg.DebitTotal;
                                        oldnewaccgrup5.CreditAmount = oldnewcreditbalance5 - oldaccggg.CreditTotal;
                                        services.UpdateAccgroup(oldnewaccgrup5);
                                    }


                                }
                                else
                                {
                                    oldnewaccgrup4.Accgroupbalance = oldnewbalance4 - oldaccggg.ClosingBalance;
                                    oldnewaccgrup4.DebitAmount = oldnewdebitbalance4 - oldaccggg.DebitTotal;
                                    oldnewaccgrup4.CreditAmount = oldnewcreditbalance4 - oldaccggg.CreditTotal;
                                    services.UpdateAccgroup(oldnewaccgrup4);
                                }


                            }
                            else
                            {
                                oldnewaccgrup3.Accgroupbalance = oldnewbalance3 - oldaccggg.ClosingBalance;
                                oldnewaccgrup3.DebitAmount = oldnewdebitbalance3 - oldaccggg.DebitTotal;
                                oldnewaccgrup3.CreditAmount = oldnewcreditbalance3 - oldaccggg.CreditTotal;
                                services.UpdateAccgroup(oldnewaccgrup3);

                            }

                        }
                        else
                        {
                            oldnewaccgrup2.Accgroupbalance = oldnewbalance2 - oldaccggg.ClosingBalance;
                            oldnewaccgrup2.DebitAmount = oldnewdebitbalance2 - oldaccggg.DebitTotal;
                            oldnewaccgrup2.CreditAmount = oldnewcreditbalance2 - oldaccggg.CreditTotal;
                            services.UpdateAccgroup(oldnewaccgrup2);


                        }

                    }
                    else
                    {
                        oldnewaccgrup.Accgroupbalance = oldnewbalance - oldaccggg.ClosingBalance;
                        oldnewaccgrup.DebitAmount = oldnewdebitbalance - oldaccggg.DebitTotal;
                        oldnewaccgrup.CreditAmount = oldnewcreditbalance - oldaccggg.CreditTotal;
                        services.UpdateAccgroup(oldnewaccgrup);


                    }



                    ////////////////////////////////////////////////////////////////////////

                    var findnewaccgrup = services.Getaccgroupbyid(account.Accgroupid);


                    var findnewbalance = findnewaccgrup.Accgroupbalance;
                    var findnewdebitbalance = findnewaccgrup.DebitAmount;
                    var findnewcreditbalance = findnewaccgrup.CreditAmount;


                    if (findnewaccgrup.Accunderprimarygroupid != 0 && findnewaccgrup.Accunderprimarygroupid != null)
                    {

                        findnewaccgrup.Accgroupbalance = findnewbalance + oldaccggg.ClosingBalance;
                        findnewaccgrup.DebitAmount = findnewdebitbalance + oldaccggg.DebitTotal;
                        findnewaccgrup.CreditAmount = findnewcreditbalance + oldaccggg.CreditTotal;
                        services.UpdateAccgroup(findnewaccgrup);

                        //---------------------Update Account group 2 credit----------------------------//
                        var findnewaccgrup2 = services.Getaccgroupbyid(findnewaccgrup.Accunderprimarygroupid);
                        var findnewbalance2 = findnewaccgrup2.Accgroupbalance;
                        var findnewdebitbalance2 = findnewaccgrup2.DebitAmount;
                        var findnewcreditbalance2 = findnewaccgrup2.CreditAmount;

                        if (findnewaccgrup2.Accunderprimarygroupid != 0 && findnewaccgrup2.Accunderprimarygroupid != null)
                        {

                            findnewaccgrup2.Accgroupbalance = findnewbalance2 + oldaccggg.ClosingBalance;
                            findnewaccgrup2.DebitAmount = findnewdebitbalance2 + oldaccggg.DebitTotal;
                            findnewaccgrup2.CreditAmount = findnewcreditbalance2 + oldaccggg.CreditTotal;
                            services.UpdateAccgroup(findnewaccgrup2);

                            //---------------------Update Account group 3 credit----------------------------//
                            var findnewaccgrup3 = services.Getaccgroupbyid(findnewaccgrup2.Accunderprimarygroupid);
                            var findnewbalance3 = findnewaccgrup3.Accgroupbalance;
                            var findnewdebitbalance3 = findnewaccgrup3.DebitAmount;
                            var findnewcreditbalance3 = findnewaccgrup3.CreditAmount;
                            if (findnewaccgrup3.Accunderprimarygroupid != 0 && findnewaccgrup3.Accunderprimarygroupid != null)
                            {
                                findnewaccgrup3.Accgroupbalance = findnewbalance3 + oldaccggg.ClosingBalance;
                                findnewaccgrup3.DebitAmount = findnewdebitbalance3 + oldaccggg.DebitTotal;
                                findnewaccgrup3.CreditAmount = findnewcreditbalance3 + oldaccggg.CreditTotal;
                                services.UpdateAccgroup(findnewaccgrup3);


                                //---------------------Update Account primary group 4 credit----------------------------//
                                var findnewaccgrup4 = services.Getaccgroupbyid(findnewaccgrup3.Accunderprimarygroupid);
                                var findnewbalance4 = findnewaccgrup4.Accgroupbalance;
                                var findnewdebitbalance4 = findnewaccgrup4.DebitAmount;
                                var findnewcreditbalance4 = findnewaccgrup4.CreditAmount;

                                if (findnewaccgrup4.Accunderprimarygroupid != 0 && findnewaccgrup4.Accunderprimarygroupid != null)
                                {
                                    findnewaccgrup4.Accgroupbalance = findnewbalance4 + oldaccggg.ClosingBalance;
                                    findnewaccgrup4.DebitAmount = findnewdebitbalance4 + oldaccggg.DebitTotal;
                                    findnewaccgrup4.CreditAmount = findnewcreditbalance4 + oldaccggg.CreditTotal;
                                    services.UpdateAccgroup(findnewaccgrup4);


                                    //---------------------Update Account primary group 5 credit----------------------------//
                                    var findnewaccgrup5 = services.Getaccgroupbyid(findnewaccgrup4.Accunderprimarygroupid);
                                    var findnewbalance5 = findnewaccgrup5.Accgroupbalance;
                                    var findnewdebitbalance5 = findnewaccgrup5.DebitAmount;
                                    var findnewcreditbalance5 = findnewaccgrup5.CreditAmount;

                                    if (findnewaccgrup5.Accunderprimarygroupid != 0 && findnewaccgrup5.Accunderprimarygroupid != null)
                                    {
                                        findnewaccgrup5.Accgroupbalance = findnewbalance5 + oldaccggg.ClosingBalance;
                                        findnewaccgrup5.DebitAmount = findnewdebitbalance5 + oldaccggg.DebitTotal;
                                        findnewaccgrup5.CreditAmount = findnewcreditbalance5 + oldaccggg.CreditTotal;
                                        services.UpdateAccgroup(findnewaccgrup5);


                                        //---------------------Update Account primary group 6 credit----------------------------//
                                        var findnewaccgrup6 = services.Getaccgroupbyid(findnewaccgrup5.Accunderprimarygroupid);
                                        var findnewbalance6 = findnewaccgrup6.Accgroupbalance;
                                        var findnewdebitbalance6 = findnewaccgrup6.DebitAmount;
                                        var findnewcreditbalance6 = findnewaccgrup6.CreditAmount;

                                        if (findnewaccgrup6.Accunderprimarygroupid != 0 && findnewaccgrup6.Accunderprimarygroupid != null)
                                        {
                                            findnewaccgrup6.Accgroupbalance = findnewbalance6 + oldaccggg.ClosingBalance;
                                            findnewaccgrup6.DebitAmount = findnewdebitbalance6 + oldaccggg.DebitTotal;
                                            findnewaccgrup6.CreditAmount = findnewcreditbalance6 + oldaccggg.CreditTotal;
                                            services.UpdateAccgroup(findnewaccgrup6);


                                            //---------------------Update Account primary group 7 credit----------------------------//
                                            var findnewaccgrup7 = services.Getaccgroupbyid(findnewaccgrup6.Accunderprimarygroupid);
                                            var findnewbalance7 = findnewaccgrup7.Accgroupbalance;
                                            var findnewdebitbalance7 = findnewaccgrup7.DebitAmount;
                                            var findnewcreditbalance7 = findnewaccgrup7.CreditAmount;


                                            if (findnewaccgrup7.Accunderprimarygroupid != 0 && findnewaccgrup7.Accunderprimarygroupid != null)
                                            {
                                                findnewaccgrup7.Accgroupbalance = findnewbalance7 + oldaccggg.ClosingBalance;
                                                findnewaccgrup7.DebitAmount = findnewdebitbalance7 + oldaccggg.DebitTotal;
                                                findnewaccgrup7.CreditAmount = findnewcreditbalance7 + oldaccggg.CreditTotal;
                                                services.UpdateAccgroup(findnewaccgrup7);



                                                //---------------------Update Account primary group 8 credit----------------------------//
                                                var findnewaccgrup8 = services.Getaccgroupbyid(findnewaccgrup7.Accunderprimarygroupid);
                                                var findnewbalance8 = findnewaccgrup8.Accgroupbalance;
                                                var findnewdebitbalance8 = findnewaccgrup8.DebitAmount;
                                                var findnewcreditbalance8 = findnewaccgrup8.CreditAmount;

                                                if (findnewaccgrup8.Accunderprimarygroupid != 0 && findnewaccgrup8.Accunderprimarygroupid != null)
                                                {
                                                    findnewaccgrup8.Accgroupbalance = findnewbalance8 + oldaccggg.ClosingBalance;
                                                    findnewaccgrup8.DebitAmount = findnewdebitbalance8 + oldaccggg.DebitTotal;
                                                    findnewaccgrup8.CreditAmount = findnewcreditbalance8 + oldaccggg.CreditTotal;
                                                    services.UpdateAccgroup(findnewaccgrup8);

                                                    //---------------------Update Account primary group 9 credit----------------------------//
                                                    var findnewaccgrup9 = services.Getaccgroupbyid(findnewaccgrup8.Accunderprimarygroupid);
                                                    var findnewbalance9 = findnewaccgrup9.Accgroupbalance;
                                                    var findnewdebitbalance9 = findnewaccgrup9.DebitAmount;
                                                    var findnewcreditbalance9 = findnewaccgrup9.CreditAmount;

                                                    if (findnewaccgrup9.Accunderprimarygroupid != 0 && findnewaccgrup9.Accunderprimarygroupid != null)
                                                    {
                                                        findnewaccgrup9.Accgroupbalance = findnewbalance9 + oldaccggg.ClosingBalance;
                                                        findnewaccgrup9.DebitAmount = findnewdebitbalance9 + oldaccggg.DebitTotal;
                                                        findnewaccgrup9.CreditAmount = findnewcreditbalance9 + oldaccggg.CreditTotal;
                                                        services.UpdateAccgroup(findnewaccgrup9);


                                                        //---------------------Update Account primary group 10 credit----------------------------//
                                                        var findnewaccgrup10 = services.Getaccgroupbyid(findnewaccgrup9.Accunderprimarygroupid);
                                                        var findnewbalance10 = findnewaccgrup10.Accgroupbalance;
                                                        var findnewdebitbalance10 = findnewaccgrup10.DebitAmount;
                                                        var findnewcreditbalance10 = findnewaccgrup10.CreditAmount;

                                                        if (findnewaccgrup10.Accunderprimarygroupid != 0 && findnewaccgrup10.Accunderprimarygroupid != null)
                                                        {
                                                            findnewaccgrup10.Accgroupbalance = findnewbalance10 + oldaccggg.ClosingBalance;
                                                            findnewaccgrup10.DebitAmount = findnewdebitbalance10 + oldaccggg.DebitTotal;
                                                            findnewaccgrup10.CreditAmount = findnewcreditbalance10 + oldaccggg.CreditTotal;
                                                            services.UpdateAccgroup(findnewaccgrup10);

                                                            //---------------------Update Account primary group 9 credit----------------------------//
                                                            var findnewaccgrup11 = services.Getaccgroupbyid(findnewaccgrup10.Accunderprimarygroupid);
                                                            var findnewbalance11 = findnewaccgrup11.Accgroupbalance;
                                                            var findnewdebitbalance11 = findnewaccgrup11.DebitAmount;
                                                            var findnewcreditbalance11 = findnewaccgrup11.CreditAmount;

                                                            if (findnewaccgrup11.Accunderprimarygroupid != 0 && findnewaccgrup11.Accunderprimarygroupid != null)
                                                            {
                                                                findnewaccgrup11.Accgroupbalance = findnewbalance11 + oldaccggg.ClosingBalance;
                                                                findnewaccgrup11.DebitAmount = findnewdebitbalance11 + oldaccggg.DebitTotal;
                                                                findnewaccgrup11.CreditAmount = findnewcreditbalance11 + oldaccggg.CreditTotal;
                                                                services.UpdateAccgroup(findnewaccgrup11);


                                                            }
                                                            else
                                                            {
                                                                findnewaccgrup11.Accgroupbalance = findnewbalance11 + oldaccggg.ClosingBalance;
                                                                findnewaccgrup11.DebitAmount = findnewdebitbalance11 + oldaccggg.DebitTotal;
                                                                findnewaccgrup11.CreditAmount = findnewcreditbalance11 + oldaccggg.CreditTotal;
                                                                services.UpdateAccgroup(findnewaccgrup11);
                                                            }





                                                        }
                                                        else
                                                        {
                                                            findnewaccgrup10.Accgroupbalance = findnewbalance10 + oldaccggg.ClosingBalance;
                                                            findnewaccgrup10.DebitAmount = findnewdebitbalance10 + oldaccggg.DebitTotal;
                                                            findnewaccgrup10.CreditAmount = findnewcreditbalance10 + oldaccggg.CreditTotal;
                                                            services.UpdateAccgroup(findnewaccgrup10);
                                                        }




                                                    }
                                                    else
                                                    {
                                                        findnewaccgrup9.Accgroupbalance = findnewbalance9 + oldaccggg.ClosingBalance;
                                                        findnewaccgrup9.DebitAmount = findnewdebitbalance9 + oldaccggg.DebitTotal;
                                                        findnewaccgrup9.CreditAmount = findnewcreditbalance9 + oldaccggg.CreditTotal;
                                                        services.UpdateAccgroup(findnewaccgrup9);
                                                    }





                                                }
                                                else
                                                {
                                                    findnewaccgrup8.Accgroupbalance = findnewbalance8 + oldaccggg.ClosingBalance;
                                                    findnewaccgrup8.DebitAmount = findnewdebitbalance8 + oldaccggg.DebitTotal;
                                                    findnewaccgrup8.CreditAmount = findnewcreditbalance8 + oldaccggg.CreditTotal;
                                                    services.UpdateAccgroup(findnewaccgrup8);
                                                }



                                            }
                                            else
                                            {
                                                findnewaccgrup7.Accgroupbalance = findnewbalance7 + oldaccggg.ClosingBalance;
                                                findnewaccgrup7.DebitAmount = findnewdebitbalance7 + oldaccggg.DebitTotal;
                                                findnewaccgrup7.CreditAmount = findnewcreditbalance7 + oldaccggg.CreditTotal;
                                                services.UpdateAccgroup(findnewaccgrup7);
                                            }




                                        }
                                        else
                                        {
                                            findnewaccgrup6.Accgroupbalance = findnewbalance6 + oldaccggg.ClosingBalance;
                                            findnewaccgrup6.DebitAmount = findnewdebitbalance6 + oldaccggg.DebitTotal;
                                            findnewaccgrup6.CreditAmount = findnewcreditbalance6 + oldaccggg.CreditTotal;
                                            services.UpdateAccgroup(findnewaccgrup6);
                                        }



                                    }
                                    else
                                    {
                                        findnewaccgrup5.Accgroupbalance = findnewbalance5 + oldaccggg.ClosingBalance;
                                        findnewaccgrup5.DebitAmount = findnewdebitbalance5 + oldaccggg.DebitTotal;
                                        findnewaccgrup5.CreditAmount = findnewcreditbalance5 + oldaccggg.CreditTotal;
                                        services.UpdateAccgroup(findnewaccgrup5);
                                    }


                                }
                                else
                                {
                                    findnewaccgrup4.Accgroupbalance = findnewbalance4 + oldaccggg.ClosingBalance;
                                    findnewaccgrup4.DebitAmount = findnewdebitbalance4 + oldaccggg.DebitTotal;
                                    findnewaccgrup4.CreditAmount = findnewcreditbalance4 + oldaccggg.CreditTotal;
                                    services.UpdateAccgroup(findnewaccgrup4);
                                }


                            }
                            else
                            {
                                findnewaccgrup3.Accgroupbalance = findnewbalance3 + oldaccggg.ClosingBalance;
                                findnewaccgrup3.DebitAmount = findnewdebitbalance3 + oldaccggg.DebitTotal;
                                findnewaccgrup3.CreditAmount = findnewcreditbalance3 + oldaccggg.CreditTotal;
                                services.UpdateAccgroup(findnewaccgrup3);

                            }

                        }
                        else
                        {
                            findnewaccgrup2.Accgroupbalance = findnewbalance2 + oldaccggg.ClosingBalance;
                            findnewaccgrup2.DebitAmount = findnewdebitbalance2 + oldaccggg.DebitTotal;
                            findnewaccgrup2.CreditAmount = findnewcreditbalance2 + oldaccggg.CreditTotal;
                            services.UpdateAccgroup(findnewaccgrup2);


                        }

                    }
                    else
                    {
                        findnewaccgrup.Accgroupbalance = findnewbalance + oldaccggg.ClosingBalance;
                        findnewaccgrup.DebitAmount = findnewdebitbalance + oldaccggg.DebitTotal;
                        findnewaccgrup.CreditAmount = findnewcreditbalance + oldaccggg.CreditTotal;
                        services.UpdateAccgroup(findnewaccgrup);


                    }

















                    oldaccggg.AccountName = account.AccountName;
                    oldaccggg.Alias = account.Alias;
                    oldaccggg.Print_Name = account.Print_Name;
                    //oldaccg.LedgerTypeid = 
                    oldaccggg.Accgroupid = account.Accgroupid;
                    oldaccggg.CurrencyID = account.CurrencyID;
                    oldaccggg.Opening_Balance = account.Opening_Balance;
                    oldaccggg.Address = account.Address;
                    oldaccggg.CountryID = account.CountryID;
                    oldaccggg.CNIC = account.CNIC;
                    oldaccggg.mobileNum = account.mobileNum;
                    oldaccggg.email = account.email;
                    oldaccggg.BeneficiaryName = account.BeneficiaryName;
                    if (account.SubledgeraccountID == null)
                    {
                        oldaccggg.SubledgeraccountID = 0;
                    }
                    else
                    {
                        oldaccggg.SubledgeraccountID = account.SubledgeraccountID;
                    }

                    oldaccggg.Accgroupid = account.Accgroupid;
                    services.Updateaccount(oldaccggg);

                    return RedirectToAction("Accounts");

                }
                return RedirectToAction("Accounts");
            }
        }
        public JsonResult getaccounts(int AccgroupID)
        {


            var olddata = services.Getaccountbyid(AccgroupID);
            Account account = new Account();
            account.AccountID = olddata.AccountID;
            account.AccountName = olddata.AccountName;


            account.Alias = olddata.Alias;

            account.Print_Name = olddata.Print_Name;
            account.LedgerTypeid = olddata.LedgerTypeid;
            account.Accgroupid = olddata.Accgroupid;
            account.CurrencyID = olddata.CurrencyID;
            account.Opening_Balance = olddata.Opening_Balance;
            account.Address = olddata.Address;
            account.CountryID = olddata.CountryID;
            account.CNIC = olddata.CNIC;
            account.mobileNum = olddata.mobileNum;
            account.email = olddata.email;
            account.BeneficiaryName = olddata.BeneficiaryName;

            return Json(account, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindAccount(string accname)
        {
            //var findacc = db.Account.Contains(accname);
            var name = "";
            var namematch = db.Account.Where(h => h.AccountName.Contains(accname)).ToList();

            if (namematch.Count > 0)
            {
                name = "Account Name already exits";
            }
            else
            {
                name = "";
            }

            return Json(name, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteAccount(int id)
        {
            if (id > 0)
            {
                var olddata = services.Getaccountbyid(id);
                services.deleteaccount(olddata);


                return RedirectToAction("Accounts");

            }
            else
            {
                return RedirectToAction("Accounts");

            }

        }
        public JsonResult getsubgroupid(int accid)
        {
            var Accgroupid = db.Account.Find(accid).Accgroupid;



            return Json(Accgroupid, JsonRequestBehavior.AllowGet);
        }
        #endregion Accounts

        //------------------Currency------------------//

        #region Currency
        public ActionResult Currency()
        {
            var getitems = db.Currency.ToList();

            //ViewBag.purchaseaccountid = new SelectList(db.Units, "AccountID", "AccountName");


            return View(getitems);
        }
        public ActionResult GetCurrency(int itemid = 0)
        {
            if (itemid > 0)
            {
                Currency items = new Currency();

                var item = services.GetCurrencyid(itemid);
                items.CurrencyID = item.CurrencyID;
                items.CurrencyName = item.CurrencyName;
                //items.MaterialCentreGroupID = item.MaterialCentreGroupID;
                items.CurrencyShortName = item.CurrencyShortName;
                items.LowRate = item.LowRate;
                items.HighRate = item.HighRate;

                return Json(items, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("");
            }

        }

        [HttpPost]
        public ActionResult SaveCurrency(Currency itemform)
        {

            if (itemform.CurrencyID > 0)
            {
                if (itemform.CurrencyID != 0)
                {
                    var itemid = services.GetCurrencyid(itemform.CurrencyID);



                    itemid.CurrencyName = itemform.CurrencyName;
                    //itemid.MaterialCentreGroupID = itemform.MaterialCentreGroupID;
                    itemid.CurrencyShortName = itemform.CurrencyShortName;
                    itemid.LowRate = itemform.LowRate;
                    itemid.HighRate = itemform.HighRate;



                    services.UpdateCurrency(itemid);



                }
                return RedirectToAction("Currency");

            }
            else
            {
                if (itemform.CurrencyName != null)
                {
                    Currency currencyform = new Currency();
                    //materialcentre.MaterialCentreGroupID = itemform.MaterialCentreGroupID;
                    currencyform.CurrencyName = itemform.CurrencyName;
                    currencyform.LowRate = itemform.LowRate;
                    currencyform.CurrencyShortName = itemform.CurrencyShortName;
                    currencyform.HighRate = itemform.HighRate;

                    services.AddCurrency(currencyform);

                }
                return RedirectToAction("Currency");

            }

        }

        public ActionResult DeleteCurrency(int itemid = 0)
        {
            if (itemid > 0)
            {
                var items = services.Getmaterialcentrebyid(itemid);
                services.DEleteMaterialCentre(items);

                //var findstock = db.Stock.Find(itemid);
                //db.Stock.Remove(findstock);
                //db.SaveChanges();


                return RedirectToAction("MaterialCentre");

            }
            return RedirectToAction("MaterialCentre");


        }

        #endregion Currency

        //------------------Material Group Centre------------------//
        #region MaterialCentreGroup
        public ActionResult MaterialGroup()
        {
            var masterialgrouplist = db.MaterialCentreGroup.ToList();
            ViewBag.materialgroups = new SelectList(db.MaterialCentreGroup, "MaterialGroupID", "MaterialGroupName", 0);



            return View(masterialgrouplist);
        }


        public ActionResult SaveMaterialGroup(int itemgrupid, string itemgrupname, string itemgrupalias, string itemgrupprimname, int? itemgrupprimid)
        {
            if (itemgrupid == 0 && itemgrupname != null)
            {
                MaterialCentreGroup acg = new MaterialCentreGroup();
                acg.MaterialGroupName = itemgrupname;
                acg.Alias = itemgrupalias;
                acg.MaterialGroupUndergroupName = itemgrupprimname;
                acg.MaterialGroupUndergroupid = itemgrupprimid;
                int? val = 0;
                if (itemgrupprimid == 0)
                {
                    val = 0;
                }
                else
                {
                    val = itemgrupprimid;
                }
                acg.MaterialGroupID = (int)val;


                var itemgroup = acg;

                services.AddMaterialCentregroup(itemgroup);


                return RedirectToAction("MaterialGroup");
            }
            else
            {
                if (itemgrupname != null && itemgrupid != 0)
                {

                    var olditemg = services.GetmaterialcentreGroupbyid(itemgrupid);

                    olditemg.MaterialGroupName = itemgrupname;
                    olditemg.Alias = itemgrupalias;
                    olditemg.MaterialGroupUndergroupName = itemgrupprimname;
                    olditemg.MaterialGroupUndergroupid = (int)itemgrupprimid;
                    services.UpdatematerialcentreGroup(olditemg);

                    return RedirectToAction("MaterialGroup");

                }
                return RedirectToAction("MaterialGroup");


            }


        }

        public ActionResult GetMaterialGroup(int itemgid)
        {
            if (itemgid > 0)
            {
                MaterialCentreGroup itemgroup = new MaterialCentreGroup();

                var olditemgrop = services.GetmaterialcentreGroupbyid(itemgid);

                itemgroup.MaterialGroupID = olditemgrop.MaterialGroupID;
                itemgroup.MaterialGroupName = olditemgrop.MaterialGroupName;
                itemgroup.Alias = olditemgrop.Alias;
                itemgroup.MaterialGroupUndergroupid = olditemgrop.MaterialGroupUndergroupid;

                return Json(itemgroup, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("");
            }


        }

        public ActionResult DeleteMaterialGroup(int itemgroupid)
        {
            if (itemgroupid > 0)
            {
                var items = services.GetmaterialcentreGroupbyid(itemgroupid);
                services.DEleteMaterialCentreGroup(items);
                return RedirectToAction("MaterialGroup");

            }
            return RedirectToAction("MaterialGroup");


        }

        #endregion MaterialCentreGroup

        //------------------Material  Centre------------------//

        #region MaterialCentre
        public ActionResult MaterialCentre()
        {
            var getitems = db.MaterialCentre.ToList();
            ViewBag.materialcentregroup = new SelectList(db.MaterialCentreGroup, "MaterialGroupID", "MaterialGroupName");

            ViewBag.salesaccountid = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.purchaseaccountid = new SelectList(db.Account, "AccountID", "AccountName");
            //ViewBag.purchaseaccountid = new SelectList(db.Units, "AccountID", "AccountName");


            return View(getitems);
        }
        public ActionResult GetMaterialCentre(int itemid = 0)
        {
            if (itemid > 0)
            {
                MaterialCentre items = new MaterialCentre();

                var item = services.Getmaterialcentrebyid(itemid);
                items.MaterialcentreID = item.MaterialcentreID;
                items.MaterialCentreName = item.MaterialCentreName;
                //items.MaterialCentreGroupID = item.MaterialCentreGroupID;
                items.SalesAccountid = item.SalesAccountid;
                items.PurchaseAccountid = item.PurchaseAccountid;
                items.Alias = item.Alias;
                items.Address = item.Address;

                return Json(items, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("");
            }

        }

        [HttpPost]
        public ActionResult SaveMaterialCentre(MaterialCentre itemform)
        {

            if (itemform.MaterialcentreID > 0)
            {
                if (itemform.MaterialcentreID != 0)
                {
                    var itemid = services.Getmaterialcentrebyid(itemform.MaterialcentreID);



                    itemid.MaterialCentreName = itemform.MaterialCentreName;
                    //itemid.MaterialCentreGroupID = itemform.MaterialCentreGroupID;
                    itemid.SalesAccountid = itemform.SalesAccountid;
                    itemid.PurchaseAccountid = itemform.PurchaseAccountid;
                    itemid.Alias = itemform.Alias;
                    itemid.PrintName = itemform.PrintName;
                    itemid.Address = itemform.Address;



                    services.Updatematerialcentre(itemid);



                }
                return RedirectToAction("MaterialCentre");

            }
            else
            {
                if (itemform.MaterialCentreName != null)
                {
                    MaterialCentre materialcentre = new MaterialCentre();
                    //materialcentre.MaterialCentreGroupID = itemform.MaterialCentreGroupID;
                    materialcentre.MaterialCentreName = itemform.MaterialCentreName;
                    materialcentre.Address = itemform.Address;
                    materialcentre.Alias = itemform.Alias;
                    materialcentre.PrintName = itemform.PrintName;
                    materialcentre.PurchaseAccountid = itemform.PurchaseAccountid;
                    materialcentre.SalesAccountid = itemform.SalesAccountid;

                    services.AddMaterialCentre(materialcentre);

                }
                return RedirectToAction("MaterialCentre");

            }

        }

        public ActionResult DeleteMaterialCentre(int itemid = 0)
        {
            if (itemid > 0)
            {
                var items = services.Getmaterialcentrebyid(itemid);
                services.DEleteMaterialCentre(items);

                //var findstock = db.Stock.Find(itemid);
                //db.Stock.Remove(findstock);
                //db.SaveChanges();


                return RedirectToAction("MaterialCentre");

            }
            return RedirectToAction("MaterialCentre");


        }

        #endregion MaterialCentre
        //------------------Reports ------------------// 

        #region Reports
        [HttpGet]
        public ActionResult ShowAccLedger()
        {
            ViewBag.accounts = new SelectList(db.Account, "AccountID", "AccountName");

            return View();
        }

        [HttpPost]
        public ActionResult ShowAccLedger(int accountname, DateTime startdate, DateTime enddate)
        {
            try
            {
                con.Open();

                ViewBag.accounts = new SelectList(db.Account, "AccountID", "AccountName");
                //SqlConnection con = new SqlConnection("Data Source=CLOUDXP\\SAADPC;Initial Catalog=MVCDB;Integrated Security=True"); 
                SqlCommand cmd = new SqlCommand("Sp_AccLedger", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sdate", startdate);
                cmd.Parameters.AddWithValue("@edate", enddate);
                cmd.Parameters.AddWithValue("@accountid", accountname);
                SqlDataAdapter sd = new SqlDataAdapter(cmd);
                accledger ds = new accledger();
                sd.Fill(ds);


                DataSet ds2 = ds;
                ViewBag.accountledger = ds2.Tables[0];
                con.Close();


                var accountname2 = db.Account.Where(m => m.AccountID == accountname).FirstOrDefault();
                tbmodel.Account = accountname2;
                tbmodel.startdate2 = startdate.ToShortDateString();
                tbmodel.enddate3 = enddate.ToShortDateString();
                //ReportDocument rd = new ReportDocument();
                //rd.Load(Path.Combine(Server.MapPath("~/Views/Masters/Accledger.rpt")));
                //rd.SetDataSource(ds);
                //rd.SetParameterValue("@sdate", startdate);
                //rd.SetParameterValue("@edate", enddate); 
                //rd.SetParameterValue("@accountid", accountname);
                //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //return File(stream, "application/pdf",);

                //throw new Exception("abc");
                return PartialView("~/Views/Masters/accledgerview.cshtml", tbmodel);
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }

        }



        public ActionResult testview()
        {
            try
            {

                ViewBag.accounts = new SelectList(db.Account, "AccountID", "AccountName");


                var accountname2 = db.Account.Where(m => m.AccountID == 2054).FirstOrDefault().AccountName;
                ViewBag.AccountName = accountname2;
                //ReportDocument rd = new ReportDocument();
                //rd.Load(Path.Combine(Server.MapPath("~/Views/Masters/Accledger.rpt")));
                //rd.SetDataSource(ds);
                //rd.SetParameterValue("@sdate", startdate);
                //rd.SetParameterValue("@edate", enddate); 
                //rd.SetParameterValue("@accountid", accountname);
                //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //return File(stream, "application/pdf"); 

                //throw new Exception("abc");
                return PartialView("~/Views/Masters/accledgerview.cshtml");
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }

        }
        public ActionResult TrialBalance()
            {
            //DataSet ds = treeservice.TrialBalance();
            //ViewBag.trialbalance = ds.Tables[0];

            //DataSet ds2 = treeservice.GetTrialBHierchal();
            //ViewBag.trialbalanceheirchal = ds2.Tables[0];


            trialbalanceviewmodel tbmodel = new trialbalanceviewmodel();
            tbmodel.reporttypeid = 1;

            //DataTable dt = treeservice.TrialBalance(); 

            //tbmodel.childId = dt.Columns[0];
            //tbmodel.parentId = Convert.ToInt16(dt.Columns[1]);
            //tbmodel.childName = Convert.ToString(dt.Columns[2]);
            //tbmodel.level = Convert.ToInt16(dt.Columns[3]);
            //tbmodel.orderSequence = Convert.ToString(dt.Columns[4]);
            //tbmodel.DebitTotal = Convert.ToInt32(dt.Columns[5]);
            //tbmodel.CreditTotal = Convert.ToInt16(dt.Columns[6]);
            //tbmodel.ClosingBalance = Convert.ToInt16(dt.Columns[7]); 
            var totaldebit = db.VoucherMaster.Where(m => m.DrCrType == 2).ToList();


            for (int i = 0; i < totaldebit.Count; i++)
            {
                var ss = totaldebit[i].Partyid_Accountid;
                var accounttotal = totaldebit.Where(m => m.Partyid_Accountid == ss).Sum(m => m.VoucherFinalTotal);
            }

            return View(tbmodel);
        }
        [HttpPost]
        public ActionResult TrialBalance(DateTime enddate)
        {


            ////DataTable dt = treeservice.TrialBalance(); 
            //DataSet ds = treeservice.TrialBalance();
            //ViewBag.trialbalance = ds.Tables[0];


            //DataSet ds2 = treeservice.GetTrialBHierchal();
            //ViewBag.trialbalanceheirchal = ds2.Tables[0];
            //tbmodel.childId = dt.Columns[0];
            //tbmodel.parentId = Convert.ToInt16(dt.Columns[1]);
            //tbmodel.childName = Convert.ToString(dt.Columns[2]);
            //tbmodel.level = Convert.ToInt16(dt.Columns[3]);
            //tbmodel.orderSequence = Convert.ToString(dt.Columns[4]);
            //tbmodel.DebitTotal = Convert.ToInt32(dt.Columns[5]);
            //tbmodel.CreditTotal = Convert.ToInt16(dt.Columns[6]);
            //tbmodel.ClosingBalance = Convert.ToInt16(dt.Columns[7]); 


            var trialdata = services.Trialbalance(enddate);
            trialdata.reporttypeid = 1;

            return View(trialdata);
        }

        public ActionResult CombineTrialBalance()
        {


            tbmodel.reporttypeid = 5;


            //tbmodel.childId = dt.Columns[0];
            //tbmodel.parentId = Convert.ToInt16(dt.Columns[1]);
            //tbmodel.childName = Convert.ToString(dt.Columns[2]);
            //tbmodel.level = Convert.ToInt16(dt.Columns[3]);
            //tbmodel.orderSequence = Convert.ToString(dt.Columns[4]);
            //tbmodel.DebitTotal = Convert.ToInt32(dt.Columns[5]);
            //tbmodel.CreditTotal = Convert.ToInt16(dt.Columns[6]);
            //tbmodel.ClosingBalance = Convert.ToInt16(dt.Columns[7]); 
            var totaldebit = db.VoucherMaster.Where(m => m.DrCrType == 2).ToList();


            for (int i = 0; i < totaldebit.Count; i++)
            {
                var ss = totaldebit[i].Partyid_Accountid;
                var accounttotal = totaldebit.Where(m => m.Partyid_Accountid == ss).Sum(m => m.VoucherFinalTotal);
            }

            return View("~/Views/Masters/TrialBalance.cshtml", tbmodel);
        }
        [HttpPost]
        public ActionResult CombineTrialBalance(DateTime enddate)
        {


            ////DataTable dt = treeservice.TrialBalance(); 
            //DataSet ds = treeservice.TrialBalance();
            //ViewBag.trialbalance = ds.Tables[0];

         
            //DataSet ds2 = treeservice.GetTrialBHierchal();
            //ViewBag.trialbalanceheirchal = ds2.Tables[0];
            //tbmodel.childId = dt.Columns[0];
            //tbmodel.parentId = Convert.ToInt16(dt.Columns[1]);
            //tbmodel.childName = Convert.ToString(dt.Columns[2]);
            //tbmodel.level = Convert.ToInt16(dt.Columns[3]);
            //tbmodel.orderSequence = Convert.ToString(dt.Columns[4]);
            //tbmodel.DebitTotal = Convert.ToInt32(dt.Columns[5]);
            //tbmodel.CreditTotal = Convert.ToInt16(dt.Columns[6]);
            //tbmodel.ClosingBalance = Convert.ToInt16(dt.Columns[7]); 
                
            ViewBag.accgruplist = treeservice.GetSubaccounts().Tables[0];

            var trialdata = services.Trialbalance(enddate,1);
            trialdata.accgrouplist = db.AccGroup.ToList();
            trialdata.reporttypeid = 5;


            return View("~/Views/Masters/TrialBalance.cshtml", trialdata);
        }

        public ActionResult PayablesReport()
        {
            try
            {


                trialbalanceviewmodel tbmodel = new trialbalanceviewmodel();
                tbmodel.reporttypeid = 3;
                //DataTable dt = treeservice.TrialBalance(); 
                var sundrydebitoraccount = db.Preferences.Where(m => m.PreferencetypeID == 5).FirstOrDefault().Preferencevalue;
                ViewBag.accounts = new SelectList(db.AccGroup.Where(m => m.Accunderprimarygroupid == sundrydebitoraccount), "AccgroupID", "Accgroupname");

                //tbmodel.childId = dt.Columns[0];
                //tbmodel.parentId = Convert.ToInt16(dt.Columns[1]);
                //tbmodel.childName = Convert.ToString(dt.Columns[2]);
                //tbmodel.level = Convert.ToInt16(dt.Columns[3]);
                //tbmodel.orderSequence = Convert.ToString(dt.Columns[4]);
                //tbmodel.DebitTotal = Convert.ToInt32(dt.Columns[5]);
                //tbmodel.CreditTotal = Convert.ToInt16(dt.Columns[6]);
                //tbmodel.ClosingBalance = Convert.ToInt16(dt.Columns[7]); 
                var totaldebit = db.VoucherMaster.Where(m => m.DrCrType == 2).ToList();


                for (int i = 0; i < totaldebit.Count; i++)
                {
                    var ss = totaldebit[i].Partyid_Accountid;
                    var accounttotal = totaldebit.Where(m => m.Partyid_Accountid == ss).Sum(m => m.VoucherFinalTotal);
                }

                return View("~/Views/Masters/TrialBalance.cshtml", tbmodel);
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }
        }

        [HttpPost]
        public ActionResult PayablesReport(int accountname, DateTime enddate)
        {

            try
            {


                //DataTable dt = treeservice.TrialBalance(); 
                var sundrydebitoraccount = db.Preferences.Where(m => m.PreferencetypeID == 5).FirstOrDefault().Preferencevalue;
                ViewBag.accounts = new SelectList(db.AccGroup.Where(m => m.Accunderprimarygroupid == sundrydebitoraccount), "AccgroupID", "Accgroupname");
                //tbmodel.childId = dt.Columns[0];
                //tbmodel.parentId = Convert.ToInt16(dt.Columns[1]);
                //tbmodel.childName = Convert.ToString(dt.Columns[2]);
                //tbmodel.level = Convert.ToInt16(dt.Columns[3]);
                //tbmodel.orderSequence = Convert.ToString(dt.Columns[4]);
                //tbmodel.DebitTotal = Convert.ToInt32(dt.Columns[5]);
                //tbmodel.CreditTotal = Convert.ToInt16(dt.Columns[6]);
                //tbmodel.ClosingBalance = Convert.ToInt16(dt.Columns[7]); 
                var totalaccounts = db.Account.Where(m => m.Accgroupid == accountname).ToList();
                tbmodel = services.payablereport(totalaccounts, enddate);
                tbmodel.reporttypeid = 3;


                return View("~/Views/Masters/TrialBalance.cshtml", tbmodel);

            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }
        }

        public ActionResult RecieveableReport()
        {
            try
            {


                trialbalanceviewmodel tbmodel = new trialbalanceviewmodel();
                tbmodel.reporttypeid = 4;
                //DataTable dt = treeservice.TrialBalance(); 
                var sundrydebitoraccount = db.Preferences.Where(m => m.PreferencetypeID == 5).FirstOrDefault().Preferencevalue;
                ViewBag.accounts = new SelectList(db.AccGroup.Where(m => m.Accunderprimarygroupid == sundrydebitoraccount), "AccgroupID", "Accgroupname");

                //tbmodel.childId = dt.Columns[0];
                //tbmodel.parentId = Convert.ToInt16(dt.Columns[1]);
                //tbmodel.childName = Convert.ToString(dt.Columns[2]);
                //tbmodel.level = Convert.ToInt16(dt.Columns[3]);
                //tbmodel.orderSequence = Convert.ToString(dt.Columns[4]);
                //tbmodel.DebitTotal = Convert.ToInt32(dt.Columns[5]);
                //tbmodel.CreditTotal = Convert.ToInt16(dt.Columns[6]);
                //tbmodel.ClosingBalance = Convert.ToInt16(dt.Columns[7]); 

                return View("~/Views/Masters/TrialBalance.cshtml", tbmodel);
            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }
        }

        [HttpPost]
        public ActionResult RecieveableReport(int accountname, DateTime enddate)
        {

            try
            {
                //DataTable dt = treeservice.TrialBalance(); 
                var sundrydebitoraccount = db.Preferences.Where(m => m.PreferencetypeID == 5).FirstOrDefault().Preferencevalue;
                ViewBag.accounts = new SelectList(db.AccGroup.Where(m => m.Accunderprimarygroupid == sundrydebitoraccount), "AccgroupID", "Accgroupname");
                //tbmodel.childId = dt.Columns[0];
                //tbmodel.parentId = Convert.ToInt16(dt.Columns[1]);
                //tbmodel.childName = Convert.ToString(dt.Columns[2]);
                //tbmodel.level = Convert.ToInt16(dt.Columns[3]);
                //tbmodel.orderSequence = Convert.ToString(dt.Columns[4]);
                //tbmodel.DebitTotal = Convert.ToInt32(dt.Columns[5]);
                //tbmodel.CreditTotal = Convert.ToInt16(dt.Columns[6]);
                //tbmodel.ClosingBalance = Convert.ToInt16(dt.Columns[7]); 
                var totalaccounts = db.Account.Where(m => m.Accgroupid == accountname).ToList();

                tbmodel = services.recieveablereport(totalaccounts, enddate);
                tbmodel.reporttypeid = 4;
                tbmodel.childName = db.AccGroup.Where(m => m.AccgroupID == accountname).FirstOrDefault().Accgroupname;
                tbmodel.enddate3 = enddate.ToShortDateString();

                return View("~/Views/Masters/TrialBalance.cshtml", tbmodel);


            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }
        }
        public ActionResult SalesRegister()
        {
            ViewBag.accounts = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.ItemLists = new SelectList(db.Items, "ItemID", "ItemName");
            ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName");
            ViewBag.VoucherTypes = new SelectList(db.VoucherType, "VoucherTypeID", "VoucherTypeName");
            reportmodel reportmodel = new reportmodel();
            reportmodel.vouchertypeid = 1;
            return View(reportmodel);


            
        }

        [HttpPost]
        public ActionResult SalesRegister(reportmodel reportmodel)
        {
            try
            {
                int drcrid = 0;
                VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();

                switch (reportmodel.vouchertypeid)
                {
                    case 1:
                        drcrid = 1;
                        break;
                    case 9:
                        drcrid = 2;
                        break;
                    case 2:
                        drcrid = 2;
                        break;
                    case 10:
                        drcrid = 1;
                        break;
                    case 12:
                        drcrid = 1;
                        break;
                    default:
                        drcrid = 0;
                        break;
                }

                vmmodel.Vouchermasterlist = db.VoucherMaster.Where(m => m.Partyid_Accountid == reportmodel.AccountID && m.VoucherCreateDate >= reportmodel.startdate
                   && m.VoucherCreateDate <= reportmodel.enddate && m.VoucherTypeID == reportmodel.vouchertypeid && m.DrCrType == drcrid && m.LocationID == reportmodel.LocationID).ToList();


                vmmodel.VoucherChildlist = new List<VoucherChild>();
                vmmodel.masterids = new List<int?>();
                vmmodel.vouchertypeid = 1;
                ViewBag.sdate = reportmodel.startdate.ToShortDateString();
                ViewBag.edate = reportmodel.enddate.ToShortDateString();
                ViewBag.location = db.MaterialCentre.Find(reportmodel.LocationID).MaterialCentreName;

                for (int i = 0; i < vmmodel.Vouchermasterlist.Count; i++)
                {
                    var vmmasterid = vmmodel.Vouchermasterlist[i].vouchermasterid;

                    var childlist = db.VoucherChild.Where(m => m.VoucherMasterID == vmmasterid && m.ItemID == reportmodel.ItemID).FirstOrDefault();
                    if (childlist != null)
                    {
                        vmmodel.masterids.Add(childlist.VoucherMasterID);
                        vmmodel.VoucherChildlist.Add(childlist);

                    }
                    else
                    {
                        vmmodel.masterids.Add(0);


                    }

                }


                return PartialView("~/Views/Shared/SPRegister.cshtml", vmmodel);


                //if (reportmodel.vouchertypeid == 1 || reportmodel.vouchertypeid == 9)
                //{
                //    VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();

                //    if (reportmodel.vouchertypeid == 1)
                //    {


                //    }
                //    else if (reportmodel.vouchertypeid == 9)
                //    {
                //        vmmodel.Vouchermasterlist = db.VoucherMaster.Where(m => m.Partyid_Accountid == reportmodel.AccountID && m.VoucherCreateDate >= reportmodel.startdate
                //    && m.VoucherCreateDate <= reportmodel.enddate && m.VoucherTypeID == 9 && m.DrCrType == 2 && m.LocationID == reportmodel.LocationID).ToList();

                //    }


                //    //List<VoucherChild> voucherchildlist = new List<VoucherChild>();
                //    vmmodel.VoucherChildlist = new List<VoucherChild>();
                //    vmmodel.masterids = new List<int?>();

                //    for (int i = 0; i < vmmodel.Vouchermasterlist.Count; i++)
                //    {
                //        var vmmasterid = vmmodel.Vouchermasterlist[i].vouchermasterid;

                //        var childlist = db.VoucherChild.Where(m => m.VoucherMasterID == vmmasterid && m.ItemID == reportmodel.ItemID).FirstOrDefault();
                //        if (childlist != null)
                //        {
                //            vmmodel.masterids.Add(childlist.VoucherMasterID);
                //            vmmodel.VoucherChildlist.Add(childlist);

                //        }
                //        else
                //        {
                //            vmmodel.masterids.Add(0);


                //        }

                //    }
                //    //ReportDocument rd = new ReportDocument();
                //    //rd.Load(Path.Combine(Server.MapPath("~/Views/Masters/Accledger.rpt")));
                //    //rd.SetDataSource(ds);
                //    //rd.SetParameterValue("@sdate", startdate);
                //    //rd.SetParameterValue("@edate", enddate); 
                //    //rd.SetParameterValue("@accountid", accountname);
                //    //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    //return File(stream, "application/pdf"); 

                //    //throw new Exception("abc");
                //    return PartialView("~/Views/Shared/SPRegister.cshtml", vmmodel);
                //}
                //else if (reportmodel.vouchertypeid == 2 || reportmodel.vouchertypeid == 10)
                //{
                //    VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();

                //    vmmodel.Vouchermasterlist = db.VoucherMaster.Where(m => m.Partyid_Accountid == reportmodel.AccountID && m.VoucherCreateDate >= reportmodel.startdate
                //    && m.VoucherCreateDate <= reportmodel.enddate && m.VoucherTypeID == 2 && m.DrCrType == 2 && m.LocationID == reportmodel.LocationID).ToList();



                //    //List<VoucherChild> voucherchildlist = new List<VoucherChild>();
                //    vmmodel.VoucherChildlist = new List<VoucherChild>();
                //    vmmodel.masterids = new List<int?>();
                //    vmmodel.vouchertypeid = 2;


                //    for (int i = 0; i < vmmodel.Vouchermasterlist.Count; i++)
                //    {
                //        var vmmasterid = vmmodel.Vouchermasterlist[i].vouchermasterid;

                //        var childlist = db.VoucherChild.Where(m => m.VoucherMasterID == vmmasterid && m.ItemID == reportmodel.ItemID).FirstOrDefault();
                //        if (childlist != null)
                //        {
                //            vmmodel.masterids.Add(childlist.VoucherMasterID);
                //            vmmodel.VoucherChildlist.Add(childlist);

                //        }
                //        else
                //        {
                //            vmmodel.masterids.Add(0);


                //        }

                //    }
                //    //ReportDocument rd = new ReportDocument();
                //    //rd.Load(Path.Combine(Server.MapPath("~/Views/Masters/Accledger.rpt")));
                //    //rd.SetDataSource(ds);
                //    //rd.SetParameterValue("@sdate", startdate);
                //    //rd.SetParameterValue("@edate", enddate); 
                //    //rd.SetParameterValue("@accountid", accountname);
                //    //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    //return File(stream, "application/pdf"); 

                //    //throw new Exception("abc");
                //    return PartialView("~/Views/Shared/SPRegister.cshtml", vmmodel);
                //}
                //else if (reportmodel.vouchertypeid == 12)
                //{
                //    VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();

                //    vmmodel.Vouchermasterlist = db.VoucherMaster.Where(m => m.Partyid_Accountid == reportmodel.AccountID && m.VoucherCreateDate >= reportmodel.startdate
                //    && m.VoucherCreateDate <= reportmodel.enddate && m.VoucherTypeID == 12 && m.DrCrType == 1 && m.LocationID == reportmodel.LocationID).ToList();



                //    //List<VoucherChild> voucherchildlist = new List<VoucherChild>();
                //    vmmodel.VoucherChildlist = new List<VoucherChild>();
                //    vmmodel.masterids = new List<int?>();
                //    vmmodel.vouchertypeid = 2;


                //    for (int i = 0; i < vmmodel.Vouchermasterlist.Count; i++)
                //    {
                //        var vmmasterid = vmmodel.Vouchermasterlist[i].vouchermasterid;

                //        var childlist = db.VoucherChild.Where(m => m.VoucherMasterID == vmmasterid && m.ItemID == reportmodel.ItemID).FirstOrDefault();
                //        if (childlist != null)
                //        {
                //            vmmodel.masterids.Add(childlist.VoucherMasterID);
                //            vmmodel.VoucherChildlist.Add(childlist);

                //        }
                //        else
                //        {
                //            vmmodel.masterids.Add(0);


                //        }

                //    }
                //    //ReportDocument rd = new ReportDocument();
                //    //rd.Load(Path.Combine(Server.MapPath("~/Views/Masters/Accledger.rpt")));
                //    //rd.SetDataSource(ds);
                //    //rd.SetParameterValue("@sdate", startdate);
                //    //rd.SetParameterValue("@edate", enddate); 
                //    //rd.SetParameterValue("@accountid", accountname);
                //    //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    //return File(stream, "application/pdf"); 

                //    //throw new Exception("abc");
                //    return PartialView("~/Views/Shared/SPRegister.cshtml", vmmodel);
                //}
                //else
                //{
                //    return PartialView("~/Views/Shared/Error.cshtml");

                //}

            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }

        }




        public ActionResult StockRegister()
        {
            ViewBag.accounts = new SelectList(db.Account, "AccountID", "AccountName");
            ViewBag.ItemLists = new SelectList(db.Items, "ItemID", "ItemName");
            ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName");
            reportmodel reportmodel = new reportmodel();
            reportmodel.vouchertypeid = 12;
            return View("~/Views/Masters/SalesRegister.cshtml", reportmodel);

        }
        [HttpPost]
        public ActionResult StockRegister(reportmodel reportmodel)
        {
            try
            {


                    VoucherMasterViewModel vmmodel = new VoucherMasterViewModel();

                    vmmodel.Vouchermasterlist = db.VoucherMaster.Where(m => m.Partyid_Accountid == reportmodel.AccountID && m.VoucherCreateDate >= reportmodel.startdate
                    && m.VoucherCreateDate <= reportmodel.enddate && m.LocationID == reportmodel.LocationID && m.VoucherTypeID == reportmodel.vouchertypeid).ToList();


                //List<VoucherChild> voucherchildlist = new List<VoucherChild>();
                
                    vmmodel.VoucherChildlist = new List<VoucherChild>();
                    vmmodel.masterids = new List<int?>();
                    vmmodel.vouchertypeid = 12;
                ViewBag.sdate = reportmodel.startdate.ToShortDateString();
                ViewBag.edate = reportmodel.enddate.ToShortDateString();

                for (int i = 0; i < vmmodel.Vouchermasterlist.Count; i++)
                    {
                        var vmmasterid = vmmodel.Vouchermasterlist[i].vouchermasterid;

                        var childlist = db.VoucherChild.Where(m => m.VoucherMasterID == vmmasterid && m.ItemID == reportmodel.ItemID).FirstOrDefault();
                        if (childlist != null)
                        {
                            vmmodel.masterids.Add(childlist.VoucherMasterID);
                            vmmodel.VoucherChildlist.Add(childlist);

                        }
                        else
                        {
                            vmmodel.masterids.Add(0);


                        }

                    }
                    //ReportDocument rd = new ReportDocument();
                    //rd.Load(Path.Combine(Server.MapPath("~/Views/Masters/Accledger.rpt")));
                    //rd.SetDataSource(ds);
                    //rd.SetParameterValue("@sdate", startdate);
                    //rd.SetParameterValue("@edate", enddate); 
                    //rd.SetParameterValue("@accountid", accountname);
                    //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //return File(stream, "application/pdf"); 

                    //throw new Exception("abc");
                    return PartialView("~/Views/Shared/SPRegister.cshtml", vmmodel);
              

            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }
        }
        public ActionResult accountbalance(int id)
        {

            var accountsgrouplist = db.AccGroup.Where(m => m.Accunderprimarygroupid == id).ToList();
            var accountslist = db.Account.Where(m => m.Accgroupid == id).ToList();

            tbmodel.accgrouplist = accountsgrouplist;
            tbmodel.accountlist = accountslist;

            return View(tbmodel);
        }

        public ActionResult getsubbacctrialB(int id)
        {
            var accountslist = db.Account.Where(m => m.Accgroupid == id).ToList();
            var accgrouplist = db.AccGroup.Where(m => m.Accunderprimarygroupid == id).ToList();
            tbmodel.accountlist = accountslist;
            tbmodel.accgrouplist = accgrouplist;

            return View(tbmodel);
        }

        public ActionResult Getprofitloss()
        {


            var finaccgroup = db.AccGroup.Where(m => m.AccgroupID == 24).FirstOrDefault();

            var findaccgroup2 = db.AccGroup.Where(m => m.Accunderprimarygroupid == finaccgroup.AccgroupID).ToList();

            var expenseaccounts = db.AccGroup.Where(m => m.AccgroupID == 1058).FirstOrDefault();
            var expenseaccounts2 = db.AccGroup.Where(m => m.AccgroupID == 1059).FirstOrDefault();
            var incomeaccounts = db.AccGroup.Where(m => m.AccgroupID == 1060).FirstOrDefault();
            var incomeaccounts2 = db.AccGroup.Where(m => m.AccgroupID == 1061).FirstOrDefault();

            var purchaseaccgrup = db.AccGroup.Where(m => m.AccgroupID == 1062).FirstOrDefault();
            var saleaccgroup = db.AccGroup.Where(m => m.AccgroupID == 1063).FirstOrDefault();

            proftlossmodel.expaccount = expenseaccounts;
            proftlossmodel.expaccount2 = expenseaccounts2;

            proftlossmodel.incomeaccount = incomeaccounts;
            proftlossmodel.incomeaccount2 = incomeaccounts2;

            proftlossmodel.salesaccount = saleaccgroup;
            proftlossmodel.purchaseaccount = purchaseaccgrup;








            //select aa.AccgroupID,aa.Accgroupname,ag.Accunderprimarygroupid,ag.Accgroupname,ac.Accgroupid,ac.AccountName from AccGroup aa

            //inner join AccGroup as ag
            //on ag.Accunderprimarygroupid = aa.AccgroupID

            //inner join Account as ac
            //on ac.Accgroupid = ag.AccgroupID

            //   union all


            //   select AccgroupID, Accgroupname, Accunderprimarygroupid, Accgroupname, AccgroupID, Accgroupname from AccGroup
            //    where     









            return View(proftlossmodel);
        }

        //------------------Show Accounts in tree------------------//
        public ActionResult ShowAccountsTree()
        {
            //var accountparentgrouplist = db.AccGroup.Where(m => m.Accunderprimarygroupid == 0).ToList();
            //  var accgrouplist = db.AccGroup.Where(m => m.Accunderprimarygroupid != 0).ToList();
            //  var acclistconsolidated = db.AccGroup.ToList();


            //  AccountGroupTreeViewModel viewmodel = new AccountGroupTreeViewModel();
            //  viewmodel.Accparentgrouplist = accountparentgrouplist;
            //  viewmodel.Accgrouplist = accgrouplist;
            //  viewmodel.Acclistconsolidated = acclistconsolidated;

            //  //for (int i = 0; i < accountparentgrouplist.Count; i++)
            //  //{
            //  //    services.Getaccgroupbyid(accgrouplist.FirstOrDefault().Accunderprimarygroupid);

            //  //}

            //  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  ViewBag.primarygroups = new SelectList(db.AccGroup.Where(a => a.Accunderprimarygroupid == 0), "AccgroupID", "Accgroupname");
            //  //db.AccGroup.Where(a=>a.Accunderprimarygroupid!=0).ToList();viewmodel



            //  int findaccparentgroup = 10;

            //  var accountparentgroup = db.AccGroup.Where(m => m.AccgroupID == findaccparentgroup && m.Accunderprimarygroupid == 0).FirstOrDefault();

            //  viewmodel.parentgroup = accountparentgroup.Accgroupname;



            //  //    var accountparentgroup = db.AccGroup.Where(m=> m.Accunderprimarygroupid == 0).ToList();
            //  //for (int i1 = 0; i1 < accountparentgroup.Count; i1++)
            //  //{
            //  //    viewmodel.parentgroup = accountparentgroup;
            //  //        int accountparentlist = accountparentgroup[i1].AccgroupID;


            //  var accountparentgroup2 = db.AccGroup.Where(m=>m.Accunderprimarygroupid == accountparentgroup.AccgroupID).ToList();



            //for (int i3 = 0; i3 < accountparentgroup2.Count; i3++)
            //{
            //      viewmodel.parentgroup2 = accountparentgroup2;
            //      int accountp2list = accountparentgroup2[i3].AccgroupID;


            //      var accountparentgroup3 = db.AccGroup.Where(m => m.Accunderprimarygroupid == accountp2list).ToList();

            //  for (int i2 = 0; i2 < accountparentgroup3.Count; i2++)
            //  {

            //    if (accountparentgroup3.Count != 0)
            //    {
            //       viewmodel.parentgroup3 = accountparentgroup3;

            //      for (int i = 0; i < accountparentgroup3.Count; i++)
            //      {

            //      int accid = accountparentgroup3[i].AccgroupID;

            //      var accountparentgroup33 = db.AccGroup.Where(m => m.Accunderprimarygroupid == accid).ToList();
            //      if (accountparentgroup33.Count != 0)
            //      {
            //          viewmodel.parentgroup33 = accountparentgroup33;

            //      }


            //      }
            //    }
            //  }
            //}


            //var accountparentgroup4 = db.AccGroup.Where(m => m.Accunderprimarygroupid == accountparentgroup3.AccgroupID).FirstOrDefault();
            //viewmodel.parentgroup4 = accountparentgroup4.Accgroupname;

            //var accountparentgroup5 = db.AccGroup.Where(m => m.Accunderprimarygroupid == accountparentgroup4.AccgroupID).FirstOrDefault();
            //viewmodel.parentgroup5 = accountparentgroup5.Accgroupname;

            //var accountparentgroup6 = db.AccGroup.Where(m => m.Accunderprimarygroupid == accountparentgroup5.AccgroupID).FirstOrDefault();
            treemodel tree = new treemodel();

            DataSet ds = treeservice.GetSubaccounts();

            ViewBag.treedata = ds.Tables[0];
            //viewmodel.parentgroup6 = accountparentgroup6.Accgroupname;


            //---------------GET SUB ACCOUNTS---------------//
            DataSet ds2 = treeservice.GetSubaccounts();
            ViewBag.subaccoutns = ds2.Tables[0];




            return View();

        }

        public ActionResult ShowmaterialsTree()
        {
            //arygroupid == accountparentgroup5.AccgroupID).FirstOrDefault();
            //treemodel tree = new treemodel();

            //DataSet ds = treeservice.GetSubaccounts();

            //ViewBag.treedata = ds.Tables[0];
            //viewmodel.parentgroup6 = accountparentgroup6.Accgroupname;


            //---------------GET SUB ACCOUNTS---------------//
            DataSet ds = treeservice.Getmaterialcentresherichal();
            ViewBag.materialcentres = ds.Tables[0];




            return View();

        }
        public ActionResult fgg()
        {


            return View();
        }

        #endregion Reports


    }
}