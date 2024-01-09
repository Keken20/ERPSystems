using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPSystem.Models
{
    public class ViewModel
    {
        //Inventory ViewModel//
        [DisplayName("DATE")]
        public DateTime inv_InDate { get; set; }

        [DisplayName("PRODUCT ID")]
        public int prod_Id { get; set; }

        [DisplayName("NAME")]
        public string prod_Name { get; set; }

        [DisplayName("DESCRIPTION")]
        public string prod_Description { get; set; }

        [DisplayName("QOH")]
        public int inv_QOH { get; set; }

        [DisplayName("UNIT")]
        public string ProdUnit { get; set; }

        [DisplayName("PRICE UNIT")]
        public double ProdPriceUnit { get; set; }

        [DisplayName("TOTAL PRICE")]
        public double ProdTotalPrice { get; set; }

        [DisplayName("STATE")]
        public int isActive { get; set; }
        
        public class ProductViewModel
        {
            [DisplayName("PRODUCT ID")]
            public int prod_Id { get; set; }

            [DisplayName("NAME")]
            public string prod_Name { get; set; }

            [DisplayName("DESCRIPTION")]
            public string prod_Description { get; set; }

            [DisplayName("DATE")]
            public DateTime inDate { get; set; }

            [DisplayName("UPDATE")]
            public DateTime? upDate { get; set; }

            [DisplayName("CATEGORY")]
            public int prodCategoryID { get; set; }

            [DisplayName("SUPPLIER")]
            public int supplierID { get; set; }

            [DisplayName("SUPPLIER")]
            public string SuppName { get; set; }

            [DisplayName("CATEGORY")]
            public int categID { get; set; }

            [DisplayName("CATEGORY")]
            public string CategoryName { get; set; }

            [DisplayName("STATE")]
            public int isActive { get; set; }
        }
    }
}