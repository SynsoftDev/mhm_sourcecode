namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MHMCommonBenefitsMst")]
    public partial class MHMCommonBenefitsMst
    {
        public MHMCommonBenefitsMst()
        {
            BenefitUserDetails = new HashSet<BenefitUserDetail>();
            MHMBenefitCostByAreaMsts = new HashSet<MHMBenefitCostByAreaMst>();
            MHMBenefitMappingMsts = new HashSet<MHMBenefitMappingMst>();
        }

        [Key]
        public long MHMBenefitID { get; set; }

        [StringLength(250)]
        public string MHMBenefitName { get; set; }

        public short? CategoryId { get; set; }

        public bool? IsDefault { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
        [JsonIgnore]
        public virtual ICollection<BenefitUserDetail> BenefitUserDetails { get; set; }
        [JsonIgnore]
        public virtual CategoryMst CategoryMst { get; set; }

        public virtual ICollection<MHMBenefitMappingMst> MHMBenefitMappingMsts { get; set; }
        [JsonIgnore]
        public virtual ICollection<MHMBenefitCostByAreaMst> MHMBenefitCostByAreaMsts { get; set; }

    }
}
