using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class RequestModel
    {
        public List<RequestForm> requestform {get;set;}
        public List<RequestItem> requestItems { get; set; }
    }
}