using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotcakes.Shipping.UpsFreight
{
    public enum HandlineOneUnitType
    {
        [Description("SKID")]
        SKD,
        [Description("CARBOY")]
        CBY,
        [Description("PALLET")]
        PLT,
        [Description("TOTES")]
        TOT
    }
}
