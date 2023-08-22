using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class DynamicMenuviewmodel
    {

        public class Menu_List
        {
            public  int ID { get; set; }
            public string MenuName { get; set; }
            public string Url { get; set; }
            public string Icon { get; set; }            
            public Nullable<int> ParentID { get; set; }
            public Nullable<bool> IsParent { get; set; }
            public Nullable<bool> IsActive { get; set; }
            public Nullable<System.DateTime> CreationDate { get; set; }

            public int? sort { get; set; }
        }

        
    }
}