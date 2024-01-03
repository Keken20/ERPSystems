using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPSystem.Models
{
    public class ViewModel
    {
        //Inventory ViewModel//
        public DateTime inv_InDate { get; set; }
        public string prodCode { get; set; }
        public int prod_Id { get; set; }
        public string prod_Name { get; set; }
        public string prod_Description { get; set; }
        public int inv_QOH { get; set; }
        public string ProdUnit { get; set; }
        public double ProdPriceUnit { get; set; }
        public double ProdTotalPrice { get; set; }
        public int isActive { get; set; }
        
        public class ProductViewModel
        {
            public int prod_Id { get; set; }
            [Required]
            public string prod_Name { get; set; }
            [Required]
            public string prod_Description { get; set; }
            public DateTime inDate { get; set; }
            public DateTime? upDate { get; set; }
            public int prodCategoryID { get; set; }
            public int supplierID { get; set; }
            public string SuppName { get; set; }
            public int categID { get; set; }
            public string CategoryName { get; set; }
       
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public int isActive { get; set; }
        }
    }
}