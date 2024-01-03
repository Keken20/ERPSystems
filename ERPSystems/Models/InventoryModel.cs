using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERPSystem.Models
{
    public class InventoryModel
    {
        [Required]
        public int prod_Id { get; set; }
        [Required]
        public int inv_QOH { get; set; }
        [Required]
        public string ProdUnit { get; set; }
        [Required]
        public double ProdUnitPrice { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public double totalPrice { get; set; }
        public DateTime invt_InDate { get; set; }
        public DateTime invt_UpDate { get; set; }
        public int isActive { get; set; }

        public InventoryModel()
        {
            prod_Id = 0;
            inv_QOH = 0;
            ProdUnit = "NO ITEM";
            ProdUnitPrice = 0;
            totalPrice = 0.00;
            invt_InDate = DateTime.UtcNow;
            invt_UpDate = DateTime.UtcNow;
            isActive = 0;
        }

        public InventoryModel(int prod_Id, int inv_QOH, string prodUnit, double prodUnitPrice, double totalPrice, DateTime invt_InDate, DateTime invt_UpDate, int isActive)
        {
            this.prod_Id = prod_Id;
            this.inv_QOH = inv_QOH;
            ProdUnit = prodUnit;
            ProdUnitPrice = prodUnitPrice;
            this.totalPrice = totalPrice;
            this.invt_InDate = invt_InDate;
            this.invt_UpDate = invt_UpDate;
            this.isActive = isActive;
        }
    }
}