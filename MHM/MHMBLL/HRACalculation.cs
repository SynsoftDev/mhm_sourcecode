using MHMDal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHMBLL
{
    public class HRACalculation
    {
        public decimal CalculateHRA(int UsagesCode, decimal DeductibleCosts, JobMaster JobDetail)
        {
            decimal? HRADedLimitPrimary = JobDetail.HRADedLimitPrimary == null ? 0 : JobDetail.HRADedLimitPrimary;
            decimal? HRAMaxReimbursePrimary = JobDetail.HRAMaxReimbursePrimary == null ? 0 : JobDetail.HRAMaxReimbursePrimary;
            decimal? HRAReimburseRatePrimary = JobDetail.HRAReimburseRatePrimary == null ? 0 : JobDetail.HRAReimburseRatePrimary;

            decimal? HRADedLimitDependent = JobDetail.HRADedLimitDependent == null ? 0 : JobDetail.HRADedLimitDependent;
            decimal? HRAMaxReimburseDependent = JobDetail.HRAMaxReimburseDependent == null ? 0 : JobDetail.HRAMaxReimburseDependent;
            decimal? HRAReimburseRateDependent = JobDetail.HRAReimburseRateDependent == null ? 0 : JobDetail.HRAReimburseRateDependent;

            //decimal? FamilyHRAReimbursementTotal = 0;
            decimal? HRAReimbursement = 0;


            if (UsagesCode == 1)
            {
                if ((DeductibleCosts - HRADedLimitPrimary) < 0)
                {
                    return (decimal)HRAReimbursement;
                }


                if (HRAMaxReimbursePrimary > 0)
                {
                    if ((DeductibleCosts - HRADedLimitPrimary) * HRAReimburseRatePrimary < HRAMaxReimbursePrimary)
                    {
                        HRAReimbursement = (DeductibleCosts - HRADedLimitPrimary) * HRAReimburseRatePrimary;
                    }
                    else
                    {
                        HRAReimbursement = HRAMaxReimbursePrimary;
                    }
                }
                return (decimal)HRAReimbursement;
            }
            else
            {
                if (HRAMaxReimburseDependent > 0)
                {
                    if ((DeductibleCosts - HRADedLimitDependent) < 0)
                    {
                        return (decimal)HRAReimbursement;
                    }

                    if ((DeductibleCosts - HRADedLimitDependent) * HRAReimburseRateDependent < HRAMaxReimburseDependent)
                    {
                        HRAReimbursement = (DeductibleCosts - HRADedLimitDependent) * HRAReimburseRateDependent;
                    }
                    else
                    {
                        HRAReimbursement = HRAMaxReimburseDependent;
                    }
                }
                else
                {
                    if ((DeductibleCosts - HRADedLimitPrimary) < 0)
                    {
                        return (decimal)HRAReimbursement;
                    }

                    if (HRAMaxReimbursePrimary > 0)
                    {
                        if ((DeductibleCosts - HRADedLimitPrimary) * HRAReimburseRatePrimary < HRAMaxReimbursePrimary)
                        {
                            HRAReimbursement = (DeductibleCosts - HRADedLimitPrimary) * HRAReimburseRatePrimary;
                        }
                        else
                        {
                            HRAReimbursement = HRAMaxReimbursePrimary;
                        }
                    }
                }
                return (decimal)HRAReimbursement;
            }
        }
    }
}
