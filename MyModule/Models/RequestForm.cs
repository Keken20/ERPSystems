using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ERPSystems.Models
{
    public class RequestForm
    {
        [DisplayName("Request ID")]
        public int RequestId { get; set; }
        [DisplayName("Request Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [DisplayName("Total Item")]
        public int TotalItem { get; set; }
        [DisplayName("Requestor Name")]
        public string RequestorName{ get; set; }
        [DisplayName("Status")]
        public string Status{ get; set; }
        
        
    }
}