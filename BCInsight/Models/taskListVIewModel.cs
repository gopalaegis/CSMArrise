using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class taskListVIewModel
    {
        public int TaskId { get; set; }
        public int TaskDeptId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public int CompletedBy { get; set; }
        public DateTime? CompletedOn { get; set; }
        public int Rating { get; set; }
        public int RatedBy { get; set; }
        public DateTime RatedOn { get; set; }
        public string Comment { get; set; }
        public int CommentBy { get; set; }
        public DateTime CommentOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsForManager { get; set; }
    }
}