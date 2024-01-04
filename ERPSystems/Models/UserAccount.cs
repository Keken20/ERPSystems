using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystem.Models
{
    public class UserAccount
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }
}