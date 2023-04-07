using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class ClockInRequestViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }
        public bool Approved { get; set; }
        public int? ApprovedBy { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string UserMobile { get; set; }
        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
    }
}