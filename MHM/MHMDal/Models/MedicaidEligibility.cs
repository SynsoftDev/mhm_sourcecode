namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MedicaidEligibility")]
    public partial class MedicaidEligibility
    {
        public short Id { get; set; }

        public decimal? WithChildren { get; set; }

        public decimal? WithoutChildren { get; set; }

        [StringLength(2)]
        public string StateCode { get; set; }

        public virtual tblStateAbrev tblStateAbrev { get; set; }
    }
}
