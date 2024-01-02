using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ERPSystem.Models
{
    public class Supplier
    {
        public int SuppID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SuppName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SuppContact { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SuppCity { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SuppMunicipality { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SuppBarangay { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SuppZipcode { get; set; }
        public DateTime SuppCreatedAt { get; set; }
        [Required(AllowEmptyStrings = true)]
        public DateTime SuppUpdatedAt { get; set; }
        public int SuppIsActive { get; set; }

        public Supplier()
        {
            SuppID = -1;
            SuppName = "N/A";
            SuppContact = "N/A";
            SuppCity = "N/A";
            SuppMunicipality = "N/A";
            SuppBarangay = "N/A";
            SuppZipcode = "N/A";
            SuppCreatedAt = DateTime.Now;
            SuppUpdatedAt = DateTime.Now;
            SuppIsActive = 0;
        }

        public Supplier(int supplierID, string supplierName, string supplierContact, string supplierCity, string supplierMunicipality, string supplierBarangay, string supplierZipcode, DateTime supplierCreatedAt, DateTime supplierUpdatedAt, int supplierIsActive)
        {
            SuppID = supplierID;
            SuppName = supplierName;
            SuppContact = supplierContact;
            SuppCity = supplierCity;
            SuppMunicipality = supplierMunicipality;
            SuppBarangay = supplierBarangay;
            SuppZipcode = supplierZipcode;
            SuppCreatedAt = supplierCreatedAt;
            SuppUpdatedAt = supplierUpdatedAt;
            SuppIsActive = supplierIsActive;
        }
    }
}