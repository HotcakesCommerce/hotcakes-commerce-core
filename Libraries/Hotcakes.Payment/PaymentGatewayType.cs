using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotcakes.Payment
{
    public static class PaymentGatewayType
    {
        public const string AuthorizeNet = "828F3F70-EF01-4db6-A385-C5467CF91587";
        public const string BeanStream = "D14D5F35-F5CE-4C7E-9495-E9164EBDFF4E";
        private const string Ogone = "268F8127-760F-4D71-A4AA-39EA95DF35D5";
        private const string PayLead = "6FC76AD8-66BF-47b0-8982-1C4118F01645";
        public const string PayPalPro = "0B81046B-7A24-4512-8A6B-6C4C59D4C503";
        public const string Stripe = "15011DF5-13DA-42BE-9DFF-31C71ED64D4A";
        public const string Test = "FCACE46F-7B9C-4b49-82B6-426CF522C0C6";
    }
}
