using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHM.Api.ViewModel
{
    public class BenefitCostViewModel
    {
        [Required]
        public long RatingAreaID { get; set; }

        [Required]
        public string StateCode { get; set; }

        [Required]
        public long Createdby { get; set; }

        public List<BenefitsViewModel> Benefits { get; set; }
    }

    public class BenefitsViewModel
    {
        [Required]
        public long MHMBenefitId { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal? MHMBenefitCost { get; set; }

        // public int? MHMBenefitUnitOfMeasure { get; set; }
    }
}
