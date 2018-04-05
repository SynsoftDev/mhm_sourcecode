using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Models;
using MHMDal.Interfaces;

namespace MHMDal.Repository
{
    public class FedPovertyLevelMasterRepo : IFedPovertyLevelMaster
    {
        //MHM _context;

        public IEnumerable<FedPovertyLevelMst> GetFedPovertyLevelMaster()
        {
            using (var _context = new MHM())
            {
                return _context.FedPovertyLevelMsts.ToList();
            }
        }
    }
}
