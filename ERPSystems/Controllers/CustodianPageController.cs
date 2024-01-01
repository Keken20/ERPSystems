using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPSystems.Controllers
{
    public class CustodianPageController : Controller
    {
        // GET: CustodianPage
        public ActionResult CustodianDashboard()
        {
            return View();
        }
        public ActionResult CustodianPurchaseOrder()
        {
            return View();
        }
        public ActionResult CustodianInventory()
        {
            return View();
        }
    }
}