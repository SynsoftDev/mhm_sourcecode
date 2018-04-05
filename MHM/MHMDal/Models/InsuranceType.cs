using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MHMDal.Models
{
    [Table("InsuranceType")]
    public partial class InsuranceType
    {
        public InsuranceType()
        {
            PlanAttributeMsts = new HashSet<PlanAttributeMst>();
        }

        public int InsuranceTypeId { get; set; }

        [Column("InsuranceType")]
        [Required]
        [StringLength(20)]
        public string InsuranceType1 { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        [JsonIgnore]
        public virtual ICollection<PlanAttributeMst> PlanAttributeMsts { get; set; }
    }
}
