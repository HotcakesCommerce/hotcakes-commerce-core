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

namespace Hotcakes.Commerce.Reporting
{
    /// <summary>Describes a product found in abandoned carts and the related contact activity.</summary>
    public class AbandonedProduct
    {
        /// <summary>Gets or sets the product identifier.</summary>
        public Guid ProductGuid { get; set; }
        /// <summary>Gets or sets the product name used in the report.</summary>
        public string ProductName { get; set; }
        /// <summary>Gets or sets the total quantity of this product in the abandoned carts.</summary>
        public int Quantity { get; set; }
        /// <summary>Gets or sets the number of carts containing this product.</summary>
        public int CartsCount { get; set; }
        /// <summary>Gets or sets the number of contacts associated with the abandoned-cart activity.</summary>
        public int ContactsCount { get; set; }
    }
}
