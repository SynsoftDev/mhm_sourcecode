using MHMDal.Interfaces;
using MHMDal.Models;
using MHMDal.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHMBLL
{
    public class MedicaidRule : IRule
    {
        MHMCache MHMCache = new MHMCache();
        IMedicaidEligibility medicaidEligibility = new MedicaidEligibilityRepo();

        public void ExecuteRule(Dictionary<string, object> InObject, out Dictionary<string, object> OutObject)
        {
            try
            {
                List<FamilyMemberList> lstFamilyMembers; string StateCode; decimal MagiasPctFPL; bool IsChild; string BusinessYear;
                lstFamilyMembers = (List<FamilyMemberList>)InObject["lstFamilyMembers"];
                StateCode = InObject["StateCode"].ToString();
                MagiasPctFPL = (decimal)InObject["MagiasPctFPL"];
                IsChild = (bool)InObject["IsChild"];
                BusinessYear = InObject["BusinessYear"].ToString();
                if (IsChild)
                {
                    decimal MedicaidPercentage = 0;
                    //Get MedicaidPercentage based on ChildStatus and State
                    var MedicaidEligibilityCache = MHMCache.GetMyCachedItem("MedicaidEligibility");
                    if (MedicaidEligibilityCache != null)
                    {
                        MedicaidPercentage = (decimal)((IEnumerable<MedicaidEligibility>)MedicaidEligibilityCache).Where(r => r.StateCode == StateCode).FirstOrDefault().WithChildren;
                    }
                    else
                    {
                        var MedicaidEligibilityList = medicaidEligibility.GetMedicaidEligibility();
                        MHMCache.AddToMyCache("MedicaidEligibility", MedicaidEligibilityList, MyCachePriority.Default);
                        MedicaidPercentage = (decimal)MedicaidEligibilityList.Where(r => r.StateCode == StateCode).FirstOrDefault().WithChildren;
                    }

                    int MemberRemoveMedicaidEligibility = 0;
                    var Medicaid = MedicaidPercentage * 100;
                    if (MedicaidPercentage != 0 && MagiasPctFPL < Medicaid)
                    {
                        MemberRemoveMedicaidEligibility = lstFamilyMembers.Where(r => r.Age > 18).Count();
                        lstFamilyMembers.RemoveAll(r => r.Age > 18);
                    }
                    OutObject = new Dictionary<string, object>();
                    OutObject.Add("StateFPL", Medicaid);
                    OutObject.Add("MemberCount", MemberRemoveMedicaidEligibility);
                }
                else
                {
                    decimal MedicaidPercentage = 0;
                    //Get MedicaidPercentage based on ChildStatus and State
                    var MedicaidEligibilityCache = MHMCache.GetMyCachedItem("MedicaidEligibility");
                    if (MedicaidEligibilityCache != null)
                    {
                        MedicaidPercentage = (decimal)((IEnumerable<MedicaidEligibility>)MedicaidEligibilityCache).Where(r => r.StateCode == StateCode).FirstOrDefault().WithoutChildren;
                    }
                    else
                    {
                        var MedicaidEligibilityList = medicaidEligibility.GetMedicaidEligibility();
                        MHMCache.AddToMyCache("MedicaidEligibility", MedicaidEligibilityList, MyCachePriority.Default);
                        MedicaidPercentage = (decimal)MedicaidEligibilityList.Where(r => r.StateCode == StateCode).FirstOrDefault().WithoutChildren;
                    }

                    int MemberRemoveMedicaidEligibility = 0;
                    var Medicaid = MedicaidPercentage * 100;
                    if (MedicaidPercentage != 0 && MagiasPctFPL < Medicaid)
                    {
                        MemberRemoveMedicaidEligibility = lstFamilyMembers.Where(r => r.Age > 18).Count();
                        lstFamilyMembers.RemoveAll(r => r.Age > 18);
                    }
                    OutObject = new Dictionary<string, object>();
                    OutObject.Add("StateFPL", Medicaid);
                    OutObject.Add("MemberCount", MemberRemoveMedicaidEligibility);
                }
            }
            catch (Exception)
            {
                
                throw new Exception("MedicaId Rule Error");
            }
            
        }

    }
}
