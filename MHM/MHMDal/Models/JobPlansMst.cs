namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("JobPlansMst")]
    public partial class JobPlansMst
    {
        public long Id { get; set; }

        [Required]
        [StringLength(13)]
        public string JobNumber { get; set; }

        [StringLength(20)]
        public string PlanId { get; set; }

        [StringLength(4)]
        public string BusinessYear { get; set; }

        public virtual JobMaster JobMaster { get; set; }

        public virtual PlanAttributeMst PlanAttributeMst { get; set; }
    }
}
