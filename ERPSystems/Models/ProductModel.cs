﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERPSystem.Models
{
    public class ProductModel
    {
        [DisplayName("PRODUCT ID")]
        public int prod_Id { get; set; }

        [DisplayName("PRODUCT NAME")]
        [Required(ErrorMessage = "The Product Name field is REQUIRED")]
        public string prod_Name { get; set; }

        [DisplayName("DESCRIPTION")]
        [Required(ErrorMessage = "This Field is REQUIRED")]
        public string prod_Description { get; set; }

        [DisplayName("DATE")]
        public DateTime inDate { get; set; }

        [DisplayName("UPDATE DATE")]
        public DateTime upDate { get; set; }
     
        [DisplayName("CATEGORY")]
        [Required(ErrorMessage = "REQUIRED!! APPLY A CATEGORY")]
        public int prodCategoryID { get; set; }

        [DisplayName("SUPPLIER")]
        [Required(ErrorMessage = "REQUIRED!! SELECT A SUPPLIER")]
        public int supplierID { get; set; }

        [DisplayName("STATE")]
        public int isActive { get; set; }

        public ProductModel()
        {
            prod_Id = -1;
            prod_Name = "NO ITEM";
            prod_Description = "NO ITEM";
            inDate = DateTime.UtcNow;
            upDate = DateTime.UtcNow;
            prodCategoryID = 0;
            supplierID = 0;
            isActive = 0;
        }

        public ProductModel(int prod_Id, string prod_Name, string prod_Description, DateTime inDate, DateTime upDate, int prodCategoryID, int supplierID, int isActive)
        {
            this.prod_Id = prod_Id;
            this.prod_Name = prod_Name;
            this.prod_Description = prod_Description;
            this.inDate = inDate;
            this.upDate = upDate;
            this.prodCategoryID = prodCategoryID;
            this.supplierID = supplierID;
            this.isActive = isActive;
        }
    }
}