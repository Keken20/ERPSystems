using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystem.Models
{
    public class RequestModel
    {
        public List<RequestForm> requestform {get;set;}
        public List<RequestItem> requestItems { get; set; }
    }


}