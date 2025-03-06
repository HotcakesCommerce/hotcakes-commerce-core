#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2020-2025 Upendo Ventures, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

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
