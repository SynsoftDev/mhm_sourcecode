using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Interfaces;
using MHMDal.Models;

namespace MHMDal.Repository
{
    public class RulesRepo : IRules
    {
        //MHM _context;

        public IEnumerable<Rule> GetRules()
        {
            using (var _context = new MHM())
            {
                var Rules = _context.Rules.ToList();
                return Rules;
            }
        }
    }
}
