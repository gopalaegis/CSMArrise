//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BCInsight.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public Nullable<double> NoofLeave { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<System.TimeSpan> StartTIme { get; set; }
        public Nullable<System.TimeSpan> EndTIme { get; set; }
    }
}
