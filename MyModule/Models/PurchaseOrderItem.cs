using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class PurchaseOrderItem
    {
        public int PurId { get; set; }
        public int ProdId { get; set; }
        public string ProdName { get; set; }
        public string ProdDescription { get; set; }
        public int PurQuantity { get; set; }
        public string PurUnit { get; set; }  
    }
}