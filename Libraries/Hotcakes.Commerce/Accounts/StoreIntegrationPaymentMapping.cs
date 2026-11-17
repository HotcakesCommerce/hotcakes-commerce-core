#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
    /// <summary>Represents StoreIntegrationPaymentMapping data and behavior within the Hotcakes Commerce account and store boundary.</summary>
    [Serializable]
    public class StoreIntegrationPaymentMapping
    {
        private const string COMMA = ",";

        /// <summary>Performs the StoreIntegrationPaymentMapping operation.</summary>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public StoreIntegrationPaymentMapping()
        {
            HccPaymentMethodId = string.Empty;
            HccCardType = string.Empty;
            IntegrationPaymentMethodId = string.Empty;
        }

        /// <summary>Gets or sets HccPaymentMethodId.</summary>
        /// <remarks>The Hotcakes payment method identifier in the serialized mapping.</remarks>
        /// <value>The Hotcakes payment method identifier in the serialized mapping.</value>
        public string HccPaymentMethodId { get; set; }
        /// <summary>Gets or sets HccCardType.</summary>
        /// <remarks>The Hotcakes card type in the serialized mapping.</remarks>
        /// <value>The Hotcakes card type in the serialized mapping.</value>
        public string HccCardType { get; set; }
        /// <summary>Gets or sets IntegrationPaymentMethodId.</summary>
        /// <remarks>The external integration payment method identifier in the serialized mapping.</remarks>
        /// <value>The external integration payment method identifier in the serialized mapping.</value>
        public string IntegrationPaymentMethodId { get; set; }

        /// <summary>Serializes the payment mapping into its compatibility string format.</summary>
        /// <returns>A comma-separated string containing <see cref="HccPaymentMethodId"/>, <see cref="HccCardType"/>, and <see cref="IntegrationPaymentMethodId"/> in that order.</returns>
        /// <remarks>The comma delimiter is part of the persisted integration-settings contract.</remarks>
        public string Serialize()
        {
            var output = string.Concat(HccPaymentMethodId, COMMA, HccCardType, COMMA, IntegrationPaymentMethodId);
            return output;
        }

        /// <summary>Loads mapping values from the compatibility string format.</summary>
        /// <param name="data">A comma-separated value containing at least three fields.</param>
        /// <returns><see langword="true"/> when at least three fields were available and assigned; otherwise <see langword="false"/>.</returns>
        /// <remarks>Malformed input returns <see langword="false"/> without changing the mapping.</remarks>
        public bool Deserialize(string data)
        {
            var parts = data.Split(',');
            if (parts.Length > 2)
            {
                HccPaymentMethodId = parts[0];
                HccCardType = parts[1];
                IntegrationPaymentMethodId = parts[2];
                return true;
            }
            return false;
        }
    }
}
