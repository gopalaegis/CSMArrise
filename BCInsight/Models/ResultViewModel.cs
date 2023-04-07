using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace BCInsight.Models
{
    public class ResultViewModel
    {
        public int result { get; set; }
        public int resultActive { get; set; }
    }
}