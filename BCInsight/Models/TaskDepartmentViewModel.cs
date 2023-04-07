using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class TaskDepartmentViewModel
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int DepartmentId { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }

        public bool IsForManager { get; set; }
    }
}