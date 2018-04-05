using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Models;
using MHMDal.Interfaces;
using System.Linq.Expressions;

namespace MHMDal.Repository
{
    public class ZipCodeMasterRepo : IZipCodeMaster
    {
        //MHM _context;

        public IEnumerable<ZipCodeMst> GetZipCodeMaster()
        {
            using (var _context = new MHM())
            {
                var ZipCodeMsts = _context.ZipCodeMsts.ToList();
                
                return ZipCodeMsts;
            }
        }
    }
}
