﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class AttendanceReportViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ClockIn { get; set; }
    }
}