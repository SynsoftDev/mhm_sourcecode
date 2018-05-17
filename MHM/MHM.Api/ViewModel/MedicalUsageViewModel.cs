using MHMDal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHM.Api.ViewModel
{
    public class MedicalUsageViewModel : Response
    {
        public List<MedicalUsagelistViewModel> MedicalsUsage { get; set; }

        public long RatingAreaId { get; set; }
    }

    public class MedicalUsagelistViewModel
    {
        public short CategoryId { get; set; }

        public string CategoryName { get; set; }

        public long MHMBenefitID { get; set; }

        public string MHMBenefitName { get; set; }

        public bool? IsDefault { get; set; }

        public decimal? MHMBenefitCost { get; set; }

        //public int CategoryDisplayOrder { get; set; }
        //public int BenefitDisplayOrder { get; set; }

    }
}
