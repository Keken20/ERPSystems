using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERPSystem.Models
{
    public class InventoryModel
    {
        [DisplayName("PRODUCT ID")]
        [Required(ErrorMessage = "REQUIRED!! SELECT A PRODUCT")]
        public int prod_Id { get; set; }

        [DisplayName("QUANTITY ON HAND")]
        public int? inv_QOH { get; set; }

        [DisplayName("UNIT")]
        [Required(ErrorMessage = "REQUIRED!! SPECIFY THE UNIT")]
        public string ProdUnit { get; set; }

        [DisplayName("PRICE UNIT")]
        public double? ProdUnitPrice { get; set; }

        [DisplayName("STATUS")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public double totalPrice { get; set; }

        [DisplayName("DATE")]
        public DateTime invt_InDate { get; set; }

        [DisplayName("UPDATED DATE")]
        public DateTime invt_UpDate { get; set; }

        [DisplayName("STATE")]
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