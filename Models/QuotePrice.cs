using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class QuotePrice
    {
        public int PurId { get; set; }
        public int QuoteId { get; set; }
        public string ProdId { get; set; }
        public string QuoteUnit { get; set; }
        public int QuoteQuantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}