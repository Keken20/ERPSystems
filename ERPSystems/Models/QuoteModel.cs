using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPSystems.Models
{
    public class QuoteModel
    {
        public List<QuoteForm> quoteForms { get; set; }
        public List<QuoteItem> quoteItems { get; set; }
        public List<QuoteFormItem> quoteFormItems { get; set; }
        public List<QuotePrice> quotePrices { get; set; }
        public List<UpdateInventory> updateInventory { get; set; }
    }
}