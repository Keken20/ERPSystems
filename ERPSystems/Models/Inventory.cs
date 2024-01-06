using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class Inventory
    {
        // Product details
        public int ProdId { get; set; }
        public string ProdName { get; set; }
        public string ProdDescription { get; set; }
        //public DateTime ProdCreatedAt { get; set; }

        // Inventory details
        public int ProdQOH { get; set; }
        public string ProdUnit { get; set; }
        public decimal ProdPriceUnit { get; set; }
        //public decimal ProdTotalPrice { get; set; }

        // Product category details
        //public int ProdcategoryId { get; set; }
        public string ProcategoryName { get; set; }
        //public string ProdcategoryDescription { get; set; }

        public Inventory()
        {
            ProdId = 0;
            ProdName = "N/A";
            ProdDescription = "N/A";
            //ProdCreatedAt = DateTime.Now;

            ProdQOH = 0;
            ProdUnit = "N/A";
            ProdPriceUnit = 0;
            //ProdTotalPrice = 0;

            //ProdcategoryId = 0;
            ProcategoryName = "N/A";
            //ProdcategoryDescription = "N/A";
        }

        public Inventory(int prodId, string prodName, string prodDescription/*, DateTime prodCreatedAt*/, int prodQOH, string prodUnit, decimal prodPriceUnit/*, decimal prodTotalPrice, int prodcategoryId*/, string procategoryName/*, string prodcategoryDescription*/)
        {
            ProdId = prodId;
            ProdName = prodName;
            ProdDescription = prodDescription;
            //ProdCreatedAt = prodCreatedAt;

            ProdQOH = prodQOH;
            ProdUnit = prodUnit;
            ProdPriceUnit = prodPriceUnit;
            ////ProdTotalPrice = prodTotalPrice;

            //ProdcategoryId = prodcategoryId;
            ProcategoryName = procategoryName;
            //ProdcategoryDescription = prodcategoryDescription;
        }
    }
}
