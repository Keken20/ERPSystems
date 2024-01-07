using ERPSystems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystem.Models
{
    public class ItemRequestModel
    {
        public List<RequestItem> reqItems { get; set; }
        public RequestItem item { get; set; }
    }
}