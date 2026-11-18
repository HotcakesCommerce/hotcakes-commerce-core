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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Marketing
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of DiscountDetail in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is DiscountDetail.</remarks>
    [DataContract]
    [Serializable]
    public class DiscountDetailDTO
    {
        /// <summary>
        /// Initializes a discount detail with empty descriptive data and zero-valued numeric fields.
        /// </summary>
        public DiscountDetailDTO()
        {
            Id = new Guid();
            Description = string.Empty;
            Amount = 0;
            DiscountType = 0;
        }

        /// <summary>
        /// Gets or sets the unique identifier of the discount detail.
        /// </summary>
        /// <value>The identifier used to distinguish this discount detail.</value>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the human-readable description of the discount.
        /// </summary>
        /// <value>The discount description, or an empty string when no description is supplied.</value>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount applied to qualifying orders or line items.
        /// </summary>
        /// <value>The discount amount represented by this detail.</value>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the numeric value identifying the type of discount.
        /// </summary>
        /// <value>The serialized discount type value.</value>
        [DataMember]
        public int DiscountType { get; set; }
    }
}
