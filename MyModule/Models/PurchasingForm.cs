using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ERPSystems.Models
{
    public class PurchasingForm
    {
        public int PurId { get; set; }
        public int ReqId { get; set; }
        public int SuppId{ get; set; }
        public int PurTotalItem { get; set; }
        public string PurStatus { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PurCreated { get; set; }
        public string RequestFrom { get; set; }
    }
}