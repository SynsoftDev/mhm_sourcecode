using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Interfaces;
using MHMDal.Models;
using MHMDal.Repository;

namespace MHMBLL
{
    public class HSACalculation
    {
        MHMCache MHMCache = new MHMCache();
        IHSAFunding hsaFunding = new HSAFundingRepo();

        /// <summary>
        /// This method is used to calculate HSA
        /// </summary>
        /// <param name="UsageCode">Number of Members  in Family</param>
        /// <param name="PrimaryAge">Age of First Member</param>
        /// <param name="HSAPercentage">HSA Percentage</param>
        /// <returns>HSA</returns>
        public decimal CalculateAnnualHSA(int UsageCode, int PrimaryAge, decimal HSAPercentage, string BusinessYear, out decimal HSALimit)
        {
            decimal TotalHSA = 0;
            decimal ContributeLimit = 0;
            
            try
            {
                //Type = 1 for Usage Code 1 and Type = 2 for Usage Code more then 1 (means Single person them Type 1 and One than One Person Then Type 2)
                var HSAFundingFromCache = MHMCache.GetMyCachedItem("HSAFunding");
                if (HSAFundingFromCache != null)
                {
                    ContributeLimit = UsageCode > 1 ?
                        PrimaryAge >= Constants.HSAAgeLimit ? ((IEnumerable<HSAFunding>)HSAFundingFromCache).Where(r => r.Type == 2 && r.BusinessYear == BusinessYear).SingleOrDefault().AboveAgeContribute : ((IEnumerable<HSAFunding>)HSAFundingFromCache).Where(r => r.Type == 2 && r.BusinessYear == BusinessYear).SingleOrDefault().ContributeLimit :
                        PrimaryAge >= Constants.HSAAgeLimit ? ((IEnumerable<HSAFunding>)HSAFundingFromCache).Where(r => r.Type == 1 && r.BusinessYear == BusinessYear).SingleOrDefault().AboveAgeContribute : ((IEnumerable<HSAFunding>)HSAFundingFromCache).Where(r => r.Type == 1 && r.BusinessYear == BusinessYear).SingleOrDefault().ContributeLimit;
                }
                else
                {
                    var HSAFundingMasterList = hsaFunding.GetHSAFunding();
                    MHMCache.AddToMyCache("HSAFunding", HSAFundingMasterList, MyCachePriority.Default);
                    ContributeLimit = UsageCode > 1 ?
                          PrimaryAge >= Constants.HSAAgeLimit ? HSAFundingMasterList.Where(r => r.Type == 2 && r.BusinessYear == BusinessYear).SingleOrDefault().AboveAgeContribute : HSAFundingMasterList.Where(r => r.Type == 2 && r.BusinessYear == BusinessYear).SingleOrDefault().ContributeLimit :
                          PrimaryAge >= Constants.HSAAgeLimit ? HSAFundingMasterList.Where(r => r.Type == 1 && r.BusinessYear == BusinessYear).SingleOrDefault().AboveAgeContribute : HSAFundingMasterList.Where(r => r.Type == 1 && r.BusinessYear == BusinessYear).SingleOrDefault().ContributeLimit;
                }

                TotalHSA = (HSAPercentage / 100) * ContributeLimit;
                HSALimit = ContributeLimit;
                return TotalHSA;
            }
            catch (Exception ex)
            {
                throw new System.Exception("HSA limit is not defined. ");
            }

        }


        public decimal CalculateCompanyHSA(int UsageCode , decimal EmployeeHSA, JobMaster jobdetail )
        {
            bool isHSAMatch = Convert.ToBoolean(jobdetail.IsHSAMatch);
            decimal HSAMatchLimit1 = Convert.ToDecimal(jobdetail.HSAMatchLimit1);
            decimal HSAMatchRate1 = Convert.ToDecimal(jobdetail.HSAMatchRate1);
            decimal HSAMatchLimit2 = Convert.ToDecimal(jobdetail.HSAMatchLimit2);
            decimal HSAMatchRate2 = Convert.ToDecimal(jobdetail.HSAMatchRate2);
            decimal HSAMatchLimit3 = Convert.ToDecimal(jobdetail.HSAMatchLimit3);
            decimal HSAMatchRate3 = Convert.ToDecimal(jobdetail.HSAMatchRate3);
            decimal HSAMatchLimit4 = Convert.ToDecimal(jobdetail.HSAMatchLimit4);
            decimal HSAMatchRate4 = Convert.ToDecimal(jobdetail.HSAMatchRate4);
            decimal CompanyHSAMatch = 0;

            try
            {
                if (isHSAMatch)
                {
                    if (UsageCode == 1)
                    {
                        if (EmployeeHSA * HSAMatchRate1 > HSAMatchLimit1 * HSAMatchRate1)
                            CompanyHSAMatch = HSAMatchLimit1 * HSAMatchRate1;
                        else
                            CompanyHSAMatch = EmployeeHSA * HSAMatchRate1;
                    }
                    else if(UsageCode == 2)
                    {
                        if (EmployeeHSA * HSAMatchRate2 > HSAMatchLimit2 * HSAMatchRate2)
                            CompanyHSAMatch = HSAMatchLimit2 * HSAMatchRate2;
                        else
                            CompanyHSAMatch = EmployeeHSA * HSAMatchRate2;
                    }
                    else if (UsageCode == 3)
                    {
                        if (EmployeeHSA * HSAMatchRate3 > HSAMatchLimit3 * HSAMatchRate3)
                            CompanyHSAMatch = HSAMatchLimit3 * HSAMatchRate3;
                        else
                            CompanyHSAMatch = EmployeeHSA * HSAMatchRate3;
                    }
                    else 
                    {
                        if (EmployeeHSA * HSAMatchRate4 > HSAMatchLimit4 * HSAMatchRate4)
                            CompanyHSAMatch = HSAMatchLimit4 * HSAMatchRate4;
                        else
                            CompanyHSAMatch = EmployeeHSA * HSAMatchRate4;
                    }
                }

                return CompanyHSAMatch;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Company HSA limit is not defined. ");
            }

            

        }

    }
}
