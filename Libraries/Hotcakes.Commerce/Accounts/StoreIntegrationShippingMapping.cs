#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public class StoreIntegrationShippingMapping
    {
        private const string COMMA = ",";

        public StoreIntegrationShippingMapping()
        {
            HccShippingMethodId = string.Empty;
            HccServiceCode = string.Empty;
            IntegrationShippingMethodId = string.Empty;
        }

        public string HccShippingMethodId { get; set; }
        public string HccServiceCode { get; set; }
        public string IntegrationShippingMethodId { get; set; }

        public string Serialize()
        {
            var output = string.Concat(HccShippingMethodId, COMMA, HccServiceCode, COMMA, IntegrationShippingMethodId);
            return output;
        }

        public bool Deserialize(string data)
        {
            var parts = data.Split(',');
            if (parts.Length > 2)
            {
                HccShippingMethodId = parts[0];
                HccServiceCode = parts[1];
                IntegrationShippingMethodId = parts[2];
                return true;
            }
            return false;
        }
    }
}