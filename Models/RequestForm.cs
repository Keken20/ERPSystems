using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class RequestForm
    {
        public int RequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public int TotalItem { get; set; }
        public string RequestorName{ get; set; }
        public string Status{ get; set; }
    }
}