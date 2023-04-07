using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCInsight.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Newpassword { get; set; }
        public string Confirmpassword { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PushNotificationId { get; set; }
        public string DeviceId { get; set; }
        public int? ParentId { get; set; }
        public string Role { get; set; }
        public double? Salary { get; set; }
        public string SalaryDuration { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        public List<SiteViewModel> SiteData { get; set; }
        public string SiteName { get; set; }

        public string DepartmentName { get; set; }

        public int UserdptId { get; set; }
        public string DepartmentIds { get; set; }

        public string ManagerName { get; set; }

        public int ManagerId { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string WeekOffDay { get; set; }
    }
}
