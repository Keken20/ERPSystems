using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERPSystem.Models
{
    public class CategoryModel
    {
        [DisplayName("ID")]
        public int categID { get; set; }

        [Required]
        [DisplayName("CATEGORY")]
        public string CategoryName { get; set; }

        [DisplayName("STATE")]
        public int categ_IsActive { get; set; }

        public CategoryModel()
        {
            categID = -1;
            CategoryName = "UNCATEGORIZED";
            categ_IsActive = 0;
        }

        public CategoryModel(int categID, string categoryName, int categ_IsActive)
        {
            this.categID = categID;
            CategoryName = categoryName;
            this.categ_IsActive = categ_IsActive;
        }
    }
}