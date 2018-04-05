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
    public class PlanSelectionRule : IRule
    {
        public void ExecuteRule(Dictionary<string, object> InObject, out Dictionary<string, object> OutObject)
        {
            try
            {
                List<CasePlanResult> TopPlans = new List<CasePlanResult>();
                List<CasePlanResult> PlanResults = (List<CasePlanResult>)InObject["PlanResults"];
                bool ResultStatus = (bool)InObject["ResultStatus"];
                if (ResultStatus)
                {
                    foreach (var item in PlanResults)
                    {
                        var MatchedPlan = TopPlans.Where(r => r.PlanIdIndiv1.Remove(r.PlanIdIndiv1.IndexOf('-')).ToString() == item.PlanIdIndiv1.Remove(r.PlanIdIndiv1.IndexOf('-')).ToString()).FirstOrDefault();
                        if (MatchedPlan != null)
                        {
                            var DefaultPlan = "0" + Constants.DefaultPlanNumber;
                            if (MatchedPlan.PlanIdIndiv1.Substring(MatchedPlan.PlanIdIndiv1.IndexOf('-') + 1).ToString() == DefaultPlan)
                            {
                                TopPlans.Remove(MatchedPlan);
                                TopPlans.Add(item);
                            }
                        }
                        else
                        {
                            TopPlans.Add(item);
                        }
                    }
                }
                else
                {
                    foreach (var item in PlanResults)
                    {
                        if (TopPlans.Count() <= 6)
                        {
                            var MatchedPlan = TopPlans.Where(r => r.PlanIdIndiv1.Remove(r.PlanIdIndiv1.IndexOf('-')).ToString() == item.PlanIdIndiv1.Remove(r.PlanIdIndiv1.IndexOf('-')).ToString()).FirstOrDefault();
                            if (MatchedPlan != null)
                            {
                                var DefaultPlan = "0" + Constants.DefaultPlanNumber;
                                if (MatchedPlan.PlanIdIndiv1.Substring(MatchedPlan.PlanIdIndiv1.IndexOf('-') + 1).ToString() == DefaultPlan)
                                {
                                    TopPlans.Remove(MatchedPlan);
                                    TopPlans.Add(item);
                                }
                            }
                            else
                            {
                                if (TopPlans.Count() < 4) { TopPlans.Add(item); } else { break; }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                OutObject = new Dictionary<string, object>();
                OutObject.Add("TopPlans", TopPlans);
            }
            catch (Exception ex)
            {
                throw new Exception("Plan Selection Error");
            }
        }
    }
}
