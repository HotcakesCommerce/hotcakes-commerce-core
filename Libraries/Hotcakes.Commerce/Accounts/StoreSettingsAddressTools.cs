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
    /// <summary>Provides the address-validation settings façade owned by a <see cref="StoreSettings"/> instance.</summary>
    /// <remarks>Values belong to the current store. Setters update the parent façade's in-memory settings collection immediately; persistence to the database occurs when the store settings are saved by the caller.</remarks>
    [Serializable]
    public class StoreSettingsAddressTools
    {
        private readonly StoreSettings parent;

        /// <summary>Initializes a new address-tools façade for <paramref name="s"/>.</summary>
        /// <param name="s">The store settings façade that owns the settings.</param>
        public StoreSettingsAddressTools(StoreSettings s)
        {
            parent = s;
        }

        /// <summary>Gets or sets whether address validation is enabled for the store.</summary>
        /// <value>The value persisted under <c>UseAddressValidation</c>; an unset value defaults to <see langword="false"/>.</value>
        public bool UseAddressValidation
        {
            get { return parent.GetPropBoolWithDefault("UseAddressValidation", false); }
            set { parent.SetProp("UseAddressValidation", value); }
        }

        /// <summary>Gets or sets the address-validation provider identifier.</summary>
        /// <value>The value persisted under <c>AddressToolsID</c>.</value>
        public string AddressToolsID
        {
            get { return parent.GetProp("AddressToolsID"); }
            set { parent.SetProp("AddressToolsID", value); }
        }
    }
}
