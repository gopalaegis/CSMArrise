using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCInsight.DAL;
using System.ComponentModel.DataAnnotations;

namespace BCInsight.Models
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Department name is required")]
        public string Name { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        public int Employees { get; set; }
    }
}