using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Represents the mini cart (pending order) shown on the cart page before checkout.
    /// </summary>
    [Serializable]
    public class MiniCartViewModel
    {
        /// <summary>
        ///     Set default values for each property.
        /// </summary>
        public MiniCartViewModel()
        {
            TotalQuantity = 0;
        }

        /// <summary>
        ///     The total cont of items currently in the cart / order.
        ///     To keep this lightweight we just return the number of items.
        ///     Upon hover of the mini cart it will load the items in the cart.
        /// </summary>
        public int TotalQuantity { get; set; }
    }
}