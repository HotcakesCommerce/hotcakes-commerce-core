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
    /// <summary>Represents Store data and behavior within the Hotcakes Commerce account and store boundary.</summary>
    [Serializable]
    public class Store
    {
        private string _CustomUrl = string.Empty;


        /// <summary>Performs the Store operation.</summary>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public Store()
        {
            Id = 0;
            StoreGuid = Guid.NewGuid();
            StoreName = string.Empty;
            DateCreated = DateTime.UtcNow;
            CustomUrl = string.Empty;
            Settings = new StoreSettings(this);
        }

        /// <summary>Gets or sets Id.</summary>
        /// <remarks>The database identifier for this record; a newly constructed model uses its established unsaved sentinel value unless otherwise noted.</remarks>
        /// <value>The database identifier for this record; a newly constructed model uses its established unsaved sentinel value unless otherwise noted.</value>
        public long Id { get; set; }
        /// <summary>Gets or sets StoreGuid.</summary>
        /// <remarks>The stable globally unique identifier for the store.</remarks>
        /// <value>The stable globally unique identifier for the store.</value>
        public Guid StoreGuid { get; set; }
        /// <summary>Gets or sets StoreName.</summary>
        /// <remarks>The display name of the store.</remarks>
        /// <value>The display name of the store.</value>
        public string StoreName { get; set; }
        /// <summary>Gets or sets DateCreated.</summary>
        /// <remarks>The UTC date and time when the store was created.</remarks>
        /// <value>The UTC date and time when the store was created.</value>
        public DateTime DateCreated { get; set; }

        /// <summary>Gets or sets the  value for this instance.</summary>
        /// <value>The current  value. The meaning and persistence scope are defined by the containing type.</value>
        public string CustomUrl
        {
            get { return _CustomUrl; }
            set { _CustomUrl = value.Trim().ToLowerInvariant(); }
        }

        /// <summary>Gets or sets Settings.</summary>
        /// <remarks>The store-scoped settings façade. Changes are staged in this object until the owning repository persists the store.</remarks>
        /// <value>The store-scoped settings façade. Changes are staged in this object until the owning repository persists the store.</value>
        public StoreSettings Settings { get; set; }

        /// <summary>Performs the RootUrl operation.</summary>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public string RootUrl()
        {
            return Factory.CreateHccUrlResolver().GetStoreRootUrl(this);
        }

        /// <summary>Performs the RootUrlSecure operation.</summary>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public string RootUrlSecure()
        {
            return Factory.CreateHccUrlResolver().GetStoreRootUrl(this, true);
        }
    }
}
