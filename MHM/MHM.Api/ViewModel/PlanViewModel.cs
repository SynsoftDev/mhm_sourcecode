using MHMDal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHM.Api.ViewModel
{
    public class PlanViewModel : Response
    {
        public List<PlanMaster> Plans { get; set; }
    }

    public class PlanDetails
    {

        public string PlanId { get; set; }

        public string PlanMarketingName { get; set; }

        public long? Id { get; set; }

        public long? IssuerId { get; set; }

        public string IssuerName { get; set; }

        public List<string> BusinessYear { get; set; }
    }

    public class PlanDetailsCSR
    {

        public string PlanId { get; set; }

        public string PlanMarketingName { get; set; }

        public long? Id { get; set; }

        public long? IssuerId { get; set; }

        public string IssuerName { get; set; }

        public string BusinessYear { get; set; }
    }

    public class JobPlanDetails
    {
        public string JobNumber { get; set; }
        public List<long> SelectedPlanIds { get; set; }
    }

    public class JobPlanData
    {
        public long Id { get; set; }
        public string SelectedPlanTitle { get; set; }
    }

    public class BenefitsList
    {

        public long MHMBenefitID { get; set; }

        public string MHMBenefitName { get; set; }
    }

    public class IssuerssList
    {

        public long Id { get; set; }

        public string IssuerCode { get; set; }

        public string IssuerName { get; set; }
    }

    public class PlanBenefitList
    {
        public long Id { get; set; }
        public string PlanId { get; set; }
        public string BenefitName { get; set; }
        //public bool? IsCovered { get; set; }
        public string CostSharingType1 { get; set; }
        //public string CostSharingType2 { get; set; }
        public decimal? CopayInnTier1 { get; set; }
        public long? MHMBenefitId { get; set; }
        public decimal? CoinsInnTier1 { get; set; }
        public string BusinessYear { get; set; }
        public string Category { get; set; }
        //public int? Unassign { get; set; }
        //public long? IssuerId { get; set; }
        public string IssuerName { get; set; }
        public int TotalCount { get; set; }
        public int pagecount { get; set; }
        public string PlanMarketingName { get; set; }

        public int LimitQty { get; set; }
        public string LimitUnit { get; set; }
    }

    public class CSRRateMst
    {
        public long Id { get; set; }
        public string PlanId { get; set; }
        public long RatingAreaId { get; set; }
        public string Age { get; set; }
        public decimal? IndividualRate { get; set; }
        public decimal? GrpCostAmt { get; set; }
        public decimal? GrpEmplrPremAmt { get; set; }
        public decimal? GrpHSAAmt { get; set; }
        public string IssuerName { get; set; }
        public string PlanName { get; set; }
        public int TotalCount { get; set; }
        public string BusinessYear { get; internal set; }
    }

    public class PlanAttributeList
    {
        public long Id { get; set; }
        public string PlanId { get; set; }
        public string BusinessYear { get; set; }
        public long? CarrierId { get; set; }
        public string IssuerName { get; set; }
        public string MrktCover { get; set; }
        public string PlanType { get; set; }
        public string MetalLevel { get; set; }
        public bool? OpenForEnrollment { get; set; }
        public string StateCode { get; set; }
        public string ApprovalStatus { get; set; }
        public bool IsHSAEligible { get; set; }
        public decimal? TEHBDedInnTier1Individual { get; set; }
        public decimal? TEHBDedInnTier1FamilyPerPerson { get; set; }
        public decimal? TEHBInnTier1IndividualMOOP { get; set; }
        public decimal? TEHBInnTier1FamilyPerPersonMOOP { get; set; }
        public decimal? TEHBDedInnTier1FamilyPerGroup { get; set; }
        public decimal? TEHBInnTier1FamilyPerGroupMOOP { get; set; }
        public int UnassgnBen { get; set; }
        public int NoOfCases { get; set; }
        public string PlanMarketingName { get; set; }
        public string GroupName { get; set; }
        public int TotalCount { get; set; }
    }

}
