using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHMDal.Models
{
    public class PlanBenefits
    {
        public long BenefitId { get; set; }
        public decimal CoPay { get; set; }
        public decimal CoIns { get; set; }
        public string CopayPaymentFinal { get; set; }
        public string CostSharingType { get; set; }
    }
}
