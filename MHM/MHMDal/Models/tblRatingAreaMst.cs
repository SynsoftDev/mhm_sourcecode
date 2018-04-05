namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblRatingAreaMst")]
    public partial class tblRatingAreaMst
    {
        public tblRatingAreaMst()
        {
            Cases = new HashSet<Case>();
            CSR_Rate_Mst = new HashSet<CSR_Rate_Mst>();
            tblRatingAreas = new HashSet<tblRatingArea>();
        }

        [Key]
        public long RatingAreaID { get; set; }

        [Required]
        [StringLength(30)]
        public string RatingAreaName { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
        [JsonIgnore]
        public virtual ICollection<Case> Cases { get; set; }
        [JsonIgnore]
        public virtual ICollection<CSR_Rate_Mst> CSR_Rate_Mst { get; set; }
        [JsonIgnore]
        public virtual ICollection<tblRatingArea> tblRatingAreas { get; set; }
    }
}
