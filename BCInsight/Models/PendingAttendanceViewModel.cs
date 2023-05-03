using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class PendingAttendanceViewModel
    {
        public int Id { get; set; }
        public string Users { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}