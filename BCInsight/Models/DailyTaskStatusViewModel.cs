using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class DailyTaskStatusViewModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? CompletedOn { get; set; }
        public double? Rating { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int? RatingBy { get; set; }
        public DateTime? RatingOn { get; set; }
        public string Comment { get; set; }
        public int? CommentedBy { get; set; }
        public DateTime? CommentedOn { get; set; }
    }
}