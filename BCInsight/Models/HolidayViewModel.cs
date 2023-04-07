using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BCInsight.Models
{
    public class HolidayViewModel
    {
        public int Id { get; set; }
        public string DepartmentIds { get; set; }
        public int Year { get; set; }
        
        public string Date { get; set; }

        public DateTime? fdate { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        public string DeaprtmentName { get; set; }

       
    }
}