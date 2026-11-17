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
    /// <summary>
    ///     This method is used to store API key information for
    ///     specific store. Represents ApiKey data and behavior within the Hotcakes Commerce account and store boundary.
    /// </summary>
    [Serializable]
    public class ApiKey
    {
        /// <summary>Performs the ApiKey operation.</summary>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public ApiKey()
        {
            Id = -1;
            StoreId = -1;
            Key = string.Empty;
        }

        /// <summary>Gets or sets Id.</summary>
        /// <remarks>The database identifier for this record; a newly constructed model uses its established unsaved sentinel value unless otherwise noted.</remarks>
        /// <value>The database identifier for this record; a newly constructed model uses its established unsaved sentinel value unless otherwise noted.</value>
        public long Id { get; set; }
        /// <summary>Gets or sets StoreId.</summary>
        /// <remarks>The identifier of the store that owns this record. Store data is isolated by this value.</remarks>
        /// <value>The identifier of the store that owns this record. Store data is isolated by this value.</value>
        public long StoreId { get; set; }
        /// <summary>Gets or sets Key.</summary>
        /// <remarks>The API credential value. Treat it as sensitive and do not expose it in logs or user-facing output.</remarks>
        /// <value>The API credential value. Treat it as sensitive and do not expose it in logs or user-facing output.</value>
        public string Key { get; set; }
    }
}
