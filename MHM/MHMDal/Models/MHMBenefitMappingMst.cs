namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MHMBenefitMappingMst")]
    public partial class MHMBenefitMappingMst
    {
        public long Id { get; set; }

        public long MHMCommonBenefitID { get; set; }

        public long? IssuerID { get; set; }

        public int? IssuerBenefitID { get; set; }

        [StringLength(10)]
        public string IssuerBenefitVersion { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
        [JsonIgnore]
        public virtual IssuerMst IssuerMst { get; set; }
        [JsonIgnore]
        public virtual MHMCommonBenefitsMst MHMCommonBenefitsMst { get; set; }
    }
}
