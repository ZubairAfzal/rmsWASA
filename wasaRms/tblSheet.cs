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
    
    public partial class tblSheet
    {
        public long sheetID { get; set; }
        public System.DateTime sheetInsertionDateTime { get; set; }
        public int resourceID { get; set; }
        public int parameterID { get; set; }
        public double parameterValue { get; set; }
        public int companyID { get; set; }
    
        public virtual tblCompany tblCompany { get; set; }
        public virtual tblParameter tblParameter { get; set; }
        public virtual tblResource tblResource { get; set; }
    }
}
