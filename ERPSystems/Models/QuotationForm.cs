using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class QuotationForm
    {
        public int QuoteId { get; set; }
        public string SuppName { get; set; }
        public string SuppPhone { get; set; }
        public string SuppCity { get; set; }
        public string SuppMunicipality { get; set; }
        public string SuppBarangay { get; set; }
        public string SuppZipcode { get; set; }
        public int QuoteTotalProduct { get; set; }
        public string QuoteStatus { get; set; }
        public DateTime QuoteCreated { get; set; }
        public double QuoteSubTotal { get; set; }
        public double QuoteDiscount { get; set; }
        public double QuoteTotal { get; set; }
        public int PurId { get; set; }

    }
}