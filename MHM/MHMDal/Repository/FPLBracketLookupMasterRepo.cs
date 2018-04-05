using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Models;
using MHMDal.Interfaces;

namespace MHMDal.Repository
{
    public class FPLBracketLookupMasterRepo : IFPLBracketLookupMaster
    {
        //MHM _context;
        public IEnumerable<FPLBracketLookupMst> GetFPLBracketLookupMaster()
        {
            using (var _context = new MHM())
            {
                return _context.FPLBracketLookupMsts.ToList();
            }
        }
    }
}
