//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RMS.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BillSundry
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BillSundry()
        {
            this.BillSundryChild = new HashSet<BillSundryChild>();
        }
    
        public int BiilSundryID { get; set; }
        public string BillSundryName { get; set; }
        public double BillSundryPrice { get; set; }
        public string BillSundryShortName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BillSundryChild> BillSundryChild { get; set; }
    }
}
