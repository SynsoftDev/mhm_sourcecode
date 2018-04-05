namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CriticalillnessMst")]
    public partial class CriticalillnessMst
    {
        public CriticalillnessMst()
        {
            Criticalillnesses = new HashSet<Criticalillness>();
        }

        [Key]
        public long IllnessId { get; set; }

        [Required]
        [StringLength(30)]
        public string IllnessName { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
        [JsonIgnore]
        public virtual ICollection<Criticalillness> Criticalillnesses { get; set; }
    }
}
