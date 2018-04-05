using MHMDal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHM.Api.ViewModel
{
    public class CategoryViewModel:Response
    {
        public List<CategoryMst> CategoryMst { get; set; }
    }
}
