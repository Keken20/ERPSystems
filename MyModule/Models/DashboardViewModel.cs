using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystem.Models
{
    public class DashboardViewModel
    {
        public UserAccount User { get; set; }

        public int inventoryCount { get; set; }
        public int supplierCount { get; set; }
        public int accountCount { get; set; }
        public int requisitionCount { get; set; }
        public int purchaseOrderCount { get; set; }

    }
}