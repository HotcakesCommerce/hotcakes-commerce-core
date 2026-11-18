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
    /// <summary>Represents StoreIntegrationShippingMapping data and behavior within the Hotcakes Commerce account and store boundary.</summary>
    [Serializable]
    public class StoreIntegrationShippingMapping
    {
        private const string COMMA = ",";

        /// <summary>Performs the StoreIntegrationShippingMapping operation.</summary>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public StoreIntegrationShippingMapping()
        {
            HccShippingMethodId = string.Empty;
            HccServiceCode = string.Empty;
            IntegrationShippingMethodId = string.Empty;
        }

        /// <summary>Gets or sets HccShippingMethodId.</summary>
        /// <remarks>The Hotcakes shipping method identifier in the serialized mapping.</remarks>
        /// <value>The Hotcakes shipping method identifier in the serialized mapping.</value>
        public string HccShippingMethodId { get; set; }
        /// <summary>Gets or sets HccServiceCode.</summary>
        /// <remarks>The Hotcakes shipping service code in the serialized mapping.</remarks>
        /// <value>The Hotcakes shipping service code in the serialized mapping.</value>
        public string HccServiceCode { get; set; }
        /// <summary>Gets or sets IntegrationShippingMethodId.</summary>
        /// <remarks>The external integration shipping method identifier in the serialized mapping.</remarks>
        /// <value>The external integration shipping method identifier in the serialized mapping.</value>
        public string IntegrationShippingMethodId { get; set; }

        /// <summary>Serializes the shipping mapping into its compatibility string format.</summary>
        /// <returns>A comma-separated string containing <see cref="HccShippingMethodId"/>, <see cref="HccServiceCode"/>, and <see cref="IntegrationShippingMethodId"/> in that order.</returns>
        /// <remarks>The comma delimiter is part of the persisted integration-settings contract.</remarks>
        public string Serialize()
        {
            var output = string.Concat(HccShippingMethodId, COMMA, HccServiceCode, COMMA, IntegrationShippingMethodId);
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
                HccShippingMethodId = parts[0];
                HccServiceCode = parts[1];
                IntegrationShippingMethodId = parts[2];
                return true;
            }
            return false;
        }
    }
}
