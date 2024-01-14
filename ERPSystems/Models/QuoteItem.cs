using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class QuoteItem
    {
        public int QuoteId { get; set; }
        public int PurId { get; set; }
        public int ProdId { get; set; }
        public string ProdName { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}