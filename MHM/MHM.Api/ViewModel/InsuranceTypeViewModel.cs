using MHMDal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHM.Api.ViewModel
{
    public class InsuranceTypeViewModel : Response
    {
        public dynamic InsuranceTypes { get; set; }
    }
}