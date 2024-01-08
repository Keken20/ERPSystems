using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class Account
    {
        public int UserID { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserFname { get; set; }
        public string UserMname { get; set; }
        public string UserLname { get; set; }
 
        public string Age { get; set; }
        public string UserPhone { get; set; }
        public string UserEmail { get; set; }
        public string UserType { get; set; }
    }
}