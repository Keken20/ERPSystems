using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ERPSystems.Models
{
    public class Supplier
    {
        [DisplayName("Supplier ID")]
        public int SuppID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Name")]
        public string SuppName { get; set; }

        //[RegularExpression(@"^\d+(?:-\d+)*$|^\d{12}$", ErrorMessage = "Invalid format.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Contact")]
        public string SuppContact { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Address")]
        public string SuppCity { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Municipality")]
        public string SuppMunicipality { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Barangay")]
        public string SuppBarangay { get; set; }

        //[RegularExpression(@"^\d{4}$", ErrorMessage = "Invalid format.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Zip Code")]
        public string SuppZipcode { get; set; }

        [DisplayName("Created At")]
        public DateTime SuppCreatedAt { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayName("Updated At")]
        public DateTime SuppUpdatedAt { get; set; }

        [DisplayName("Status")]
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