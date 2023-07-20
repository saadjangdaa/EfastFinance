using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Web.Models.ViewModels
{
    public class treemodel
    {

        public Nullable<int> childId { get; set; }
        public Nullable<int> parentId { get; set; }
        public string childName { get; set; }
        public Nullable<int> level { get; set; }
        public string orderSequence { get; set; }
    }
}