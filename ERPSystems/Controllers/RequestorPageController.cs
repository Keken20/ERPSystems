using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPSystems.Controllers
{
    public class RequestorPageController : Controller
    {
        // GET: RequestorPage
        public ActionResult RequestorDashboard()
        {
            return View();
        }
        public ActionResult RequestorRequisition()
        {
            return View();
        }
        public ActionResult RequestorInventory()
        {
            return View();
        }
    }
}