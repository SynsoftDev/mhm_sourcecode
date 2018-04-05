namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlanMaster")]
    public partial class PlanMaster
    {
        public PlanMaster()
        {
            PlanAttributeMsts = new HashSet<PlanAttributeMst>();
        }

        [Key]
        public int PlanID { get; set; }

        [Required]
        [StringLength(50)]
        public string PlanType { get; set; }
        [JsonIgnore]
        public virtual ICollection<PlanAttributeMst> PlanAttributeMsts { get; set; }
    }
}
