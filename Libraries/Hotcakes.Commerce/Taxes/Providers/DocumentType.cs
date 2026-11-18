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

namespace Hotcakes.Commerce.Taxes.Providers
{
    /// <summary>
    /// Identifies the business document type used for a tax-provider operation.
    /// </summary>
    /// <remarks>
    /// The members intentionally retain their existing declaration order and implicit numeric values
    /// (0 through 5). Do not rename, reorder, or insert members because providers and serialized data
    /// may depend on these compatibility-sensitive values.
    /// </remarks>
    public enum DocumentType
    {
        /// <summary>Represents a sales order.</summary>
        SalesOrder,

        /// <summary>Represents a sales invoice.</summary>
        SalesInvoice,

        /// <summary>Represents a purchase order.</summary>
        PurchaseOrder,

        /// <summary>Represents a purchase invoice.</summary>
        PurchaseInvoice,

        /// <summary>Represents a return order.</summary>
        ReturnOrder,

        /// <summary>Represents a return invoice.</summary>
        ReturnInvoice
    }
}
