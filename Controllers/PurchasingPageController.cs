using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPSystems.Controllers
{
    public class PurchasingPageController : Controller
    {
        // GET: PurchasingPage
        public ActionResult PurchasingDashboard()
        {
            return View();
        }
        public ActionResult PurchasingPageRequest()
        {
            return View();
        }
        public ActionResult PurchasingPageCanvass()
        {
            return View();
        }
        public ActionResult PurchasingPageQoutation()
        {
            return View();
        }
    }
}