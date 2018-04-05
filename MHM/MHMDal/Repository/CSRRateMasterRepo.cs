using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Interfaces;
using MHMDal.Models;

namespace MHMDal.Repository
{
    public class CSRRateMasterRepo : ICSRRateMaster
    {
        //MHM _context;

        public IEnumerable<CSR_Rate_Mst> GetCSRRateMaster()
        {
            using (var _context = new MHM())
            {
                //var ActiveIssuers = _context.IssuerMsts.Select(t => t.Id).ToList();
                //var IndividualPlanAttributes = _context.PlanAttributeMsts.Where(r => ActiveIssuers.Contains((long)r.CarrierId) && r.OpenForEnrollment == true && (r.MrktCover == "Indi" || r.MrktCover == "SHOP")).Select(t => t.StandardComponentId).ToList();
                //var GroupPlanAttributes = _context.PlanAttributeMsts.Where(r => ActiveIssuers.Contains((long)r.CarrierId) && r.MrktCover == "Group" && r.OpenForEnrollment == true).Select(t => t.PlanId).ToList();

                //var ActiveIssuers = _context.IssuerMsts.Select(t => t.Id).ToList();
                //var IndividualPlanAttributes = _context.PlanAttributeMsts.Where(r => (r.MrktCover == "Indi" || r.MrktCover == "SHOP")).Select(t => t.StandardComponentId).ToList();
                //var GroupPlanAttributes = _context.PlanAttributeMsts.Where(r => r.MrktCover == "Group").Select(t => t.PlanId).ToList();


                //var fullEntries = _context.CSR_Rate_Mst
                //.Join(
                //_context.PlanAttributeMsts.Where(r => r.MrktCover == "Indi"),
                //entryPoint => entryPoint.PlanID,
                //entry => entry.StandardComponentId,
                //(entryPoint, entry) => new { entryPoint, entry }
                //)
                //.Join(
                //_context.PlanAttributeMsts.Where(r => r.MrktCover == "GRP"),
                //entryPoint => entryPoint.entryPoint.PlanID,
                //entry1 => entry1.PlanId,
                //(entryPoint, entry1) => new { entryPoint, entry1 }
                //)
                //.Join(
                //_context.IssuerMsts,
                //combinedEntry => combinedEntry.entryPoint.entry.CarrierId,
                //Issuer => Issuer.Id,
                //(combinedEntry, Issuer) => new
                //{
                //    Issuers = Issuer,
                //    PlanAttributes = combinedEntry.entryPoint.entry,
                //    PlanAttributes1 = combinedEntry.entry1,
                //    CSRRateMst = combinedEntry.entryPoint
                //}
                //)
                //.Where(fullEntry => fullEntry.CSRRateMst).ToList();

                //var yey = _context.CSR_Rate_Mst.Where(r => IndividualPlanAttributes.Contains(r.PlanID) || GroupPlanAttributes.Contains(r.PlanID)).ToList();

                return _context.CSR_Rate_Mst.Include("tblRatingAreaMst").Include("tblStateAbrev").ToList();
            }
        }
    }
}
