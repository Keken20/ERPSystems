using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class QuotePrice
    {
        public int QuoteId { get; set; }
        public int PurId { get; set; }
        public string ProdId { get; set; }
        public string QuoteUnit { get; set; }
        public int QuoteQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalUnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}