using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class SiteViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Site Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Latitude is required")]
        public double? Latitude { get; set; }
        [Required(ErrorMessage = "Longitude is required")]
        public double? Longitude { get; set; }
        public bool IsClosed { get; set; } = false;
        public DateTime? ClosedOn { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}