using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class AttendanceViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserMobile { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public string Remarks { get; set; }
        public string Reason { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string CheckType { get; set; }
        public string Description { get; set; }
        public bool IsMannualReq { get; set; }
        public bool IsApprove { get; set; }
    }
}