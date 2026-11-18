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
using System.Collections.Generic;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Payment.Methods;

namespace Hotcakes.Modules.Core.Models.Json
{
/// <summary>
    /// Represents the CustomerDataJsonModel storefront model.
    /// </summary>
    [Serializable]
    public class CustomerDataJsonModel
    {
        /// <summary>
        /// Initializes a new instance of the CustomerDataJsonModel type.
        /// </summary>
        public CustomerDataJsonModel()
        {
            RewardPoints = new RewardPoints();
            Addresses = new List<Address>();
        }

        /// <summary>
        /// Gets or sets the RewardPoints value used by this storefront model.
        /// </summary>
        public RewardPoints RewardPoints { get; set; }
        /// <summary>
        /// Gets or sets the Addresses value used by this storefront model.
        /// </summary>
        public List<Address> Addresses { get; set; }
        /// <summary>
        /// Gets or sets the BillingAddress value used by this storefront model.
        /// </summary>
        public Address BillingAddress { get; set; }
        /// <summary>
        /// Gets or sets the ShippingAddress value used by this storefront model.
        /// </summary>
        public Address ShippingAddress { get; set; }
    }
}
