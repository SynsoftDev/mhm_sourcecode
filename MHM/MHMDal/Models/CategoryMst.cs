namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CategoryMst")]
    public partial class CategoryMst
    {
        public CategoryMst()
        {
            MHMCommonBenefitsMsts = new HashSet<MHMCommonBenefitsMst>();
        }

        [Key]
        public short CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }

        [JsonIgnore]
        public virtual ICollection<MHMCommonBenefitsMst> MHMCommonBenefitsMsts { get; set; }
    }
}
