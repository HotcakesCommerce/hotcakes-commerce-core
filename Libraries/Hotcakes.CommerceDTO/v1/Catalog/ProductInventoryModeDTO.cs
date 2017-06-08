#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This enumeration helps to define how a product will be displayed and managed when a product is out of stock.
    /// </summary>
    [DataContract]
    [Serializable]
    public enum ProductInventoryModeDTO
    {
        /// <summary>
        ///     Not specified - Should not be used!
        /// </summary>
        [EnumMember] NotSet = -1,

        /// <summary>
        ///     Not specified - Should not be used!
        /// </summary>
        [EnumMember] Unknown = 0,

        /// <summary>
        ///     This setting specifies that the product will always be allowed to be viewed and purchased, regardless of inventory
        ///     levels.
        /// </summary>
        /// <remarks>This is the default setting in the application.</remarks>
        [EnumMember] AlwayInStock = 100,

        /// <summary>
        ///     If the product is out of stock, it will no longer be available in the user interface and it cannot be purchased.
        /// </summary>
        [EnumMember] WhenOutOfStockHide = 101,

        /// <summary>
        ///     When this setting is used, the product can be seen when out of stock, but it cannot be purchased.
        /// </summary>
        [EnumMember] WhenOutOfStockShow = 102,

        /// <summary>
        ///     This setting will allow the product to be purchased when out of stock, but as a back order.
        /// </summary>
        [EnumMember] WhenOutOfStockAllowBackorders = 103
    }
}