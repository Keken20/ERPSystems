using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystem.Models
{
    public class RequestItem
    {
        public int RequestId { get; set; }
        public int ProdId { get; set; }
        public string ProdName { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }

    }
}