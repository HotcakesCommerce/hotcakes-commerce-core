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

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Represents the mini cart (pending order) shown on the cart page before checkout.
    /// </summary>
    [Serializable]
    public class MiniCartViewModel
    {
        /// <summary>
        ///     Set default values for each property.
        /// </summary>
        public MiniCartViewModel()
        {
            TotalQuantity = 0;
        }

        /// <summary>
        ///     The total cont of items currently in the cart / order.
        ///     To keep this lightweight we just return the number of items.
        ///     Upon hover of the mini cart it will load the items in the cart.
        /// </summary>
        public int TotalQuantity { get; set; }
    }
}