using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class QuoteForm
    {
        public int PurID { get; set; }
        public int QuoteID { get; set; }
        public DateTime PurDate { get; set; }
        public int TotalItem { get; set; }
        public string Status { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierCity { get; set; }
        public string SupplierMunicipality { get; set; }
        public string SupplierBarangay { get; set; }
        public string SupplierZipcode { get; set; }
        public decimal Discount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
    }
}