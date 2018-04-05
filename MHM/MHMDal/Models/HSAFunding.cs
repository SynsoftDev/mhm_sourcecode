namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HSAFunding")]
    public partial class HSAFunding
    {
        public short Id { get; set; }

        public short Type { get; set; }

        public decimal Deductible { get; set; }

        //public decimal OOP { get; set; }

        public decimal ContributeLimit { get; set; }

        public decimal AboveAgeContribute { get; set; }

        public string BusinessYear { get; set; }

        public decimal HSAMOOPLimit { get; set; }

        public decimal QHPMOOPLimit { get; set; }
    }
}