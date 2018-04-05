using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal;
using MHMDal.Interfaces;
using MHMDal.Repository;
using MHMDal.Models;

namespace MHMBLL
{
    public class BaseRateCal
    {
        ICSRRateMaster csrRateMaster = new CSRRateMasterRepo();
        MHMCache MHMCache = new MHMCache();

        /// <summary>
        /// This Method is used to calculate base amount according to perticular location and family members
        /// </summary>
        /// <param name="RatingAreaId">Rating Area Id</param>
        /// <param name="SecondLowestBaserate">Second Lowest BaseRate/Standard Componenet Id</param>
        /// <param name="lstFamilyMembers">Family List</param>
        /// <returns>Base Amount</returns>
        public decimal BaseRate(List<FamilyMemberList> lstFamilyMembers, IEnumerable<CSR_Rate_Mst> CSRRateList, int CalculationType)
        {
            decimal BaseSum = 0;

            try
            {
                //var CSR_Rate = CSRRateList.Where(r => r.PlanID == SecondLowestBaserate);
                var CSR_Rate = CSRRateList;
                //here we are finding plan by using PlanId if plan not found for prticulor RatingArea so BaseRate will be 0 and the plan will not be considre for other execution.
                if (CSR_Rate.Count() > 0)
                {
                    foreach (var item in lstFamilyMembers)
                    {
                        if (CalculationType == (int)EnumStatusModel.CalculationType.Premium)
                        {
                            if (item.SmokingStatus == true)
                            {
                                if (item.Age <= Constants.LowAgeLimit)
                                    BaseSum = BaseSum + Convert.ToDecimal(CSR_Rate.Where(r => r.Age == "0-20").FirstOrDefault().IndividualTobaccoRate);
                                else if (item.Age > Constants.AboveAgeLimit)
                                    BaseSum = BaseSum + Convert.ToDecimal(CSR_Rate.Where(r => r.Age == "65 and over").FirstOrDefault().IndividualTobaccoRate);
                                else
                                    BaseSum = BaseSum + Convert.ToDecimal(CSR_Rate.Where(r => r.Age == item.Age.ToString()).FirstOrDefault().IndividualTobaccoRate);
                            }
                            else
                            {
                                if (item.Age <= Constants.LowAgeLimit)
                                    BaseSum = BaseSum + Convert.ToDecimal(CSR_Rate.Where(r => r.Age == "0-20").FirstOrDefault().IndividualRate);
                                else if (item.Age > Constants.AboveAgeLimit)
                                    BaseSum = BaseSum + Convert.ToDecimal(CSR_Rate.Where(r => r.Age == "65 and over").FirstOrDefault().IndividualRate);
                                else
                                    BaseSum = BaseSum + Convert.ToDecimal(CSR_Rate.Where(r => r.Age == item.Age.ToString()).FirstOrDefault().IndividualRate);
                            }
                        }
                        else   //This is used when doing subsidy calculation becouse as client changes on 29-March-2016, He said -- The SUBSIDY should be calculated on the NON-SMOKING premium rate.
                        {
                            if (item.Age <= Constants.LowAgeLimit)
                                BaseSum = BaseSum + Convert.ToDecimal(CSR_Rate.Where(r => r.Age == "0-20").FirstOrDefault().EHBRate);
                            else if (item.Age > Constants.AboveAgeLimit)
                                BaseSum = BaseSum + Convert.ToDecimal(CSR_Rate.Where(r => r.Age == "65 and over").FirstOrDefault().EHBRate);
                            else
                                BaseSum = BaseSum + Convert.ToDecimal(CSR_Rate.Where(r => r.Age == item.Age.ToString()).FirstOrDefault().EHBRate);
                        }
                    }
                    return BaseSum;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to calculate premium due to an internal issue. Please contact your system administrator.");
            }

        }

        public decimal GroupPlanBaseRate(IEnumerable<CSR_Rate_Mst> CSRRateList, out decimal Subsidy, out decimal TotalERHSA)
        {
            decimal BaseSum = 0;

            try
            {
                var CSR_Rate = CSRRateList.FirstOrDefault();
                //here we are finding plan by using PlanId if plan not found for perticulor RatingArea so BaseRate will be 0 and the plan will not be considre for other execution.
                if (CSR_Rate != null)
                {
                    BaseSum = Convert.ToDecimal(CSR_Rate.GrpCostAmt);
                    Subsidy = Convert.ToDecimal(CSR_Rate.GrpEmplrPremAmt);
                    TotalERHSA = Convert.ToDecimal(CSR_Rate.GrpHSAAmt);
                    return BaseSum;
                }
                else
                {
                    Subsidy = 0;
                    TotalERHSA = 0;
                    return BaseSum;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to calculate premium due to an internal issue. Please contact your system administrator.");
            }

        }

    }
}
