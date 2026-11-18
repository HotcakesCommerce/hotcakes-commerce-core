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
using System.Globalization;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Shipping;

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>
    /// Validates and, when possible, normalizes customer addresses using the store's address settings.
    /// </summary>
    /// <remarks>
    /// The current implementation creates a <see cref="USPSAddressProvider"/> with the configured
    /// address-tools identifier. Validation is attempted only when address validation is enabled and
    /// the address is identified as a United States address.
    /// </remarks>
    public class AddressService
    {
        private readonly IAddressProvider _provider;
        private readonly StoreSettingsAddressTools _sett;

        /// <summary>
        /// Initializes an address service for a store.
        /// </summary>
        /// <param name="store">The store whose address settings select the address-tools provider.</param>
        public AddressService(Store store)
        {
            _sett = new StoreSettingsAddressTools(store.Settings);
            _provider = new USPSAddressProvider(_sett.AddressToolsID);
        }

        /// <summary>
        /// Validates an address and optionally returns a provider-normalized copy.
        /// </summary>
        /// <param name="address">The address to validate.</param>
        /// <param name="message">The provider validation message, or <see langword="null"/> when validation is not attempted.</param>
        /// <param name="normalized">A normalized copy when the provider changes a relevant value; otherwise <see langword="null"/>.</param>
        /// <param name="mergeAddressLines">Whether to merge line 2 before line 1 in the normalized copy.</param>
        /// <returns><see langword="true"/> when the provider validates the address; otherwise <see langword="false"/>.</returns>
        /// <remarks>
        /// Provider exceptions are logged and result in a failed validation. A normalized copy is
        /// discarded when normalization makes no relevant change. Non-United States addresses and
        /// addresses rejected by the store settings return <see langword="false"/> without provider validation.
        /// </remarks>
        public bool Validate(Address address, out string message, out Address normalized, bool mergeAddressLines = false)
        {
            if (_sett.UseAddressValidation && address.CountryData != null &&
                address.CountryData.Bvin == Country.UnitedStatesCountryBvin)
            {
                var isValid = false;
                var isNormalized = false;
                normalized = new Address();
                message = string.Empty;

                try
                {
                    isValid = _provider.ValidateAddress(address, out message);
                    address.CopyTo(normalized);

                    string message2;
                    isNormalized = _provider.NormalizeAddress(normalized, out message2);
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent(ex);
                }

                if (normalized != null && mergeAddressLines)
                {
                    normalized.Line1 = string.Join(", ", normalized.Line2, normalized.Line1);
                    normalized.Line2 = string.Empty;
                }

                if (isNormalized &&
                    IsEqual(normalized.Line1, address.Line1) &&
                    IsEqual(normalized.Line2, address.Line2) &&
                    IsEqual(normalized.City, address.City) &&
                    IsEqual(normalized.RegionBvin, address.RegionBvin) &&
                    IsEqual(normalized.PostalCode, address.PostalCode))
                {
                    isNormalized = false;
                }

                if (!isNormalized)
                {
                    normalized = null;
                }

                return isValid;
            }
            message = null;
            normalized = null;
            return false;
        }

        private bool IsEqual(string line1, string line2)
        {
            if (line1 == null && line2 == null) return true;
            if (line1 == null || line2 == null) return false;
            return string.Compare(line1.Trim(), line2.Trim(), true, CultureInfo.InvariantCulture) == 0;
        }
    }
}
