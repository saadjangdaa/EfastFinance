﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class currencyEntities : DbContext
    {
        public currencyEntities()
            : base("name=currencyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<users> users { get; set; }
    
        public virtual ObjectResult<Getusers_Result> Getusers(Nullable<decimal> sn, string id, string pwd, string name, string company, string address, string phone, string cell, string email, Nullable<System.DateTime> sdate, Nullable<int> days, Nullable<int> multi, string block, Nullable<decimal> rate, Nullable<decimal> releif, string autoref, Nullable<byte> automonth, string newblock, Nullable<byte> package, Nullable<int> agentId, Nullable<int> agentUserId)
        {
            var snParameter = sn.HasValue ?
                new ObjectParameter("sn", sn) :
                new ObjectParameter("sn", typeof(decimal));
    
            var idParameter = id != null ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(string));
    
            var pwdParameter = pwd != null ?
                new ObjectParameter("pwd", pwd) :
                new ObjectParameter("pwd", typeof(string));
    
            var nameParameter = name != null ?
                new ObjectParameter("name", name) :
                new ObjectParameter("name", typeof(string));
    
            var companyParameter = company != null ?
                new ObjectParameter("company", company) :
                new ObjectParameter("company", typeof(string));
    
            var addressParameter = address != null ?
                new ObjectParameter("address", address) :
                new ObjectParameter("address", typeof(string));
    
            var phoneParameter = phone != null ?
                new ObjectParameter("phone", phone) :
                new ObjectParameter("phone", typeof(string));
    
            var cellParameter = cell != null ?
                new ObjectParameter("cell", cell) :
                new ObjectParameter("cell", typeof(string));
    
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            var sdateParameter = sdate.HasValue ?
                new ObjectParameter("sdate", sdate) :
                new ObjectParameter("sdate", typeof(System.DateTime));
    
            var daysParameter = days.HasValue ?
                new ObjectParameter("days", days) :
                new ObjectParameter("days", typeof(int));
    
            var multiParameter = multi.HasValue ?
                new ObjectParameter("multi", multi) :
                new ObjectParameter("multi", typeof(int));
    
            var blockParameter = block != null ?
                new ObjectParameter("block", block) :
                new ObjectParameter("block", typeof(string));
    
            var rateParameter = rate.HasValue ?
                new ObjectParameter("rate", rate) :
                new ObjectParameter("rate", typeof(decimal));
    
            var releifParameter = releif.HasValue ?
                new ObjectParameter("releif", releif) :
                new ObjectParameter("releif", typeof(decimal));
    
            var autorefParameter = autoref != null ?
                new ObjectParameter("autoref", autoref) :
                new ObjectParameter("autoref", typeof(string));
    
            var automonthParameter = automonth.HasValue ?
                new ObjectParameter("automonth", automonth) :
                new ObjectParameter("automonth", typeof(byte));
    
            var newblockParameter = newblock != null ?
                new ObjectParameter("newblock", newblock) :
                new ObjectParameter("newblock", typeof(string));
    
            var packageParameter = package.HasValue ?
                new ObjectParameter("package", package) :
                new ObjectParameter("package", typeof(byte));
    
            var agentIdParameter = agentId.HasValue ?
                new ObjectParameter("AgentId", agentId) :
                new ObjectParameter("AgentId", typeof(int));
    
            var agentUserIdParameter = agentUserId.HasValue ?
                new ObjectParameter("AgentUserId", agentUserId) :
                new ObjectParameter("AgentUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Getusers_Result>("Getusers", snParameter, idParameter, pwdParameter, nameParameter, companyParameter, addressParameter, phoneParameter, cellParameter, emailParameter, sdateParameter, daysParameter, multiParameter, blockParameter, rateParameter, releifParameter, autorefParameter, automonthParameter, newblockParameter, packageParameter, agentIdParameter, agentUserIdParameter);
        }
    }
}
