using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MerchantTribe.Commerce.Orders;

namespace Hotcakes.Modules.Core.Models
{
    public class EditLineItemViewModel
    {
        public LineItem Item { get; set; }
        public string OrderId { get; set; }
        public string SaveButtonUrl { get; set; }
    }
}