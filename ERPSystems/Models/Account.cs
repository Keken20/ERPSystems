using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ERPSystems.Models
{
    public class Account
    {
        public string _mname;
        public int AccId { get; set; }
        public string UserFullName { get; set; }
        [DisplayName("Username:")]
        public string AccUserName { get; set; }
        [DisplayName("Password:")]
               
        public string AccPassword { get; set; }
       
        [DisplayName("First Name:")]
        public string AccFname { get; set; }
        [DisplayName("Middle Name:")]
        public string AccMname
        {
            get { return _mname ?? DefaultValue(); }
            set { _mname = value; }
        }
        private string DefaultValue()
        {
            return " ";
        }
        [DisplayName("Last Name:")]
        public string AccLname { get; set; }
        public string AccType { get; set; }
        public int AccStatus { get; set; }
    }
}