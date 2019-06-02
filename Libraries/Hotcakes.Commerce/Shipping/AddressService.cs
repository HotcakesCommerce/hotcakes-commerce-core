#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
    public class AddressService
    {
        private readonly IAddressProvider _provider;
        private readonly StoreSettingsAddressTools _sett;

        public AddressService(Store store)
        {
            _sett = new StoreSettingsAddressTools(store.Settings);
            _provider = new USPSAddressProvider(_sett.AddressToolsID);
        }

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