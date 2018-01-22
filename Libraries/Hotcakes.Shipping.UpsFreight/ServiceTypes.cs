using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotcakes.Shipping.UpsFreight
{
    public enum ServiceTypes
    {
        [Description("UPS Freight LTL")]
        UPSFreightLTL = 308,
        [Description("UPS Freight LTL Guaranteed")]
        UPSFreightLTLGuaranteed = 309,
        [Description("UPS Freight LTL Guaranteed AM")]
        UPSFreightLTLGuaranteedAM = 334,
        [Description("UPS Standard LTL")]
        UPSStandardLTL = 339
    }
}
