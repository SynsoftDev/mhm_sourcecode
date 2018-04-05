using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Models;
using MHMDal.Interfaces;

namespace MHMDal.Repository
{
    public class JobMasterRepo : IJobMaster
    {
        //MHM _context;

        public IEnumerable<JobMaster> GetJobMaster()
        {
            using (var _context = new MHM())
            {
                var JobMasters = _context.JobMasters.ToList();
                JobMasters.ForEach(r => r.JobPlansMsts = r.JobPlansMsts);
                return JobMasters;
            }
        }
    }
}
