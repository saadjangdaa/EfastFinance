using RMS.Web.Models;
using RMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace RMS.Web.Controllers
{
    public class ItemController : Controller
    {
        itemServices services = new itemServices();
        ADOServices treeservice = new ADOServices();

        MVCDBLiveEntities db = new MVCDBLiveEntities();

        public ActionResult Index()
        {
            var getitems = services.Getitems();
            ViewBag.itemgroups = new SelectList(db.ItemGroup, "ItemGroupId", "ItemGroupName");

             ViewBag.itemunits = new SelectList(db.Units, "UnitID", "UnitName");
            return View(getitems);
        }
        public ActionResult Getitem(int itemid = 0)
        {
            if (itemid > 0)
            {
                Items items = new Items();

                var item = services.Getitemid(itemid);
                items.ItemID = item.ItemID;
                items.ItemName = item.ItemName;
                items.ItemPrice = item.ItemPrice;
                items.ItemGroupID = item.ItemGroupID;
                items.ItemUnitID = item.ItemUnitID;
                items.ItemMinDiscount = item.ItemMinDiscount;
                items.ItemMaxDiscount = item.ItemMaxDiscount;

                return Json(items, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("");
            }


        }


        [HttpPost]
        public ActionResult SaveData(Items itemform ,string Itemname, float Itemprice, int Itemid = 0)
        {

            if (Itemid > 0)
            {
                if (itemform.ItemID != 0)
                {
                    var itemid = services.Getitemid(itemform.ItemID);

                    itemid.ItemName = itemform.ItemName;
                    itemid.ItemPrice = itemform.ItemPrice;
                    itemid.ItemGroupID = itemform.ItemGroupID;
                    itemid.ItemUnitID = itemform.ItemUnitID;
                    itemid.ItemDescription = itemform.ItemDescription;
                    itemid.ItemMaxDiscount = itemform.ItemMaxDiscount;
                    itemid.ItemMinDiscount = itemform.ItemMinDiscount;

                    services.UpdateItems(itemid);



                }
            }
            else
            {
                if (Itemname != null)
                {
                   // Items items = new Items();
                    //items.ItemName = Itemname;  
                   // items.ItemPrice = Itemprice;

                    services.AddItems(itemform);

                    var stockcount = db.Stock.Count();
                    var olditemid = services.Getitemid(itemform.ItemID);
                    Stock addinstock = new Stock();
                    addinstock.ItemID = itemform.ItemID;
                    addinstock.Quantity = 0;
                    services.Additeminstock(addinstock);


                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Deleteitem(int itemid = 0)
        {
            if (itemid > 0)
            {
                var items = services.Getitemid(itemid);
                services.DeleteItems(items);

                var findstock = db.Stock.Find(itemid);
                db.Stock.Remove(findstock);
                db.SaveChanges();


                return RedirectToAction("Index");

            }
            return RedirectToAction("Index");


        }


        ////////////////////////////--------Item Groups------///////////// 

        public ActionResult ItemGroupIndex()
        {
            var itemlist = db.ItemGroup.ToList();
            ViewBag.itemgroups = new SelectList(db.ItemGroup, "ItemGroupId", "ItemGroupName", 0);



            return View(itemlist);
        }


        public ActionResult SaveItemgroup(int itemgrupid, string itemgrupname, string itemgrupalias, string itemgrupprimname, int? itemgrupprimid)
        {
            if (itemgrupid == 0 && itemgrupname != null)
            {
                ItemGroup acg = new ItemGroup();
                acg.ItemGroupName = itemgrupname;
                acg.ItemGroupAlias = itemgrupalias;
                acg.Itemundergroupname = itemgrupprimname;
                int? val = 0;
                if (itemgrupprimid == null)
                {
                    val = 0;
                }
                else
                {
                    val = itemgrupprimid;
                }
                acg.ItemUndergroupid = (int)val;

                var itemgroup = acg;
                services.AddItemgroups(itemgroup);
                return RedirectToAction("ItemGroupIndex");
            }
            else
            {
                if (itemgrupname != null && itemgrupid != 0)
                {

                    var olditemg = services.Getitemgroupbyid(itemgrupid);

                    olditemg.ItemGroupName = itemgrupname;
                    olditemg.ItemGroupAlias = itemgrupalias;
                    olditemg.Itemundergroupname = itemgrupprimname;
                    olditemg.ItemUndergroupid = (int)itemgrupprimid;
                    services.UpdateItemgroup(olditemg);
                    return RedirectToAction("ItemGroupIndex");

                }
                return RedirectToAction("ItemGroupIndex");


            }


        }

        public ActionResult Getitemgroup(int itemgid = 0)
        {
            if (itemgid > 0)
            {
                ItemGroup itemgroup = new ItemGroup();

                var olditemgrop = services.Getitemgroupbyid(itemgid);

                itemgroup.ItemGroupId = olditemgrop.ItemGroupId;
                itemgroup.ItemGroupName = olditemgrop.ItemGroupName;
                itemgroup.ItemGroupAlias = olditemgrop.ItemGroupAlias;
                itemgroup.ItemUndergroupid = olditemgrop.ItemUndergroupid;

                return Json(itemgroup, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("");
            }


        }

        public ActionResult Deleteitemgroup(int itemgroupid )
        {
            if (itemgroupid > 0)
            {
                var items = services.Getitemgroupbyid(itemgroupid);
                services.DeleteItemgroup(items);
                return RedirectToAction("ItemGroupIndex");

            }
            return RedirectToAction("ItemGroupIndex");


        }


        public ActionResult ItemTree()
        {
            //treeservice.Getitemstree
            DataSet ds = treeservice.Getitemstree();
            ViewBag.itemstreetable = ds.Tables[0];



            return View();
        }


        //public ActionResult ItemNew()
        //{
        //    var itemlist = db.Items1Set.ToList();
        //    ViewBag.itemgroups = new SelectList(db.ItemGroup, "ItemGroupId", "ItemGroupName");

        //    ViewBag.itemunits = new SelectList(db.Units, "UnitID", "UnitName");


        //    return View(itemlist);
        //}

        //public ActionResult Savenewitem(Items1 itemnewform)
        //{

        //    if (itemnewform.ItemID > 0)
        //    {
        //        var itemid = services.Getitemid(itemnewform.ItemID);
        //        if (itemid != null)
        //        {
        //            itemid.ItemName = itemnewform.ItemName;
        //            itemid.ItemPrice = itemnewform.ItemPrice;
        //            itemid.ItemGroupID = itemnewform.ItemGroupID;
        //            itemid.ItemUnitID = itemnewform.ItemUnitID;

        //            itemid.ItemMaxDiscount = itemnewform.ItemMaxDiscount;
        //            itemid.ItemMinDiscount = itemnewform.ItemMinDiscount;
        //            itemid.ItemDescription = itemnewform.ItemDescription;


        //            services.UpdateItems(itemid);


        //            return RedirectToAction("ItemNew");

        //        }
        //    }
        //    else
        //    {
        //        if (itemnewform.ItemName != null)
        //        {
        //            Items1 items2 = new Items1();
        //            items2.ItemName = itemnewform.ItemName;
        //            items2.ItemPrice = itemnewform.ItemPrice;
        //            items2.ItemGroupID = itemnewform.ItemGroupID;
        //            items2.ItemUnitID = itemnewform.ItemUnitID;

        //            items2.ItemMaxDiscount = itemnewform.ItemMaxDiscount;
        //            items2.ItemMinDiscount = itemnewform.ItemMinDiscount;
        //            items2.ItemDescription = itemnewform.ItemDescription;

        //            db.Items1Set.Add(items2);
        //            db.SaveChanges();


        //            //var olditemid = services.Getitemid(itemnewform.ItemID);
        //            //Stock addinstock = new Stock();
        //            //addinstock.ItemID = olditemid.ItemID;
        //            //addinstock.Quantity = 0;
        //            //services.Additeminstock(addinstock);


        //            return RedirectToAction("ItemNew");
        //        }
        //    }

        //    return RedirectToAction("ItemNew");


        //}
    }

}