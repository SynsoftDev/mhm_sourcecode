using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Models;
using MHMDal.Interfaces;

namespace MHMDal.Repository
{
    public class PlanAttributeMasterRepo : IPlanAttributeMaster
    {
        //MHM _context;

        public IEnumerable<PlanAttributeMst> GetPlanAttributeMaster()
        {
            using (var _context = new MHM())
            {
                //var ActiveIssuers = _context.IssuerMsts.Where(r => r.Status == true).Select(t => t.Id).ToList();
                var planAttributes = _context.PlanAttributeMsts.ToList();
                //.Include("InsuranceType1").Include("IssuerMst").Include("JobPlansMsts").Include("JobPlansMsts.JobMaster").Include("tblStateAbrev").Include("PlanMaster").Include("PlanBenefitMsts")
                //.Include("InsuranceType1").Include("IssuerMst").Include("JobPlansMsts").Include("JobPlansMsts.JobMaster").Include("tblStateAbrev").Include("PlanMaster").Include("PlanBenefitMsts")
                return planAttributes;//.Where(r => ActiveIssuers.Contains((long)r.CarrierId)).ToList();
            }
        }

        public IEnumerable<PlanAttributeMst> GetPlanAttributeMasterExcel()
        {
            using (var _context = new MHM())
            {
                var planAttributes = _context.PlanAttributeMsts.Include("InsuranceType1").Include("IssuerMst").Include("JobPlansMsts").Include("JobPlansMsts.JobMaster").Include("tblStateAbrev").Include("PlanMaster").Include("PlanBenefitMsts").ToList();
                return planAttributes;
            }
        }

    }
}
