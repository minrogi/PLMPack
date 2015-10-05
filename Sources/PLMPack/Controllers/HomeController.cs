using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PLMPack.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Contacts";
            return View();
        }
    }
}