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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    /// Represents the catalog enum <see cref="CategorySortOrder" /> and its associated behavior.
    /// </summary>
    public enum CategorySortOrder
    {
    /// <summary>
    /// Serialized catalog value <c>None = 0</c>.
    /// </summary>
        None = 0,
    /// <summary>
    /// Serialized catalog value <c>ManualOrder = 1</c>.
    /// </summary>
        ManualOrder = 1,
    /// <summary>
    /// Serialized catalog value <c>ProductName = 2</c>.
    /// </summary>
        ProductName = 2,
    /// <summary>
    /// Serialized catalog value <c>ProductPriceAscending = 3</c>.
    /// </summary>
        ProductPriceAscending = 3,
    /// <summary>
    /// Serialized catalog value <c>ProductPriceDescending = 4</c>.
    /// </summary>
        ProductPriceDescending = 4,
    /// <summary>
    /// Serialized catalog value <c>ManufacturerName = 5</c>.
    /// </summary>
        ManufacturerName = 5,

    /// <summary>
    /// Serialized catalog value <c>ProductNameDescending = 6</c>.
    /// </summary>
        ProductNameDescending = 6,
    /// <summary>
    /// Serialized catalog value <c>ProductSKUAscending = 7</c>.
    /// </summary>
        ProductSKUAscending = 7,
    /// <summary>
    /// Serialized catalog value <c>ProductSKUDescending = 8</c>.
    /// </summary>
        ProductSKUDescending = 8
    }
}
