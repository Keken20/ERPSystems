using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class QuotationItem
    {
        public int QuoteId { get; set; }
        public int ProdId { get; set; }
        public int QuoteQouantity { get; set; }
        public string QuoteUnit { get; set; }
        public double QuotePricePerUnit { get; set; }
        public string ProdName { get; set; }
        public string ProdDescription { get; set; }
    }
}