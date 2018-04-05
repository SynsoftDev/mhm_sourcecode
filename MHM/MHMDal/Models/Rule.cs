using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MHMDal.Models
{
    public class Rule
    {
        public int Id { get; set; }
        public string RuleName { get; set; }
        public string ClassName { get; set; }
        public bool ChildStatus { get; set; }
        public bool RuleStatus { get; set; }
    }
}
