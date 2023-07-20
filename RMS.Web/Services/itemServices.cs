using RMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Services
{
    public class itemServices
    {
        MVCDBLiveEntities db = new MVCDBLiveEntities();

       public List<Items> Getitems()
        {
            return db.Items.ToList();
           //.ToList();

        }
        public Items Getitemid(int itemid)
        {
           return db.Items.Find(itemid);
        }

        public void AddItems(Items item)
        {
            
            db.Items.Add(item);
            db.SaveChanges();
        }
        public void UpdateItems(Items item1)
        {
            
            db.Entry(item1).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        public void DeleteItems(Items item)
        {
            db.Items.Remove(item);
            db.SaveChanges();
        }


        public void Additeminstock(Stock stock)
        {
            db.Stock.Add(stock);
            db.SaveChanges();

        }
        ///---------------new itemgroup-----------------------////
        public ItemGroup Getitemgroupbyid(int itemid)
        {
            return db.ItemGroup.Find(itemid);
        }

        public void AddItemgroups(ItemGroup ItemGroup1)
        {

            db.ItemGroup.Add(ItemGroup1);
            db.SaveChanges();
        }
        public void UpdateItemgroup(ItemGroup itemgroup)
        {

            db.Entry(itemgroup).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        public void DeleteItemgroup(ItemGroup ItemGroup1)
        {
            db.ItemGroup.Remove(ItemGroup1);
            db.SaveChanges();
        }


    }
}