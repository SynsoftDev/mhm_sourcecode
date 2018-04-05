using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Models;
using MHMDal.Interfaces;
using MHMDal.Repository;

namespace MHMBLL
{
    public class ChipRule : IRule
    {
        MHMCache MHMCache = new MHMCache();
        IChipEligibility chipEligibility = new ChipEligibilityRepo();

        public void ExecuteRule(Dictionary<string, object> InObject, out Dictionary<string, object> OutObject)
        {
            try
            {
                List<FamilyMemberList> lstFamilyMembers; string StateCode; decimal MagiasPctFPL; string BusinessYear;
                IEnumerable<ChipEligibility> ChipEligibilityList;
                int MemberRemoveChipEligibility = 0;

                lstFamilyMembers = (List<FamilyMemberList>)InObject["lstFamilyMembers"];
                StateCode = InObject["StateCode"].ToString();
                MagiasPctFPL = (decimal)InObject["MagiasPctFPL"];
                BusinessYear = InObject["BusinessYear"].ToString();

                var ChipEligibilityFromCache = MHMCache.GetMyCachedItem("ChipEligibility");
                if (ChipEligibilityFromCache != null)
                {
                    ChipEligibilityList = (IEnumerable<ChipEligibility>)ChipEligibilityFromCache;
                }
                else
                {
                    ChipEligibilityList = chipEligibility.GetChipEligibility();
                    MHMCache.AddToMyCache("ChipEligibility", ChipEligibilityList, MyCachePriority.Default);
                }

                var childList = lstFamilyMembers.Where(r => r.Age <= 18).ToList();
                foreach (var item in childList)
                {
                    decimal? HighestPercentage = null;

                    if (item.Age <= 1)
                    {
                        HighestPercentage = ChipEligibilityList.Where(r => r.Age == "0-1" && r.StateCode == StateCode && r.Businessyear == BusinessYear).Max(t => t.FundPercent);
                    }
                    else if (item.Age > 1 && item.Age <= 5)
                    {
                        HighestPercentage = ChipEligibilityList.Where(r => r.Age == "1-5" && r.StateCode == StateCode && r.Businessyear == BusinessYear).Max(t => t.FundPercent);
                    }
                    else if (item.Age > 5 && item.Age <= 18)
                    {
                        HighestPercentage = ChipEligibilityList.Where(r => r.Age == "6-18" && r.StateCode == StateCode && r.Businessyear == BusinessYear).Max(t => t.FundPercent);
                    }

                    if ((MagiasPctFPL + Convert.ToDecimal(Constants.MagiasPctFPLPercentage * 100)) < HighestPercentage) { lstFamilyMembers.Remove(item); MemberRemoveChipEligibility = MemberRemoveChipEligibility + 1; }
                }

                OutObject = new Dictionary<string, object>();
                OutObject.Add("MemberCount", MemberRemoveChipEligibility);
            }
            catch (Exception ex)
            {

                throw new Exception("Chip Rule Error");
            }

        }

    }
}
