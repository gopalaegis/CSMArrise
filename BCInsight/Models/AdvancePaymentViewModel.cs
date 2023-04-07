using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace BCInsight.Models
{
    public class AdvancePaymentViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UName { get; set; }
        public string UEmail { get; set; }
        public string URole { get; set; }
        public string UMobile { get; set; }
        public double USalary { get; set; }
        public string USalaryDuration { get; set; }
        public double RequestAmount { get; set; }
        public string Reason { get; set; }
        public double? ApprovedAmount { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsApprove { get; set; }
    }
}