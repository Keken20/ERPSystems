using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERPSystem.Models
{
    public class ProductModel
    {
        public int prod_Id { get; set; }
        [Required]
        public string prod_Name { get; set; }
        [Required]
        public string prod_Description { get; set; }
        public DateTime inDate { get; set; }
        public DateTime? upDate { get; set; }
        public int prodCategoryID { get; set; }
        public int? supplierID { get; set; }
        public int isActive { get; set; }

        public ProductModel()
        {
            prod_Id = -1;
            prod_Name = "NO ITEM";
            prod_Description = "NO ITEM";
            inDate = DateTime.Now;
            upDate = DateTime.Now;
            prodCategoryID = 0;
            supplierID = 0;
            isActive = 0;
        }

        public ProductModel(int prod_Id, string prod_Name, string prod_Description, DateTime inDate, DateTime? upDate, int prodCategoryID, int? supplierID, int isActive)
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