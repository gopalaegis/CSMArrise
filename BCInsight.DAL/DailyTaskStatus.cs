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
    
    public partial class DailyTaskStatus
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public Nullable<bool> IsCompleted { get; set; }
        public Nullable<System.DateTime> CompletedOn { get; set; }
        public Nullable<double> Rating { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<int> RatingBy { get; set; }
        public Nullable<System.DateTime> RatingOn { get; set; }
        public string Comment { get; set; }
        public Nullable<int> CommentedBy { get; set; }
        public Nullable<System.DateTime> CommentedOn { get; set; }
    }
}
