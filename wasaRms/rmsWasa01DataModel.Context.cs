﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace wasaRms.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class rmsWasa01Entities : DbContext
    {
        public rmsWasa01Entities()
            : base("name=rmsWasa01Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblCompany> tblCompanies { get; set; }
        public virtual DbSet<tblParameter> tblParameters { get; set; }
        public virtual DbSet<tblResource> tblResources { get; set; }
        public virtual DbSet<tblResourceType> tblResourceTypes { get; set; }
        public virtual DbSet<tblResourceTypeParameter> tblResourceTypeParameters { get; set; }
        public virtual DbSet<tblSheet> tblSheets { get; set; }
        public virtual DbSet<tblSubResource> tblSubResources { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
    }
}