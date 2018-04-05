using MHMDal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MHM.Api.Helpers;
using SynSecurity;

namespace MHM.Api.Controllers
{
    public class HomeController : Controller
    {
        MHMDal.Models.MHM _context = new MHMDal.Models.MHM();

        public ActionResult Index()
        {
            //  var SetString = GenerateEncryptedString.GetEncryptedString("M");
            //  var GetString = GenerateEncryptedString.GetDecryptedString(SetString);

            //  var SetDate = GenerateEncryptedString.GetEncryptedString("12/1/2016 12:34:34 PM");
            //  var GetDate = GenerateEncryptedString.GetDecryptedString(SetDate);

            //var lstCases = _context.Cases.ToList();
            //foreach (var item in lstCases.Skip(4))
            //{
            //    foreach (var fa in item.Families)
            //    {

            //        var test = fa.Gender.Decrypt();
            //        fa.Gender = GenerateEncryptedString.GetEncryptedString(fa.Gender.Decrypt());
            //        fa.DOB = GenerateEncryptedString.GetEncryptedString(fa.DOB.Decrypt());
            //    }
            //}

            ViewBag.Title = "Home Page";
            return View();
        }

        public ActionResult SubsidyCalculation()
        {
            return View();
        }
        public ActionResult SubsidyCalculationUsingCache()
        {
            return View();
        }

        public ActionResult Demo()
        {
            return View();
        }

        public ActionResult DemoNew()
        {
            return View();
        }

        public ActionResult GroupPlan()
        {
            return View();
        }

        public ActionResult TestGraph()
        {
            ViewBag.MyCase = "Testing";
            return View();
        }

    }
}
