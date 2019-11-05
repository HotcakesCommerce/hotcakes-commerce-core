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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Contacts
{
    /// <summary>
    ///     This class is the main object that helps manage price groups in the REST API
    /// </summary>
    /// <remarks>The main application equivalent is PriceGroup.</remarks>
    [DataContract]
    [Serializable]
    public class PriceGroupDTO
    {
        public PriceGroupDTO()
        {
            StoreId = 0;
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            Name = string.Empty;
            PricingType = PricingTypesDTO.PercentageOffListPrice;
            AdjustmentAmount = 0m;
        }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     This is the ID of the price group.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the category was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The name is used primarily for administrative purposes to give a meaningful label to the created groups which makes
        ///     for easier management.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     The pricing type determines how the pricing is adjusted for customers.
        /// </summary>
        [DataMember]
        public PricingTypesDTO PricingType { get; set; }

        /// <summary>
        ///     Depending on the pricing type, this amount is used to adjust the pricing for the customer.
        /// </summary>
        [DataMember]
        public decimal AdjustmentAmount { get; set; }
    }
}