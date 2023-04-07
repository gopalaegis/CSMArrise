using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class LeaveRequestViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int NoOfDays { get; set; }
        public string Reason { get; set; }
        public int? ApproveDays { get; set; }
        public int? ApprovedBy { get; set; }
        public bool? IsApproved { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? By { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}