namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChipEligibility")]
    public partial class ChipEligibility
    {
        public short Id { get; set; }

        [StringLength(10)]
        public string Age { get; set; }

        [Required]
        [StringLength(200)]
        public string FundingType { get; set; }

        public decimal? FundPercent { get; set; }

        [StringLength(2)]
        public string StateCode { get; set; }

        [StringLength(4)]
        public string Businessyear { get; set; }

        public virtual tblStateAbrev tblStateAbrev { get; set; }
    }
}
