//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace wasaRms
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblUser
    {
        public int userID { get; set; }
        public string userFullName { get; set; }
        public string userLoginName { get; set; }
        public string userPassword { get; set; }
        public string userEmail { get; set; }
        public Nullable<bool> userConfirmEmail { get; set; }
        public int companyID { get; set; }
    
        public virtual tblCompany tblCompany { get; set; }
    }
}
