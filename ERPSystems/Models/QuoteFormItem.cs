using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class QuoteFormItem
    {
        public int PurID { get; set; }
        [Required]
        public string SupplierName { get; set; }
        [Required]
        public string SupplierPhone { get; set; }
        [Required]
        public string SupplierCity { get; set; }
        [Required]
        public string SupplierMunicipality { get; set; }
        [Required]
        public string SupplierBarangay { get; set; }
        [Required]
        public string SupplierZipcode { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}