using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHMDal.Models
{
    public class FamilyMemberUsesList
    {
        public int FamilyMemberNumber { get; set; }

        public List<BenefitDetails> BenefitUses { get; set; }

        //MHMCommonBenefitId, UsesAmountTotal
        //public Dictionary<int, long> BenefitUses { get; set; }
    }

    public class BenefitDetails
    {
        public long BenefitId { get; set; }

        public decimal UsageCost { get; set; }

        public int UsageQty { get; set; }
    }

}
