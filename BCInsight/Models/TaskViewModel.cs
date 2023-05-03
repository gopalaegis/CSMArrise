using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string DeptIds { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskRemarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeptName { get; set; }
        public int TaskDeptId { get; set; }
        public bool IsForManager { get; set; }
    }
}